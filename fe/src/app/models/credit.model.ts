import { Meme } from './meme.model';

export interface CreditBalance {
    credits: number;
}

export interface GenerateMemesResponse {
    memes: Meme[];
    remainingCredits: number;
}
