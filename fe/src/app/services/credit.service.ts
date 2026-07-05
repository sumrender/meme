import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { CreditBalance } from '../models/credit.model';
import { BASE_URL } from '../models/constants';

@Injectable({
  providedIn: 'root',
})
export class CreditService {
  private http = inject(HttpClient);

  credits = signal<number | null>(null);
  hasCredits = computed(() => (this.credits() ?? 0) > 0);

  fetchCredits() {
    return this.http.get<CreditBalance>(`${BASE_URL}/api/credits`).pipe(
      tap((res) => this.credits.set(res.credits))
    );
  }

  updateCredits(amount: number) {
    this.credits.set(amount);
  }
}
