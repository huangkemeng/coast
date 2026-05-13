import React from 'react';
import { Link } from 'react-router-dom';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/Table';
import { formatDateTime } from '@/utils/dateUtils';
import type { NotificationLogListItem } from '@/types/notification';
import { Eye } from 'lucide-react';

interface NotificationTableProps {
  data: NotificationLogListItem[];
  isLoading?: boolean;
}

export const NotificationTable: React.FC<NotificationTableProps> = ({
  data,
  isLoading,
}) => {
  const getStatusVariant = (status: number): 'pending' | 'success' | 'error' => {
    switch (status) {
      case 0: return 'pending';
      case 1: return 'success';
      case 2: return 'error';
      default: return 'pending';
    }
  };

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>需求名称</TableHead>
          <TableHead>需求编号</TableHead>
          <TableHead>机器人</TableHead>
          <TableHead>状态</TableHead>
          <TableHead>发送时间</TableHead>
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
              <TableCell>
                <Link to={`/requirements/${item.requirementId}`} className="hover:text-primary">
                  {item.requirementName}
                </Link>
              </TableCell>
              <TableCell className="text-text-muted">{item.requirementNo}</TableCell>
              <TableCell>{item.robotName}</TableCell>
              <TableCell>
                <Badge variant={getStatusVariant(item.status)}>{item.statusName}</Badge>
              </TableCell>
              <TableCell className="text-text-muted">
                {formatDateTime(item.sentAt)}
              </TableCell>
              <TableCell>
                <Button variant="ghost" size="icon" asChild>
                  <Link to={`/requirements/${item.requirementId}`}>
                    <Eye className="h-4 w-4" />
                  </Link>
                </Button>
              </TableCell>
            </TableRow>
          ))
        )}
      </TableBody>
    </Table>
  );
};