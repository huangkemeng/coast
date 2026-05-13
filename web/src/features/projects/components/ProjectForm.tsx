import React from 'react';
import { useForm } from 'react-hook-form';
import { Input } from '@/components/ui/Input';
import { Textarea } from '@/components/ui/Textarea';
import { Button } from '@/components/ui/Button';
import { FormField } from '@/components/ui/FormField';
import type { CreateProjectRequest, Project } from '@/types/project';

interface ProjectFormProps {
  initialData?: Partial<Project>;
  onSubmit: (data: CreateProjectRequest) => void;
  isLoading?: boolean;
}

export const ProjectForm: React.FC<ProjectFormProps> = ({
  initialData,
  onSubmit,
  isLoading,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateProjectRequest>({
    defaultValues: {
      name: initialData?.name || '',
      description: initialData?.description || '',
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <FormField label="项目名称" required error={errors.name?.message}>
        <Input
          {...register('name', { required: '请输入项目名称' })}
          placeholder="请输入项目名称"
        />
      </FormField>

      <FormField label="项目描述">
        <Textarea
          {...register('description')}
          placeholder="请输入项目描述（可选）"
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