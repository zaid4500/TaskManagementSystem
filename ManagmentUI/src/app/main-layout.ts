import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    NavbarComponent
  ],
  templateUrl: './main-layout.html',
  styles: [`
    .main-content {
      padding-top: 64px; 
      min-height: calc(100vh - 64px);
    }
  `]
})
export class MainLayoutComponent {}