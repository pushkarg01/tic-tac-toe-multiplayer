import { Controller, Get, Param, HttpStatus } from '@nestjs/common';
import { ApiOperation, ApiResponse, ApiTags } from '@nestjs/swagger';
import { UsersService } from './users.service';
import { successMessages } from './constants/successMessages.constant';

@ApiTags('Users')
@Controller('users')
export class UsersController {
  constructor(private readonly userService: UsersService) {}

  @Get('me/:playerId')
  @ApiOperation({ summary: 'Get user by player ID' })
  @ApiResponse({
    status: HttpStatus.OK,
    description: 'User retrieved successfully',
    example: {
      isSuccess: true,
      message: 'User Found Successfully',
      data: {
        userName: 'john_doe',
      },
    },
  })
  @ApiResponse({
    status: HttpStatus.NOT_FOUND,
    description: 'User not found',
    example: {
      isSuccess: false,
      message: 'User not found',
      data: null,
    },
  })
  async getUserByPlayerId(@Param('playerId') playerId: string) {
    const user = await this.userService.findOne(playerId);
    return {
      message: successMessages.USER_FOUND,
      data: { userName: user.userName },
    };
  }
}
