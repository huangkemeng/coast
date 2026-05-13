import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ProjectForm } from '../components/ProjectForm';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { ErrorState } from '@/components/common/ErrorState';
import { useUIStore } from '@/stores/uiStore';
import { getProjectByIdApi, updateProjectApi } from '@/api/projects';
import type { CreateProjectRequest } from '@/types/project';

export const ProjectEditPage: React.FC = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const queryClient = useQueryClient();

  const {
    data: project,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['project', id],
    queryFn: () => getProjectByIdApi(Number(id)),
    enabled: !!id,
  });

  const mutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: CreateProjectRequest }) =>
      updateProjectApi(id, data),
    onSuccess: () => {
      addToast({ message: '更新成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      navigate('/projects');
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '更新失败', variant: 'error' });
    },
  });

  if (isLoading) return <LoadingOverlay fullScreen text="加载中..." />;
  if (error) return <ErrorState message="加载失败" onRetry={refetch} />;
  if (!project) return <ErrorState message="项目不存在" />;

  const handleSubmit = (data: CreateProjectRequest) => {
    mutation.mutate({ id: Number(id), data });
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-text-primary">编辑项目</h1>
        <p className="text-text-muted mt-1">修改项目信息</p>
      </div>

      <div className="bg-surface rounded-lg border border-border p-6">
        <ProjectForm
          initialData={project}
          onSubmit={handleSubmit}
          isLoading={mutation.isPending}
        />
      </div>
    </div>
  );
};