import React, { useState } from 'react';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/Select';
import { Badge } from '@/components/ui/Badge';
import { FormField } from '@/components/ui/FormField';
import { Search, X, Filter } from 'lucide-react';

interface RequirementFiltersProps {
  onFilterChange: (filters: RequirementFiltersState) => void;
  projectOptions: { id: number; name: string }[];
  userOptions: { id: number; name: string }[];
}

interface RequirementFiltersState {
  keyword: string;
  projectId: number | null;
  status: number | null;
  priority: number | null;
  isConfirmed: boolean | null;
}

export const RequirementFilters: React.FC<RequirementFiltersProps> = ({
  onFilterChange,
  projectOptions,
  userOptions,
}) => {
  const [filters, setFilters] = useState<RequirementFiltersState>({
    keyword: '',
    projectId: null,
    status: null,
    priority: null,
    isConfirmed: null,
  });
  const [showAdvanced, setShowAdvanced] = useState(false);

  const handleChange = (key: keyof RequirementFiltersState, value: string | number | boolean | null) => {
    const newFilters = { ...filters, [key]: value === '' ? null : value };
    setFilters(newFilters);
    onFilterChange(newFilters);
  };

  const handleClear = () => {
    const clearedFilters = {
      keyword: '',
      projectId: null,
      status: null,
      priority: null,
      isConfirmed: null,
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
            placeholder="搜索需求名称或需求号..."
            value={filters.keyword}
            onChange={(e) => handleChange('keyword', e.target.value)}
            className="pl-10"
          />
        </div>
        <Button
          variant="outline"
          onClick={() => setShowAdvanced(!showAdvanced)}
          className={showAdvanced ? 'bg-surface' : ''}
        >
          <Filter className="h-4 w-4 mr-2" />
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
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 p-4 bg-surface rounded-lg border border-border">
          <FormField label="项目">
            <Select
              value={filters.projectId?.toString() || ''}
              onValueChange={(value) => handleChange('projectId', value ? parseInt(value) : null)}
            >
              <SelectTrigger>
                <SelectValue placeholder="全部项目" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">全部项目</SelectItem>
                {projectOptions.map((project) => (
                  <SelectItem key={project.id} value={project.id.toString()}>
                    {project.name}
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
                <SelectItem value="0">待排期</SelectItem>
                <SelectItem value="1">开发中</SelectItem>
                <SelectItem value="2">测试中</SelectItem>
                <SelectItem value="3">已上线</SelectItem>
                <SelectItem value="4">已驳回</SelectItem>
                <SelectItem value="5">已暂停</SelectItem>
              </SelectContent>
            </Select>
          </FormField>

          <FormField label="优先级">
            <Select
              value={filters.priority?.toString() || ''}
              onValueChange={(value) => handleChange('priority', value ? parseInt(value) : null)}
            >
              <SelectTrigger>
                <SelectValue placeholder="全部优先级" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">全部优先级</SelectItem>
                <SelectItem value="0">普通</SelectItem>
                <SelectItem value="1">紧急</SelectItem>
                <SelectItem value="2">非常重要</SelectItem>
              </SelectContent>
            </Select>
          </FormField>

          <FormField label="确认状态">
            <Select
              value={filters.isConfirmed === null ? '' : filters.isConfirmed.toString()}
              onValueChange={(value) =>
                handleChange('isConfirmed', value === '' ? null : value === 'true')
              }
            >
              <SelectTrigger>
                <SelectValue placeholder="全部" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">全部</SelectItem>
                <SelectItem value="true">已确认</SelectItem>
                <SelectItem value="false">未确认</SelectItem>
              </SelectContent>
            </Select>
          </FormField>
        </div>
      )}
    </div>
  );
};