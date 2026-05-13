import React, { useState } from 'react';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/Select';
import { FormField } from '@/components/ui/FormField';
import { Search, X } from 'lucide-react';
import type { NotificationFilters as NotificationFiltersType } from '@/types/api';

interface NotificationFiltersProps {
  onFilterChange: (filters: NotificationFiltersType) => void;
  robotOptions: { id: number; name: string }[];
}

interface NotificationFiltersState {
  requirementId?: number | null;
  robotId?: number | null;
  status?: number | null;
  dateFrom?: string | null;
  dateTo?: string | null;
}

export const NotificationFilters: React.FC<NotificationFiltersProps> = ({
  onFilterChange,
  robotOptions,
}) => {
  const [filters, setFilters] = useState<NotificationFiltersState>({
    requirementId: null,
    robotId: null,
    status: null,
    dateFrom: null,
    dateTo: null,
  });
  const [showAdvanced, setShowAdvanced] = useState(false);

  const handleChange = (key: keyof NotificationFiltersState, value: string | number | null) => {
    const newFilters = { ...filters, [key]: value === '' ? null : value };
    setFilters(newFilters);
    onFilterChange(newFilters);
  };

  const handleClear = () => {
    const clearedFilters = {
      requirementId: null,
      robotId: null,
      status: null,
      dateFrom: null,
      dateTo: null,
    };
    setFilters(clearedFilters);
    onFilterChange(clearedFilters);
  };

  const hasActiveFilters = Object.values(filters).some((v) => v !== null && v !== '');

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-4">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-text-muted" />
          <Input
            placeholder="搜索需求名称或编号..."
            value=""
            onChange={() => {}}
            className="pl-10"
          />
        </div>
        <Button
          variant="outline"
          onClick={() => setShowAdvanced(!showAdvanced)}
          className={showAdvanced ? 'bg-surface' : ''}
        >
          高级筛选
        </Button>
        {hasActiveFilters && (
          <Button variant="ghost" onClick={handleClear}>
            <X className="h-4 w-4 mr-2" />
            清除筛选
          </Button>
        )}
      </div>

      {showAdvanced && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 p-4 bg-surface rounded-lg border border-border">
          <FormField label="机器人">
            <Select
              value={filters.robotId?.toString() || ''}
              onValueChange={(value) => handleChange('robotId', value ? parseInt(value) : null)}
            >
              <SelectTrigger>
                <SelectValue placeholder="全部机器人" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">全部机器人</SelectItem>
                {robotOptions.map((robot) => (
                  <SelectItem key={robot.id} value={robot.id.toString()}>
                    {robot.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </FormField>

          <FormField label="状态">
            <Select
              value={filters.status?.toString() || ''}
              onValueChange={(value) => handleChange('status', value ? parseInt(value) : null)}
            >
              <SelectTrigger>
                <SelectValue placeholder="全部状态" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">全部状态</SelectItem>
                <SelectItem value="0">发送中</SelectItem>
                <SelectItem value="1">发送成功</SelectItem>
                <SelectItem value="2">发送失败</SelectItem>
              </SelectContent>
            </Select>
          </FormField>

          <FormField label="发送日期">
            <Input
              type="date"
              value={filters.dateFrom || ''}
              onChange={(e) => handleChange('dateFrom', e.target.value || null)}
            />
          </FormField>
        </div>
      )}
    </div>
  );
};