import { cn } from '@/lib/utils';

interface LoadingSpinnerProps {
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

export const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({ size = 'md', className }) => {
  const sizeClasses = {
    sm: 'h-4 w-4',
    md: 'h-6 w-6',
    lg: 'h-8 w-8',
  };

  return (
    <div className={cn('flex items-center justify-center', className)}>
      <div
        className={cn(
          'animate-spin rounded-full border-2 border-primary border-t-transparent',
          sizeClasses[size]
        )}
      />
    </div>
  );
};

interface LoadingOverlayProps {
  fullScreen?: boolean;
  text?: string;
}

export const LoadingOverlay: React.FC<LoadingOverlayProps> = ({ fullScreen = false, text }) => {
  return (
    <div
      className={cn(
        'flex flex-col items-center justify-center gap-4 bg-background/80',
        fullScreen ? 'fixed inset-0 z-50' : 'p-8'
      )}
    >
      <div className="h-10 w-10 animate-spin rounded-full border-4 border-primary border-t-transparent" />
      {text && <p className="text-text-muted">{text}</p>}
    </div>
  );
};