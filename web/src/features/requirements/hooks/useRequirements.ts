import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getRequirementsApi, GetRequirementsParams } from '@/api/requirements';
import type { PageResponse, RequirementFilters } from '@/types/api';

export const useRequirements = (params: GetRequirementsParams) => {
  return useQuery({
    queryKey: ['requirements', params],
    queryFn: () => getRequirementsApi(params),
  });
};

export const useRequirement = (id: number) => {
  return useQuery({
    queryKey: ['requirement', id],
    queryFn: () => import('@/api/requirements').then(api => api.getRequirementByIdApi(id)),
    enabled: !!id,
  });
};

export const useRequirementTimeline = (id: number) => {
  return useQuery({
    queryKey: ['requirement-timeline', id],
    queryFn: () => import('@/api/requirements').then(api => api.getRequirementTimelineApi(id)),
    enabled: !!id,
  });
};