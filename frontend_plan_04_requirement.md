# 前端代码生成计划 - Plan 4: 需求管理核心模块

## 计划信息
- **计划编号**: Plan 4
- **项目名称**: 需求跟踪管理系统 - 前端
- **目标**: 实现需求管理页面
- **依赖**: Plan 1-3

---

## 1. 目标描述

实现需求管理的核心前端页面：
- 需求列表页面（表格展示、筛选、排序、分页）
- 需求创建/编辑弹窗
- 需求详情页面
- 需求状态变更功能
- 需求删除确认

---

## 2. 文件清单

### 2.1 页面文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/app/requirements/page.tsx` | 创建 | 需求列表页面 |
| `src/app/requirements/[id]/page.tsx` | 创建 | 需求详情页面 |

### 2.2 组件文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/components/requirements/RequirementForm.tsx` | 创建 | 需求表单组件 |
| `src/components/requirements/RequirementTable.tsx` | 创建 | 需求表格组件 |
| `src/components/requirements/StatusBadge.tsx` | 创建 | 状态徽章组件 |
| `src/components/requirements/StatusChangeDialog.tsx` | 创建 | 状态变更弹窗 |
| `src/components/requirements/RequirementFilters.tsx` | 创建 | 需求筛选组件 |

### 2.3 UI组件（需要补充）

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/components/ui/popover.tsx` | 创建 | 弹出层组件 |
| `src/components/ui/calendar.tsx` | 创建 | 日历组件 |
| `src/components/ui/progress.tsx` | 创建 | 进度条组件 |

---

## 3. 实现细节

### 3.1 src/components/ui/popover.tsx

```typescript
"use client"

import * as React from "react"
import * as PopoverPrimitive from "@radix-ui/react-popover"

import { cn } from "@/src/lib/utils"

const Popover = PopoverPrimitive.Root
const PopoverTrigger = PopoverPrimitive.Trigger
const PopoverAnchor = PopoverPrimitive.Anchor

const PopoverContent = React.forwardRef<
  React.ElementRef<typeof PopoverPrimitive.Content>,
  React.ComponentPropsWithoutRef<typeof PopoverPrimitive.Content>
>(({ className, align = "center", sideOffset = 4, ...props }, ref) => (
  <PopoverPrimitive.Portal>
    <PopoverPrimitive.Content
      ref={ref}
      align={align}
      sideOffset={sideOffset}
      className={cn(
        "z-50 w-72 rounded-md border bg-popover p-4 text-popover-foreground shadow-md outline-none data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 data-[side=bottom]:slide-in-from-top-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2",
        className
      )}
      {...props}
    />
  </PopoverPrimitive.Portal>
))
PopoverContent.displayName = PopoverPrimitive.Content.displayName

export { Popover, PopoverTrigger, PopoverContent, PopoverAnchor }
```

### 3.2 src/components/ui/calendar.tsx

```typescript
"use client"

import * as React from "react"
import { ChevronLeft, ChevronRight } from "lucide-react"
import { DayPicker } from "react-day-picker"

import { cn } from "@/src/lib/utils"
import { buttonVariants } from "@/src/components/ui/button"

export type CalendarProps = React.ComponentProps<typeof DayPicker>

function Calendar({
  className,
  classNames,
  showOutsideDays = true,
  ...props
}: CalendarProps) {
  return (
    <DayPicker
      showOutsideDays={showOutsideDays}
      className={cn("p-3", className)}
      classNames={{
        months: "flex flex-col sm:flex-row space-y-4 sm:space-x-4 sm:space-y-0",
        month: "space-y-4",
        caption: "flex justify-center pt-1 relative items-center",
        caption_label: "text-sm font-medium",
        nav: "space-x-1 flex items-center",
        nav_button: cn(
          buttonVariants({ variant: "outline" }),
          "h-7 w-7 bg-transparent p-0 opacity-50 hover:opacity-100"
        ),
        nav_button_previous: "absolute left-1",
        nav_button_next: "absolute right-1",
        table: "w-full border-collapse space-y-1",
        head_row: "flex",
        head_cell:
          "text-muted-foreground rounded-md w-9 font-normal text-[0.8rem]",
        row: "flex w-full mt-2",
        cell: "h-9 w-9 text-center text-sm p-0 relative [&:has([aria-selected].day-range-end)]:rounded-r-md [&:has([aria-selected].day-outside)]:bg-accent/50 [&:has([aria-selected])]:bg-accent first:[&:has([aria-selected])]:rounded-l-md last:[&:has([aria-selected])]:rounded-r-md focus-within:relative focus-within:z-20",
        day: cn(
          buttonVariants({ variant: "ghost" }),
          "h-9 w-9 p-0 font-normal aria-selected:opacity-100"
        ),
        day_range_end: "day-range-end",
        day_selected:
          "bg-primary text-primary-foreground hover:bg-primary hover:text-primary-foreground focus:bg-primary focus:text-primary-foreground",
        day_today: "bg-accent text-accent-foreground",
        day_outside:
          "day-outside text-muted-foreground opacity-50 aria-selected:bg-accent/50 aria-selected:text-muted-foreground aria-selected:opacity-30",
        day_disabled: "text-muted-foreground opacity-50",
        day_range_middle:
          "aria-selected:bg-accent aria-selected:text-accent-foreground",
        day_hidden: "invisible",
        ...classNames,
      }}
      components={{
        IconLeft: ({ ...props }) => <ChevronLeft className="h-4 w-4" />,
        IconRight: ({ ...props }) => <ChevronRight className="h-4 w-4" />,
      }}
      {...props}
    />
  )
}
Calendar.displayName = "Calendar"

export { Calendar }
```

### 3.3 src/components/ui/progress.tsx

```typescript
"use client"

import * as React from "react"
import * as ProgressPrimitive from "@radix-ui/react-progress"

import { cn } from "@/src/lib/utils"

const Progress = React.forwardRef<
  React.ElementRef<typeof ProgressPrimitive.Root>,
  React.ComponentPropsWithoutRef<typeof ProgressPrimitive.Root>
>(({ className, value, ...props }, ref) => (
  <ProgressPrimitive.Root
    ref={ref}
    className={cn(
      "relative h-4 w-full overflow-hidden rounded-full bg-secondary",
      className
    )}
    {...props}
  >
    <ProgressPrimitive.Indicator
      className="h-full w-full flex-1 bg-primary transition-all"
      style={{ transform: `translateX(-${100 - (value || 0)}%)` }}
    />
  </ProgressPrimitive.Root>
))
Progress.displayName = ProgressPrimitive.Root.displayName

export { Progress }
```

### 3.4 src/components/requirements/StatusBadge.tsx

```typescript
import { Badge } from '@/src/components/ui/badge';
import { REQUIREMENT_STATUSES, PRIORITIES } from '@/src/lib/constants';
import { RequirementStatus, Priority } from '@/src/types';

interface StatusBadgeProps {
  status: RequirementStatus;
}

export function StatusBadge({ status }: StatusBadgeProps) {
  const config = REQUIREMENT_STATUSES[status];
  return (
    <Badge className={config?.color || ''} variant="outline">
      {config?.label || status}
    </Badge>
  );
}

interface PriorityBadgeProps {
  priority: Priority;
}

export function PriorityBadge({ priority }: PriorityBadgeProps) {
  const config = PRIORITIES[priority];
  return (
    <Badge className={config?.color || ''} variant="outline">
      {config?.label || priority}
    </Badge>
  );
}
```

### 3.5 src/components/requirements/RequirementFilters.tsx

```typescript
'use client';

import { Button } from '@/src/components/ui/button';
import { Input } from '@/src/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/src/components/ui/select';
import { REQUIREMENT_STATUSES, PRIORITIES } from '@/src/lib/constants';
import { Search, X } from 'lucide-react';

interface RequirementFiltersProps {
  keyword: string;
  onKeywordChange: (value: string) => void;
  status: string;
  onStatusChange: (value: string) => void;
  priority: string;
  onPriorityChange: (value: string) => void;
  projectId: string;
  onProjectIdChange: (value: string) => void;
  followerId: string;
  onFollowerIdChange: (value: string) => void;
  projects: { id: string; name: string }[];
  users: { id: string; name: string }[];
  onReset: () => void;
}

export function RequirementFilters({
  keyword,
  onKeywordChange,
  status,
  onStatusChange,
  priority,
  onPriorityChange,
  projectId,
  onProjectIdChange,
  followerId,
  onFollowerIdChange,
  projects,
  users,
  onReset,
}: RequirementFiltersProps) {
  const hasFilters = keyword || status || priority || projectId || followerId;

  return (
    <div className="flex flex-wrap items-center gap-4">
      <div className="relative w-64">
        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          placeholder="搜索需求名称、编号..."
          value={keyword}
          onChange={(e) => onKeywordChange(e.target.value)}
          className="pl-10"
        />
      </div>

      <Select value={status} onValueChange={onStatusChange}>
        <SelectTrigger className="w-40">
          <SelectValue placeholder="全部状态" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="">全部状态</SelectItem>
          {Object.values(REQUIREMENT_STATUSES).map((s) => (
            <SelectItem key={s.value} value={s.value}>
              {s.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>

      <Select value={priority} onValueChange={onPriorityChange}>
        <SelectTrigger className="w-32">
          <SelectValue placeholder="全部优先级" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="">全部优先级</SelectItem>
          {Object.values(PRIORITIES).map((p) => (
            <SelectItem key={p.value} value={p.value}>
              {p.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>

      <Select value={projectId} onValueChange={onProjectIdChange}>
        <SelectTrigger className="w-40">
          <SelectValue placeholder="全部项目" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="">全部项目</SelectItem>
          {projects.map((p) => (
            <SelectItem key={p.id} value={p.id}>
              {p.name}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>

      <Select value={followerId} onValueChange={onFollowerIdChange}>
        <SelectTrigger className="w-40">
          <SelectValue placeholder="全部跟进人" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="">全部跟进人</SelectItem>
          {users.map((u) => (
            <SelectItem key={u.id} value={u.id}>
              {u.name}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>

      {hasFilters && (
        <Button variant="ghost" size="sm" onClick={onReset}>
          <X className="mr-1 h-4 w-4" />
          清除筛选
        </Button>
      )}
    </div>
  );
}
```

### 3.6 src/components/requirements/RequirementTable.tsx

```typescript
'use client';

import Link from 'next/link';
import { Requirement } from '@/src/types';
import { Button } from '@/src/components/ui/button';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/src/components/ui/table';
import { Progress } from '@/src/components/ui/progress';
import { StatusBadge, PriorityBadge } from './StatusBadge';
import { Pencil, Trash2, ArrowRight } from 'lucide-react';
import { formatDate } from '@/src/lib/utils';

interface RequirementTableProps {
  requirements: Requirement[];
  onEdit: (req: Requirement) => void;
  onDelete: (req: Requirement) => void;
}

export function RequirementTable({ requirements, onEdit, onDelete }: RequirementTableProps) {
  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>需求号</TableHead>
            <TableHead>需求名称</TableHead>
            <TableHead>状态</TableHead>
            <TableHead>优先级</TableHead>
            <TableHead>进度</TableHead>
            <TableHead>跟进人</TableHead>
            <TableHead>所属项目</TableHead>
            <TableHead>计划交测</TableHead>
            <TableHead>计划上线</TableHead>
            <TableHead className="text-right">操作</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {requirements.length === 0 ? (
            <TableRow>
              <TableCell colSpan={10} className="text-center py-8 text-muted-foreground">
                暂无数据
              </TableCell>
            </TableRow>
          ) : (
            requirements.map((req) => (
              <TableRow key={req.id}>
                <TableCell className="font-medium">{req.requirementNo}</TableCell>
                <TableCell>
                  <Link
                    href={`/requirements/${req.id}`}
                    className="hover:underline text-primary"
                  >
                    {req.title}
                  </Link>
                </TableCell>
                <TableCell>
                  <StatusBadge status={req.status} />
                </TableCell>
                <TableCell>
                  <PriorityBadge priority={req.priority} />
                </TableCell>
                <TableCell>
                  <div className="flex items-center gap-2">
                    <Progress value={req.progress} className="w-16 h-2" />
                    <span className="text-xs">{req.progress}%</span>
                  </div>
                </TableCell>
                <TableCell>{req.follower?.name}</TableCell>
                <TableCell>{req.project?.name}</TableCell>
                <TableCell>{formatDate(req.plannedTestDate)}</TableCell>
                <TableCell>{formatDate(req.plannedReleaseDate)}</TableCell>
                <TableCell className="text-right">
                  <div className="flex justify-end gap-2">
                    <Button
                      variant="ghost"
                      size="icon"
                      asChild
                    >
                      <Link href={`/requirements/${req.id}`}>
                        <ArrowRight className="h-4 w-4" />
                      </Link>
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => onEdit(req)}
                      title="编辑"
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => onDelete(req)}
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

### 3.7 src/components/requirements/StatusChangeDialog.tsx

```typescript
'use client';

import { useState } from 'react';
import { Button } from '@/src/components/ui/button';
import { Textarea } from '@/src/components/ui/textarea';
import { Label } from '@/src/components/ui/label';
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
import { Requirement, RequirementStatus } from '@/src/types';
import { REQUIREMENT_STATUSES } from '@/src/lib/constants';

// 状态流转规则（简化版，实际应从后端获取）
const STATUS_FLOW: Record<RequirementStatus, RequirementStatus[]> = {
  PENDING_CONFIRM: ['CONFIRMED'],
  CONFIRMED: ['PENDING_QUOTE'],
  PENDING_QUOTE: ['QUOTED'],
  QUOTED: ['PENDING_DEVELOP'],
  PENDING_DEVELOP: ['DEVELOPING'],
  DEVELOPING: ['TESTING'],
  TESTING: ['ACCEPTED_PENDING_RELEASE'],
  ACCEPTED_PENDING_RELEASE: ['RELEASED'],
  RELEASED: [],
};

interface StatusChangeDialogProps {
  requirement: Requirement | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (requirementId: string, status: RequirementStatus, remark?: string) => Promise<void>;
}

export function StatusChangeDialog({
  requirement,
  open,
  onOpenChange,
  onSubmit,
}: StatusChangeDialogProps) {
  const [newStatus, setNewStatus] = useState<RequirementStatus>('');
  const [remark, setRemark] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const availableStatuses = requirement ? STATUS_FLOW[requirement.status] : [];

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!requirement || !newStatus) return;

    setIsLoading(true);
    try {
      await onSubmit(requirement.id, newStatus, remark || undefined);
      setNewStatus('');
      setRemark('');
      onOpenChange(false);
    } finally {
      setIsLoading(false);
    }
  };

  if (!requirement) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[400px]">
        <DialogHeader>
          <DialogTitle>变更需求状态</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label>当前状态</Label>
            <div className="p-2 bg-muted rounded">
              {REQUIREMENT_STATUSES[requirement.status]?.label}
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="newStatus">新状态 *</Label>
            <Select
              value={newStatus}
              onValueChange={(value) => setNewStatus(value as RequirementStatus)}
            >
              <SelectTrigger>
                <SelectValue placeholder="选择新状态" />
              </SelectTrigger>
              <SelectContent>
                {availableStatuses.map((status) => (
                  <SelectItem key={status} value={status}>
                    {REQUIREMENT_STATUSES[status]?.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="remark">备注</Label>
            <Textarea
              id="remark"
              value={remark}
              onChange={(e) => setRemark(e.target.value)}
              placeholder="填写状态变更原因或备注信息"
              rows={3}
            />
          </div>

          <div className="flex justify-end space-x-2">
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              取消
            </Button>
            <Button type="submit" disabled={isLoading || !newStatus}>
              {isLoading ? '保存中...' : '确认变更'}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
```

由于篇幅限制，我将继续生成剩余的计划文件。让我继续生成需求表单组件和需求页面。