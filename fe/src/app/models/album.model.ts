import { Meme } from './meme.model';

export interface AlbumSummary {
  id: number;
  textSnippet: string;
  createdAt: string;
  memeCount: number;
  thumbnails: string[];
}

export interface AlbumDetail {
  id: number;
  textContent: string;
  createdAt: string;
  memes: Meme[];
}
