import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Button } from '@/components/ui/Button';
import { Pagination } from '@/components/ui/Pagination';
import { UserTable } from '../components/UserTable';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { EmptyState } from '@/components/common/EmptyState';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useUIStore } from '@/stores/uiStore';
import { getUsersApi, deleteUserApi } from '@/api/users';
import { Plus } from 'lucide-react';

export const UsersListPage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const queryClient = useQueryClient();

  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize] = useState(10);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['users', pageIndex, pageSize],
    queryFn: () => getUsersApi({ pageIndex, pageSize, sortBy: 'createdAt', sortOrder: 'desc' }),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteUserApi(id),
    onSuccess: () => {
      addToast({ message: '删除成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['users'] });
      setDeleteId(null);
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '删除失败', variant: 'error' });
    },
  });

  const handleDelete = () => {
    if (deleteId !== null) {
      deleteMutation.mutate(deleteId);
    }
  };

  const totalPages = data ? Math.ceil(data.totalCount / pageSize) : 0;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">用户管理</h1>
        <Button onClick={() => navigate('/users/new')}>
          <Plus className="h-4 w-4 mr-2" />
          新建用户
        </Button>
      </div>

      <div className="bg-surface rounded-lg border border-border">
        {isLoading ? (
          <LoadingOverlay text="加载中..." />
        ) : !data?.items.length ? (
          <EmptyState
            title="暂无用户"
            description="创建您的第一个用户来开始使用系统"
          />
        ) : (
          <>
            <UserTable
              data={data.items}
              isLoading={isLoading}
              onDelete={(id) => setDeleteId(id)}
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
        description="删除后无法恢复，确定要删除该用户吗？"
        onConfirm={handleDelete}
        confirmText="删除"
        variant="destructive"
      />
    </div>
  );
};