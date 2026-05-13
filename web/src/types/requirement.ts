export interface Requirement {
  id: number;
  name: string;
  requirementNo: string;
  projectId: number | null;
  projectName: string | null;
  followerId: number | null;
  followerName: string | null;
  status: RequirementStatus;
  statusName: string;
  priority: RequirementPriority;
  priorityName: string;
  isConfirmed: boolean;
  progress: number;
  price: number | null;
  deadline: string | null;
  docUrl: string | null;
  version: string | null;
  content: string | null;
  createdAt: string;
  updatedAt: string;
}

export enum RequirementStatus {
  待排期 = 0,
  开发中 = 1,
  测试中 = 2,
  已上线 = 3,
  已驳回 = 4,
  已暂停 = 5,
}

export enum RequirementPriority {
  普通 = 0,
  紧急 = 1,
  非常重要 = 2,
}

export interface RequirementListItem {
  id: number;
  name: string;
  requirementNo: string;
  projectName: string | null;
  followerName: string | null;
  status: RequirementStatus;
  statusName: string;
  priority: RequirementPriority;
  priorityName: string;
  isConfirmed: boolean;
  progress: number;
  deadline: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface RequirementTimeline {
  id: number;
  requirementId: number;
  oldStatus: RequirementStatus | null;
  oldStatusName: string | null;
  newStatus: RequirementStatus;
  newStatusName: string;
  operatorId: number;
  operatorName: string;
  remark: string | null;
  createdAt: string;
}

export interface CreateRequirementRequest {
  name: string;
  projectId?: number;
  followerId?: number;
  priority?: number;
  isConfirmed?: boolean;
  price?: number | null;
  deadline?: string | null;
  docUrl?: string | null;
  content?: string;
}

export interface UpdateRequirementRequest extends CreateRequirementRequest {
  version?: string;
}

export interface ChangeStatusRequest {
  status: RequirementStatus;
  remark?: string;
}