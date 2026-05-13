import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { RequirementForm } from '../components/RequirementForm';
import { useUIStore } from '@/stores/uiStore';
import { createRequirementApi } from '@/api/requirements';
import type { CreateRequirementRequest } from '@/types/requirement';

export const RequirementCreatePage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (data: CreateRequirementRequest) => createRequirementApi(data),
    onSuccess: (response) => {
      addToast({ message: '创建成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['requirements'] });
      navigate(`/requirements/${response.id}`);
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '创建失败', variant: 'error' });
    },
  });

  const handleSubmit = (data: CreateRequirementRequest) => {
    mutation.mutate(data);
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">新建需求</h1>
        <p className="text-text-muted mt-1">填写需求信息以创建新的需求</p>
      </div>

      <div className="bg-surface rounded-lg border border-border p-6">
        <RequirementForm onSubmit={handleSubmit} isLoading={mutation.isPending} />
      </div>
    </div>
  );
};