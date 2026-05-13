import { apiClient } from './client';
import type { User, UserListItem, CreateUserRequest, UpdateUserRequest } from '@/types/user';
import type { PageRequest, PageResponse } from '@/types/api';

export interface GetUsersParams extends PageRequest {}

export const getUsersApi = async (params: GetUsersParams): Promise<PageResponse<UserListItem>> => {
  const response = await apiClient.get<PageResponse<UserListItem>>('/users', { params });
  return response.data;
};

export const getUserByIdApi = async (id: number): Promise<User> => {
  const response = await apiClient.get<User>(`/users/${id}`);
  return response.data;
};

export const createUserApi = async (data: CreateUserRequest): Promise<User> => {
  const response = await apiClient.post<User>('/users', data);
  return response.data;
};

export const updateUserApi = async (id: number, data: UpdateUserRequest): Promise<User> => {
  const response = await apiClient.put<User>(`/users/${id}`, data);
  return response.data;
};

export const deleteUserApi = async (id: number): Promise<void> => {
  await apiClient.delete(`/users/${id}`);
};

export const getAllUsersApi = async (): Promise<UserListItem[]> => {
  const response = await apiClient.get<PageResponse<UserListItem>>('/users', { params: { pageIndex: 1, pageSize: 1000 } });
  return response.data.items;
};