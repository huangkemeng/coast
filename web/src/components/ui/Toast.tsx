/* eslint-disable react-refresh/only-export-components */
import * as React from 'react';
import { X } from 'lucide-react';
import { cn } from '@/lib/utils';

interface ToastProps {
  id: string;
  title?: string;
  description?: string;
  variant?: 'default' | 'success' | 'error' | 'warning';
  onClose: (id: string) => void;
}

const Toast: React.FC<ToastProps> = ({ id, title, description, variant = 'default', onClose }) => {
  const variantStyles = {
    default: 'bg-surface border-border',
    success: 'bg-emerald-900/90 border-emerald-700',
    error: 'bg-red-900/90 border-red-700',
    warning: 'bg-amber-900/90 border-amber-700',
  };

  return (
    <div
      className={cn(
        'pointer-events-auto flex w-full max-w-md items-start gap-4 rounded-lg border p-4 shadow-lg transition-all animate-in slide-in-from-right',
        variantStyles[variant]
      )}
    >
      <div className="flex flex-1 flex-col gap-1">
        {title && <div className="text-sm font-semibold text-text-primary">{title}</div>}
        {description && <div className="text-sm text-text-muted">{description}</div>}
      </div>
      <button
        onClick={() => onClose(id)}
        className="h-5 w-5 rounded-sm opacity-70 hover:opacity-100 transition-opacity"
      >
        <X className="h-4 w-4" />
      </button>
    </div>
  );
};

interface ToastContainerProps {
  toasts: ToastProps[];
  onClose: (id: string) => void;
}

const ToastContainer: React.FC<ToastContainerProps> = ({ toasts, onClose }) => {
  return (
    <div className="fixed bottom-4 right-4 z-[100] flex flex-col gap-2">
      {toasts.map((toast) => (
        <Toast key={toast.id} {...toast} onClose={onClose} />
      ))}
    </div>
  );
};

interface ToastHook {
  toasts: ToastProps[];
  addToast: (toast: Omit<ToastProps, 'id' | 'onClose'>) => void;
  removeToast: (id: string) => void;
}

function useToast(): ToastHook {
  const [toasts, setToasts] = React.useState<ToastProps[]>([]);

  const removeToast = React.useCallback((id: string) => {
    setToasts((prev) => prev.filter((t) => t.id !== id));
  }, []);

  const addToast = React.useCallback((toast: Omit<ToastProps, 'id' | 'onClose'>) => {
    const id = Math.random().toString(36).substring(7);
    setToasts((prev) => [...prev, { ...toast, id, onClose: removeToast }]);
  }, [removeToast]);

  return { toasts, addToast, removeToast };
}

export { Toast, ToastContainer, useToast };
export type { ToastProps, ToastContainerProps, ToastHook };