import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TextareaModule } from 'primeng/textarea';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-text-area',
  imports: [TextareaModule, FormsModule, ButtonModule],
  templateUrl: './text-area.component.html',
  styleUrl: './text-area.component.scss',
})
export class TextAreaComponent {
  text = '';

  @Input() loading = false;

  @Output() onClick = new EventEmitter<string>();

  emitText(): void {
    this.onClick.emit(this.text);
  }

  clearText(): void {
    this.text = '';
  }
}
