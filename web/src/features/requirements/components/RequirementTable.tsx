import React from 'react';
import { Link } from 'react-router-dom';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { Checkbox } from '@/components/ui/Checkbox';
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
import type { RequirementListItem } from '@/types/requirement';
import { MoreHorizontal, Edit, Trash2, Eye, ArrowRight } from 'lucide-react';

interface RequirementTableProps {
  data: RequirementListItem[];
  isLoading?: boolean;
  onDelete?: (id: number) => void;
  onStatusChange?: (id: number, status: number) => void;
}

export const RequirementTable: React.FC<RequirementTableProps> = ({
  data,
  isLoading,
  onDelete,
  onStatusChange,
}) => {
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

  const getPriorityVariant = (priority: number): 'default' | 'warning' | 'error' => {
    switch (priority) {
      case 1: return 'warning';
      case 2: return 'error';
      default: return 'default';
    }
  };

  const priorityLabels = ['普通', '紧急', '非常重要'];

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-12">
            <Checkbox />
          </TableHead>
          <TableHead>需求名称</TableHead>
          <TableHead>项目</TableHead>
          <TableHead>跟进人</TableHead>
          <TableHead>状态</TableHead>
          <TableHead>优先级</TableHead>
          <TableHead>截止日期</TableHead>
          <TableHead>创建时间</TableHead>
          <TableHead className="w-12">操作</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {isLoading ? (
          <TableRow>
            <TableCell colSpan={9} className="text-center py-8 text-text-muted">
              加载中...
            </TableCell>
          </TableRow>
        ) : data.length === 0 ? (
          <TableRow>
            <TableCell colSpan={9} className="text-center py-8 text-text-muted">
              暂无数据
            </TableCell>
          </TableRow>
        ) : (
          data.map((item) => (
            <TableRow key={item.id}>
              <TableCell>
                <Checkbox />
              </TableCell>
              <TableCell>
                <Link
                  to={`/requirements/${item.id}`}
                  className="hover:text-primary"
                >
                  <div className="font-medium">{item.name}</div>
                  <div className="text-xs text-text-muted">{item.requirementNo}</div>
                </Link>
              </TableCell>
              <TableCell className="text-text-muted">
                {item.projectName || '-'}
              </TableCell>
              <TableCell className="text-text-muted">
                {item.followerName || '-'}
              </TableCell>
              <TableCell>
                <Badge variant={getStatusVariant(item.status)}>{item.statusName}</Badge>
              </TableCell>
              <TableCell>
                <Badge variant={getPriorityVariant(item.priority)}>
                  {priorityLabels[item.priority]}
                </Badge>
              </TableCell>
              <TableCell className="text-text-muted">
                {item.deadline ? formatDate(item.deadline) : '-'}
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
                    <DropdownMenuSeparator />
                    {item.status < 3 && (
                      <DropdownMenuItem onClick={() => onStatusChange?.(item.id, item.status + 1)}>
                        <ArrowRight className="h-4 w-4 mr-2" />
                        下一状态
                      </DropdownMenuItem>
                    )}
                    <DropdownMenuSeparator />
                    <DropdownMenuItem
                      onClick={() => onDelete?.(item.id)}
                      className="text-error"
                    >
                      <Trash2 className="h-4 w-4 mr-2" />
                      删除
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </TableCell>
            </TableRow>
          ))
        )}
      </TableBody>
    </Table>
  );
};