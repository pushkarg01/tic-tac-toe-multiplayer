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
                translateTime: 'SYS:standard',
                singleLine: false,
                messageFormat: '{msg} [{method} {url}]', // optional formatting
                ignore: 'pid,hostname,req,res', // optional
              },
            }
          : undefined,
        autoLogging: appConstant.TRUTHY_FALSY_VALUES.FALSE,
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
