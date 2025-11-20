import { Controller, Post } from '@nestjs/common';
import { GameManagerService } from './game-manager.service';

@Controller('game-manager')
export class GameManagerController {
  constructor(private gameManagerService: GameManagerService) {}

  @Post('create-room')
  async createRoom() {}
}
