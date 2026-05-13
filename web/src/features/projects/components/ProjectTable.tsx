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
import type { ProjectListItem } from '@/types/project';
import { MoreHorizontal, Edit, Trash2, Eye } from 'lucide-react';

interface ProjectTableProps {
  data: ProjectListItem[];
  isLoading?: boolean;
  onDelete?: (id: number) => void;
}

export const ProjectTable: React.FC<ProjectTableProps> = ({
  data,
  isLoading,
  onDelete,
}) => {
  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>项目名称</TableHead>
          <TableHead>需求数量</TableHead>
          <TableHead>创建时间</TableHead>
          <TableHead className="w-12">操作</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {isLoading ? (
          <TableRow>
            <TableCell colSpan={4} className="text-center py-8 text-text-muted">
              加载中...
            </TableCell>
          </TableRow>
        ) : data.length === 0 ? (
          <TableRow>
            <TableCell colSpan={4} className="text-center py-8 text-text-muted">
              暂无数据
            </TableCell>
          </TableRow>
        ) : (
          data.map((item) => (
            <TableRow key={item.id}>
              <TableCell>
                <div className="font-medium">{item.name}</div>
                {item.description && (
                  <div className="text-xs text-text-muted mt-1">{item.description}</div>
                )}
              </TableCell>
              <TableCell>
                <Badge variant="outline">{item.requirementCount}</Badge>
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
                      <Link to={`/requirements?projectId=${item.id}`}>
                        <Eye className="h-4 w-4 mr-2" />
                        查看需求
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuItem asChild>
                      <Link to={`/projects/${item.id}/edit`}>
                        <Edit className="h-4 w-4 mr-2" />
                        编辑
                      </Link>
                    </DropdownMenuItem>
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