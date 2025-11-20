import { Inject, Injectable } from '@nestjs/common';
import { IGame } from 'src/common/interfaces/game/igame.interface';
import { BaseGameSocketService } from '../game-socket/interfaces';

export const GAME_SOCKET_SERVICE = 'GAME_SOCKET_SERVICE';

@Injectable()
export class TicTacToeService implements IGame {
  socketService: BaseGameSocketService;

  constructor(
    @Inject(GAME_SOCKET_SERVICE) socketService: BaseGameSocketService,
  ) {
    this.socketService = socketService;
  }

  joinRoom(/*clientId: string, roomId: string*/) {
    // Implementation for joining a Tic Tac Toe room
  }

  createRoom(/*clientId: string*/) {
    // Implementation for creating a Tic Tac Toe room
  }
}
