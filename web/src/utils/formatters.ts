export function validateUrl(url: string): boolean {
  try {
    new URL(url);
    return true;
  } catch {
    return false;
  }
}

export function formatPrice(price: number | null | undefined): string {
  if (price === null || price === undefined) return '-';
  return `¥${price.toFixed(2)}`;
}

export function formatPercentage(value: number | null | undefined): string {
  if (value === null || value === undefined) return '-';
  return `${value}%`;
}