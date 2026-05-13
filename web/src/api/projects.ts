import { apiClient } from './client';
import type { Project, ProjectListItem, CreateProjectRequest, UpdateProjectRequest } from '@/types/project';
import type { PageRequest, PageResponse } from '@/types/api';

export interface GetProjectsParams extends PageRequest {}

export const getProjectsApi = async (params: GetProjectsParams): Promise<PageResponse<ProjectListItem>> => {
  const response = await apiClient.get<PageResponse<ProjectListItem>>('/projects', { params });
  return response.data;
};

export const getProjectByIdApi = async (id: number): Promise<Project> => {
  const response = await apiClient.get<Project>(`/projects/${id}`);
  return response.data;
};

export const createProjectApi = async (data: CreateProjectRequest): Promise<{ id: number }> => {
  const response = await apiClient.post<{ id: number }>('/projects', data);
  return response.data;
};

export const updateProjectApi = async (id: number, data: UpdateProjectRequest): Promise<Project> => {
  const response = await apiClient.put<Project>(`/projects/${id}`, data);
  return response.data;
};

export const deleteProjectApi = async (id: number): Promise<void> => {
  await apiClient.delete(`/projects/${id}`);
};