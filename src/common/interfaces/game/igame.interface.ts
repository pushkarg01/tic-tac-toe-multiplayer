import { ISocketService } from '../socket';

export interface IGame {
  socketService: ISocketService;
  joinRoom(clientId: string, roomId: string);
  createRoom(clientId: string);
}
