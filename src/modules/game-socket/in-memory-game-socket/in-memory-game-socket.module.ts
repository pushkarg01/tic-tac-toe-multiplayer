import { Module } from '@nestjs/common';
import { InMemoryGameSocketService } from './in-memory-game-socket.service';

@Module({
  providers: [InMemoryGameSocketService],
  exports: [InMemoryGameSocketService],
})
export class InMemoryGameSocketModule {}
