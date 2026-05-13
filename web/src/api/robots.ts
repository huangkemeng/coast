import { apiClient } from './client';
import type {
  Robot,
  RobotListItem,
  CreateRobotRequest,
  UpdateRobotRequest,
  TestRobotResponse,
} from '@/types/robot';
import type { PageRequest, PageResponse } from '@/types/api';

export interface GetRobotsParams extends PageRequest {
  keyword?: string;
}

export const getRobotsApi = async (
  params: GetRobotsParams
): Promise<PageResponse<RobotListItem>> => {
  const response = await apiClient.get<PageResponse<RobotListItem>>('/robots', { params });
  return response.data;
};

export const getRobotByIdApi = async (id: number): Promise<Robot> => {
  const response = await apiClient.get<Robot>(`/robots/${id}`);
  return response.data;
};

export const createRobotApi = async (
  data: CreateRobotRequest
): Promise<Robot> => {
  const response = await apiClient.post<Robot>('/robots', data);
  return response.data;
};

export const updateRobotApi = async (
  id: number,
  data: UpdateRobotRequest
): Promise<Robot> => {
  const response = await apiClient.put<Robot>(`/robots/${id}`, data);
  return response.data;
};

export const deleteRobotApi = async (id: number): Promise<void> => {
  await apiClient.delete(`/robots/${id}`);
};

export const testRobotApi = async (id: number): Promise<TestRobotResponse> => {
  const response = await apiClient.post<TestRobotResponse>(`/robots/${id}/test`);
  return response.data;
};