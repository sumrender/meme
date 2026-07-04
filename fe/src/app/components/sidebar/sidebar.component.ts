import { Component, OnInit, signal } from '@angular/core';
import { AvatarModule } from 'primeng/avatar';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../services/auth.service';
import { GoogleUser } from '../../models/user.model';
import { MenuItem } from 'primeng/api';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [AvatarModule, MenuModule, ButtonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent implements OnInit {
  menuItems = signal<MenuItem[]>([]);
  constructor(private authService: AuthService, private router: Router) {}

  get user(): GoogleUser | null {
    return this.authService.user();
  }

  initializeMenu(): void {
    this.menuItems.set([
      {
        label: 'Sign Out',
        command: () => {
          this.signout();
        },
        icon: 'pi pi-sign-out',
      },
      {
        label: 'Settings',
        command: () => {
          // TODO: create settings page
          alert('settings');
        },
        icon: 'pi pi-cog',
      },
    ]);
  }

  signout(): void {
    this.authService.logout();
    this.router.navigate(['auth']);
  }

  ngOnInit(): void {
    this.initializeMenu();
  }
}
