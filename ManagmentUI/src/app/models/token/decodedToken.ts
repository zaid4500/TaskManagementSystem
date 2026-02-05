export interface DecodedToken {
  nameid: string;
  email: string;
  given_name: string;
  family_name: string;
  role: string | string[];
  exp: number;
  nbf: number;
  iat: number;
}