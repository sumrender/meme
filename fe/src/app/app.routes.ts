import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { NoAuthGuard } from './guards/no-auth.guard';
import { AuthComponent } from './components/auth/auth.component';
import { TextMemeGenComponent } from './components/text-meme-gen/text-meme-gen.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { SavedComponent } from './components/saved/saved.component';

export const routes: Routes = [
  { path: '', component: DashboardComponent },
  // { path: '', component: DashboardComponent, canActivate: [AuthGuard] },
  // { path: 'auth', component: AuthComponent, canActivate: [NoAuthGuard] },
  // { path: 'saved', component: SavedComponent },
];
