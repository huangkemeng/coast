import React, { Suspense, lazy } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { TooltipProvider } from '@/components/ui/Tooltip';
import { Layout } from '@/components/layout/Layout';
import { ProtectedRoute, PublicRoute } from '@/features/auth/ProtectedRoute';
import { LoginPage } from '@/features/auth/pages/LoginPage';
import { DashboardPage } from '@/features/dashboard/pages/DashboardPage';
import { LoadingOverlay } from '@/components/common/LoadingOverlay';

const RequirementsListPage = lazy(() => import('@/features/requirements/pages/RequirementsListPage').then(m => ({ default: m.RequirementsListPage })));
const RequirementDetailPage = lazy(() => import('@/features/requirements/pages/RequirementDetailPage').then(m => ({ default: m.RequirementDetailPage })));
const RequirementCreatePage = lazy(() => import('@/features/requirements/pages/RequirementCreatePage').then(m => ({ default: m.RequirementCreatePage })));
const RequirementEditPage = lazy(() => import('@/features/requirements/pages/RequirementEditPage').then(m => ({ default: m.RequirementEditPage })));

const ProjectsListPage = lazy(() => import('@/features/projects/pages/ProjectsListPage').then(m => ({ default: m.ProjectsListPage })));
const ProjectCreatePage = lazy(() => import('@/features/projects/pages/ProjectCreatePage').then(m => ({ default: m.ProjectCreatePage })));
const ProjectEditPage = lazy(() => import('@/features/projects/pages/ProjectEditPage').then(m => ({ default: m.ProjectEditPage })));

const UsersListPage = lazy(() => import('@/features/users/pages/UsersListPage').then(m => ({ default: m.UsersListPage })));
const UserCreatePage = lazy(() => import('@/features/users/pages/UserCreatePage').then(m => ({ default: m.UserCreatePage })));
const UserEditPage = lazy(() => import('@/features/users/pages/UserEditPage').then(m => ({ default: m.UserEditPage })));

const RobotsListPage = lazy(() => import('@/features/robots/pages/RobotsListPage').then(m => ({ default: m.RobotsListPage })));
const RobotCreatePage = lazy(() => import('@/features/robots/pages/RobotCreatePage').then(m => ({ default: m.RobotCreatePage })));
const RobotEditPage = lazy(() => import('@/features/robots/pages/RobotEditPage').then(m => ({ default: m.RobotEditPage })));

const NotificationsListPage = lazy(() => import('@/features/notifications/pages/NotificationsListPage').then(m => ({ default: m.NotificationsListPage })));

const PageLoader = () => (
  <div className="flex items-center justify-center min-h-[400px]">
    <LoadingOverlay text="加载中..." />
  </div>
);

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5,
      retry: 1,
    },
  },
});

const App: React.FC = () => {
  return (
    <QueryClientProvider client={queryClient}>
      <TooltipProvider>
        <BrowserRouter>
          <Routes>
            <Route
              path="/login"
              element={
                <PublicRoute>
                  <LoginPage />
                </PublicRoute>
              }
            />
            
            <Route
              element={
                <ProtectedRoute>
                  <Layout />
                </ProtectedRoute>
              }
            >
              <Route path="/dashboard" element={<DashboardPage />} />
              
              <Route path="/requirements" element={<Suspense fallback={<PageLoader />}><RequirementsListPage /></Suspense>} />
              <Route path="/requirements/new" element={<Suspense fallback={<PageLoader />}><RequirementCreatePage /></Suspense>} />
              <Route path="/requirements/:id" element={<Suspense fallback={<PageLoader />}><RequirementDetailPage /></Suspense>} />
              <Route path="/requirements/:id/edit" element={<Suspense fallback={<PageLoader />}><RequirementEditPage /></Suspense>} />
              
              <Route path="/projects" element={<Suspense fallback={<PageLoader />}><ProjectsListPage /></Suspense>} />
              <Route path="/projects/new" element={<Suspense fallback={<PageLoader />}><ProjectCreatePage /></Suspense>} />
              <Route path="/projects/:id/edit" element={<Suspense fallback={<PageLoader />}><ProjectEditPage /></Suspense>} />
              
              <Route path="/users" element={<Suspense fallback={<PageLoader />}><UsersListPage /></Suspense>} />
              <Route path="/users/new" element={<Suspense fallback={<PageLoader />}><UserCreatePage /></Suspense>} />
              <Route path="/users/:id/edit" element={<Suspense fallback={<PageLoader />}><UserEditPage /></Suspense>} />
              
              <Route path="/robots" element={<Suspense fallback={<PageLoader />}><RobotsListPage /></Suspense>} />
              <Route path="/robots/new" element={<Suspense fallback={<PageLoader />}><RobotCreatePage /></Suspense>} />
              <Route path="/robots/:id/edit" element={<Suspense fallback={<PageLoader />}><RobotEditPage /></Suspense>} />
              
              <Route path="/notifications" element={<Suspense fallback={<PageLoader />}><NotificationsListPage /></Suspense>} />
            </Route>
            
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
          </Routes>
        </BrowserRouter>
      </TooltipProvider>
    </QueryClientProvider>
  );
};

export default App;