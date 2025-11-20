import { Injectable } from '@nestjs/common';
import { GameFactory } from 'src/common/factories/game.factory';

@Injectable()
export class GameManagerService {
  constructor(private gameFactory: GameFactory) {}

  createRoom() {}
}
