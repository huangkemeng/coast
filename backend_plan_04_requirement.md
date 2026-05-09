# 后端代码生成计划 - Plan 4: 需求管理核心模块

## 计划信息
- **计划编号**: Plan 4
- **项目名称**: 需求跟踪管理系统 - 后端
- **目标**: 实现需求CRUD和状态流转功能
- **依赖**: Plan 1, Plan 2, Plan 3

---

## 1. 目标描述

实现需求管理的核心功能：
- 需求列表查询（支持筛选、排序、分页）
- 需求创建、编辑、删除
- 需求状态流转控制
- 状态变更时自动发送通知

---

## 2. 文件清单

### 2.1 API路由文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `app/api/requirements/route.ts` | 创建 | 需求列表查询、创建需求 |
| `app/api/requirements/[id]/route.ts` | 创建 | 需求详情、更新、删除 |
| `app/api/requirements/[id]/status/route.ts` | 创建 | 需求状态变更 |
| `app/api/requirements/stats/route.ts` | 创建 | 需求统计信息 |

### 2.2 服务文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/services/notification.ts` | 创建 | 通知发送服务 |

### 2.3 验证文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/lib/validation.ts` | 修改 | 添加需求验证Schema |
| `src/lib/constants.ts` | 创建 | 状态流转规则常量 |

---

## 3. 实现细节

### 3.1 src/lib/constants.ts

```typescript
import { RequirementStatus } from '@prisma/client';

// 状态流转规则定义
export const STATUS_TRANSITIONS: Record<RequirementStatus, RequirementStatus[]> = {
  [RequirementStatus.PENDING_CONFIRM]: [RequirementStatus.CONFIRMED],
  [RequirementStatus.CONFIRMED]: [RequirementStatus.PENDING_QUOTE],
  [RequirementStatus.PENDING_QUOTE]: [RequirementStatus.QUOTED],
  [RequirementStatus.QUOTED]: [RequirementStatus.PENDING_DEVELOP],
  [RequirementStatus.PENDING_DEVELOP]: [RequirementStatus.DEVELOPING],
  [RequirementStatus.DEVELOPING]: [RequirementStatus.TESTING],
  [RequirementStatus.TESTING]: [RequirementStatus.ACCEPTED_PENDING_RELEASE],
  [RequirementStatus.ACCEPTED_PENDING_RELEASE]: [RequirementStatus.RELEASED],
  [RequirementStatus.RELEASED]: [],
};

// 状态显示名称
export const STATUS_LABELS: Record<RequirementStatus, string> = {
  [RequirementStatus.PENDING_CONFIRM]: '待确认',
  [RequirementStatus.CONFIRMED]: '已确认',
  [RequirementStatus.PENDING_QUOTE]: '待报价',
  [RequirementStatus.QUOTED]: '已报价',
  [RequirementStatus.PENDING_DEVELOP]: '待开发',
  [RequirementStatus.DEVELOPING]: '开发中',
  [RequirementStatus.TESTING]: '测试中',
  [RequirementStatus.ACCEPTED_PENDING_RELEASE]: '已验收待上线',
  [RequirementStatus.RELEASED]: '已上线',
};

// 优先级显示名称
export const PRIORITY_LABELS = {
  HIGH: '高',
  MEDIUM: '中',
  LOW: '低',
};

// 角色显示名称
export const ROLE_LABELS = {
  ADMIN: '管理员',
  DEVELOPER: '开发人员',
  TESTER: '测试人员',
};
```

### 3.2 src/lib/validation.ts (追加内容)

```typescript
// 需求创建验证
export const createRequirementSchema = z.object({
  title: z.string().min(1, '需求名称不能为空').max(100, '需求名称最多100个字符'),
  requirementNo: z.string().min(1, '需求号不能为空').max(50, '需求号最多50个字符'),
  followerId: z.string().min(1, '请选择跟进人'),
  projectId: z.string().min(1, '请选择所属项目'),
  status: z.nativeEnum(RequirementStatus).optional(),
  progress: z.coerce.number().min(0).max(100).default(0),
  plannedStartDate: z.coerce.date().optional().nullable(),
  plannedTestDate: z.coerce.date().optional().nullable(),
  plannedReleaseDate: z.coerce.date().optional().nullable(),
  docUrl: z.string().url('请输入有效的URL').max(500, '链接最多500个字符').optional().nullable(),
  quoteAmount: z.coerce.number().min(0).optional().nullable(),
  botId: z.string().optional().nullable(),
  priority: z.nativeEnum(Priority).default(Priority.MEDIUM),
  remark: z.string().max(500, '备注最多500个字符').optional().nullable(),
});

export type CreateRequirementInput = z.infer<typeof createRequirementSchema>;

// 需求更新验证
export const updateRequirementSchema = createRequirementSchema.partial().extend({
  actualTestDate: z.coerce.date().optional().nullable(),
  actualReleaseDate: z.coerce.date().optional().nullable(),
  isConfirmed: z.boolean().optional(),
});

export type UpdateRequirementInput = z.infer<typeof updateRequirementSchema>;

// 需求查询参数验证
export const requirementQuerySchema = z.object({
  page: z.coerce.number().min(1).default(1),
  pageSize: z.coerce.number().min(1).max(100).default(20),
  keyword: z.string().optional(),
  status: z.nativeEnum(RequirementStatus).optional(),
  followerId: z.string().optional(),
  projectId: z.string().optional(),
  priority: z.nativeEnum(Priority).optional(),
  startDate: z.coerce.date().optional(),
  endDate: z.coerce.date().optional(),
  sortField: z.enum(['createdAt', 'plannedStartDate', 'plannedTestDate', 'plannedReleaseDate', 'progress']).default('createdAt'),
  sortOrder: z.enum(['asc', 'desc']).default('desc'),
});

export type RequirementQueryInput = z.infer<typeof requirementQuerySchema>;

// 状态变更验证
export const changeStatusSchema = z.object({
  status: z.nativeEnum(RequirementStatus),
  remark: z.string().max(500, '备注最多500个字符').optional().nullable(),
});

export type ChangeStatusInput = z.infer<typeof changeStatusSchema>;
```

### 3.3 src/services/notification.ts

```typescript
import { prisma } from '@/src/lib/prisma';
import { sendWebhookMessage, buildStatusChangeMessage, buildTimeReminderMessage } from './webhook';
import { NotificationType, NotificationStatus, RequirementStatus } from '@prisma/client';
import { STATUS_LABELS } from '@/src/lib/constants';

export async function sendStatusChangeNotification(params: {
  requirementId: string;
  oldStatus: RequirementStatus;
  newStatus: RequirementStatus;
  remark?: string | null;
}) {
  const { requirementId, oldStatus, newStatus, remark } = params;

  try {
    const requirement = await prisma.requirement.findUnique({
      where: { id: requirementId },
      include: {
        follower: { select: { name: true } },
        bot: true,
      },
    });

    if (!requirement || !requirement.bot || !requirement.bot.isActive) {
      return;
    }

    const message = buildStatusChangeMessage({
      requirementTitle: requirement.title,
      requirementNo: requirement.requirementNo,
      oldStatus: STATUS_LABELS[oldStatus],
      newStatus: STATUS_LABELS[newStatus],
      followerName: requirement.follower.name,
      plannedTestDate: requirement.plannedTestDate?.toISOString().split('T')[0],
      actualTestDate: requirement.actualTestDate?.toISOString().split('T')[0],
      remark,
    });

    const result = await sendWebhookMessage(requirement.bot.webhookUrl, message);

    // 记录通知日志
    await prisma.notificationLog.create({
      data: {
        type: NotificationType.STATUS_CHANGE,
        requirementId,
        botId: requirement.bot.id,
        content: JSON.stringify(message),
        status: result.success ? NotificationStatus.SUCCESS : NotificationStatus.FAILED,
        errorMsg: result.error,
      },
    });

    return result;
  } catch (error) {
    console.error('Send status change notification error:', error);
  }
}

export async function sendTimeReminderNotification(params: {
  requirementId: string;
  reminderType: '交测' | '上线';
  plannedDate: Date;
  daysRemaining: number;
}) {
  const { requirementId, reminderType, plannedDate, daysRemaining } = params;

  try {
    const requirement = await prisma.requirement.findUnique({
      where: { id: requirementId },
      include: {
        follower: { select: { name: true } },
        bot: true,
      },
    });

    if (!requirement || !requirement.bot || !requirement.bot.isActive) {
      return;
    }

    const message = buildTimeReminderMessage({
      requirementTitle: requirement.title,
      requirementNo: requirement.requirementNo,
      reminderType,
      plannedDate: plannedDate.toISOString().split('T')[0],
      daysRemaining,
      followerName: requirement.follower.name,
    });

    const result = await sendWebhookMessage(requirement.bot.webhookUrl, message);

    // 记录通知日志
    await prisma.notificationLog.create({
      data: {
        type: NotificationType.TIME_REMINDER,
        requirementId,
        botId: requirement.bot.id,
        content: JSON.stringify(message),
        status: result.success ? NotificationStatus.SUCCESS : NotificationStatus.FAILED,
        errorMsg: result.error,
      },
    });

    return result;
  } catch (error) {
    console.error('Send time reminder notification error:', error);
  }
}
```

### 3.4 app/api/requirements/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { requireAdmin, getCurrentUserId, getCurrentUserRole } from '@/src/lib/auth-guard';
import { createRequirementSchema, requirementQuerySchema } from '@/src/lib/validation';
import { UserRole, RequirementStatus } from '@prisma/client';
import { sendStatusChangeNotification } from '@/src/services/notification';

// GET /api/requirements - 需求列表
export async function GET(request: NextRequest) {
  try {
    const { searchParams } = new URL(request.url);
    const queryResult = requirementQuerySchema.safeParse({
      page: searchParams.get('page') || '1',
      pageSize: searchParams.get('pageSize') || '20',
      keyword: searchParams.get('keyword') || undefined,
      status: searchParams.get('status') || undefined,
      followerId: searchParams.get('followerId') || undefined,
      projectId: searchParams.get('projectId') || undefined,
      priority: searchParams.get('priority') || undefined,
      startDate: searchParams.get('startDate') || undefined,
      endDate: searchParams.get('endDate') || undefined,
      sortField: searchParams.get('sortField') || 'createdAt',
      sortOrder: searchParams.get('sortOrder') || 'desc',
    });

    if (!queryResult.success) {
      return NextResponse.json(
        { success: false, error: queryResult.error.errors[0].message },
        { status: 400 }
      );
    }

    const { page, pageSize, keyword, status, followerId, projectId, priority, startDate, endDate, sortField, sortOrder } = queryResult.data;
    const skip = (page - 1) * pageSize;

    const where: any = {};
    
    if (keyword) {
      where.OR = [
        { title: { contains: keyword } },
        { requirementNo: { contains: keyword } },
      ];
    }
    
    if (status) where.status = status;
    if (followerId) where.followerId = followerId;
    if (projectId) where.projectId = projectId;
    if (priority) where.priority = priority;
    
    if (startDate || endDate) {
      where.plannedStartDate = {};
      if (startDate) where.plannedStartDate.gte = startDate;
      if (endDate) where.plannedStartDate.lte = endDate;
    }

    // 非管理员只能看到自己跟进的需求
    const userRole = getCurrentUserRole(request);
    const userId = getCurrentUserId(request);
    if (userRole !== UserRole.ADMIN) {
      where.followerId = userId;
    }

    const [requirements, total] = await Promise.all([
      prisma.requirement.findMany({
        where,
        skip,
        take: pageSize,
        orderBy: { [sortField]: sortOrder },
        include: {
          follower: {
            select: { id: true, name: true, username: true },
          },
          project: {
            select: { id: true, name: true, code: true },
          },
          bot: {
            select: { id: true, name: true },
          },
        },
      }),
      prisma.requirement.count({ where }),
    ]);

    return NextResponse.json({
      success: true,
      data: {
        items: requirements,
        total,
        page,
        pageSize,
        totalPages: Math.ceil(total / pageSize),
      },
    });
  } catch (error) {
    console.error('Get requirements error:', error);
    return NextResponse.json(
      { success: false, error: '获取需求列表失败' },
      { status: 500 }
    );
  }
}

// POST /api/requirements - 创建需求
export async function POST(request: NextRequest) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const body = await request.json();
    const result = createRequirementSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const data = result.data;

    // 检查需求号是否重复
    const existingRequirement = await prisma.requirement.findUnique({
      where: { requirementNo: data.requirementNo },
    });

    if (existingRequirement) {
      return NextResponse.json(
        { success: false, error: '需求号已存在' },
        { status: 409 }
      );
    }

    // 验证跟进人是否存在
    const follower = await prisma.user.findUnique({
      where: { id: data.followerId },
    });

    if (!follower) {
      return NextResponse.json(
        { success: false, error: '指定的跟进人不存在' },
        { status: 400 }
      );
    }

    // 验证项目是否存在
    const project = await prisma.project.findUnique({
      where: { id: data.projectId },
    });

    if (!project) {
      return NextResponse.json(
        { success: false, error: '指定的项目不存在' },
        { status: 400 }
      );
    }

    // 验证机器人是否存在
    if (data.botId) {
      const bot = await prisma.webhookBot.findUnique({
        where: { id: data.botId },
      });

      if (!bot) {
        return NextResponse.json(
          { success: false, error: '指定的机器人不存在' },
          { status: 400 }
        );
      }
    }

    // 验证时间逻辑
    if (data.plannedStartDate && data.plannedTestDate && data.plannedStartDate > data.plannedTestDate) {
      return NextResponse.json(
        { success: false, error: '计划交测时间不能早于计划开始时间' },
        { status: 400 }
      );
    }

    if (data.plannedTestDate && data.plannedReleaseDate && data.plannedTestDate > data.plannedReleaseDate) {
      return NextResponse.json(
        { success: false, error: '计划上线时间不能早于计划交测时间' },
        { status: 400 }
      );
    }

    const requirement = await prisma.requirement.create({
      data: {
        ...data,
        status: data.status || RequirementStatus.PENDING_CONFIRM,
      },
      include: {
        follower: {
          select: { id: true, name: true, username: true },
        },
        project: {
          select: { id: true, name: true, code: true },
        },
        bot: {
          select: { id: true, name: true },
        },
      },
    });

    return NextResponse.json({
      success: true,
      data: requirement,
      message: '需求创建成功',
    });
  } catch (error) {
    console.error('Create requirement error:', error);
    return NextResponse.json(
      { success: false, error: '创建需求失败' },
      { status: 500 }
    );
  }
}
```

### 3.5 app/api/requirements/[id]/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { requireAdmin, getCurrentUserId, getCurrentUserRole } from '@/src/lib/auth-guard';
import { updateRequirementSchema } from '@/src/lib/validation';
import { UserRole, RequirementStatus } from '@prisma/client';
import { sendStatusChangeNotification } from '@/src/services/notification';
import { STATUS_TRANSITIONS } from '@/src/lib/constants';

// GET /api/requirements/:id - 需求详情
export async function GET(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const { id } = params;
    const userRole = getCurrentUserRole(request);
    const userId = getCurrentUserId(request);

    const requirement = await prisma.requirement.findUnique({
      where: { id },
      include: {
        follower: {
          select: { id: true, name: true, username: true },
        },
        project: {
          select: { id: true, name: true, code: true },
        },
        bot: {
          select: { id: true, name: true },
        },
      },
    });

    if (!requirement) {
      return NextResponse.json(
        { success: false, error: '需求不存在' },
        { status: 404 }
      );
    }

    // 非管理员只能查看自己跟进的需求
    if (userRole !== UserRole.ADMIN && requirement.followerId !== userId) {
      return NextResponse.json(
        { success: false, error: '无权查看此需求' },
        { status: 403 }
      );
    }

    return NextResponse.json({
      success: true,
      data: requirement,
    });
  } catch (error) {
    console.error('Get requirement error:', error);
    return NextResponse.json(
      { success: false, error: '获取需求详情失败' },
      { status: 500 }
    );
  }
}

// PUT /api/requirements/:id - 更新需求
export async function PUT(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const { id } = params;
    const userRole = getCurrentUserRole(request);
    const userId = getCurrentUserId(request);

    const existingRequirement = await prisma.requirement.findUnique({
      where: { id },
    });

    if (!existingRequirement) {
      return NextResponse.json(
        { success: false, error: '需求不存在' },
        { status: 404 }
      );
    }

    // 检查权限：管理员或跟进人可以编辑
    if (userRole !== UserRole.ADMIN && existingRequirement.followerId !== userId) {
      return NextResponse.json(
        { success: false, error: '无权编辑此需求' },
        { status: 403 }
      );
    }

    const body = await request.json();
    const result = updateRequirementSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const data = result.data;
    const oldStatus = existingRequirement.status;
    let newStatus = data.status || oldStatus;

    // 验证状态流转是否合法
    if (data.status && data.status !== oldStatus) {
      const allowedTransitions = STATUS_TRANSITIONS[oldStatus];
      if (!allowedTransitions.includes(data.status)) {
        return NextResponse.json(
          { success: false, error: '非法的状态流转' },
          { status: 400 }
        );
      }
    }

    // 检查需求号是否重复
    if (data.requirementNo && data.requirementNo !== existingRequirement.requirementNo) {
      const duplicateRequirement = await prisma.requirement.findUnique({
        where: { requirementNo: data.requirementNo },
      });

      if (duplicateRequirement) {
        return NextResponse.json(
          { success: false, error: '需求号已存在' },
          { status: 409 }
        );
      }
    }

    // 验证跟进人是否存在
    if (data.followerId) {
      const follower = await prisma.user.findUnique({
        where: { id: data.followerId },
      });

      if (!follower) {
        return NextResponse.json(
          { success: false, error: '指定的跟进人不存在' },
          { status: 400 }
        );
      }
    }

    // 验证项目是否存在
    if (data.projectId) {
      const project = await prisma.project.findUnique({
        where: { id: data.projectId },
      });

      if (!project) {
        return NextResponse.json(
          { success: false, error: '指定的项目不存在' },
          { status: 400 }
        );
      }
    }

    // 验证机器人是否存在
    if (data.botId) {
      const bot = await prisma.webhookBot.findUnique({
        where: { id: data.botId },
      });

      if (!bot) {
        return NextResponse.json(
          { success: false, error: '指定的机器人不存在' },
          { status: 400 }
        );
      }
    }

    // 验证时间逻辑
    const plannedStartDate = data.plannedStartDate || existingRequirement.plannedStartDate;
    const plannedTestDate = data.plannedTestDate || existingRequirement.plannedTestDate;
    const plannedReleaseDate = data.plannedReleaseDate || existingRequirement.plannedReleaseDate;

    if (plannedStartDate && plannedTestDate && plannedStartDate > plannedTestDate) {
      return NextResponse.json(
        { success: false, error: '计划交测时间不能早于计划开始时间' },
        { status: 400 }
      );
    }

    if (plannedTestDate && plannedReleaseDate && plannedTestDate > plannedReleaseDate) {
      return NextResponse.json(
        { success: false, error: '计划上线时间不能早于计划交测时间' },
        { status: 400 }
      );
    }

    // 自动设置实际时间
    let actualTestDate = data.actualTestDate;
    let actualReleaseDate = data.actualReleaseDate;
    let isConfirmed = data.isConfirmed;

    if (newStatus === RequirementStatus.TESTING && !existingRequirement.actualTestDate && !actualTestDate) {
      actualTestDate = new Date();
    }

    if (newStatus === RequirementStatus.RELEASED && !existingRequirement.actualReleaseDate && !actualReleaseDate) {
      actualReleaseDate = new Date();
    }

    if (newStatus !== RequirementStatus.PENDING_CONFIRM && !isConfirmed) {
      isConfirmed = true;
    }

    const requirement = await prisma.requirement.update({
      where: { id },
      data: {
        ...data,
        actualTestDate,
        actualReleaseDate,
        isConfirmed,
      },
      include: {
        follower: {
          select: { id: true, name: true, username: true },
        },
        project: {
          select: { id: true, name: true, code: true },
        },
        bot: {
          select: { id: true, name: true },
        },
      },
    });

    // 发送状态变更通知
    if (newStatus !== oldStatus) {
      await sendStatusChangeNotification({
        requirementId: id,
        oldStatus,
        newStatus,
        remark: data.remark,
      });
    }

    return NextResponse.json({
      success: true,
      data: requirement,
      message: '需求更新成功',
    });
  } catch (error) {
    console.error('Update requirement error:', error);
    return NextResponse.json(
      { success: false, error: '更新需求失败' },
      { status: 500 }
    );
  }
}

// DELETE /api/requirements/:id - 删除需求
export async function DELETE(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { id } = params;

    const existingRequirement = await prisma.requirement.findUnique({
      where: { id },
    });

    if (!existingRequirement) {
      return NextResponse.json(
        { success: false, error: '需求不存在' },
        { status: 404 }
      );
    }

    await prisma.requirement.delete({
      where: { id },
    });

    return NextResponse.json({
      success: true,
      message: '需求删除成功',
    });
  } catch (error) {
    console.error('Delete requirement error:', error);
    return NextResponse.json(
      { success: false, error: '删除需求失败' },
      { status: 500 }
    );
  }
}
```

### 3.6 app/api/requirements/[id]/status/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { getCurrentUserId, getCurrentUserRole } from '@/src/lib/auth-guard';
import { changeStatusSchema } from '@/src/lib/validation';
import { UserRole, RequirementStatus } from '@prisma/client';
import { sendStatusChangeNotification } from '@/src/services/notification';
import { STATUS_TRANSITIONS } from '@/src/lib/constants';

// PUT /api/requirements/:id/status - 变更需求状态
export async function PUT(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const { id } = params;
    const userRole = getCurrentUserRole(request);
    const userId = getCurrentUserId(request);

    const existingRequirement = await prisma.requirement.findUnique({
      where: { id },
    });

    if (!existingRequirement) {
      return NextResponse.json(
        { success: false, error: '需求不存在' },
        { status: 404 }
      );
    }

    // 检查权限
    if (userRole !== UserRole.ADMIN && existingRequirement.followerId !== userId) {
      return NextResponse.json(
        { success: false, error: '无权修改此需求状态' },
        { status: 403 }
      );
    }

    const body = await request.json();
    const result = changeStatusSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const { status: newStatus, remark } = result.data;
    const oldStatus = existingRequirement.status;

    if (newStatus === oldStatus) {
      return NextResponse.json(
        { success: false, error: '新状态不能与当前状态相同' },
        { status: 400 }
      );
    }

    // 验证状态流转是否合法
    const allowedTransitions = STATUS_TRANSITIONS[oldStatus];
    if (!allowedTransitions.includes(newStatus)) {
      return NextResponse.json(
        { success: false, error: `不能从"${oldStatus}"流转到"${newStatus}"` },
        { status: 400 }
      );
    }

    // 自动设置实际时间
    let actualTestDate = existingRequirement.actualTestDate;
    let actualReleaseDate = existingRequirement.actualReleaseDate;
    let isConfirmed = existingRequirement.isConfirmed;

    if (newStatus === RequirementStatus.TESTING && !actualTestDate) {
      actualTestDate = new Date();
    }

    if (newStatus === RequirementStatus.RELEASED && !actualReleaseDate) {
      actualReleaseDate = new Date();
    }

    if (newStatus !== RequirementStatus.PENDING_CONFIRM && !isConfirmed) {
      isConfirmed = true;
    }

    const requirement = await prisma.requirement.update({
      where: { id },
      data: {
        status: newStatus,
        actualTestDate,
        actualReleaseDate,
        isConfirmed,
      },
      include: {
        follower: {
          select: { id: true, name: true, username: true },
        },
        project: {
          select: { id: true, name: true, code: true },
        },
        bot: {
          select: { id: true, name: true },
        },
      },
    });

    // 发送状态变更通知
    await sendStatusChangeNotification({
      requirementId: id,
      oldStatus,
      newStatus,
      remark,
    });

    return NextResponse.json({
      success: true,
      data: requirement,
      message: '状态变更成功',
    });
  } catch (error) {
    console.error('Change status error:', error);
    return NextResponse.json(
      { success: false, error: '变更状态失败' },
      { status: 500 }
    );
  }
}
```

### 3.7 app/api/requirements/stats/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { RequirementStatus } from '@prisma/client';

// GET /api/requirements/stats - 需求统计
export async function GET(request: NextRequest) {
  try {
    const stats = await Promise.all([
      // 各状态数量统计
      prisma.requirement.groupBy({
        by: ['status'],
        _count: { status: true },
      }),
      // 总数
      prisma.requirement.count(),
      // 本周新增
      prisma.requirement.count({
        where: {
          createdAt: {
            gte: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
          },
        },
      }),
      // 已上线数量
      prisma.requirement.count({
        where: { status: RequirementStatus.RELEASED },
      }),
    ]);

    const [statusStats, total, thisWeekCount, releasedCount] = stats;

    const statusCounts = Object.values(RequirementStatus).reduce((acc, status) => {
      const stat = statusStats.find(s => s.status === status);
      acc[status] = stat?._count.status || 0;
      return acc;
    }, {} as Record<RequirementStatus, number>);

    return NextResponse.json({
      success: true,
      data: {
        total,
        thisWeekCount,
        releasedCount,
        statusCounts,
      },
    });
  } catch (error) {
    console.error('Get stats error:', error);
    return NextResponse.json(
      { success: false, error: '获取统计数据失败' },
      { status: 500 }
    );
  }
}
```

---

## 4. 验证方法

1. 测试需求CRUD接口
2. 测试状态流转规则
3. 测试状态变更通知功能（需要配置有效的Webhook）
4. 测试筛选、排序、分页功能

---

## 5. 交付清单

- [ ] 需求CRUD接口完整
- [ ] 状态流转规则正常工作
- [ ] 状态变更通知可正常发送
- [ ] 筛选、排序、分页功能正常
- [ ] 统计接口可用
- [ ] 无 TypeScript 编译错误
