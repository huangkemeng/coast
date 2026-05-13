export enum NotificationType {
  StatusChange = 0,
  Reminder = 1,
  Test = 2,
}

export enum NotificationStatus {
  Pending = 0,
  Success = 1,
  Failed = 2,
}

export const NotificationStatusName: Record<NotificationStatus, string> = {
  [NotificationStatus.Pending]: '发送中',
  [NotificationStatus.Success]: '发送成功',
  [NotificationStatus.Failed]: '发送失败',
};

export interface NotificationLog {
  id: number;
  requirementId: number;
  requirementName: string;
  requirementNo: string;
  robotId: number;
  robotName: string;
  notificationType: NotificationType;
  status: NotificationStatus;
  statusName: string;
  message: string | null;
  sentAt: string | null;
  createdAt: string;
}

export interface NotificationLogListItem {
  id: number;
  requirementId: number;
  requirementName: string;
  requirementNo: string;
  robotName: string;
  notificationType: NotificationType;
  status: NotificationStatus;
  statusName: string;
  sentAt: string | null;
  createdAt: string;
}