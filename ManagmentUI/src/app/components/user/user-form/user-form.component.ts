import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { UsersService } from '../../../services/user/users.services';
import { LookupsService } from '../../../services/lookup/lookups.services';
import { User } from '../../../models/user/user';

interface DialogData {
  mode: 'create' | 'edit' | 'view';
  user?: User;
}

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.scss']
})
export class UserFormComponent implements OnInit {
  userForm!: FormGroup;
  loading = false;
  genders: any;
  hidePassword = true;
  hideConfirmPassword = true;
  availableRoles: any[] = [];

  constructor(
    private fb: FormBuilder,
    private usersService: UsersService,
    private lookupsService: LookupsService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef,
    public dialogRef: MatDialogRef<UserFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {}

  ngOnInit(): void {
    this.loadFormData();
    this.initializeForm();
    this.cdr.detectChanges();

    if (this.data.mode === 'view') {
      this.userForm.disable();
    }
  }

    loadFormData(): void {
      this.loading = true;
      
      forkJoin({
        genders: this.lookupsService.getGenders(),
        roles: this.lookupsService.getRoles()
      }).subscribe({
        next: (result) => {
          this.genders = result.genders.data;
          this.availableRoles = result.roles.data;
                  
          if (this.data.user) {
            this.userForm.patchValue({
              email: this.data.user.email,
              firstName: this.data.user.firstName,
              lastName: this.data.user.lastName,
              firstNameAr: this.data.user.firstNameAr,
              lastNameAr: this.data.user.lastNameAr,
              phoneNumber: this.data.user.phoneNumber,
              genderId: this.data.user.genderId,
              role: this.data.user.role,
            });
          }
          
          if (this.isViewMode) {
            this.userForm.disable();
          }
          this.loading = false;
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error('Failed to load form data:', error);
          this.toastr.error('Failed to load form data', 'Error');
          this.loading = false;
          this.cdr.detectChanges();
        }
      });
    }
  
  

  initializeForm(): void {
    const isCreateMode = this.data.mode === 'create';
    const user = this.data.user;

    this.userForm = this.fb.group({
      email: [user?.email || '', [Validators.required, Validators.email]],
      firstNameEn: [user?.firstName || '', Validators.required],
      lastNameEn: [user?.lastName || '', Validators.required],
      firstNameAr: [user?.firstNameAr || '', Validators.required],
      lastNameAr: [user?.lastNameAr || '', Validators.required],
      phoneNumber: [user?.phoneNumber || '', [Validators.required, Validators.pattern(/^\+962[0-9]{7,9}$/)]],
      genderId: [user?.genderId || null, Validators.required],
      role: [user?.role || [], Validators.required]
    });
    
    if (isCreateMode) {
      this.userForm.addControl('password', this.fb.control('', [Validators.required, Validators.minLength(6)]));
      this.userForm.addControl('confirmPassword', this.fb.control('', [Validators.required]));
    }
  }

  get f() {
    return this.userForm.controls;
  }

  get title(): string {
    switch (this.data.mode) {
      case 'create': return 'Create New User';
      case 'edit': return 'Edit User';
      case 'view': return 'View User';
      default: return '';
    }
  }

  get isViewMode(): boolean {
    return this.data.mode === 'view';
  }

  get isCreateMode(): boolean {
    return this.data.mode === 'create';
  }

  passwordsMatch(): boolean {
    if (!this.isCreateMode) return true;
    const password = this.userForm.get('password')?.value;
    const confirmPassword = this.userForm.get('confirmPassword')?.value;
    return password === confirmPassword;
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      Object.keys(this.userForm.controls).forEach(key => {
        this.userForm.controls[key].markAsTouched();
      });
      return;
    }

    if (this.isCreateMode && !this.passwordsMatch()) {
      this.toastr.error('Passwords do not match', 'Validation Error');
      return;
    }

    this.loading = true;
    const formValue = this.userForm.value;

    if (this.isCreateMode) {
      const createRequest = {
        email: formValue.email,
        password: formValue.password,
        confirmPassword: formValue.confirmPassword,
        firstName: formValue.firstNameEn,
        lastName: formValue.lastNameEn,
        firstNameAr: formValue.firstNameAr,
        lastNameAr: formValue.lastNameAr,
        phoneNumber: formValue.phoneNumber,
        genderId: formValue.genderId,
        role: formValue.roles
      };

      this.usersService.createUser(createRequest).subscribe({
        next: (user) => {
          this.toastr.success('User created successfully', 'Success');
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.loading = false;
          const errorMessage = error?.error?.message || 'Failed to create user';
          this.toastr.error(errorMessage, 'Error');
        }
      });
    } else {
      const updateRequest = {
        id: this.data.user!.id,
        email: formValue.email,
        firstNameEn: formValue.firstNameEn,
        lastNameEn: formValue.lastNameEn,
        firstNameAr: formValue.firstNameAr,
        lastNameAr: formValue.lastNameAr,
        phoneNumber: formValue.phoneNumber,
        genderId: formValue.genderId,
        roles: formValue.roles
      };

      this.usersService.updateUser(this.data.user!.id, updateRequest).subscribe({
        next: (user) => {
          this.toastr.success('User updated successfully', 'Success');
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.loading = false;
          const errorMessage = error?.error?.message || 'Failed to update user';
          this.toastr.error(errorMessage, 'Error');
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}