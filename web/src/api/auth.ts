import { apiClient } from './client';
import type { LoginRequest, LoginResponse } from '@/types/user';

export const loginApi = async (data: LoginRequest): Promise<LoginResponse> => {
  const response = await apiClient.post<LoginResponse>('/user/login', data);
  return response.data;
};

export const getUserInfoApi = async (): Promise<unknown> => {
  const response = await apiClient.get('/user');
  return response.data;
};