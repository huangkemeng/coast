import { create } from 'zustand';

interface Toast {
  id: string;
  title?: string;
  message?: string;
  variant: 'default' | 'success' | 'error' | 'warning';
}

interface UIState {
  toasts: Toast[];
  sidebarCollapsed: boolean;
  addToast: (toast: Omit<Toast, 'id'>) => void;
  removeToast: (id: string) => void;
  toggleSidebar: () => void;
  setSidebarCollapsed: (collapsed: boolean) => void;
}

export const useUIStore = create<UIState>((set) => ({
  toasts: [],
  sidebarCollapsed: false,

  addToast: (toast) => {
    const id = Math.random().toString(36).substring(7);
    set((state) => ({
      toasts: [...state.toasts, { ...toast, id }],
    }));
    setTimeout(() => {
      set((state) => ({
        toasts: state.toasts.filter((t) => t.id !== id),
      }));
    }, 5000);
  },

  removeToast: (id) => {
    set((state) => ({
      toasts: state.toasts.filter((t) => t.id !== id),
    }));
  },

  toggleSidebar: () => {
    set((state) => ({ sidebarCollapsed: !state.sidebarCollapsed }));
  },

  setSidebarCollapsed: (collapsed) => {
    set({ sidebarCollapsed: collapsed });
  },
}));

export const toast = {
  success: (message: string, title?: string) =>
    useUIStore.getState().addToast({ message, title, variant: 'success' }),
  error: (message: string, title?: string) =>
    useUIStore.getState().addToast({ message, title, variant: 'error' }),
  warning: (message: string, title?: string) =>
    useUIStore.getState().addToast({ message, title, variant: 'warning' }),
  info: (message: string, title?: string) =>
    useUIStore.getState().addToast({ message, title, variant: 'default' }),
};