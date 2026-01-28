import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponseWithData } from '../models/api-response.model';
import { CreateUserRequest } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  register(user: CreateUserRequest): Observable<any> {
    return this.http.post<ApiResponseWithData<any>>(this.apiUrl, user).pipe(
      map(res => res.data)
    );
  }
}
