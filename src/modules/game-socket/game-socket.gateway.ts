import {
  OnGatewayConnection,
  OnGatewayDisconnect,
  WebSocketGateway,
  WebSocketServer,
} from '@nestjs/websockets';
import { Server, Socket } from 'socket.io';
import { ISocketGateway } from 'src/common/interfaces';
import { BaseGameSocketService } from './interfaces';

@WebSocketGateway({ cors: { origin: '*' } })
export class GameSocketGateway
  implements ISocketGateway, OnGatewayConnection, OnGatewayDisconnect
{
  @WebSocketServer()
  private server: Server;

  private socketService: BaseGameSocketService;

  constructor(socketService: BaseGameSocketService) {
    this.socketService = socketService;
  }

  handleConnection(client: Socket): void {
    this.socketService.setServer(this.server);
    this.socketService.registerClient(client);
  }
  handleDisconnect(client: Socket): void {
    this.socketService.removeClient(client.id);
  }
}
