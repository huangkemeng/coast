import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { TooltipProvider } from '@/components/ui/Tooltip';
import { Layout } from '@/components/layout/Layout';
import { ProtectedRoute, PublicRoute } from '@/features/auth/ProtectedRoute';
import { LoginPage } from '@/features/auth/pages/LoginPage';
import { DashboardPage } from '@/features/dashboard/pages/DashboardPage';
import { RequirementsListPage, RequirementDetailPage, RequirementCreatePage, RequirementEditPage } from '@/features/requirements/pages';
import { ProjectsListPage, ProjectCreatePage, ProjectEditPage } from '@/features/projects/pages';
import { UsersListPage, UserCreatePage, UserEditPage } from '@/features/users/pages';
import { RobotsListPage, RobotCreatePage, RobotEditPage } from '@/features/robots/pages';
import { NotificationsListPage } from '@/features/notifications/pages';

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
              
              <Route path="/requirements" element={<RequirementsListPage />} />
              <Route path="/requirements/new" element={<RequirementCreatePage />} />
              <Route path="/requirements/:id" element={<RequirementDetailPage />} />
              <Route path="/requirements/:id/edit" element={<RequirementEditPage />} />
              
              <Route path="/projects" element={<ProjectsListPage />} />
              <Route path="/projects/new" element={<ProjectCreatePage />} />
              <Route path="/projects/:id/edit" element={<ProjectEditPage />} />
              
              <Route path="/users" element={<UsersListPage />} />
              <Route path="/users/new" element={<UserCreatePage />} />
              <Route path="/users/:id/edit" element={<UserEditPage />} />
              
              <Route path="/robots" element={<RobotsListPage />} />
              <Route path="/robots/new" element={<RobotCreatePage />} />
              <Route path="/robots/:id/edit" element={<RobotEditPage />} />
              
              <Route path="/notifications" element={<NotificationsListPage />} />
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