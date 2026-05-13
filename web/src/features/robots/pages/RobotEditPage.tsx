import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { RobotForm } from '../components/RobotForm';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { ErrorState } from '@/components/common/ErrorState';
import { useUIStore } from '@/stores/uiStore';
import { getRobotByIdApi, updateRobotApi } from '@/api/robots';
import type { CreateRobotRequest } from '@/types/robot';

export const RobotEditPage: React.FC = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const queryClient = useQueryClient();

  const {
    data: robot,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['robot', id],
    queryFn: () => getRobotByIdApi(Number(id)),
    enabled: !!id,
  });

  const mutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: CreateRobotRequest }) =>
      updateRobotApi(id, data),
    onSuccess: () => {
      addToast({ message: '更新成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['robots'] });
      navigate('/robots');
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '更新失败', variant: 'error' });
    },
  });

  if (isLoading) return <LoadingOverlay fullScreen text="加载中..." />;
  if (error) return <ErrorState message="加载失败" onRetry={refetch} />;
  if (!robot) return <ErrorState message="机器人不存在" />;

  const handleSubmit = (data: CreateRobotRequest) => {
    mutation.mutate({ id: Number(id), data });
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-text-primary">编辑机器人</h1>
        <p className="text-text-muted mt-1">修改机器人配置信息</p>
      </div>

      <div className="bg-surface rounded-lg border border-border p-6">
        <RobotForm
          initialData={robot}
          onSubmit={handleSubmit}
          isLoading={mutation.isPending}
        />
      </div>
    </div>
  );
};