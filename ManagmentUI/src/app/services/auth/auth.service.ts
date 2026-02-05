import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { UserDto } from '../../models/login/UserDto';
import { LoginResponse } from '../../models/login/loginResponse';
import { DecodedToken } from '../../models/token/decodedToken';
import { environment } from '../../../environments/environment';


@Injectable({
  providedIn: 'root'
})


export class AuthService {
  private apiUrl = `${environment.apiUrl}/api/v1/tokens`;
  private currentUserSubject: BehaviorSubject<UserDto | null>;
  public currentUser: Observable<UserDto | null>;
  private refreshTokenTimeout?: ReturnType<typeof setTimeout>;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    const user = this.getUserFromStorage();
    this.currentUserSubject = new BehaviorSubject<UserDto | null>(user);
    this.currentUser = this.currentUserSubject.asObservable();

    if (user && this.getToken()) {
      this.startRefreshTokenTimer();
    }
  }

  public get currentUserValue(): UserDto | null {
    return this.currentUserSubject.value;
  }

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, { email, password })
      .pipe(
        tap(response => {
          this.storeTokens(response.data.token, response.data.refreshToken);
          this.storeUser(response.data.user);
          this.currentUserSubject.next(response.data.user);
          this.startRefreshTokenTimer();
        }),
        catchError(error => {
          return throwError(() => error);
        })
      );
  }

  logout(): void {
    this.stopRefreshTokenTimer();
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('current_user');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  refreshToken(): Observable<LoginResponse> {
    const token = this.getToken();
    const refreshToken = this.getRefreshToken();

    if (!token || !refreshToken) {
      this.logout();
      return throwError(() => new Error('No tokens available'));
    }

    return this.http.post<LoginResponse>(`${this.apiUrl}/refresh-token`, {
      token,
      refreshToken
    }).pipe(
      tap(response => {
        this.storeTokens(response.data.token, response.data.refreshToken);
        this.storeUser(response.data.user);
        this.currentUserSubject.next(response.data.user);
        this.startRefreshTokenTimer();
      }),
      catchError(error => {
        this.logout();
        return throwError(() => error);
      })
    );
  }

  getToken(): string | null {
    return localStorage.getItem('access_token');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const decoded = jwtDecode<DecodedToken>(token);
      const currentTime = Math.floor(Date.now() / 1000);
      return decoded.exp > currentTime;
    } catch {
      return false;
    }
  }

  getUserRoles(): string[] {
    const token = this.getToken();
    if (!token) return [];

    try {
      const decoded = jwtDecode<DecodedToken>(token);
      if (Array.isArray(decoded.role)) {
        return decoded.role;
      }
      return decoded.role ? [decoded.role] : [];
    } catch {
      return [];
    }
  }

  hasRole(role: string): boolean {
    return this.getUserRoles().includes(role);
  }

  hasAnyRole(roles: string[]): boolean {
    const userRoles = this.getUserRoles();
    return roles.some(role => userRoles.includes(role));
  }

  private storeTokens(token: string, refreshToken: string): void {
    localStorage.setItem('access_token', token);
    localStorage.setItem('refresh_token', refreshToken);
  }

  private storeUser(user: UserDto): void {
    localStorage.setItem('current_user', JSON.stringify(user));
  }

  private getUserFromStorage(): UserDto | null {
    const userJson = localStorage.getItem('current_user');
    if (!userJson) return null;

    try {
      return JSON.parse(userJson);
    } catch {
      return null;
    }
  }

  private startRefreshTokenTimer(): void {
    const token = this.getToken();
    if (!token) return;

    try {
      const decoded = jwtDecode<DecodedToken>(token);
      const expires = new Date(decoded.exp * 1000);
      const timeout = expires.getTime() - Date.now() - (60 * 1000); 

      this.refreshTokenTimeout = setTimeout(() => {
        this.refreshToken().subscribe();
      }, timeout);
    } catch (error) {
      console.error('Error parsing token:', error);
    }
  }

  private stopRefreshTokenTimer(): void {
    if (this.refreshTokenTimeout) {
      clearTimeout(this.refreshTokenTimeout);
    }
  }
}