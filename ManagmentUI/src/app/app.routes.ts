import { Routes } from '@angular/router';
import { authGuard } from './services/auth/auth.guard';
import { MainLayoutComponent } from './main-layout';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'my-tasks',
        loadComponent: () => import('./components/task/tasks-list/tasks.component').then(m => m.TasksComponent),
        data: { isMyTasks: true }
      },
      {
        path: 'tasks',
        loadComponent: () => import('./components/task/tasks-list/tasks.component').then(m => m.TasksComponent),
        data: { isMyTasks: false }
      },
      {
        path: 'users',
        loadComponent: () => import('./components/user/users-list/users.component').then(m => m.UsersComponent)
      },
      {
        path: '',
        redirectTo: 'tasks',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
