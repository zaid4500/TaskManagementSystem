import { Gender } from '../user/gender'

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  firstNameAr: string;
  lastNameAr: string;
  phoneNumber: string;
  genderId: number;
  gender?: Gender;
  isActive: boolean;
  emailConfirmed: boolean;
  createdAt: Date;
  role: string;
}