export const paginate = <T,>(request: Request, list: T[], defaultTake = 20): T[] => {
  const url = new URL(request.url);
  const skip = Math.max(0, Number(url.searchParams.get('skip')) || 0);
  const take = Math.min(100, Math.max(1, Number(url.searchParams.get('take')) || defaultTake));

  return list.slice(skip, skip + take);
};