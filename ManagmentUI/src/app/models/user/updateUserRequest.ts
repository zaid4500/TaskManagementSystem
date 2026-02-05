export interface UpdateUserRequest {
  id: string;
  email: string;
  firstNameEn: string;
  lastNameEn: string;
  firstNameAr: string;
  lastNameAr: string;
  phoneNumber: string;
  genderId: number;
  roles: string[];
}