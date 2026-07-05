import { Component, OnInit, signal } from '@angular/core';
import { AvatarModule } from 'primeng/avatar';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../services/auth.service';
import { UserProfile } from '../../models/user.model';
import { MenuItem } from 'primeng/api';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [AvatarModule, MenuModule, ButtonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent implements OnInit {
  menuItems = signal<MenuItem[]>([]);

  constructor(private authService: AuthService) {}

  get user(): UserProfile | null {
    return this.authService.user();
  }

  initializeMenu(): void {
    this.menuItems.set([
      {
        label: 'Sign Out',
        command: () => {
          this.authService.logout();
        },
        icon: 'pi pi-sign-out',
      },
      {
        label: 'Settings',
        command: () => {
          alert('settings');
        },
        icon: 'pi pi-cog',
      },
    ]);
  }

  ngOnInit(): void {
    this.initializeMenu();
  }
}
