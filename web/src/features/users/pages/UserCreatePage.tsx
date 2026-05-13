import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { UserForm } from '../components/UserForm';
import { useUIStore } from '@/stores/uiStore';
import { createUserApi } from '@/api/users';

interface UserFormData {
  username: string;
  realName: string;
  password?: string;
  role: number;
}

export const UserCreatePage: React.FC = () => {
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: createUserApi,
    onSuccess: () => {
      addToast({ message: '创建成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['all-users'] });
      navigate('/users');
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '创建失败', variant: 'error' });
    },
  });

  const handleSubmit = (data: UserFormData) => {
    mutation.mutate(data);
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-text-primary">新建用户</h1>
        <p className="text-text-muted mt-1">填写用户信息以创建新的用户</p>
      </div>

      <div className="bg-surface rounded-lg border border-border p-6">
        <UserForm onSubmit={handleSubmit} isLoading={mutation.isPending} />
      </div>
    </div>
  );
};