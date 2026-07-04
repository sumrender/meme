import { HttpClient } from '@angular/common/http';
import { Injectable, isDevMode } from '@angular/core';
import { Meme } from '../models/meme.model';
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

  /**
   * Fetches a random meme image.
   * Returns the image as a Blob.
   */
  generateRandomMemes() {
    return this.http.get<Meme[]>(`${this.baseUrl}/random-memes`);
  }

  /**
   * Generates memes based on provided text content.
   * @param textContent - The text content for meme generation.
   * @returns An observable of GenerateMemesResponse.
   */
  generateMemes(textContent: string) {
    return this.http.post<Meme[]>(`${this.baseUrl}/generate-memes`, {
      textContent,
    });
  }
}
