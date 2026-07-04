import {
  Component,
  ElementRef,
  input,
  ViewChild,
  OnChanges,
  SimpleChanges,
  effect,
} from '@angular/core';
import { Meme } from '../../models/meme.model';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-meme',
  templateUrl: './meme.component.html',
  styleUrls: ['./meme.component.scss'],
  standalone: true,
  imports: [ButtonModule],
})
export class MemeComponent {
  meme = input.required<Meme>();

  @ViewChild('canvas', { static: true }) canvas!: ElementRef<HTMLCanvasElement>;

  constructor() {
    effect(() => {
      this.updateCanvas();
    });
  }

  private updateCanvas() {
    const canvas = this.canvas.nativeElement;
    const ctx = canvas.getContext('2d');
    const MAX_SIZE = 500;

    if (!ctx) {
      console.error('Canvas rendering context not available.');
      return;
    }

    const image = new Image();
    image.crossOrigin = 'anonymous';
    image.src = this.meme().memeTemplate;
    image.onload = () => {
      // Calculate scaled dimensions
      let width = image.width;
      let height = image.height;

      if (width > MAX_SIZE || height > MAX_SIZE) {
        if (width > height) {
          height = (height * MAX_SIZE) / width;
          width = MAX_SIZE;
        } else {
          width = (width * MAX_SIZE) / height;
          height = MAX_SIZE;
        }
      }

      // Set canvas size to scaled dimensions
      canvas.width = width;
      canvas.height = height;

      // Clear the canvas
      ctx.clearRect(0, 0, width, height);

      // Draw the scaled image
      ctx.drawImage(image, 0, 0, width, height);

      // Overlay text
      ctx.font = '24px Arial';
      ctx.fillStyle = 'white';
      ctx.textAlign = 'center';

      const lineHeight = 30; // Space between lines
      const margin = 20; // Margin for text area
      const maxWidth = canvas.width - margin * 2;

      const wrapText = (text: string, maxWidth: number) => {
        const words = text.split(' ');
        const lines: string[] = [];
        let currentLine = words[0];

        for (let i = 1; i < words.length; i++) {
          const word = words[i];
          const testLine = currentLine + ' ' + word;
          const metrics = ctx.measureText(testLine);
          const testWidth = metrics.width;
          if (testWidth > maxWidth) {
            lines.push(currentLine);
            currentLine = word;
          } else {
            currentLine = testLine;
          }
        }
        lines.push(currentLine);
        return lines;
      };

      const lines = wrapText(this.meme().caption, maxWidth);

      // Calculate the y-coordinate for text placement in the lower half
      const totalTextHeight = lines.length * lineHeight;
      const startY = canvas.height - totalTextHeight - 50; // 50px padding from the bottom

      // Draw semi-transparent background for text
      ctx.fillStyle = 'rgba(0, 0, 0, 0.5)';
      ctx.fillRect(
        0,
        startY - lineHeight / 2,
        canvas.width,
        totalTextHeight + lineHeight
      );

      // Draw text line by line
      ctx.fillStyle = 'white';
      lines.forEach((line, index) => {
        ctx.fillText(
          line,
          canvas.width / 2,
          startY + index * lineHeight + lineHeight / 2
        );
      });
    };
  }

  downloadMeme() {
    const canvas = this.canvas.nativeElement;
    const link = document.createElement('a');
    link.href = canvas.toDataURL('image/png');
    link.download = 'meme.png';
    link.click();
  }
}
