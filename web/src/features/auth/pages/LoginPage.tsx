import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { useAuthStore } from '@/stores/authStore';
import { useUIStore } from '@/stores/uiStore';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { FormField } from '@/components/ui/FormField';

interface LoginFormData {
  username: string;
  password: string;
}

export const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { login, isLoading } = useAuthStore();
  const { addToast } = useUIStore();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    defaultValues: {
      username: '',
      password: '',
    },
  });

  const onSubmit = async (data: LoginFormData) => {
    try {
      await login(data);
      addToast({ message: '登录成功', variant: 'success' });
      navigate('/dashboard');
    } catch (error) {
      addToast({
        message: error instanceof Error ? error.message : '登录失败',
        variant: 'error',
      });
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-4">
      <div className="w-full max-w-md p-8 bg-surface rounded-lg border border-border shadow-xl">
        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold text-text-primary">需求跟踪管理系统</h1>
          <p className="text-text-muted mt-2">请登录以继续</p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <FormField label="用户名" required error={errors.username?.message}>
            <Input
              {...register('username', {
                required: '请输入用户名',
              })}
              placeholder="请输入用户名"
              autoComplete="username"
            />
          </FormField>

          <FormField label="密码" required error={errors.password?.message}>
            <Input
              {...register('password', {
                required: '请输入密码',
              })}
              type="password"
              placeholder="请输入密码"
              autoComplete="current-password"
            />
          </FormField>

          <Button type="submit" className="w-full" loading={isLoading}>
            登录
          </Button>
        </form>

        <div className="mt-6 text-center text-sm text-text-muted">
          <p>演示账号: admin / admin123</p>
        </div>
      </div>
    </div>
  );
};