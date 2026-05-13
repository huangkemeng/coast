import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Button } from '@/components/ui/Button';
import { Pagination } from '@/components/ui/Pagination';
import { RequirementTable } from '../components/RequirementTable';
import { RequirementFilters, RequirementFiltersState } from '../components/RequirementFilters';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { EmptyState } from '@/components/common/EmptyState';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useUIStore } from '@/stores/uiStore';
import { usePermission } from '@/stores/permissionStore';
import { getRequirementsApi, GetRequirementsParams } from '@/api/requirements';
import { getProjectsApi } from '@/api/projects';
import { getAllUsersApi } from '@/api/users';
import { deleteRequirementApi, changeRequirementStatusApi } from '@/api/requirements';
import type { RequirementStatus } from '@/types/requirement';
import { Plus } from 'lucide-react';

export const RequirementsListPage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const { canCreateRequirement, canDeleteRequirement } = usePermission();
  const queryClient = useQueryClient();

  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize] = useState(10);
  const [filters, setFilters] = useState<RequirementFiltersState>({});
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const params: GetRequirementsParams = {
    pageIndex,
    pageSize,
    sortBy: 'createdAt',
    sortOrder: 'desc',
    ...filters,
  };

  const { data, isLoading } = useQuery({
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

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteRequirementApi(id),
    onSuccess: () => {
      addToast({ message: '删除成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['requirements'] });
      setDeleteId(null);
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '删除失败', variant: 'error' });
    },
  });

  const statusMutation = useMutation({
    mutationFn: ({ id, status }: { id: number; status: RequirementStatus }) =>
      changeRequirementStatusApi(id, { status }),
    onSuccess: () => {
      addToast({ message: '状态变更成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['requirements'] });
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '状态变更失败', variant: 'error' });
    },
  });

  const handleFilterChange = (newFilters: RequirementFiltersState) => {
    setFilters(newFilters);
    setPageIndex(1);
  };

  const handleDelete = () => {
    if (deleteId !== null) {
      deleteMutation.mutate(deleteId);
    }
  };

  const handleStatusChange = (id: number, status: RequirementStatus) => {
    statusMutation.mutate({ id, status });
  };

  const totalPages = data ? Math.ceil(data.totalCount / pageSize) : 0;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">需求管理</h1>
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
          />
        ) : (
          <>
            <RequirementTable
              data={data.items}
              isLoading={isLoading}
              onDelete={(id) => canDeleteRequirement() && setDeleteId(id)}
              onStatusChange={handleStatusChange}
            />
            <div className="p-4 border-t border-border">
              <Pagination
                pageIndex={pageIndex}
                pageSize={pageSize}
                totalCount={data.totalCount}
                totalPages={totalPages}
                onPageChange={setPageIndex}
              />
            </div>
          </>
        )}
      </div>

      <ConfirmDialog
        open={deleteId !== null}
        onOpenChange={() => setDeleteId(null)}
        title="确认删除"
        description="删除后无法恢复，确定要删除该需求吗？"
        onConfirm={handleDelete}
        confirmText="删除"
        variant="destructive"
      />
    </div>
  );
};