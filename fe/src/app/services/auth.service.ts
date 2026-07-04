import { Injectable, signal, WritableSignal } from '@angular/core';
import { GoogleUser } from '../models/user.model';
import { MEME_USER } from '../models/constants';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  user: WritableSignal<GoogleUser | null> = signal(null);

  constructor() {
    const userString = sessionStorage.getItem(MEME_USER);
    if (userString) {
      const user = JSON.parse(userString) as GoogleUser;
      this.user.set(user);
    }
  }

  createSignInButton(htmlElement: HTMLElement, callback?: (user: GoogleUser) => void) {
    google.accounts.id.initialize({
      client_id:
        '978695525628-nv9bbeoo108untd736ht9qieag6ddq50.apps.googleusercontent.com',
      callback: (res: google.accounts.id.CredentialResponse) => {
        const user = this.decodeJWTToken(res.credential);
        this.user.set(user);
        sessionStorage.setItem(MEME_USER, JSON.stringify(user));
        if (callback) {
          callback(user);
        }
      },
    });
    google.accounts.id.prompt();
    google.accounts.id.renderButton(htmlElement, {
      theme: 'filled_blue',
      size: 'large',
      shape: 'rectangular',
      type: 'standard',
      width: 350,
    });
  }

  logout() {
    google.accounts.id.disableAutoSelect();
    sessionStorage.removeItem(MEME_USER);
    this.user.set(null);
  }

  private decodeJWTToken(token: string): GoogleUser {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
