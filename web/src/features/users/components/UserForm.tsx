import React from 'react';
import { useForm, Controller } from 'react-hook-form';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/Select';
import { FormField } from '@/components/ui/FormField';
import type { User } from '@/types/user';

interface UserFormData {
  username: string;
  realName: string;
  password?: string;
  role: number;
}

interface UserFormProps {
  initialData?: Partial<User>;
  onSubmit: (data: UserFormData) => void;
  isLoading?: boolean;
  isEdit?: boolean;
}

export const UserForm: React.FC<UserFormProps> = ({
  initialData,
  onSubmit,
  isLoading,
  isEdit = false,
}) => {
  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<UserFormData>({
    defaultValues: {
      username: initialData?.username || '',
      realName: initialData?.realName || '',
      password: '',
      role: initialData?.role ?? 0,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <FormField label="用户名" required error={errors.username?.message}>
        <Input
          {...register('username', {
            required: '请输入用户名',
            minLength: { value: 2, message: '用户名至少2个字符' },
          })}
          placeholder="请输入用户名"
          disabled={isEdit}
        />
      </FormField>

      <FormField label="真实姓名" required error={errors.realName?.message}>
        <Input
          {...register('realName', { required: '请输入真实姓名' })}
          placeholder="请输入真实姓名"
        />
      </FormField>

      <FormField
        label={isEdit ? '新密码（留空则不修改）' : '密码'}
        required={!isEdit}
        error={errors.password?.message}
      >
        <Input
          type="password"
          {...register('password', {
            required: isEdit ? false : '请输入密码',
            minLength: isEdit ? undefined : { value: 6, message: '密码至少6个字符' },
          })}
          placeholder={isEdit ? '留空则不修改密码' : '请输入密码'}
        />
      </FormField>

      <FormField label="角色" required>
        <Controller
          name="role"
          control={control}
          rules={{ required: '请选择角色' }}
          render={({ field }) => (
            <Select value={field.value.toString()} onValueChange={(v) => field.onChange(parseInt(v))}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="0">用户</SelectItem>
                <SelectItem value="1">管理员</SelectItem>
              </SelectContent>
            </Select>
          )}
        />
      </FormField>

      <div className="flex justify-end gap-4">
        <Button type="button" variant="outline" onClick={() => window.history.back()}>
          取消
        </Button>
        <Button type="submit" loading={isLoading}>
          提交
        </Button>
      </div>
    </form>
  );
};