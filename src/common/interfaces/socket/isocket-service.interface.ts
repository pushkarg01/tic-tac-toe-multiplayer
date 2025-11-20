import { Socket } from 'socket.io';

export interface ISocketService {
  setServer(server: any);
  registerClient(client: Socket);
  removeClient(clientId: string);
}
