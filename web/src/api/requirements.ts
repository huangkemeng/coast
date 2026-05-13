import { apiClient } from './client';
import type { Requirement, RequirementListItem, RequirementTimeline, CreateRequirementRequest, UpdateRequirementRequest, ChangeRequirementStatusRequest } from '@/types/requirement';
import type { PageRequest, PageResponse } from '@/types/api';

export interface GetRequirementsParams extends PageRequest {
  status?: string;
  followerId?: number | null;
  projectId?: number | null;
  planStartDateFrom?: string | null;
  planStartDateTo?: string | null;
  planTestDateFrom?: string | null;
  planTestDateTo?: string | null;
}

export const getRequirementsApi = async (params: GetRequirementsParams): Promise<PageResponse<RequirementListItem>> => {
  const response = await apiClient.get<PageResponse<RequirementListItem>>('/requirements', { params });
  return response.data;
};

export const getRequirementByIdApi = async (id: number): Promise<Requirement> => {
  const response = await apiClient.get<Requirement>(`/requirements/${id}`);
  return response.data;
};

export const createRequirementApi = async (data: CreateRequirementRequest): Promise<{ id: number }> => {
  const response = await apiClient.post<{ id: number }>('/requirements', data);
  return response.data;
};

export const updateRequirementApi = async (id: number, data: UpdateRequirementRequest): Promise<{ id: number }> => {
  const response = await apiClient.put<{ id: number }>(`/requirements/${id}`, data);
  return response.data;
};

export const deleteRequirementApi = async (id: number): Promise<void> => {
  await apiClient.delete(`/requirements/${id}`);
};

export const changeRequirementStatusApi = async (id: number, data: ChangeRequirementStatusRequest): Promise<Requirement> => {
  const response = await apiClient.put<Requirement>(`/requirements/${id}/status`, data);
  return response.data;
};

export const getRequirementTimelineApi = async (id: number): Promise<RequirementTimeline[]> => {
  const response = await apiClient.get<RequirementTimeline[]>(`/requirements/${id}/timeline`);
  return response.data;
};