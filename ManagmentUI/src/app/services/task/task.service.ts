import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Task } from '../../models/task/task';
import { CreateTaskRequest } from '../../models/task/createTaskRequest';
import { UpdateTaskRequest } from '../../models/task/updateTaskRequest';
import { PagedResult } from '../../models/shared/pagedResult';


@Injectable({
  providedIn: 'root'
})

export class TasksService {
  private apiUrl = `${environment.apiUrl}/api/v1/tasks`;

  constructor(private http: HttpClient) {}

  getTasks(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTerm?: string,
    statusId?: number,
    assignedToUserId?: string,
    getMyTasks?: boolean
  ): Observable<PagedResult<Task>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    if (statusId) {
      params = params.set('statusId', statusId.toString());
    }
    if (assignedToUserId) {
      params = params.set('assignedToUserId', assignedToUserId);
    }

    let apiUrl = this.apiUrl;
    if (!getMyTasks) {
      apiUrl += '/all-tasks';
    }

    return this.http.get<PagedResult<Task>>(apiUrl, { params });
  }

  getTaskById(id: string): Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/${id}`);
  }

  createTask(task: CreateTaskRequest): Observable<Task> {
    return this.http.post<Task>(this.apiUrl+'/Create', task);
  }

  updateTask(id: string, task: UpdateTaskRequest): Observable<Task> {
    return this.http.put<Task>(`${this.apiUrl}/${id}`, task);
  }

  deleteTask(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getMyTasks(pageNumber: number = 1, pageSize: number = 10): Observable<PagedResult<Task>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<Task>>(`${this.apiUrl}/my-tasks`, { params });
  }
}