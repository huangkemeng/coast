import { useAuthStore } from './authStore';
import type { Requirement } from '@/types/requirement';

export const usePermission = () => {
  const user = useAuthStore((state) => state.user);
  const isAdmin = user?.role === 1;

  const canCreateRequirement = () => isAdmin;

  const canEditRequirement = (requirement: Requirement) => {
    if (isAdmin) return true;
    return requirement.followerId === user?.id;
  };

  const canDeleteRequirement = () => isAdmin;

  const canChangeStatus = (requirement: Requirement) => {
    if (isAdmin) return true;
    return requirement.followerId === user?.id;
  };

  const canManageProjects = () => isAdmin;

  const canManageUsers = () => isAdmin;

  const canManageRobots = () => isAdmin;

  const canViewPrice = () => isAdmin;

  const canViewNotifications = () => isAdmin;

  return {
    canCreateRequirement,
    canEditRequirement,
    canDeleteRequirement,
    canChangeStatus,
    canManageProjects,
    canManageUsers,
    canManageRobots,
    canViewPrice,
    canViewNotifications,
    isAdmin,
  };
};