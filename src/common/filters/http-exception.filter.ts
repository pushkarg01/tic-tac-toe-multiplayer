/* eslint-disable @typescript-eslint/no-unsafe-member-access */

/* eslint-disable @typescript-eslint/no-unsafe-assignment */
import {
  ExceptionFilter,
  Catch,
  ArgumentsHost,
  HttpException,
  HttpStatus,
  Injectable,
} from '@nestjs/common';
import { Logger } from 'nestjs-pino';
import { appConstant, errorMessages } from '../constants';
import { ResponseFormat } from '../interfaces';
import { Request, Response } from 'express';

@Catch()
@Injectable()
export class GlobalExceptionFilter implements ExceptionFilter {
  constructor(private readonly logger: Logger) {}

  catch(exception: unknown, host: ArgumentsHost) {
    const ctx = host.switchToHttp();
    const response = ctx.getResponse<Response>();
    const request = ctx.getRequest<Request>();

    const status =
      exception instanceof HttpException
        ? exception.getStatus()
        : HttpStatus.INTERNAL_SERVER_ERROR;

    const message =
      exception instanceof HttpException
        ? (exception.getResponse() as any)?.message || exception.message
        : errorMessages.INTERNAL_SERVER_ERROR;

    const errorResponse: ResponseFormat = {
      isSuccess: appConstant.TRUTHY_FALSY_VALUES.FALSE,
      message: Array.isArray(message) ? message.join(', ') : String(message),
      data: appConstant.TRUTHY_FALSY_VALUES.NULL as null,
    };

    // 🧾 Log the error in structured format using pino
    this.logger.error({
      message: '❌ Error Response',
      method: request.method,
      url: request.url,
      statusCode: status,
      errorResponse,
    });

    // Send formatted response
    response.status(status).json(errorResponse);
  }
}
