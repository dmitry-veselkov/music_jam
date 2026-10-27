const fmt = new Intl.DateTimeFormat('ru-RU', { dateStyle: 'medium' });

export const formatDate = (iso: string) => fmt.format(new Date(iso));
export const formatTime = (iso: string) =>
  new Date(iso).toLocaleTimeString("ru-RU", { hour: "2-digit", minute: "2-digit" });
