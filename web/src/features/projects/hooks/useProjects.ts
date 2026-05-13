import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getProjectsApi, getProjectByIdApi, createProjectApi, updateProjectApi, deleteProjectApi, GetProjectsParams } from '@/api/projects';

export const useProjects = (params: GetProjectsParams) => {
  return useQuery({
    queryKey: ['projects', params],
    queryFn: () => getProjectsApi(params),
  });
};

export const useProject = (id: number) => {
  return useQuery({
    queryKey: ['project', id],
    queryFn: () => getProjectByIdApi(id),
    enabled: !!id,
  });
};

export const useCreateProject = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createProjectApi,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    },
  });
};

export const useUpdateProject = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: Parameters<typeof updateProjectApi>[1] }) =>
      updateProjectApi(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['project', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    },
  });
};

export const useDeleteProject = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteProjectApi,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    },
  });
};