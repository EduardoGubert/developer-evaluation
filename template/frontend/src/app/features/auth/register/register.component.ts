import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { CreateUserRequest } from '../../../core/models/user.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  user: CreateUserRequest = {
    email: '',
    username: '',
    password: '',
    name: { firstname: '', lastname: '' },
    address: {
      city: '',
      street: '',
      number: 0,
      zipcode: '',
      geolocation: { lat: '0', long: '0' }
    },
    phone: '',
    status: 1,
    role: 2
  };

  confirmPassword = '';
  isLoading = false;
  hidePassword = true;

  constructor(
    private userService: UserService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  onSubmit(): void {
    if (!this.user.email || !this.user.username || !this.user.password) {
      this.snackBar.open('Email, username and password are required.', 'Close', { duration: 3000 });
      return;
    }

    if (this.user.password !== this.confirmPassword) {
      this.snackBar.open('Passwords do not match.', 'Close', { duration: 3000 });
      return;
    }

    this.isLoading = true;

    this.userService.register(this.user).subscribe({
      next: () => {
        this.snackBar.open('Account created successfully! Please sign in.', 'Close', { duration: 5000 });
        this.router.navigate(['/login']);
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }
}
