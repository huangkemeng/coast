### 3.8 src/components/requirements/RequirementForm.tsx

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
import { Popover, PopoverContent, PopoverTrigger } from '@/src/components/ui/popover';
import { Calendar } from '@/src/components/ui/calendar';
import { format } from 'date-fns';
import { zhCN } from 'date-fns/locale';
import { CalendarIcon } from 'lucide-react';
import { cn } from '@/src/lib/utils';
import { Requirement, Project, User, WebhookBot } from '@/src/types';
import { REQUIREMENT_STATUSES, PRIORITIES } from '@/src/lib/constants';

interface RequirementFormProps {
  requirement?: Requirement | null;
  projects: Project[];
  users: User[];
  bots: WebhookBot[];
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: any) => Promise<void>;
  isAdmin?: boolean;
}

export function RequirementForm({
  requirement,
  projects,
  users,
  bots,
  open,
  onOpenChange,
  onSubmit,
  isAdmin,
}: RequirementFormProps) {
  const [formData, setFormData] = useState({
    title: '',
    requirementNo: '',
    status: 'PENDING_CONFIRM',
    progress: 0,
    followerId: '',
    projectId: '',
    plannedStartDate: null as Date | null,
    plannedTestDate: null as Date | null,
    plannedReleaseDate: null as Date | null,
    docUrl: '',
    quoteAmount: '',
    botId: '',
    priority: 'MEDIUM',
    remark: '',
  });
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (requirement) {
      setFormData({
        title: requirement.title,
        requirementNo: requirement.requirementNo,
        status: requirement.status,
        progress: requirement.progress,
        followerId: requirement.followerId,
        projectId: requirement.projectId,
        plannedStartDate: requirement.plannedStartDate ? new Date(requirement.plannedStartDate) : null,
        plannedTestDate: requirement.plannedTestDate ? new Date(requirement.plannedTestDate) : null,
        plannedReleaseDate: requirement.plannedReleaseDate ? new Date(requirement.plannedReleaseDate) : null,
        docUrl: requirement.docUrl || '',
        quoteAmount: requirement.quoteAmount?.toString() || '',
        botId: requirement.botId || '',
        priority: requirement.priority,
        remark: requirement.remark || '',
      });
    } else {
      setFormData({
        title: '',
        requirementNo: '',
        status: 'PENDING_CONFIRM',
        progress: 0,
        followerId: '',
        projectId: '',
        plannedStartDate: null,
        plannedTestDate: null,
        plannedReleaseDate: null,
        docUrl: '',
        quoteAmount: '',
        botId: '',
        priority: 'MEDIUM',
        remark: '',
      });
    }
  }, [requirement, open]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      const submitData = {
        ...formData,
        quoteAmount: formData.quoteAmount ? parseFloat(formData.quoteAmount) : null,
        botId: formData.botId || null,
        plannedStartDate: formData.plannedStartDate?.toISOString() || null,
        plannedTestDate: formData.plannedTestDate?.toISOString() || null,
        plannedReleaseDate: formData.plannedReleaseDate?.toISOString() || null,
      };
      await onSubmit(submitData);
      onOpenChange(false);
    } finally {
      setIsLoading(false);
    }
  };

  const DatePicker = ({
    value,
    onChange,
    placeholder,
  }: {
    value: Date | null;
    onChange: (date: Date | null) => void;
    placeholder: string;
  }) => (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          className={cn(
            'w-full justify-start text-left font-normal',
            !value && 'text-muted-foreground'
          )}
        >
          <CalendarIcon className="mr-2 h-4 w-4" />
          {value ? format(value, 'yyyy-MM-dd', { locale: zhCN }) : placeholder}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={value || undefined}
          onSelect={onChange}
          initialFocus
        />
      </PopoverContent>
    </Popover>
  );

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[600px] max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{requirement ? '编辑需求' : '创建需求'}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="requirementNo">需求号 *</Label>
              <Input
                id="requirementNo"
                value={formData.requirementNo}
                onChange={(e) => setFormData({ ...formData, requirementNo: e.target.value })}
                disabled={!!requirement}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="title">需求名称 *</Label>
              <Input
                id="title"
                value={formData.title}
                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                required
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="project">所属项目 *</Label>
              <Select
                value={formData.projectId}
                onValueChange={(value) => setFormData({ ...formData, projectId: value })}
              >
                <SelectTrigger>
                  <SelectValue placeholder="选择项目" />
                </SelectTrigger>
                <SelectContent>
                  {projects.map((p) => (
                    <SelectItem key={p.id} value={p.id}>
                      {p.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="follower">跟进人 *</Label>
              <Select
                value={formData.followerId}
                onValueChange={(value) => setFormData({ ...formData, followerId: value })}
              >
                <SelectTrigger>
                  <SelectValue placeholder="选择跟进人" />
                </SelectTrigger>
                <SelectContent>
                  {users.map((u) => (
                    <SelectItem key={u.id} value={u.id}>
                      {u.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="priority">优先级</Label>
              <Select
                value={formData.priority}
                onValueChange={(value) => setFormData({ ...formData, priority: value })}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {Object.values(PRIORITIES).map((p) => (
                    <SelectItem key={p.value} value={p.value}>
                      {p.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="bot">通知机器人</Label>
              <Select
                value={formData.botId}
                onValueChange={(value) => setFormData({ ...formData, botId: value })}
              >
                <SelectTrigger>
                  <SelectValue placeholder="选择机器人（可选）" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="">不发送通知</SelectItem>
                  {bots.map((b) => (
                    <SelectItem key={b.id} value={b.id}>
                      {b.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="progress">进度 ({formData.progress}%)</Label>
            <Input
              id="progress"
              type="range"
              min="0"
              max="100"
              value={formData.progress}
              onChange={(e) => setFormData({ ...formData, progress: parseInt(e.target.value) })}
            />
          </div>

          <div className="grid grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label>计划开始时间</Label>
              <DatePicker
                value={formData.plannedStartDate}
                onChange={(date) => setFormData({ ...formData, plannedStartDate: date })}
                placeholder="选择日期"
              />
            </div>
            <div className="space-y-2">
              <Label>计划交测时间</Label>
              <DatePicker
                value={formData.plannedTestDate}
                onChange={(date) => setFormData({ ...formData, plannedTestDate: date })}
                placeholder="选择日期"
              />
            </div>
            <div className="space-y-2">
              <Label>计划上线时间</Label>
              <DatePicker
                value={formData.plannedReleaseDate}
                onChange={(date) => setFormData({ ...formData, plannedReleaseDate: date })}
                placeholder="选择日期"
              />
            </div>
          </div>

          {isAdmin && (
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="quoteAmount">报价金额</Label>
                <Input
                  id="quoteAmount"
                  type="number"
                  value={formData.quoteAmount}
                  onChange={(e) => setFormData({ ...formData, quoteAmount: e.target.value })}
                  placeholder="元"
                />
              </div>
            </div>
          )}

          <div className="space-y-2">
            <Label htmlFor="docUrl">需求文档链接</Label>
            <Input
              id="docUrl"
              type="url"
              value={formData.docUrl}
              onChange={(e) => setFormData({ ...formData, docUrl: e.target.value })}
              placeholder="https://..."
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="remark">备注</Label>
            <Textarea
              id="remark"
              value={formData.remark}
              onChange={(e) => setFormData({ ...formData, remark: e.target.value })}
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

### 3.9 src/app/requirements/page.tsx

```typescript
'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/src/hooks/useAuth';
import { MainLayout } from '@/src/components/layout/MainLayout';
import { Button } from '@/src/components/ui/button';
import { RequirementTable } from '@/src/components/requirements/RequirementTable';
import { RequirementForm } from '@/src/components/requirements/RequirementForm';
import { RequirementFilters } from '@/src/components/requirements/RequirementFilters';
import { StatusChangeDialog } from '@/src/components/requirements/StatusChangeDialog';
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
import { Requirement, Project, User, WebhookBot, RequirementStatus } from '@/src/types';
import { Plus } from 'lucide-react';

export default function RequirementsPage() {
  const router = useRouter();
  const { user, isLoading } = useAuth();
  const [requirements, setRequirements] = useState<Requirement[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [bots, setBots] = useState<WebhookBot[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(20);

  // 筛选条件
  const [keyword, setKeyword] = useState('');
  const [status, setStatus] = useState('');
  const [priority, setPriority] = useState('');
  const [projectId, setProjectId] = useState('');
  const [followerId, setFollowerId] = useState('');

  // 表单状态
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingRequirement, setEditingRequirement] = useState<Requirement | null>(null);

  // 状态变更
  const [isStatusDialogOpen, setIsStatusDialogOpen] = useState(false);
  const [statusChangeRequirement, setStatusChangeRequirement] = useState<Requirement | null>(null);

  // 删除确认
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [deletingRequirement, setDeletingRequirement] = useState<Requirement | null>(null);

  useEffect(() => {
    if (!isLoading && !user) {
      router.push('/');
      return;
    }
    if (user) {
      fetchRequirements();
      fetchProjects();
      fetchUsers();
      fetchBots();
    }
  }, [user, isLoading, router, page, keyword, status, priority, projectId, followerId]);

  const fetchRequirements = async () => {
    try {
      const params: any = { page, pageSize };
      if (keyword) params.keyword = keyword;
      if (status) params.status = status;
      if (priority) params.priority = priority;
      if (projectId) params.projectId = projectId;
      if (followerId) params.followerId = followerId;

      const response = await api.get('/requirements', { params });
      if (response.success) {
        setRequirements(response.data.items);
        setTotal(response.data.total);
      }
    } catch (error) {
      console.error('Failed to fetch requirements:', error);
    }
  };

  const fetchProjects = async () => {
    try {
      const response = await api.get('/projects', { params: { pageSize: 100 } });
      if (response.success) {
        setProjects(response.data.items);
      }
    } catch (error) {
      console.error('Failed to fetch projects:', error);
    }
  };

  const fetchUsers = async () => {
    try {
      const response = await api.get('/users', { params: { pageSize: 100 } });
      if (response.success) {
        setUsers(response.data.items);
      }
    } catch (error) {
      console.error('Failed to fetch users:', error);
    }
  };

  const fetchBots = async () => {
    try {
      const response = await api.get('/webhook-bots', { params: { pageSize: 100 } });
      if (response.success) {
        setBots(response.data.items);
      }
    } catch (error) {
      console.error('Failed to fetch bots:', error);
    }
  };

  const handleCreate = () => {
    setEditingRequirement(null);
    setIsFormOpen(true);
  };

  const handleEdit = (req: Requirement) => {
    setEditingRequirement(req);
    setIsFormOpen(true);
  };

  const handleDelete = (req: Requirement) => {
    setDeletingRequirement(req);
    setIsDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (!deletingRequirement) return;
    try {
      await api.delete(`/requirements/${deletingRequirement.id}`);
      fetchRequirements();
    } catch (error) {
      console.error('Failed to delete requirement:', error);
    } finally {
      setIsDeleteDialogOpen(false);
      setDeletingRequirement(null);
    }
  };

  const handleFormSubmit = async (data: any) => {
    if (editingRequirement) {
      await api.put(`/requirements/${editingRequirement.id}`, data);
    } else {
      await api.post('/requirements', data);
    }
    fetchRequirements();
  };

  const handleStatusChange = (req: Requirement) => {
    setStatusChangeRequirement(req);
    setIsStatusDialogOpen(true);
  };

  const handleStatusSubmit = async (requirementId: string, newStatus: RequirementStatus, remark?: string) => {
    await api.put(`/requirements/${requirementId}/status`, { status: newStatus, remark });
    fetchRequirements();
  };

  const handleResetFilters = () => {
    setKeyword('');
    setStatus('');
    setPriority('');
    setProjectId('');
    setFollowerId('');
    setPage(1);
  };

  if (isLoading || !user) {
    return null;
  }

  return (
    <MainLayout>
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <h1 className="text-3xl font-bold">需求管理</h1>
          {user.role === 'ADMIN' && (
            <Button onClick={handleCreate}>
              <Plus className="mr-2 h-4 w-4" />
              新建需求
            </Button>
          )}
        </div>

        <RequirementFilters
          keyword={keyword}
          onKeywordChange={setKeyword}
          status={status}
          onStatusChange={setStatus}
          priority={priority}
          onPriorityChange={setPriority}
          projectId={projectId}
          onProjectIdChange={setProjectId}
          followerId={followerId}
          onFollowerIdChange={setFollowerId}
          projects={projects}
          users={users}
          onReset={handleResetFilters}
        />

        <RequirementTable
          requirements={requirements}
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

      <RequirementForm
        requirement={editingRequirement}
        projects={projects}
        users={users}
        bots={bots}
        open={isFormOpen}
        onOpenChange={setIsFormOpen}
        onSubmit={handleFormSubmit}
        isAdmin={user.role === 'ADMIN'}
      />

      <StatusChangeDialog
        requirement={statusChangeRequirement}
        open={isStatusDialogOpen}
        onOpenChange={setIsStatusDialogOpen}
        onSubmit={handleStatusSubmit}
      />

      <AlertDialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>确认删除</AlertDialogTitle>
            <AlertDialogDescription>
              确定要删除需求 &quot;{deletingRequirement?.title}&quot; 吗？此操作不可恢复。
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

### 3.10 src/app/requirements/[id]/page.tsx

```typescript
'use client';

import { useEffect, useState } from 'react';
import { useRouter, useParams } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/src/hooks/useAuth';
import { MainLayout } from '@/src/components/layout/MainLayout';
import { Button } from '@/src/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/src/components/ui/card';
import { Badge } from '@/src/components/ui/badge';
import { Progress } from '@/src/components/ui/progress';
import { StatusBadge, PriorityBadge } from '@/src/components/requirements/StatusBadge';
import { StatusChangeDialog } from '@/src/components/requirements/StatusChangeDialog';
import api from '@/src/lib/api';
import { Requirement, RequirementStatus } from '@/src/types';
import { ArrowLeft, ExternalLink } from 'lucide-react';
import { formatDate, formatDateTime } from '@/src/lib/utils';
import { REQUIREMENT_STATUSES } from '@/src/lib/constants';

export default function RequirementDetailPage() {
  const router = useRouter();
  const params = useParams();
  const { user, isLoading } = useAuth();
  const [requirement, setRequirement] = useState<Requirement | null>(null);
  const [isStatusDialogOpen, setIsStatusDialogOpen] = useState(false);

  useEffect(() => {
    if (!isLoading && !user) {
      router.push('/');
      return;
    }
    if (user && params.id) {
      fetchRequirement();
    }
  }, [user, isLoading, router, params.id]);

  const fetchRequirement = async () => {
    try {
      const response = await api.get(`/requirements/${params.id}`);
      if (response.success) {
        setRequirement(response.data);
      }
    } catch (error) {
      console.error('Failed to fetch requirement:', error);
    }
  };

  const handleStatusSubmit = async (requirementId: string, newStatus: RequirementStatus, remark?: string) => {
    await api.put(`/requirements/${requirementId}/status`, { status: newStatus, remark });
    fetchRequirement();
  };

  if (isLoading || !user || !requirement) {
    return null;
  }

  return (
    <MainLayout>
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button variant="outline" size="icon" asChild>
            <Link href="/requirements">
              <ArrowLeft className="h-4 w-4" />
            </Link>
          </Button>
          <h1 className="text-3xl font-bold">需求详情</h1>
        </div>

        <div className="grid gap-6 md:grid-cols-2">
          <Card>
            <CardHeader>
              <CardTitle>基本信息</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm text-muted-foreground">需求号</label>
                  <p className="font-medium">{requirement.requirementNo}</p>
                </div>
                <div>
                  <label className="text-sm text-muted-foreground">需求名称</label>
                  <p className="font-medium">{requirement.title}</p>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm text-muted-foreground">当前状态</label>
                  <div className="mt-1">
                    <StatusBadge status={requirement.status} />
                  </div>
                </div>
                <div>
                  <label className="text-sm text-muted-foreground">优先级</label>
                  <div className="mt-1">
                    <PriorityBadge priority={requirement.priority} />
                  </div>
                </div>
              </div>

              <div>
                <label className="text-sm text-muted-foreground">进度</label>
                <div className="flex items-center gap-2 mt-1">
                  <Progress value={requirement.progress} className="flex-1" />
                  <span className="text-sm">{requirement.progress}%</span>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm text-muted-foreground">跟进人</label>
                  <p className="font-medium">{requirement.follower?.name}</p>
                </div>
                <div>
                  <label className="text-sm text-muted-foreground">所属项目</label>
                  <p className="font-medium">{requirement.project?.name}</p>
                </div>
              </div>

              {requirement.docUrl && (
                <div>
                  <label className="text-sm text-muted-foreground">需求文档</label>
                  <a
                    href={requirement.docUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="flex items-center gap-1 text-primary hover:underline mt-1"
                  >
                    查看文档 <ExternalLink className="h-3 w-3" />
                  </a>
                </div>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>时间安排</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm text-muted-foreground">计划开始时间</label>
                  <p className="font-medium">{formatDate(requirement.plannedStartDate)}</p>
                </div>
                <div>
                  <label className="text-sm text-muted-foreground">计划交测时间</label>
                  <p className="font-medium">{formatDate(requirement.plannedTestDate)}</p>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm text-muted-foreground">计划上线时间</label>
                  <p className="font-medium">{formatDate(requirement.plannedReleaseDate)}</p>
                </div>
                <div>
                  <label className="text-sm text-muted-foreground">需求确认</label>
                  <p className="font-medium">{requirement.isConfirmed ? '已确认' : '待确认'}</p>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm text-muted-foreground">实际交测时间</label>
                  <p className="font-medium">{formatDate(requirement.actualTestDate)}</p>
                </div>
                <div>
                  <label className="text-sm text-muted-foreground">实际上线时间</label>
                  <p className="font-medium">{formatDate(requirement.actualReleaseDate)}</p>
                </div>
              </div>

              {user.role === 'ADMIN' && requirement.quoteAmount && (
                <div>
                  <label className="text-sm text-muted-foreground">报价金额</label>
                  <p className="font-medium">¥{requirement.quoteAmount}</p>
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {requirement.remark && (
          <Card>
            <CardHeader>
              <CardTitle>备注</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="whitespace-pre-wrap">{requirement.remark}</p>
            </CardContent>
          </Card>
        )}

        <Card>
          <CardHeader>
            <CardTitle>操作</CardTitle>
          </CardHeader>
          <CardContent>
            <Button onClick={() => setIsStatusDialogOpen(true)}>
              变更状态
            </Button>
          </CardContent>
        </Card>
      </div>

      <StatusChangeDialog
        requirement={requirement}
        open={isStatusDialogOpen}
        onOpenChange={setIsStatusDialogOpen}
        onSubmit={handleStatusSubmit}
      />
    </MainLayout>
  );
}
```

---

## 4. 验证方法

1. 启动前后端服务
2. 访问需求列表页面 `/requirements`
3. 测试需求创建、编辑、删除功能
4. 测试筛选、排序功能
5. 测试状态变更功能
6. 访问需求详情页面

---

## 5. 交付清单

- [ ] 需求列表页面可用
- [ ] 需求创建/编辑功能正常
- [ ] 需求筛选功能正常
- [ ] 需求状态变更功能正常
- [ ] 需求详情页面可用
- [ ] 无 TypeScript 编译错误
