import { useQuery } from '@tanstack/react-query';
import { getNotificationLogsApi, GetNotificationLogsParams } from '@/api/notifications';

export const useNotifications = (params: GetNotificationLogsParams) => {
  return useQuery({
    queryKey: ['notification-logs', params],
    queryFn: () => getNotificationLogsApi(params),
  });
};

export const useNotificationLog = (id: number) => {
  return useQuery({
    queryKey: ['notification-log', id],
    queryFn: () => import('@/api/notifications').then(api => api.getNotificationLogByIdApi(id)),
    enabled: !!id,
  });
};