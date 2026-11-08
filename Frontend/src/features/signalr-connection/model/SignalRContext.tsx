import { createContext, type ReactNode, useContext, useEffect, useRef, useState } from 'react';
import type { HubConnection } from '@microsoft/signalr';
import { createHubConnection } from "@/shared/api/createHubConnection.ts";

type SignalRState = 'connecting' | 'connected' | 'disconnected' | 'reconnecting';

type SignalRCtx = {
  connection: HubConnection | null;
  state: SignalRState;
}

const SignalRContext = createContext<SignalRCtx>({ connection: null, state: 'disconnected' });

export const SignalRProvider = ({ children }: { children: ReactNode }) => {
  const [state, setState] = useState<SignalRState>('disconnected');
  const connRef = useRef<HubConnection | null>(null);

  useEffect(() => {
    const conn = createHubConnection();
    connRef.current = conn;

    conn.onreconnecting(() => setState('reconnecting'));
    conn.onreconnected(() => setState('connected'));
    conn.onclose(() => setState('disconnected'));

    conn.start().then(() => setState('connected')).catch(() => setState('disconnected'));

    return () => { conn.stop(); };
  }, []);

  return (
    <SignalRContext.Provider value={{ connection: connRef.current, state }}>
      {children}
    </SignalRContext.Provider>
  );
}

export const useSignalR = () => useContext(SignalRContext);