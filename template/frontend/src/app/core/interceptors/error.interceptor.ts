import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private router: Router,
    private snackBar: MatSnackBar,
    private authService: AuthService
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let message = 'An unexpected error occurred';

        switch (error.status) {
          case 401:
            this.authService.logout();
            message = 'Session expired. Please log in again.';
            break;
          case 403:
            message = 'You do not have permission to perform this action.';
            break;
          case 404:
            message = 'Resource not found.';
            break;
          case 400:
            if (error.error?.errors?.length > 0) {
              message = error.error.errors.map((e: any) => e.detail || e.error).join(', ');
            } else if (error.error?.message) {
              message = error.error.message;
            }
            break;
          case 0:
            message = 'Unable to connect to the server.';
            break;
          default:
            if (error.error?.message) {
              message = error.error.message;
            }
        }

        this.snackBar.open(message, 'Close', {
          duration: 5000,
          horizontalPosition: 'end',
          verticalPosition: 'top'
        });

        return throwError(() => error);
      })
    );
  }
}
