import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getRobotsApi, getRobotByIdApi, createRobotApi, updateRobotApi, deleteRobotApi, testRobotApi, GetRobotsParams } from '@/api/robots';

export const useRobots = (params: GetRobotsParams) => {
  return useQuery({
    queryKey: ['robots', params],
    queryFn: () => getRobotsApi(params),
  });
};

export const useRobot = (id: number) => {
  return useQuery({
    queryKey: ['robot', id],
    queryFn: () => getRobotByIdApi(id),
    enabled: !!id,
  });
};

export const useCreateRobot = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createRobotApi,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['robots'] });
    },
  });
};

export const useUpdateRobot = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: Parameters<typeof updateRobotApi>[1] }) =>
      updateRobotApi(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['robot', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['robots'] });
    },
  });
};

export const useDeleteRobot = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteRobotApi,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['robots'] });
    },
  });
};

export const useTestRobot = () => {
  return useMutation({
    mutationFn: testRobotApi,
  });
};