# 前端代码生成计划 - Plan 3: 项目管理与机器人配置模块

## 计划信息
- **计划编号**: Plan 3
- **项目名称**: 需求跟踪管理系统 - 前端
- **目标**: 实现项目管理和企业微信机器人配置页面
- **依赖**: Plan 1, Plan 2

---

## 1. 目标描述

实现项目管理和机器人配置功能的前端页面：
- 项目列表页面（表格展示、分页、搜索）
- 项目创建/编辑弹窗
- 机器人列表页面
- 机器人创建/编辑弹窗
- 机器人Webhook测试功能

---

## 2. 文件清单

### 2.1 页面文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/app/projects/page.tsx` | 创建 | 项目列表页面 |
| `src/app/bots/page.tsx` | 创建 | 机器人列表页面 |

### 2.2 组件文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/components/projects/ProjectForm.tsx` | 创建 | 项目表单组件 |
| `src/components/projects/ProjectTable.tsx` | 创建 | 项目表格组件 |
| `src/components/bots/BotForm.tsx` | 创建 | 机器人表单组件 |
| `src/components/bots/BotTable.tsx` | 创建 | 机器人表格组件 |

---

## 3. 实现细节

### 3.1 src/components/projects/ProjectForm.tsx

```typescript
'use client';

import { useState, useEffect } from 'react';
import { Button } from '@/src/components/ui/button';
import { Input } from '@/src/components/ui/input';
import { Label } from '@/src/components/ui/label';
import { Textarea } from '@/src/components/ui/textarea';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/src/components/ui/dialog';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/src/components/ui/select';
import { Project, User } from '@/src/types';

interface ProjectFormProps {
  project?: Project | null;
  users: User[];
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: any) => Promise<void>;
}

export function ProjectForm({ project, users, open, onOpenChange, onSubmit }: ProjectFormProps) {
  const [formData, setFormData] = useState({
    name: '',
    code: '',
    managerId: '',
    description: '',
  });
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (project) {
      setFormData({
        name: project.name,
        code: project.code || '',
        managerId: project.managerId || '',
        description: project.description || '',
      });
    } else {
      setFormData({
        name: '',
        code: '',
        managerId: '',
        description: '',
      });
    }
  }, [project, open]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      const submitData = { ...formData };
      if (!submitData.managerId) delete submitData.managerId;
      if (!submitData.code) delete submitData.code;
      await onSubmit(submitData);
      onOpenChange(false);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>{project ? '编辑项目' : '创建项目'}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="name">项目名称 *</Label>
            <Input
              id="name"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="code">项目编码</Label>
            <Input
              id="code"
              value={formData.code}
              onChange={(e) => setFormData({ ...formData, code: e.target.value })}
              placeholder="可选，用于唯一标识项目"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="manager">项目负责人</Label>
            <Select
              value={formData.managerId}
              onValueChange={(value) => setFormData({ ...formData, managerId: value })}
            >
              <SelectTrigger>
                <SelectValue placeholder="选择负责人" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">无</SelectItem>
                {users.map((user) => (
                  <SelectItem key={user.id} value={user.id}>
                    {user.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="description">项目描述</Label>
            <Textarea
              id="description"
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              rows={3}
            />
          </div>

          <div className="flex justify-end space-x-2">
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              取消
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? '保存中...' : '保存'}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
```

### 3.2 src/components/ui/textarea.tsx

```typescript
import * as React from "react"

import { cn } from "@/src/lib/utils"

export interface TextareaProps
  extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {}

const Textarea = React.forwardRef<HTMLTextAreaElement, TextareaProps>(
  ({ className, ...props }, ref) => {
    return (
      <textarea
        className={cn(
          "flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50",
          className
        )}
        ref={ref}
        {...props}
      />
    )
  }
)
Textarea.displayName = "Textarea"

export { Textarea }
```

### 3.3 src/components/projects/ProjectTable.tsx

```typescript
'use client';

import { Project } from '@/src/types';
import { Button } from '@/src/components/ui/button';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/src/components/ui/table';
import { Pencil, Trash2 } from 'lucide-react';
import { formatDateTime } from '@/src/lib/utils';

interface ProjectTableProps {
  projects: Project[];
  onEdit: (project: Project) => void;
  onDelete: (project: Project) => void;
}

export function ProjectTable({ projects, onEdit, onDelete }: ProjectTableProps) {
  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>项目名称</TableHead>
            <TableHead>项目编码</TableHead>
            <TableHead>负责人</TableHead>
            <TableHead>需求数量</TableHead>
            <TableHead>创建时间</TableHead>
            <TableHead className="text-right">操作</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {projects.length === 0 ? (
            <TableRow>
              <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                暂无数据
              </TableCell>
            </TableRow>
          ) : (
            projects.map((project) => (
              <TableRow key={project.id}>
                <TableCell className="font-medium">{project.name}</TableCell>
                <TableCell>{project.code || '-'}</TableCell>
                <TableCell>{project.manager?.name || '-'}</TableCell>
                <TableCell>{project._count?.requirements || 0}</TableCell>
                <TableCell>{formatDateTime(project.createdAt)}</TableCell>
                <TableCell className="text-right">
                  <div className="flex justify-end gap-2">
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => onEdit(project)}
                      title="编辑"
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => onDelete(project)}
                      title="删除"
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </div>
  );
}
```

### 3.4 src/app/projects/page.tsx

```typescript
'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/src/hooks/useAuth';
import { MainLayout } from '@/src/components/layout/MainLayout';
import { Button } from '@/src/components/ui/button';
import { Input } from '@/src/components/ui/input';
import { ProjectTable } from '@/src/components/projects/ProjectTable';
import { ProjectForm } from '@/src/components/projects/ProjectForm';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/src/components/ui/alert-dialog';
import api from '@/src/lib/api';
import { Project, User } from '@/src/types';
import { Plus, Search } from 'lucide-react';

export default function ProjectsPage() {
  const router = useRouter();
  const { user, isLoading } = useAuth();
  const [projects, setProjects] = useState<Project[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(20);
  const [keyword, setKeyword] = useState('');

  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingProject, setEditingProject] = useState<Project | null>(null);

  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [deletingProject, setDeletingProject] = useState<Project | null>(null);

  useEffect(() => {
    if (!isLoading && !user) {
      router.push('/');
      return;
    }
    if (user) {
      fetchProjects();
      fetchUsers();
    }
  }, [user, isLoading, router, page, keyword]);

  const fetchProjects = async () => {
    try {
      const response = await api.get('/projects', {
        params: { page, pageSize, keyword },
      });
      if (response.success) {
        setProjects(response.data.items);
        setTotal(response.data.total);
      }
    } catch (error) {
      console.error('Failed to fetch projects:', error);
    }
  };

  const fetchUsers = async () => {
    try {
      const response = await api.get('/users', {
        params: { page: 1, pageSize: 100 },
      });
      if (response.success) {
        setUsers(response.data.items);
      }
    } catch (error) {
      console.error('Failed to fetch users:', error);
    }
  };

  const handleCreate = () => {
    setEditingProject(null);
    setIsFormOpen(true);
  };

  const handleEdit = (project: Project) => {
    setEditingProject(project);
    setIsFormOpen(true);
  };

  const handleDelete = (project: Project) => {
    setDeletingProject(project);
    setIsDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!deletingProject) return;
    try {
      await api.delete(`/projects/${deletingProject.id}`);
      fetchProjects();
    } catch (error) {
      console.error('Failed to delete project:', error);
    } finally {
      setIsDeleteDialogOpen(false);
      setDeletingProject(null);
    }
  };

  const handleFormSubmit = async (data: any) => {
    if (editingProject) {
      await api.put(`/projects/${editingProject.id}`, data);
    } else {
      await api.post('/projects', data);
    }
    fetchProjects();
  };

  if (isLoading || !user) {
    return null;
  }

  return (
    <MainLayout>
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <h1 className="text-3xl font-bold">项目管理</h1>
          <Button onClick={handleCreate}>
            <Plus className="mr-2 h-4 w-4" />
            创建项目
          </Button>
        </div>

        <div className="flex items-center gap-4">
          <div className="relative flex-1 max-w-sm">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              placeholder="搜索项目名称、编码..."
              value={keyword}
              onChange={(e) => setKeyword(e.target.value)}
              className="pl-10"
            />
          </div>
        </div>

        <ProjectTable
          projects={projects}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />

        {total > pageSize && (
          <div className="flex items-center justify-between">
            <div className="text-sm text-muted-foreground">
              共 {total} 条记录，第 {page} 页
            </div>
            <div className="flex gap-2">
              <Button
                variant="outline"
                onClick={() => setPage(page - 1)}
                disabled={page === 1}
              >
                上一页
              </Button>
              <Button
                variant="outline"
                onClick={() => setPage(page + 1)}
                disabled={page * pageSize >= total}
              >
                下一页
              </Button>
            </div>
          </div>
        )}
      </div>

      <ProjectForm
        project={editingProject}
        users={users}
        open={isFormOpen}
        onOpenChange={setIsFormOpen}
        onSubmit={handleFormSubmit}
      />

      <AlertDialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>确认删除</AlertDialogTitle>
            <AlertDialogDescription>
              确定要删除项目 &quot;{deletingProject?.name}&quot; 吗？
              {deletingProject?._count?.requirements ? (
                <span className="text-destructive block mt-2">
                  该项目下有 {deletingProject._count.requirements} 个需求，无法删除。
                </span>
              ) : (
                '此操作不可恢复。'
              )}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>取消</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="bg-destructive"
              disabled={!!deletingProject?._count?.requirements}
            >
              删除
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </MainLayout>
  );
}
```

### 3.5 src/components/bots/BotForm.tsx

```typescript
'use client';

import { useState, useEffect } from 'react';
import { Button } from '@/src/components/ui/button';
import { Input } from '@/src/components/ui/input';
import { Label } from '@/src/components/ui/label';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/src/components/ui/dialog';
import { Switch } from '@/src/components/ui/switch';
import { WebhookBot } from '@/src/types';

interface BotFormProps {
  bot?: WebhookBot | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: any) => Promise<void>;
  onTest?: (botId: string) => Promise<void>;
}

export function BotForm({ bot, open, onOpenChange, onSubmit, onTest }: BotFormProps) {
  const [formData, setFormData] = useState({
    name: '',
    webhookUrl: '',
    groupName: '',
    isActive: true,
  });
  const [isLoading, setIsLoading] = useState(false);
  const [isTesting, setIsTesting] = useState(false);

  useEffect(() => {
    if (bot) {
      setFormData({
        name: bot.name,
        webhookUrl: bot.webhookUrl,
        groupName: bot.groupName || '',
        isActive: bot.isActive,
      });
    } else {
      setFormData({
        name: '',
        webhookUrl: '',
        groupName: '',
        isActive: true,
      });
    }
  }, [bot, open]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      await onSubmit(formData);
      onOpenChange(false);
    } finally {
      setIsLoading(false);
    }
  };

  const handleTest = async () => {
    if (!bot || !onTest) return;
    setIsTesting(true);
    try {
      await onTest(bot.id);
    } finally {
      setIsTesting(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>{bot ? '编辑机器人' : '创建机器人'}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="name">机器人名称 *</Label>
            <Input
              id="name"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="webhookUrl">Webhook地址 *</Label>
            <Input
              id="webhookUrl"
              value={formData.webhookUrl}
              onChange={(e) => setFormData({ ...formData, webhookUrl: e.target.value })}
              placeholder="https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=..."
              required
            />
            <p className="text-xs text-muted-foreground">
              从企业微信机器人设置中获取Webhook地址
            </p>
          </div>

          <div className="space-y-2">
            <Label htmlFor="groupName">群组名称</Label>
            <Input
              id="groupName"
              value={formData.groupName}
              onChange={(e) => setFormData({ ...formData, groupName: e.target.value })}
              placeholder="可选，用于标识接收消息的群组"
            />
          </div>

          <div className="flex items-center space-x-2">
            <Switch
              id="isActive"
              checked={formData.isActive}
              onCheckedChange={(checked) => setFormData({ ...formData, isActive: checked })}
            />
            <Label htmlFor="isActive">启用机器人</Label>
          </div>

          <div className="flex justify-between">
            {bot && onTest && (
              <Button
                type="button"
                variant="outline"
                onClick={handleTest}
                disabled={isTesting || !formData.isActive}
              >
                {isTesting ? '测试中...' : '测试连接'}
              </Button>
            )}
            <div className="flex gap-2 ml-auto">
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                取消
              </Button>
              <Button type="submit" disabled={isLoading}>
                {isLoading ? '保存中...' : '保存'}
              </Button>
            </div>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
```

### 3.6 src/components/bots/BotTable.tsx

```typescript
'use client';

import { WebhookBot } from '@/src/types';
import { Button } from '@/src/components/ui/button';
import { Badge } from '@/src/components/ui/badge';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/src/components/ui/table';
import { Pencil, Trash2, Send } from 'lucide-react';
import { formatDateTime } from '@/src/lib/utils';

interface BotTableProps {
  bots: WebhookBot[];
  onEdit: (bot: WebhookBot) => void;
  onDelete: (bot: WebhookBot) => void;
  onTest: (bot: WebhookBot) => void;
}

export function BotTable({ bots, onEdit, onDelete, onTest }: BotTableProps) {
  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>机器人名称</TableHead>
            <TableHead>群组名称</TableHead>
            <TableHead>Webhook地址</TableHead>
            <TableHead>关联需求数</TableHead>
            <TableHead>状态</TableHead>
            <TableHead>创建时间</TableHead>
            <TableHead className="text-right">操作</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {bots.length === 0 ? (
            <TableRow>
              <TableCell colSpan={7} className="text-center py-8 text-muted-foreground">
                暂无数据
              </TableCell>
            </TableRow>
          ) : (
            bots.map((bot) => (
              <TableRow key={bot.id}>
                <TableCell className="font-medium">{bot.name}</TableCell>
                <TableCell>{bot.groupName || '-'}</TableCell>
                <TableCell className="max-w-xs truncate">{bot.webhookUrl}</TableCell>
                <TableCell>{bot._count?.requirements || 0}</TableCell>
                <TableCell>
                  <Badge variant={bot.isActive ? 'default' : 'secondary'}>
                    {bot.isActive ? '启用' : '禁用'}
                  </Badge>
                </TableCell>
                <TableCell>{formatDateTime(bot.createdAt)}</TableCell>
                <TableCell className="text-right">
                  <div className="flex justify-end gap-2">
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => onTest(bot)}
                      disabled={!bot.isActive}
                      title="测试"
                    >
                      <Send className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => onEdit(bot)}
                      title="编辑"
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => onDelete(bot)}
                      title="删除"
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </div>
  );
}
```

### 3.7 src/app/bots/page.tsx

```typescript
'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/src/hooks/useAuth';
import { MainLayout } from '@/src/components/layout/MainLayout';
import { Button } from '@/src/components/ui/button';
import { Input } from '@/src/components/ui/input';
import { BotTable } from '@/src/components/bots/BotTable';
import { BotForm } from '@/src/components/bots/BotForm';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/src/components/ui/alert-dialog';
import api from '@/src/lib/api';
import { WebhookBot } from '@/src/types';
import { Plus, Search } from 'lucide-react';

export default function BotsPage() {
  const router = useRouter();
  const { user, isLoading } = useAuth();
  const [bots, setBots] = useState<WebhookBot[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(20);
  const [keyword, setKeyword] = useState('');

  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingBot, setEditingBot] = useState<WebhookBot | null>(null);

  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [deletingBot, setDeletingBot] = useState<WebhookBot | null>(null);

  useEffect(() => {
    if (!isLoading && !user) {
      router.push('/');
      return;
    }
    if (user) {
      fetchBots();
    }
  }, [user, isLoading, router, page, keyword]);

  const fetchBots = async () => {
    try {
      const response = await api.get('/webhook-bots', {
        params: { page, pageSize, keyword },
      });
      if (response.success) {
        setBots(response.data.items);
        setTotal(response.data.total);
      }
    } catch (error) {
      console.error('Failed to fetch bots:', error);
    }
  };

  const handleCreate = () => {
    setEditingBot(null);
    setIsFormOpen(true);
  };

  const handleEdit = (bot: WebhookBot) => {
    setEditingBot(bot);
    setIsFormOpen(true);
  };

  const handleDelete = (bot: WebhookBot) => {
    setDeletingBot(bot);
    setIsDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!deletingBot) return;
    try {
      await api.delete(`/webhook-bots/${deletingBot.id}`);
      fetchBots();
    } catch (error) {
      console.error('Failed to delete bot:', error);
    } finally {
      setIsDeleteDialogOpen(false);
      setDeletingBot(null);
    }
  };

  const handleFormSubmit = async (data: any) => {
    if (editingBot) {
      await api.put(`/webhook-bots/${editingBot.id}`, data);
    } else {
      await api.post('/webhook-bots', data);
    }
    fetchBots();
  };

  const handleTest = async (bot: WebhookBot) => {
    try {
      const response = await api.post(`/webhook-bots/${bot.id}/test`);
      if (response.success) {
        alert('测试消息发送成功！');
      } else {
        alert(response.error || '测试失败');
      }
    } catch (error) {
      alert('测试失败：' + (error instanceof Error ? error.message : '未知错误'));
    }
  };

  if (isLoading || !user) {
    return null;
  }

  return (
    <MainLayout>
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold">机器人配置</h1>
            <p className="text-muted-foreground mt-1">
              配置企业微信机器人，用于接收需求状态变更通知
            </p>
          </div>
          <Button onClick={handleCreate}>
            <Plus className="mr-2 h-4 w-4" />
            添加机器人
          </Button>
        </div>

        <div className="flex items-center gap-4">
          <div className="relative flex-1 max-w-sm">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              placeholder="搜索机器人名称、群组..."
              value={keyword}
              onChange={(e) => setKeyword(e.target.value)}
              className="pl-10"
            />
          </div>
        </div>

        <BotTable
          bots={bots}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onTest={handleTest}
        />

        {total > pageSize && (
          <div className="flex items-center justify-between">
            <div className="text-sm text-muted-foreground">
              共 {total} 条记录，第 {page} 页
            </div>
            <div className="flex gap-2">
              <Button
                variant="outline"
                onClick={() => setPage(page - 1)}
                disabled={page === 1}
              >
                上一页
              </Button>
              <Button
                variant="outline"
                onClick={() => setPage(page + 1)}
                disabled={page * pageSize >= total}
              >
                下一页
              </Button>
            </div>
          </div>
        )}
      </div>

      <BotForm
        bot={editingBot}
        open={isFormOpen}
        onOpenChange={setIsFormOpen}
        onSubmit={handleFormSubmit}
        onTest={editingBot ? handleTest : undefined}
      />

      <AlertDialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>确认删除</AlertDialogTitle>
            <AlertDialogDescription>
              确定要删除机器人 &quot;{deletingBot?.name}&quot; 吗？此操作不可恢复。
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>取消</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} className="bg-destructive">
              删除
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </MainLayout>
  );
}
```

---

## 4. 验证方法

1. 启动前后端服务
2. 访问项目管理页面 `/projects`
3. 测试项目创建、编辑、删除功能
4. 访问机器人配置页面 `/bots`
5. 测试机器人创建、编辑、删除、测试功能

---

## 5. 交付清单

- [ ] 项目管理页面可用
- [ ] 项目CRUD功能正常
- [ ] 机器人配置页面可用
- [ ] 机器人CRUD功能正常
- [ ] 机器人Webhook测试功能正常
- [ ] 无 TypeScript 编译错误
