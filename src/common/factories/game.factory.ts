import { TicTacToeService } from 'src/modules/tic-tac-toe/tic-tac-toe.service';
import { Games, SocketServiceType } from '../enums';
import { InMemoryGameSocketService } from 'src/modules/game-socket/in-memory-game-socket/in-memory-game-socket.service';
import { Injectable } from '@nestjs/common';

@Injectable()
export class GameFactory {
  constructor(private inMemorySocketService: InMemoryGameSocketService) {}
  createGame(gameType: Games, socketServiceType: SocketServiceType) {
    if (
      gameType === Games.TIC_TAC_TOE &&
      socketServiceType === SocketServiceType.IN_MEMORY
    ) {
      return new TicTacToeService(this.inMemorySocketService);
    }
  }
}
