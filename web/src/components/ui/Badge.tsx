/* eslint-disable react-refresh/only-export-components */
import * as React from 'react';
import { cva, type VariantProps } from 'class-variance-authority';
import { cn } from '@/lib/utils';

export const badgeVariants = cva(
  'inline-flex items-center rounded-md border px-2.5 py-0.5 text-xs font-semibold transition-colors focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2',
  {
    variants: {
      variant: {
        default: 'border-transparent bg-primary text-white',
        secondary: 'border-transparent bg-secondary text-white',
        success: 'border-transparent bg-success text-white',
        warning: 'border-transparent bg-warning text-black',
        error: 'border-transparent bg-error text-white',
        outline: 'text-text-primary border-border',
        pending: 'border-transparent bg-slate-500 text-white',
        dev: 'border-transparent bg-blue-500 text-white',
        test: 'border-transparent bg-amber-500 text-black',
        launched: 'border-transparent bg-emerald-500 text-white',
        rejected: 'border-transparent bg-red-500 text-white',
        paused: 'border-transparent bg-slate-400 text-white',
      },
    },
    defaultVariants: {
      variant: 'default',
    },
  }
);

export interface BadgeProps
  extends React.HTMLAttributes<HTMLDivElement>,
    VariantProps<typeof badgeVariants> {}

function Badge({ className, variant, ...props }: BadgeProps) {
  return (
    <div className={cn(badgeVariants({ variant }), className)} {...props} />
  );
}

export { Badge };