import { UserDto } from './UserDto';

export interface LoginResponse {
  succeeded: boolean;
  message: string;
  brokenRules: string[];
  statusCode: number;
  data: {
    token: string;
    refreshToken: string;
    expiration: Date;
    user: UserDto;
  }
}