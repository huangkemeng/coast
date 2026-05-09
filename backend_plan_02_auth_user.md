# 后端代码生成计划 - Plan 2: 用户认证与用户管理模块

## 计划信息
- **计划编号**: Plan 2
- **项目名称**: 需求跟踪管理系统 - 后端
- **目标**: 实现用户认证和用户管理功能
- **依赖**: Plan 1（项目脚手架）

---

## 1. 目标描述

实现用户登录认证（JWT）、用户CRUD接口，包括：
- 用户登录/登出
- 获取当前用户信息
- 用户列表查询
- 用户创建、编辑、删除
- 密码修改

---

## 2. 文件清单

### 2.1 API路由文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `app/api/auth/login/route.ts` | 创建 | 用户登录接口 |
| `app/api/auth/me/route.ts` | 创建 | 获取当前用户信息 |
| `app/api/users/route.ts` | 创建 | 用户列表查询、创建用户 |
| `app/api/users/[id]/route.ts` | 创建 | 用户详情、更新、删除 |
| `app/api/users/[id]/password/route.ts` | 创建 | 修改密码 |

### 2.2 工具/验证文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/lib/validation.ts` | 创建 | Zod验证Schema |
| `src/lib/auth-guard.ts` | 创建 | 权限验证工具 |

### 2.3 种子数据

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `prisma/seed.ts` | 创建 | 初始化管理员用户数据 |

---

## 3. 实现细节

### 3.1 src/lib/validation.ts

```typescript
import { z } from 'zod';
import { UserRole, RequirementStatus, Priority } from '@prisma/client';

// 登录验证
export const loginSchema = z.object({
  username: z.string().min(1, '用户名不能为空'),
  password: z.string().min(1, '密码不能为空'),
});

export type LoginInput = z.infer<typeof loginSchema>;

// 用户创建验证
export const createUserSchema = z.object({
  username: z.string().min(3, '用户名至少3个字符').max(50, '用户名最多50个字符'),
  password: z.string().min(6, '密码至少6个字符').max(100, '密码最多100个字符'),
  name: z.string().min(1, '姓名不能为空').max(50, '姓名最多50个字符'),
  role: z.nativeEnum(UserRole),
  phone: z.string().max(20, '手机号最多20个字符').optional().nullable(),
  email: z.string().email('邮箱格式不正确').max(100, '邮箱最多100个字符').optional().nullable(),
  isActive: z.boolean().optional(),
});

export type CreateUserInput = z.infer<typeof createUserSchema>;

// 用户更新验证
export const updateUserSchema = z.object({
  name: z.string().min(1, '姓名不能为空').max(50, '姓名最多50个字符').optional(),
  role: z.nativeEnum(UserRole).optional(),
  phone: z.string().max(20, '手机号最多20个字符').optional().nullable(),
  email: z.string().email('邮箱格式不正确').max(100, '邮箱最多100个字符').optional().nullable(),
  isActive: z.boolean().optional(),
});

export type UpdateUserInput = z.infer<typeof updateUserSchema>;

// 修改密码验证
export const changePasswordSchema = z.object({
  oldPassword: z.string().min(1, '原密码不能为空'),
  newPassword: z.string().min(6, '新密码至少6个字符').max(100, '新密码最多100个字符'),
});

export type ChangePasswordInput = z.infer<typeof changePasswordSchema>;

// 用户查询参数验证
export const userQuerySchema = z.object({
  page: z.coerce.number().min(1).default(1),
  pageSize: z.coerce.number().min(1).max(100).default(20),
  keyword: z.string().optional(),
  role: z.nativeEnum(UserRole).optional(),
  isActive: z.coerce.boolean().optional(),
});

export type UserQueryInput = z.infer<typeof userQuerySchema>;
```

### 3.2 src/lib/auth-guard.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { UserRole } from '@prisma/client';

export function requireAdmin(request: NextRequest): NextResponse | null {
  const userRole = request.headers.get('x-user-role');
  
  if (userRole !== UserRole.ADMIN) {
    return NextResponse.json(
      { success: false, error: '需要管理员权限' },
      { status: 403 }
    );
  }
  
  return null;
}

export function getCurrentUserId(request: NextRequest): string {
  return request.headers.get('x-user-id') || '';
}

export function getCurrentUserRole(request: NextRequest): UserRole | null {
  const role = request.headers.get('x-user-role');
  return role as UserRole | null;
}
```

### 3.3 app/api/auth/login/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { verifyPassword, generateToken } from '@/src/lib/auth';
import { loginSchema } from '@/src/lib/validation';

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const result = loginSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const { username, password } = result.data;

    const user = await prisma.user.findUnique({
      where: { username },
    });

    if (!user || !user.isActive) {
      return NextResponse.json(
        { success: false, error: '用户名或密码错误' },
        { status: 401 }
      );
    }

    const isValidPassword = await verifyPassword(password, user.password);

    if (!isValidPassword) {
      return NextResponse.json(
        { success: false, error: '用户名或密码错误' },
        { status: 401 }
      );
    }

    const token = generateToken({
      userId: user.id,
      username: user.username,
      role: user.role,
    });

    return NextResponse.json({
      success: true,
      data: {
        token,
        user: {
          id: user.id,
          username: user.username,
          name: user.name,
          role: user.role,
          phone: user.phone,
          email: user.email,
        },
      },
    });
  } catch (error) {
    console.error('Login error:', error);
    return NextResponse.json(
      { success: false, error: '登录失败' },
      { status: 500 }
    );
  }
}
```

### 3.4 app/api/auth/me/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { getCurrentUserId } from '@/src/lib/auth-guard';

export async function GET(request: NextRequest) {
  try {
    const userId = getCurrentUserId(request);

    if (!userId) {
      return NextResponse.json(
        { success: false, error: '未登录' },
        { status: 401 }
      );
    }

    const user = await prisma.user.findUnique({
      where: { id: userId },
      select: {
        id: true,
        username: true,
        name: true,
        role: true,
        phone: true,
        email: true,
        isActive: true,
        createdAt: true,
      },
    });

    if (!user) {
      return NextResponse.json(
        { success: false, error: '用户不存在' },
        { status: 404 }
      );
    }

    return NextResponse.json({
      success: true,
      data: user,
    });
  } catch (error) {
    console.error('Get current user error:', error);
    return NextResponse.json(
      { success: false, error: '获取用户信息失败' },
      { status: 500 }
    );
  }
}
```

### 3.5 app/api/users/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { hashPassword } from '@/src/lib/auth';
import { requireAdmin, getCurrentUserId } from '@/src/lib/auth-guard';
import { createUserSchema, userQuerySchema } from '@/src/lib/validation';

// GET /api/users - 用户列表
export async function GET(request: NextRequest) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { searchParams } = new URL(request.url);
    const queryResult = userQuerySchema.safeParse({
      page: searchParams.get('page') || '1',
      pageSize: searchParams.get('pageSize') || '20',
      keyword: searchParams.get('keyword') || undefined,
      role: searchParams.get('role') || undefined,
      isActive: searchParams.get('isActive') || undefined,
    });

    if (!queryResult.success) {
      return NextResponse.json(
        { success: false, error: queryResult.error.errors[0].message },
        { status: 400 }
      );
    }

    const { page, pageSize, keyword, role, isActive } = queryResult.data;
    const skip = (page - 1) * pageSize;

    const where: any = {};
    
    if (keyword) {
      where.OR = [
        { username: { contains: keyword } },
        { name: { contains: keyword } },
        { email: { contains: keyword } },
      ];
    }
    
    if (role) where.role = role;
    if (isActive !== undefined) where.isActive = isActive;

    const [users, total] = await Promise.all([
      prisma.user.findMany({
        where,
        skip,
        take: pageSize,
        orderBy: { createdAt: 'desc' },
        select: {
          id: true,
          username: true,
          name: true,
          role: true,
          phone: true,
          email: true,
          isActive: true,
          createdAt: true,
        },
      }),
      prisma.user.count({ where }),
    ]);

    return NextResponse.json({
      success: true,
      data: {
        items: users,
        total,
        page,
        pageSize,
        totalPages: Math.ceil(total / pageSize),
      },
    });
  } catch (error) {
    console.error('Get users error:', error);
    return NextResponse.json(
      { success: false, error: '获取用户列表失败' },
      { status: 500 }
    );
  }
}

// POST /api/users - 创建用户
export async function POST(request: NextRequest) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const body = await request.json();
    const result = createUserSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const { username, password, name, role, phone, email, isActive } = result.data;

    const existingUser = await prisma.user.findUnique({
      where: { username },
    });

    if (existingUser) {
      return NextResponse.json(
        { success: false, error: '用户名已存在' },
        { status: 409 }
      );
    }

    const hashedPassword = await hashPassword(password);

    const user = await prisma.user.create({
      data: {
        username,
        password: hashedPassword,
        name,
        role,
        phone,
        email,
        isActive: isActive ?? true,
      },
      select: {
        id: true,
        username: true,
        name: true,
        role: true,
        phone: true,
        email: true,
        isActive: true,
        createdAt: true,
      },
    });

    return NextResponse.json({
      success: true,
      data: user,
      message: '用户创建成功',
    });
  } catch (error) {
    console.error('Create user error:', error);
    return NextResponse.json(
      { success: false, error: '创建用户失败' },
      { status: 500 }
    );
  }
}
```

### 3.6 app/api/users/[id]/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { requireAdmin, getCurrentUserId } from '@/src/lib/auth-guard';
import { updateUserSchema } from '@/src/lib/validation';

// GET /api/users/:id - 用户详情
export async function GET(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { id } = params;

    const user = await prisma.user.findUnique({
      where: { id },
      select: {
        id: true,
        username: true,
        name: true,
        role: true,
        phone: true,
        email: true,
        isActive: true,
        createdAt: true,
        updatedAt: true,
      },
    });

    if (!user) {
      return NextResponse.json(
        { success: false, error: '用户不存在' },
        { status: 404 }
      );
    }

    return NextResponse.json({
      success: true,
      data: user,
    });
  } catch (error) {
    console.error('Get user error:', error);
    return NextResponse.json(
      { success: false, error: '获取用户详情失败' },
      { status: 500 }
    );
  }
}

// PUT /api/users/:id - 更新用户
export async function PUT(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { id } = params;
    const body = await request.json();
    const result = updateUserSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const existingUser = await prisma.user.findUnique({
      where: { id },
    });

    if (!existingUser) {
      return NextResponse.json(
        { success: false, error: '用户不存在' },
        { status: 404 }
      );
    }

    const user = await prisma.user.update({
      where: { id },
      data: result.data,
      select: {
        id: true,
        username: true,
        name: true,
        role: true,
        phone: true,
        email: true,
        isActive: true,
        createdAt: true,
        updatedAt: true,
      },
    });

    return NextResponse.json({
      success: true,
      data: user,
      message: '用户更新成功',
    });
  } catch (error) {
    console.error('Update user error:', error);
    return NextResponse.json(
      { success: false, error: '更新用户失败' },
      { status: 500 }
    );
  }
}

// DELETE /api/users/:id - 删除用户
export async function DELETE(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const adminCheck = requireAdmin(request);
    if (adminCheck) return adminCheck;

    const { id } = params;
    const currentUserId = getCurrentUserId(request);

    if (id === currentUserId) {
      return NextResponse.json(
        { success: false, error: '不能删除当前登录用户' },
        { status: 400 }
      );
    }

    const existingUser = await prisma.user.findUnique({
      where: { id },
    });

    if (!existingUser) {
      return NextResponse.json(
        { success: false, error: '用户不存在' },
        { status: 404 }
      );
    }

    // 检查用户是否有关联的需求
    const hasRequirements = await prisma.requirement.count({
      where: { followerId: id },
    });

    if (hasRequirements > 0) {
      return NextResponse.json(
        { success: false, error: '该用户有关联的需求，无法删除' },
        { status: 400 }
      );
    }

    await prisma.user.delete({
      where: { id },
    });

    return NextResponse.json({
      success: true,
      message: '用户删除成功',
    });
  } catch (error) {
    console.error('Delete user error:', error);
    return NextResponse.json(
      { success: false, error: '删除用户失败' },
      { status: 500 }
    );
  }
}
```

### 3.7 app/api/users/[id]/password/route.ts

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';
import { hashPassword, verifyPassword } from '@/src/lib/auth';
import { requireAdmin, getCurrentUserId } from '@/src/lib/auth-guard';
import { changePasswordSchema } from '@/src/lib/validation';

// PUT /api/users/:id/password - 修改密码
export async function PUT(
  request: NextRequest,
  { params }: { params: { id: string } }
) {
  try {
    const { id } = params;
    const currentUserId = getCurrentUserId(request);
    const isAdmin = !requireAdmin(request);

    // 只有管理员或本人可以修改密码
    if (id !== currentUserId && !isAdmin) {
      return NextResponse.json(
        { success: false, error: '无权修改他人密码' },
        { status: 403 }
      );
    }

    const body = await request.json();
    const result = changePasswordSchema.safeParse(body);

    if (!result.success) {
      return NextResponse.json(
        { success: false, error: result.error.errors[0].message },
        { status: 400 }
      );
    }

    const { oldPassword, newPassword } = result.data;

    const user = await prisma.user.findUnique({
      where: { id },
    });

    if (!user) {
      return NextResponse.json(
        { success: false, error: '用户不存在' },
        { status: 404 }
      );
    }

    // 非管理员修改自己密码需要验证原密码
    if (id === currentUserId && !isAdmin) {
      const isValidPassword = await verifyPassword(oldPassword, user.password);
      if (!isValidPassword) {
        return NextResponse.json(
          { success: false, error: '原密码错误' },
          { status: 400 }
        );
      }
    }

    const hashedPassword = await hashPassword(newPassword);

    await prisma.user.update({
      where: { id },
      data: { password: hashedPassword },
    });

    return NextResponse.json({
      success: true,
      message: '密码修改成功',
    });
  } catch (error) {
    console.error('Change password error:', error);
    return NextResponse.json(
      { success: false, error: '修改密码失败' },
      { status: 500 }
    );
  }
}
```

### 3.8 prisma/seed.ts

```typescript
import { PrismaClient, UserRole } from '@prisma/client';
import bcrypt from 'bcryptjs';

const prisma = new PrismaClient();

async function main() {
  // 创建默认管理员用户
  const adminPassword = await bcrypt.hash('admin123', 10);
  
  const admin = await prisma.user.upsert({
    where: { username: 'admin' },
    update: {},
    create: {
      username: 'admin',
      password: adminPassword,
      name: '系统管理员',
      role: UserRole.ADMIN,
      email: 'admin@example.com',
      isActive: true,
    },
  });

  console.log('Created admin user:', admin.username);

  // 创建示例开发人员
  const devPassword = await bcrypt.hash('dev123', 10);
  
  const developer = await prisma.user.upsert({
    where: { username: 'developer' },
    update: {},
    create: {
      username: 'developer',
      password: devPassword,
      name: '开发人员',
      role: UserRole.DEVELOPER,
      email: 'dev@example.com',
      isActive: true,
    },
  });

  console.log('Created developer user:', developer.username);

  // 创建示例测试人员
  const testerPassword = await bcrypt.hash('test123', 10);
  
  const tester = await prisma.user.upsert({
    where: { username: 'tester' },
    update: {},
    create: {
      username: 'tester',
      password: testerPassword,
      name: '测试人员',
      role: UserRole.TESTER,
      email: 'tester@example.com',
      isActive: true,
    },
  });

  console.log('Created tester user:', tester.username);
}

main()
  .catch((e) => {
    console.error(e);
    process.exit(1);
  })
  .finally(async () => {
    await prisma.$disconnect();
  });
```

---

## 4. 验证方法

1. 运行 `npm run db:seed` 初始化用户数据
2. 测试登录接口：`POST /api/auth/login`
   ```json
   {
     "username": "admin",
     "password": "admin123"
   }
   ```
3. 使用返回的token测试其他接口
4. 测试用户CRUD接口

---

## 5. 交付清单

- [ ] 登录/登出接口可用
- [ ] 用户CRUD接口完整
- [ ] 权限控制正常工作
- [ ] 种子数据可正常初始化
- [ ] 无 TypeScript 编译错误
