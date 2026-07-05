import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { UserProfile } from '../models/user.model';
import { BASE_URL } from '../models/constants';

interface AuthResponse {
  accessToken: string;
  user: UserProfile;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private accessTokenSignal = signal<string | null>(null);
  user = signal<UserProfile | null>(null);
  isAuthenticated = computed(() => !!this.accessTokenSignal());

  constructor(private http: HttpClient, private router: Router) {}

  async restoreSession(): Promise<void> {
    try {
      const response = await this.http
        .post<AuthResponse>(`${BASE_URL}/api/auth/refresh`, {}, { withCredentials: true })
        .toPromise();

      if (response) {
        this.accessTokenSignal.set(response.accessToken);
        this.user.set(response.user);
      }
    } catch {
      // Silent fail - app boots in unauthenticated state
    }
  }

  createSignInButton(htmlElement: HTMLElement, callback?: () => void) {
    google.accounts.id.initialize({
      client_id:
        '978695525628-nv9bbeoo108untd736ht9qieag6ddq50.apps.googleusercontent.com',
      callback: async (res: google.accounts.id.CredentialResponse) => {
        await this.loginWithGoogle(res.credential);
        if (callback) callback();
      },
      ux_mode: 'popup',
      auto_select: true,
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

  async loginWithGoogle(idToken: string): Promise<void> {
    const response = await this.http
      .post<AuthResponse>(`${BASE_URL}/api/auth/google`, { idToken }, { withCredentials: true })
      .toPromise();

    if (!response) throw new Error('Login failed');

    this.accessTokenSignal.set(response.accessToken);
    this.user.set(response.user);
  }

  async refreshToken(): Promise<string> {
    try {
      const currentToken = this.accessTokenSignal();
      const response = await this.http
        .post<AuthResponse>(
          `${BASE_URL}/api/auth/refresh`,
          {},
          {
            headers: currentToken
              ? { Authorization: `Bearer ${currentToken}` }
              : {},
            withCredentials: true,
          }
        )
        .toPromise();

      if (!response) throw new Error('Refresh returned empty response');

      this.accessTokenSignal.set(response.accessToken);
      this.user.set(response.user);
      return response.accessToken;
    } catch (err) {
      this.accessTokenSignal.set(null);
      this.user.set(null);
      this.router.navigate(['/auth']);
      throw err;
    }
  }

  getAccessToken(): string | null {
    return this.accessTokenSignal();
  }

  async logout(): Promise<void> {
    try {
      await this.http.post(`${BASE_URL}/api/auth/revoke`, {}, { withCredentials: true }).toPromise();
    } catch {
      // Ignore errors
    }
    google.accounts.id.disableAutoSelect();
    this.accessTokenSignal.set(null);
    this.user.set(null);
    this.router.navigate(['/auth']);
  }
}
