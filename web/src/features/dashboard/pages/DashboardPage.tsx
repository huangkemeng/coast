import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { useAuthStore } from '@/stores/authStore';
import { getDashboardStatsApi, getRequirementsApi } from '@/api';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { ErrorState } from '@/components/common/ErrorState';
import { Badge } from '@/components/ui/Badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/Table';
import { formatDate } from '@/utils/dateUtils';
import {
  FileText,
  Code,
  FlaskConical,
  CheckCircle,
  TrendingUp,
} from 'lucide-react';
import { Link } from 'react-router-dom';

interface StatsCardProps {
  title: string;
  value: number;
  icon: React.ElementType;
  color: string;
  change?: string;
}

const StatsCard: React.FC<StatsCardProps> = ({ title, value, icon: Icon, color, change }) => {
  return (
    <div className="bg-surface rounded-lg border border-border p-6">
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm text-text-muted">{title}</p>
          <p className="text-3xl font-bold text-text-primary mt-2">{value}</p>
          {change && (
            <p className="text-xs text-success mt-1 flex items-center">
              <TrendingUp className="h-3 w-3 mr-1" />
              {change}
            </p>
          )}
        </div>
        <div className={`p-3 rounded-lg ${color}`}>
          <Icon className="h-6 w-6" />
        </div>
      </div>
    </div>
  );
};

export const DashboardPage: React.FC = () => {
  const { user } = useAuthStore();

  const { data: stats, isLoading: statsLoading, error: statsError, refetch: refetchStats } = useQuery({
    queryKey: ['dashboard-stats'],
    queryFn: getDashboardStatsApi,
  });

  const { data: recentRequirements, isLoading: requirementsLoading } = useQuery({
    queryKey: ['recent-requirements'],
    queryFn: () => getRequirementsApi({ pageIndex: 1, pageSize: 5, sortBy: 'createdAt', sortOrder: 'desc' }),
  });

  if (statsLoading) return <LoadingOverlay fullScreen text="加载中..." />;
  if (statsError) return <ErrorState message="加载失败" onRetry={refetchStats} />;

  const getStatusVariant = (status: number): 'pending' | 'dev' | 'test' | 'launched' | 'rejected' | 'paused' => {
    switch (status) {
      case 0: return 'pending';
      case 1: return 'dev';
      case 2: return 'test';
      case 3: return 'launched';
      case 4: return 'rejected';
      case 5: return 'paused';
      default: return 'pending';
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-text-primary">
          欢迎回来，{user?.realName || user?.username}
        </h1>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatsCard
          title="总需求数"
          value={stats?.totalRequirements ?? 0}
          icon={FileText}
          color="bg-primary/10 text-primary"
        />
        <StatsCard
          title="开发中"
          value={stats?.inDevCount ?? 0}
          icon={Code}
          color="bg-blue-500/10 text-blue-500"
        />
        <StatsCard
          title="测试中"
          value={stats?.inTestCount ?? 0}
          icon={FlaskConical}
          color="bg-amber-500/10 text-amber-500"
        />
        <StatsCard
          title="已上线"
          value={stats?.launchedCount ?? 0}
          icon={CheckCircle}
          color="bg-success/10 text-success"
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-surface rounded-lg border border-border p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-text-primary">最新需求</h2>
            <Link to="/requirements" className="text-sm text-primary hover:underline">
              查看全部
            </Link>
          </div>
          
          {requirementsLoading ? (
            <LoadingOverlay text="加载中..." />
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>需求名称</TableHead>
                  <TableHead>状态</TableHead>
                  <TableHead>创建时间</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {recentRequirements?.items.map((item) => (
                  <TableRow key={item.id}>
                    <TableCell>
                      <Link to={`/requirements/${item.id}`} className="hover:text-primary">
                        <div>
                          <div className="font-medium">{item.name}</div>
                          <div className="text-xs text-text-muted">{item.requirementNo}</div>
                        </div>
                      </Link>
                    </TableCell>
                    <TableCell>
                      <Badge variant={getStatusVariant(item.status)}>{item.statusName}</Badge>
                    </TableCell>
                    <TableCell className="text-text-muted">{formatDate(item.createdAt)}</TableCell>
                  </TableRow>
                ))}
                {(!recentRequirements?.items || recentRequirements.items.length === 0) && (
                  <TableRow>
                    <TableCell colSpan={3} className="text-center text-text-muted py-8">
                      暂无数据
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          )}
        </div>

        <div className="bg-surface rounded-lg border border-border p-6">
          <h2 className="text-lg font-semibold text-text-primary mb-4">状态分布</h2>
          <div className="space-y-3">
            {stats?.statusDistribution.map((item) => (
              <div key={item.status} className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <Badge variant={getStatusVariant(item.status)}>{item.statusName}</Badge>
                </div>
                <div className="flex items-center gap-2">
                  <div className="w-32 h-2 bg-slate-700 rounded-full overflow-hidden">
                    <div
                      className="h-full bg-primary"
                      style={{
                        width: `${stats.totalRequirements > 0 ? (item.count / stats.totalRequirements) * 100 : 0}%`,
                      }}
                    />
                  </div>
                  <span className="text-sm text-text-muted w-8">{item.count}</span>
                </div>
              </div>
            ))}
            {(!stats?.statusDistribution || stats.statusDistribution.length === 0) && (
              <div className="text-center text-text-muted py-8">暂无数据</div>
            )}
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="bg-surface rounded-lg border border-border p-6">
          <h3 className="text-sm text-text-muted mb-2">本周新增</h3>
          <p className="text-2xl font-bold text-text-primary">
            {recentRequirements?.items.filter((r) => {
              const created = new Date(r.createdAt);
              const now = new Date();
              const diff = now.getTime() - created.getTime();
              return diff < 7 * 24 * 60 * 60 * 1000;
            }).length ?? 0}
          </p>
        </div>
        <div className="bg-surface rounded-lg border border-border p-6">
          <h3 className="text-sm text-text-muted mb-2">紧急需求</h3>
          <p className="text-2xl font-bold text-error">
            {recentRequirements?.items.filter((r) => r.priority === 1).length ?? 0}
          </p>
        </div>
        <div className="bg-surface rounded-lg border border-border p-6">
          <h3 className="text-sm text-text-muted mb-2">今日待办</h3>
          <p className="text-2xl font-bold text-warning">
            {recentRequirements?.items.filter((r) => r.status === 1 || r.status === 2).length ?? 0}
          </p>
        </div>
      </div>
    </div>
  );
};