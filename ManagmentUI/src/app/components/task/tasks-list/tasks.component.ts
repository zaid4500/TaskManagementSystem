import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { ToastrService } from 'ngx-toastr';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { TasksService } from '../../../services/task/task.service';
import { LookupsService } from '../../../services/lookup/lookups.services';
import { TaskFormComponent } from '../task-form/task-form.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { Task } from '../../../models/task/task';
import { TaskStatus } from '../../../models/task/taskStatus';
import { AuthService } from '../../../services/auth/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatTooltipModule,
    MatChipsModule
  ],
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.scss']
})
export class TasksComponent implements OnInit {
  displayedColumns: string[] = ['title', 'assignedToUser', 'status', 'createdOn', 'actions'];
  dataSource = new MatTableDataSource<Task>([]);
  
  totalCount = 0;
  pageSize = 10;
  pageNumber = 1;
  loading = false;
  searchTerm = '';
  selectedStatus?: number;
  searchSubject = new Subject<string>();
  taskStatuses: TaskStatus['data'] | null = null; 
  isMyTasks: boolean = false;

  isAdmin(): boolean {
    return this.authService.hasRole('Admin');
  }

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private tasksService: TasksService,
    private lookupsService: LookupsService,
    private authService: AuthService,
    private dialog: MatDialog,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.isMyTasks = this.route.snapshot.data['isMyTasks'] || false;
    this.loadTaskStatuses();
    this.loadTasks();
    this.isAdmin();

    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(searchTerm => {
      this.searchTerm = searchTerm;
      this.pageNumber = 1;
      this.loadTasks();
    });
  }

  loadTaskStatuses(): void {
    this.lookupsService.getTaskStatuses().subscribe({
      next: (response: TaskStatus) => {
        this.taskStatuses = response.data; 
      },
      error: (error) => {
        this.toastr.error('Failed to load task statuses', 'Error');
      }
    });
  }

loadTasks(): void {
  this.loading = true;
  
  this.tasksService.getTasks(
    this.pageNumber,
    this.pageSize,
    this.searchTerm,
    this.selectedStatus,
    undefined,
    this.isMyTasks
  ).subscribe({
    next: (result) => {
      this.dataSource.data = result.data.items || [];
      this.totalCount = result.data.totalCount || 0;
      this.loading = false;
      this.cdr.detectChanges();
    },
    error: (error) => {
      console.error('Failed to load tasks:', error);
      this.toastr.error('Failed to load tasks', 'Error');
      this.loading = false;
      this.cdr.detectChanges();
    }
  });
}
  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchSubject.next(value);
  }

  onStatusFilter(statusId?: number): void {
    this.selectedStatus = statusId;
    this.pageNumber = 1;
    this.loadTasks();
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1;
    this.loadTasks();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(TaskFormComponent, {
      width: '800px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadTasks();
      }
    });
  }

  openEditDialog(task: Task): void {
    
    if (!task || !task.id) {
      this.toastr.error('Invalid task data', 'Error');
      return;
    }

    const dialogRef = this.dialog.open(TaskFormComponent, {
      width: '800px',
      data: { mode: 'edit', task }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadTasks();
      }
    });
  }

  viewTask(task: Task): void {
    if (!task || !task.id) {
      this.toastr.error('Invalid task data', 'Error');
      return;
    }

    this.dialog.open(TaskFormComponent, {
      width: '800px',
      data: { mode: 'view', task }
    });
  }

  deleteTask(task: Task): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete Task',
        message: `Are you sure you want to delete "${task.title}"? This action cannot be undone.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.tasksService.deleteTask(task.id).subscribe({
          next: () => {
            this.toastr.success('Task deleted successfully', 'Success');
            this.loadTasks();
          },
          error: (error) => {
            console.error('Failed to delete task:', error);
            this.toastr.error('Failed to delete task', 'Error');
          }
        });
      }
    });
  }

  getStatusClass(statusCode: string): string {
    switch (statusCode?.toLowerCase()) {
      case 'new': return 'status-new';
      case 'active': return 'status-active';
      case 'closed': return 'status-closed';
      default: return '';
    }
  }
}