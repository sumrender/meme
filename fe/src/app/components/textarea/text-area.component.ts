import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TextareaModule } from 'primeng/textarea';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-text-area',
  imports: [TextareaModule, FormsModule, ButtonModule, TooltipModule],
  templateUrl: './text-area.component.html',
  styleUrl: './text-area.component.scss',
})
export class TextAreaComponent {
  text = '';

  @Input() loading = false;
  @Input() credits: number | null = null;

  @Output() onClick = new EventEmitter<string>();

  get isGenerateDisabled(): boolean {
    const noText = this.text.trim().length === 0;
    if (noText) return true;
    if (this.loading) return true;
    if (this.credits !== null && this.credits <= 0) return true;
    return false;
  }

  emitText(): void {
    this.onClick.emit(this.text);
  }

  clearText(): void {
    this.text = '';
  }
}
