export interface UpdateTaskRequest {
  id: string;
  title: string;
  descreption: string; 
  statusId: number;
  assignedToUserId: string;
}