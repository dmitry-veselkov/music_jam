const fmt = new Intl.DateTimeFormat('ru-RU', { dateStyle: 'medium' });

export const formatDate = (iso: string) => fmt.format(new Date(iso));