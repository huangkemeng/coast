import { useMutation, useQueryClient } from '@tanstack/react-query';
import { updateRequirementApi } from '@/api/requirements';
import type { UpdateRequirementRequest } from '@/types/requirement';

export const useUpdateRequirement = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateRequirementRequest }) =>
      updateRequirementApi(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['requirement', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['requirements'] });
    },
  });
};