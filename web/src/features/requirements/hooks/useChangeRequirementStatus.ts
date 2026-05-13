import { useMutation } from '@tanstack/react-query';
import { changeRequirementStatusApi } from '@/api/requirements';
import type { RequirementStatus, ChangeRequirementStatusRequest } from '@/types/requirement';

export const useChangeRequirementStatus = () => {
  return useMutation({
    mutationFn: ({ id, status, remark }: { id: number; status: RequirementStatus; remark?: string }) =>
      changeRequirementStatusApi(id, { status, remark } as ChangeRequirementStatusRequest),
  });
};