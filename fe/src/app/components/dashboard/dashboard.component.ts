import { Component, inject, signal } from '@angular/core';
import { TextAreaComponent } from '../textarea/text-area.component';
import { Meme } from '../../models/meme.model';
import { ApiService } from '../../services/api.service';
import { CreditService } from '../../services/credit.service';
import { MemeComponent } from '../meme/meme.component';


@Component({
  selector: 'app-dashboard',
  imports: [TextAreaComponent, MemeComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  isLoading = signal(false);
  memes = signal<Meme[]>([]);

  private apiService = inject(ApiService);
  creditService = inject(CreditService);

  generateMemes(text: string) {
    this.isLoading.set(true);
    this.memes.set([]);
    this.apiService.generateMemes(text).subscribe({
      next: (response) => {
        this.memes.set(response.memes);
        this.creditService.updateCredits(response.remainingCredits);
      },
      error: (error) => {
        console.error('Error generating memes:', error);
        alert('Please try again after some time 😁');
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }
}
