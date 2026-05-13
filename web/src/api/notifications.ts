import { apiClient } from './client';
import type { NotificationLog, NotificationLogListItem } from '@/types/notification';
import type { PageRequest, PageResponse } from '@/types/api';

export interface GetNotificationLogsParams extends PageRequest {
  requirementId?: number | null;
  robotId?: number | null;
  status?: number | null;
  startTime?: string | null;
  endTime?: string | null;
}

export const getNotificationLogsApi = async (params: GetNotificationLogsParams): Promise<PageResponse<NotificationLogListItem>> => {
  const response = await apiClient.get<PageResponse<NotificationLogListItem>>('/notifications', { params });
  return response.data;
};

export const getNotificationLogByIdApi = async (id: number): Promise<NotificationLog> => {
  const response = await apiClient.get<NotificationLog>(`/notifications/${id}`);
  return response.data;
};