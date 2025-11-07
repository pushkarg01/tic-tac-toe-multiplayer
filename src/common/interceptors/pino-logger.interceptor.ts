/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */
import {
  Injectable,
  NestInterceptor,
  ExecutionContext,
  CallHandler,
} from '@nestjs/common';
import { Observable, tap } from 'rxjs';
import { Logger } from 'nestjs-pino';
import { Response } from 'express';

@Injectable()
export class PinoLoggerInterceptor implements NestInterceptor {
  constructor(private readonly logger: Logger) {}

  intercept(context: ExecutionContext, next: CallHandler): Observable<any> {
    const now = Date.now();
    const httpContext = context.switchToHttp();
    const request = httpContext.getRequest();
    const { method, url, body, headers, params, query } = request;

    // 🟢 Remove sensitive data from request body
    const safeBody = { ...body };
    if (safeBody.password) safeBody.password = '[REDACTED]';
    if (safeBody.confirmPassword) safeBody.confirmPassword = '[REDACTED]';
    if (safeBody.token) safeBody.token = '[REDACTED]';

    this.logger.log({
      message: '⏩ Incoming Request',
      method,
      url,
      params,
      query,
      headers: {
        'user-agent': headers['user-agent'],
        'content-type': headers['content-type'],
      },
      body: safeBody,
    });

    // ✅ Must return observable pipeline
    return next.handle().pipe(
      tap((responseBody) => {
        const response = httpContext.getResponse<Response>();
        const statusCode = response.statusCode;
        const delay = Date.now() - now;

        // 🟡 Optionally redact large/sensitive response data
        const safeResponse =
          typeof responseBody === 'object'
            ? JSON.parse(
                JSON.stringify(responseBody, (_: string, value: unknown) => {
                  if (typeof value === 'string' && value.length > 500)
                    return '[TRUNCATED]';
                  return value;
                }),
              )
            : responseBody;

        this.logger.log({
          message: '⏪ Outgoing Response',
          method,
          url,
          statusCode,
          responseTime: `${delay}ms`,
          response: safeResponse,
        });
      }),
    );
  }
}
