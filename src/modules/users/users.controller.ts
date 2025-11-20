import { Controller, Get, Param } from '@nestjs/common';
import { UsersService } from './users.service';
import { successMessages } from './constants/successMessages.constant';

@Controller('users')
export class UsersController {
  constructor(private readonly userService: UsersService) {}

  @Get('me/:playerId')
  async getUserById(@Param('playerId') playerId: string) {
    const user = await this.userService.findOne(playerId);
    return {
      message: successMessages.USER_FOUND,
      data: { userName: user.userName },
    };
  }
}
