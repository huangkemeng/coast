# 后端代码生成计划 - Plan 1: 项目脚手架与数据库配置

## 计划信息
- **计划编号**: Plan 1
- **项目名称**: 需求跟踪管理系统 - 后端
- **目标**: 搭建项目基础架构，配置数据库连接
- **依赖**: 无（首个计划）

---

## 1. 目标描述

搭建 Next.js + TypeScript + Prisma + MySQL 后端项目基础架构，完成数据库配置和基础工具链设置。

---

## 2. 文件清单

### 2.1 配置文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `package.json` | 创建 | 项目依赖配置 |
| `tsconfig.json` | 创建 | TypeScript配置 |
| `.env` | 创建 | 环境变量配置 |
| `.env.example` | 创建 | 环境变量模板 |
| `.gitignore` | 创建 | Git忽略配置 |
| `next.config.js` | 创建 | Next.js配置 |
| `prisma/schema.prisma` | 创建 | 数据库Schema定义 |

### 2.2 源代码文件

| 文件路径 | 操作 | 说明 |
|---------|------|------|
| `src/lib/prisma.ts` | 创建 | Prisma客户端实例 |
| `src/lib/utils.ts` | 创建 | 工具函数 |
| `src/types/index.ts` | 创建 | 类型定义文件 |
| `src/middleware.ts` | 创建 | 全局中间件 |
| `app/api/health/route.ts` | 创建 | 健康检查接口 |
| `app/layout.ts` | 创建 | 根布局文件 |

---

## 3. 实现细节

### 3.1 package.json

```json
{
  "name": "requirement-tracker-backend",
  "version": "1.0.0",
  "private": true,
  "scripts": {
    "dev": "next dev",
    "build": "next build",
    "start": "next start",
    "lint": "next lint",
    "db:generate": "prisma generate",
    "db:migrate": "prisma migrate dev",
    "db:studio": "prisma studio",
    "db:seed": "tsx prisma/seed.ts"
  },
  "dependencies": {
    "next": "^14.2.0",
    "@prisma/client": "^5.12.0",
    "bcryptjs": "^2.4.3",
    "jsonwebtoken": "^9.0.2",
    "zod": "^3.22.4",
    "dayjs": "^1.11.10",
    "axios": "^1.6.8"
  },
  "devDependencies": {
    "@types/bcryptjs": "^2.4.6",
    "@types/jsonwebtoken": "^9.0.6",
    "@types/node": "^20.12.0",
    "@types/react": "^18.2.0",
    "prisma": "^5.12.0",
    "tsx": "^4.7.0",
    "typescript": "^5.4.0"
  }
}
```

### 3.2 tsconfig.json

```json
{
  "compilerOptions": {
    "lib": ["dom", "dom.iterable", "esnext"],
    "allowJs": true,
    "skipLibCheck": true,
    "strict": true,
    "noEmit": true,
    "esModuleInterop": true,
    "module": "esnext",
    "moduleResolution": "bundler",
    "resolveJsonModule": true,
    "isolatedModules": true,
    "jsx": "preserve",
    "incremental": true,
    "plugins": [{ "name": "next" }],
    "paths": {
      "@/*": ["./src/*"]
    }
  },
  "include": ["next-env.d.ts", "**/*.ts", "**/*.tsx", ".next/types/**/*.ts"],
  "exclude": ["node_modules"]
}
```

### 3.3 .env

```env
# Database
DATABASE_URL="mysql://root:123456@localhost:3306/requirement_tracker"

# JWT
JWT_SECRET="your-secret-key-change-in-production"
JWT_EXPIRES_IN="7d"

# App
NEXT_PUBLIC_APP_URL="http://localhost:3000"
PORT=3001
```

### 3.4 .env.example

```env
# Database
DATABASE_URL="mysql://username:password@localhost:3306/requirement_tracker"

# JWT
JWT_SECRET="your-secret-key"
JWT_EXPIRES_IN="7d"

# App
NEXT_PUBLIC_APP_URL="http://localhost:3000"
PORT=3001
```

### 3.5 .gitignore

```gitignore
# Dependencies
node_modules

# Next.js
.next/
out/

# Environment
.env
.env.local
.env.*.local

# Debug
npm-debug.log*
yarn-debug.log*
yarn-error.log*

# IDE
.idea
.vscode
*.swp
*.swo

# OS
.DS_Store
Thumbs.db

# Prisma
prisma/*.db
```

### 3.6 next.config.js

```javascript
/** @type {import('next').NextConfig} */
const nextConfig = {
  experimental: {
    serverComponentsExternalPackages: ['@prisma/client'],
  },
  async headers() {
    return [
      {
        source: '/api/:path*',
        headers: [
          { key: 'Access-Control-Allow-Credentials', value: 'true' },
          { key: 'Access-Control-Allow-Origin', value: '*' },
          { key: 'Access-Control-Allow-Methods', value: 'GET,DELETE,PATCH,POST,PUT,OPTIONS' },
          { key: 'Access-Control-Allow-Headers', value: 'X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Content-Type, Date, X-Api-Version, Authorization' },
        ],
      },
    ];
  },
};

module.exports = nextConfig;
```

### 3.7 prisma/schema.prisma

```prisma
generator client {
  provider = "prisma-client-js"
}

datasource db {
  provider = "mysql"
  url      = env("DATABASE_URL")
}

// 用户表
model User {
  id        String   @id @default(uuid())
  username  String   @unique
  password  String
  name      String
  role      UserRole @default(DEVELOPER)
  phone     String?
  email     String?
  isActive  Boolean  @default(true)
  createdAt DateTime @default(now())
  updatedAt DateTime @updatedAt

  // 关联
  requirements Requirement[] @relation("FollowerRequirements")
  projects     Project[]     @relation("ProjectManager")

  @@map("users")
}

enum UserRole {
  ADMIN
  DEVELOPER
  TESTER
}

// 项目表
model Project {
  id          String   @id @default(uuid())
  name        String
  code        String?  @unique
  managerId   String?
  description String?
  createdAt   DateTime @default(now())
  updatedAt   DateTime @updatedAt

  // 关联
  manager      User?         @relation("ProjectManager", fields: [managerId], references: [id], onDelete: SetNull)
  requirements Requirement[]

  @@map("projects")
}

// 企业微信机器人配置表
model WebhookBot {
  id        String   @id @default(uuid())
  name      String
  webhookUrl String
  groupName String?
  isActive  Boolean  @default(true)
  createdAt DateTime @default(now())
  updatedAt DateTime @updatedAt

  // 关联
  requirements Requirement[]
  notifications NotificationLog[]

  @@map("webhook_bots")
}

// 需求表
model Requirement {
  id                String           @id @default(uuid())
  title             String
  requirementNo     String           @unique
  status            RequirementStatus @default(PENDING_CONFIRM)
  progress          Int              @default(0)
  followerId        String
  plannedStartDate  DateTime?
  plannedTestDate   DateTime?
  plannedReleaseDate DateTime?
  actualTestDate    DateTime?
  actualReleaseDate DateTime?
  isConfirmed       Boolean          @default(false)
  docUrl            String?
  quoteAmount       Decimal?         @db.Decimal(10, 2)
  projectId         String
  botId             String?
  priority          Priority         @default(MEDIUM)
  remark            String?
  createdAt         DateTime         @default(now())
  updatedAt         DateTime         @updatedAt

  // 关联
  follower     User              @relation("FollowerRequirements", fields: [followerId], references: [id], onDelete: Restrict)
  project      Project           @relation(fields: [projectId], references: [id], onDelete: Restrict)
  bot          WebhookBot?       @relation(fields: [botId], references: [id], onDelete: SetNull)
  notifications NotificationLog[]

  @@map("requirements")
}

enum RequirementStatus {
  PENDING_CONFIRM    // 待确认
  CONFIRMED          // 已确认
  PENDING_QUOTE      // 待报价
  QUOTED             // 已报价
  PENDING_DEVELOP    // 待开发
  DEVELOPING         // 开发中
  TESTING            // 测试中
  ACCEPTED_PENDING_RELEASE // 已验收待上线
  RELEASED           // 已上线
}

enum Priority {
  HIGH
  MEDIUM
  LOW
}

// 通知日志表
model NotificationLog {
  id          String           @id @default(uuid())
  type        NotificationType
  requirementId String?
  botId       String?
  content     String           @db.Text
  status      NotificationStatus
  errorMsg    String?
  createdAt   DateTime         @default(now())

  // 关联
  requirement Requirement? @relation(fields: [requirementId], references: [id], onDelete: SetNull)
  bot         WebhookBot?  @relation(fields: [botId], references: [id], onDelete: SetNull)

  @@map("notification_logs")
}

enum NotificationType {
  STATUS_CHANGE
  TIME_REMINDER
}

enum NotificationStatus {
  PENDING
  SUCCESS
  FAILED
}
```

### 3.8 src/lib/prisma.ts

```typescript
import { PrismaClient } from '@prisma/client';

const globalForPrisma = globalThis as unknown as {
  prisma: PrismaClient | undefined;
};

export const prisma = globalForPrisma.prisma ?? new PrismaClient();

if (process.env.NODE_ENV !== 'production') globalForPrisma.prisma = prisma;
```

### 3.9 src/lib/utils.ts

```typescript
import { type ClassValue, clsx } from 'clsx';

export function cn(...inputs: ClassValue[]) {
  return clsx(inputs);
}

export function generateResponse<T>(
  success: boolean,
  data?: T,
  message?: string,
  error?: string
) {
  return {
    success,
    data,
    message,
    error,
  };
}

export function formatDate(date: Date | string | null): string | null {
  if (!date) return null;
  const d = new Date(date);
  return d.toISOString().split('T')[0];
}

export function formatDateTime(date: Date | string | null): string | null {
  if (!date) return null;
  const d = new Date(date);
  return d.toISOString();
}
```

### 3.10 src/types/index.ts

```typescript
import { UserRole, RequirementStatus, Priority, NotificationType, NotificationStatus } from '@prisma/client';

export { UserRole, RequirementStatus, Priority, NotificationType, NotificationStatus };

export interface ApiResponse<T = unknown> {
  success: boolean;
  data?: T;
  message?: string;
  error?: string;
}

export interface PaginationParams {
  page?: number;
  pageSize?: number;
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface JwtPayload {
  userId: string;
  username: string;
  role: UserRole;
}
```

### 3.11 src/middleware.ts

```typescript
import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';
import { verifyToken } from './lib/auth';

const PUBLIC_PATHS = ['/api/auth/login', '/api/health'];

export async function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (PUBLIC_PATHS.some(path => pathname.startsWith(path))) {
    return NextResponse.next();
  }

  if (!pathname.startsWith('/api/')) {
    return NextResponse.next();
  }

  const token = request.headers.get('authorization')?.replace('Bearer ', '');

  if (!token) {
    return NextResponse.json(
      { success: false, error: '未提供认证令牌' },
      { status: 401 }
    );
  }

  try {
    const payload = await verifyToken(token);
    const requestHeaders = new Headers(request.headers);
    requestHeaders.set('x-user-id', payload.userId);
    requestHeaders.set('x-user-role', payload.role);

    return NextResponse.next({
      request: { headers: requestHeaders },
    });
  } catch {
    return NextResponse.json(
      { success: false, error: '认证令牌无效' },
      { status: 401 }
    );
  }
}

export const config = {
  matcher: ['/api/:path*'],
};
```

### 3.12 app/api/health/route.ts

```typescript
import { NextResponse } from 'next/server';
import { prisma } from '@/src/lib/prisma';

export async function GET() {
  try {
    await prisma.$queryRaw`SELECT 1`;
    return NextResponse.json({
      success: true,
      data: { status: 'healthy', database: 'connected' },
    });
  } catch (error) {
    return NextResponse.json(
      {
        success: false,
        error: '数据库连接失败',
        details: error instanceof Error ? error.message : 'Unknown error',
      },
      { status: 500 }
    );
  }
}
```

### 3.13 app/layout.ts

```typescript
export const metadata = {
  title: '需求跟踪管理系统 API',
  description: 'Requirement Tracker Backend API',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return children;
}
```

### 3.14 src/lib/auth.ts

```typescript
import jwt from 'jsonwebtoken';
import bcrypt from 'bcryptjs';
import { JwtPayload } from '@/src/types';

const JWT_SECRET = process.env.JWT_SECRET || 'default-secret';
const JWT_EXPIRES_IN = process.env.JWT_EXPIRES_IN || '7d';

export async function hashPassword(password: string): Promise<string> {
  return bcrypt.hash(password, 10);
}

export async function verifyPassword(
  password: string,
  hashedPassword: string
): Promise<boolean> {
  return bcrypt.compare(password, hashedPassword);
}

export function generateToken(payload: JwtPayload): string {
  return jwt.sign(payload, JWT_SECRET, { expiresIn: JWT_EXPIRES_IN });
}

export function verifyToken(token: string): JwtPayload {
  return jwt.verify(token, JWT_SECRET) as JwtPayload;
}
```

---

## 4. 验证方法

1. 运行 `npm install` 安装依赖
2. 运行 `npx prisma migrate dev --name init` 初始化数据库
3. 运行 `npm run dev` 启动开发服务器
4. 访问 `http://localhost:3001/api/health` 应返回健康状态

---

## 5. 交付清单

- [ ] 所有配置文件已创建
- [ ] Prisma schema 定义完成
- [ ] 数据库迁移成功执行
- [ ] 健康检查接口可正常访问
- [ ] 无 TypeScript 编译错误
