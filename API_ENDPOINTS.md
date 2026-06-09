# API 端点文档

> 本文档汇总项目中的所有 API 端点，供 AI Coding 参考现有模式。

---

## 用户管理 (User)

### 认证相关

| 方法 | 端点 | 描述 | 状态 |
|------|------|------|------|
| POST | `/api/user/login` | 用户登录 | ✅ 已实现 |
| POST | `/api/user/register` | 用户注册 | 🔜 待实现 |
| POST | `/api/user/refresh` | 刷新 Token | 🔜 待实现 |
| POST | `/api/user/logout` | 退出登录 | 🔜 待实现 |
| POST | `/api/user/forgot-password` | 忘记密码 | 🔜 待实现 |
| POST | `/api/user/change-password` | 修改密码 | 🔜 待实现 |

### 用户信息

| 方法 | 端点 | 描述 | 状态 |
|------|------|------|------|
| GET | `/api/user` | 获取用户信息 | ✅ 已实现 |
| GET | `/api/user/{id}` | 根据ID获取用户 | 🔜 待实现 |
| GET | `/api/user/list` | 用户列表 | 🔜 待实现 |
| PUT | `/api/user/{id}` | 更新用户信息 | 🔜 待实现 |
| DELETE | `/api/user/{id}` | 删除用户 | 🔜 待实现 |

---

## 需求管理 (Requirement)

### 需求 CRUD

| 方法 | 端点 | 描述 | 状态 |
|------|------|------|------|
| GET | `/api/requirement` | 需求列表（分页、筛选） | 🔜 待实现 |
| GET | `/api/requirement/{id}` | 获取需求详情 | 🔜 待实现 |
| POST | `/api/requirement` | 创建需求 | 🔜 待实现 |
| PUT | `/api/requirement/{id}` | 更新需求 | 🔜 待实现 |
| DELETE | `/api/requirement/{id}` | 删除需求 | 🔜 待实现 |

### 需求操作

| 方法 | 端点 | 描述 | 状态 |
|------|------|------|------|
| PUT | `/api/requirement/{id}/status` | 更新需求状态 | 🔜 待实现 |
| PUT | `/api/requirement/{id}/progress` | 更新需求进度 | 🔜 待实现 |

---

## 项目管理 (Project)

| 方法 | 端点 | 描述 | 状态 |
|------|------|------|------|
| GET | `/api/project` | 项目列表 | 🔜 待实现 |
| GET | `/api/project/{id}` | 获取项目详情 | 🔜 待实现 |
| POST | `/api/project` | 创建项目 | 🔜 待实现 |
| PUT | `/api/project/{id}` | 更新项目 | 🔜 待实现 |
| DELETE | `/api/project/{id}` | 删除项目 | 🔜 待实现 |

---

## 机器人配置 (Robot)

| 方法 | 端点 | 描述 | 状态 |
|------|------|------|------|
| GET | `/api/robot` | 机器人列表 | 🔜 待实现 |
| GET | `/api/robot/{id}` | 获取机器人详情 | 🔜 待实现 |
| POST | `/api/robot` | 创建机器人 | 🔜 待实现 |
| PUT | `/api/robot/{id}` | 更新机器人 | 🔜 待实现 |
| DELETE | `/api/robot/{id}` | 删除机器人 | 🔜 待实现 |
| POST | `/api/robot/{id}/test` | 测试机器人连接 | 🔜 待实现 |

---

## 通知管理 (Notification)

| 方法 | 端点 | 描述 | 状态 |
|------|------|------|------|
| GET | `/api/notification` | 通知列表 | 🔜 待实现 |
| GET | `/api/notification/log` | 通知日志 | 🔜 待实现 |

---

## API 响应格式

### 成功响应
```json
{
  "success": true,
  "data": { ... },
  "message": null
}
```

### 分页响应
```json
{
  "success": true,
  "data": {
    "items": [...],
    "totalCount": 100,
    "totalPages": 5,
    "page": 1,
    "pageSize": 20
  },
  "message": null
}
```

### 错误响应
```json
{
  "success": false,
  "data": null,
  "message": "错误信息",
  "errors": ["详细错误1", "详细错误2"]
}
```

---

## HTTP 状态码使用规范

| 状态码 | 用途 |
|--------|------|
| 200 OK | 成功响应 |
| 201 Created | 资源创建成功 |
| 204 No Content | 删除成功（无返回内容） |
| 400 Bad Request | 请求参数错误 |
| 401 Unauthorized | 未登录或 Token 无效 |
| 403 Forbidden | 无权限 |
| 404 Not Found | 资源不存在 |
| 409 Conflict | 资源冲突（如重复创建） |
| 422 Unprocessable Entity | 验证错误 |
| 500 Internal Server Error | 服务器内部错误 |

---

## 控制器文件位置

所有控制器位于: `src/Coast.Api/src/Coast.Api/Controllers/`

示例:
- [UserController.cs](src/Coast.Api/src/Coast.Api/Controllers/UserController.cs) - 用户相关接口

---

**维护者**: AI Coding Team  
**版本**: v1.0  
**最后更新**: 2026-06-10