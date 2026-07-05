import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ApiService } from '../../services/api.service';
import { AlbumSummary } from '../../models/album.model';

@Component({
  selector: 'app-my-memes',
  standalone: true,
  imports: [RouterLink, DatePipe, ProgressSpinnerModule],
  templateUrl: './my-memes.component.html',
  styleUrl: './my-memes.component.scss',
})
export class MyMemesComponent implements OnInit {
  albums = signal<AlbumSummary[]>([]);
  isLoading = signal(true);

  private apiService = inject(ApiService);

  ngOnInit() {
    this.apiService.getAlbums().subscribe({
      next: (albums) => this.albums.set(albums),
      error: (err) => console.error('Failed to load albums:', err),
      complete: () => this.isLoading.set(false),
    });
  }
}
