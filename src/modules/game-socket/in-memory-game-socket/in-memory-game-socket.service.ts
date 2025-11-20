import { Injectable } from '@nestjs/common';
import { Server, Socket } from 'socket.io';
import { BaseGameSocketService } from '../interfaces';

@Injectable()
export class InMemoryGameSocketService implements BaseGameSocketService {
  private server: Server;
  private clients: Map<string, any> = new Map();

  setServer(server: Server) {
    this.server = server;
  }

  registerClient(client: Socket) {
    this.clients.set(client.id, client);
  }

  removeClient(clientId: string) {
    this.clients.delete(clientId);
  }
}
