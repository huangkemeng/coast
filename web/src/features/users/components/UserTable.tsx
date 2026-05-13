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
import { UserRoleName } from '@/types/user';
import type { UserListItem } from '@/types/user';
import { MoreHorizontal, Edit, Trash2 } from 'lucide-react';

interface UserTableProps {
  data: UserListItem[];
  isLoading?: boolean;
  onDelete?: (id: number) => void;
}

export const UserTable: React.FC<UserTableProps> = ({
  data,
  isLoading,
  onDelete,
}) => {
  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>用户名</TableHead>
          <TableHead>真实姓名</TableHead>
          <TableHead>角色</TableHead>
          <TableHead>创建时间</TableHead>
          <TableHead className="w-12">操作</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {isLoading ? (
          <TableRow>
            <TableCell colSpan={5} className="text-center py-8 text-text-muted">
              加载中...
            </TableCell>
          </TableRow>
        ) : data.length === 0 ? (
          <TableRow>
            <TableCell colSpan={5} className="text-center py-8 text-text-muted">
              暂无数据
            </TableCell>
          </TableRow>
        ) : (
          data.map((item) => (
            <TableRow key={item.id}>
              <TableCell className="font-medium">{item.username}</TableCell>
              <TableCell>{item.realName}</TableCell>
              <TableCell>
                <Badge variant={item.role === 0 ? 'secondary' : 'outline'}>
                  {UserRoleName[item.role]}
                </Badge>
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
                    <DropdownMenuItem asChild>
                      <button onClick={() => window.location.href = `/users/${item.id}/edit`}>
                        <Edit className="h-4 w-4 mr-2" />
                        编辑
                      </button>
                    </DropdownMenuItem>
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
          ))
        )}
      </TableBody>
    </Table>
  );
};