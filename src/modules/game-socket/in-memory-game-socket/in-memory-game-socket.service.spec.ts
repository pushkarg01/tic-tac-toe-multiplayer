import { Test, TestingModule } from '@nestjs/testing';
import { InMemoryGameSocketService } from './in-memory-game-socket.service';

describe('InMemoryGameSocketService', () => {
  let service: InMemoryGameSocketService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [InMemoryGameSocketService],
    }).compile();

    service = module.get<InMemoryGameSocketService>(InMemoryGameSocketService);
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });
});
