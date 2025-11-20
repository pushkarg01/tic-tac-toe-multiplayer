import { Module } from '@nestjs/common';
import { GameManagerController } from './game-manager.controller';
import { GameManagerService } from './game-manager.service';
import { GameFactory } from 'src/common/factories/game.factory';
import { InMemoryGameSocketModule } from '../game-socket/in-memory-game-socket/in-memory-game-socket.module';

@Module({
  imports: [InMemoryGameSocketModule],
  controllers: [GameManagerController],
  providers: [GameFactory, GameManagerService],
})
export class GameManagerModule {}
