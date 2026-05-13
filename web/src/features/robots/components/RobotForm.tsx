import React from 'react';
import { useForm, Controller } from 'react-hook-form';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { Checkbox } from '@/components/ui/Checkbox';
import { FormField } from '@/components/ui/FormField';
import type { CreateRobotRequest, Robot } from '@/types/robot';

interface RobotFormProps {
  initialData?: Partial<Robot>;
  onSubmit: (data: CreateRobotRequest) => void;
  isLoading?: boolean;
}

export const RobotForm: React.FC<RobotFormProps> = ({
  initialData,
  onSubmit,
  isLoading,
}) => {
  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<CreateRobotRequest>({
    defaultValues: {
      name: initialData?.name || '',
      webhookUrl: initialData?.webhookUrl || '',
      secret: initialData?.secret || '',
      isEnabled: initialData?.isEnabled ?? true,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <FormField label="机器人名称" required error={errors.name?.message}>
        <Input
          {...register('name', { required: '请输入机器人名称' })}
          placeholder="请输入机器人名称"
        />
      </FormField>

      <FormField label="Webhook 地址" required error={errors.webhookUrl?.message}>
        <Input
          {...register('webhookUrl', {
            required: '请输入 Webhook 地址',
            pattern: {
              value: /^https?:\/\/.+/,
              message: '请输入有效的 URL',
            },
          })}
          placeholder="https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=xxx"
        />
      </FormField>

      <FormField label="加签密钥（可选）">
        <Input
          {...register('secret')}
          placeholder="请输入加签密钥（可选）"
        />
      </FormField>

      <Controller
        name="isEnabled"
        control={control}
        render={({ field }) => (
          <Checkbox
            checked={field.value}
            onChange={field.onChange}
            label="启用机器人"
          />
        )}
      />

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