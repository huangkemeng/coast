import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Pagination } from '@/components/ui/Pagination';
import { RobotTable } from '../components/RobotTable';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { EmptyState } from '@/components/common/EmptyState';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useUIStore } from '@/stores/uiStore';
import { getRobotsApi, GetRobotsParams } from '@/api/robots';
import { useDeleteRobot, useTestRobot } from '@/features/robots/hooks';
import { Plus, Search } from 'lucide-react';

export const RobotsListPage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const deleteRobot = useDeleteRobot();
  const testRobot = useTestRobot();

  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize] = useState(10);
  const [keyword, setKeyword] = useState('');
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const params: GetRobotsParams = {
    pageIndex,
    pageSize,
    keyword: keyword || undefined,
    sortBy: 'createdAt',
    sortOrder: 'desc',
  };

  const { data, isLoading } = useQuery({
    queryKey: ['robots', params],
    queryFn: () => getRobotsApi(params),
  });

  const handleDelete = async () => {
    if (!deleteId) return;
    try {
      await deleteRobot.mutateAsync(deleteId);
      addToast({ message: '删除成功', variant: 'success' });
      setDeleteId(null);
    } catch {
      addToast({ message: '删除失败', variant: 'error' });
    }
  };

  const handleTest = async (id: number) => {
    try {
      const result = await testRobot.mutateAsync(id);
      if (result.success) {
        addToast({ message: '测试消息发送成功', variant: 'success' });
      } else {
        addToast({ message: result.message || '测试失败', variant: 'error' });
      }
    } catch {
      addToast({ message: '测试失败', variant: 'error' });
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-text-primary">机器人配置</h1>
        <Button onClick={() => navigate('/robots/new')}>
          <Plus className="h-4 w-4 mr-2" />
          新建机器人
        </Button>
      </div>

      <div className="flex items-center gap-4">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-text-muted" />
          <Input
            placeholder="搜索机器人名称..."
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
            title="暂无机器人"
            description="配置您的企业微信机器人来接收通知"
            action={{ label: '新建机器人', onClick: () => navigate('/robots/new') }}
          />
        ) : (
          <>
            <RobotTable
              data={data.items}
              onDelete={(id) => setDeleteId(id)}
              onTest={handleTest}
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
        description="删除后无法恢复，确定要删除这个机器人吗？"
        confirmText="删除"
        variant="destructive"
        onConfirm={handleDelete}
        loading={deleteRobot.isPending}
      />
    </div>
  );
};