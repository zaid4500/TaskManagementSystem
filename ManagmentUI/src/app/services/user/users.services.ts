import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { User } from '../../models/user/user';
import { CreateUserRequest } from '../../models/user/createUserRequest';
import { UpdateUserRequest } from '../../models/user/updateUserRequest';
import { ChangePasswordRequest } from '../../models/user/changePasswordRequest';
import { PagedResult } from '../../models/shared/pagedResult';

@Injectable({
  providedIn: 'root'
})


export class UsersService {
  private apiUrl = `${environment.apiUrl}/api/v1/users`;

  constructor(private http: HttpClient) {}

  getUsers(pageNumber: number = 1, pageSize: number = 10, searchTerm?: string): Observable<PagedResult<User>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<PagedResult<User>>(this.apiUrl, { params });
  }

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }

  createUser(user: CreateUserRequest): Observable<User> {
    return this.http.post<User>(this.apiUrl, user);
  }

  updateUser(id: string, user: UpdateUserRequest): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${id}`, user);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  changeUserStatus(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/change-user-status`, {});
  }
}