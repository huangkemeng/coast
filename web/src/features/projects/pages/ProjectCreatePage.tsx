import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { ProjectForm } from '../components/ProjectForm';
import { useUIStore } from '@/stores/uiStore';
import { createProjectApi } from '@/api/projects';
import type { CreateProjectRequest } from '@/types/project';

export const ProjectCreatePage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (data: CreateProjectRequest) => createProjectApi(data),
    onSuccess: () => {
      addToast({ message: '创建成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      navigate('/projects');
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '创建失败', variant: 'error' });
    },
  });

  const handleSubmit = (data: CreateProjectRequest) => {
    mutation.mutate(data);
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-text-primary">新建项目</h1>
        <p className="text-text-muted mt-1">填写项目信息以创建新的项目</p>
      </div>

      <div className="bg-surface rounded-lg border border-border p-6">
        <ProjectForm onSubmit={handleSubmit} isLoading={mutation.isPending} />
      </div>
    </div>
  );
};