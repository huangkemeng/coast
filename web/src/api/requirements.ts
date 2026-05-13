import { apiClient } from './client';
import type {
  Requirement,
  RequirementListItem,
  RequirementTimeline,
  CreateRequirementRequest,
  UpdateRequirementRequest,
  ChangeStatusRequest,
} from '@/types/requirement';
import type { PageRequest, PageResponse } from '@/types/api';

export interface GetRequirementsParams extends PageRequest {
  keyword?: string;
  projectId?: number | null;
  followerId?: number | null;
  status?: number | null;
  priority?: number | null;
  isConfirmed?: boolean | null;
  dateFrom?: string | null;
  dateTo?: string | null;
}

export const getRequirementsApi = async (
  params: GetRequirementsParams
): Promise<PageResponse<RequirementListItem>> => {
  const response = await apiClient.get<PageResponse<RequirementListItem>>('/requirements', { params });
  return response.data;
};

export const getRequirementByIdApi = async (id: number): Promise<Requirement> => {
  const response = await apiClient.get<Requirement>(`/requirements/${id}`);
  return response.data;
};

export const createRequirementApi = async (
  data: CreateRequirementRequest
): Promise<Requirement> => {
  const response = await apiClient.post<Requirement>('/requirements', data);
  return response.data;
};

export const updateRequirementApi = async (
  id: number,
  data: UpdateRequirementRequest
): Promise<Requirement> => {
  const response = await apiClient.put<Requirement>(`/requirements/${id}`, data);
  return response.data;
};

export const deleteRequirementApi = async (id: number): Promise<void> => {
  await apiClient.delete(`/requirements/${id}`);
};

export const changeRequirementStatusApi = async (
  id: number,
  data: ChangeStatusRequest
): Promise<Requirement> => {
  const response = await apiClient.post<Requirement>(`/requirements/${id}/status`, data);
  return response.data;
};

export const getRequirementTimelineApi = async (
  id: number
): Promise<RequirementTimeline[]> => {
  const response = await apiClient.get<RequirementTimeline[]>(`/requirements/${id}/timeline`);
  return response.data;
};