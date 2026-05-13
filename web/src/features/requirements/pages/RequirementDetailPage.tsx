import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Button } from '@/components/ui/Button';
import { Badge } from '@/components/ui/Badge';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/Tabs';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { ErrorState } from '@/components/common/ErrorState';
import { usePermission } from '@/stores/permissionStore';
import { getRequirementByIdApi } from '@/api/requirements';
import { RequirementStatusName, PriorityName } from '@/types/requirement';
import { formatDate } from '@/utils/dateUtils';
import { ArrowLeft, Edit, Clock, User, FolderKanban, Link as LinkIcon, MessageSquare, CheckCircle } from 'lucide-react';

export const RequirementDetailPage: React.FC = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { canEditRequirement, canViewPrice } = usePermission();

  const { data: requirement, isLoading, error, refetch } = useQuery({
    queryKey: ['requirement', id],
    queryFn: () => getRequirementByIdApi(Number(id)),
    enabled: !!id,
  });

  if (isLoading) return <LoadingOverlay fullScreen text="加载中..." />;
  if (error) return <ErrorState message="加载失败" onRetry={refetch} />;
  if (!requirement) return <ErrorState message="需求不存在" />;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Button variant="ghost" onClick={() => navigate('/requirements')}>
            <ArrowLeft className="h-4 w-4 mr-2" />
            返回列表
          </Button>
          <div>
            <h1 className="text-2xl font-bold">{requirement.name}</h1>
            <p className="text-text-muted mt-1 font-mono">{requirement.requirementNo}</p>
          </div>
        </div>
        {canEditRequirement(requirement) && (
          <Button onClick={() => navigate(`/requirements/${id}/edit`)}>
            <Edit className="h-4 w-4 mr-2" />
            编辑
          </Button>
        )}
      </div>

      <Tabs defaultValue="info">
        <TabsList>
          <TabsTrigger value="info">基本信息</TabsTrigger>
          <TabsTrigger value="dates">时间安排</TabsTrigger>
          <TabsTrigger value="timeline">状态流转</TabsTrigger>
        </TabsList>

        <TabsContent value="info" className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="bg-surface rounded-lg border border-border p-4">
              <h3 className="text-sm font-medium text-text-muted mb-3">基本信息</h3>
              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">状态</span>
                  <Badge variant="default">{RequirementStatusName[requirement.status]}</Badge>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">优先级</span>
                  <Badge variant={requirement.priority === 2 ? 'destructive' : requirement.priority === 1 ? 'warning' : 'default'}>
                    {PriorityName[requirement.priority]}
                  </Badge>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">进度</span>
                  <span className="font-medium">{requirement.progress}%</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">需求已确认</span>
                  <span className="flex items-center gap-1">
                    {requirement.isConfirmed ? (
                      <CheckCircle className="h-4 w-4 text-success" />
                    ) : (
                      <Clock className="h-4 w-4 text-warning" />
                    )}
                    {requirement.isConfirmed ? '是' : '否'}
                  </span>
                </div>
              </div>
            </div>

            <div className="bg-surface rounded-lg border border-border p-4">
              <h3 className="text-sm font-medium text-text-muted mb-3">关联信息</h3>
              <div className="space-y-3">
                <div className="flex items-center gap-2">
                  <FolderKanban className="h-4 w-4 text-text-muted" />
                  <span className="text-text-muted">项目</span>
                  <span className="ml-auto">{requirement.projectName || '-'}</span>
                </div>
                <div className="flex items-center gap-2">
                  <User className="h-4 w-4 text-text-muted" />
                  <span className="text-text-muted">跟进人</span>
                  <span className="ml-auto">{requirement.followerName || '-'}</span>
                </div>
                {requirement.robotName && (
                  <div className="flex items-center gap-2">
                    <span className="text-text-muted">通知机器人</span>
                    <span className="ml-auto">{requirement.robotName}</span>
                  </div>
                )}
                {canViewPrice() && requirement.price !== null && (
                  <div className="flex items-center justify-between">
                    <span className="text-text-muted">报价</span>
                    <span className="font-medium text-success">¥{requirement.price.toFixed(2)}</span>
                  </div>
                )}
              </div>
            </div>
          </div>

          {requirement.docUrl && (
            <div className="bg-surface rounded-lg border border-border p-4">
              <h3 className="text-sm font-medium text-text-muted mb-3 flex items-center gap-2">
                <LinkIcon className="h-4 w-4" />
                需求文档
              </h3>
              <a
                href={requirement.docUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="text-primary hover:underline break-all"
              >
                {requirement.docUrl}
              </a>
            </div>
          )}

          {requirement.remark && (
            <div className="bg-surface rounded-lg border border-border p-4">
              <h3 className="text-sm font-medium text-text-muted mb-3 flex items-center gap-2">
                <MessageSquare className="h-4 w-4" />
                备注
              </h3>
              <p className="whitespace-pre-wrap">{requirement.remark}</p>
            </div>
          )}

          <div className="bg-surface rounded-lg border border-border p-4">
            <h3 className="text-sm font-medium text-text-muted mb-3">版本信息</h3>
            <div className="flex items-center justify-between">
              <span className="text-text-muted">当前版本</span>
              <Badge variant="secondary">v{requirement.version}</Badge>
            </div>
          </div>
        </TabsContent>

        <TabsContent value="dates" className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="bg-surface rounded-lg border border-border p-4">
              <h3 className="text-sm font-medium text-text-muted mb-3">计划时间</h3>
              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">计划开始日期</span>
                  <span>{requirement.planStartDate ? formatDate(requirement.planStartDate) : '-'}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">计划测试日期</span>
                  <span>{requirement.planTestDate ? formatDate(requirement.planTestDate) : '-'}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">计划上线日期</span>
                  <span>{requirement.planLaunchDate ? formatDate(requirement.planLaunchDate) : '-'}</span>
                </div>
              </div>
            </div>

            <div className="bg-surface rounded-lg border border-border p-4">
              <h3 className="text-sm font-medium text-text-muted mb-3">实际时间</h3>
              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">实际测试日期</span>
                  <span>{requirement.actualTestDate ? formatDate(requirement.actualTestDate) : '-'}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">实际上线日期</span>
                  <span>{requirement.actualLaunchDate ? formatDate(requirement.actualLaunchDate) : '-'}</span>
                </div>
              </div>
            </div>
          </div>
        </TabsContent>

        <TabsContent value="timeline">
          <div className="bg-surface rounded-lg border border-border p-6">
            <h3 className="text-lg font-semibold mb-4">状态流转记录</h3>
            <div className="text-center text-text-muted py-8">
              暂不支持查看流转记录（需要后端支持）
            </div>
          </div>
        </TabsContent>
      </Tabs>

      <div className="text-sm text-text-muted">
        创建于 {formatDate(requirement.createdAt)} · 更新于 {formatDate(requirement.updatedAt)}
      </div>
    </div>
  );
};