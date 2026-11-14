import { Component, inject, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient, Roteiro } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { DIALOG } from '../../shared/dialog';

@Component({
  selector: 'app-roteiro-editor-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule],
  template: `
    <h2 mat-dialog-title>{{ dados ? 'Editar roteiro' : 'Novo roteiro' }}</h2>
    <mat-dialog-content>
      <form class="formulario" [formGroup]="form">
        <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="nome" /></mat-form-field>
        <div formArrayName="secoes">
          @for (secao of secoes.controls; track $index; let i = $index) {
            <div class="bloco" [formGroupName]="i">
              <div class="linha">
                <mat-form-field appearance="outline" class="grow"><mat-label>Título</mat-label><input matInput formControlName="titulo" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Horário</mat-label><input matInput type="time" formControlName="horario" /></mat-form-field>
                <button mat-icon-button type="button" (click)="removerSecao(i)"><mat-icon>delete</mat-icon></button>
              </div>
              <mat-form-field appearance="outline" class="grow"><mat-label>Música</mat-label><input matInput formControlName="musica" /></mat-form-field>
              <div formArrayName="itens">
                @for (item of itensDe(i).controls; track $index; let j = $index) {
                  <div class="linha" [formGroupName]="j">
                    <mat-form-field appearance="outline" class="grow"><mat-label>Item</mat-label><input matInput formControlName="texto" /></mat-form-field>
                    <button mat-icon-button type="button" (click)="removerItem(i, j)"><mat-icon>close</mat-icon></button>
                  </div>
                }
              </div>
              <button mat-stroked-button type="button" (click)="adicionarItem(i)">Adicionar item</button>
            </div>
          }
        </div>
        <button mat-stroked-button type="button" (click)="adicionarSecao()">Adicionar seção</button>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="salvar()">Salvar</button>
    </mat-dialog-actions>
  `,
  styles: `
    .bloco { border: 1px solid var(--gd-borda, #e0e0e0); border-radius: 8px; padding: 12px; margin: 8px 0; }
    .linha { display: flex; gap: 8px; align-items: flex-start; }
    .grow { flex: 1; }
  `,
})
export class RoteiroEditorDialog {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly dados = inject<Roteiro | null>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<RoteiroEditorDialog, boolean>);
  protected readonly form = this.fb.group({
    nome: [this.dados?.nome ?? '', Validators.required],
    secoes: this.fb.array([]),
  });

  protected get secoes(): FormArray {
    return this.form.get('secoes') as FormArray;
  }

  constructor() {
    for (const secao of this.dados?.secoes ?? []) {
      const grupo = this.novaSecao(secao.titulo ?? '', (secao.horario ?? '').slice(0, 5), secao.musica ?? '');
      const itens = grupo.get('itens') as FormArray;
      for (const item of secao.itens) {
        itens.push(this.fb.nonNullable.group({ texto: item.texto }));
      }
      this.secoes.push(grupo);
    }
  }

  protected itensDe(i: number): FormArray {
    return this.secoes.at(i).get('itens') as FormArray;
  }

  protected adicionarSecao(): void {
    this.secoes.push(this.novaSecao('', '', ''));
  }

  protected removerSecao(i: number): void {
    this.secoes.removeAt(i);
  }

  protected adicionarItem(i: number): void {
    this.itensDe(i).push(this.fb.nonNullable.group({ texto: '' }));
  }

  protected removerItem(i: number, j: number): void {
    this.itensDe(i).removeAt(j);
  }

  protected salvar(): void {
    const valor = this.form.getRawValue() as {
      nome: string | null;
      secoes: { titulo: string; horario: string; musica: string; itens: { texto: string }[] }[];
    };
    const pedido = {
      nome: valor.nome,
      secoes: (valor.secoes ?? []).map((secao) => ({
        titulo: secao.titulo || null,
        horario: secao.horario ? `${secao.horario}:00` : null,
        musica: secao.musica || null,
        itens: (secao.itens ?? [])
          .filter((item) => item.texto?.trim())
          .map((item) => ({ texto: item.texto })),
      })),
    };
    const req = this.dados
      ? this.negocio.atualizarRoteiro(this.dados.id, pedido)
      : this.negocio.criarRoteiro(pedido);
    req.subscribe({
      next: () => {
        this.toast.sucesso('Roteiro salvo.');
        this.ref.close(true);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  private novaSecao(titulo: string, horario: string, musica: string) {
    return this.fb.group({
      titulo: [titulo],
      horario: [horario],
      musica: [musica],
      itens: this.fb.array([]),
    });
  }
}

@Component({
  selector: 'app-modelos-roteiro',
  imports: [MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Modelos de roteiro</h1>
        <p>Biblioteca de roteiros do dia.</p>
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
          <mat-label>Buscar por nome</mat-label>
          <input matInput [value]="busca()" (input)="buscar($event)" />
        </mat-form-field>
      </div>
      @if (lista().length === 0) {
        <p class="gd-vazio">Não há modelo de roteiro.</p>
      } @else {
        <table class="gd-grade" mat-table [dataSource]="lista()">
          <ng-container matColumnDef="nome">
            <th mat-header-cell *matHeaderCellDef>Nome</th>
            <td mat-cell *matCellDef="let item">{{ item.nome }}</td>
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
export class ModelosRoteiroPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly lista = signal<Roteiro[]>([]);
  protected readonly busca = signal('');
  protected readonly colunas = ['nome', 'acoes'];

  ngOnInit(): void {
    this.carregar();
  }

  protected buscar(evento: Event): void {
    this.busca.set((evento.target as HTMLInputElement).value);
    this.carregar();
  }

  protected abrir(item?: Roteiro): void {
    if (item) {
      this.negocio.roteiro(item.id).subscribe({
        next: (detalhe) => this.dialog.open(RoteiroEditorDialog, { ...DIALOG, data: detalhe }).afterClosed().subscribe((ok) => ok && this.carregar()),
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
      return;
    }
    this.dialog.open(RoteiroEditorDialog, { ...DIALOG, data: null }).afterClosed().subscribe((ok) => ok && this.carregar());
  }

  protected clonar(item: Roteiro): void {
    this.negocio.clonarRoteiro(item.id).subscribe({
      next: () => {
        this.toast.sucesso('Roteiro clonado.');
        this.carregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected excluir(item: Roteiro): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir roteiro', texto: `Confirma a exclusão de "${item.nome}"?`, confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (!confirmou) {
        return;
      }
      this.negocio.excluirRoteiro(item.id).subscribe({
        next: () => {
          this.toast.sucesso('Roteiro excluído.');
          this.carregar();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  private carregar(): void {
    this.negocio.roteiros(this.busca()).subscribe({
      next: (lista) => this.lista.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
