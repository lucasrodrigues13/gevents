import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient, Oportunidade, TarefaFunil, TIPOS_TAREFA_FUNIL } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { CampoDataComponent } from '../../shared/campo-data';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { deDataParaIso, deIsoParaData } from '../../shared/data';
import { DIALOG } from '../../shared/dialog';
import { GdDataPipe } from '../../shared/gd-data.pipe';

@Component({
  selector: 'app-tarefa-funil-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, CampoDataComponent],
  template: `
    <h2 mat-dialog-title>{{ dados ? 'Editar tarefa' : 'Nova tarefa' }}</h2>
    <mat-dialog-content>
      <form class="formulario" [formGroup]="form">
        <mat-form-field appearance="outline"><mat-label>Título</mat-label><input matInput formControlName="titulo" /></mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Oportunidade</mat-label>
          <mat-select formControlName="oportunidadeId">
            @for (o of oportunidades(); track o.id) { <mat-option [value]="o.id">{{ o.nome }}</mat-option> }
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Tipo</mat-label>
          <mat-select formControlName="tipo">
            @for (t of tipos; track t.valor) { <mat-option [value]="t.valor">{{ t.rotulo }}</mat-option> }
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Responsável</mat-label><input matInput formControlName="responsavel" /></mat-form-field>
        <app-campo-data formControlName="prazo" rotulo="Prazo" />
        <mat-form-field appearance="outline"><mat-label>Observação</mat-label><textarea matInput rows="3" formControlName="observacao"></textarea></mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="salvar()">Salvar</button>
    </mat-dialog-actions>
  `,
})
export class TarefaFunilDialog implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly dados = inject<TarefaFunil | null>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<TarefaFunilDialog, boolean>);
  protected readonly tipos = TIPOS_TAREFA_FUNIL;
  protected readonly oportunidades = signal<Oportunidade[]>([]);
  protected readonly form = this.fb.nonNullable.group({
    titulo: [this.dados?.titulo ?? '', Validators.required],
    oportunidadeId: [this.dados?.oportunidadeId ?? '', Validators.required],
    tipo: [this.dados?.tipo ?? 'Tarefa', Validators.required],
    responsavel: [this.dados?.responsavel ?? '', Validators.required],
    prazo: this.fb.control<Date | null>(deIsoParaData(this.dados?.prazo), Validators.required),
    observacao: [this.dados?.observacao ?? ''],
  });

  ngOnInit(): void {
    this.negocio.oportunidades().subscribe({ next: (lista) => this.oportunidades.set(lista) });
  }

  protected salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toast.erro('Preencha os campos obrigatórios.');
      return;
    }
    const v = this.form.getRawValue();
    const pedido = { ...v, prazo: deDataParaIso(v.prazo) };
    const req = this.dados
      ? this.negocio.atualizarTarefaFunil(this.dados.id, pedido)
      : this.negocio.criarTarefaFunil(pedido);
    req.subscribe({
      next: () => {
        this.toast.sucesso('Tarefa salva.');
        this.ref.close(true);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}

@Component({
  selector: 'app-tarefas-funil',
  imports: [MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule, MatSelectModule, GdDataPipe],
  template: `
    <header class="gd-cabecalho">
      <div><h1>Tarefas do funil</h1><p>Reuniões, ligações, e-mails e tarefas comerciais.</p></div>
      <button mat-flat-button type="button" (click)="abrir()"><mat-icon>add</mat-icon> Adicionar tarefa</button>
    </header>
    <section class="gd-painel">
      <div class="gd-barra">
        <div class="gd-abas">
          <button type="button" [class.ativa]="completa() === false" (click)="completa.set(false); carregar()">Incompletas</button>
          <button type="button" [class.ativa]="completa() === true" (click)="completa.set(true); carregar()">Completas</button>
        </div>
        <mat-form-field appearance="outline" subscriptSizing="dynamic">
          <mat-label>Tipo</mat-label>
          <mat-select [value]="tipo()" (selectionChange)="tipo.set($event.value); carregar()">
            <mat-option value="">Todos</mat-option>
            @for (t of tipos; track t.valor) { <mat-option [value]="t.valor">{{ t.rotulo }}</mat-option> }
          </mat-select>
        </mat-form-field>
      </div>
      @if (lista().length === 0) {
        <p class="gd-vazio">Não há tarefa.</p>
      } @else {
        <table class="gd-grade" mat-table [dataSource]="lista()">
          <ng-container matColumnDef="titulo">
            <th mat-header-cell *matHeaderCellDef>Título</th>
            <td mat-cell *matCellDef="let item">{{ item.titulo }}<span class="gd-sub">{{ item.oportunidadeNome }}</span></td>
          </ng-container>
          <ng-container matColumnDef="tipo">
            <th mat-header-cell *matHeaderCellDef>Tipo</th>
            <td mat-cell *matCellDef="let item">{{ item.tipo }}</td>
          </ng-container>
          <ng-container matColumnDef="prazo">
            <th mat-header-cell *matHeaderCellDef>Prazo</th>
            <td mat-cell *matCellDef="let item">{{ item.prazo | gdData }}</td>
          </ng-container>
          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
            <td mat-cell *matCellDef="let item" class="gd-acoes">
              <button mat-icon-button type="button" (click)="abrir(item)"><mat-icon>edit</mat-icon></button>
              @if (!item.completa) {
                <button mat-icon-button type="button" (click)="concluir(item)"><mat-icon>check</mat-icon></button>
              } @else {
                <button mat-icon-button type="button" (click)="reabrir(item)"><mat-icon>undo</mat-icon></button>
              }
              <button mat-icon-button type="button" (click)="excluir(item)"><mat-icon>delete</mat-icon></button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="colunas"></tr>
          <tr mat-row *matRowDef="let row; columns: colunas"></tr>
        </table>
      }
    </section>
  `,
})
export class TarefasFunilPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly tipos = TIPOS_TAREFA_FUNIL;
  protected readonly completa = signal(false);
  protected readonly tipo = signal('');
  protected readonly lista = signal<TarefaFunil[]>([]);
  protected readonly colunas = ['titulo', 'tipo', 'prazo', 'acoes'];

  ngOnInit(): void {
    this.carregar();
  }

  protected abrir(item?: TarefaFunil): void {
    this.dialog.open(TarefaFunilDialog, { ...DIALOG, data: item ?? null }).afterClosed().subscribe((ok) => ok && this.carregar());
  }

  protected concluir(item: TarefaFunil): void {
    this.negocio.concluirTarefaFunil(item.id).subscribe({
      next: () => { this.toast.sucesso('Tarefa concluída.'); this.carregar(); },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected reabrir(item: TarefaFunil): void {
    this.negocio.reabrirTarefaFunil(item.id).subscribe({
      next: () => { this.toast.sucesso('Tarefa reaberta.'); this.carregar(); },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected excluir(item: TarefaFunil): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir tarefa', texto: `Confirma a exclusão de "${item.titulo}"?`, confirmar: 'Excluir' },
    }).afterClosed().subscribe((ok) => {
      if (!ok) return;
      this.negocio.excluirTarefaFunil(item.id).subscribe({
        next: () => { this.toast.sucesso('Tarefa excluída.'); this.carregar(); },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  protected carregar(): void {
    this.negocio.tarefasFunil({
      completa: String(this.completa()),
      tipo: this.tipo() || undefined,
    }).subscribe({
      next: (lista) => this.lista.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
