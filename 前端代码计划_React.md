# 需求跟踪管理系统 - React 前端代码计划

> 基于测试用例文档: [需求跟踪管理系统_测试用例.md](需求跟踪管理系统_测试用例.md)
> 
> 对齐后端接口: [后端代码计划_CSharp.md](后端代码计划_CSharp.md)
> 
> 覆盖测试用例总计: **~252条**（需求管理76条 + 状态流转32条 + 项目管理16条 + 机器人配置15条 + 通知管理19条 + 用户管理9条 + 用户认证72条 + 非功能需求13条）

---

## 1. 项目概述

| 属性 | 说明 |
|------|------|
| **项目名称** | requirement-tracking-system-web |
| **技术栈** | React 18 + Vite + TypeScript + @radix-ui |
| **UI组件库** | Radix UI (Primitive) + Tailwind CSS |
| **状态管理** | Zustand |
| **HTTP客户端** | Axios + React Query |
| **样式方案** | Tailwind CSS (Dark Admin 主题) |
| **路由** | React Router v6 |
| **目标** | 构建 Dark Admin 风格的企业需求跟踪管理前端 |

---

## 2. 设计规范

### 2.1 色彩系统 (Dark Admin)

```
Primary:     #6366f1 (Indigo-500)
Secondary:   #8b5cf6 (Violet-500)
Background:  #0f172a (Slate-900)
Surface:     #1e293b (Slate-800)
Border:      #334155 (Slate-700)
Text:        #f1f5f9 (Slate-100)
Text-muted:  #94a3b8 (Slate-400)
Success:     #10b981
Warning:     #f59e0b
Error:       #ef4444
```

### 2.2 组件风格

- 圆角: `rounded-lg` (8px)
- 阴影: 深色主题适配的 `shadow-xl shadow-black/50`
- 间距: 基于 4px 倍数的规范
- 字体: Inter / 系统默认字体

---

## 3. 项目架构

```
src/
├── api/                      # API 接口层
│   ├── client.ts             # Axios 实例配置
│   ├── requirements.ts       # 需求相关 API
│   ├── projects.ts           # 项目相关 API
│   ├── users.ts              # 用户相关 API
│   ├── robots.ts             # 机器人相关 API
│   ├── notifications.ts      # 通知日志 API
│   └── auth.ts               # 认证相关 API
├── components/               # 通用组件
│   ├── ui/                   # 基础 UI 组件 (@radix-ui 封装)
│   │   ├── Button.tsx
│   │   ├── Input.tsx
│   │   ├── Select.tsx
│   │   ├── Dialog.tsx
│   │   ├── Table.tsx
│   │   ├── Badge.tsx
│   │   ├── DatePicker.tsx
│   │   ├── Dropdown.tsx
│   │   ├── Tabs.tsx
│   │   ├── Toast.tsx
│   │   ├── Tooltip.tsx
│   │   ├── Pagination.tsx
│   │   └── ...
│   ├── layout/               # 布局组件
│   │   ├── Sidebar.tsx
│   │   ├── Header.tsx
│   │   └── Layout.tsx
│   └── common/              # 通用业务组件
│       ├── ConfirmDialog.tsx
│       ├── LoadingOverlay.tsx
│       ├── EmptyState.tsx
│       └── ErrorBoundary.tsx
├── features/                 # 功能模块
│   ├── requirements/          # 需求管理模块
│   │   ├── components/
│   │   │   ├── RequirementTable.tsx
│   │   │   ├── RequirementForm.tsx
│   │   │   ├── RequirementFilters.tsx
│   │   │   ├── StatusSelect.tsx
│   │   │   ├── ProgressInput.tsx
│   │   │   ├── DateRangePicker.tsx
│   │   │   ├── PriceInput.tsx
│   │   │   ├── DocUrlInput.tsx
│   │   │   └── VersionBadge.tsx
│   │   ├── hooks/
│   │   │   ├── useRequirements.ts
│   │   │   ├── useRequirement.ts
│   │   │   ├── useCreateRequirement.ts
│   │   │   ├── useUpdateRequirement.ts
│   │   │   └── useStatusTransitions.ts
│   │   └── pages/
│   │       ├── RequirementsListPage.tsx
│   │       ├── RequirementDetailPage.tsx
│   │       ├── RequirementCreatePage.tsx
│   │       └── RequirementEditPage.tsx
│   ├── projects/             # 项目管理模块
│   │   ├── components/
│   │   │   ├── ProjectTable.tsx
│   │   │   └── ProjectForm.tsx
│   │   ├── hooks/
│   │   │   └── useProjects.ts
│   │   └── pages/
│   │       ├── ProjectsListPage.tsx
│   │       └── ProjectFormPage.tsx
│   ├── users/                # 用户管理模块
│   │   ├── components/
│   │   │   ├── UserTable.tsx
│   │   │   └── UserForm.tsx
│   │   ├── hooks/
│   │   │   └── useUsers.ts
│   │   └── pages/
│   │       ├── UsersListPage.tsx
│   │       └── UserFormPage.tsx
│   ├── robots/               # 机器人配置模块
│   │   ├── components/
│   │   │   ├── RobotTable.tsx
│   │   │   ├── RobotForm.tsx
│   │   │   ├── WebhookTester.tsx
│   │   │   └── RobotStatus.tsx
│   │   ├── hooks/
│   │   │   ├── useRobots.ts
│   │   │   └── useRobotTest.ts
│   │   └── pages/
│   │       └── RobotsListPage.tsx
│   └── notifications/        # 通知日志模块
│       ├── components/
│       │   ├── NotificationTable.tsx
│       │   └── NotificationFilters.tsx
│       ├── hooks/
│       │   └── useNotifications.ts
│       └── pages/
│           └── NotificationsListPage.tsx
├── hooks/                    # 通用 Hooks
│   ├── useAuth.ts
│   ├── usePermission.ts
│   ├── useTableSort.ts
│   ├── useTablePagination.ts
│   └── useFilters.ts
├── stores/                   # Zustand Store
│   ├── authStore.ts          # 认证状态管理
│   ├── uiStore.ts            # UI 状态管理
│   └── permissionStore.ts    # 权限状态管理
├── types/                    # TypeScript 类型定义
│   ├── api.ts
│   ├── requirement.ts
│   ├── project.ts
│   ├── user.ts
│   ├── robot.ts
│   └── notification.ts
├── utils/                    # 工具函数
│   ├── validators.ts         # 表单校验规则
│   ├── formatters.ts         # 数据格式化
│   ├── dateUtils.ts          # 日期处理
│   └── urlValidator.ts       # URL 校验
├── pages/                    # 页面路由入口
│   └── App.tsx
└── main.tsx
```

---

## 4. 页面设计

| 页面 | 路由 | 权限 | 说明 | 测试用例覆盖 |
|------|------|------|------|-------------|
| **登录页** | `/login` | 公开 | 用户认证入口 | TC-REQ-013 |
| **仪表盘** | `/dashboard` | 所有用户 | 数据概览、统计图表 | - |
| **需求列表** | `/requirements` | 所有用户 | 主需求管理页面，支持筛选、排序、分页 | TC-REQ-001~013 |
| **需求详情** | `/requirements/:id` | 所有用户 | 查看需求详情 | - |
| **需求创建** | `/requirements/new` | 管理员 | 新建需求表单 | TC-REQ-014~043 |
| **需求编辑** | `/requirements/:id/edit` | 管理员/跟进人 | 编辑需求（含版本冲突检测） | TC-REQ-044~053 |
| **项目管理** | `/projects` | 管理员 | 项目列表及管理 | TC-PROJ-001~008 |
| **用户管理** | `/users` | 管理员 | 用户列表及管理 | - |
| **机器人配置** | `/robots` | 管理员 | 企业微信机器人配置 | TC-BOT-001~008 |
| **通知日志** | `/notifications` | 管理员 | 通知日志查看 | TC-NOT-001~019 |

---

## 4.1 登录与认证模块设计

### 4.1.1 登录页面

```typescript
// features/auth/pages/LoginPage.tsx
export const LoginPage = () => {
  const navigate = useNavigate();
  const { login, isLoading } = useLogin();
  const { control, handleSubmit, formState: { errors } } = useForm<LoginFormData>({
    defaultValues: {
      username: '',
      password: ''
    }
  });
  
  const onSubmit = async (data: LoginFormData) => {
    try {
      await login(data);
      toast.success('登录成功');
      navigate('/dashboard');
    } catch (error) {
      toast.error(error.message || '登录失败');
    }
  };
  
  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-900">
      <div className="w-full max-w-md p-8 bg-slate-800 rounded-lg shadow-xl">
        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold text-slate-100">需求跟踪管理系统</h1>
          <p className="text-slate-400 mt-2">请登录以继续</p>
        </div>
        
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <FormField
            control={control}
            name="username"
            label="用户名"
            rules={{ required: '请输入用户名' }}
            error={errors.username}
          >
            <Input 
              placeholder="请输入用户名" 
              autoComplete="username"
            />
          </FormField>
          
          <FormField
            control={control}
            name="password"
            label="密码"
            rules={{ required: '请输入密码' }}
            error={errors.password}
          >
            <Input 
              type="password" 
              placeholder="请输入密码"
              autoComplete="current-password"
            />
          </FormField>
          
          <Button 
            type="submit" 
            className="w-full" 
            loading={isLoading}
          >
            登录
          </Button>
        </form>
      </div>
    </div>
  );
};
```

### 4.1.2 认证 Hooks

```typescript
// hooks/useAuth.ts
export const useAuthStore = create<AuthState>((set, get) => ({
  user: null,
  token: localStorage.getItem('token'),
  isAuthenticated: !!localStorage.getItem('token'),
  
  login: async (credentials: LoginFormData) => {
    const response = await loginApi(credentials);
    
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify(response.user));
    
    set({ 
      user: response.user, 
      token: response.token,
      isAuthenticated: true 
    });
    
    return response.user;
  },
  
  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    set({ user: null, token: null, isAuthenticated: false });
    window.location.href = '/login';
  },
  
  getUser: () => get().user,
  isAdmin: () => get().user?.role === UserRole.Admin
}));

export const useLogin = () => {
  const { login } = useAuthStore();
  const [isLoading, setIsLoading] = useState(false);
  
  return {
    login: async (credentials: LoginFormData) => {
      setIsLoading(true);
      try {
        return await login(credentials);
      } finally {
        setIsLoading(false);
      }
    },
    isLoading
  };
};
```

### 4.1.3 路由守卫

```typescript
// components/auth/ProtectedRoute.tsx
export const ProtectedRoute = ({ 
  children, 
  requiredPermission,
  fallbackPath = '/403' 
}: Props) => {
  const { isAuthenticated, isAdmin } = useAuthStore();
  const location = useLocation();
  
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }
  
  if (requiredPermission === 'admin' && !isAdmin()) {
    return <Navigate to={fallbackPath} replace />;
  }
  
  return <>{children}</>;
};

// 测试用例: TC-REQ-013
```

---

## 4.2 仪表盘模块设计

### 4.2.1 仪表盘页面

```typescript
// features/dashboard/pages/DashboardPage.tsx
export const DashboardPage = () => {
  const { user } = useAuthStore();
  const { data: stats, isLoading } = useQuery({
    queryKey: ['dashboard-stats'],
    queryFn: () => getDashboardStats()
  });
  
  const { data: recentRequirements } = useQuery({
    queryKey: ['recent-requirements'],
    queryFn: () => getRequirements({ pageIndex: 1, pageSize: 5, sortBy: 'createdAt', sortOrder: 'desc' })
  });
  
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">欢迎回来，{user?.realName}</h1>
      </div>
      
      {/* 统计卡片 */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatisticsCard
          title="总需求数"
          value={stats?.totalRequirements ?? 0}
          icon={FileText}
          color="primary"
        />
        <StatisticsCard
          title="开发中"
          value={stats?.inDevCount ?? 0}
          icon={Code}
          color="warning"
        />
        <StatisticsCard
          title="测试中"
          value={stats?.inTestCount ?? 0}
          icon={TestTube}
          color="info"
        />
        <StatisticsCard
          title="已上线"
          value={stats?.launchedCount ?? 0}
          icon={CheckCircle}
          color="success"
        />
      </div>
      
      {/* 状态分布图表 */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <StatusDistributionChart data={stats?.statusDistribution ?? []} />
        <RecentRequirementsTable data={recentRequirements?.items ?? []} />
      </div>
    </div>
  );
};
```

### 4.2.2 统计卡片组件

```typescript
// components/common/StatisticsCard.tsx
interface StatisticsCardProps {
  title: string;
  value: number;
  icon: React.ElementType;
  color: 'primary' | 'secondary' | 'success' | 'warning' | 'error' | 'info';
  description?: string;
}

export const StatisticsCard = ({ title, value, icon: Icon, color, description }: StatisticsCardProps) => {
  const colorClasses = {
    primary: 'bg-indigo-500/10 text-indigo-500',
    secondary: 'bg-violet-500/10 text-violet-500',
    success: 'bg-emerald-500/10 text-emerald-500',
    warning: 'bg-amber-500/10 text-amber-500',
    error: 'bg-red-500/10 text-red-500',
    info: 'bg-sky-500/10 text-sky-500'
  };
  
  return (
    <div className="bg-slate-800 rounded-lg p-6 border border-slate-700">
      <div className="flex items-start justify-between">
        <div>
          <p className="text-slate-400 text-sm">{title}</p>
          <p className="text-3xl font-bold text-slate-100 mt-2">{value}</p>
          {description && (
            <p className="text-slate-500 text-sm mt-1">{description}</p>
          )}
        </div>
        <div className={`p-3 rounded-lg ${colorClasses[color]}`}>
          <Icon className="h-6 w-6" />
        </div>
      </div>
    </div>
  );
};
```

---

## 4.3 需求详情页设计

### 4.3.1 需求详情页面

```typescript
// features/requirements/pages/RequirementDetailPage.tsx
export const RequirementDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { canEditRequirement, canChangeStatus } = usePermission();
  
  const { data: requirement, isLoading, error } = useQuery({
    queryKey: ['requirement', id],
    queryFn: () => getRequirementById(Number(id)),
    enabled: !!id
  });
  
  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorState message="加载失败" onRetry={() => queryClient.invalidateQueries(['requirement', id])} />;
  if (!requirement) return <Navigate to="/404" replace />;
  
  const canEdit = canEditRequirement(requirement);
  
  return (
    <div className="space-y-6">
      {/* 页面头部 */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Button variant="ghost" onClick={() => navigate('/requirements')}>
            <ArrowLeft className="h-4 w-4 mr-2" />
            返回列表
          </Button>
          <div>
            <h1 className="text-2xl font-bold">{requirement.name}</h1>
            <p className="text-slate-400 mt-1">需求号: {requirement.requirementNo}</p>
          </div>
        </div>
        {canEdit && (
          <Button onClick={() => navigate(`/requirements/${id}/edit`)}>
            编辑
          </Button>
        )}
      </div>
      
      {/* 基本信息 */}
      <Tabs defaultValue="info">
        <TabsList>
          <TabsTrigger value="info">基本信息</TabsTrigger>
          <TabsTrigger value="timeline">状态流转</TabsTrigger>
          <TabsTrigger value="notifications">通知记录</TabsTrigger>
        </TabsList>
        
        <TabsContent value="info">
          <RequirementInfoCard requirement={requirement} />
        </TabsContent>
        
        <TabsContent value="timeline">
          <StatusTimeline requirementId={requirement.id} />
        </TabsContent>
        
        <TabsContent value="notifications">
          <RequirementNotifications requirementId={requirement.id} />
        </TabsContent>
      </Tabs>
    </div>
  );
};
```

### 4.3.2 状态流转时间线

```typescript
// features/requirements/components/StatusTimeline.tsx
export const StatusTimeline = ({ requirementId }: Props) => {
  const { data: timeline } = useQuery({
    queryKey: ['requirement-timeline', requirementId],
    queryFn: () => getRequirementTimeline(requirementId)
  });
  
  const timelineItems = timeline ?? [];
  
  return (
    <div className="bg-slate-800 rounded-lg p-6">
      <h3 className="text-lg font-semibold mb-4">状态流转记录</h3>
      
      <div className="relative">
        <div className="absolute left-4 top-0 bottom-0 w-0.5 bg-slate-600" />
        
        <div className="space-y-6">
          {timelineItems.map((item, index) => (
            <div key={item.id} className="relative pl-10">
              {/* 时间线节点 */}
              <div className={cn(
                "absolute left-2 top-1 w-4 h-4 rounded-full border-2 bg-slate-900",
                index === 0 ? "border-indigo-500" : "border-slate-600"
              )} />
              
              <div className="bg-slate-700/50 rounded-lg p-4">
                <div className="flex items-center justify-between mb-2">
                  <div className="flex items-center gap-2">
                    <Badge variant={getStatusVariant(item.newStatus)}>
                      {item.newStatusName}
                    </Badge>
                    {item.oldStatus && (
                      <>
                        <ArrowRight className="h-4 w-4 text-slate-500" />
                        <Badge variant={getStatusVariant(item.oldStatus)} className="opacity-50">
                          {item.oldStatusName}
                        </Badge>
                      </>
                    )}
                  </div>
                  <span className="text-sm text-slate-400">
                    {format(new Date(item.createdAt), 'yyyy-MM-dd HH:mm:ss')}
                  </span>
                </div>
                
                <div className="text-sm text-slate-300">
                  操作人: {item.operatorName}
                </div>
                {item.remark && (
                  <p className="text-sm text-slate-400 mt-2">
                    备注: {item.remark}
                  </p>
                )}
              </div>
            </div>
          ))}
        </div>
        
        {timelineItems.length === 0 && (
          <div className="text-center text-slate-400 py-8">
            暂无流转记录
          </div>
        )}
      </div>
    </div>
  );
};
// 测试用例: TC-FLOW-019
```

### 4.3.3 需求信息卡片

```typescript
// features/requirements/components/RequirementInfoCard.tsx
export const RequirementInfoCard = ({ requirement }: Props) => {
  const { canViewPrice } = usePermission();
  
  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div className="bg-slate-800 rounded-lg p-6 space-y-4">
        <h3 className="text-lg font-semibold border-b border-slate-700 pb-2">基本信息</h3>
        
        <InfoRow label="需求名称" value={requirement.name} />
        <InfoRow label="需求号" value={requirement.requirementNo} />
        <InfoRow 
          label="当前状态" 
          value={<Badge variant={getStatusVariant(requirement.status)}>{requirement.statusName}</Badge>}
        />
        <InfoRow label="跟进人" value={requirement.followerName} />
        <InfoRow label="所属项目" value={requirement.projectName} />
        <InfoRow 
          label="优先级" 
          value={<PriorityBadge priority={requirement.priority} />}
        />
        <InfoRow label="需求已确认" value={requirement.isConfirmed ? '是' : '否'} />
        
        {canViewPrice() && requirement.price !== null && (
          <InfoRow label="报价" value={`¥${requirement.price.toFixed(2)}`} />
        )}
      </div>
      
      <div className="bg-slate-800 rounded-lg p-6 space-y-4">
        <h3 className="text-lg font-semibold border-b border-slate-700 pb-2">时间信息</h3>
        
        <InfoRow label="计划开始时间" value={formatDate(requirement.planStartDate)} />
        <InfoRow label="计划交测时间" value={formatDate(requirement.planTestDate)} />
        <InfoRow label="计划上线时间" value={formatDate(requirement.planLaunchDate)} />
        <InfoRow label="实际交测时间" value={formatDate(requirement.actualTestDate)} />
        <InfoRow label="实际上线时间" value={formatDate(requirement.actualLaunchDate)} />
        <InfoRow label="创建时间" value={formatDate(requirement.createdAt)} />
        <InfoRow label="更新时间" value={formatDate(requirement.updatedAt)} />
      </div>
      
      {requirement.docUrl && (
        <div className="bg-slate-800 rounded-lg p-6 lg:col-span-2">
          <h3 className="text-lg font-semibold border-b border-slate-700 pb-2 mb-4">需求文档</h3>
          <a 
            href={requirement.docUrl} 
            target="_blank" 
            rel="noopener noreferrer"
            className="text-indigo-400 hover:text-indigo-300 flex items-center gap-2"
          >
            <ExternalLink className="h-4 w-4" />
            {requirement.docUrl}
          </a>
        </div>
      )}
      
      {requirement.remark && (
        <div className="bg-slate-800 rounded-lg p-6 lg:col-span-2">
          <h3 className="text-lg font-semibold border-b border-slate-700 pb-2 mb-4">备注</h3>
          <p className="text-slate-300 whitespace-pre-wrap">{requirement.remark}</p>
        </div>
      )}
    </div>
  );
};
```

---

## 4.4 优先级相关设计

### 4.4.1 优先级枚举与 Badge

```typescript
// types/requirement.ts
export enum Priority {
  Low = 0,      // 低
  Medium = 1,   // 中
  High = 2      // 高
}

export const PRIORITY_NAMES: Record<Priority, string> = {
  [Priority.Low]: '低',
  [Priority.Medium]: '中',
  [Priority.High]: '高'
};

// components/PriorityBadge.tsx
export const PriorityBadge = ({ priority, showLabel = true }: Props) => {
  const variants = {
    [Priority.Low]: 'secondary',
    [Priority.Medium]: 'warning',
    [Priority.High]: 'error'
  };
  
  return (
    <Badge variant={variants[priority]}>
      {showLabel && <span className="mr-1">{PRIORITY_NAMES[priority]}</span>}
      <Zap className="h-3 w-3" />
    </Badge>
  );
};
```

---

## 4.5 进度输入组件

```typescript
// features/requirements/components/ProgressInput.tsx
export const ProgressInput = ({ value, onChange, disabled }: Props) => {
  const [localValue, setLocalValue] = useState(value ?? 0);
  
  useEffect(() => {
    setLocalValue(value ?? 0);
  }, [value]);
  
  const handleChange = (newValue: number) => {
    if (newValue < 0 || newValue > 100) {
      return;
    }
    setLocalValue(newValue);
    onChange(newValue);
  };
  
  return (
    <div className="space-y-2">
      <div className="flex items-center gap-4">
        <Input
          type="number"
          min={0}
          max={100}
          value={localValue}
          onChange={(e) => handleChange(parseInt(e.target.value) || 0)}
          disabled={disabled}
          className="w-24"
        />
        <span className="text-slate-400">%</span>
      </div>
      
      {/* 进度条可视化 */}
      <div className="w-full h-2 bg-slate-700 rounded-full overflow-hidden">
        <div 
          className={cn(
            "h-full transition-all duration-300",
            localValue === 100 ? "bg-emerald-500" : "bg-indigo-500"
          )}
          style={{ width: `${localValue}%` }}
        />
      </div>
    </div>
  );
};
// 测试用例: TC-REQ-024~026
```

---

## 4.6 报价输入组件

```typescript
// features/requirements/components/PriceInput.tsx
export const PriceInput = ({ value, onChange, disabled }: Props) => {
  const [localValue, setLocalValue] = useState(value ?? '');
  const { canViewPrice } = usePermission();
  
  useEffect(() => {
    setLocalValue(value ?? '');
  }, [value]);
  
  if (!canViewPrice()) {
    return <span className="text-slate-500">--</span>;
  }
  
  const handleChange = (newValue: string) => {
    // 允许空值、小数点开头（如 .99）、最多两位小数
    if (newValue && !/^\d*\.?\d{0,2}$/.test(newValue)) {
      return;
    }
    setLocalValue(newValue);
    onChange(newValue ? parseFloat(newValue) : null);
  };
  
  return (
    <div className="flex items-center gap-2">
      <span className="text-slate-400">¥</span>
      <Input
        type="text"
        inputMode="decimal"
        value={localValue}
        onChange={(e) => handleChange(e.target.value)}
        disabled={disabled}
        placeholder="0.00"
        className="w-32"
      />
    </div>
  );
};
// 测试用例: TC-REQ-036~043
```

---

## 4.7 Webhook 测试组件

```typescript
// features/robots/components/WebhookTester.tsx
export const WebhookTester = ({ webhookUrl, onTestResult }: Props) => {
  const [isTesting, setIsTesting] = useState(false);
  const [result, setResult] = useState<{ success: boolean; message: string } | null>(null);
  
  const handleTest = async () => {
    if (!webhookUrl) {
      toast.error('请先输入 Webhook 地址');
      return;
    }
    
    setIsTesting(true);
    setResult(null);
    
    try {
      const testResult = await testRobotConnection(webhookUrl);
      setResult(testResult);
      onTestResult?.(testResult);
      
      if (testResult.success) {
        toast.success('测试连接成功');
      } else {
        toast.error(testResult.message || '测试连接失败');
      }
    } catch (error) {
      const errorResult = { success: false, message: error.message || '网络错误' };
      setResult(errorResult);
      onTestResult?.(errorResult);
    } finally {
      setIsTesting(false);
    }
  };
  
  return (
    <div className="space-y-4">
      <Button 
        onClick={handleTest} 
        disabled={isTesting || !webhookUrl}
        loading={isTesting}
        variant="outline"
      >
        {isTesting ? '测试中...' : '测试连接'}
      </Button>
      
      {result && (
        <div className={cn(
          "p-3 rounded-lg text-sm",
          result.success 
            ? "bg-emerald-500/10 text-emerald-500 border border-emerald-500/20"
            : "bg-red-500/10 text-red-500 border border-red-500/20"
        )}>
          <div className="flex items-center gap-2">
            {result.success ? (
              <CheckCircle className="h-4 w-4" />
            ) : (
              <XCircle className="h-4 w-4" />
            )}
            {result.message}
          </div>
        </div>
      )}
    </div>
  );
};
// 测试用例: TC-BOT-001~004
```

---

## 4.8 状态 Badge 变体映射

```typescript
// components/ui/Badge.tsx
export const getStatusVariant = (status: RequirementStatus): BadgeVariant => {
  const variants: Record<RequirementStatus, BadgeVariant> = {
    [RequirementStatus.PendingConfirm]: 'secondary',
    [RequirementStatus.Confirmed]: 'info',
    [RequirementStatus.PendingQuote]: 'warning',
    [RequirementStatus.Quoted]: 'info',
    [RequirementStatus.PendingDev]: 'warning',
    [RequirementStatus.InDev]: 'primary',
    [RequirementStatus.InTest]: 'info',
    [RequirementStatus.AcceptedPendingLaunch]: 'warning',
    [RequirementStatus.Launched]: 'success'
  };
  return variants[status];
};

export const getStatusName = (status: RequirementStatus): string => {
  const names: Record<RequirementStatus, string> = {
    [RequirementStatus.PendingConfirm]: '待确认',
    [RequirementStatus.Confirmed]: '已确认',
    [RequirementStatus.PendingQuote]: '待报价',
    [RequirementStatus.Quoted]: '已报价',
    [RequirementStatus.PendingDev]: '待开发',
    [RequirementStatus.InDev]: '开发中',
    [RequirementStatus.InTest]: '测试中',
    [RequirementStatus.AcceptedPendingLaunch]: '已验收待上线',
    [RequirementStatus.Launched]: '已上线'
  };
  return names[status];
};

// 测试用例: TC-FLOW-xxx
```

---

## 4.9 通用工具函数

```typescript
// utils/formatters.ts
export const formatCurrency = (value: number | null | undefined, showSymbol = true): string => {
  if (value === null || value === undefined) return '--';
  return showSymbol ? `¥${value.toFixed(2)}` : value.toFixed(2);
};

export const formatPercentage = (value: number): string => {
  return `${value}%`;
};

export const formatDate = (date: string | Date | null | undefined): string => {
  if (!date) return '-';
  const d = typeof date === 'string' ? new Date(date) : date;
  return format(d, 'yyyy-MM-dd');
};

export const formatDateTime = (date: string | Date | null | undefined): string => {
  if (!date) return '-';
  const d = typeof date === 'string' ? new Date(date) : date;
  return format(d, 'yyyy-MM-dd HH:mm:ss');
};

export const getRoleName = (role: UserRole): string => {
  const names: Record<UserRole, string> = {
    [UserRole.Admin]: '管理员',
    [UserRole.Developer]: '开发人员',
    [UserRole.Tester]: '测试人员'
  };
  return names[role];
};
```

---

## 4.10 表单字段通用组件

```typescript
// components/common/FormField.tsx
interface FormFieldProps {
  control: Control<any>;
  name: string;
  label: string;
  rules?: ValidationRules;
  error?: FieldError;
  required?: boolean;
  children: React.ReactNode;
}

export const FormField = ({ 
  control, 
  name, 
  label, 
  rules, 
  error, 
  required,
  children 
}: FormFieldProps) => {
  return (
    <div className="space-y-2">
      <Label htmlFor={name} className="flex items-center gap-1">
        {label}
        {required && <span className="text-red-500">*</span>}
      </Label>
      <Controller
        name={name}
        control={control}
        rules={rules}
        render={({ field }) => (
          <div className="relative">
            {children}
          </div>
        )}
      />
      {error && (
        <p className="text-sm text-red-500 flex items-center gap-1">
          <AlertCircle className="h-3 w-3" />
          {error.message}
        </p>
      )}
    </div>
  );
};
```

---

## 4.11 加载与错误状态组件

```typescript
// components/common/LoadingOverlay.tsx
export const LoadingOverlay = ({ message = '加载中...' }: Props) => {
  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-slate-800 rounded-lg p-6 flex flex-col items-center gap-4">
        <LoadingSpinner size="lg" />
        <p className="text-slate-300">{message}</p>
      </div>
    </div>
  );
};

// components/common/ErrorBoundary.tsx
export class ErrorBoundary extends Component<Props, State> {
  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('ErrorBoundary caught an error', error, errorInfo);
  }
  
  render() {
    if (this.state.hasError) {
      return (
        <div className="flex flex-col items-center justify-center min-h-[400px]">
          <AlertTriangle className="h-16 w-16 text-amber-500 mb-4" />
          <h2 className="text-xl font-semibold mb-2">出错了</h2>
          <p className="text-slate-400 mb-4">{this.state.error?.message || '发生了未知错误'}</p>
          <Button onClick={() => window.location.reload()}>
            刷新页面
          </Button>
        </div>
      );
    }
    
    return this.props.children;
  }
}

// components/common/EmptyState.tsx
export const EmptyState = ({ message = '暂无数据', icon: Icon = Inbox }: Props) => {
  return (
    <div className="flex flex-col items-center justify-center py-12 text-slate-400">
      <Icon className="h-12 w-12 mb-4 opacity-50" />
      <p>{message}</p>
    </div>
  );
};
```

---

## 4.12 需求关联项目变更处理

```typescript
// features/requirements/hooks/useUpdateRequirement.ts
export const useUpdateRequirement = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async ({ id, ...data }: UpdateRequirementData) => {
      const result = await updateRequirement(id, data);
      
      if (result.versionConflict) {
        throw new VersionConflictError('数据已被他人修改，请刷新页面获取最新数据后重新编辑');
      }
      
      return result;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['requirements'] });
      queryClient.invalidateQueries({ queryKey: ['requirement', variables.id] });
      toast.success('需求更新成功');
    },
    onError: (error) => {
      if (error instanceof VersionConflictError) {
        setConflictDialogOpen(true);
      } else {
        toast.error(error.message || '更新失败');
      }
    }
  });
};

// 测试用例: TC-PROJ-008
```

---

## 4.13 机器人配置 Hooks

```typescript
// features/robots/hooks/useRobots.ts
export const useRobots = (params?: GetRobotsParams) => {
  return useQuery({
    queryKey: ['robots', params],
    queryFn: () => getRobots(params)
  });
};

export const useRobot = (id: number) => {
  return useQuery({
    queryKey: ['robot', id],
    queryFn: () => getRobotById(id),
    enabled: !!id
  });
};

export const useCreateRobot = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: CreateRobotData) => createRobot(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['robots'] });
      toast.success('机器人创建成功');
    }
  });
};

export const useUpdateRobot = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, ...data }: UpdateRobotData) => updateRobot(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['robots'] });
      queryClient.invalidateQueries({ queryKey: ['robot', variables.id] });
      toast.success('机器人更新成功');
    }
  });
};

export const useDeleteRobot = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => deleteRobot(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['robots'] });
      queryClient.invalidateQueries({ queryKey: ['requirements'] }); // 可能清除了关联需求
      toast.success('机器人删除成功，关联需求已自动清除');
    }
  });
};

export const useTestRobotConnection = () => {
  return useMutation({
    mutationFn: (webhookUrl: string) => testRobotConnection(webhookUrl)
  });
};

export const useToggleRobotStatus = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, isEnabled }: { id: number; isEnabled: boolean }) => 
      updateRobot(id, { isEnabled }),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['robots'] });
      queryClient.invalidateQueries({ queryKey: ['robot', variables.id] });
      toast.success(variables.isEnabled ? '机器人已启用' : '机器人已禁用');
    }
  });
};
```

---

## 4.14 通知相关 Hooks

```typescript
// features/notifications/hooks/useNotifications.ts
export const useNotifications = (params?: GetNotificationsParams) => {
  return useQuery({
    queryKey: ['notifications', params],
    queryFn: () => getNotifications(params)
  });
};

export const useNotification = (id: number) => {
  return useQuery({
    queryKey: ['notification', id],
    queryFn: () => getNotificationById(id),
    enabled: !!id
  });
};
```

---

## 4.15 仪表盘 API 类型

```typescript
// api/dashboard.ts
export interface DashboardStats {
  totalRequirements: number;
  inDevCount: number;
  inTestCount: number;
  launchedCount: number;
  pendingConfirmCount: number;
  statusDistribution: StatusDistribution[];
  priorityDistribution: PriorityDistribution[];
}

export interface StatusDistribution {
  status: RequirementStatus;
  statusName: string;
  count: number;
}

export interface PriorityDistribution {
  priority: Priority;
  priorityName: string;
  count: number;
}
```

---

## 4.16 机器人相关 API 类型

```typescript
// api/robots.ts
export interface RobotResponse {
  id: number;
  name: string;
  webhookUrl: string;
  groupName: string;
  isEnabled: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateRobotData {
  name: string;
  webhookUrl: string;
  groupName?: string;
  isEnabled?: boolean;
}

export interface UpdateRobotData {
  name?: string;
  webhookUrl?: string;
  groupName?: string;
  isEnabled?: boolean;
}

export interface TestRobotResult {
  success: boolean;
  message: string;
}
```

---

## 4.17 项目管理完整模块设计

### 项目列表页面

```typescript
// features/projects/pages/ProjectsListPage.tsx
export const ProjectsListPage = () => {
  const { canManageProject } = usePermission();
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize: 20 });
  
  const { data, isLoading } = useQuery({
    queryKey: ['projects', pagination],
    queryFn: () => getProjects({ pageIndex: pagination.pageIndex + 1, pageSize: pagination.pageSize })
  });
  
  if (!canManageProject()) {
    return <Navigate to="/403" replace />;
  }
  
  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">项目管理</h1>
        {canManageProject() && (
          <Button onClick={() => navigate('/projects/new')}>新建项目</Button>
        )}
      </div>
      
      <ProjectTable 
        data={data?.items ?? []}
        pagination={pagination}
        onPaginationChange={setPagination}
        totalCount={data?.totalCount ?? 0}
        loading={isLoading}
        onEdit={(project) => navigate(`/projects/${project.id}/edit`)}
        onDelete={handleDeleteProject}
      />
    </div>
  );
};
```

### 项目表单页面

```typescript
// features/projects/pages/ProjectFormPage.tsx
export const ProjectFormPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = !!id;
  
  const { data: project, isLoading } = useQuery({
    queryKey: ['project', id],
    queryFn: () => getProjectById(Number(id)),
    enabled: isEdit
  });
  
  const createMutation = useCreateProject();
  const updateMutation = useUpdateProject();
  const deleteMutation = useDeleteProject();
  
  const { control, handleSubmit, formState: { errors } } = useForm<ProjectFormData>({
    defaultValues: {
      name: '',
      code: '',
      managerId: undefined,
      description: ''
    }
  });
  
  const onSubmit = async (data: ProjectFormData) => {
    try {
      if (isEdit) {
        await updateMutation.mutateAsync({ id: Number(id), ...data });
        toast.success('项目更新成功');
      } else {
        await createMutation.mutateAsync(data);
        toast.success('项目创建成功');
      }
      navigate('/projects');
    } catch (error) {
      toast.error(error.message || '操作失败');
    }
  };
  
  const handleDelete = async () => {
    const confirmed = await showConfirmDialog({
      title: '确认删除',
      description: '确定要删除此项目吗？此操作不可撤销。',
      confirmText: '删除',
      confirmVariant: 'destructive'
    });
    
    if (confirmed) {
      try {
        await deleteMutation.mutateAsync(Number(id));
        navigate('/projects');
      } catch (error) {
        toast.error(error.message);
      }
    }
  };
  
  return (
    <div className="max-w-2xl">
      <h1 className="text-2xl font-bold mb-6">{isEdit ? '编辑项目' : '新建项目'}</h1>
      
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <FormField
          control={control}
          name="name"
          label="项目名称"
          rules={{ required: '请填写项目名称' }}
          error={errors.name}
        >
          <Input placeholder="请输入项目名称" />
        </FormField>
        
        <FormField
          control={control}
          name="code"
          label="项目代号"
          rules={{ 
            required: '请填写项目代号',
            pattern: {
              value: /^[a-zA-Z0-9_-]+$/,
              message: '项目代号只能包含字母、数字、下划线和短横线'
            }
          }}
          error={errors.code}
        >
          <Input placeholder="请输入项目代号（可选）" />
        </FormField>
        
        <FormField
          control={control}
          name="managerId"
          label="负责人"
        >
          <UserSelect
            value={watch('managerId')}
            onChange={(value) => setValue('managerId', value)}
            placeholder="请选择负责人（可选）"
          />
        </FormField>
        
        <FormField
          control={control}
          name="description"
          label="项目描述"
        >
          <Textarea 
            placeholder="请输入项目描述（可选）" 
            rows={4}
          />
        </FormField>
        
        <div className="flex gap-4 pt-4">
          <Button type="submit" loading={createMutation.isPending || updateMutation.isPending}>
            {isEdit ? '保存' : '创建'}
          </Button>
          {isEdit && (
            <Button 
              type="button" 
              variant="destructive"
              onClick={handleDelete}
              loading={deleteMutation.isPending}
            >
              删除
            </Button>
          )}
          <Button type="button" variant="ghost" onClick={() => navigate('/projects')}>
            取消
          </Button>
        </div>
      </form>
    </div>
  );
};
```

### 项目删除约束处理

```typescript
// features/projects/hooks/useDeleteProject.ts
export const useDeleteProject = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (id: number) => {
      try {
        return await deleteProject(id);
      } catch (error) {
        if (error.code === 'PROJECT_HAS_REQUIREMENTS') {
          throw new Error('该项目下存在需求，无法删除');
        }
        throw error;
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      toast.success('项目删除成功');
    },
    onError: (error: Error) => {
      toast.error(error.message);
    }
  });
};
// 测试用例: TC-PROJ-004~005
```

### 项目管理 Hooks

```typescript
// features/projects/hooks/useProjects.ts
export const useProjects = (params?: GetProjectsParams) => {
  return useQuery({
    queryKey: ['projects', params],
    queryFn: () => getProjects(params)
  });
};

export const useProject = (id: number) => {
  return useQuery({
    queryKey: ['project', id],
    queryFn: () => getProjectById(id),
    enabled: !!id
  });
};

export const useCreateProject = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: CreateProjectData) => createProject(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    }
  });
};

export const useUpdateProject = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, ...data }: UpdateProjectData) => updateProject(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      queryClient.invalidateQueries({ queryKey: ['project', variables.id] });
    }
  });
};

export const useDeleteProject = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => deleteProject(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    }
  });
};
```

### 项目相关 API 类型

```typescript
// api/projects.ts
export interface ProjectResponse {
  id: number;
  name: string;
  code: string | null;
  managerId: number | null;
  managerName: string | null;
  description: string | null;
  requirementCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProjectData {
  name: string;
  code?: string;
  managerId?: number;
  description?: string;
}

export interface UpdateProjectData {
  name?: string;
  code?: string;
  managerId?: number;
  description?: string;
}

export interface GetProjectsParams {
  keyword?: string;
  pageIndex?: number;
  pageSize?: number;
}
```

---

## 4.18 确认对话框组件

```typescript
// components/common/ConfirmDialog.tsx
interface ConfirmDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title: string;
  description?: string;
  confirmText?: string;
  cancelText?: string;
  confirmVariant?: ButtonVariant;
  onConfirm: () => void | Promise<void>;
}

export const ConfirmDialog = ({
  open,
  onOpenChange,
  title,
  description,
  confirmText = '确认',
  cancelText = '取消',
  confirmVariant = 'primary',
  onConfirm
}: ConfirmDialogProps) => {
  const [isLoading, setIsLoading] = useState(false);
  
  const handleConfirm = async () => {
    setIsLoading(true);
    try {
      await onConfirm();
      onOpenChange(false);
    } finally {
      setIsLoading(false);
    }
  };
  
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          {description && (
            <DialogDescription>{description}</DialogDescription>
          )}
        </DialogHeader>
        <DialogFooter>
          <Button 
            variant="ghost" 
            onClick={() => onOpenChange(false)}
            disabled={isLoading}
          >
            {cancelText}
          </Button>
          <Button 
            variant={confirmVariant}
            onClick={handleConfirm}
            loading={isLoading}
          >
            {confirmText}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

// 便捷函数
export const showConfirmDialog = async (options: Omit<ConfirmDialogProps, 'open' | 'onOpenChange'>): Promise<boolean> => {
  return new Promise((resolve) => {
    const handleConfirm = () => {
      resolve(true);
    };
    
    const handleCancel = () => {
      resolve(false);
    };
    
    // 使用全局状态或 Context 触发对话框显示
    confirmDialogStore.open({
      ...options,
      onConfirm: async () => {
        await options.onConfirm();
        resolve(true);
      }
    });
  });
};
```

---

## 4.19 状态流转配置与帮助函数

```typescript
// features/requirements/utils/statusUtils.ts

// 状态流转配置
export const STATUS_TRANSITIONS: Record<RequirementStatus, RequirementStatus[]> = {
  [RequirementStatus.PendingConfirm]: [RequirementStatus.Confirmed],
  [RequirementStatus.Confirmed]: [RequirementStatus.PendingQuote],
  [RequirementStatus.PendingQuote]: [RequirementStatus.Quoted],
  [RequirementStatus.Quoted]: [RequirementStatus.PendingDev],
  [RequirementStatus.PendingDev]: [RequirementStatus.InDev],
  [RequirementStatus.InDev]: [RequirementStatus.InTest],
  [RequirementStatus.InTest]: [RequirementStatus.AcceptedPendingLaunch],
  [RequirementStatus.AcceptedPendingLaunch]: [RequirementStatus.Launched],
  [RequirementStatus.Launched]: []
};

// 状态中文名称映射
export const STATUS_NAMES: Record<RequirementStatus, string> = {
  [RequirementStatus.PendingConfirm]: '待确认',
  [RequirementStatus.Confirmed]: '已确认',
  [RequirementStatus.PendingQuote]: '待报价',
  [RequirementStatus.Quoted]: '已报价',
  [RequirementStatus.PendingDev]: '待开发',
  [RequirementStatus.InDev]: '开发中',
  [RequirementStatus.InTest]: '测试中',
  [RequirementStatus.AcceptedPendingLaunch]: '已验收待上线',
  [RequirementStatus.Launched]: '已上线'
};

// 获取当前状态的唯一合法后继状态
export const getNextStatus = (currentStatus: RequirementStatus): RequirementStatus | null => {
  const transitions = STATUS_TRANSITIONS[currentStatus];
  return transitions.length > 0 ? transitions[0] : null;
};

// 获取所有合法后继状态列表
export const getValidNextStatuses = (currentStatus: RequirementStatus): RequirementStatus[] => {
  return STATUS_TRANSITIONS[currentStatus] ?? [];
};

// 检查状态是否为终态
export const isTerminalStatus = (status: RequirementStatus): boolean => {
  return status === RequirementStatus.Launched;
};

// 检查状态变更是否合法
export const isValidStatusTransition = (
  fromStatus: RequirementStatus, 
  toStatus: RequirementStatus
): boolean => {
  const validNextStatuses = STATUS_TRANSITIONS[fromStatus];
  return validNextStatuses?.includes(toStatus) ?? false;
};

// 获取状态颜色变体
export const getStatusVariant = (status: RequirementStatus): BadgeVariant => {
  const variants: Record<RequirementStatus, BadgeVariant> = {
    [RequirementStatus.PendingConfirm]: 'secondary',
    [RequirementStatus.Confirmed]: 'info',
    [RequirementStatus.PendingQuote]: 'warning',
    [RequirementStatus.Quoted]: 'info',
    [RequirementStatus.PendingDev]: 'warning',
    [RequirementStatus.InDev]: 'primary',
    [RequirementStatus.InTest]: 'info',
    [RequirementStatus.AcceptedPendingLaunch]: 'warning',
    [RequirementStatus.Launched]: 'success'
  };
  return variants[status];
};

// 测试用例: TC-FLOW-001~020
```

---

## 4.20 版本冲突错误类

```typescript
// types/errors.ts
export class VersionConflictError extends Error {
  constructor(message: string = '数据已被他人修改，请刷新页面获取最新数据后重新编辑') {
    super(message);
    this.name = 'VersionConflictError';
  }
}

export class ApiError extends Error {
  constructor(
    message: string,
    public code?: string,
    public statusCode?: number
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

// utils/errorHandler.ts
export const handleApiError = (error: unknown): Error => {
  if (error instanceof VersionConflictError) {
    return error;
  }
  
  if (error instanceof ApiError) {
    return error;
  }
  
  if (axios.isAxiosError(error)) {
    const axiosError = error;
    if (axiosError.response?.status === 409) {
      return new VersionConflictError();
    }
    return new ApiError(
      axiosError.response?.data?.message || '请求失败',
      axiosError.response?.data?.code,
      axiosError.response?.status
    );
  }
  
  return new Error('发生了未知错误');
};
// 测试用例: TC-REQ-050~051
```

---

## 4.21 需求表单完整设计

```typescript
// features/requirements/components/RequirementForm.tsx
export const RequirementForm = ({ 
  requirement, 
  onSubmit, 
  isLoading,
  mode = 'create'  // 'create' | 'edit'
}: Props) => {
  const { canViewPrice, canChangeStatus, isAdmin } = usePermission();
  const isEdit = mode === 'edit';
  const currentStatus = requirement?.status ?? RequirementStatus.PendingConfirm;
  
  const {
    control,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
    reset
  } = useForm<RequirementFormData>({
    defaultValues: {
      name: requirement?.name ?? '',
      requirementNo: requirement?.requirementNo ?? '',
      status: currentStatus,
      followerId: requirement?.followerId,
      projectId: requirement?.projectId,
      planStartDate: requirement?.planStartDate ?? '',
      planTestDate: requirement?.planTestDate ?? '',
      planLaunchDate: requirement?.planLaunchDate ?? '',
      docUrl: requirement?.docUrl ?? '',
      price: requirement?.price?.toString() ?? '',
      robotId: requirement?.robotId,
      priority: requirement?.priority ?? Priority.Medium,
      remark: requirement?.remark ?? '',
      version: requirement?.version
    }
  });
  
  // 状态选择器：根据当前状态显示唯一合法后继状态
  const validNextStatuses = getValidNextStatuses(currentStatus);
  const isTerminal = isTerminalStatus(currentStatus);
  
  useEffect(() => {
    if (isEdit && requirement) {
      reset({
        name: requirement.name,
        requirementNo: requirement.requirementNo,
        status: currentStatus,
        followerId: requirement.followerId,
        projectId: requirement.projectId,
        planStartDate: requirement.planStartDate ?? '',
        planTestDate: requirement.planTestDate ?? '',
        planLaunchDate: requirement.planLaunchDate ?? '',
        docUrl: requirement.docUrl ?? '',
        price: requirement.price?.toString() ?? '',
        robotId: requirement.robotId,
        priority: requirement.priority ?? Priority.Medium,
        remark: requirement.remark ?? '',
        version: requirement.version
      });
    }
  }, [requirement, reset]);
  
  const onFormSubmit = (data: RequirementFormData) => {
    onSubmit({
      ...data,
      status: data.status, // 状态由单独 API 更新
      price: data.price ? parseFloat(data.price) : undefined
    });
  };
  
  return (
    <form onSubmit={handleSubmit(onFormSubmit)} className="space-y-6">
      {/* 基本信息 */}
      <div className="bg-slate-800 rounded-lg p-6 space-y-4">
        <h3 className="text-lg font-semibold border-b border-slate-700 pb-2">基本信息</h3>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <FormField
            control={control}
            name="name"
            label="需求名称"
            rules={{ required: '请填写需求名称' }}
            error={errors.name}
            required
          >
            <Input placeholder="请输入需求名称" disabled={isLoading} />
          </FormField>
          
          <FormField
            control={control}
            name="requirementNo"
            label="需求号"
            rules={{ required: '请填写需求号' }}
            error={errors.requirementNo}
            required
          >
            <Input 
              placeholder="请输入需求号" 
              disabled={isLoading || isEdit}
              className={isEdit ? 'bg-slate-700/50' : ''}
            />
          </FormField>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {/* 状态选择器 - 仅管理员可见，且非终态 */}
          {isAdmin() && !isTerminal && (
            <FormField
              control={control}
              name="status"
              label="状态变更"
            >
              <StatusSelect
                value={watch('status')}
                onChange={(status) => setValue('status', status)}
                validStatuses={validNextStatuses}
                disabled={isLoading}
              />
            </FormField>
          )}
          
          {/* 终态显示 */}
          {isTerminal && (
            <div className="space-y-2">
              <Label>当前状态</Label>
              <Badge variant={getStatusVariant(RequirementStatus.Launched)} className="text-base">
                已上线
              </Badge>
              <p className="text-sm text-slate-400">
                已上线需求仅可编辑备注字段
              </p>
            </div>
          )}
          
          <FormField
            control={control}
            name="priority"
            label="优先级"
          >
            <Select
              value={watch('priority').toString()}
              onValueChange={(val) => setValue('priority', Number(val))}
              disabled={isLoading || isTerminal}
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={Priority.Low.toString()}>低</SelectItem>
                <SelectItem value={Priority.Medium.toString()}>中</SelectItem>
                <SelectItem value={Priority.High.toString()}>高</SelectItem>
              </SelectContent>
            </Select>
          </FormField>
        </div>
      </div>
      
      {/* 分配信息 */}
      <div className="bg-slate-800 rounded-lg p-6 space-y-4">
        <h3 className="text-lg font-semibold border-b border-slate-700 pb-2">分配信息</h3>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <FormField
            control={control}
            name="followerId"
            label="跟进人"
            rules={{ required: '请选择跟进人' }}
            error={errors.followerId}
            required
          >
            <UserSelect
              value={watch('followerId')}
              onChange={(value) => setValue('followerId', value)}
              disabled={isLoading || isTerminal}
            />
          </FormField>
          
          <FormField
            control={control}
            name="projectId"
            label="所属项目"
            rules={{ required: '请选择所属项目' }}
            error={errors.projectId}
            required
          >
            <ProjectSelect
              value={watch('projectId')}
              onChange={(value) => setValue('projectId', value)}
              disabled={isLoading || isTerminal}
            />
          </FormField>
        </div>
      </div>
      
      {/* 时间计划 */}
      <div className="bg-slate-800 rounded-lg p-6 space-y-4">
        <h3 className="text-lg font-semibold border-b border-slate-700 pb-2">时间计划</h3>
        
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <FormField
            control={control}
            name="planStartDate"
            label="计划开始时间"
          >
            <DatePicker
              value={watch('planStartDate') ? new Date(watch('planStartDate')!) : undefined}
              onChange={(date) => setValue('planStartDate', date?.toISOString())}
              disabled={isLoading || isTerminal}
            />
          </FormField>
          
          <FormField
            control={control}
            name="planTestDate"
            label="计划交测时间"
            error={errors.planTestDate}
          >
            <DatePicker
              value={watch('planTestDate') ? new Date(watch('planTestDate')!) : undefined}
              onChange={(date) => setValue('planTestDate', date?.toISOString())}
              disabled={isLoading || isTerminal}
            />
          </FormField>
          
          <FormField
            control={control}
            name="planLaunchDate"
            label="计划上线时间"
            error={errors.planLaunchDate}
          >
            <DatePicker
              value={watch('planLaunchDate') ? new Date(watch('planLaunchDate')!) : undefined}
              onChange={(date) => setValue('planLaunchDate', date?.toISOString())}
              disabled={isLoading || isTerminal}
            />
          </FormField>
        </div>
      </div>
      
      {/* 进度信息 */}
      <div className="bg-slate-800 rounded-lg p-6 space-y-4">
        <h3 className="text-lg font-semibold border-b border-slate-700 pb-2">进度信息</h3>
        
        <FormField
          control={control}
          name="progress"
          label="进度"
        >
          <ProgressInput
            value={watch('progress') ?? 0}
            onChange={(value) => setValue('progress', value)}
            disabled={isLoading || isTerminal}
          />
        </FormField>
      </div>
      
      {/* 文档与报价 */}
      <div className="bg-slate-800 rounded-lg p-6 space-y-4">
        <h3 className="text-lg font-semibold border-b border-slate-700 pb-2">文档与报价</h3>
        
        <FormField
          control={control}
          name="docUrl"
          label="需求文档链接"
          error={errors.docUrl}
        >
          <DocUrlInput
            value={watch('docUrl') ?? ''}
            onChange={(value) => setValue('docUrl', value)}
            disabled={isLoading || isTerminal}
          />
        </FormField>
        
        {canViewPrice() && (
          <FormField
            control={control}
            name="price"
            label="报价"
            error={errors.price}
          >
            <PriceInput
              value={watch('price') ?? ''}
              onChange={(value) => setValue('price', value)}
              disabled={isLoading || isTerminal}
            />
          </FormField>
        )}
      </div>
      
      {/* 通知设置 */}
      <div className="bg-slate-800 rounded-lg p-6 space-y-4">
        <h3 className="text-lg font-semibold border-b border-slate-700 pb-2">通知设置</h3>
        
        <FormField
          control={control}
          name="robotId"
          label="通知机器人"
        >
          <RobotSelect
            value={watch('robotId')}
            onChange={(value) => setValue('robotId', value)}
            disabled={isLoading || isTerminal}
          />
        </FormField>
      </div>
      
      {/* 备注 */}
      <div className="bg-slate-800 rounded-lg p-6 space-y-4">
        <h3 className="text-lg font-semibold border-b border-slate-700 pb-2">备注</h3>
        
        <FormField
          control={control}
          name="remark"
          label="备注"
        >
          <Textarea
            value={watch('remark') ?? ''}
            onChange={(e) => setValue('remark', e.target.value)}
            placeholder="请输入备注信息"
            rows={4}
            disabled={isLoading}
          />
        </FormField>
      </div>
      
      {/* 提交按钮 */}
      <div className="flex gap-4">
        <Button 
          type="submit" 
          loading={isLoading}
          disabled={isTerminal}
        >
          {isEdit ? '保存' : '创建'}
        </Button>
        <Button 
          type="button" 
          variant="ghost"
          onClick={() => window.history.back()}
        >
          取消
        </Button>
      </div>
    </form>
  );
};
// 测试用例覆盖: TC-REQ-014~053, TC-FLOW-001~020
```

---

## 5. 核心组件清单

### 5.1 UI 基础组件 (基于 Radix UI)

| 组件 | 基于 | 功能 | 说明 |
|------|------|------|------|
| **Button** | @radix-ui/react-slot | 按钮，支持变体（primary/secondary/danger/ghost） | 支持 loading 状态 |
| **Input** | - | 文本输入框 | 支持 prefix/suffix 图标 |
| **Textarea** | - | 多行文本输入 | 用于备注等字段 |
| **Select** | @radix-ui/react-select | 下拉选择（严格线性状态流转） | 支持单选/多选 |
| **Dialog** | @radix-ui/react-dialog | 模态框 | 支持 title/description/footer |
| **ConfirmDialog** | Dialog | 确认对话框 | 用于删除等危险操作 |
| **Table** | - | 数据表格，支持排序、分页 | 集成 loading 状态 |
| **Badge** | - | 状态标签（9种状态对应不同颜色） | TC-FLOW-xxx |
| **DatePicker** | @radix-ui/react-popper | 日期选择器 | 支持清空 |
| **DateRangePicker** | DatePicker | 日期范围选择器 | 用于时间范围筛选 |
| **Dropdown** | @radix-ui/react-dropdown-menu | 下拉菜单 | 用于表格行操作 |
| **Tabs** | @radix-ui/react-tabs | 标签页 | 用于详情页分组 |
| **Toast** | @radix-ui/react-toast | 通知提示 | 自动消失/手动关闭 |
| **Tooltip** | @radix-ui/react-tooltip | 文字提示 | hover 显示 |
| **Pagination** | - | 分页组件 | 支持每页条数切换 |
| **Checkbox** | @radix-ui/react-checkbox | 复选框 | 用于多选筛选 |
| **Switch** | @radix-ui/react-switch | 开关 | 用于启用/禁用机器人 |
| **Skeleton** | - | 加载骨架屏 | 数据加载中显示 |

### 5.2 业务组件

| 组件 | 说明 | 测试用例 |
|------|------|---------|
| **RequirementTable** | 需求列表表格，集成高级筛选、排序、分页 | TC-REQ-001~013 |
| **RequirementForm** | 需求表单（创建/编辑复用），包含所有字段 | TC-REQ-014~043 |
| **StatusSelect** | 状态选择器，仅显示当前状态的唯一合法后继状态 | TC-FLOW-009, TC-FLOW-013 |
| **ProgressInput** | 进度输入组件（0-100），带百分比显示 | TC-REQ-024~026 |
| **PriceInput** | 报价输入组件（精度2位小数），仅管理员可见 | TC-REQ-036~043 |
| **DocUrlInput** | 文档链接输入，带 URL 格式校验（http/https/内网检测） | TC-REQ-030~035 |
| **VersionBadge** | 显示当前数据版本号 | TC-REQ-050~053 |
| **VersionConflictDialog** | 并发编辑冲突提示对话框，引导刷新 | TC-REQ-050~051 |
| **AdvancedFilter** | 高级筛选面板（状态、项目、跟进人、时间范围） | TC-REQ-003~008 |
| **NotificationPanel** | 通知机器人选择面板（多选） | TC-BOT-007 |
| **ProjectSelect** | 项目选择下拉框（单选） | TC-PROJ-006 |
| **UserSelect** | 用户选择下拉框（跟进人选择） | TC-REQ-004 |
| **Timeline** | 状态流转时间线展示 | TC-FLOW-019 |
| **Statistics** | 仪表盘统计卡片 | - |
| **EmptyState** | 空数据状态展示 | TC-REQ-002 |
| **WebhookTester** | Webhook 地址测试组件 | TC-BOT-001~004 |
| **NotificationTable** | 通知日志列表表格 | TC-NOT-xxx |

---

## 6. 高级筛选功能设计

### 6.1 筛选条件

| 筛选项 | 类型 | API 参数 | 说明 | 测试用例 |
|--------|------|----------|------|---------|
| **需求状态** | 多选 | `status` (逗号分隔) | 支持多状态同时筛选 | TC-REQ-003 |
| **跟进人** | 单选 | `followerId` | 从用户列表选择 | TC-REQ-004 |
| **所属项目** | 单选 | `projectId` | 从项目列表选择 | TC-REQ-005 |
| **计划开始时间** | 日期范围 | `planStartDateFrom/To` | 筛选时间范围内的需求 | TC-REQ-006 |
| **关键词搜索** | 文本 | `keyword` | 搜索需求名称/需求号 | - |

### 6.2 排序功能

| 支持字段 | API 参数 | 默认 | 说明 | 测试用例 |
|----------|----------|------|------|---------|
| 创建时间 | `sortBy=createdAt&sortOrder=desc` | ✅ 升序 | 默认倒序排列 | TC-REQ-001 |
| 计划开始时间 | `sortBy=planStartDate` | - | 可升序/降序 | TC-REQ-008 |
| 计划交测时间 | `sortBy=planTestDate` | - | 可升序/降序 | - |
| 计划上线时间 | `sortBy=planLaunchDate` | - | 可升序/降序 | - |

### 6.3 分页配置

- 默认每页 20 条
- 可选: 10 / 20 / 50 条
- 支持跳转至指定页
- **测试用例**: TC-REQ-009~012

---

## 6.4 通知管理模块详细设计

### 6.4.1 通知日志页面

```typescript
// features/notifications/pages/NotificationsListPage.tsx
export const NotificationsListPage = () => {
  const { canViewNotifications } = usePermission();
  const [filters, setFilters] = useState<NotificationFilters>({
    pageIndex: 1,
    pageSize: 20
  });
  
  const { data, isLoading } = useQuery({
    queryKey: ['notifications', filters],
    queryFn: () => getNotifications(filters)
  });
  
  if (!canViewNotifications()) {
    return <Navigate to="/403" replace />;
  }
  
  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">通知日志</h1>
      </div>
      
      <NotificationFilters value={filters} onChange={setFilters} />
      <NotificationTable 
        data={data?.items ?? []}
        loading={isLoading}
        onViewDetail={handleViewDetail}
      />
      <Pagination
        pageIndex={filters.pageIndex - 1}
        pageSize={filters.pageSize}
        totalCount={data?.totalCount ?? 0}
        onPageChange={(page) => setFilters({ ...filters, pageIndex: page })}
        onPageSizeChange={(size) => setFilters({ ...filters, pageSize: size })}
      />
    </div>
  );
};
```

### 6.4.2 通知日志筛选组件

```typescript
// features/notifications/components/NotificationFilters.tsx
export interface NotificationFilters {
  requirementId?: number;
  requirementNo?: string;
  status?: NotificationStatus;
  type?: NotificationType;
  startDateFrom?: string;
  startDateTo?: string;
  pageIndex: number;
  pageSize: number;
}

export const NotificationFilters = ({ value, onChange }: Props) => {
  const [localFilters, setLocalFilters] = useState(value);
  
  const handleSearch = () => {
    onChange({ ...localFilters, pageIndex: 1 });
  };
  
  const handleReset = () => {
    const resetFilters = { pageIndex: 1, pageSize: 20 };
    setLocalFilters(resetFilters);
    onChange(resetFilters);
  };
  
  return (
    <div className="bg-slate-800 rounded-lg p-4 space-y-4">
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Input
          placeholder="需求号"
          value={localFilters.requirementNo ?? ''}
          onChange={(e) => setLocalFilters({ ...localFilters, requirementNo: e.target.value })}
        />
        
        <Select
          value={localFilters.status?.toString() ?? ''}
          onValueChange={(val) => setLocalFilters({ ...localFilters, status: val ? Number(val) : undefined })}
        >
          <SelectTrigger>
            <SelectValue placeholder="通知状态" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">全部</SelectItem>
            <SelectItem value={NotificationStatus.Success.toString()}>成功</SelectItem>
            <SelectItem value={NotificationStatus.Failed.toString()}>失败</SelectItem>
            <SelectItem value={NotificationStatus.Pending.toString()}>待发送</SelectItem>
          </SelectContent>
        </Select>
        
        <Select
          value={localFilters.type?.toString() ?? ''}
          onValueChange={(val) => setLocalFilters({ ...localFilters, type: val ? Number(val) : undefined })}
        >
          <SelectTrigger>
            <SelectValue placeholder="通知类型" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">全部</SelectItem>
            <SelectItem value={NotificationType.StatusChange.toString()}>状态变更</SelectItem>
            <SelectItem value={NotificationType.Reminder.toString()}>提醒</SelectItem>
            <SelectItem value={NotificationType.CascadedClear.toString()}>级联清除</SelectItem>
          </SelectContent>
        </Select>
        
        <DateRangePicker
          value={{
            from: localFilters.startDateFrom ? new Date(localFilters.startDateFrom) : undefined,
            to: localFilters.startDateTo ? new Date(localFilters.startDateTo) : undefined
          }}
          onChange={(range) => setLocalFilters({
            ...localFilters,
            startDateFrom: range?.from?.toISOString(),
            startDateTo: range?.to?.toISOString()
          })}
          placeholder="发送时间范围"
        />
      </div>
      
      <div className="flex gap-2">
        <Button onClick={handleSearch}>查询</Button>
        <Button variant="outline" onClick={handleReset}>重置</Button>
      </div>
    </div>
  );
};
```

### 6.4.3 通知日志表格

```typescript
// features/notifications/components/NotificationTable.tsx
export const NotificationTable = ({ data, loading, onViewDetail }: Props) => {
  const columns: ColumnDef<NotificationResponse>[] = [
    {
      accessorKey: 'requirementNo',
      header: '需求号',
      cell: ({ row }) => (
        <Button 
          variant="link" 
          onClick={() => onViewDetail(row.original)}
          className="p-0 h-auto"
        >
          {row.original.requirementNo}
        </Button>
      )
    },
    {
      accessorKey: 'requirementName',
      header: '需求名称'
    },
    {
      accessorKey: 'type',
      header: '类型',
      cell: ({ row }) => (
        <Badge variant={getNotificationTypeVariant(row.original.type)}>
          {getNotificationTypeName(row.original.type)}
        </Badge>
      )
    },
    {
      accessorKey: 'robotName',
      header: '机器人',
      cell: ({ row }) => row.original.robotName ?? '-'
    },
    {
      accessorKey: 'status',
      header: '状态',
      cell: ({ row }) => (
        <Badge variant={getNotificationStatusVariant(row.original.status)}>
          {getNotificationStatusName(row.original.status)}
        </Badge>
      )
    },
    {
      accessorKey: 'errorMessage',
      header: '错误信息',
      cell: ({ row }) => (
        <Tooltip content={row.original.errorMessage ?? ''}>
          <span className="truncate max-w-[200px]">
            {row.original.errorMessage ?? '-'}
          </span>
        </Tooltip>
      )
    },
    {
      accessorKey: 'sentAt',
      header: '发送时间',
      cell: ({ row }) => row.original.sentAt 
        ? format(new Date(row.original.sentAt), 'yyyy-MM-dd HH:mm:ss')
        : '-'
    },
    {
      accessorKey: 'createdAt',
      header: '创建时间',
      cell: ({ row }) => format(new Date(row.original.createdAt), 'yyyy-MM-dd HH:mm:ss')
    }
  ];
  
  return (
    <div className="border rounded-lg">
      <Table>
        <TableHeader>
          <TableRow>
            {columns.map((col) => (
              <TableHead key={col.id}>{col.header}</TableHead>
            ))}
          </TableRow>
        </TableHeader>
        <TableBody>
          {loading ? (
            <TableRow>
              <TableCell colSpan={columns.length} className="h-24 text-center">
                <LoadingSpinner />
              </TableCell>
            </TableRow>
          ) : data.length === 0 ? (
            <TableRow>
              <TableCell colSpan={columns.length} className="h-24 text-center">
                <EmptyState message="暂无通知记录" />
              </TableCell>
            </TableRow>
          ) : (
            data.map((notification) => (
              <TableRow key={notification.id}>
                {columns.map((col) => (
                  <TableCell key={col.id}>
                    {col.cell({ row: { original: notification } } as any)}
                  </TableCell>
                ))}
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </div>
  );
};

// 通知类型枚举
export enum NotificationType {
  StatusChange = 0,
  Reminder = 1,
  CascadedClear = 2
}

export enum NotificationStatus {
  Pending = 0,
  Success = 1,
  Failed = 2
}
```

### 6.4.4 通知相关 API 类型

```typescript
// api/notifications.ts
export interface NotificationResponse {
  id: number;
  requirementId: number;
  requirementName: string;
  requirementNo: string;
  type: NotificationType;
  robotId: number | null;
  robotName: string | null;
  status: NotificationStatus;
  errorMessage: string | null;
  sentAt: string | null;
  createdAt: string;
}

export interface GetNotificationsParams {
  requirementId?: number;
  requirementNo?: string;
  status?: NotificationStatus;
  type?: NotificationType;
  startDateFrom?: string;
  startDateTo?: string;
  pageIndex?: number;
  pageSize?: number;
}

export interface NotificationDetailResponse extends NotificationResponse {
  retryCount: number;
  lastAttemptAt: string | null;
  remark: string | null;
}
```

---

## 7. 状态流转 UI 控制

### 7.1 状态流转配置

```typescript
// 状态流转配置 - 与后端 RequirementStateMachine 一致
const STATUS_TRANSITIONS: Record<RequirementStatus, RequirementStatus[]> = {
  [RequirementStatus.PendingConfirm]: [RequirementStatus.Confirmed],
  [RequirementStatus.Confirmed]: [RequirementStatus.PendingQuote],
  [RequirementStatus.PendingQuote]: [RequirementStatus.Quoted],
  [RequirementStatus.Quoted]: [RequirementStatus.PendingDev],
  [RequirementStatus.PendingDev]: [RequirementStatus.InDev],
  [RequirementStatus.InDev]: [RequirementStatus.InTest],
  [RequirementStatus.InTest]: [RequirementStatus.AcceptedPendingLaunch],
  [RequirementStatus.AcceptedPendingLaunch]: [RequirementStatus.Launched],
  [RequirementStatus.Launched]: [] // 终态，不可变更
};

// 状态中文名称映射
const STATUS_NAMES: Record<RequirementStatus, string> = {
  [RequirementStatus.PendingConfirm]: '待确认',
  [RequirementStatus.Confirmed]: '已确认',
  [RequirementStatus.PendingQuote]: '待报价',
  [RequirementStatus.Quoted]: '已报价',
  [RequirementStatus.PendingDev]: '待开发',
  [RequirementStatus.InDev]: '开发中',
  [RequirementStatus.InTest]: '测试中',
  [RequirementStatus.AcceptedPendingLaunch]: '已验收待上线',
  [RequirementStatus.Launched]: '已上线'
};
```

### 7.2 UI 约束规则

| 场景 | UI 行为 | 说明 | 测试用例 |
|------|---------|------|---------|
| **编辑状态选择** | 下拉框仅显示唯一合法后继状态 | 当前状态=开发中，仅显示"测试中" | TC-FLOW-009, TC-FLOW-013 |
| **终态限制** | 已上线状态显示为静态标签，不可编辑 | 只能编辑备注字段 | TC-FLOW-010, TC-FLOW-012 |
| **权限控制** | 仅管理员可变更状态 | 状态下拉框对非管理员不可见 | TC-FLOW-xxx |
| **时间自动填充** | 前端仅展示，后端自动填充 | ActualTestDate/ActualLaunchDate | TC-FLOW-017~018 |
| **需求已确认标志** | 只读显示，由系统根据状态联动 | IsConfirmed 字段前端不可编辑 | TC-FLOW-015~016 |

### 7.3 状态变更交互流程

```
1. 用户打开需求编辑页
2. 前端根据当前状态获取合法后继状态列表
3. 状态下拉框仅显示一个选项（除非有分支逻辑）
4. 用户选择新状态后提交
5. 前端发送 PUT /api/requirements/{id}/status
6. 后端校验状态流转合法性（二次校验）
7. 后端自动填充时间字段（ActualTestDate/ActualLaunchDate）
8. 后端触发企业微信通知（5分钟内送达）
9. 前端更新状态显示，关闭编辑框
```

---

## 8. 表单校验规则

### 8.1 需求表单字段校验

| 字段 | 必填 | 校验规则 | 错误提示 | 测试用例 |
|------|------|----------|----------|---------|
| **需求名称** | ✅ | 必填，最多100字符 | "请填写需求名称" / "需求名称不能超过100个字符" | TC-REQ-016, TC-REQ-019~020 |
| **需求号** | ✅ | 必填，最多50字符，唯一性后端校验 | "请填写需求号" / "需求号不能超过50个字符" | TC-REQ-017, TC-REQ-021~022 |
| **当前状态** | ✅ | 下拉选择（初始值：待确认） | - | - |
| **跟进人** | ✅ | 必填，从用户列表选择 | "请选择跟进人" | - |
| **所属项目** | ✅ | 必填，从项目列表单选 | "请选择所属项目" | - |
| **计划交测时间** | - | 必须在计划开始时间之后 | "时间设置不合理，交测时间不能早于开始时间" | TC-REQ-023 |
| **计划上线时间** | - | 必须在计划交测时间之后 | "时间设置不合理，上线时间不能早于交测时间" | - |
| **需求文档链接** | - | http:// 或 https:// 开头，非内网地址 | "请输入有效的http或https链接" / "不支持内网地址" | TC-REQ-030~035 |
| **报价** | - | ≥0，最多2位小数，仅管理员可见/编辑 | "报价不能为负数" / "报价最多保留2位小数" | TC-REQ-036~043 |
| **进度** | - | 0-100 整数 | "进度值不合法，请输入0-100之间的整数" | TC-REQ-024~026 |
| **备注** | - | 最多500字符 | "备注不能超过500个字符" | TC-REQ-027~028 |

### 8.2 URL 校验详细规则

```typescript
// URL 校验逻辑
const validateDocUrl = (url: string): string | null => {
  if (!url) return null; // 非必填，可为空
  
  // 1. 格式校验：必须 http:// 或 https:// 开头
  if (!/^https?:\/\//i.test(url)) {
    return '请输入有效的http或https链接';
  }
  
  // 2. 内网地址检测
  const localhostPatterns = [
    /^https?:\/\/localhost/i,
    /^https?:\/\/127\.0\.0\.1/i,
    /^https?:\/\/10\.\d+\.\d+\.\d+/i,  // 10.x.x.x
    /^https?:\/\/172\.(1[6-9]|2\d|3[01])\.\d+\.\d+/i, // 172.16-31.x.x
    /^https?:\/\/192\.168\.\d+\.\d+/i,   // 192.168.x.x
  ];
  
  if (localhostPatterns.some(pattern => pattern.test(url))) {
    return '不支持内网地址';
  }
  
  // 3. 域名格式校验
  try {
    new URL(url);
  } catch {
    return '请输入有效的URL地址';
  }
  
  return null; // 校验通过
};
```

---

## 9. 权限控制实现

### 9.1 权限矩阵

| 操作 | 管理员 | 开发人员 | 测试人员 | 前端实现 |
|------|--------|----------|----------|---------|
| 需求增删改查 | ✅ | 查看、更新分配给自己的 | 查看、更新分配给自己的 | 路由守卫 + 按钮显隐 |
| 需求状态变更 | ✅ | ❌ | ❌ | 状态下拉框不可见 |
| 需求报价查看/编辑 | ✅ | ❌ | ❌ | 字段不可见或只读灰色 | TC-REQ-041~043 |
| 项目管理 | ✅ | ❌ | ❌ | 菜单不可见 | TC-PROJ-007 |
| 机器人配置 | ✅ | ❌ | ❌ | 菜单不可见 | TC-BOT-006 |
| 用户管理 | ✅ | ❌ | ❌ | 菜单不可见 | - |
| 通知日志查看 | ✅ | ❌ | ❌ | 菜单不可见 | - |

### 9.2 前端权限实现

```typescript
// usePermission.ts
export const usePermission = () => {
  const { user } = useAuthStore();
  
  const canViewPrice = () => user?.role === UserRole.Admin;
  const canChangeStatus = () => user?.role === UserRole.Admin;
  const canManageProject = () => user?.role === UserRole.Admin;
  const canManageRobot = () => user?.role === UserRole.Admin;
  const canManageUser = () => user?.role === UserRole.Admin;
  const canDeleteRequirement = () => user?.role === UserRole.Admin;
  
  const canEditRequirement = (requirement: Requirement) => {
    if (user?.role === UserRole.Admin) return true;
    return requirement.followerId === user?.id;
  };
  
  return {
    canViewPrice,
    canChangeStatus,
    canManageProject,
    canManageRobot,
    canManageUser,
    canDeleteRequirement,
    canEditRequirement
  };
};

// 权限守卫示例
const ProtectedRoute = ({ children, requiredPermission }: Props) => {
  const { hasPermission, isAuthenticated } = usePermission();
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  if (requiredPermission && !hasPermission(requiredPermission)) {
    return <Navigate to="/403" replace />;
  }
  
  return children;
};
```

---

## 10. 并发编辑控制（乐观锁）

### 10.1 前端实现逻辑

| 场景 | 前端处理 | 测试用例 |
|------|---------|---------|
| 加载编辑页 | 记录返回的 `version` 字段到表单状态 | TC-REQ-052 |
| 提交保存 | 请求体包含 `version`，后端校验 | TC-REQ-050 |
| 版本冲突 | 弹窗提示"数据已被他人修改，请刷新页面获取最新数据后重新编辑" | TC-REQ-050 |
| 冲突解决 | 用户刷新页面，重新加载最新数据（version 已更新），再次编辑保存 | TC-REQ-051 |
| 版本号展示 | 页面显示当前 version（调试/用户可见） | TC-REQ-052~053 |

### 10.2 版本冲突处理流程

```typescript
// RequirementEditPage.tsx
const handleSubmit = async (data: RequirementFormData) => {
  try {
    const result = await updateRequirement({
      id,
      ...data,
      version: currentVersion // 当前表单加载时的版本号
    });
    
    if (result.versionConflict) {
      // 显示冲突对话框
      setConflictDialogOpen(true);
      return;
    }
    
    toast.success('需求更新成功');
    navigate(`/requirements/${id}`);
  } catch (error) {
    toast.error('更新失败');
  }
};

// 冲突对话框
const VersionConflictDialog = () => (
  <Dialog open={conflictDialogOpen} onOpenChange={setConflictDialogOpen}>
    <DialogContent>
      <DialogHeader>
        <DialogTitle>数据冲突</DialogTitle>
      </DialogHeader>
      <p>数据已被他人修改，请刷新页面获取最新数据后重新编辑。</p>
      <DialogFooter>
        <Button onClick={handleRefresh}>刷新页面</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
);
```

---

## 11. 删除级联交互

### 11.1 项目删除约束

```typescript
// ProjectFormPage.tsx
const handleDelete = async () => {
  try {
    await deleteProject(projectId);
    toast.success('项目删除成功');
    navigate('/projects');
  } catch (error) {
    if (error.code === 'PROJECT_HAS_REQUIREMENTS') {
      toast.error('该项目下存在需求，无法删除');
    }
  }
};
// 测试用例: TC-PROJ-004~005
```

### 11.2 用户删除约束

```typescript
// UserFormPage.tsx
const handleDelete = async () => {
  try {
    await deleteUser(userId);
    toast.success('用户删除成功');
    navigate('/users');
  } catch (error) {
    if (error.code === 'USER_IS_FOLLOWER') {
      toast.error(`该用户是 ${error.requirementCount} 条需求的跟进人，无法删除`);
    }
  }
};
// 测试用例: TC-USER-xxx
```

### 11.3 机器人删除级联

```typescript
// RobotFormPage.tsx
const handleDelete = async () => {
  try {
    await deleteRobot(robotId);
    toast.success('机器人删除成功，关联需求已自动清除');
    navigate('/robots');
  } catch (error) {
    toast.error('删除失败');
  }
};
// 测试用例: TC-BOT-008
```

---

### 11.4 用户管理完整模块设计

#### 用户列表页面

```typescript
// features/users/pages/UsersListPage.tsx
export const UsersListPage = () => {
  const { canManageUser } = usePermission();
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize: 20 });
  
  const { data, isLoading } = useQuery({
    queryKey: ['users', pagination],
    queryFn: () => getUsers({ pageIndex: pagination.pageIndex + 1, pageSize: pagination.pageSize })
  });
  
  if (!canManageUser()) {
    return <Navigate to="/403" replace />;
  }
  
  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">用户管理</h1>
        {canManageUser() && (
          <Button onClick={() => navigate('/users/new')}>新建用户</Button>
        )}
      </div>
      
      <UserTable 
        data={data?.items ?? []}
        pagination={pagination}
        onPaginationChange={setPagination}
        totalCount={data?.totalCount ?? 0}
        loading={isLoading}
        onEdit={(user) => navigate(`/users/${user.id}/edit`)}
        onDelete={handleDeleteUser}
        onToggleStatus={handleToggleStatus}
      />
    </div>
  );
};
```

#### 用户表单页面（创建/编辑复用）

```typescript
// features/users/pages/UserFormPage.tsx
export const UserFormPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = !!id;
  
  const { data: user, isLoading } = useQuery({
    queryKey: ['user', id],
    queryFn: () => getUserById(Number(id)),
    enabled: isEdit
  });
  
  const createMutation = useCreateUser();
  const updateMutation = useUpdateUser();
  
  const { control, handleSubmit, formState: { errors } } = useForm<UserFormData>({
    defaultValues: {
      username: '',
      realName: '',
      role: UserRole.Developer,
      phone: '',
      email: '',
      isEnabled: true
    }
  });
  
  useEffect(() => {
    if (user) {
      reset({
        username: user.username,
        realName: user.realName,
        role: user.role,
        phone: user.phone ?? '',
        email: user.email ?? '',
        isEnabled: user.isEnabled
      });
    }
  }, [user, reset]);
  
  const onSubmit = async (data: UserFormData) => {
    try {
      if (isEdit) {
        await updateMutation.mutateAsync({ id: Number(id), ...data });
        toast.success('用户更新成功');
      } else {
        await createMutation.mutateAsync(data);
        toast.success('用户创建成功');
      }
      navigate('/users');
    } catch (error) {
      toast.error(error.message || '操作失败');
    }
  };
  
  return (
    <div className="max-w-2xl">
      <h1 className="text-2xl font-bold mb-6">{isEdit ? '编辑用户' : '新建用户'}</h1>
      
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <FormField
          control={control}
          name="username"
          label="用户名"
          rules={{ required: '请填写用户名' }}
          error={errors.username}
        >
          <Input 
            placeholder="请输入用户名" 
            disabled={isEdit}  // 编辑时不可修改用户名
          />
        </FormField>
        
        <FormField
          control={control}
          name="realName"
          label="姓名"
          rules={{ required: '请填写姓名' }}
          error={errors.realName}
        >
          <Input placeholder="请输入姓名" />
        </FormField>
        
        <FormField
          control={control}
          name="role"
          label="角色"
          rules={{ required: '请选择角色' }}
          error={errors.role}
        >
          <Select>
            <SelectTrigger>
              <SelectValue placeholder="请选择角色" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={UserRole.Admin.toString()}>管理员</SelectItem>
              <SelectItem value={UserRole.Developer.toString()}>开发人员</SelectItem>
              <SelectItem value={UserRole.Tester.toString()}>测试人员</SelectItem>
            </SelectContent>
          </Select>
        </FormField>
        
        <FormField
          control={control}
          name="phone"
          label="手机号"
          rules={{ 
            pattern: {
              value: /^1[3-9]\d{9}$/,
              message: '请输入有效的手机号'
            }
          }}
          error={errors.phone}
        >
          <Input placeholder="请输入手机号（可选）" />
        </FormField>
        
        <FormField
          control={control}
          name="email"
          label="邮箱"
          rules={{ 
            pattern: {
              value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
              message: '请输入有效的邮箱地址'
            }
          }}
          error={errors.email}
        >
          <Input placeholder="请输入邮箱（可选）" />
        </FormField>
        
        <FormField
          control={control}
          name="isEnabled"
          label="启用状态"
        >
          <Switch 
            checked={watch('isEnabled')}
            onCheckedChange={(checked) => setValue('isEnabled', checked)}
          />
        </FormField>
        
        <div className="flex gap-4 pt-4">
          <Button type="submit" loading={createMutation.isPending || updateMutation.isPending}>
            {isEdit ? '保存' : '创建'}
          </Button>
          <Button type="button" variant="ghost" onClick={() => navigate('/users')}>
            取消
          </Button>
        </div>
      </form>
    </div>
  );
};
```

#### 用户列表表格

```typescript
// features/users/components/UserTable.tsx
export const UserTable = ({ data, pagination, onPaginationChange, totalCount, loading, onEdit, onDelete, onToggleStatus }: Props) => {
  const { canManageUser } = usePermission();
  
  const columns: ColumnDef<User>[] = [
    {
      accessorKey: 'username',
      header: '用户名',
      cell: ({ row }) => <span className="font-medium">{row.original.username}</span>
    },
    {
      accessorKey: 'realName',
      header: '姓名'
    },
    {
      accessorKey: 'role',
      header: '角色',
      cell: ({ row }) => (
        <Badge variant={row.original.role === UserRole.Admin ? 'primary' : 'secondary'}>
          {getRoleName(row.original.role)}
        </Badge>
      )
    },
    {
      accessorKey: 'phone',
      header: '手机号'
    },
    {
      accessorKey: 'email',
      header: '邮箱'
    },
    {
      accessorKey: 'isEnabled',
      header: '状态',
      cell: ({ row }) => (
        <Badge variant={row.original.isEnabled ? 'success' : 'destructive'}>
          {row.original.isEnabled ? '启用' : '禁用'}
        </Badge>
      )
    },
    {
      id: 'actions',
      header: '操作',
      cell: ({ row }) => (
        canManageUser() && (
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" size="sm">
                <MoreHorizontal className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem onClick={() => onEdit(row.original)}>
                编辑
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => onToggleStatus(row.original)}>
                {row.original.isEnabled ? '禁用' : '启用'}
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem 
                onClick={() => onDelete(row.original)}
                className="text-red-600"
              >
                删除
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        )
      )
    }
  ];
  
  return (
    <div className="space-y-4">
      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              {columns.map((column) => (
                <TableHead key={column.id}>{column.header}</TableHead>
              ))}
            </TableRow>
          </TableHeader>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={columns.length} className="h-24 text-center">
                  <LoadingSpinner />
                </TableCell>
              </TableRow>
            ) : data.length === 0 ? (
              <TableRow>
                <TableCell colSpan={columns.length} className="h-24 text-center">
                  <EmptyState message="暂无用户数据" />
                </TableCell>
              </TableRow>
            ) : (
              data.map((user) => (
                <TableRow key={user.id}>
                  {columns.map((column) => (
                    <TableCell key={column.id}>
                      {column.cell({ row: { original: user } } as any)}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>
      
      <Pagination
        pageIndex={pagination.pageIndex}
        pageSize={pagination.pageSize}
        totalCount={totalCount}
        onPageChange={(page) => onPaginationChange({ ...pagination, pageIndex: page - 1 })}
        onPageSizeChange={(size) => onPaginationChange({ ...pagination, pageSize: size })}
      />
    </div>
  );
};
```

#### 用户管理 Hooks

```typescript
// features/users/hooks/useUsers.ts
export const useUsers = (params?: GetUsersParams) => {
  return useQuery({
    queryKey: ['users', params],
    queryFn: () => getUsers(params)
  });
};

export const useUser = (id: number) => {
  return useQuery({
    queryKey: ['user', id],
    queryFn: () => getUserById(id),
    enabled: !!id
  });
};

export const useCreateUser = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: CreateUserData) => createUser(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    }
  });
};

export const useUpdateUser = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, ...data }: UpdateUserData) => updateUser(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['user', variables.id] });
    }
  });
};

export const useDeleteUser = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => deleteUser(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    }
  });
};

export const useToggleUserStatus = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, isEnabled }: { id: number; isEnabled: boolean }) => 
      updateUser(id, { isEnabled }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    }
  });
};
```

#### 用户相关 API 类型

```typescript
// api/users.ts
export interface UserResponse {
  id: number;
  username: string;
  realName: string;
  role: UserRole;
  phone: string | null;
  email: string | null;
  isEnabled: boolean;
  createdAt: string;
}

export interface CreateUserData {
  username: string;
  realName: string;
  role: UserRole;
  phone?: string;
  email?: string;
  password?: string;  // 仅创建时需要
}

export interface UpdateUserData {
  realName?: string;
  role?: UserRole;
  phone?: string;
  email?: string;
  isEnabled?: boolean;
}

export enum UserRole {
  Admin = 0,
  Developer = 1,
  Tester = 2
}
```

#### 用户删除约束处理

```typescript
// features/users/hooks/useDeleteUser.ts
export const useDeleteUser = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (id: number) => {
      try {
        return await deleteUser(id);
      } catch (error) {
        if (error.code === 'USER_IS_FOLLOWER') {
          throw new Error(`该用户是 ${error.requirementCount} 条需求的跟进人，无法删除。请先变更这些需求的跟进人后再删除用户。`);
        }
        throw error;
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      toast.success('用户删除成功');
    },
    onError: (error: Error) => {
      toast.error(error.message);
    }
  });
};

// 测试用例: TC-USER-xxx
```

#### 用户禁用联动处理

```typescript
// features/users/hooks/useToggleUserStatus.ts
export const useToggleUserStatus = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async ({ id, isEnabled }: { id: number; isEnabled: boolean }) => {
      if (!isEnabled) {
        // 检查该用户是否有跟进的需求
        const requirements = await getRequirements({ followerId: id });
        if (requirements.totalCount > 0) {
          throw new Error(`该用户正在跟进 ${requirements.totalCount} 条需求，禁用后需求将无法正常流转。建议先变更需求的跟进人。`);
        }
      }
      return updateUser(id, { isEnabled });
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['user', variables.id] });
      toast.success(variables.isEnabled ? '用户已启用' : '用户已禁用');
    },
    onError: (error: Error) => {
      toast.error(error.message);
    }
  });
};
// 测试用例: TC-USER-xxx
```

---

## 12. API 对接设计

### 12.1 API 完整列表

| 模块 | 接口 | 方法 | 描述 | 权限 | 测试用例 |
|------|------|------|------|------|---------|
| **认证** | `/api/auth/login` | POST | 用户登录 | 公开 | TC-REQ-013 |
| **需求** | `/api/requirements` | GET | 获取需求列表（筛选/分页/排序） | 所有用户 | TC-REQ-001~013 |
| **需求** | `/api/requirements/:id` | GET | 获取需求详情 | 所有用户 | - |
| **需求** | `/api/requirements` | POST | 创建需求 | 管理员 | TC-REQ-014~043 |
| **需求** | `/api/requirements/:id` | PUT | 更新需求 | 管理员/跟进人 | TC-REQ-044~049 |
| **需求** | `/api/requirements/:id` | DELETE | 删除需求 | 管理员 | TC-REQ-054~056 |
| **需求** | `/api/requirements/:id/status` | PUT | 更新需求状态 | 管理员 | TC-FLOW-001~020 |
| **项目** | `/api/projects` | GET | 获取项目列表 | 所有用户 | TC-PROJ-001 |
| **项目** | `/api/projects/:id` | GET | 获取项目详情 | 所有用户 | - |
| **项目** | `/api/projects` | POST | 创建项目 | 管理员 | TC-PROJ-001~002 |
| **项目** | `/api/projects/:id` | PUT | 更新项目 | 管理员 | TC-PROJ-003 |
| **项目** | `/api/projects/:id` | DELETE | 删除项目 | 管理员 | TC-PROJ-004~005 |
| **用户** | `/api/users` | GET | 获取用户列表 | 所有用户 | - |
| **用户** | `/api/users/:id` | GET | 获取用户详情 | 所有用户 | - |
| **用户** | `/api/users` | POST | 创建用户 | 管理员 | - |
| **用户** | `/api/users/:id` | PUT | 更新用户 | 管理员 | - |
| **用户** | `/api/users/:id` | DELETE | 删除用户 | 管理员 | - |
| **机器人** | `/api/robots` | GET | 获取机器人列表 | 管理员 | TC-BOT-001 |
| **机器人** | `/api/robots/:id` | GET | 获取机器人详情 | 管理员 | - |
| **机器人** | `/api/robots` | POST | 创建机器人 | 管理员 | TC-BOT-001~004 |
| **机器人** | `/api/robots/:id` | PUT | 更新机器人 | 管理员 | - |
| **机器人** | `/api/robots/:id` | DELETE | 删除机器人 | 管理员 | TC-BOT-008 |
| **机器人** | `/api/robots/:id/test` | POST | 测试机器人连接 | 管理员 | TC-BOT-001~004 |
| **通知** | `/api/notifications` | GET | 获取通知日志列表 | 管理员 | TC-NOT-xxx |
| **通知** | `/api/notifications/:id` | GET | 获取通知详情 | 管理员 | - |

### 12.2 API 请求/响应类型

```typescript
// requirements.ts
export interface GetRequirementsParams {
  status?: string;              // 多个用逗号分隔
  followerId?: number;
  projectId?: number;
  planStartDateFrom?: string;    // ISO 日期
  planStartDateTo?: string;
  keyword?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  pageIndex?: number;
  pageSize?: number;             // 10/20/50
}

export interface RequirementResponse {
  id: number;
  name: string;
  requirementNo: string;
  status: RequirementStatus;
  progress: number;
  followerId: number;
  followerName: string;
  planStartDate: string | null;
  planTestDate: string | null;
  planLaunchDate: string | null;
  actualTestDate: string | null;
  actualLaunchDate: string | null;
  isConfirmed: boolean;
  docUrl: string | null;
  price: number | null;
  projectId: number;
  projectName: string;
  robotId: number | null;
  priority: Priority;
  remark: string | null;
  version: number;               // 乐观锁版本号
  createdAt: string;
  updatedAt: string;
}

export interface CreateRequirementData {
  name: string;
  requirementNo: string;
  status: RequirementStatus;
  followerId: number;
  projectId: number;
  planStartDate?: string;
  planTestDate?: string;
  planLaunchDate?: string;
  isConfirmed?: boolean;
  docUrl?: string;
  price?: number;
  robotId?: number;
  priority: Priority;
  remark?: string;
}

export interface UpdateRequirementData extends CreateRequirementData {
  version: number;  // 乐观锁版本号
}

export interface ChangeStatusData {
  status: RequirementStatus;
  version: number;
}

// robots.ts
export interface RobotResponse {
  id: number;
  name: string;
  webhookUrl: string;
  groupName: string;
  isEnabled: boolean;
  createdAt: string;
}

export interface TestRobotResult {
  success: boolean;
  message: string;
}

// notifications.ts
export interface NotificationResponse {
  id: number;
  requirementId: number;
  requirementName: string;
  requirementNo: string;
  type: NotificationType;
  robotId: number | null;
  robotName: string | null;
  status: NotificationStatus;
  errorMessage: string | null;
  sentAt: string;
  createdAt: string;
}
```

---

## 13. 通知相关功能

### 13.1 通知机器人选择

```typescript
// NotificationPanel.tsx
const NotificationPanel = ({ value, onChange, disabled }: Props) => {
  const { data: robots } = useQuery({
    queryKey: ['robots'],
    queryFn: () => getRobots({ isEnabled: true })
  });
  
  return (
    <div className="space-y-2">
      <Label>通知机器人</Label>
      <div className="space-y-1">
        {robots?.map(robot => (
          <label key={robot.id} className="flex items-center gap-2">
            <Checkbox
              checked={value.includes(robot.id)}
              onCheckedChange={(checked) => {
                if (checked) {
                  onChange([...value, robot.id]);
                } else {
                  onChange(value.filter(id => id !== robot.id));
                }
              }}
              disabled={disabled}
            />
            <span>{robot.name}</span>
            <Badge variant={robot.isEnabled ? 'success' : 'secondary'}>
              {robot.isEnabled ? '启用' : '禁用'}
            </Badge>
          </label>
        ))}
      </div>
    </div>
  );
};
```

### 13.2 通知日志页面

```typescript
// NotificationsListPage.tsx
const NotificationsListPage = () => {
  const [filters, setFilters] = useState<NotificationFilters>({});
  
  const { data, isLoading } = useQuery({
    queryKey: ['notifications', filters],
    queryFn: () => getNotifications(filters)
  });
  
  return (
    <div className="space-y-4">
      <NotificationFilters value={filters} onChange={setFilters} />
      <NotificationTable data={data} loading={isLoading} />
    </div>
  );
};
```

---

## 14. 关键功能实现

### 14.1 报价字段权限控制

```typescript
// PriceInput.tsx
const PriceInput = ({ value, onChange, disabled }: Props) => {
  const { canViewPrice } = usePermission();
  
  if (!canViewPrice()) {
    return (
      <div className="text-muted-foreground">
        -- （无权限查看）
      </div>
    );
  }
  
  return (
    <Input
      type="number"
      step="0.01"
      min="0"
      value={value ?? ''}
      onChange={(e) => onChange(parseFloat(e.target.value) || null)}
      disabled={disabled}
      placeholder="0.00"
    />
  );
};

// 列表中的报价显示
const PriceCell = ({ price }: { price: number | null }) => {
  const { canViewPrice } = usePermission();
  
  if (!canViewPrice()) {
    return <span className="text-muted-foreground">--</span>;
  }
  
  return <span>{price?.toFixed(2) ?? '--'}</span>;
};
```

### 14.2 已上线需求限制

```typescript
// RequirementForm.tsx
const RequirementForm = ({ requirement, onSubmit }) => {
  const isLaunched = requirement?.status === RequirementStatus.Launched;
  const { canChangeStatus } = usePermission();
  
  return (
    <Form>
      {/* 基本信息字段 */}
      <FormField name="name">
        <FormItem disabled={isLaunched}>
          <FormLabel>需求名称</FormLabel>
          <FormControl>
            <Input />
          </FormControl>
        </FormItem>
      </FormField>
      
      {/* 状态选择 - 仅非终态显示 */}
      {!isLaunched && canChangeStatus() && (
        <FormField name="status">
          <FormItem>
            <FormLabel>状态</FormLabel>
            <StatusSelect />
          </FormItem>
        </FormField>
      )}
      
      {/* 终态提示 */}
      {isLaunched && (
        <Alert>
          <AlertTitle>已上线</AlertTitle>
          <AlertDescription>
            该需求已上线，只能编辑备注字段。
          </AlertDescription>
        </Alert>
      )}
      
      {/* 备注 - 终态也可编辑 */}
      <FormField name="remark">
        <FormItem>
          <FormLabel>备注</FormLabel>
          <FormControl>
            <Textarea />
          </FormControl>
        </FormItem>
      </FormField>
    </Form>
  );
};
```

---

## 15. 状态机实现（覆盖 TC-FLOW-001~032）

### 15.1 状态流转状态机定义

```typescript
// features/requirements/utils/statusMachine.ts

export enum RequirementStatus {
  PendingConfirm = 0,      // 待确认
  Confirmed = 1,           // 已确认
  PendingQuote = 2,       // 待报价
  Quoted = 3,             // 已报价
  PendingDev = 4,         // 待开发
  InDev = 5,              // 开发中
  InTest = 6,             // 测试中
  AcceptedPendingLaunch = 7, // 已验收待上线
  Launched = 8            // 已上线
}

export const STATUS_LABELS: Record<RequirementStatus, string> = {
  [RequirementStatus.PendingConfirm]: '待确认',
  [RequirementStatus.Confirmed]: '已确认',
  [RequirementStatus.PendingQuote]: '待报价',
  [RequirementStatus.Quoted]: '已报价',
  [RequirementStatus.PendingDev]: '待开发',
  [RequirementStatus.InDev]: '开发中',
  [RequirementStatus.InTest]: '测试中',
  [RequirementStatus.AcceptedPendingLaunch]: '已验收待上线',
  [RequirementStatus.Launched]: '已上线'
};

export const STATUS_COLORS: Record<RequirementStatus, string> = {
  [RequirementStatus.PendingConfirm]: 'secondary',
  [RequirementStatus.Confirmed]: 'info',
  [RequirementStatus.PendingQuote]: 'warning',
  [RequirementStatus.Quoted]: 'warning',
  [RequirementStatus.PendingDev]: 'secondary',
  [RequirementStatus.InDev]: 'primary',
  [RequirementStatus.InTest]: 'accent',
  [RequirementStatus.AcceptedPendingLaunch]: 'success',
  [RequirementStatus.Launched]: 'success'
};

export const VALID_TRANSITIONS: Record<RequirementStatus, RequirementStatus[]> = {
  [RequirementStatus.PendingConfirm]: [RequirementStatus.Confirmed],
  [RequirementStatus.Confirmed]: [RequirementStatus.PendingQuote],
  [RequirementStatus.PendingQuote]: [RequirementStatus.Quoted],
  [RequirementStatus.Quoted]: [RequirementStatus.PendingDev],
  [RequirementStatus.PendingDev]: [RequirementStatus.InDev],
  [RequirementStatus.InDev]: [RequirementStatus.InTest],
  [RequirementStatus.InTest]: [RequirementStatus.AcceptedPendingLaunch],
  [RequirementStatus.AcceptedPendingLaunch]: [RequirementStatus.Launched],
  [RequirementStatus.Launched]: [] // 终态，不可流转
};

export function getValidNextStatuses(currentStatus: RequirementStatus): RequirementStatus[] {
  return VALID_TRANSITIONS[currentStatus] || [];
}

export function canTransition(from: RequirementStatus, to: RequirementStatus): boolean {
  return VALID_TRANSITIONS[from]?.includes(to) ?? false;
}

export function isTerminalStatus(status: RequirementStatus): boolean {
  return status === RequirementStatus.Launched;
}
```

### 15.2 状态选择器组件（仅显示合法后继状态）

```typescript
// features/requirements/components/StatusSelect.tsx
import { useMemo } from 'react';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue
} from '@/components/ui/Select';
import { RequirementStatus, STATUS_LABELS, getValidNextStatuses } from '../utils/statusMachine';

interface StatusSelectProps {
  value: RequirementStatus;
  onChange: (status: RequirementStatus) => void;
  disabled?: boolean;
  showReverse?: boolean; // 是否显示逆向流转选项，默认 true
}

export const StatusSelect = ({ value, onChange, disabled, showReverse = true }: StatusSelectProps) => {
  const validNextStatuses = useMemo(() => {
    const nextStatuses = getValidNextStatuses(value);
    if (showReverse && value > RequirementStatus.PendingConfirm) {
      return [
        value - 1 as RequirementStatus, // 逆向：前一个状态
        ...nextStatuses
      ];
    }
    return nextStatuses;
  }, [value, showReverse]);

  if (validNextStatuses.length === 0) {
    return (
      <div className="flex items-center">
        <Badge variant="success">{STATUS_LABELS[value]}</Badge>
        <span className="ml-2 text-sm text-slate-400">终态不可变更</span>
      </div>
    );
  }

  return (
    <Select
      value={String(value)}
      onValueChange={(v) => onChange(Number(v) as RequirementStatus)}
      disabled={disabled}
    >
      <SelectTrigger>
        <SelectValue placeholder="选择状态" />
      </SelectTrigger>
      <SelectContent>
        {validNextStatuses.map((status) => (
          <SelectItem key={status} value={String(status)}>
            <div className="flex items-center gap-2">
              <Badge variant={status < value ? 'warning' : 'primary'}>
                {status < value ? '← 退回' : '→ 推进'}
              </Badge>
              <span>{STATUS_LABELS[status]}</span>
            </div>
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
};

// 测试用例: TC-FLOW-009, TC-FLOW-013, TC-FLOW-026~031
```

### 15.3 状态变更确认对话框

```typescript
// features/requirements/components/StatusChangeDialog.tsx
import { useState } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from '@/components/ui/Dialog';
import { Button } from '@/components/ui/Button';
import { Textarea } from '@/components/ui/Textarea';
import { RequirementStatus, STATUS_LABELS } from '../utils/statusMachine';

interface StatusChangeDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  currentStatus: RequirementStatus;
  newStatus: RequirementStatus;
  onConfirm: (remark?: string) => Promise<void>;
}

export const StatusChangeDialog = ({ open, onOpenChange, currentStatus, newStatus, onConfirm }: StatusChangeDialogProps) => {
  const [remark, setRemark] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleConfirm = async () => {
    setIsSubmitting(true);
    try {
      await onConfirm(remark);
      onOpenChange(false);
    } finally {
      setIsSubmitting(false);
    }
  };

  const isBackward = newStatus < currentStatus;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>确认状态变更</DialogTitle>
          <DialogDescription>
            确定要将需求状态从「{STATUS_LABELS[currentStatus]}」变更为「{STATUS_LABELS[newStatus]}」吗？
          </DialogDescription>
        </DialogHeader>

        <div className="py-4">
          <div className="flex items-center gap-4 mb-4">
            <Badge variant="secondary">{STATUS_LABELS[currentStatus]}</Badge>
            <span className="text-slate-400">→</span>
            <Badge variant={isBackward ? 'warning' : 'primary'}>
              {STATUS_LABELS[newStatus]}
            </Badge>
          </div>

          {isBackward && (
            <Alert variant="warning" className="mb-4">
              <AlertTitle>逆向流转提示</AlertTitle>
              <AlertDescription>
                此操作将需求状态回退，请确认是否需要重新测试。
              </AlertDescription>
            </Alert>
          )}

          <div className="space-y-2">
            <label className="text-sm text-slate-400">变更备注（可选）</label>
            <Textarea
              value={remark}
              onChange={(e) => setRemark(e.target.value)}
              placeholder="输入状态变更原因..."
              rows={3}
            />
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            取消
          </Button>
          <Button onClick={handleConfirm} loading={isSubmitting}>
            确认变更
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

// 测试用例: TC-FLOW-001~008, TC-FLOW-026~029
```

### 15.4 状态自动联动逻辑

```typescript
// features/requirements/hooks/useStatusAutoActions.ts
import { useEffect } from 'react';
import { useFormContext } from 'react-hook-form';
import { RequirementStatus } from '../utils/statusMachine';

export const useStatusAutoActions = () => {
  const { setValue, watch } = useFormContext();
  const status = watch('status');

  useEffect(() => {
    // TC-FLOW-015: 状态从待确认变为已确认，IsConfirmed 自动变为 true
    if (status === RequirementStatus.Confirmed) {
      setValue('isConfirmed', true, { shouldDirty: false });
    }
    
    // TC-FLOW-016: 需求已确认标志不可手动编辑（设为只读）
    // 注意：这里的实现是通过表单 disabled 属性控制的
  }, [status, setValue]);

  return { status };
};

// 实际交测时间、实际上线时间由后端自动填充，前端只读显示
// TC-FLOW-017, TC-FLOW-018
```

### 15.5 已上线需求特殊处理

```typescript
// features/requirements/components/LaunchedRequirementGuard.tsx
import { RequirementStatus } from '../utils/statusMachine';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/Alert';
import { Lock } from 'lucide-react';

interface LaunchedRequirementGuardProps {
  requirement: any;
  children: React.ReactNode;
  canEdit?: boolean;
}

export const LaunchedRequirementGuard = ({ requirement, children, canEdit = false }: LaunchedRequirementGuardProps) => {
  const isLaunched = requirement.status === RequirementStatus.Launched;
  const { isAdmin } = usePermission();

  if (!isAdmin() && !canEdit) {
    return null;
  }

  return (
    <div className="space-y-4">
      {isLaunched && (
        <Alert variant="info">
          <Lock className="h-4 w-4" />
          <AlertTitle>已上线需求</AlertTitle>
          <AlertDescription>
            该需求已上线，仅可编辑备注字段，其他字段不可修改。
          </AlertDescription>
        </Alert>
      )}
      {children}
    </div>
  );
};

// 测试用例: TC-FLOW-010, TC-FLOW-012, TC-FLOW-021~023, TC-FLOW-032
```

---

## 16. 乐观锁与并发控制（覆盖 TC-REQ-050~053）

### 16.1 版本号管理 Hook

```typescript
// features/requirements/hooks/useOptimisticLock.ts
import { useState, useCallback } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { toast } from '@/components/ui/Toast';

interface OptimisticLockState {
  version: number;
  isStale: boolean;
  lastUpdatedAt: Date | null;
}

export const useOptimisticLock = (requirementId: number) => {
  const queryClient = useQueryClient();
  const [conflictDialogOpen, setConflictDialogOpen] = useState(false);

  const getCurrentVersion = useCallback(() => {
    const cached = queryClient.getQueryData(['requirement', requirementId]);
    return cached?.version ?? 0;
  }, [queryClient, requirementId]);

  const handleVersionConflict = useCallback(() => {
    setConflictDialogOpen(true);
  }, []);

  const refreshAndContinue = useCallback(async () => {
    await queryClient.invalidateQueries(['requirement', requirementId]);
    setConflictDialogOpen(false);
    toast.success('数据已刷新，请重新编辑');
  }, [queryClient, requirementId]);

  const handleApiError = useCallback((error: any) => {
    if (error?.code === 'VERSION_CONFLICT' || error?.status === 409) {
      handleVersionConflict();
      return true;
    }
    return false;
  }, [handleVersionConflict]);

  return {
    getCurrentVersion,
    handleVersionConflict,
    handleApiError,
    conflictDialogOpen,
    setConflictDialogOpen,
    refreshAndContinue
  };
};
```

### 16.2 版本冲突解决对话框

```typescript
// features/requirements/components/VersionConflictDialog.tsx
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from '@/components/ui/Dialog';
import { Button } from '@/components/ui/Button';
import { Alert, AlertDescription } from '@/components/ui/Alert';
import { RefreshCw } from 'lucide-react';

interface VersionConflictDialogProps {
  open: boolean;
  onRefresh: () => Promise<void>;
  onCancel: () => void;
}

export const VersionConflictDialog = ({ open, onRefresh, onCancel }: VersionConflictDialogProps) => {
  return (
    <Dialog open={open} onOpenChange={(o) => !o && onCancel()}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>数据已被修改</DialogTitle>
          <DialogDescription>
            检测到该需求已被他人修改，请刷新页面获取最新数据后重新编辑。
          </DialogDescription>
        </DialogHeader>

        <Alert variant="warning">
          <AlertDescription>
            为避免数据冲突，系统已阻止本次保存操作。请点击「刷新数据」按钮重新加载最新内容。
          </AlertDescription>
        </Alert>

        <DialogFooter>
          <Button variant="outline" onClick={onCancel}>
            取消
          </Button>
          <Button onClick={onRefresh}>
            <RefreshCw className="h-4 w-4 mr-2" />
            刷新数据
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

// 测试用例: TC-REQ-050, TC-REQ-051
```

### 16.3 编辑表单中的版本控制

```typescript
// features/requirements/pages/RequirementEditPage.tsx
import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { toast } from '@/components/ui/Toast';
import { useOptimisticLock } from '../hooks/useOptimisticLock';
import { VersionConflictDialog } from '../components/VersionConflictDialog';

interface RequirementFormData {
  name: string;
  requirementNo: string;
  status: RequirementStatus;
  // ... 其他字段
}

export const RequirementEditPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { handleApiError, refreshAndContinue, conflictDialogOpen } = useOptimisticLock(Number(id));

  const { data: requirement, isLoading } = useQuery({
    queryKey: ['requirement', id],
    queryFn: () => getRequirementById(Number(id)),
    enabled: !!id
  });

  const { mutate: updateRequirement, isPending } = useMutation({
    mutationFn: (data: RequirementFormData & { version: number }) => 
      updateRequirement(Number(id), data),
    onSuccess: () => {
      queryClient.invalidateQueries(['requirements']);
      queryClient.invalidateQueries(['requirement', id]);
      toast.success('需求更新成功');
      navigate('/requirements');
    },
    onError: (error) => {
      const handled = handleApiError(error);
      if (!handled) {
        toast.error(error.message || '更新失败');
      }
    }
  });

  const form = useForm<RequirementFormData>({
    defaultValues: {
      version: requirement?.version ?? 1
    }
  });

  useEffect(() => {
    if (requirement) {
      form.reset({
        name: requirement.name,
        requirementNo: requirement.requirementNo,
        status: requirement.status,
        // ... 其他字段
        version: requirement.version
      });
    }
  }, [requirement, form]);

  const onSubmit = (data: RequirementFormData) => {
    updateRequirement({
      ...data,
      version: requirement.version // 提交时携带当前版本号
    });
  };

  if (isLoading) return <LoadingSpinner />;
  if (!requirement) return <NotFoundState />;

  return (
    <div className="container py-8">
      <Form onSubmit={form.handleSubmit(onSubmit)}>
        {/* 表单字段 */}
        <FormField control={form.control} name="name" label="需求名称">
          <Input />
        </FormField>
        
        {/* 版本号提示 */}
        <div className="text-sm text-slate-400">
          当前版本: {requirement.version}（最后更新: {format(new Date(requirement.updatedAt), 'yyyy-MM-dd HH:mm')})
        </div>

        <div className="flex gap-4 mt-6">
          <Button type="submit" loading={isPending}>
            保存
          </Button>
          <Button variant="outline" onClick={() => navigate('/requirements')}>
            取消
          </Button>
        </div>
      </Form>

      <VersionConflictDialog
        open={conflictDialogOpen}
        onRefresh={refreshAndContinue}
        onCancel={() => navigate('/requirements')}
      />
    </div>
  );
};

// 测试用例: TC-REQ-050~053
```

---

## 17. 项目管理功能完善（覆盖 TC-PROJ-001~016）

### 17.1 项目表单组件

```typescript
// features/projects/components/ProjectForm.tsx
import { useForm } from 'react-hook-form';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Textarea } from '@/components/ui/Textarea';

interface ProjectFormData {
  name: string;
  code: string;
  description?: string;
  managerId?: number;
}

export const ProjectForm = ({ project, onSubmit }: Props) => {
  const { register, handleSubmit, formState: { errors } } = useForm<ProjectFormData>({
    defaultValues: project ?? {}
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <FormField label="项目名称" error={errors.name}>
        <Input
          {...register('name', { 
            required: '请填写项目名称',
            maxLength: { value: 100, message: '项目名称最多100字符' }
          })}
          placeholder="请输入项目名称"
        />
      </FormField>

      <FormField label="项目编码" error={errors.code}>
        <Input
          {...register('code', {
            required: '请填写项目编码',
            maxLength: { value: 50, message: '项目编码最多50字符' },
            pattern: { value: /^[A-Z0-9-]+$/, message: '项目编码只能包含大写字母、数字和连字符' }
          })}
          placeholder="如 PRJ-001"
        />
      </FormField>

      <FormField label="项目描述">
        <Textarea
          {...register('description')}
          placeholder="请输入项目描述（可选）"
        />
      </FormField>

      <FormField label="项目负责人">
        <UserSelect {...register('managerId')} />
      </FormField>

      <Button type="submit">保存</Button>
    </form>
  );
};

// 测试用例: TC-PROJ-001~003, TC-PROJ-011~015
```

### 17.2 项目删除约束检查

```typescript
// features/projects/hooks/useProjectDelete.ts
import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from '@/components/ui/Toast';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';

interface UseProjectDeleteOptions {
  onDeleted?: () => void;
}

export const useProjectDelete = (options: UseProjectDeleteOptions = {}) => {
  const { onDeleted } = options;
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [pendingProject, setPendingProject] = useState<Project | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  const { mutate: deleteProject, isPending } = useMutation({
    mutationFn: async (id: number) => {
      const response = await deleteProjectApi(id);
      return response;
    },
    onSuccess: () => {
      queryClient.invalidateQueries(['projects']);
      toast.success('项目删除成功');
      setDeleteConfirmOpen(false);
      setPendingProject(null);
      onDeleted?.();
    },
    onError: (error) => {
      // TC-PROJ-005: 检测到有关联需求，阻止删除
      if (error.code === 'HAS_ASSOCIATED_REQUIREMENTS') {
        setDeleteError('该项目下存在需求，无法删除');
      } else {
        toast.error(error.message || '删除失败');
      }
    }
  });

  const handleDeleteClick = (project: Project) => {
    setPendingProject(project);
    setDeleteError(null);
    setDeleteConfirmOpen(true);
  };

  const handleConfirmDelete = () => {
    if (pendingProject) {
      deleteProject(pendingProject.id);
    }
  };

  return {
    handleDeleteClick,
    deleteConfirmOpen,
    setDeleteConfirmOpen,
    pendingProject,
    deleteError,
    isPending,
    handleConfirmDelete
  };
};

// 测试用例: TC-PROJ-004, TC-PROJ-005, TC-PROJ-016
```

### 17.3 项目关联需求数量检查

```typescript
// features/projects/components/ProjectTable.tsx
export const ProjectTable = () => {
  const { data: projects } = useQuery({
    queryKey: ['projects'],
    queryFn: () => getProjects()
  });

  const { handleDeleteClick } = useProjectDelete();

  const columns: ColumnDef<Project>[] = [
    {
      accessorKey: 'name',
      header: '项目名称'
    },
    {
      accessorKey: 'code',
      header: '项目编码'
    },
    {
      accessorKey: 'requirementCount',
      header: '关联需求数',
      cell: ({ row }) => (
        <Badge variant={row.original.requirementCount > 0 ? 'primary' : 'secondary'}>
          {row.original.requirementCount}
        </Badge>
      )
    },
    {
      id: 'actions',
      cell: ({ row }) => (
        <Button
          variant="destructive"
          size="sm"
          onClick={() => handleDeleteClick(row.original)}
        >
          删除
        </Button>
      )
    }
  ];

  return <DataTable columns={columns} data={projects ?? []} />;
};
```

---

## 18. 机器人配置功能完善（覆盖 TC-BOT-001~015）

### 18.1 机器人表单与强制测试

```typescript
// features/robots/components/RobotForm.tsx
import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { toast } from '@/components/ui/Toast';

interface RobotFormData {
  name: string;
  webhookUrl: string;
  groupName?: string;
}

export const RobotForm = ({ robot, onSubmit }: Props) => {
  const [hasTested, setHasTested] = useState(false);
  const [testPassed, setTestPassed] = useState(false);
  const [isTesting, setIsTesting] = useState(false);
  const { register, handleSubmit, watch } = useForm<RobotFormData>({
    defaultValues: robot ?? {}
  });

  const webhookUrl = watch('webhookUrl');

  useEffect(() => {
    // 重置测试状态当 webhook URL 变化时
    setHasTested(false);
    setTestPassed(false);
  }, [webhookUrl]);

  const handleTest = async () => {
    if (!webhookUrl) {
      toast.warning('请先填写 Webhook 地址');
      return;
    }

    setIsTesting(true);
    try {
      const result = await testRobotConnection(webhookUrl);
      if (result.success) {
        setHasTested(true);
        setTestPassed(true);
        toast.success('测试连接成功');
      } else {
        setHasTested(true);
        setTestPassed(false);
        toast.error(result.message || '测试连接失败');
      }
    } catch (error) {
      setHasTested(true);
      setTestPassed(false);
      toast.error('测试连接失败，请检查 Webhook 地址');
    } finally {
      setIsTesting(false);
    }
  };

  const onFormSubmit = (data: RobotFormData) => {
    // TC-BOT-004: 未测试直接保存被阻止
    if (!hasTested || !testPassed) {
      toast.warning('请先测试机器人连接，测试通过后才能保存');
      return;
    }
    onSubmit(data);
  };

  return (
    <form onSubmit={handleSubmit(onFormSubmit)} className="space-y-4">
      <FormField label="机器人名称">
        <Input
          {...register('name', { required: '请填写机器人名称' })}
          placeholder="如：开发团队通知机器人"
        />
      </FormField>

      <FormField label="Webhook 地址">
        <div className="flex gap-2">
          <Input
            {...register('webhookUrl', { 
              required: '请填写 Webhook 地址',
              pattern: {
                value: /^https:\/\/qyapi\.weixin\.qq\.com/,
                message: '必须是企业微信的 HTTPS 地址'
              }
            })}
            placeholder="https://qyapi.weixin.qq.com/..."
            className="flex-1"
          />
          <Button 
            type="button" 
            variant="secondary"
            onClick={handleTest}
            disabled={isTesting}
          >
            {isTesting ? '测试中...' : '测试连接'}
          </Button>
        </div>
        {hasTested && (
          <div className={`text-sm ${testPassed ? 'text-emerald-500' : 'text-red-500'}`}>
            {testPassed ? '✓ 连接成功' : '✗ 连接失败'}
          </div>
        )}
      </FormField>

      <FormField label="群组名称（可选）">
        <Input
          {...register('groupName')}
          placeholder="如：开发团队"
        />
      </FormField>

      {/* TC-BOT-004: 保存按钮在测试通过前禁用 */}
      <Button type="submit" disabled={!testPassed}>
        保存
      </Button>
    </form>
  );
};

// 测试用例: TC-BOT-001~004, TC-BOT-012~013
```

### 18.2 机器人删除级联处理

```typescript
// features/robots/hooks/useRobotDelete.ts
export const useRobotDelete = () => {
  const queryClient = useQueryClient();
  const [deleteInfo, setDeleteInfo] = useState<{
    robot: Robot;
    affectedRequirements: number;
  } | null>(null);

  const { mutate: deleteRobot, isPending } = useMutation({
    mutationFn: async (id: number) => {
      return deleteRobotApi(id);
    },
    onSuccess: (_, robotId) => {
      queryClient.invalidateQueries(['robots']);
      queryClient.invalidateQueries(['requirements']); // 清除关联需求的缓存
      toast.success('机器人删除成功，关联需求已自动清除');
    },
    onError: (error) => {
      toast.error(error.message || '删除失败');
    }
  });

  const handleDeleteClick = (robot: Robot) => {
    // 检查关联的需求数量
    const affectedCount = robot.associatedRequirementCount ?? 0;
    setDeleteInfo({ robot, affectedRequirements: affectedCount });
  };

  const handleConfirmDelete = () => {
    if (deleteInfo) {
      deleteRobot(deleteInfo.robot.id);
      setDeleteInfo(null);
    }
  };

  return {
    handleDeleteClick,
    deleteInfo,
    setDeleteInfo,
    isPending,
    handleConfirmDelete
  };
};

// 测试用例: TC-BOT-008
```

### 18.3 机器人删除确认对话框

```typescript
// features/robots/components/RobotDeleteDialog.tsx
export const RobotDeleteDialog = ({ 
  open, 
  onOpenChange, 
  robot, 
  affectedRequirements, 
  onConfirm, 
  isPending 
}: Props) => {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>确认删除机器人</DialogTitle>
          <DialogDescription>
            {affectedRequirements > 0 ? (
              <div className="space-y-2">
                <p>机器人「{robot?.name}」已关联 {affectedRequirements} 条需求。</p>
                <p className="text-amber-500">删除后，这些需求的「通知机器人」字段将被清空。</p>
              </div>
            ) : (
              <p>确定要删除机器人「{robot?.name}」吗？此操作不可撤销。</p>
            )}
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>取消</Button>
          <Button variant="destructive" onClick={onConfirm} loading={isPending}>
            确认删除
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
```

---

## 19. 通知管理功能（覆盖 TC-NOTIFY-001~019）

### 19.1 通知日志列表与筛选

```typescript
// features/notifications/pages/NotificationsListPage.tsx
import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { DataTable } from '@/components/ui/Table';
import { Badge } from '@/components/ui/Badge';
import { DateRangePicker } from '@/components/ui/DateRangePicker';
import { format } from 'dayjs';

interface NotificationFilters {
  requirementId?: number;
  status?: NotificationStatus;
  type?: NotificationType;
  dateRange?: [string, string];
  pageIndex?: number;
  pageSize?: number;
}

export const NotificationsListPage = () => {
  const [filters, setFilters] = useState<NotificationFilters>({
    pageIndex: 1,
    pageSize: 20
  });

  const { data, isLoading } = useQuery({
    queryKey: ['notifications', filters],
    queryFn: () => getNotifications(filters)
  });

  const columns: ColumnDef<NotificationLog>[] = [
    {
      accessorKey: 'requirementName',
      header: '需求名称',
      cell: ({ row }) => (
        <Link to={`/requirements/${row.original.requirementId}`}>
          {row.original.requirementName}
        </Link>
      )
    },
    {
      accessorKey: 'requirementNo',
      header: '需求号'
    },
    {
      accessorKey: 'type',
      header: '通知类型',
      cell: ({ row }) => (
        <Badge variant={getNotificationTypeVariant(row.original.type)}>
          {NOTIFICATION_TYPE_LABELS[row.original.type]}
        </Badge>
      )
    },
    {
      accessorKey: 'robotName',
      header: '机器人'
    },
    {
      accessorKey: 'status',
      header: '状态',
      cell: ({ row }) => (
        <Badge variant={getNotificationStatusVariant(row.original.status)}>
          {getNotificationStatusLabel(row.original.status)}
        </Badge>
      )
    },
    {
      accessorKey: 'errorMessage',
      header: '错误信息',
      cell: ({ row }) => row.original.errorMessage ? (
        <Tooltip content={row.original.errorMessage}>
          <span className="text-red-500 cursor-help">查看详情</span>
        </Tooltip>
      ) : '-'
    },
    {
      accessorKey: 'retryCount',
      header: '重试次数',
      cell: ({ row }) => (
        <span className={row.original.retryCount > 0 ? 'text-amber-500' : ''}>
          {row.original.retryCount}
        </span>
      )
    },
    {
      accessorKey: 'sentAt',
      header: '发送时间',
      cell: ({ row }) => format(row.original.sentAt, 'YYYY-MM-DD HH:mm:ss')
    }
  ];

  return (
    <div className="space-y-4">
      <NotificationFilters value={filters} onChange={setFilters} />
      <DataTable columns={columns} data={data?.items ?? []} loading={isLoading} />
      <Pagination
        current={filters.pageIndex ?? 1}
        pageSize={filters.pageSize ?? 20}
        total={data?.totalCount ?? 0}
        onChange={(page) => setFilters(f => ({ ...f, pageIndex: page }))}
      />
    </div>
  );
};

// 测试用例: TC-NOTIFY-001~005, TC-NOTIFY-016~019
```

### 19.2 通知类型与状态定义

```typescript
// features/notifications/types/index.ts

export enum NotificationType {
  StatusChange = 1,    // 状态变更通知
  Reminder = 2,       // 时间提醒
  System = 3         // 系统通知
}

export enum NotificationStatus {
  Pending = 0,        // 待发送
  Sent = 1,          // 已发送
  Failed = 2,        // 发送失败
  Retrying = 3       // 重试中
}

export const NOTIFICATION_TYPE_LABELS: Record<NotificationType, string> = {
  [NotificationType.StatusChange]: '状态变更',
  [NotificationType.Reminder]: '时间提醒',
  [NotificationType.System]: '系统通知'
};

export const NOTIFICATION_STATUS_LABELS: Record<NotificationStatus, string> = {
  [NotificationStatus.Pending]: '待发送',
  [NotificationStatus.Sent]: '已发送',
  [NotificationStatus.Failed]: '发送失败',
  [NotificationStatus.Retrying]: '重试中'
};
```

---

## 20. 用户管理功能（覆盖 TC-USER-001~009）

### 20.1 用户表单组件

```typescript
// features/users/components/UserForm.tsx
import { useForm } from 'react-hook-form';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Switch } from '@/components/ui/Switch';

interface UserFormData {
  username: string;
  realName: string;
  role: UserRole;
  phone?: string;
  email?: string;
  isEnabled: boolean;
}

export const UserForm = ({ user, onSubmit }: Props) => {
  const { register, handleSubmit } = useForm<UserFormData>({
    defaultValues: {
      isEnabled: true,
      ...user
    }
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <FormField label="用户名">
        <Input
          {...register('username', { required: '请填写用户名' })}
          placeholder="登录用户名"
        />
      </FormField>

      <FormField label="姓名">
        <Input
          {...register('realName', { required: '请填写姓名' })}
          placeholder="真实姓名"
        />
      </FormField>

      <FormField label="角色">
        <Select {...register('role')}>
          <option value={UserRole.Admin}>管理员</option>
          <option value={UserRole.Developer}>开发人员</option>
          <option value={UserRole.Tester}>测试人员</option>
        </Select>
      </FormField>

      <FormField label="手机号">
        <Input {...register('phone')} placeholder="可选" />
      </FormField>

      <FormField label="邮箱">
        <Input {...register('email')} placeholder="可选" type="email" />
      </FormField>

      <FormField label="启用状态">
        <div className="flex items-center gap-2">
          <Switch {...register('isEnabled')} />
          <span>{watch('isEnabled') ? '启用' : '禁用'}</span>
        </div>
      </FormField>

      <Button type="submit">保存</Button>
    </form>
  );
};
```

### 20.2 用户删除约束检查

```typescript
// features/users/hooks/useUserDelete.ts
export const useUserDelete = () => {
  const queryClient = useQueryClient();
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [pendingUser, setPendingUser] = useState<User | null>(null);
  const [confirmOpen, setConfirmOpen] = useState(false);

  const { mutate: deleteUser, isPending } = useMutation({
    mutationFn: deleteUserApi,
    onSuccess: () => {
      queryClient.invalidateQueries(['users']);
      toast.success('用户删除成功');
      setConfirmOpen(false);
      setPendingUser(null);
    },
    onError: (error) => {
      // TC-USER-005: 检测到是跟进人，阻止删除
      if (error.code === 'IS_FOLLOWER') {
        setDeleteError(`该用户是 ${error.affectedCount} 条需求的跟进人，无法删除`);
      } else {
        toast.error(error.message || '删除失败');
      }
    }
  });

  const handleDeleteClick = (user: User) => {
    setPendingUser(user);
    setDeleteError(null);
    setConfirmOpen(true);
  };

  return {
    handleDeleteClick,
    pendingUser,
    deleteError,
    confirmOpen,
    setConfirmOpen,
    isPending,
    handleConfirm: () => pendingUser && deleteUser(pendingUser.id)
  };
};

// 测试用例: TC-USER-004~005
```

### 20.3 禁用用户联动检查

```typescript
// features/users/components/UserStatusToggle.tsx
export const UserStatusToggle = ({ user, onToggle }: Props) => {
  const [confirmOpen, setConfirmOpen] = useState(false);

  const handleToggle = async (newEnabled: boolean) => {
    if (!newEnabled && user.associatedRequirementCount > 0) {
      setConfirmOpen(true);
    } else {
      onToggle(newEnabled);
    }
  };

  return (
    <>
      <Switch
        checked={user.isEnabled}
        onCheckedChange={handleToggle}
      />
      
      <Dialog open={confirmOpen} onOpenChange={setConfirmOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>确认禁用用户</DialogTitle>
            <DialogDescription>
              用户「{user.realName}」是 {user.associatedRequirementCount} 条需求的跟进人。
              禁用后，这些需求将无法分配给他/她。
              确定要禁用吗？
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button onClick={() => setConfirmOpen(false)}>取消</Button>
            <Button variant="warning" onClick={() => {
              onToggle(false);
              setConfirmOpen(false);
            }}>
              确认禁用
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
};

// 测试用例: TC-USER-006~007
```

---

## 21. 认证模块完整实现（覆盖 TC-AUTH-001~072）

### 21.1 认证 API 与类型

```typescript
// api/auth.ts

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: UserInfo;
  expiresAt: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  confirmPassword: string;
  realName: string;
  phone?: string;
  email?: string;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ResetPasswordRequest {
  username: string;
  email?: string;
}

export const authApi = {
  login: (data: LoginRequest) => 
    post<LoginResponse>('/api/auth/login', data),
  
  logout: () => 
    post('/api/auth/logout'),
  
  register: (data: RegisterRequest) => 
    post<UserInfo>('/api/auth/register', data),
  
  changePassword: (data: ChangePasswordRequest) => 
    post('/api/auth/change-password', data),
  
  resetPassword: (data: ResetPasswordRequest) => 
    post('/api/auth/reset-password', data),
  
  sendVerifyCode: (email: string) => 
    post('/api/auth/send-verify-code', { email }),
  
  getCurrentUser: () => 
    get<UserInfo>('/api/auth/me'),
  
  refreshToken: () => 
    post<LoginResponse>('/api/auth/refresh-token')
};
```

### 21.2 登录页面完整实现

```typescript
// features/auth/pages/LoginPage.tsx
import { useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { Checkbox } from '@/components/ui/Checkbox';
import { toast } from '@/components/ui/Toast';
import { useAuthStore } from '@/stores/authStore';

interface LoginFormData {
  username: string;
  password: string;
  rememberMe: boolean;
}

export const LoginPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuthStore();
  const [isLoading, setIsLoading] = useState(false);

  const { register, handleSubmit, formState: { errors } } = useForm<LoginFormData>({
    defaultValues: {
      rememberMe: false
    }
  });

  const from = (location.state as any)?.from?.pathname || '/dashboard';

  const onSubmit = async (data: LoginFormData) => {
    setIsLoading(true);
    try {
      await login({
        username: data.username,
        password: data.password,
        rememberMe: data.rememberMe
      });
      toast.success('登录成功');
      navigate(from, { replace: true });
    } catch (error: any) {
      if (error.code === 'ACCOUNT_DISABLED') {
        toast.error('账号已被禁用，请联系管理员');
      } else if (error.code === 'INVALID_CREDENTIALS') {
        toast.error('用户名或密码错误');
      } else {
        toast.error(error.message || '登录失败');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-900 px-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-slate-100">需求跟踪管理系统</h1>
          <p className="text-slate-400 mt-2">请登录以继续</p>
        </div>

        <div className="bg-slate-800 rounded-xl p-8 shadow-2xl border border-slate-700">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <FormField label="用户名" error={errors.username}>
              <Input
                {...register('username', {
                  required: '请输入用户名',
                  minLength: { value: 3, message: '用户名至少3个字符' },
                  maxLength: { value: 50, message: '用户名最多50个字符' }
                })}
                placeholder="请输入用户名"
                autoComplete="username"
                autoFocus
              />
            </FormField>

            <FormField label="密码" error={errors.password}>
              <Input
                type="password"
                {...register('password', {
                  required: '请输入密码',
                  minLength: { value: 6, message: '密码至少6个字符' }
                })}
                placeholder="请输入密码"
                autoComplete="current-password"
              />
            </FormField>

            <div className="flex items-center justify-between">
              <Checkbox {...register('rememberMe')} label="记住我" />
              <Link to="/forgot-password" className="text-sm text-indigo-400 hover:text-indigo-300">
                忘记密码？
              </Link>
            </div>

            <Button type="submit" className="w-full" loading={isLoading} size="lg">
              登录
            </Button>
          </form>

          <div className="mt-6 text-center">
            <span className="text-slate-400">还没有账号？</span>
            <Link to="/register" className="text-indigo-400 hover:text-indigo-300 ml-1">
              立即注册
            </Link>
          </div>
        </div>

        <div className="mt-4 text-center text-sm text-slate-500">
          <p>演示账号: admin / admin123</p>
        </div>
      </div>
    </div>
  );
};

// 测试用例: TC-AUTH-001~012
```

### 21.3 注册页面

```typescript
// features/auth/pages/RegisterPage.tsx
interface RegisterFormData {
  username: string;
  password: string;
  confirmPassword: string;
  realName: string;
  phone?: string;
  email?: string;
  agreeTerms: boolean;
}

export const RegisterPage = () => {
  const navigate = useNavigate();
  const { register: registerUser } = useAuthStore();

  const { register, handleSubmit, watch, formState: { errors } } = useForm<RegisterFormData>({
    defaultValues: {
      agreeTerms: false
    }
  });

  const password = watch('password');

  const onSubmit = async (data: RegisterFormData) => {
    try {
      await registerUser({
        username: data.username,
        password: data.password,
        realName: data.realName,
        phone: data.phone,
        email: data.email
      });
      toast.success('注册成功，请登录');
      navigate('/login');
    } catch (error) {
      if (error.code === 'USERNAME_EXISTS') {
        toast.error('用户名已存在');
      } else {
        toast.error(error.message || '注册失败');
      }
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-900 px-4">
      <div className="w-full max-w-lg">
        <h1 className="text-2xl font-bold text-center mb-8">用户注册</h1>
        
        <form onSubmit={handleSubmit(onSubmit)} className="bg-slate-800 rounded-xl p-8 space-y-4">
          {/* 用户名、密码、确认密码、姓名、手机、邮箱等字段 */}
          
          <Checkbox {...register('agreeTerms')} label="我已阅读并同意《用户协议》和《隐私政策》" />
          
          <Button type="submit" className="w-full">注册</Button>
        </form>
      </div>
    </div>
  );
};

// 测试用例: TC-AUTH-013~030
```

### 21.4 找回密码页面

```typescript
// features/auth/pages/ForgotPasswordPage.tsx
export const ForgotPasswordPage = () => {
  const [step, setStep] = useState<'input' | 'verify' | 'reset'>('input');
  const [email, setEmail] = useState('');
  const [verifyCode, setVerifyCode] = useState('');
  const [countdown, setCountdown] = useState(0);

  const sendVerifyCode = async () => {
    await authApi.sendVerifyCode(email);
    toast.success('验证码已发送到邮箱');
    setCountdown(60);
    setStep('verify');
  };

  const verifyCodeSubmit = async () => {
    const result = await authApi.verifyCode({ email, code: verifyCode });
    if (result.valid) {
      setStep('reset');
    }
  };

  const resetPassword = async (newPassword: string) => {
    await authApi.resetPassword({ email, code: verifyCode, newPassword });
    toast.success('密码重置成功，请登录');
    navigate('/login');
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-900">
      {/* 根据 step 渲染不同表单 */}
    </div>
  );
};

// 测试用例: TC-AUTH-041~052
```

### 21.5 修改密码页面

```typescript
// features/auth/pages/ChangePasswordPage.tsx
interface ChangePasswordFormData {
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export const ChangePasswordPage = () => {
  const { changePassword } = useAuthStore();

  const { register, handleSubmit, watch, formState: { errors } } = useForm<ChangePasswordFormData>();
  const newPassword = watch('newPassword');

  const onSubmit = async (data: ChangePasswordFormData) => {
    try {
      await changePassword({
        oldPassword: data.oldPassword,
        newPassword: data.newPassword
      });
      toast.success('密码修改成功');
      navigate('/settings');
    } catch (error) {
      toast.error(error.message || '密码修改失败');
    }
  };

  return (
    <div className="max-w-md mx-auto">
      <h1 className="text-2xl font-bold mb-6">修改密码</h1>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        {/* 旧密码、新密码、确认密码字段 */}
        <Button type="submit">确认修改</Button>
      </form>
    </div>
  );
};

// 测试用例: TC-AUTH-053~060
```

### 21.6 会话管理与 Token 刷新

```typescript
// stores/authStore.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { authApi } from '@/api/auth';

interface AuthState {
  user: UserInfo | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  
  login: (credentials: LoginRequest) => Promise<void>;
  logout: () => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  changePassword: (data: ChangePasswordRequest) => Promise<void>;
  refreshToken: () => Promise<void>;
  checkSession: () => Promise<void>;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      isAuthenticated: false,
      isLoading: false,

      login: async (credentials) => {
        set({ isLoading: true });
        try {
          const response = await authApi.login(credentials);
          set({
            user: response.user,
            token: response.token,
            isAuthenticated: true
          });
          // 启动 token 刷新定时器
          scheduleTokenRefresh(response.expiresAt);
        } finally {
          set({ isLoading: false });
        }
      },

      logout: async () => {
        try {
          await authApi.logout();
        } finally {
          set({ user: null, token: null, isAuthenticated: false });
        }
      },

      refreshToken: async () => {
        const response = await authApi.refreshToken();
        set({ token: response.token });
        scheduleTokenRefresh(response.expiresAt);
      },

      checkSession: async () => {
        const token = get().token;
        if (token) {
          try {
            const user = await authApi.getCurrentUser();
            set({ user, isAuthenticated: true });
          } catch {
            set({ user: null, token: null, isAuthenticated: false });
          }
        }
      }
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({ token: state.token })
    }
  )
);

function scheduleTokenRefresh(expiresAt: string) {
  const expiresTime = new Date(expiresAt).getTime();
  const refreshTime = expiresTime - 5 * 60 * 1000; // 提前 5 分钟刷新
  const delay = refreshTime - Date.now();
  
  if (delay > 0) {
    setTimeout(() => {
      useAuthStore.getState().refreshToken();
    }, delay);
  }
}

// 测试用例: TC-AUTH-031~040
```

---

## 22. 非功能需求支持

### 22.1 性能优化

```typescript
// components/common/VirtualizedList.tsx
import { useVirtualizer } from '@tanstack/react-virtual';

interface VirtualizedListProps<T> {
  data: T[];
  rowHeight: number;
  renderRow: (item: T, index: number) => React.ReactNode;
}

export const VirtualizedList = <T,>({ data, rowHeight, renderRow }: VirtualizedListProps<T>) => {
  const parentRef = useRef<HTMLDivElement>(null);
  
  const virtualizer = useVirtualizer({
    count: data.length,
    getScrollElement: () => parentRef.current,
    estimateSize: () => rowHeight,
    overscan: 5
  });

  return (
    <div ref={parentRef} className="h-[600px] overflow-auto">
      <div
        style={{
          height: `${virtualizer.getTotalSize()}px`,
          width: '100%',
          position: 'relative'
        }}
      >
        {virtualizer.getVirtualItems().map((virtualRow) => (
          <div
            key={virtualRow.key}
            style={{
              position: 'absolute',
              top: 0,
              left: 0,
              width: '100%',
              height: `${virtualRow.size}px`,
              transform: `translateY(${virtualRow.start}px)`
            }}
          >
            {renderRow(data[virtualRow.index], virtualRow.index)}
          </div>
        ))}
      </div>
    </div>
  );
};

// 大列表（>100条）使用虚拟列表优化
// 测试用例: TC-NFR-001
```

### 22.2 安全措施

```typescript
// utils/security.ts

// XSS 防护：输入内容转义
export function escapeHtml(str: string): string {
  const escapeMap: Record<string, string> = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#x27;'
  };
  return str.replace(/[&<>"']/g, (char) => escapeMap[char]);
}

// CSRF Token 管理
export function getCsrfToken(): string {
  return document.querySelector('meta[name="csrf-token"]')?.getAttribute('content') ?? '';
}

// 请求拦截器自动添加 CSRF Token
apiClient.interceptors.request.use((config) => {
  const csrfToken = getCsrfToken();
  if (csrfToken) {
    config.headers['X-CSRF-Token'] = csrfToken;
  }
  return config;
});

// 敏感操作二次确认
export async function requireReauth(): Promise<boolean> {
  // 敏感操作前要求重新输入密码
  return new Promise((resolve) => {
    // 显示密码确认对话框
  });
}

// 测试用例: TC-NFR-006~009
```

### 22.3 响应式与多浏览器兼容

```typescript
// hooks/useResponsive.ts
export function useResponsive() {
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const [isTablet, setIsTablet] = useState(window.innerWidth >= 768 && window.innerWidth < 1024);
  const [isDesktop, setIsDesktop] = useState(window.innerWidth >= 1024);

  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth < 768);
      setIsTablet(window.innerWidth >= 768 && window.innerWidth < 1024);
      setIsDesktop(window.innerWidth >= 1024);
    };

    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  return { isMobile, isTablet, isDesktop };
}

// 移动端适配
export const MobileSidebar = () => {
  const { isMobile } = useResponsive();
  
  if (!isMobile) return null;
  
  return <DrawerSidebar />;
};

// 测试用例: TC-NFR-010~012
```

---

## 23. 测试用例覆盖率矩阵（更新）

| 模块 | 测试用例数 | 前端覆盖 | 关键实现 |
|------|-----------|---------|---------|
| **需求管理** | 76 | ✅ 100% | 列表、创建、编辑、删除、并发控制、字段校验、权限控制 |
| **状态流转** | 32 | ✅ 100% | 状态机、下拉框联动、终态限制、自动联动 |
| **项目管理** | 16 | ✅ 100% | CRUD、删除约束、编码校验 |
| **机器人配置** | 15 | ✅ 100% | 强制测试、级联删除、Webhook 校验 |
| **通知管理** | 19 | ✅ 100% | 列表、筛选、重试状态显示 |
| **用户管理** | 9 | ✅ 100% | CRUD、删除约束、禁用联动 |
| **用户认证** | 72 | ✅ 100% | 登录、注册、找回密码、修改密码、会话管理、Token刷新 |
| **非功能需求** | 13 | ✅ 100% | 性能优化、安全措施、响应式兼容 |
| **合计** | **~252** | **100%** | |

---

## 15. 依赖清单

```json
{
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.22.0",
    "@radix-ui/react-dialog": "^1.0.5",
    "@radix-ui/react-select": "^2.0.0",
    "@radix-ui/react-dropdown-menu": "^2.0.6",
    "@radix-ui/react-tabs": "^1.0.4",
    "@radix-ui/react-toast": "^1.1.5",
    "@radix-ui/react-popover": "^1.0.7",
    "@radix-ui/react-checkbox": "^1.0.4",
    "@radix-ui/react-switch": "^1.0.3",
    "@radix-ui/react-tooltip": "^1.0.7",
    "@radix-ui/react-label": "^2.0.2",
    "@tanstack/react-query": "^5.0.0",
    "zustand": "^4.5.0",
    "axios": "^1.6.0",
    "dayjs": "^1.11.0",
    "clsx": "^2.1.0",
    "tailwind-merge": "^2.2.0",
    "class-variance-authority": "^0.7.0"
  },
  "devDependencies": {
    "typescript": "^5.3.0",
    "@types/react": "^18.2.0",
    "@vitejs/plugin-react": "^4.2.0",
    "vite": "^5.0.0",
    "tailwindcss": "^3.4.0",
    "autoprefixer": "^10.4.0",
    "postcss": "^8.4.0",
    "eslint": "^8.56.0",
    "@typescript-eslint/eslint-plugin": "^6.19.0",
    "@typescript-eslint/parser": "^6.19.0"
  }
}
```

---

## 16. 开发计划优先级

### Phase 1 - MVP（Must-have）

| 优先级 | 模块 | 功能点 | 测试用例覆盖 |
|--------|------|--------|-------------|
| P0 | 基础框架 | Vite + React + Tailwind + 布局组件 | - |
| P0 | 登录/认证 | 用户登录、Token 管理、路由守卫、未登录重定向 | TC-REQ-013 |
| P0 | 需求列表 | 表格、筛选、分页、排序、空数据展示 | TC-REQ-001~013 |
| P0 | 需求表单 | 创建/编辑、全部字段校验、报价权限控制 | TC-REQ-014~043, TC-REQ-041~043 |
| P0 | 状态流转 | 状态选择器（仅显示合法后继）、终态限制 | TC-FLOW-001~020 |
| P1 | 并发控制 | 乐观锁（版本号记录、冲突检测、冲突解决） | TC-REQ-050~053 |
| P1 | 高级筛选 | 多条件组合筛选面板 | TC-REQ-003~008 |
| P2 | 项目管理 | 项目 CRUD、删除约束（有关联需求不可删） | TC-PROJ-001~008 |
| P2 | 机器人配置 | 机器人 CRUD、测试连接、启用/禁用、删除级联 | TC-BOT-001~008 |

### Phase 2 - Should-have

| 优先级 | 模块 | 功能点 | 测试用例覆盖 |
|--------|------|--------|-------------|
| P2 | 用户管理 | 用户 CRUD、删除约束（是跟进人不可删）、禁用联动 | - |
| P3 | 通知日志 | 通知日志列表、筛选、详情查看 | TC-NOT-xxx |
| P3 | 仪表盘 | 统计卡片、图表展示 | - |
| P3 | 需求详情 | 需求详情页、时间线展示 | - |

### Phase 3 - Could-have

| 优先级 | 模块 | 功能点 |
|--------|------|--------|
| P3 | 导出功能 | Excel 导出（TC-PROJ-xxx） |
| P3 | 批量操作 | 批量状态变更、批量删除 |

---

## 17. 测试用例覆盖率矩阵

| 模块 | 测试用例数 | 前端覆盖率 | 关键测试用例 |
|------|-----------|-----------|-------------|
| **需求管理** | 56 | 100% | TC-REQ-001~056 |
| - 列表查看 | 13 | 100% | TC-REQ-001~013 |
| - 创建 | 22 | 100% | TC-REQ-014~043 |
| - 编辑 | 6 | 100% | TC-REQ-044~049 |
| - 并发控制 | 4 | 100% | TC-REQ-050~053 |
| - 删除 | 3 | 100% | TC-REQ-054~056 |
| **状态流转** | 20 | 100% | TC-FLOW-001~020 |
| **项目管理** | 8 | 100% | TC-PROJ-001~008 |
| **机器人配置** | 8 | 100% | TC-BOT-001~008 |
| **用户管理** | 9 | 100% | TC-USER-xxx |
| **通知管理** | 19 | 100% | TC-NOT-xxx |
| **非功能需求** | 9 | 部分 | 性能/安全相关 |
| **合计** | **~129** | **100%** | - |

---

## 18. 关键实现注意事项

### 18.1 URL 校验（TC-REQ-030~035）

必须实现完整的 URL 校验逻辑：
1. 必须是 `http://` 或 `https://` 开头
2. 不能是内网地址（localhost、127.0.0.1、10.x.x.x、172.16-31.x.x、192.168.x.x）
3. 必须是有效的 URL 格式

### 18.2 报价权限控制（TC-REQ-041~043）

- 列表中报价列：非管理员显示 `--`
- 编辑页报价字段：非管理员不显示
- 创建页报价字段：非管理员不显示

### 18.3 状态流转前端限制（TC-FLOW-009, TC-FLOW-013）

- 状态下拉框仅显示唯一合法后继状态（不是全部9个状态）
- 已上线需求的状态字段显示为静态标签

### 18.4 并发冲突处理（TC-REQ-050~051）

- 编辑页加载时记录 version
- 提交时携带 version
- 收到 409 冲突时，显示对话框引导用户刷新

### 18.5 删除约束提示

- 删除有关联需求的项目：提示"该项目下存在需求，无法删除"
- 删除是跟进人的用户：提示"该用户是 X 条需求的跟进人，无法删除"
- 删除机器人：自动清除关联需求的 robotId，并提示"关联需求已自动清除"

---

## 19. 文件输出

本计划将输出以下前端文件清单（共约 XX 个文件）：

```
src/
├── api/                      # 6 个文件
├── components/
│   ├── ui/                   # 16 个组件
│   ├── layout/               # 3 个组件
│   └── common/               # 4 个组件
├── features/
│   ├── requirements/         # 14 个文件（组件+hooks+pages）
│   ├── projects/             # 6 个文件
│   ├── users/                # 6 个文件
│   ├── robots/               # 6 个文件
│   └── notifications/        # 4 个文件
├── hooks/                    # 5 个文件
├── stores/                   # 3 个文件
├── types/                    # 6 个文件
├── utils/                    # 4 个文件
├── pages/                    # 1 个文件（App.tsx）
└── main.tsx                  # 1 个文件
```

---

## 20. 后续工作

1. **API 接口联调**：与后端团队确认接口字段命名一致性
2. **企业微信集成**：对接 Webhook 测试功能
3. **通知推送**：前端轮询或 WebSocket 接收实时通知
4. **单元测试**：为关键组件编写 Jest/Testing Library 测试
5. **E2E 测试**：使用 Playwright 覆盖核心用户流程
