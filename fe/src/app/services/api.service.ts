import { HttpClient } from '@angular/common/http';
import { Injectable, isDevMode } from '@angular/core';
import { Meme } from '../models/meme.model';
import { AlbumSummary, AlbumDetail } from '../models/album.model';
import { BASE_URL } from '../models/constants';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private baseUrl = BASE_URL;

  constructor(private http: HttpClient) {
    // if (isDevMode()) {
    //   this.baseUrl = 'http://localhost:8080';
    //   console.log('using localhost api');
    // }
  }

  generateRandomMemes() {
    return this.http.get<Meme[]>(`${this.baseUrl}/generate-random-memes`);
  }

  generateMemes(textContent: string) {
    return this.http.post<Meme[]>(`${this.baseUrl}/generate-memes`, {
      textContent,
    });
  }

  getAlbums() {
    return this.http.get<AlbumSummary[]>(`${this.baseUrl}/api/albums`);
  }

  getAlbum(id: number) {
    return this.http.get<AlbumDetail>(`${this.baseUrl}/api/albums/${id}`);
  }
}
