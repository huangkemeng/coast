import React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { RequirementForm } from '../components/RequirementForm';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { ErrorState } from '@/components/common/ErrorState';
import { useUIStore } from '@/stores/uiStore';
import { getRequirementByIdApi, updateRequirementApi } from '@/api/requirements';
import type { CreateRequirementRequest, UpdateRequirementRequest } from '@/types/requirement';

export const RequirementEditPage: React.FC = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const queryClient = useQueryClient();

  const {
    data: requirement,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['requirement', id],
    queryFn: () => getRequirementByIdApi(Number(id)),
    enabled: !!id,
  });

  const mutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateRequirementRequest }) =>
      updateRequirementApi(id, data),
    onSuccess: () => {
      addToast({ message: '更新成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['requirements'] });
      navigate(`/requirements/${id}`);
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '更新失败', variant: 'error' });
    },
  });

  if (isLoading) return <LoadingOverlay fullScreen text="加载中..." />;
  if (error) return <ErrorState message="加载失败" onRetry={refetch} />;
  if (!requirement) return <ErrorState message="需求不存在" />;

  const handleSubmit = (data: CreateRequirementRequest) => {
    mutation.mutate({
      id: Number(id),
      data: {
        ...data,
        version: requirement.version || undefined,
      },
    });
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-text-primary">编辑需求</h1>
        <p className="text-text-muted mt-1">修改需求信息</p>
      </div>

      <div className="bg-surface rounded-lg border border-border p-6">
        <RequirementForm
          initialData={requirement}
          onSubmit={handleSubmit}
          isLoading={mutation.isPending}
        />
      </div>
    </div>
  );
};