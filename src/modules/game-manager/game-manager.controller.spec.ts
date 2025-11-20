import { Test, TestingModule } from '@nestjs/testing';
import { GameManagerController } from './game-manager.controller';

describe('GameManagerController', () => {
  let controller: GameManagerController;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      controllers: [GameManagerController],
    }).compile();

    controller = module.get<GameManagerController>(GameManagerController);
  });

  it('should be defined', () => {
    expect(controller).toBeDefined();
  });
});
