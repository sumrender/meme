import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ButtonModule } from 'primeng/button';
import { ApiService } from '../../services/api.service';
import { AlbumDetail } from '../../models/album.model';
import { MemeComponent } from '../meme/meme.component';

@Component({
  selector: 'app-album-detail',
  standalone: true,
  imports: [RouterLink, DatePipe, MemeComponent, ProgressSpinnerModule, ButtonModule],
  templateUrl: './album-detail.component.html',
  styleUrl: './album-detail.component.scss',
})
export class AlbumDetailComponent implements OnInit {
  album = signal<AlbumDetail | null>(null);
  isLoading = signal(true);

  private apiService = inject(ApiService);
  private route = inject(ActivatedRoute);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.apiService.getAlbum(id).subscribe({
      next: (album) => this.album.set(album),
      error: (err) => console.error('Failed to load album:', err),
      complete: () => this.isLoading.set(false),
    });
  }
}
