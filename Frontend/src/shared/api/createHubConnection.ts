import * as signalR from '@microsoft/signalr';
import { HUB_URL } from "@/shared/config.ts";

export const createHubConnection = () => {
  return new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL)
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(signalR.LogLevel.Warning)
    .build();
};