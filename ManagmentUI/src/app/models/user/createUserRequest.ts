export interface CreateUserRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  firstNameAr: string;
  lastNameAr: string;
  phoneNumber: string;
  genderId: number;
  role: string[];
}