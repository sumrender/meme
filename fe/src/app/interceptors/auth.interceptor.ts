import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { CreditService } from '../services/credit.service';
import { BASE_URL } from '../models/constants';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';

let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const authService = inject(AuthService);

  if (!req.url.startsWith(BASE_URL) || req.url.includes('/api/auth/')) {
    return next(req);
  }

  const token = authService.getAccessToken();
  let clonedReq = req;

  if (token) {
    clonedReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(clonedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && token) {
        return handle401AndRetry(req, next, authService);
      }
      if (error.status === 402) {
        const creditService = inject(CreditService);
        creditService.updateCredits(0);
      }
      return throwError(() => error);
    })
  );
};

function handle401AndRetry(req: HttpRequest<unknown>, next: HttpHandlerFn, authService: AuthService) {
  if (!isRefreshing) {
    isRefreshing = true;
    refreshTokenSubject.next(null);

    authService.refreshToken().then((newToken) => {
      isRefreshing = false;
      refreshTokenSubject.next(newToken);
    });
  }

  return refreshTokenSubject.pipe(
    filter((t) => t !== null),
    take(1),
    switchMap((newToken) => {
      const retryReq = req.clone({
        setHeaders: { Authorization: `Bearer ${newToken!}` },
      });
      return next(retryReq);
    }),
    catchError((err) => throwError(() => err))
  );
}
