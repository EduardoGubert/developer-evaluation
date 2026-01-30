import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
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
  registerForm!: FormGroup;
  isLoading = false;
  hidePassword = true;
  backendErrors: { [key: string]: string } = {};

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.initForm();
  }

  initForm(): void {
    this.registerForm = this.fb.group({
      firstname: ['', [Validators.required, Validators.maxLength(100)]],
      lastname: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      phone: ['', [Validators.required, Validators.minLength(8)]],
      city: ['', Validators.required],
      street: ['', Validators.required],
      number: [1, [Validators.required, Validators.min(1)]],
      zipcode: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: AbstractControl): { [key: string]: boolean } | null {
    const password = g.get('password')?.value;
    const confirmPassword = g.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }

  get f() { return this.registerForm.controls; }

  getPasswordErrors(): string[] {
    const errors: string[] = [];
    const ctrl = this.f['password'];
    if (!ctrl.value) return [];

    if (ctrl.value.length < 8) errors.push('Minimum 8 characters');
    if (!/[A-Z]/.test(ctrl.value)) errors.push('At least one uppercase letter');
    if (!/[a-z]/.test(ctrl.value)) errors.push('At least one lowercase letter');
    if (!/[0-9]/.test(ctrl.value)) errors.push('At least one number');
    if (!/[!?*.@#$%^&+=]/.test(ctrl.value)) errors.push('At least one special character (!?*.@#$%^&+=)');
    return errors;
  }

  isPasswordValid(): boolean {
    return this.getPasswordErrors().length === 0 && this.f['password'].value?.length >= 8;
  }

  onSubmit(): void {
    if (this.registerForm.invalid || !this.isPasswordValid()) {
      this.registerForm.markAllAsTouched();
      if (!this.isPasswordValid() && this.f['password'].value) {
        this.snackBar.open('Please fix password requirements', 'Close', { duration: 3000 });
      }
      return;
    }

    this.isLoading = true;
    this.backendErrors = {};

    const formValue = this.registerForm.value;
    const user: CreateUserRequest = {
      email: formValue.email,
      username: formValue.username,
      password: formValue.password,
      name: { firstname: formValue.firstname, lastname: formValue.lastname },
      address: {
        city: formValue.city,
        street: formValue.street,
        number: formValue.number,
        zipcode: formValue.zipcode,
        geolocation: { lat: '0', long: '0' }
      },
      phone: formValue.phone,
      status: 1,
      role: 2
    };

    this.userService.register(user).subscribe({
      next: () => {
        this.snackBar.open('Account created successfully! Please sign in.', 'Close', { duration: 5000 });
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isLoading = false;
        this.mapBackendErrors(err);
      }
    });
  }

  mapBackendErrors(error: any): void {
    if (error.error?.errors) {
      for (const e of error.error.errors) {
        const fieldMap: { [key: string]: string } = {
          'Email': 'email',
          'Username': 'username',
          'Password': 'password',
          'Phone': 'phone',
          'Name.Firstname': 'firstname',
          'Name.Lastname': 'lastname',
          'Address.City': 'city',
          'Address.Street': 'street',
          'Address.Number': 'number',
          'Address.Zipcode': 'zipcode'
        };
        const field = fieldMap[e.error] || e.error?.toLowerCase();
        if (field) {
          this.backendErrors[field] = e.detail;
        }
      }
    }
  }
}
