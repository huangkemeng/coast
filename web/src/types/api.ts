export interface PageRequest {
  pageIndex: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface PageResponse<T> {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
}

export interface BaseResponse<T> {
  code: number;
  message: string;
  data: T;
}

export interface DashboardStats {
  totalRequirements: number;
  inDevCount: number;
  inTestCount: number;
  launchedCount: number;
  statusDistribution: StatusDistributionItem[];
}

export interface StatusDistributionItem {
  status: number;
  statusName: string;
  count: number;
}

export interface RequirementFilters {
  keyword?: string;
  projectId?: number | null;
  followerId?: number | null;
  status?: number | null;
  priority?: number | null;
  isConfirmed?: boolean | null;
  dateFrom?: string | null;
  dateTo?: string | null;
}

export interface NotificationFilters {
  requirementId?: number | null;
  robotId?: number | null;
  status?: number | null;
  dateFrom?: string | null;
  dateTo?: string | null;
}