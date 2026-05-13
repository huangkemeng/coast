import React from 'react';
import { useForm, Controller } from 'react-hook-form';
import { useQuery } from '@tanstack/react-query';
import { Input } from '@/components/ui/Input';
import { Textarea } from '@/components/ui/Textarea';
import { Button } from '@/components/ui/Button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/Select';
import { Checkbox } from '@/components/ui/Checkbox';
import { FormField } from '@/components/ui/FormField';
import { getAllUsersApi } from '@/api/users';
import { getProjectsApi } from '@/api/projects';
import type { CreateRequirementRequest, Requirement } from '@/types/requirement';

interface RequirementFormProps {
  initialData?: Partial<Requirement>;
  onSubmit: (data: CreateRequirementRequest) => void;
  isLoading?: boolean;
}

export const RequirementForm: React.FC<RequirementFormProps> = ({
  initialData,
  onSubmit,
  isLoading,
}) => {
  const { data: users } = useQuery({
    queryKey: ['all-users'],
    queryFn: getAllUsersApi,
  });

  const { data: projects } = useQuery({
    queryKey: ['all-projects'],
    queryFn: () => getProjectsApi({ pageIndex: 1, pageSize: 100 }),
  });

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<CreateRequirementRequest>({
    defaultValues: {
      name: initialData?.name || '',
      projectId: initialData?.projectId ?? undefined,
      followerId: initialData?.followerId ?? undefined,
      priority: initialData?.priority ?? 0,
      isConfirmed: initialData?.isConfirmed ?? false,
      price: initialData?.price ?? undefined,
      deadline: initialData?.deadline?.split('T')[0] || undefined,
      docUrl: initialData?.docUrl || undefined,
      content: initialData?.content || undefined,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <FormField label="需求名称" required error={errors.name?.message}>
        <Input
          {...register('name', { required: '请输入需求名称' })}
          placeholder="请输入需求名称"
        />
      </FormField>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField label="所属项目">
          <Controller
            name="projectId"
            control={control}
            render={({ field }) => (
              <Select
                value={field.value?.toString() || ''}
                onValueChange={(value) => field.onChange(value ? parseInt(value) : undefined)}
              >
                <SelectTrigger>
                  <SelectValue placeholder="请选择项目" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="">无</SelectItem>
                  {projects?.items.map((project) => (
                    <SelectItem key={project.id} value={project.id.toString()}>
                      {project.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
        </FormField>

        <FormField label="跟进人">
          <Controller
            name="followerId"
            control={control}
            render={({ field }) => (
              <Select
                value={field.value?.toString() || ''}
                onValueChange={(value) => field.onChange(value ? parseInt(value) : undefined)}
              >
                <SelectTrigger>
                  <SelectValue placeholder="请选择跟进人" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="">无</SelectItem>
                  {users?.map((user) => (
                    <SelectItem key={user.id} value={user.id.toString()}>
                      {user.realName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
        </FormField>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField label="优先级">
          <Controller
            name="priority"
            control={control}
            render={({ field }) => (
              <Select
                value={field.value?.toString() || '0'}
                onValueChange={(value) => field.onChange(parseInt(value))}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="0">普通</SelectItem>
                  <SelectItem value="1">紧急</SelectItem>
                  <SelectItem value="2">非常重要</SelectItem>
                </SelectContent>
              </Select>
            )}
          />
        </FormField>

        <FormField label="截止日期">
          <Input type="date" {...register('deadline')} />
        </FormField>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField label="报价">
          <Input
            type="number"
            step="0.01"
            {...register('price', { valueAsNumber: true })}
            placeholder="请输入报价"
          />
        </FormField>

        <FormField label="文档链接">
          <Input
            {...register('docUrl')}
            placeholder="请输入文档链接"
          />
        </FormField>
      </div>

      <FormField label="需求已确认">
        <Controller
          name="isConfirmed"
          control={control}
          render={({ field }) => (
            <Checkbox
              checked={field.value}
              onChange={field.onChange}
              label="已确认"
            />
          )}
        />
      </FormField>

      <FormField label="需求描述">
        <Textarea
          {...register('content')}
          placeholder="请输入需求描述"
          rows={4}
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