import { z } from 'zod';

export const requirementSchema = z.object({
  name: z.string().min(1, '请输入需求名称').max(100, '需求名称不能超过100个字符'),
  projectId: z.number().optional(),
  followerId: z.number().optional(),
  priority: z.number().min(0).max(2).default(1),
  isConfirmed: z.boolean().default(false),
  price: z.number().min(0).optional().nullable(),
  deadline: z.string().optional().nullable(),
  docUrl: z.string().optional().nullable(),
  version: z.string().optional().nullable(),
  content: z.string().optional(),
});

export const projectSchema = z.object({
  name: z.string().min(1, '请输入项目名称').max(100, '项目名称不能超过100个字符'),
  description: z.string().optional(),
});

export const userSchema = z.object({
  username: z.string().min(1, '请输入用户名').max(50, '用户名不能超过50个字符'),
  realName: z.string().min(1, '请输入真实姓名').max(50, '真实姓名不能超过50个字符'),
  password: z.string().min(6, '密码至少6个字符').optional(),
  role: z.number().min(0).max(1).default(0),
});

export const robotSchema = z.object({
  name: z.string().min(1, '请输入机器人名称').max(50, '机器人名称不能超过50个字符'),
  webhookUrl: z.string().min(1, '请输入Webhook地址').refine(validateUrl, '请输入有效的URL'),
  secret: z.string().optional(),
  isEnabled: z.boolean().default(true),
});

function validateUrl(url: string): boolean {
  try {
    new URL(url);
    return true;
  } catch {
    return false;
  }
}

export type RequirementFormData = z.infer<typeof requirementSchema>;
export type ProjectFormData = z.infer<typeof projectSchema>;
export type UserFormData = z.infer<typeof userSchema>;
export type RobotFormData = z.infer<typeof robotSchema>;