import { useMutation, useQueryClient } from '@tanstack/react-query';
import { deleteRequirementApi } from '@/api/requirements';

export const useDeleteRequirement = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => deleteRequirementApi(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['requirements'] });
    },
  });
};