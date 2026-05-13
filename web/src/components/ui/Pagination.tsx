import * as React from 'react';
import { cn } from '@/lib/utils';

export interface PaginationProps {
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  className?: string;
}

const Pagination: React.FC<PaginationProps> = ({
  pageIndex,
  pageSize,
  totalCount,
  totalPages,
  onPageChange,
  className,
}) => {
  const [windowSize, setWindowSize] = React.useState(pageSize >= 20 ? 7 : 5);
  
  React.useEffect(() => {
    const updateWindowSize = () => {
      setWindowSize(window.innerWidth < 640 ? 3 : 5);
    };
    updateWindowSize();
    window.addEventListener('resize', updateWindowSize);
    return () => window.removeEventListener('resize', updateWindowSize);
  }, []);

  const getPageNumbers = () => {
    const pages: (number | string)[] = [];
    const half = Math.floor(windowSize / 2);
    let start = Math.max(1, pageIndex - half);
    let end = Math.min(totalPages, pageIndex + half);

    if (end - start + 1 < windowSize) {
      if (start === 1) {
        end = Math.min(totalPages, start + windowSize - 1);
      } else {
        start = Math.max(1, end - windowSize + 1);
      }
    }

    if (start > 1) {
      pages.push(1);
      if (start > 2) pages.push('...');
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    if (end < totalPages) {
      if (end < totalPages - 1) pages.push('...');
      pages.push(totalPages);
    }

    return pages;
  };

  if (totalPages <= 1) return null;

  return (
    <div className={cn('flex items-center justify-center space-x-2', className)}>
      <button
        onClick={() => onPageChange(pageIndex - 1)}
        disabled={pageIndex <= 1}
        className="h-8 px-3 rounded-md border border-border bg-surface text-text-primary disabled:opacity-50 disabled:cursor-not-allowed hover:bg-slate-700 transition-colors"
      >
        上一页
      </button>
      
      <div className="flex items-center space-x-1">
        {getPageNumbers().map((page, index) =>
          typeof page === 'number' ? (
            <button
              key={page}
              onClick={() => onPageChange(page)}
              className={cn(
                'h-8 min-w-[32px] px-2 rounded-md border border-border text-sm transition-colors',
                page === pageIndex
                  ? 'bg-primary text-white border-primary'
                  : 'bg-surface text-text-primary hover:bg-slate-700'
              )}
            >
              {page}
            </button>
          ) : (
            <span key={`ellipsis-${index}`} className="px-2 text-text-muted">
              {page}
            </span>
          )
        )}
      </div>
      
      <button
        onClick={() => onPageChange(pageIndex + 1)}
        disabled={pageIndex >= totalPages}
        className="h-8 px-3 rounded-md border border-border bg-surface text-text-primary disabled:opacity-50 disabled:cursor-not-allowed hover:bg-slate-700 transition-colors"
      >
        下一页
      </button>
      
      <span className="text-sm text-text-muted ml-2">
        共 {totalCount} 条
      </span>
    </div>
  );
};

export { Pagination };