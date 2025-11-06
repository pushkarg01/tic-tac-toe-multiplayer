import { registerAs } from '@nestjs/config';
import { appConstant } from 'src/common/constants';

export default registerAs('app', () => ({
  env: process.env.NODE_ENV,
  port: parseInt(process.env.PORT ?? String(appConstant.PORT), 10),
  logLevel: process.env.LOG_LEVEL ?? appConstant.LOG_LEVEL,
}));
