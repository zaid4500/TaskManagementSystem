export interface CreateTaskRequest {
  title: string;
  descreption: string;
  statusId: number;
  assignedToUserId: string;
}