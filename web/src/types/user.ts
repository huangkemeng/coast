export enum UserRole {
  Admin = 0,
  Developer = 1,
  Tester = 2,
}

export const UserRoleName: Record<UserRole, string> = {
  [UserRole.Admin]: '管理员',
  [UserRole.Developer]: '开发',
  [UserRole.Tester]: '测试',
};

export interface User {
  id: number;
  username: string;
  realName: string;
  role: UserRole;
  phone: string | null;
  email: string | null;
  isEnabled: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface UserListItem {
  id: number;
  username: string;
  realName: string;
  role: UserRole;
  phone: string | null;
  email: string | null;
  isEnabled: boolean;
  createdAt: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
}

export interface CreateUserRequest {
  username: string;
  password: string;
  realName: string;
  role: UserRole;
  phone?: string;
  email?: string;
}

export interface UpdateUserRequest extends CreateUserRequest {
  isEnabled?: boolean;
}