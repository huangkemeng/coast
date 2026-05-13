export interface Robot {
  id: number;
  name: string;
  webhookUrl: string;
  secret: string | null;
  isEnabled: boolean;
  isVerified: boolean | null;
  lastVerificationTime: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface RobotListItem {
  id: number;
  name: string;
  webhookUrl: string;
  isEnabled: boolean;
  isVerified: boolean | null;
  lastVerificationTime: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateRobotRequest {
  name: string;
  webhookUrl: string;
  secret?: string;
  isEnabled?: boolean;
}

export interface UpdateRobotRequest extends CreateRobotRequest {}

export interface TestRobotRequest {
  webhookUrl: string;
  secret?: string;
}

export interface TestRobotResponse {
  success: boolean;
  message: string;
}