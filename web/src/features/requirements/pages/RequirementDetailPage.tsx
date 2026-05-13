import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Button } from '@/components/ui/Button';
import { Badge } from '@/components/ui/Badge';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/Tabs';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';
import { ErrorState } from '@/components/common/ErrorState';
import { usePermission } from '@/stores/permissionStore';
import { getRequirementByIdApi, getRequirementTimelineApi } from '@/api/requirements';
import { formatDateTime } from '@/utils/dateUtils';
import { formatPrice } from '@/utils/formatters';
import { ArrowLeft, Edit, AlertCircle, Check } from 'lucide-react';

export const RequirementDetailPage: React.FC = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { canEditRequirement, canViewPrice } = usePermission();

  const {
    data: requirement,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['requirement', id],
    queryFn: () => getRequirementByIdApi(Number(id)),
    enabled: !!id,
  });

  const { data: timeline } = useQuery({
    queryKey: ['requirement-timeline', id],
    queryFn: () => getRequirementTimelineApi(Number(id)),
    enabled: !!id && !!requirement,
  });

  if (isLoading) return <LoadingOverlay fullScreen text="加载中..." />;
  if (error) return <ErrorState message="加载失败" onRetry={refetch} />;
  if (!requirement) return <ErrorState message="需求不存在" />;

  const canEdit = canEditRequirement(requirement);
  const statusVariants: Record<number, 'pending' | 'dev' | 'test' | 'launched' | 'rejected' | 'paused'> = {
    0: 'pending',
    1: 'dev',
    2: 'test',
    3: 'launched',
    4: 'rejected',
    5: 'paused',
  };

  const priorityLabels = ['普通', '紧急', '非常重要'];
  const priorityVariants: ('default' | 'warning' | 'error')[] = ['default', 'warning', 'error'];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Button variant="ghost" onClick={() => navigate('/requirements')}>
            <ArrowLeft className="h-4 w-4 mr-2" />
            返回列表
          </Button>
          <div>
            <h1 className="text-2xl font-bold text-text-primary">{requirement.name}</h1>
            <p className="text-text-muted mt-1">需求号: {requirement.requirementNo}</p>
          </div>
        </div>
        {canEdit && (
          <Button onClick={() => navigate(`/requirements/${id}/edit`)}>
            <Edit className="h-4 w-4 mr-2" />
            编辑
          </Button>
        )}
      </div>

      <Tabs defaultValue="info">
        <TabsList>
          <TabsTrigger value="info">基本信息</TabsTrigger>
          <TabsTrigger value="timeline">状态流转</TabsTrigger>
        </TabsList>

        <TabsContent value="info">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div className="bg-surface rounded-lg border border-border p-6 space-y-4">
              <h3 className="text-lg font-semibold text-text-primary border-b border-border pb-2">
                基本信息
              </h3>
              
              <div className="flex items-center justify-between">
                <span className="text-text-muted">状态</span>
                <Badge variant={statusVariants[requirement.status]}>
                  {requirement.statusName}
                </Badge>
              </div>

              <div className="flex items-center justify-between">
                <span className="text-text-muted">优先级</span>
                <Badge variant={priorityVariants[requirement.priority]}>
                  {priorityLabels[requirement.priority]}
                </Badge>
              </div>

              <div className="flex items-center justify-between">
                <span className="text-text-muted">需求已确认</span>
                {requirement.isConfirmed ? (
                  <span className="flex items-center text-success">
                    <Check className="h-4 w-4 mr-1" />
                    已确认
                  </span>
                ) : (
                  <span className="flex items-center text-text-muted">
                    <AlertCircle className="h-4 w-4 mr-1" />
                    未确认
                  </span>
                )}
              </div>

              <div className="flex items-center justify-between">
                <span className="text-text-muted">项目</span>
                <span className="text-text-primary">{requirement.projectName || '-'}</span>
              </div>

              <div className="flex items-center justify-between">
                <span className="text-text-muted">跟进人</span>
                <span className="text-text-primary">{requirement.followerName || '-'}</span>
              </div>

              {canViewPrice() && requirement.price !== null && (
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">报价</span>
                  <span className="text-text-primary font-semibold">{formatPrice(requirement.price)}</span>
                </div>
              )}

              {requirement.deadline && (
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">截止日期</span>
                  <span className="text-text-primary">{requirement.deadline.split('T')[0]}</span>
                </div>
              )}
            </div>

            <div className="bg-surface rounded-lg border border-border p-6 space-y-4">
              <h3 className="text-lg font-semibold text-text-primary border-b border-border pb-2">
                更多信息
              </h3>

              {requirement.version && (
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">版本号</span>
                  <span className="text-text-primary">{requirement.version}</span>
                </div>
              )}

              {requirement.docUrl && (
                <div className="flex items-center justify-between">
                  <span className="text-text-muted">文档链接</span>
                  <a
                    href={requirement.docUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="text-primary hover:underline"
                  >
                    查看文档
                  </a>
                </div>
              )}

              <div className="flex items-center justify-between">
                <span className="text-text-muted">创建时间</span>
                <span className="text-text-primary">{formatDateTime(requirement.createdAt)}</span>
              </div>

              <div className="flex items-center justify-between">
                <span className="text-text-muted">更新时间</span>
                <span className="text-text-primary">{formatDateTime(requirement.updatedAt)}</span>
              </div>
            </div>
          </div>

          {requirement.content && (
            <div className="bg-surface rounded-lg border border-border p-6 mt-6">
              <h3 className="text-lg font-semibold text-text-primary mb-4">需求描述</h3>
              <div className="prose prose-invert max-w-none">
                <p className="text-text-primary whitespace-pre-wrap">{requirement.content}</p>
              </div>
            </div>
          )}
        </TabsContent>

        <TabsContent value="timeline">
          <div className="bg-surface rounded-lg border border-border p-6">
            <h3 className="text-lg font-semibold text-text-primary mb-4">状态流转记录</h3>
            
            {!timeline?.length ? (
              <div className="text-center text-text-muted py-8">暂无流转记录</div>
            ) : (
              <div className="relative">
                <div className="absolute left-4 top-0 bottom-0 w-0.5 bg-border" />
                
                <div className="space-y-6">
                  {timeline.map((item, index) => (
                    <div key={item.id} className="relative pl-10">
                      <div
                        className={`absolute left-2 top-1 w-4 h-4 rounded-full border-2 ${
                          index === 0 ? 'border-primary' : 'border-border'
                        }`}
                        style={{ backgroundColor: index === 0 ? 'var(--color-primary)' : undefined }}
                      />
                      
                      <div className="bg-slate-700/50 rounded-lg p-4">
                        <div className="flex items-center justify-between mb-2">
                          <div className="flex items-center gap-2">
                            <Badge variant="default">{item.newStatusName}</Badge>
                            {item.oldStatus !== null && (
                              <>
                                <span className="text-text-muted">←</span>
                                <Badge variant="outline">{item.oldStatusName}</Badge>
                              </>
                            )}
                          </div>
                          <span className="text-sm text-text-muted">
                            {formatDateTime(item.createdAt)}
                          </span>
                        </div>
                        
                        <div className="text-sm text-text-primary">
                          操作人: {item.operatorName}
                        </div>
                        
                        {item.remark && (
                          <p className="text-sm text-text-muted mt-2">
                            备注: {item.remark}
                          </p>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        </TabsContent>
      </Tabs>
    </div>
  );
};