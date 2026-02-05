import { Component, Inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import { TasksService } from '../../../services/task/task.service';
import { LookupsService } from '../../../services/lookup/lookups.services';
import { UsersService } from '../../../services/user/users.services';
import { Task } from '../../../models/task/task';
import { User } from '../../../models/user/user';
import { CreateTaskRequest } from '../../../models/task/createTaskRequest';
import { UpdateTaskRequest } from '../../../models/task/updateTaskRequest';
import { AuthService } from '../../../services/auth/auth.service';

interface DialogData {
  mode: 'create' | 'edit' | 'view';
  task?: Task;
}

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.scss']
})
export class TaskFormComponent implements OnInit {
  taskForm!: FormGroup;
  loading = false;
  taskStatuses: any;
  users: User[] = [];
  minDate = new Date();

  isAdmin(): boolean {
    return this.authService.hasRole('Admin');
  }

  constructor(
    private fb: FormBuilder,
    private tasksService: TasksService,
    private lookupsService: LookupsService,
    private usersService: UsersService,
    private authService: AuthService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef,
    public dialogRef: MatDialogRef<TaskFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {}

  ngOnInit(): void {  
    this.isAdmin();  
    this.initializeForm();
    this.loadFormData();
  }

  loadFormData(): void {
    this.loading = true;
    
    forkJoin({
      statuses: this.lookupsService.getTaskStatuses(),
      users: this.usersService.getUsers(1, 1000)
    }).subscribe({
      next: (result) => {
        this.taskStatuses = result.statuses.data;
        this.users = (result.users.data?.items || []).filter((u: User) => u.isActive);
                
        if (this.data.task) {
          this.taskForm.patchValue({
            title: this.data.task.title,
            descreption: this.data.task.descreption,
            statusId: this.data.task.statusId,
            assignedToUserId: this.data.task.assignedToUserId
          });
        }
        
        if (this.isViewMode) {
          this.taskForm.disable();
        }
        else if (this.isCreateMode) {
          this.taskForm.enable();
        }
        else if (this.isEditMode && !this.isAdmin()) {
          this.taskForm.get('title')?.disable();
          this.taskForm.get('descreption')?.disable();
          this.taskForm.get('assignedToUserId')?.disable();
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
    this.taskForm = this.fb.group({
      title: ['', Validators.required],
      descreption: ['', Validators.required],
      statusId: [null, Validators.required],
      assignedToUserId: ['', Validators.required]
    });
  }

  get f() {
    return this.taskForm.controls;
  }

  get title(): string {
    switch (this.data.mode) {
      case 'create': return 'Create New Task';
      case 'edit': return 'Edit Task';
      case 'view': return 'View Task';
      default: return '';
    }
  }

  get isViewMode(): boolean {
    return this.data.mode === 'view';
  }

  get isCreateMode(): boolean {
    return this.data.mode === 'create';
  }

  get isEditMode(): boolean {
    return this.data.mode === 'edit';
  }

  onSubmit(): void {
    if (this.taskForm.invalid) {
      Object.keys(this.taskForm.controls).forEach(key => {
        this.taskForm.controls[key].markAsTouched();
      });
      return;
    }

    this.loading = true;
    const formValue = this.taskForm.getRawValue(); // Use getRawValue() to get disabled fields too

    if (this.isCreateMode) {
      const createRequest: CreateTaskRequest = {
        title: formValue.title,
        descreption: formValue.descreption,
        statusId: formValue.statusId,
        assignedToUserId: formValue.assignedToUserId
      };

      this.tasksService.createTask(createRequest).subscribe({
        next: (task) => {
          this.toastr.success('Task created successfully', 'Success');
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.loading = false;
          this.cdr.detectChanges();
          console.error('Failed to create task:', error);
          const errorMessage = error?.error?.message || error?.message || 'Failed to create task';
          this.toastr.error(errorMessage, 'Error');
        }
      });
    } else {
      const updateRequest: UpdateTaskRequest = {
        id: this.data.task!.id,
        title: formValue.title,
        descreption: formValue.descreption,
        statusId: formValue.statusId,
        assignedToUserId: formValue.assignedToUserId
      };

      this.tasksService.updateTask(this.data.task!.id, updateRequest).subscribe({
        next: (task) => {
          this.toastr.success('Task updated successfully', 'Success');
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.loading = false;
          this.cdr.detectChanges();
          console.error('Failed to update task:', error);
          const errorMessage = error?.error?.message || error?.message || 'Failed to update task';
          this.toastr.error(errorMessage, 'Error');
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}