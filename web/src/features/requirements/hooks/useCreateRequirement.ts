import { useMutation } from '@tanstack/react-query';
import { createRequirementApi } from '@/api/requirements';
import type { CreateRequirementRequest } from '@/types/requirement';

export const useCreateRequirement = () => {
  return useMutation({
    mutationFn: (data: CreateRequirementRequest) => createRequirementApi(data),
  });
};