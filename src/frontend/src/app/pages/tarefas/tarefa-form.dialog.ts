import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient, SITUACOES_TAREFA, Tarefa } from '../../core/negocio.client';

export interface TarefaFormDados {
  eventoId: string;
  tarefa?: Tarefa;
}

@Component({
  selector: 'app-tarefa-form',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  template: `
    <h2 mat-dialog-title>{{ dados.tarefa ? 'Editar tarefa' : 'Nova tarefa' }}</h2>
    <mat-dialog-content>
      <form id="tarefa-form" class="gd-dialog-miolo" [formGroup]="formulario" (ngSubmit)="salvar()">
        @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
        <mat-form-field appearance="outline"><mat-label>Título</mat-label><input matInput formControlName="titulo" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Descrição</mat-label><textarea matInput rows="2" formControlName="descricao"></textarea></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Prazo</mat-label><input matInput type="date" formControlName="prazo" /></mat-form-field>
        @if (dados.tarefa) {
          <mat-form-field appearance="outline">
            <mat-label>Situação</mat-label>
            <mat-select formControlName="situacao">
              @for (item of situacoes; track item.valor) { <mat-option [value]="item.valor">{{ item.rotulo }}</mat-option> }
            </mat-select>
          </mat-form-field>
        }
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close(false)">Cancelar</button>
      <button mat-flat-button type="submit" form="tarefa-form">Salvar</button>
    </mat-dialog-actions>
  `,
})
export class TarefaFormDialog {
  protected readonly dados = inject<TarefaFormDados>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<TarefaFormDialog, boolean>);
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  protected readonly situacoes = SITUACOES_TAREFA;
  protected readonly aviso = signal('');
  protected readonly formulario = this.fb.nonNullable.group({
    titulo: [this.dados.tarefa?.titulo ?? '', Validators.required],
    descricao: this.dados.tarefa?.descricao ?? '',
    prazo: this.dados.tarefa?.prazo ?? '',
    situacao: this.dados.tarefa?.situacao ?? 'Pendente',
  });
  protected salvar(): void {
    const valor = this.formulario.getRawValue();
    const pedido = {
      titulo: valor.titulo,
      descricao: valor.descricao,
      prazo: valor.prazo || null,
      situacao: valor.situacao,
    };
    const destino = this.dados.tarefa
      ? this.negocio.atualizarTarefa(this.dados.tarefa.id, pedido)
      : this.negocio.criarTarefa(this.dados.eventoId, pedido);
    destino.subscribe({
      next: () => this.ref.close(true),
      error: (erro: unknown) => {
        this.aviso.set(mensagemDeErro(erro));
      },
    });
  }
}
