import React from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import { cn } from '@/lib/utils';
import { useUIStore } from '@/stores/uiStore';
import { usePermission } from '@/stores/permissionStore';
import {
  LayoutDashboard,
  ListTodo,
  FolderKanban,
  Users,
  Bot,
  Bell,
  ChevronLeft,
  ChevronRight,
} from 'lucide-react';

interface NavItem {
  label: string;
  path: string;
  icon: React.ElementType;
  permission?: () => boolean;
}

const navItems: NavItem[] = [
  { label: '仪表盘', path: '/dashboard', icon: LayoutDashboard },
  { label: '需求管理', path: '/requirements', icon: ListTodo },
  { label: '项目管理', path: '/projects', icon: FolderKanban, permission: () => true },
  { label: '用户管理', path: '/users', icon: Users, permission: () => true },
  { label: '机器人配置', path: '/robots', icon: Bot, permission: () => true },
  { label: '通知日志', path: '/notifications', icon: Bell, permission: () => true },
];

export const Sidebar: React.FC = () => {
  const location = useLocation();
  const { sidebarCollapsed, toggleSidebar } = useUIStore();
  const { isAdmin } = usePermission();

  const filteredNavItems = navItems.filter((item) => {
    if (item.path === '/projects' || item.path === '/users' || item.path === '/robots' || item.path === '/notifications') {
      return isAdmin;
    }
    return true;
  });

  return (
    <aside
      className={cn(
        'flex flex-col bg-surface border-r border-border transition-all duration-300',
        sidebarCollapsed ? 'w-16' : 'w-56'
      )}
    >
      <div className="flex items-center h-16 px-4 border-b border-border">
        {!sidebarCollapsed && (
          <h1 className="text-lg font-semibold text-text-primary truncate">需求跟踪系统</h1>
        )}
      </div>

      <nav className="flex-1 p-2 space-y-1">
        {filteredNavItems.map((item) => {
          const isActive = location.pathname.startsWith(item.path);
          const Icon = item.icon;

          return (
            <NavLink
              key={item.path}
              to={item.path}
              className={cn(
                'flex items-center gap-3 px-3 py-2 rounded-md transition-colors',
                isActive
                  ? 'bg-primary text-white'
                  : 'text-text-muted hover:bg-slate-700 hover:text-text-primary',
                sidebarCollapsed && 'justify-center'
              )}
            >
              <Icon className="h-5 w-5 flex-shrink-0" />
              {!sidebarCollapsed && <span className="truncate">{item.label}</span>}
            </NavLink>
          );
        })}
      </nav>

      <div className="p-2 border-t border-border">
        <button
          onClick={toggleSidebar}
          className="flex items-center justify-center w-full p-2 rounded-md text-text-muted hover:bg-slate-700 hover:text-text-primary transition-colors"
        >
          {sidebarCollapsed ? (
            <ChevronRight className="h-5 w-5" />
          ) : (
            <ChevronLeft className="h-5 w-5" />
          )}
        </button>
      </div>
    </aside>
  );
};