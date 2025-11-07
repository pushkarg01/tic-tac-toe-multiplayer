import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { ValidationPipe } from '@nestjs/common';
import { DocumentBuilder, SwaggerModule } from '@nestjs/swagger';
import { ResponseInterceptor } from './common/interceptors/response.interceptor';
import { GlobalExceptionFilter } from './common/filters/http-exception.filter';
import { appConstant } from './common/constants';
import { Logger } from 'nestjs-pino';
import { PinoLoggerInterceptor } from './common/interceptors/pino-logger.interceptor';

async function bootstrap() {
  const app = await NestFactory.create(AppModule, {
    bufferLogs: appConstant.TRUTHY_FALSY_VALUES.TRUE,
  });

  // Register global logger
  const logger = app.get(Logger);

  // Register global interceptor
  app.useGlobalInterceptors(
    new ResponseInterceptor(),
    new PinoLoggerInterceptor(logger),
  );

  // Register global exception filter (for error responses)
  app.useGlobalFilters(new GlobalExceptionFilter(logger));

  // Validation pipe
  app.useGlobalPipes(
    new ValidationPipe({
      whitelist: appConstant.TRUTHY_FALSY_VALUES.TRUE,
      forbidNonWhitelisted: appConstant.TRUTHY_FALSY_VALUES.TRUE,
      transform: appConstant.TRUTHY_FALSY_VALUES.TRUE,
    }),
  );

  // Swagger setup
  const config = new DocumentBuilder()
    .setTitle('Tic Tac Toe Multiplayer API Documentation')
    .setDescription('API documentation for the Tic Tac Toe Multiplayer game.')
    .setVersion('1.0')
    .addBearerAuth()
    .build();

  const document = SwaggerModule.createDocument(app, config);
  SwaggerModule.setup('api', app, document);

  // Start app
  await app.listen(process.env.PORT ?? appConstant.PORT);
}
void bootstrap();
