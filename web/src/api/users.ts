import { apiClient } from './client';
import type { User } from '@/types/user';
import type { PageRequest, PageResponse } from '@/types/api';

export interface GetUsersParams extends PageRequest {
  keyword?: string;
}

export const getUsersApi = async (
  params: GetUsersParams
): Promise<PageResponse<User>> => {
  const response = await apiClient.get<PageResponse<User>>('/users', { params });
  return response.data;
};

export const getUserByIdApi = async (id: number): Promise<User> => {
  const response = await apiClient.get<User>(`/users/${id}`);
  return response.data;
};

export const createUserApi = async (data: {
  username: string;
  realName: string;
  password?: string;
  role?: number;
}): Promise<User> => {
  const response = await apiClient.post<User>('/users', data);
  return response.data;
};

export const updateUserApi = async (
  id: number,
  data: {
    realName?: string;
    password?: string;
    role?: number;
  }
): Promise<User> => {
  const response = await apiClient.put<User>(`/users/${id}`, data);
  return response.data;
};

export const deleteUserApi = async (id: number): Promise<void> => {
  await apiClient.delete(`/users/${id}`);
};

export const getAllUsersApi = async (): Promise<User[]> => {
  const response = await apiClient.get<User[]>('/users/all');
  return response.data;
};