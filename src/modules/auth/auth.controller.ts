import { Body, Controller, HttpStatus, Post } from '@nestjs/common';
import { UsersService } from '../users/users.service';
import { CreateUserDto } from '../users/dto';
import { ApiOperation, ApiResponse, ApiTags } from '@nestjs/swagger';
import { successMessages } from './constants';

@ApiTags('Authentication')
@Controller('auth')
export class AuthController {
  constructor(private readonly usersService: UsersService) {}

  @Post('register')
  @ApiOperation({ summary: 'Register a new user' })
  @ApiResponse({
    status: HttpStatus.CREATED,
    description: successMessages.USER_REGISTERED,
  })
  @ApiResponse({
    status: HttpStatus.BAD_REQUEST,
    description: 'Invalid input data',
  })
  async register(@Body() createUserDto: CreateUserDto) {
    const user = await this.usersService.create(createUserDto);
    return {
      message: successMessages.USER_REGISTERED,
      data: user,
    };
  }
}
