import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Button } from '@/components/ui/Button';
import { Pagination } from '@/components/ui/Pagination';
import { RobotTable } from '../components/RobotTable';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { EmptyState } from '@/components/common/EmptyState';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useUIStore } from '@/stores/uiStore';
import { getRobotsApi } from '@/api/robots';
import { useDeleteRobot, useTestRobot } from '@/features/robots/hooks';
import { Plus } from 'lucide-react';

export const RobotsListPage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const deleteRobot = useDeleteRobot();
  const testRobot = useTestRobot();

  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize] = useState(10);
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['robots', pageIndex, pageSize],
    queryFn: () => getRobotsApi({ pageIndex, pageSize, sortBy: 'createdAt', sortOrder: 'desc' }),
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

  const totalPages = data ? Math.ceil(data.totalCount / pageSize) : 0;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">机器人配置</h1>
        <Button onClick={() => navigate('/robots/new')}>
          <Plus className="h-4 w-4 mr-2" />
          新建机器人
        </Button>
      </div>

      <div className="bg-surface rounded-lg border border-border">
        {isLoading ? (
          <LoadingOverlay text="加载中..." />
        ) : !data?.items.length ? (
          <EmptyState
            title="暂无机器人"
            description="创建您的第一个机器人来开始使用通知功能"
          />
        ) : (
          <>
            <RobotTable
              data={data.items}
              isLoading={isLoading}
              onDelete={(id) => setDeleteId(id)}
              onTest={handleTest}
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
        description="删除后无法恢复，确定要删除该机器人吗？"
        onConfirm={handleDelete}
        confirmText="删除"
        variant="destructive"
      />
    </div>
  );
};