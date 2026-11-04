import { useEffect, useRef } from 'react';
import { useSignalR } from '../model/SignalRContext';

export const useHubEvent = <T,>(method: string, handler: (payload: T) => void) => {
  const { connection } = useSignalR();
  const handlerRef = useRef(handler);
  handlerRef.current = handler;

  useEffect(() => {
    if (!connection) return;
    const cb = (payload: T) => handlerRef.current(payload);
    connection.on(method, cb);
    return () => { connection.off(method, cb); };
  }, [connection, method]);
};