/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-call */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */
import {
  ExceptionFilter,
  Catch,
  ArgumentsHost,
  HttpException,
  HttpStatus,
} from '@nestjs/common';
import { appConstant, errorMessages } from '../constants';
import { ResponseFormat } from '../interfaces';

@Catch()
export class GlobalExceptionFilter implements ExceptionFilter {
  catch(exception: unknown, host: ArgumentsHost) {
    const ctx = host.switchToHttp();
    const response = ctx.getResponse();

    const status =
      exception instanceof HttpException
        ? exception.getStatus()
        : HttpStatus.INTERNAL_SERVER_ERROR;

    const message =
      exception instanceof HttpException
        ? exception.getResponse()['message'] || exception.message
        : errorMessages.INTERNAL_SERVER_ERROR;

    response.status(status).json({
      isSuccess: appConstant.TRUTHY_FALSY_VALUES.FALSE,
      message: Array.isArray(message) ? message.join(', ') : String(message),
      data: appConstant.TRUTHY_FALSY_VALUES.NULL as null,
    } as ResponseFormat);
  }
}
