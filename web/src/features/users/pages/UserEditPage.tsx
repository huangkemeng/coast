import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { UserForm } from '../components/UserForm';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { ErrorState } from '@/components/common/ErrorState';
import { useUIStore } from '@/stores/uiStore';
import { getUserByIdApi, updateUserApi } from '@/api/users';
import type { UpdateUserRequest } from '@/types/user';

export const UserEditPage: React.FC = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToast } = useUIStore();
  const queryClient = useQueryClient();

  const {
    data: user,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['user', id],
    queryFn: () => getUserByIdApi(Number(id)),
    enabled: !!id,
  });

  const mutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateUserRequest }) =>
      updateUserApi(id, data),
    onSuccess: () => {
      addToast({ message: '更新成功', variant: 'success' });
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['all-users'] });
      navigate('/users');
    },
    onError: (error: Error) => {
      addToast({ message: error.message || '更新失败', variant: 'error' });
    },
  });

  if (isLoading) return <LoadingOverlay fullScreen text="加载中..." />;
  if (error) return <ErrorState message="加载失败" onRetry={refetch} />;
  if (!user) return <ErrorState message="用户不存在" />;

  const handleSubmit = (data: { password?: string; realName: string; role: number; phone?: string; email?: string }) => {
    mutation.mutate({
      id: Number(id),
      data: {
        username: user.username,
        realName: data.realName,
        password: data.password && data.password.length > 0 ? data.password : undefined,
        role: data.role,
        phone: data.phone,
        email: data.email,
      },
    });
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">编辑用户</h1>
        <p className="text-text-muted mt-1">修改用户信息</p>
      </div>

      <div className="bg-surface rounded-lg border border-border p-6">
        <UserForm
          initialData={user}
          onSubmit={handleSubmit}
          isLoading={mutation.isPending}
          isEdit
        />
      </div>
    </div>
  );
};