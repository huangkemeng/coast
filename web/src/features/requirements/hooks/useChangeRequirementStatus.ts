import { useMutation, useQueryClient } from '@tanstack/react-query';
import { changeRequirementStatusApi } from '@/api/requirements';
import type { ChangeStatusRequest } from '@/types/requirement';

export const useChangeRequirementStatus = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: ChangeStatusRequest }) =>
      changeRequirementStatusApi(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['requirement', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['requirement-timeline', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['requirements'] });
    },
  });
};