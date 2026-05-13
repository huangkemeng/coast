import React from 'react';
import { useForm, Controller } from 'react-hook-form';
import { useQuery } from '@tanstack/react-query';
import { Input } from '@/components/ui/Input';
import { Textarea } from '@/components/ui/Textarea';
import { Button } from '@/components/ui/Button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/Select';
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
      requirementNo: initialData?.requirementNo || '',
      projectId: initialData?.projectId || 0,
      followerId: initialData?.followerId || 0,
      priority: initialData?.priority ?? 0,
      planStartDate: initialData?.planStartDate?.split('T')[0] || undefined,
      planTestDate: initialData?.planTestDate?.split('T')[0] || undefined,
      planLaunchDate: initialData?.planLaunchDate?.split('T')[0] || undefined,
      docUrl: initialData?.docUrl || undefined,
      price: initialData?.price ?? undefined,
      remark: initialData?.remark || undefined,
      robotId: initialData?.robotId ?? undefined,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField label="需求名称" required error={errors.name?.message}>
          <Input
            {...register('name', { required: '请输入需求名称' })}
            placeholder="请输入需求名称"
          />
        </FormField>

        <FormField label="需求编号" required error={errors.requirementNo?.message}>
          <Input
            {...register('requirementNo', { required: '请输入需求编号' })}
            placeholder="请输入需求编号"
          />
        </FormField>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField label="所属项目" required error={errors.projectId?.message}>
          <Controller
            name="projectId"
            control={control}
            rules={{ required: '请选择项目' }}
            render={({ field }) => (
              <Select
                value={field.value?.toString() || ''}
                onValueChange={(value) => field.onChange(parseInt(value))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="请选择项目" />
                </SelectTrigger>
                <SelectContent>
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

        <FormField label="跟进人" required error={errors.followerId?.message}>
          <Controller
            name="followerId"
            control={control}
            rules={{ required: '请选择跟进人' }}
            render={({ field }) => (
              <Select
                value={field.value?.toString() || ''}
                onValueChange={(value) => field.onChange(parseInt(value))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="请选择跟进人" />
                </SelectTrigger>
                <SelectContent>
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

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
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
                  <SelectItem value="0">低</SelectItem>
                  <SelectItem value="1">中</SelectItem>
                  <SelectItem value="2">高</SelectItem>
                </SelectContent>
              </Select>
            )}
          />
        </FormField>

        <FormField label="计划开始日期">
          <Input type="date" {...register('planStartDate')} />
        </FormField>

        <FormField label="计划测试日期">
          <Input type="date" {...register('planTestDate')} />
        </FormField>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <FormField label="计划上线日期">
          <Input type="date" {...register('planLaunchDate')} />
        </FormField>

        <FormField label="报价">
          <Input
            type="number"
            step="0.01"
            {...register('price', { valueAsNumber: true })}
            placeholder="请输入报价"
          />
        </FormField>
      </div>

      <FormField label="需求文档链接">
        <Input
          {...register('docUrl')}
          placeholder="请输入文档链接"
        />
      </FormField>

      <FormField label="备注">
        <Textarea
          {...register('remark')}
          placeholder="请输入备注信息"
          rows={3}
        />
      </FormField>

      <div className="flex justify-end gap-4 pt-4">
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