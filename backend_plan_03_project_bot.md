# 后端代码生成计划 - Plan 3: 项目与机器人配置模块

## 计划信息
- **计划编号**: Plan 3
- **项目名称**: 需求跟踪管理系统 - 后端
- **目标**: 实现项目管理和企业微信机器人配置功能
- **依赖**: Plan 1, Plan 2

---

## 1. 目标描述

实现项目管理和企业微信机器人配置的CRUD功能：
- 项目列表查询、创建、编辑、删除
- 机器人列表查询、创建、编辑、删除
- 机器人Webhook测试功能

---

## 2. 文件清单

### 2.1 API路由文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `app/api/projects/route.ts` | 创建 | 项目列表查询、创建项目 |
| `app/api/projects/[id]/route.ts` | 创建 | 项目详情、更新、删除 |
| `app/api/webhook-bots/route.ts` | 创建 | 机器人列表查询、创建机器人 |
| `app/api/webhook-bots/[id]/route.ts` | 创建 | 机器人详情、更新、删除 |
| `app/api/webhook-bots/[id]/test/route.ts` | 创建 | 测试机器人Webhook |

### 2.2 服务文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/services/webhook.ts` | 创建 | 企业微信Webhook发送服务 |

### 2.3 验证文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/lib/validation.ts` | 修改 | 添加项目和机器人验证Schema |

---

## 3. 实现细节

### 3.1 src/lib/validation.ts (追加内容)

```typescript
// 项目验证
export const createProjectSchema = z.object({
  name: z.string().min(1, '项目名称不能为空').max(100, '项目名称最多100个字符'),
  code: z.string().max(50, '项目编码最多50个字符').optional().nullable(),
  managerId: z.string().optional().nullable(),
  description: z.string().max(500, '项目描述最多500个字符').optional().nullable(),
});

export type CreateProjectInput = z.infer<typeof createProjectSchema>;

export const updateProjectSchema = createProjectSchema.partial();

export type UpdateProjectInput = z.infer<typeof updateProjectSchema>;

export const projectQuerySchema = z.object({
  page: z.coerce.number().min(1).default(1),
  pageSize: z.coerce.number().min(1).max(100).default(20),
  keyword: z.string().optional(),
});

export type ProjectQueryInput = z.infer<typeof projectQuerySchema>;

// 机器人验证
export const createBotSchema = z.object({
  name: z.string().min(1, '机器人名称不能为空').max(100, '机器人名称最多100个字符'),
  webhookUrl: z.string().url('请输入有效的Webhook地址').max(500, 'Webhook地址最多500个字符'),
  groupName: z.string().max(100, '群组名称最多100个字符').optional().nullable(),
  isActive: z.boolean().optional(),
});

export type CreateBotInput = z.infer<typeof createBotSchema>;

export const updateBotSchema = createBotSchema.partial();

export type UpdateBotInput = z.infer<typeof updateBotSchema>;

export const botQuerySchema = z.object({
  page: z.coerce.number().min(1).default(1),
  pageSize: z.coerce.number().min(1).max(100).default(20),
  keyword: z.string().optional(),
  isActive: z.coerce.boolean().optional(),
});

export type BotQueryInput = z.infer<typeof botQuerySchema>;
```

### 3.2 src/services/webhook.ts

```typescript
import axios from 'axios';

export interface WebhookMessage {
  msgtype: 'text' | 'markdown';
  text?: {
    content: string;
    mentioned_list?: string[];
    mentioned_mobile_list?: string[];
  };
  markdown?: {
    content: string;
  };
}

export async function sendWebhookMessage(
  webhookUrl: string,
  message: WebhookMessage
): Promise<{ success: boolean; error?: string }> {
  try {
    const response = await axios.post(webhookUrl, message, {
      headers: { 'Content-Type': 'application/json' },
      timeout: 10000,
    });

    if (response.data.errcode === 0) {
      return { success: true };
    } else {
      return {
        success: false,
        error: response.data.errmsg || '发送失败',
      };
    }
  } catch (error) {
    if (axios.isAxiosError(error)) {
      return {
        success: false,
        error: error.response?.data?.errmsg || error.message,
      };
    }
    return {
      success: false,
      error: '发送消息时发生错误',
    };
  }
}

export function buildStatusChangeMessage(params: {
  requirementTitle: string;
  requirementNo: string;
  oldStatus: string;
  newStatus: string;
  followerName: string;
  plannedTestDate?: string | null;
  actualTestDate?: string | null;
  remark?: string | null;
}): WebhookMessage {
  const {
    requirementTitle,
    requirementNo,
    oldStatus,
    newStatus,
    followerName,
    plannedTestDate,
    actualTestDate,
    remark,
  } = params;

  let content = `【需求状态变更】\n`;
  content += `需求名称：${requirementTitle}\n`;
  content += `需求号：${requirementNo}\n`;
  content += `状态变更：${oldStatus} → ${newStatus}\n`;
  content += `跟进人：${followerName}\n`;
  
  if (plannedTestDate) {
    content += `计划交测时间：${plannedTestDate}\n`;
  }
  
  if (actualTestDate) {
    content += `实际交测时间：${actualTestDate}\n`;
  }
  
  if (remark) {
    content += `备注：${remark}\n`;
  }

  return {
    msgtype: 'text',
    text: { content },
  };
}

export function buildTimeReminderMessage(params: {
  requirementTitle: string;
  requirementNo: string;
  reminderType: '交测' | '上线';
  plannedDate: string;
  daysRemaining: number;
  followerName: string;
}): WebhookMessage {
  const { requirementTitle, requirementNo, reminderType, plannedDate, daysRemaining, followerName } = params;

  let content = `【需求到期提醒】\n`;
  content += `需求名称：${requirementTitle}\n`;
  content += `需求号：${requirementNo}\n`;
  content += `到期类型：${reminderType}\n`;
  content += `计划时间：${plannedDate}\n`;
  content += `剩余天数：${daysRemaining}天\n`;
  content += `跟进人：${followerName}\n`;

  return {
    msgtype: 'text',
    text: { content },
  };
}
```

### 3.3 app/api/projects/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { requireAdmin } from '@/src/lib/auth-guard';
import { createProjectSchema, projectQuerySchema } from '@/src/lib/validation';

// GET /api/projects - 项目列表
export async function GET(request: NextRequest) {
  try {
    const { searchParams } = new URL(request.url);
    const queryResult = projectQuerySchema.safeParse({
      page: searchParams.get('page') || '1',
      pageSize: searchParams.get('pageSize') || '20',
      keyword: searchParams.get('keyword') || undefined,
    });

    if (!queryResult.success) {
      return NextResponse.json(
        { success: false, error: queryResult.error.errors[0].message },
        { status: 400 }
      );
    }

    const { page, pageSize, keyword } = queryResult.data;
    const skip = (page - 1) * pageSize;

    const where: any = {};
    
    if (keyword) {
      where.OR = [
        { name: { contains: keyword } },
        { code: { contains: keyword } },
      ];
    }

    const [projects, total] = await Promise.all([
      prisma.project.findMany({
        where,
        skip,
        take: pageSize,
        orderBy: { createdAt: 'desc' },
        include: {
          manager: {
            select: {
              id: true,
              name: true,
              username: true,
            },
          },
          _count: {
            select: { requirements: true },
          },
        },
      }),
      prisma.project.count({ where }),
    ]);

    return NextResponse.json({
      success: true,
      data: {
        items: projects,
        total,
        page,
        pageSize,
        totalPages: Math.ceil(total / pageSize),
      },
    });
  } catch (error) {
    console.error('Get projects error:', error);
    return NextResponse.json(
      { success: false, error: '获取项目列表失败' },
      { status: 500 }
    );
  }
}

// POST /api/projects - 创建项目
export async function POST(request: NextRequest) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const body = await request.json();
    const result = createProjectSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const { name, code, managerId, description } = result.data;

    // 检查项目编码是否重复
    if (code) {
      const existingProject = await prisma.project.findUnique({
        where: { code },
      });

      if (existingProject) {
        return NextResponse.json(
          { success: false, error: '项目编码已存在' },
          { status: 409 }
        );
      }
    }

    // 验证负责人是否存在
    if (managerId) {
      const manager = await prisma.user.findUnique({
        where: { id: managerId },
      });

      if (!manager) {
        return NextResponse.json(
          { success: false, error: '指定的项目负责人不存在' },
          { status: 400 }
        );
      }
    }

    const project = await prisma.project.create({
      data: {
        name,
        code,
        managerId,
        description,
      },
      include: {
        manager: {
          select: {
            id: true,
            name: true,
            username: true,
          },
        },
      },
    });

    return NextResponse.json({
      success: true,
      data: project,
      message: '项目创建成功',
    });
  } catch (error) {
    console.error('Create project error:', error);
    return NextResponse.json(
      { success: false, error: '创建项目失败' },
      { status: 500 }
    );
  }
}
```

### 3.4 app/api/projects/[id]/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { requireAdmin } from '@/src/lib/auth-guard';
import { updateProjectSchema } from '@/src/lib/validation';

// GET /api/projects/:id - 项目详情
export async function GET(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const { id } = params;

    const project = await prisma.project.findUnique({
      where: { id },
      include: {
        manager: {
          select: {
            id: true,
            name: true,
            username: true,
          },
        },
        _count: {
          select: { requirements: true },
        },
      },
    });

    if (!project) {
      return NextResponse.json(
        { success: false, error: '项目不存在' },
        { status: 404 }
      );
    }

    return NextResponse.json({
      success: true,
      data: project,
    });
  } catch (error) {
    console.error('Get project error:', error);
    return NextResponse.json(
      { success: false, error: '获取项目详情失败' },
      { status: 500 }
    );
  }
}

// PUT /api/projects/:id - 更新项目
export async function PUT(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { id } = params;
    const body = await request.json();
    const result = updateProjectSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const existingProject = await prisma.project.findUnique({
      where: { id },
    });

    if (!existingProject) {
      return NextResponse.json(
        { success: false, error: '项目不存在' },
        { status: 404 }
      );
    }

    const { code, managerId } = result.data;

    // 检查项目编码是否重复
    if (code && code !== existingProject.code) {
      const duplicateProject = await prisma.project.findUnique({
        where: { code },
      });

      if (duplicateProject) {
        return NextResponse.json(
          { success: false, error: '项目编码已存在' },
          { status: 409 }
        );
      }
    }

    // 验证负责人是否存在
    if (managerId) {
      const manager = await prisma.user.findUnique({
        where: { id: managerId },
      });

      if (!manager) {
        return NextResponse.json(
          { success: false, error: '指定的项目负责人不存在' },
          { status: 400 }
        );
      }
    }

    const project = await prisma.project.update({
      where: { id },
      data: result.data,
      include: {
        manager: {
          select: {
            id: true,
            name: true,
            username: true,
          },
        },
      },
    });

    return NextResponse.json({
      success: true,
      data: project,
      message: '项目更新成功',
    });
  } catch (error) {
    console.error('Update project error:', error);
    return NextResponse.json(
      { success: false, error: '更新项目失败' },
      { status: 500 }
    );
  }
}

// DELETE /api/projects/:id - 删除项目
export async function DELETE(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { id } = params;

    const existingProject = await prisma.project.findUnique({
      where: { id },
      include: {
        _count: {
          select: { requirements: true },
        },
      },
    });

    if (!existingProject) {
      return NextResponse.json(
        { success: false, error: '项目不存在' },
        { status: 404 }
      );
    }

    if (existingProject._count.requirements > 0) {
      return NextResponse.json(
        { success: false, error: '该项目下存在需求，无法删除' },
        { status: 400 }
      );
    }

    await prisma.project.delete({
      where: { id },
    });

    return NextResponse.json({
      success: true,
      message: '项目删除成功',
    });
  } catch (error) {
    console.error('Delete project error:', error);
    return NextResponse.json(
      { success: false, error: '删除项目失败' },
      { status: 500 }
    );
  }
}
```

### 3.5 app/api/webhook-bots/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { requireAdmin } from '@/src/lib/auth-guard';
import { createBotSchema, botQuerySchema } from '@/src/lib/validation';

// GET /api/webhook-bots - 机器人列表
export async function GET(request: NextRequest) {
  try {
    const { searchParams } = new URL(request.url);
    const queryResult = botQuerySchema.safeParse({
      page: searchParams.get('page') || '1',
      pageSize: searchParams.get('pageSize') || '20',
      keyword: searchParams.get('keyword') || undefined,
      isActive: searchParams.get('isActive') || undefined,
    });

    if (!queryResult.success) {
      return NextResponse.json(
        { success: false, error: queryResult.error.errors[0].message },
        { status: 400 }
      );
    }

    const { page, pageSize, keyword, isActive } = queryResult.data;
    const skip = (page - 1) * pageSize;

    const where: any = {};
    
    if (keyword) {
      where.OR = [
        { name: { contains: keyword } },
        { groupName: { contains: keyword } },
      ];
    }
    
    if (isActive !== undefined) where.isActive = isActive;

    const [bots, total] = await Promise.all([
      prisma.webhookBot.findMany({
        where,
        skip,
        take: pageSize,
        orderBy: { createdAt: 'desc' },
        include: {
          _count: {
            select: { requirements: true },
          },
        },
      }),
      prisma.webhookBot.count({ where }),
    ]);

    return NextResponse.json({
      success: true,
      data: {
        items: bots,
        total,
        page,
        pageSize,
        totalPages: Math.ceil(total / pageSize),
      },
    });
  } catch (error) {
    console.error('Get bots error:', error);
    return NextResponse.json(
      { success: false, error: '获取机器人列表失败' },
      { status: 500 }
    );
  }
}

// POST /api/webhook-bots - 创建机器人
export async function POST(request: NextRequest) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const body = await request.json();
    const result = createBotSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const { name, webhookUrl, groupName, isActive } = result.data;

    const bot = await prisma.webhookBot.create({
      data: {
        name,
        webhookUrl,
        groupName,
        isActive: isActive ?? true,
      },
    });

    return NextResponse.json({
      success: true,
      data: bot,
      message: '机器人创建成功',
    });
  } catch (error) {
    console.error('Create bot error:', error);
    return NextResponse.json(
      { success: false, error: '创建机器人失败' },
      { status: 500 }
    );
  }
}
```

### 3.6 app/api/webhook-bots/[id]/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { requireAdmin } from '@/src/lib/auth-guard';
import { updateBotSchema } from '@/src/lib/validation';

// GET /api/webhook-bots/:id - 机器人详情
export async function GET(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const { id } = params;

    const bot = await prisma.webhookBot.findUnique({
      where: { id },
      include: {
        _count: {
          select: { requirements: true },
        },
      },
    });

    if (!bot) {
      return NextResponse.json(
        { success: false, error: '机器人不存在' },
        { status: 404 }
      );
    }

    return NextResponse.json({
      success: true,
      data: bot,
    });
  } catch (error) {
    console.error('Get bot error:', error);
    return NextResponse.json(
      { success: false, error: '获取机器人详情失败' },
      { status: 500 }
    );
  }
}

// PUT /api/webhook-bots/:id - 更新机器人
export async function PUT(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { id } = params;
    const body = await request.json();
    const result = updateBotSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const existingBot = await prisma.webhookBot.findUnique({
      where: { id },
    });

    if (!existingBot) {
      return NextResponse.json(
        { success: false, error: '机器人不存在' },
        { status: 404 }
      );
    }

    const bot = await prisma.webhookBot.update({
      where: { id },
      data: result.data,
    });

    return NextResponse.json({
      success: true,
      data: bot,
      message: '机器人更新成功',
    });
  } catch (error) {
    console.error('Update bot error:', error);
    return NextResponse.json(
      { success: false, error: '更新机器人失败' },
      { status: 500 }
    );
  }
}

// DELETE /api/webhook-bots/:id - 删除机器人
export async function DELETE(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { id } = params;

    const existingBot = await prisma.webhookBot.findUnique({
      where: { id },
      include: {
        _count: {
          select: { requirements: true },
        },
      },
    });

    if (!existingBot) {
      return NextResponse.json(
        { success: false, error: '机器人不存在' },
        { status: 404 }
      );
    }

    await prisma.webhookBot.delete({
      where: { id },
    });

    return NextResponse.json({
      success: true,
      message: '机器人删除成功',
    });
  } catch (error) {
    console.error('Delete bot error:', error);
    return NextResponse.json(
      { success: false, error: '删除机器人失败' },
      { status: 500 }
    );
  }
}
```

### 3.7 app/api/webhook-bots/[id]/test/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { requireAdmin } from '@/src/lib/auth-guard';
import { sendWebhookMessage } from '@/src/services/webhook';

// POST /api/webhook-bots/:id/test - 测试机器人
export async function POST(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { id } = params;

    const bot = await prisma.webhookBot.findUnique({
      where: { id },
    });

    if (!bot) {
      return NextResponse.json(
        { success: false, error: '机器人不存在' },
        { status: 404 }
      );
    }

    if (!bot.isActive) {
      return NextResponse.json(
        { success: false, error: '机器人已禁用' },
        { status: 400 }
      );
    }

    const testMessage = {
      msgtype: 'text' as const,
      text: {
        content: `【测试消息】\n机器人"${bot.name}"配置成功！\n发送时间：${new Date().toLocaleString('zh-CN')}`,
      },
    };

    const result = await sendWebhookMessage(bot.webhookUrl, testMessage);

    if (result.success) {
      return NextResponse.json({
        success: true,
        message: '测试消息发送成功',
      });
    } else {
      return NextResponse.json(
        { success: false, error: result.error || '发送失败' },
        { status: 400 }
      );
    }
  } catch (error) {
    console.error('Test bot error:', error);
    return NextResponse.json(
      { success: false, error: '测试机器人失败' },
      { status: 500 }
    );
  }
}
```

---

## 4. 验证方法

1. 测试项目CRUD接口
2. 测试机器人CRUD接口
3. 测试机器人Webhook测试功能（需要有效的企业微信Webhook地址）

---

## 5. 交付清单

- [ ] 项目CRUD接口完整
- [ ] 机器人CRUD接口完整
- [ ] Webhook测试功能可用
- [ ] 权限控制正常工作
- [ ] 无 TypeScript 编译错误
