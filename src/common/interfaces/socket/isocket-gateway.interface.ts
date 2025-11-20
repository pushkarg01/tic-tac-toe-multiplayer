import { Socket } from 'socket.io';

export interface ISocketGateway {
  handleConnection(client: Socket): void;
  handleDisconnect(client: Socket): void;
}
