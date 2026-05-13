import { apiClient } from './client';
import type { LoginRequest, LoginResponse, User } from '@/types/user';

export const loginApi = async (data: LoginRequest): Promise<LoginResponse> => {
  const response = await apiClient.post<LoginResponse>('/auth/login', data);
  return response.data;
};

export const getUserInfoApi = async (): Promise<User> => {
  const response = await apiClient.get<User>('/auth/userinfo');
  return response.data;
};