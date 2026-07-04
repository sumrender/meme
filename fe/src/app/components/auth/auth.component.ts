import { Component, signal, effect, WritableSignal } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss'],
})
export class AuthComponent {
  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    const googleButton = document.getElementById('google-button');
    if (googleButton) {
      this.authService.createSignInButton(googleButton, () => {
        this.router.navigate(['']);
      });
    }
  }
}
