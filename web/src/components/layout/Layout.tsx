import React from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from './Sidebar';
import { Header } from './Header';
import { useUIStore } from '@/stores/uiStore';
import { cn } from '@/lib/utils';

export const Layout: React.FC = () => {
  const { toasts, removeToast } = useUIStore();

  return (
    <div className="flex h-screen bg-background">
      <Sidebar />
      <div className="flex flex-col flex-1 overflow-hidden">
        <Header />
        <main className="flex-1 overflow-auto p-6">
          <Outlet />
        </main>
      </div>

      {toasts.length > 0 && (
        <div className="fixed bottom-4 right-4 z-[100] flex flex-col gap-2">
          {toasts.map((toast) => (
            <div
              key={toast.id}
              className={cn(
                'flex items-start gap-4 rounded-lg border p-4 shadow-lg animate-in slide-in-from-right',
                toast.variant === 'success' && 'bg-emerald-900/90 border-emerald-700',
                toast.variant === 'error' && 'bg-red-900/90 border-red-700',
                toast.variant === 'warning' && 'bg-amber-900/90 border-amber-700',
                toast.variant === 'default' && 'bg-surface border-border'
              )}
            >
              <div className="flex-1">
                {toast.title && (
                  <div className="text-sm font-semibold text-text-primary">{toast.title}</div>
                )}
                <div className="text-sm text-text-muted">{toast.message}</div>
              </div>
              <button
                onClick={() => removeToast(toast.id)}
                className="text-text-muted hover:text-text-primary"
              >
                ✕
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};