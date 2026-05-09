# 后端代码生成计划 - Plan 5: 通知管理与定时任务模块

## 计划信息
- **计划编号**: Plan 5
- **项目名称**: 需求跟踪管理系统 - 后端
- **目标**: 实现通知日志查询和时间提醒定时任务
- **依赖**: Plan 1-4

---

## 1. 目标描述

实现通知管理和时间提醒功能：
- 通知日志列表查询
- 定时任务：每日检查并发送到期提醒
- 通知重发功能

---

## 2. 文件清单

### 2.1 API路由文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `app/api/notifications/route.ts` | 创建 | 通知日志列表查询 |
| `app/api/notifications/[id]/resend/route.ts` | 创建 | 重发通知 |
| `app/api/cron/reminders/route.ts` | 创建 | 定时任务：发送到期提醒 |

### 2.2 服务文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/services/reminder.ts` | 创建 | 提醒检查服务 |

### 2.3 验证文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/lib/validation.ts` | 修改 | 添加通知查询验证Schema |

---

## 3. 实现细节

### 3.1 src/lib/validation.ts (追加内容)

```typescript
// 通知查询参数验证
export const notificationQuerySchema = z.object({
  page: z.coerce.number().min(1).default(1),
  pageSize: z.coerce.number().min(1).max(100).default(20),
  type: z.nativeEnum(NotificationType).optional(),
  status: z.nativeEnum(NotificationStatus).optional(),
  requirementId: z.string().optional(),
  startDate: z.coerce.date().optional(),
  endDate: z.coerce.date().optional(),
});

export type NotificationQueryInput = z.infer<typeof notificationQuerySchema>;
```

### 3.2 src/services/reminder.ts

```typescript
import { prisma } from '@/src/lib/prisma';
import { Priority, RequirementStatus } from '@prisma/client';
import { sendTimeReminderNotification } from './notification';
import dayjs from 'dayjs';

// 提醒规则配置
const REMINDER_RULES: Record<Priority, number[]> = {
  [Priority.HIGH]: [3, 1, 0], // 高优先级：前3天、1天、当天
  [Priority.MEDIUM]: [2, 0],  // 中优先级：前2天、当天
  [Priority.LOW]: [1],        // 低优先级：前1天
};

interface ReminderCheck {
  requirementId: string;
  reminderType: '交测' | '上线';
  plannedDate: Date;
  daysRemaining: number;
}

export async function checkAndSendReminders() {
  const today = dayjs().startOf('day');
  const remindersToSend: ReminderCheck[] = [];

  // 获取所有非已上线的需求
  const requirements = await prisma.requirement.findMany({
    where: {
      status: {
        not: RequirementStatus.RELEASED,
      },
      botId: {
        not: null,
      },
    },
    include: {
      bot: true,
    },
  });

  for (const requirement of requirements) {
    if (!requirement.bot || !requirement.bot.isActive) continue;

    const rules = REMINDER_RULES[requirement.priority];

    // 检查交测时间提醒
    if (requirement.plannedTestDate && requirement.status !== RequirementStatus.TESTING) {
      const testDate = dayjs(requirement.plannedTestDate).startOf('day');
      const daysDiff = testDate.diff(today, 'day');

      if (rules.includes(daysDiff)) {
        // 检查今天是否已经发送过该类型的提醒
        const existingReminder = await prisma.notificationLog.findFirst({
          where: {
            requirementId: requirement.id,
            type: 'TIME_REMINDER',
            createdAt: {
              gte: today.toDate(),
              lt: today.add(1, 'day').toDate(),
            },
          },
        });

        if (!existingReminder) {
          remindersToSend.push({
            requirementId: requirement.id,
            reminderType: '交测',
            plannedDate: requirement.plannedTestDate,
            daysRemaining: daysDiff,
          });
        }
      }
    }

    // 检查上线时间提醒
    if (requirement.plannedReleaseDate && requirement.status !== RequirementStatus.RELEASED) {
      const releaseDate = dayjs(requirement.plannedReleaseDate).startOf('day');
      const daysDiff = releaseDate.diff(today, 'day');

      if (rules.includes(daysDiff)) {
        // 检查今天是否已经发送过该类型的提醒
        const existingReminder = await prisma.notificationLog.findFirst({
          where: {
            requirementId: requirement.id,
            type: 'TIME_REMINDER',
            createdAt: {
              gte: today.toDate(),
              lt: today.add(1, 'day').toDate(),
            },
          },
        });

        if (!existingReminder) {
          remindersToSend.push({
            requirementId: requirement.id,
            reminderType: '上线',
            plannedDate: requirement.plannedReleaseDate,
            daysRemaining: daysDiff,
          });
        }
      }
    }
  }

  // 发送提醒
  const results = await Promise.allSettled(
    remindersToSend.map(reminder =>
      sendTimeReminderNotification({
        requirementId: reminder.requirementId,
        reminderType: reminder.reminderType,
        plannedDate: reminder.plannedDate,
        daysRemaining: reminder.daysRemaining,
      })
    )
  );

  const successCount = results.filter(r => r.status === 'fulfilled').length;
  const failCount = results.filter(r => r.status === 'rejected').length;

  return {
    total: remindersToSend.length,
    success: successCount,
    failed: failCount,
  };
}
```

### 3.3 app/api/notifications/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { notificationQuerySchema } from '@/src/lib/validation';

// GET /api/notifications - 通知日志列表
export async function GET(request: NextRequest) {
  try {
    const { searchParams } = new URL(request.url);
    const queryResult = notificationQuerySchema.safeParse({
      page: searchParams.get('page') || '1',
      pageSize: searchParams.get('pageSize') || '20',
      type: searchParams.get('type') || undefined,
      status: searchParams.get('status') || undefined,
      requirementId: searchParams.get('requirementId') || undefined,
      startDate: searchParams.get('startDate') || undefined,
      endDate: searchParams.get('endDate') || undefined,
    });

    if (!queryResult.success) {
      return NextResponse.json(
        { success: false, error: queryResult.error.errors[0].message },
        { status: 400 }
      );
    }

    const { page, pageSize, type, status, requirementId, startDate, endDate } = queryResult.data;
    const skip = (page - 1) * pageSize;

    const where: any = {};
    
    if (type) where.type = type;
    if (status) where.status = status;
    if (requirementId) where.requirementId = requirementId;
    
    if (startDate || endDate) {
      where.createdAt = {};
      if (startDate) where.createdAt.gte = startDate;
      if (endDate) where.createdAt.lte = endDate;
    }

    const [notifications, total] = await Promise.all([
      prisma.notificationLog.findMany({
        where,
        skip,
        take: pageSize,
        orderBy: { createdAt: 'desc' },
        include: {
          requirement: {
            select: {
              id: true,
              title: true,
              requirementNo: true,
            },
          },
          bot: {
            select: {
              id: true,
              name: true,
            },
          },
        },
      }),
      prisma.notificationLog.count({ where }),
    ]);

    return NextResponse.json({
      success: true,
      data: {
        items: notifications,
        total,
        page,
        pageSize,
        totalPages: Math.ceil(total / pageSize),
      },
    });
  } catch (error) {
    console.error('Get notifications error:', error);
    return NextResponse.json(
      { success: false, error: '获取通知日志失败' },
      { status: 500 }
    );
  }
}
```

### 3.4 app/api/notifications/[id]/resend/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { requireAdmin } from '@/src/lib/auth-guard';
import { sendWebhookMessage } from '@/src/services/webhook';
import { NotificationStatus } from '@prisma/client';

// POST /api/notifications/:id/resend - 重发通知
export async function POST(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { id } = params;

    const notification = await prisma.notificationLog.findUnique({
      where: { id },
      include: {
        bot: true,
      },
    });

    if (!notification) {
      return NextResponse.json(
        { success: false, error: '通知记录不存在' },
        { status: 404 }
      );
    }

    if (!notification.bot || !notification.bot.isActive) {
      return NextResponse.json(
        { success: false, error: '关联的机器人不存在或已禁用' },
        { status: 400 }
      );
    }

    let message;
    try {
      message = JSON.parse(notification.content);
    } catch {
      return NextResponse.json(
        { success: false, error: '通知内容格式错误' },
        { status: 400 }
      );
    }

    const result = await sendWebhookMessage(notification.bot.webhookUrl, message);

    // 更新通知状态
    await prisma.notificationLog.update({
      where: { id },
      data: {
        status: result.success ? NotificationStatus.SUCCESS : NotificationStatus.FAILED,
        errorMsg: result.error || null,
      },
    });

    if (result.success) {
      return NextResponse.json({
        success: true,
        message: '通知重发成功',
      });
    } else {
      return NextResponse.json(
        { success: false, error: result.error || '重发失败' },
        { status: 400 }
      );
    }
  } catch (error) {
    console.error('Resend notification error:', error);
    return NextResponse.json(
      { success: false, error: '重发通知失败' },
      { status: 500 }
    );
  }
}
```

### 3.5 app/api/cron/reminders/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { checkAndSendReminders } from '@/src/services/reminder';

// GET /api/cron/reminders - 定时任务：发送到期提醒
export async function GET(request: NextRequest) {
  try {
    // 验证请求来源（可选：添加密钥验证）
    const authHeader = request.headers.get('authorization');
    const cronSecret = process.env.CRON_SECRET;
    
    if (cronSecret && authHeader !== `Bearer ${cronSecret}`) {
      return NextResponse.json(
        { success: false, error: '未授权' },
        { status: 401 }
      );
    }

    const result = await checkAndSendReminders();

    return NextResponse.json({
      success: true,
      data: result,
      message: `提醒任务执行完成：共${result.total}条，成功${result.success}条，失败${result.failed}条`,
    });
  } catch (error) {
    console.error('Cron reminders error:', error);
    return NextResponse.json(
      { success: false, error: '执行提醒任务失败' },
      { status: 500 }
    );
  }
}
```

---

## 4. 验证方法

1. 测试通知日志查询接口
2. 测试通知重发功能
3. 测试定时任务接口（需要配置CRON_SECRET环境变量）
4. 配置定时任务（如使用Vercel Cron Jobs）

### Vercel Cron Jobs 配置

在 `vercel.json` 中添加：

```json
{
  "crons": [
    {
      "path": "/api/cron/reminders",
      "schedule": "0 9 * * *"
    }
  ]
}
```

---

## 5. 交付清单

- [ ] 通知日志查询接口可用
- [ ] 通知重发功能正常
- [ ] 定时任务接口可正常执行
- [ ] 提醒规则按优先级正确执行
- [ ] 无 TypeScript 编译错误

---

## 6. 后端项目总结

至此，后端项目所有功能模块已完成：

| Plan | 模块 | 功能 |
|------|------|------|
| Plan 1 | 项目脚手架 | 项目初始化、数据库配置、Prisma Schema |
| Plan 2 | 用户认证 | 登录认证、用户管理、权限控制 |
| Plan 3 | 项目与机器人 | 项目管理、企业微信机器人配置 |
| Plan 4 | 需求管理 | 需求CRUD、状态流转、通知发送 |
| Plan 5 | 通知管理 | 通知日志、定时提醒任务 |

### 环境变量配置

```env
# Database
DATABASE_URL="mysql://root:123456@localhost:3306/requirement_tracker"

# JWT
JWT_SECRET="your-secret-key-change-in-production"
JWT_EXPIRES_IN="7d"

# App
NEXT_PUBLIC_APP_URL="http://localhost:3000"
PORT=3001

# Cron (可选)
CRON_SECRET="your-cron-secret"
```

### 部署说明

1. 配置MySQL数据库
2. 设置环境变量
3. 运行 `npx prisma migrate deploy` 部署数据库
4. 运行 `npm run db:seed` 初始化数据
5. 部署到Vercel或其他平台
