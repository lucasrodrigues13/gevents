import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TemaService } from '../core/tema.service';

@Component({
  selector: 'app-tema',
  host: { class: 'gd-tema-fixo' },
  imports: [MatButtonModule, MatIconModule],
  template: `
    <button
      mat-icon-button
      type="button"
      (click)="tema.alternar()"
      [attr.aria-label]="tema.escuro() ? 'Usar modo claro' : 'Usar modo escuro'"
    >
      <mat-icon>{{ tema.escuro() ? 'light_mode' : 'dark_mode' }}</mat-icon>
    </button>
  `,
})
export class TemaBotao {
  protected readonly tema = inject(TemaService);
}
