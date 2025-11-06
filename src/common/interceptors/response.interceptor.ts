import {
  CallHandler,
  ExecutionContext,
  Injectable,
  NestInterceptor,
} from '@nestjs/common';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ResponseFormat } from '../interfaces';
import { appConstant, successMessages } from '../constants';

@Injectable()
export class ResponseInterceptor implements NestInterceptor<ResponseFormat> {
  intercept(
    context: ExecutionContext,
    next: CallHandler,
  ): Observable<ResponseFormat> {
    return next.handle().pipe(
      map((response: { message: string; data: unknown }) => {
        // If the controller explicitly returns { message, data }
        if (response && response.message && 'data' in response) {
          return {
            isSuccess: appConstant.TRUTHY_FALSY_VALUES.TRUE,
            message: response.message,
            data: response.data,
          } as ResponseFormat;
        }

        // Otherwise wrap any other response
        return {
          isSuccess: appConstant.TRUTHY_FALSY_VALUES.TRUE,
          message: successMessages.SUCCESS_RESPONSE,
          data: response,
        } as ResponseFormat;
      }),
    );
  }
}
