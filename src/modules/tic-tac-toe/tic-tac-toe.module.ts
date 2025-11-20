import { Module } from '@nestjs/common';
import { TicTacToeService, GAME_SOCKET_SERVICE } from './tic-tac-toe.service';
import { InMemoryGameSocketModule } from '../game-socket/in-memory-game-socket/in-memory-game-socket.module';
import { InMemoryGameSocketService } from '../game-socket/in-memory-game-socket/in-memory-game-socket.service';

@Module({
  imports: [InMemoryGameSocketModule],
  providers: [
    {
      provide: GAME_SOCKET_SERVICE,
      useClass: InMemoryGameSocketService,
    },
    TicTacToeService,
  ],
  exports: [TicTacToeService],
})
export class TicTacToeModule {}
