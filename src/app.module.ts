import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { ConfigModule } from '@nestjs/config';
import { UsersModule } from './modules/users/users.module';
import { AuthController } from './modules/auth/auth.controller';
import { AuthService } from './modules/auth/auth.service';
import { AuthModule } from './modules/auth/auth.module';
import { PrismaModule } from './prisma/prisma.module';
import { NodeEnv } from './common/enums';
import { appConfig, dbConfig } from './config';

const isProd = process.env.NODE_ENV === String(NodeEnv.PRODUCTION);
@Module({
  imports: [
    ConfigModule.forRoot({
      isGlobal: true,
      // In local/dev/test, load .env.* files;
      // In prod, prefer runtime env and ignore .env files.
      ignoreEnvFile: isProd,
      envFilePath: isProd ? undefined : ['.env'], // fallback to .env if present
      expandVariables: true,
      load: [appConfig, dbConfig], // typed, namespaced configs
    }),
    UsersModule,
    AuthModule,
    PrismaModule,
  ],
  controllers: [AppController, AuthController],
  providers: [AppService, AuthService],
})
export class AppModule {}
