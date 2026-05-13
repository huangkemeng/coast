import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Pagination } from '@/components/ui/Pagination';
import { NotificationTable } from '../components/NotificationTable';
import { NotificationFilters } from '../components/NotificationFilters';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { EmptyState } from '@/components/common/EmptyState';
import { getNotificationLogsApi, GetNotificationLogsParams } from '@/api/notifications';
import { getRobotsApi } from '@/api/robots';
import type { NotificationFilters as NotificationFiltersType } from '@/types/api';

export const NotificationsListPage: React.FC = () => {
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize] = useState(10);
  const [filters, setFilters] = useState<NotificationFiltersType>({});

  const params: GetNotificationLogsParams = {
    pageIndex,
    pageSize,
    sortBy: 'sentAt',
    sortOrder: 'desc',
    ...filters,
  };

  const { data, isLoading } = useQuery({
    queryKey: ['notification-logs', params],
    queryFn: () => getNotificationLogsApi(params),
  });

  const { data: robots } = useQuery({
    queryKey: ['robots', { pageIndex: 1, pageSize: 100 }],
    queryFn: () => getRobotsApi({ pageIndex: 1, pageSize: 100 }),
  });

  const handleFilterChange = (newFilters: NotificationFiltersType) => {
    setFilters(newFilters);
    setPageIndex(1);
  };

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-text-primary">通知日志</h1>

      <NotificationFilters
        onFilterChange={handleFilterChange}
        robotOptions={robots?.items.map((r) => ({ id: r.id, name: r.name })) || []}
      />

      <div className="bg-surface rounded-lg border border-border">
        {isLoading ? (
          <LoadingOverlay text="加载中..." />
        ) : !data?.items.length ? (
          <EmptyState
            title="暂无通知记录"
            description="当有需求状态变更时，将自动发送通知"
          />
        ) : (
          <>
            <NotificationTable data={data.items} />
            <div className="p-4 border-t border-border">
              <Pagination
                pageIndex={pageIndex}
                pageSize={pageSize}
                totalCount={data.totalCount}
                totalPages={data.totalPages}
                onPageChange={setPageIndex}
              />
            </div>
          </>
        )}
      </div>
    </div>
  );
};