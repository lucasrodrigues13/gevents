import { Component, inject, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Cronograma, NegocioClient } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { CampoDataComponent } from '../../shared/campo-data';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { deDataParaIso, deIsoParaData } from '../../shared/data';
import { DIALOG } from '../../shared/dialog';

@Component({
  selector: 'app-cronograma-editor-dialog',
  imports: [
    ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule,
    CampoDataComponent,
  ],
  template: `
    <h2 mat-dialog-title>{{ dados ? 'Editar cronograma' : 'Novo cronograma' }}</h2>
    <mat-dialog-content>
      <form class="formulario" [formGroup]="form">
        <mat-form-field appearance="outline"><mat-label>Título</mat-label><input matInput formControlName="titulo" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Descrição</mat-label><textarea matInput rows="2" formControlName="descricao"></textarea></mat-form-field>
        <div formArrayName="itens">
          @for (item of itens.controls; track $index; let i = $index) {
            <div class="bloco" [formGroupName]="i">
              <div class="linha">
                <mat-form-field appearance="outline" class="grow"><mat-label>Rótulo</mat-label><input matInput formControlName="rotulo" /></mat-form-field>
                <button mat-icon-button type="button" (click)="removerItem(i)"><mat-icon>delete</mat-icon></button>
              </div>
              <div class="linha">
                <app-campo-data rotulo="Data" formControlName="data" />
                <mat-form-field appearance="outline"><mat-label>Hora</mat-label><input matInput type="time" formControlName="hora" /></mat-form-field>
              </div>
              <mat-form-field appearance="outline" class="grow"><mat-label>Responsável</mat-label><input matInput formControlName="responsavel" /></mat-form-field>
              <mat-form-field appearance="outline" class="grow"><mat-label>Recado fornecedores</mat-label><input matInput formControlName="recadoFornecedores" /></mat-form-field>
            </div>
          }
        </div>
        <button mat-stroked-button type="button" (click)="adicionarItem()">Adicionar item</button>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="salvar()">Salvar</button>
    </mat-dialog-actions>
  `,
  styles: `
    .bloco { border: 1px solid var(--gd-borda, #e0e0e0); border-radius: 8px; padding: 12px; margin: 8px 0; }
    .linha { display: flex; gap: 8px; align-items: flex-start; flex-wrap: wrap; }
    .grow { flex: 1; min-width: 160px; }
  `,
})
export class CronogramaEditorDialog {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly dados = inject<Cronograma | null>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<CronogramaEditorDialog, boolean>);
  protected readonly form = this.fb.group({
    titulo: [this.dados?.titulo ?? '', Validators.required],
    descricao: [this.dados?.descricao ?? ''],
    itens: this.fb.array([]),
  });

  protected get itens(): FormArray {
    return this.form.get('itens') as FormArray;
  }

  constructor() {
    for (const item of this.dados?.itens ?? []) {
      this.itens.push(this.fb.group({
        rotulo: [item.rotulo ?? ''],
        data: [deIsoParaData(item.data)],
        hora: [(item.hora ?? '').slice(0, 5)],
        responsavel: [item.responsavel ?? ''],
        recadoFornecedores: [item.recadoFornecedores ?? ''],
      }));
    }
  }

  protected adicionarItem(): void {
    this.itens.push(this.fb.group({
      rotulo: [''],
      data: [null as Date | null],
      hora: [''],
      responsavel: [''],
      recadoFornecedores: [''],
    }));
  }

  protected removerItem(i: number): void {
    this.itens.removeAt(i);
  }

  protected salvar(): void {
    const valor = this.form.getRawValue() as {
      titulo: string | null;
      descricao: string | null;
      itens: {
        rotulo: string;
        data: Date | null;
        hora: string;
        responsavel: string;
        recadoFornecedores: string;
      }[];
    };
    const pedido = {
      titulo: valor.titulo,
      descricao: valor.descricao || null,
      itens: (valor.itens ?? []).map((item) => ({
        rotulo: item.rotulo || null,
        data: deDataParaIso(item.data),
        hora: item.hora ? `${item.hora}:00` : null,
        responsavel: item.responsavel || null,
        recadoFornecedores: item.recadoFornecedores || null,
      })),
    };
    const req = this.dados
      ? this.negocio.atualizarCronograma(this.dados.id, pedido)
      : this.negocio.criarCronograma(pedido);
    req.subscribe({
      next: () => {
        this.toast.sucesso('Cronograma salvo.');
        this.ref.close(true);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}

@Component({
  selector: 'app-modelos-cronograma',
  imports: [MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Modelos de cronograma</h1>
        <p>Biblioteca de cronogramas da conta.</p>
      </div>
      <button mat-flat-button type="button" (click)="abrir()">
        <mat-icon>add</mat-icon>
        Criar novo modelo
      </button>
    </header>
    <section class="gd-painel">
      <div class="gd-barra">
        <span></span>
        <mat-form-field appearance="outline" subscriptSizing="dynamic">
          <mat-label>Buscar</mat-label>
          <input matInput [value]="busca()" (input)="buscar($event)" />
        </mat-form-field>
      </div>
      @if (lista().length === 0) {
        <p class="gd-vazio">Não há modelo de cronograma.</p>
      } @else {
        <table class="gd-grade" mat-table [dataSource]="lista()">
          <ng-container matColumnDef="titulo">
            <th mat-header-cell *matHeaderCellDef>Título</th>
            <td mat-cell *matCellDef="let item">
              {{ item.titulo }}
              @if (item.descricao) { <span class="gd-sub">{{ item.descricao }}</span> }
            </td>
          </ng-container>
          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
            <td mat-cell *matCellDef="let item" class="gd-acoes">
              <button mat-icon-button type="button" aria-label="Editar" (click)="abrir(item)"><mat-icon>edit</mat-icon></button>
              <button mat-icon-button type="button" aria-label="Clonar" (click)="clonar(item)"><mat-icon>content_copy</mat-icon></button>
              <button mat-icon-button type="button" aria-label="Excluir" (click)="excluir(item)"><mat-icon>delete</mat-icon></button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="colunas"></tr>
          <tr mat-row *matRowDef="let row; columns: colunas"></tr>
        </table>
      }
    </section>
  `,
})
export class ModelosCronogramaPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly lista = signal<Cronograma[]>([]);
  protected readonly busca = signal('');
  protected readonly colunas = ['titulo', 'acoes'];

  ngOnInit(): void {
    this.carregar();
  }

  protected buscar(evento: Event): void {
    this.busca.set((evento.target as HTMLInputElement).value);
    this.carregar();
  }

  protected abrir(item?: Cronograma): void {
    if (item) {
      this.negocio.cronograma(item.id).subscribe({
        next: (detalhe) => this.dialog.open(CronogramaEditorDialog, { ...DIALOG, data: detalhe }).afterClosed().subscribe((ok) => ok && this.carregar()),
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
      return;
    }
    this.dialog.open(CronogramaEditorDialog, { ...DIALOG, data: null }).afterClosed().subscribe((ok) => ok && this.carregar());
  }

  protected clonar(item: Cronograma): void {
    this.negocio.clonarCronograma(item.id).subscribe({
      next: () => {
        this.toast.sucesso('Cronograma clonado.');
        this.carregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected excluir(item: Cronograma): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir cronograma', texto: `Confirma a exclusão de "${item.titulo}"?`, confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (!confirmou) {
        return;
      }
      this.negocio.excluirCronograma(item.id).subscribe({
        next: () => {
          this.toast.sucesso('Cronograma excluído.');
          this.carregar();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  private carregar(): void {
    this.negocio.cronogramas(this.busca()).subscribe({
      next: (lista) => this.lista.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
