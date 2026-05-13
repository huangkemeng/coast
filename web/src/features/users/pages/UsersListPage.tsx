import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Pagination } from '@/components/ui/Pagination';
import { UserTable } from '../components/UserTable';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { EmptyState } from '@/components/common/EmptyState';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useUIStore } from '@/stores/uiStore';
import { getUsersApi, GetUsersParams } from '@/api/users';
import { useDeleteUser } from '@/features/users/hooks';
import { Plus, Search } from 'lucide-react';

export const UsersListPage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const deleteUser = useDeleteUser();

  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize] = useState(10);
  const [keyword, setKeyword] = useState('');
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const params: GetUsersParams = {
    pageIndex,
    pageSize,
    keyword: keyword || undefined,
    sortBy: 'createdAt',
    sortOrder: 'desc',
  };

  const { data, isLoading } = useQuery({
    queryKey: ['users', params],
    queryFn: () => getUsersApi(params),
  });

  const handleDelete = async () => {
    if (!deleteId) return;
    try {
      await deleteUser.mutateAsync(deleteId);
      addToast({ message: '删除成功', variant: 'success' });
      setDeleteId(null);
    } catch {
      addToast({ message: '删除失败', variant: 'error' });
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-text-primary">用户管理</h1>
        <Button onClick={() => navigate('/users/new')}>
          <Plus className="h-4 w-4 mr-2" />
          新建用户
        </Button>
      </div>

      <div className="flex items-center gap-4">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-text-muted" />
          <Input
            placeholder="搜索用户名或姓名..."
            value={keyword}
            onChange={(e) => {
              setKeyword(e.target.value);
              setPageIndex(1);
            }}
            className="pl-10"
          />
        </div>
      </div>

      <div className="bg-surface rounded-lg border border-border">
        {isLoading ? (
          <LoadingOverlay text="加载中..." />
        ) : !data?.items.length ? (
          <EmptyState
            title="暂无用户"
            description="创建您的第一个用户"
            action={{ label: '新建用户', onClick: () => navigate('/users/new') }}
          />
        ) : (
          <>
            <UserTable data={data.items} onDelete={(id) => setDeleteId(id)} />
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
        description="删除后无法恢复，确定要删除这个用户吗？"
        confirmText="删除"
        variant="destructive"
        onConfirm={handleDelete}
        loading={deleteUser.isPending}
      />
    </div>
  );
};