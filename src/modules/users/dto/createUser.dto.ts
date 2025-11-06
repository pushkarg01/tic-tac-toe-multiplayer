import { IsString, MinLength } from 'class-validator';
import { ApiProperty } from '@nestjs/swagger';
import { User } from '../interfaces';

export class CreateUserDto implements Partial<User> {
  @ApiProperty({
    description: 'The username of the user',
    minLength: 3,
  })
  @IsString()
  @MinLength(3)
  userName: string;
}
