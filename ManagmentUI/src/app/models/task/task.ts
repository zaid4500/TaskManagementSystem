import { TaskStatus } from './taskStatus';

export interface Task {
  id: string;
  title: string;
  descreption: string;
  statusId: number;
  status?: TaskStatus;
  assignedToUserId: string;
  assignedToUser?: {
    id: string;
    firstNameEn: string;
    lastNameEn: string;
    email: string;
  };
  createdByUserId: string;
  createdByUser?: {
    id: string;
    firstNameEn: string;
    lastNameEn: string;
  };
  createdAt: Date;
  updatedAt?: Date;
}