import { ChangeDetectorRef,Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule,Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'; // ← Make sure this is imported
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { ToastrService } from 'ngx-toastr';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { UsersService } from '../../../services/user/users.services';
import { UserFormComponent } from '../user-form/user-form.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { User } from '../../../models/user/user';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-users',
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
    MatProgressSpinnerModule, 
    MatSlideToggleModule,
    MatTooltipModule
  ],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  displayedColumns: string[] = ['firstName', 'lastName', 'email', 'phoneNumber', 'gender', 'isActive', 'actions'];
  dataSource = new MatTableDataSource<User>([]);
  
  totalCount = 0;
  pageSize = 10;
  pageNumber = 1;
  loading = false;
  searchTerm = '';
  searchSubject = new Subject<string>();

  isAdmin(): boolean {
    return this.authService.hasRole('Admin');
  }

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private usersService: UsersService,
    private dialog: MatDialog,
    private toastr: ToastrService,
    private authService: AuthService,
    private location: Location,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.isAdmin();
    if (!this.isAdmin()) {
      this.location.back();
      return;
    }
    this.loadUsers();
    this.cdr.detectChanges();

    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged()
    ).subscribe(searchTerm => {
      this.searchTerm = searchTerm;
      this.pageNumber = 1;
      this.loadUsers();
    });
  }

  loadUsers(): void {
    this.loading = true;
    this.usersService.getUsers(this.pageNumber, this.pageSize, this.searchTerm).subscribe({
      next: (result) => {
        this.dataSource.data = result.data.items;
        this.totalCount = result.data.totalCount;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load users', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchSubject.next(value);
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1;
    this.loadUsers();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(UserFormComponent, {
      width: '700px',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  openEditDialog(user: User): void {
    const dialogRef = this.dialog.open(UserFormComponent, {
      width: '700px',
      data: { mode: 'edit', user }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers();
      }
    });
  }

  viewUser(user: User): void {
    const dialogRef = this.dialog.open(UserFormComponent, {
      width: '700px',
      data: { mode: 'view', user }
    });
  }

  toggleUserStatus(user: User): void {
    const action = user.isActive ? 'deactivate' : 'activate';
    const message = user.isActive 
      ? 'Are you sure you want to deactivate this user?' 
      : 'Are you sure you want to activate this user?';

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: `${action.charAt(0).toUpperCase() + action.slice(1)} User`,
        message: message
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.usersService.changeUserStatus(user.id).subscribe({
          next: () => {
            this.toastr.success(`User ${action}d successfully`, 'Success');
            this.loadUsers();
          },
          error: (error) => {
            this.toastr.error(`Failed to ${action} user`, 'Error');
          }
        });
      }
    });
  }

  deleteUser(user: User): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete User',
        message: `Are you sure you want to delete ${user.firstName} ${user.lastName}? This action cannot be undone.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.usersService.deleteUser(user.id).subscribe({
          next: () => {
            this.toastr.success('User deleted successfully', 'Success');
            this.loadUsers();
          },
          error: (error) => {
            this.toastr.error('Failed to delete user', 'Error');
          }
        });
      }
    });
  }
}