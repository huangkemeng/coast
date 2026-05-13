import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Button } from '@/components/ui/Button';
import { Pagination } from '@/components/ui/Pagination';
import { RequirementTable } from '../components/RequirementTable';
import { RequirementFilters } from '../components/RequirementFilters';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { EmptyState } from '@/components/common/EmptyState';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useUIStore } from '@/stores/uiStore';
import { usePermission } from '@/stores/permissionStore';
import { getRequirementsApi, GetRequirementsParams } from '@/api/requirements';
import { getProjectsApi } from '@/api/projects';
import { getAllUsersApi } from '@/api/users';
import { useDeleteRequirement } from '@/features/requirements/hooks';
import { Plus } from 'lucide-react';
import type { RequirementFilters as RequirementFiltersType } from '@/types/api';

export const RequirementsListPage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const { canCreateRequirement, canDeleteRequirement } = usePermission();
  const deleteRequirement = useDeleteRequirement();

  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize] = useState(10);
  const [filters, setFilters] = useState<RequirementFiltersType>({});
  const [sortBy, setSortBy] = useState('createdAt');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const params: GetRequirementsParams = {
    pageIndex,
    pageSize,
    sortBy,
    sortOrder,
    ...filters,
  };

  const { data, isLoading, refetch } = useQuery({
    queryKey: ['requirements', params],
    queryFn: () => getRequirementsApi(params),
  });

  const { data: projects } = useQuery({
    queryKey: ['all-projects'],
    queryFn: () => getProjectsApi({ pageIndex: 1, pageSize: 100 }),
  });

  const { data: users } = useQuery({
    queryKey: ['all-users'],
    queryFn: getAllUsersApi,
  });

  const handleFilterChange = (newFilters: RequirementFiltersType) => {
    setFilters(newFilters);
    setPageIndex(1);
  };

  const handleSort = (column: string) => {
    if (sortBy === column) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(column);
      setSortOrder('desc');
    }
  };

  const handleDelete = async () => {
    if (!deleteId) return;
    try {
      await deleteRequirement.mutateAsync(deleteId);
      addToast({ message: '删除成功', variant: 'success' });
      setDeleteId(null);
    } catch {
      addToast({ message: '删除失败', variant: 'error' });
    }
  };

  const handleStatusChange = (id: number, status: number) => {
    navigate(`/requirements/${id}?action=changeStatus&status=${status}`);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-text-primary">需求管理</h1>
        {canCreateRequirement() && (
          <Button onClick={() => navigate('/requirements/new')}>
            <Plus className="h-4 w-4 mr-2" />
            新建需求
          </Button>
        )}
      </div>

      <RequirementFilters
        onFilterChange={handleFilterChange}
        projectOptions={projects?.items.map((p) => ({ id: p.id, name: p.name })) || []}
        userOptions={users?.map((u) => ({ id: u.id, name: u.realName })) || []}
      />

      <div className="bg-surface rounded-lg border border-border">
        {isLoading ? (
          <LoadingOverlay text="加载中..." />
        ) : !data?.items.length ? (
          <EmptyState
            title="暂无需求"
            description="创建您的第一个需求来开始使用系统"
            action={
              canCreateRequirement()
                ? { label: '新建需求', onClick: () => navigate('/requirements/new') }
                : undefined
            }
          />
        ) : (
          <>
            <RequirementTable
              data={data.items}
              onDelete={canDeleteRequirement() ? (id) => setDeleteId(id) : undefined}
              onStatusChange={handleStatusChange}
            />
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

      <ConfirmDialog
        open={!!deleteId}
        onOpenChange={() => setDeleteId(null)}
        title="确认删除"
        description="删除后无法恢复，确定要删除这条需求吗？"
        confirmText="删除"
        variant="destructive"
        onConfirm={handleDelete}
        loading={deleteRequirement.isPending}
      />
    </div>
  );
};