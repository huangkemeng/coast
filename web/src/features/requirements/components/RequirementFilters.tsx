import React from 'react';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/Select';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { RequirementStatusName } from '@/types/requirement';

export interface RequirementFiltersState {
  status?: string;
  projectId?: number | null;
  followerId?: number | null;
  planStartDateFrom?: string;
  planStartDateTo?: string;
}

interface RequirementFiltersProps {
  onFilterChange: (filters: RequirementFiltersState) => void;
  projectOptions: { id: number; name: string }[];
  userOptions: { id: number; name: string }[];
}

export const RequirementFilters: React.FC<RequirementFiltersProps> = ({
  onFilterChange,
  projectOptions,
  userOptions,
}) => {
  const [filters, setFilters] = React.useState<RequirementFiltersState>({});

  const handleChange = (key: keyof RequirementFiltersState, value: string | number | null) => {
    const newFilters = { ...filters, [key]: value || undefined };
    setFilters(newFilters);
    onFilterChange(newFilters);
  };

  const handleClear = () => {
    setFilters({});
    onFilterChange({});
  };

  const hasFilters = Object.values(filters).some(v => v !== undefined);

  return (
    <div className="bg-surface rounded-lg border border-border p-4">
      <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
        <div>
          <label className="text-sm text-text-muted mb-1 block">状态</label>
          <Select
            value={filters.status || ''}
            onValueChange={(value) => handleChange('status', value)}
          >
            <SelectTrigger>
              <SelectValue placeholder="全部状态" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="">全部状态</SelectItem>
              {Object.entries(RequirementStatusName).map(([key, name]) => (
                <SelectItem key={key} value={key}>
                  {name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div>
          <label className="text-sm text-text-muted mb-1 block">项目</label>
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
        </div>

        <div>
          <label className="text-sm text-text-muted mb-1 block">跟进人</label>
          <Select
            value={filters.followerId?.toString() || ''}
            onValueChange={(value) => handleChange('followerId', value ? parseInt(value) : null)}
          >
            <SelectTrigger>
              <SelectValue placeholder="全部跟进人" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="">全部跟进人</SelectItem>
              {userOptions.map((user) => (
                <SelectItem key={user.id} value={user.id.toString()}>
                  {user.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div>
          <label className="text-sm text-text-muted mb-1 block">计划开始日期起</label>
          <Input
            type="date"
            value={filters.planStartDateFrom || ''}
            onChange={(e) => handleChange('planStartDateFrom', e.target.value)}
          />
        </div>

        <div>
          <label className="text-sm text-text-muted mb-1 block">计划开始日期止</label>
          <Input
            type="date"
            value={filters.planStartDateTo || ''}
            onChange={(e) => handleChange('planStartDateTo', e.target.value)}
          />
        </div>
      </div>

      {hasFilters && (
        <div className="mt-4 flex justify-end">
          <Button variant="ghost" size="sm" onClick={handleClear}>
            清除筛选
          </Button>
        </div>
      )}
    </div>
  );
};