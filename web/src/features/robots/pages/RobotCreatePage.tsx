import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { RobotForm } from '../components/RobotForm';
import { useUIStore } from '@/stores/uiStore';
import { createRobotApi } from '@/api/robots';
import type { CreateRobotRequest } from '@/types/robot';

export const RobotCreatePage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: createRobotApi,
    onSuccess: () => {
      addToast({ message: '创建成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['robots'] });
      navigate('/robots');
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '创建失败', variant: 'error' });
    },
  });

  const handleSubmit = (data: CreateRobotRequest) => {
    mutation.mutate(data);
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-text-primary">新建机器人</h1>
        <p className="text-text-muted mt-1">填写机器人配置信息</p>
      </div>

      <div className="bg-surface rounded-lg border border-border p-6">
        <RobotForm onSubmit={handleSubmit} isLoading={mutation.isPending} />
      </div>
    </div>
  );
};