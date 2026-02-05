import { Component, Input, Output, EventEmitter, ViewChild, OnInit, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormsModule } from '@angular/forms';

export interface TableColumn {
  field: string;
  header: string;
  sortable?: boolean;
  type?: 'text' | 'date' | 'boolean' | 'badge' | 'custom';
  width?: string;
  align?: 'left' | 'center' | 'right';
  customTemplate?: (row: any) => string;
  badgeConfig?: {
    getValue: (row: any) => string;
    getColor: (row: any) => 'primary' | 'accent' | 'warn' | 'success' | 'danger';
  };
}

export interface TableAction {
  icon: string;
  tooltip: string;
  color?: string;
  action: (row: any) => void;
  show?: (row: any) => boolean;
}

@Component({
  selector: 'app-generic-table',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    FormsModule
  ],
  templateUrl: './generic-table.component.html',
  styleUrls: ['./generic-table.component.scss']
})
export class GenericTableComponent implements OnInit, OnChanges {
  @Input() data: any[] = [];
  @Input() columns: TableColumn[] = [];
  @Input() actions: TableAction[] = [];
  @Input() loading: boolean = false;
  @Input() title: string = '';
  @Input() searchPlaceholder: string = 'Search...';
  @Input() pageSizeOptions: number[] = [5, 10, 20, 50, 100];
  @Input() defaultPageSize: number = 10;
  @Input() showSearch: boolean = true;
  @Input() showActions: boolean = true;
  @Input() emptyMessage: string = 'No records found';
  @Input() exportEnabled: boolean = false;

  @Output() rowClick = new EventEmitter<any>();
  @Output() export = new EventEmitter<void>();

  dataSource!: MatTableDataSource<any>;
  displayedColumns: string[] = [];
  searchValue: string = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit() {
    this.initializeTable();
  }

  ngOnChanges() {
    if (this.dataSource) {
      this.dataSource.data = this.data;
    }
  }

  initializeTable() {
    // Set displayed columns
    this.displayedColumns = this.columns.map(col => col.field);
    if (this.showActions && this.actions.length > 0) {
      this.displayedColumns.push('actions');
    }

    // Initialize data source
    this.dataSource = new MatTableDataSource(this.data);
    
    // Set paginator and sort after view init
    setTimeout(() => {
      if (this.paginator) {
        this.dataSource.paginator = this.paginator;
      }
      if (this.sort) {
        this.dataSource.sort = this.sort;
      }
    });

    // Custom filter predicate for nested properties
    this.dataSource.filterPredicate = (data: any, filter: string) => {
      const searchStr = filter.toLowerCase();
      return this.columns.some(col => {
        const value = this.getNestedValue(data, col.field);
        return value?.toString().toLowerCase().includes(searchStr);
      });
    };
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  clearFilter() {
    this.searchValue = '';
    this.dataSource.filter = '';
  }

  getNestedValue(obj: any, path: string): any {
    return path.split('.').reduce((current, prop) => current?.[prop], obj);
  }

  getCellValue(row: any, column: TableColumn): any {
    const value = this.getNestedValue(row, column.field);

    switch (column.type) {
      case 'date':
        return value ? new Date(value).toLocaleDateString() : '-';
      case 'boolean':
        return value ? 'Yes' : 'No';
      case 'custom':
        return column.customTemplate ? column.customTemplate(row) : value;
      default:
        return value ?? '-';
    }
  }

  getBadgeColor(row: any, column: TableColumn): string {
    if (column.badgeConfig?.getColor) {
      const color = column.badgeConfig.getColor(row);
      return `badge-${color}`;
    }
    return 'badge-primary';
  }

  onRowClick(row: any) {
    this.rowClick.emit(row);
  }

  shouldShowAction(action: TableAction, row: any): boolean {
    return action.show ? action.show(row) : true;
  }

  executeAction(action: TableAction, row: any, event: Event) {
    event.stopPropagation();
    action.action(row);
  }

  onExport() {
    this.export.emit();
  }
}