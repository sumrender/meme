import { Component, QueryList, signal, ViewChildren, WritableSignal } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { Meme } from '../../models/meme.model';
import { FormsModule } from '@angular/forms';
import { MemeComponent } from '../meme/meme.component';

@Component({
  selector: 'app-text-meme-gen',
  imports: [FormsModule, MemeComponent],
  templateUrl: './text-meme-gen.component.html',
  styleUrl: './text-meme-gen.component.scss'
})
export class TextMemeGenComponent {
  textContent = signal('');
  isLoading = signal(false);
  isDownloading = signal(false);
  memes: WritableSignal<Meme[]> = signal([]);
  @ViewChildren(MemeComponent) memeComponents!: QueryList<MemeComponent>;

  constructor(private apiService: ApiService) {}

  generateMemes() {
    this.isLoading.set(true);
    this.memes.set([]);
    const memeFunction = this.textContent() ? this.apiService.generateMemes(this.textContent()) : this.apiService.generateRandomMemes();
    memeFunction.subscribe({
      next: (memes) => {
        this.memes.set(memes);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error generating memes:', error);
        alert("Please try again after some time 😁")
        this.isLoading.set(false);
      }
    }
    );
  }

  downloadAllMemes() {
    this.isDownloading.set(true);
    this.memeComponents.forEach((memeComponent, index) => {
      memeComponent?.downloadMeme();
    });
    this.isDownloading.set(false);
  }
}
