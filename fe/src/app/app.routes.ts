import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { NoAuthGuard } from './guards/no-auth.guard';
import { AuthComponent } from './components/auth/auth.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { MyMemesComponent } from './components/my-memes/my-memes.component';
import { AlbumDetailComponent } from './components/album-detail/album-detail.component';

export const routes: Routes = [
  { path: '', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'my-memes', component: MyMemesComponent, canActivate: [AuthGuard] },
  { path: 'my-memes/:id', component: AlbumDetailComponent, canActivate: [AuthGuard] },
  { path: 'auth', component: AuthComponent, canActivate: [NoAuthGuard] },
  { path: '**', redirectTo: '' },
];
