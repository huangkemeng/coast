export enum RequirementStatus {
  PendingConfirm = 0,
  Confirmed = 1,
  PendingQuote = 2,
  Quoted = 3,
  PendingDev = 4,
  InDev = 5,
  InTest = 6,
  AcceptedPendingLaunch = 7,
  Launched = 8,
}

export enum Priority {
  Low = 0,
  Medium = 1,
  High = 2,
}

export const RequirementStatusName: Record<RequirementStatus, string> = {
  [RequirementStatus.PendingConfirm]: '待确认',
  [RequirementStatus.Confirmed]: '已确认',
  [RequirementStatus.PendingQuote]: '待报价',
  [RequirementStatus.Quoted]: '已报价',
  [RequirementStatus.PendingDev]: '待开发',
  [RequirementStatus.InDev]: '开发中',
  [RequirementStatus.InTest]: '测试中',
  [RequirementStatus.AcceptedPendingLaunch]: '待上线',
  [RequirementStatus.Launched]: '已上线',
};

export const PriorityName: Record<Priority, string> = {
  [Priority.Low]: '低',
  [Priority.Medium]: '中',
  [Priority.High]: '高',
};

export interface Requirement {
  id: number;
  name: string;
  requirementNo: string;
  projectId: number;
  projectName: string | null;
  followerId: number;
  followerName: string | null;
  status: RequirementStatus;
  statusName: string;
  priority: Priority;
  priorityName: string;
  isConfirmed: boolean;
  progress: number;
  planStartDate: string | null;
  planTestDate: string | null;
  planLaunchDate: string | null;
  actualTestDate: string | null;
  actualLaunchDate: string | null;
  price: number | null;
  docUrl: string | null;
  remark: string | null;
  robotId: number | null;
  robotName: string | null;
  version: number;
  createdAt: string;
  updatedAt: string;
}

export interface RequirementListItem {
  id: number;
  name: string;
  requirementNo: string;
  projectId: number;
  projectName: string | null;
  followerId: number;
  followerName: string | null;
  status: RequirementStatus;
  statusName: string;
  priority: Priority;
  priorityName: string;
  isConfirmed: boolean;
  progress: number;
  planTestDate: string | null;
  version: number;
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
  requirementNo: string;
  projectId: number;
  followerId: number;
  priority: Priority;
  planStartDate?: string | null;
  planTestDate?: string | null;
  planLaunchDate?: string | null;
  docUrl?: string | null;
  price?: number | null;
  remark?: string | null;
  robotId?: number | null;
}

export interface UpdateRequirementRequest extends CreateRequirementRequest {
  version: number;
}

export interface ChangeRequirementStatusRequest {
  status: RequirementStatus;
  remark?: string;
}

export const StatusTransitions: Record<RequirementStatus, RequirementStatus[]> = {
  [RequirementStatus.PendingConfirm]: [RequirementStatus.Confirmed],
  [RequirementStatus.Confirmed]: [RequirementStatus.PendingQuote],
  [RequirementStatus.PendingQuote]: [RequirementStatus.Quoted],
  [RequirementStatus.Quoted]: [RequirementStatus.PendingDev],
  [RequirementStatus.PendingDev]: [RequirementStatus.InDev],
  [RequirementStatus.InDev]: [RequirementStatus.InTest],
  [RequirementStatus.InTest]: [RequirementStatus.AcceptedPendingLaunch],
  [RequirementStatus.AcceptedPendingLaunch]: [RequirementStatus.Launched],
  [RequirementStatus.Launched]: [],
};