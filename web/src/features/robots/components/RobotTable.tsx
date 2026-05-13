import React from 'react';
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
import { formatDateTime } from '@/utils/dateUtils';
import type { RobotListItem } from '@/types/robot';
import { MoreHorizontal, Edit, Trash2, Play } from 'lucide-react';

interface RobotTableProps {
  data: RobotListItem[];
  isLoading?: boolean;
  onDelete?: (id: number) => void;
  onTest?: (id: number) => void;
}

export const RobotTable: React.FC<RobotTableProps> = ({
  data,
  isLoading,
  onDelete,
  onTest,
}) => {
  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>机器人名称</TableHead>
          <TableHead>Webhook 地址</TableHead>
          <TableHead>启用状态</TableHead>
          <TableHead>验证状态</TableHead>
          <TableHead>创建时间</TableHead>
          <TableHead className="w-12">操作</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {isLoading ? (
          <TableRow>
            <TableCell colSpan={6} className="text-center py-8 text-text-muted">
              加载中...
            </TableCell>
          </TableRow>
        ) : data.length === 0 ? (
          <TableRow>
            <TableCell colSpan={6} className="text-center py-8 text-text-muted">
              暂无数据
            </TableCell>
          </TableRow>
        ) : (
          data.map((item) => (
            <TableRow key={item.id}>
              <TableCell className="font-medium">{item.name}</TableCell>
              <TableCell>
                <span className="text-xs text-text-muted truncate max-w-[200px] block">
                  {item.webhookUrl}
                </span>
              </TableCell>
              <TableCell>
                <Badge variant={item.isEnabled ? 'success' : 'error'}>
                  {item.isEnabled ? '启用' : '禁用'}
                </Badge>
              </TableCell>
              <TableCell>
                {item.isVerified === null ? (
                  <Badge variant="outline">未验证</Badge>
                ) : item.isVerified ? (
                  <Badge variant="success">已验证</Badge>
                ) : (
                  <Badge variant="error">验证失败</Badge>
                )}
              </TableCell>
              <TableCell className="text-text-muted">
                {formatDateTime(item.createdAt)}
              </TableCell>
              <TableCell>
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" size="icon">
                      <MoreHorizontal className="h-4 w-4" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem onClick={() => onTest?.(item.id)}>
                      <Play className="h-4 w-4 mr-2" />
                      测试
                    </DropdownMenuItem>
                    <DropdownMenuItem asChild>
                      <button onClick={() => window.location.href = `/robots/${item.id}/edit`}>
                        <Edit className="h-4 w-4 mr-2" />
                        编辑
                      </button>
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