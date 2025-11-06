/* eslint-disable @typescript-eslint/no-unsafe-assignment */
/* eslint-disable @typescript-eslint/no-unsafe-member-access */
import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { ConfigModule } from '@nestjs/config';
import { UsersModule } from './modules/users/users.module';
import { AuthController } from './modules/auth/auth.controller';
import { AuthService } from './modules/auth/auth.service';
import { AuthModule } from './modules/auth/auth.module';
import { PrismaModule } from './prisma/prisma.module';
import { LogLevel, NodeEnv } from './common/enums';
import { appConfig, dbConfig } from './config';
import { LoggerModule } from 'nestjs-pino';
import { appConstant } from './common/constants';
import { IncomingMessage, ServerResponse } from 'http';

// Extend the ServerResponse type so we can safely access res.req

const isProd = process.env.NODE_ENV === String(NodeEnv.PRODUCTION);

@Module({
  imports: [
    ConfigModule.forRoot({
      isGlobal: appConstant.TRUTHY_FALSY_VALUES.TRUE,
      ignoreEnvFile: isProd,
      envFilePath: isProd ? undefined : ['.env'],
      expandVariables: appConstant.TRUTHY_FALSY_VALUES.TRUE,
      load: [appConfig, dbConfig],
    }),

    LoggerModule.forRoot({
      pinoHttp: {
        level: process.env.LOG_LEVEL || LogLevel.INFO,

        // Pretty logs in non-production, JSON logs in production
        transport: !isProd
          ? {
              target: 'pino-pretty',
              options: {
                colorize: true,
                singleLine: true,
                translateTime: 'SYS:standard',
                ignore: 'pid,hostname',
              },
            }
          : undefined,

        // Custom log messages
        customSuccessMessage: (
          req: IncomingMessage,
          res: ServerResponse,
          responseTime: number,
        ) =>
          `✅ ${req.method} ${req.url} responded ${res.statusCode} in ${responseTime}ms\n`,
        customErrorMessage: (
          req: IncomingMessage,
          res: ServerResponse,
          error: Error,
        ) =>
          `❌ ${req.method} ${req.url} failed ${res.statusCode}: ${error.message}`,

        // Control log level based on response or error
        customLogLevel: (
          req: IncomingMessage,
          res: ServerResponse,
          err?: Error,
        ) => {
          if (res.statusCode >= 400 && res.statusCode < 500)
            return LogLevel.WARN;
          if (res.statusCode >= 500 || err) return LogLevel.ERROR;
          return LogLevel.INFO;
        },

        // Optional extra properties for context
        customProps: (req: IncomingMessage) => ({
          userAgent: req.headers['user-agent'],
        }),

        // Redact sensitive data
        redact: {
          paths: ['req.headers.authorization', 'req.body.password'],
          censor: '**REDACTED**',
        },

        // Customize serialization of req/res
        serializers: {
          req(req: any) {
            return {
              method: req.method,
              url: req.url,
              body: req.raw?.body ?? req.body,
            };
          },
          res(res: any) {
            return { statusCode: res.statusCode };
          },
        },
      },
    }),

    UsersModule,
    AuthModule,
    PrismaModule,
  ],
  controllers: [AppController, AuthController],
  providers: [AppService, AuthService],
})
export class AppModule {}
