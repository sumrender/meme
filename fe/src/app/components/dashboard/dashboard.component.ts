import { Component, signal } from '@angular/core';
import { TextAreaComponent } from '../textarea/text-area.component';
import { Meme } from '../../models/meme.model';
import { ApiService } from '../../services/api.service';
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

  constructor(private apiService: ApiService) {}

  generateMemes(text: string) {
    this.isLoading.set(true);
    this.memes.set([]);
    this.apiService.generateMemes(text).subscribe({
      next: (memes) => {
        this.memes.set(memes);
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
