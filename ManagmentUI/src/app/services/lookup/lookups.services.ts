import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Gender } from '../../models/user/gender';
import { TaskStatus } from '../../models/task/taskStatus';

@Injectable({
  providedIn: 'root'
})


export class LookupsService {
  private apiUrl = `${environment.apiUrl}/api/v1/lookups`;

  constructor(private http: HttpClient) {}

  getGenders(): Observable<Gender> {
    return this.http.get<Gender>(this.apiUrl+'/genders');
  }

  getTaskStatuses(): Observable<TaskStatus> {
    return this.http.get<TaskStatus>(this.apiUrl+'/task-statuses');
  }

  getRoles(): Observable<any> {
    return this.http.get<any>(this.apiUrl+'/roles');
  }
}