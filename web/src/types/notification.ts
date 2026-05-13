export interface NotificationLog {
  id: number;
  requirementId: number;
  requirementName: string;
  requirementNo: string;
  robotId: number;
  robotName: string;
  status: NotificationStatus;
  statusName: string;
  message: string | null;
  sentAt: string;
  createdAt: string;
}

export enum NotificationStatus {
  发送中 = 0,
  发送成功 = 1,
  发送失败 = 2,
}

export interface NotificationLogListItem {
  id: number;
  requirementId: number;
  requirementName: string;
  requirementNo: string;
  robotName: string;
  status: NotificationStatus;
  statusName: string;
  sentAt: string;
  createdAt: string;
}