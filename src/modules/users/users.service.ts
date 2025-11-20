import { Injectable, NotFoundException } from '@nestjs/common';
import { PrismaService } from '../../prisma/prisma.service';
import { CreateUserDto, UpdateUserDto } from './dto';
import { User } from './interfaces';
import { errorMessages } from './constants/errorMessages.constant';

@Injectable()
export class UsersService {
  constructor(private readonly prisma: PrismaService) {}

  async create(createUserDto: CreateUserDto): Promise<User> {
    return this.prisma.user.create({
      data: createUserDto,
    });
  }

  async findAll(): Promise<User[]> {
    return this.prisma.user.findMany();
  }

  async findOne(playerId: string): Promise<User> {
    const user = await this.prisma.user.findUnique({
      where: { playerId },
    });

    if (!user) {
      throw new NotFoundException(errorMessages.USER_NOT_FOUND);
    }

    return user;
  }

  async update(playerId: string, updateUserDto: UpdateUserDto): Promise<User> {
    const exists = await this.prisma.user.findUnique({
      where: { playerId },
    });

    if (!exists) {
      throw new NotFoundException(errorMessages.USER_NOT_FOUND);
    }

    return this.prisma.user.update({
      where: { playerId },
      data: updateUserDto,
    });
  }

  async remove(playerId: string): Promise<User> {
    const exists = await this.prisma.user.findUnique({
      where: { playerId },
    });

    if (!exists) {
      throw new NotFoundException(errorMessages.USER_NOT_FOUND);
    }

    return this.prisma.user.delete({
      where: { playerId },
    });
  }
}
