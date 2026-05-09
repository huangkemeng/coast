# 前端代码生成计划 - Plan 5: 通知日志模块与部署配置

## 计划信息
- **计划编号**: Plan 5
- **项目名称**: 需求跟踪管理系统 - 前端
- **目标**: 实现通知日志页面和部署配置
- **依赖**: Plan 1-4

---

## 1. 目标描述

实现通知日志功能的前端页面和项目部署配置：
- 通知日志列表页面（筛选、分页）
- 通知重发功能
- 项目构建和部署配置

---

## 2. 文件清单

### 2.1 页面文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/app/notifications/page.tsx` | 创建 | 通知日志列表页面 |

### 2.2 组件文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/components/notifications/NotificationTable.tsx` | 创建 | 通知日志表格组件 |
| `src/components/notifications/NotificationFilters.tsx` | 创建 | 通知筛选组件 |

### 2.3 配置文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `vercel.json` | 创建 | Vercel部署配置 |

---

## 3. 实现细节

### 3.1 src/components/notifications/NotificationFilters.tsx

```typescript
'use client';

import { Button } from '@/src/components/ui/button';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/src/components/ui/select';
import { NOTIFICATION_TYPES, NOTIFICATION_STATUSES } from '@/src/lib/constants';
import { X } from 'lucide-react';

interface NotificationFiltersProps {
  type: string;
  onTypeChange: (value: string) => void;
  status: string;
  onStatusChange: (value: string) => void;
  onReset: () => void;
}

export function NotificationFilters({
  type,
  onTypeChange,
  status,
  onStatusChange,
  onReset,
}: NotificationFiltersProps) {
  const hasFilters = type || status;

  return (
    <div className="flex items-center gap-4">
      <Select value={type} onValueChange={onTypeChange}>
        <SelectTrigger className="w-40">
          <SelectValue placeholder="全部类型" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="">全部类型</SelectItem>
          {Object.values(NOTIFICATION_TYPES).map((t) => (
            <SelectItem key={t.value} value={t.value}>
              {t.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>

      <Select value={status} onValueChange={onStatusChange}>
        <SelectTrigger className="w-40">
          <SelectValue placeholder="全部状态" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="">全部状态</SelectItem>
          {Object.values(NOTIFICATION_STATUSES).map((s) => (
            <SelectItem key={s.value} value={s.value}>
              {s.label}
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

### 3.2 src/components/notifications/NotificationTable.tsx

```typescript
'use client';

import { NotificationLog } from '@/src/types';
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
import { NOTIFICATION_TYPES, NOTIFICATION_STATUSES } from '@/src/lib/constants';
import { RefreshCw } from 'lucide-react';
import { formatDateTime } from '@/src/lib/utils';

interface NotificationTableProps {
  notifications: NotificationLog[];
  onResend: (notification: NotificationLog) => void;
  isResending: string | null;
}

export function NotificationTable({ notifications, onResend, isResending }: NotificationTableProps) {
  const getTypeLabel = (type: string) => {
    return NOTIFICATION_TYPES[type as keyof typeof NOTIFICATION_TYPES]?.label || type;
  };

  const getStatusBadge = (status: string) => {
    const config = NOTIFICATION_STATUSES[status as keyof typeof NOTIFICATION_STATUSES];
    return (
      <Badge className={config?.color || ''} variant="outline">
        {config?.label || status}
      </Badge>
    );
  };

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>时间</TableHead>
            <TableHead>类型</TableHead>
            <TableHead>需求</TableHead>
            <TableHead>机器人</TableHead>
            <TableHead>状态</TableHead>
            <TableHead>失败原因</TableHead>
            <TableHead className="text-right">操作</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {notifications.length === 0 ? (
            <TableRow>
              <TableCell colSpan={7} className="text-center py-8 text-muted-foreground">
                暂无数据
              </TableCell>
            </TableRow>
          ) : (
            notifications.map((notification) => (
              <TableRow key={notification.id}>
                <TableCell>{formatDateTime(notification.createdAt)}</TableCell>
                <TableCell>{getTypeLabel(notification.type)}</TableCell>
                <TableCell>
                  {notification.requirement ? (
                    <div>
                      <div className="font-medium">{notification.requirement.title}</div>
                      <div className="text-xs text-muted-foreground">
                        {notification.requirement.requirementNo}
                      </div>
                    </div>
                  ) : (
                    '-'
                  )}
                </TableCell>
                <TableCell>{notification.bot?.name || '-'}</TableCell>
                <TableCell>{getStatusBadge(notification.status)}</TableCell>
                <TableCell className="max-w-xs truncate text-red-500">
                  {notification.errorMsg || '-'}
                </TableCell>
                <TableCell className="text-right">
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => onResend(notification)}
                    disabled={isResending === notification.id}
                    title="重发"
                  >
                    <RefreshCw className={`h-4 w-4 ${isResending === notification.id ? 'animate-spin' : ''}`} />
                  </Button>
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

### 3.3 src/app/notifications/page.tsx

```typescript
'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/src/hooks/useAuth';
import { MainLayout } from '@/src/components/layout/MainLayout';
import { Button } from '@/src/components/ui/button';
import { NotificationTable } from '@/src/components/notifications/NotificationTable';
import { NotificationFilters } from '@/src/components/notifications/NotificationFilters';
import api from '@/src/lib/api';
import { NotificationLog } from '@/src/types';

export default function NotificationsPage() {
  const router = useRouter();
  const { user, isLoading } = useAuth();
  const [notifications, setNotifications] = useState<NotificationLog[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(20);

  // 筛选条件
  const [type, setType] = useState('');
  const [status, setStatus] = useState('');

  // 重发状态
  const [isResending, setIsResending] = useState<string | null>(null);

  useEffect(() => {
    if (!isLoading && !user) {
      router.push('/');
      return;
    }
    if (user) {
      fetchNotifications();
    }
  }, [user, isLoading, router, page, type, status]);

  const fetchNotifications = async () => {
    try {
      const params: any = { page, pageSize };
      if (type) params.type = type;
      if (status) params.status = status;

      const response = await api.get('/notifications', { params });
      if (response.success) {
        setNotifications(response.data.items);
        setTotal(response.data.total);
      }
    } catch (error) {
      console.error('Failed to fetch notifications:', error);
    }
  };

  const handleResend = async (notification: NotificationLog) => {
    setIsResending(notification.id);
    try {
      const response = await api.post(`/notifications/${notification.id}/resend`);
      if (response.success) {
        alert('重发成功');
        fetchNotifications();
      } else {
        alert(response.error || '重发失败');
      }
    } catch (error) {
      alert('重发失败');
    } finally {
      setIsResending(null);
    }
  };

  const handleResetFilters = () => {
    setType('');
    setStatus('');
    setPage(1);
  };

  if (isLoading || !user) {
    return null;
  }

  return (
    <MainLayout>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold">通知日志</h1>
          <p className="text-muted-foreground mt-1">
            查看所有发送的企业微信通知记录
          </p>
        </div>

        <NotificationFilters
          type={type}
          onTypeChange={setType}
          status={status}
          onStatusChange={setStatus}
          onReset={handleResetFilters}
        />

        <NotificationTable
          notifications={notifications}
          onResend={handleResend}
          isResending={isResending}
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
    </MainLayout>
  );
}
```

### 3.4 vercel.json

```json
{
  "version": 2,
  "builds": [
    {
      "src": "package.json",
      "use": "@vercel/next"
    }
  ],
  "routes": [
    {
      "src": "/api/(.*)",
      "dest": "${NEXT_PUBLIC_API_URL}/$1"
    },
    {
      "src": "/(.*)",
      "dest": "/$1"
    }
  ],
  "env": {
    "NEXT_PUBLIC_API_URL": "@next_public_api_url"
  }
}
```

### 3.5 package.json 更新（添加date-fns依赖）

在 frontend_plan_01_scaffold.md 的 package.json 中添加：

```json
"date-fns": "^3.0.0",
```

---

## 4. 验证方法

1. 启动前后端服务
2. 访问通知日志页面 `/notifications`
3. 测试筛选功能
4. 测试通知重发功能（需要失败的记录）
5. 运行 `npm run build` 检查构建是否成功

---

## 5. 交付清单

- [ ] 通知日志页面可用
- [ ] 通知筛选功能正常
- [ ] 通知重发功能正常
- [ ] 项目构建成功
- [ ] 无 TypeScript 编译错误

---

## 6. 前端项目总结

至此，前端项目所有功能模块已完成：

| Plan | 模块 | 功能 |
|------|------|------|
| Plan 1 | 项目脚手架 | Next.js + TailwindCSS + Radix UI 初始化 |
| Plan 2 | 用户管理 | 用户列表、创建、编辑、删除、密码修改 |
| Plan 3 | 项目与机器人 | 项目管理、机器人配置、Webhook测试 |
| Plan 4 | 需求管理 | 需求CRUD、状态流转、筛选排序、详情页 |
| Plan 5 | 通知日志 | 通知记录查询、重发、部署配置 |

### 环境变量配置

```env
NEXT_PUBLIC_API_URL=http://localhost:3001/api
```

### 部署说明

1. 构建项目：`npm run build`
2. 部署到Vercel：
   - 连接GitHub仓库
   - 设置环境变量 `NEXT_PUBLIC_API_URL`
   - 自动部署

### 项目结构

```
frontend/
├── src/
│   ├── app/                    # Next.js App Router
│   │   ├── api/               # API路由（代理到后端）
│   │   ├── dashboard/         # 仪表盘
│   │   ├── requirements/      # 需求管理
│   │   ├── projects/          # 项目管理
│   │   ├── bots/              # 机器人配置
│   │   ├── users/             # 用户管理
│   │   ├── notifications/     # 通知日志
│   │   ├── globals.css        # 全局样式
│   │   ├── layout.tsx         # 根布局
│   │   └── page.tsx           # 登录页
│   ├── components/
│   │   ├── ui/                # UI组件库
│   │   ├── layout/            # 布局组件
│   │   ├── requirements/      # 需求相关组件
│   │   ├── projects/          # 项目相关组件
│   │   ├── bots/              # 机器人相关组件
│   │   ├── users/             # 用户相关组件
│   │   └── notifications/     # 通知相关组件
│   ├── contexts/
│   │   └── AuthContext.tsx    # 认证上下文
│   ├── hooks/
│   │   └── useAuth.ts         # 认证Hook
│   ├── lib/
│   │   ├── utils.ts           # 工具函数
│   │   ├── api.ts             # API封装
│   │   └── constants.ts       # 常量定义
│   └── types/
│       └── index.ts           # TypeScript类型
├── public/                     # 静态资源
├── package.json
├── tsconfig.json
├── tailwind.config.ts
├── next.config.js
└── vercel.json
```

### 技术栈

- **框架**: Next.js 14 (App Router)
- **语言**: TypeScript
- **样式**: TailwindCSS + tailwindcss-animate
- **UI组件**: Radix UI
- **图标**: Lucide React
- **日期**: date-fns
- **HTTP**: Axios
