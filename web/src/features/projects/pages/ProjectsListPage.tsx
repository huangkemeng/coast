import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Pagination } from '@/components/ui/Pagination';
import { ProjectTable } from '../components/ProjectTable';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { EmptyState } from '@/components/common/EmptyState';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useUIStore } from '@/stores/uiStore';
import { getProjectsApi, GetProjectsParams } from '@/api/projects';
import { useDeleteProject } from '@/features/projects/hooks';
import { Plus, Search } from 'lucide-react';

export const ProjectsListPage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const deleteProject = useDeleteProject();

  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize] = useState(10);
  const [keyword, setKeyword] = useState('');
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const params: GetProjectsParams = {
    pageIndex,
    pageSize,
    keyword: keyword || undefined,
    sortBy: 'createdAt',
    sortOrder: 'desc',
  };

  const { data, isLoading, refetch } = useQuery({
    queryKey: ['projects', params],
    queryFn: () => getProjectsApi(params),
  });

  const handleDelete = async () => {
    if (!deleteId) return;
    try {
      await deleteProject.mutateAsync(deleteId);
      addToast({ message: '删除成功', variant: 'success' });
      setDeleteId(null);
    } catch {
      addToast({ message: '删除失败', variant: 'error' });
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-text-primary">项目管理</h1>
        <Button onClick={() => navigate('/projects/new')}>
          <Plus className="h-4 w-4 mr-2" />
          新建项目
        </Button>
      </div>

      <div className="flex items-center gap-4">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-text-muted" />
          <Input
            placeholder="搜索项目名称..."
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
            title="暂无项目"
            description="创建您的第一个项目来开始使用系统"
            action={{ label: '新建项目', onClick: () => navigate('/projects/new') }}
          />
        ) : (
          <>
            <ProjectTable data={data.items} onDelete={(id) => setDeleteId(id)} />
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
        description="删除后无法恢复，确定要删除这个项目吗？"
        confirmText="删除"
        variant="destructive"
        onConfirm={handleDelete}
        loading={deleteProject.isPending}
      />
    </div>
  );
};