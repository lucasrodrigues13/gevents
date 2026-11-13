import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

export interface ConfirmaDados {
  titulo: string;
  texto: string;
  confirmar: string;
}

@Component({
  selector: 'app-confirma-dialog',
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ dados.titulo }}</h2>
    <mat-dialog-content>{{ dados.texto }}</mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close(false)">Desistir</button>
      <button mat-flat-button type="button" (click)="ref.close(true)">{{ dados.confirmar }}</button>
    </mat-dialog-actions>
  `,
})
export class ConfirmaDialog {
  protected readonly dados = inject<ConfirmaDados>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<ConfirmaDialog, boolean>);
}
