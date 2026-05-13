import React from 'react';
import { Link } from 'react-router-dom';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/Dropdown';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/Table';
import { formatDate } from '@/utils/dateUtils';
import { RequirementStatus, Priority, RequirementStatusName, PriorityName, StatusTransitions } from '@/types/requirement';
import type { RequirementListItem } from '@/types/requirement';
import { MoreHorizontal, Edit, Trash2, Eye, Play } from 'lucide-react';

interface RequirementTableProps {
  data: RequirementListItem[];
  isLoading?: boolean;
  onDelete?: (id: number) => void;
  onStatusChange?: (id: number, status: RequirementStatus) => void;
}

export const RequirementTable: React.FC<RequirementTableProps> = ({
  data,
  isLoading,
  onDelete,
  onStatusChange,
}) => {
  const getStatusVariant = (status: RequirementStatus): 'default' | 'success' | 'warning' | 'info' | 'secondary' => {
    switch (status) {
      case RequirementStatus.PendingConfirm:
        return 'default';
      case RequirementStatus.Confirmed:
        return 'secondary';
      case RequirementStatus.PendingQuote:
        return 'default';
      case RequirementStatus.Quoted:
        return 'default';
      case RequirementStatus.PendingDev:
        return 'warning';
      case RequirementStatus.InDev:
        return 'info';
      case RequirementStatus.InTest:
        return 'warning';
      case RequirementStatus.AcceptedPendingLaunch:
        return 'secondary';
      case RequirementStatus.Launched:
        return 'success';
      default:
        return 'default';
    }
  };

  const getPriorityVariant = (priority: Priority): 'default' | 'warning' | 'destructive' => {
    switch (priority) {
      case Priority.Low:
        return 'default';
      case Priority.Medium:
        return 'warning';
      case Priority.High:
        return 'destructive';
      default:
        return 'default';
    }
  };

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>需求名称</TableHead>
          <TableHead>需求编号</TableHead>
          <TableHead>项目</TableHead>
          <TableHead>跟进人</TableHead>
          <TableHead>状态</TableHead>
          <TableHead>优先级</TableHead>
          <TableHead>进度</TableHead>
          <TableHead>计划测试日期</TableHead>
          <TableHead>创建时间</TableHead>
          <TableHead className="w-12">操作</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {isLoading ? (
          <TableRow>
            <TableCell colSpan={10} className="text-center py-8 text-text-muted">
              加载中...
            </TableCell>
          </TableRow>
        ) : data.length === 0 ? (
          <TableRow>
            <TableCell colSpan={10} className="text-center py-8 text-text-muted">
              暂无数据
            </TableCell>
          </TableRow>
        ) : (
          data.map((item) => {
            const nextStatuses = StatusTransitions[item.status];
            return (
              <TableRow key={item.id}>
                <TableCell>
                  <Link
                    to={`/requirements/${item.id}`}
                    className="hover:text-primary font-medium"
                  >
                    {item.name}
                  </Link>
                </TableCell>
                <TableCell className="text-text-muted font-mono text-sm">
                  {item.requirementNo}
                </TableCell>
                <TableCell className="text-text-muted">
                  {item.projectName || '-'}
                </TableCell>
                <TableCell className="text-text-muted">
                  {item.followerName || '-'}
                </TableCell>
                <TableCell>
                  <Badge variant={getStatusVariant(item.status)}>
                    {RequirementStatusName[item.status]}
                  </Badge>
                </TableCell>
                <TableCell>
                  <Badge variant={getPriorityVariant(item.priority)}>
                    {PriorityName[item.priority]}
                  </Badge>
                </TableCell>
                <TableCell>
                  <div className="flex items-center gap-2">
                    <div className="w-16 h-2 bg-slate-700 rounded-full overflow-hidden">
                      <div
                        className="h-full bg-primary rounded-full transition-all"
                        style={{ width: `${item.progress}%` }}
                      />
                    </div>
                    <span className="text-xs text-text-muted">{item.progress}%</span>
                  </div>
                </TableCell>
                <TableCell className="text-text-muted">
                  {item.planTestDate ? formatDate(item.planTestDate) : '-'}
                </TableCell>
                <TableCell className="text-text-muted">
                  {formatDate(item.createdAt)}
                </TableCell>
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" size="icon">
                        <MoreHorizontal className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem asChild>
                        <Link to={`/requirements/${item.id}`}>
                          <Eye className="h-4 w-4 mr-2" />
                          查看详情
                        </Link>
                      </DropdownMenuItem>
                      <DropdownMenuItem asChild>
                        <Link to={`/requirements/${item.id}/edit`}>
                          <Edit className="h-4 w-4 mr-2" />
                          编辑
                        </Link>
                      </DropdownMenuItem>
                      {nextStatuses.length > 0 && (
                        <>
                          <DropdownMenuSeparator />
                          {nextStatuses.map((status) => (
                            <DropdownMenuItem
                              key={status}
                              onClick={() => onStatusChange?.(item.id, status)}
                            >
                              <Play className="h-4 w-4 mr-2" />
                              变更为{RequirementStatusName[status]}
                            </DropdownMenuItem>
                          ))}
                        </>
                      )}
                      <DropdownMenuSeparator />
                      <DropdownMenuItem
                        className="text-red-500 focus:text-red-500"
                        onClick={() => onDelete?.(item.id)}
                      >
                        <Trash2 className="h-4 w-4 mr-2" />
                        删除
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            );
          })
        )}
      </TableBody>
    </Table>
  );
};