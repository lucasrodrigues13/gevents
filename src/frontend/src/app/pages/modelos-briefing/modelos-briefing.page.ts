import { Component, inject, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Briefing, NegocioClient } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { DIALOG } from '../../shared/dialog';
import { GdDataPipe } from '../../shared/gd-data.pipe';

@Component({
  selector: 'app-briefing-editor-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule],
  template: `
    <h2 mat-dialog-title>{{ dados ? 'Editar briefing' : 'Novo briefing' }}</h2>
    <mat-dialog-content>
      @if (avisoPadrao()) {
        <p class="aviso">Modelo padrão: clone antes de editar.</p>
      }
      <form class="formulario" [formGroup]="form">
        <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="nome" [readonly]="avisoPadrao()" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Origem</mat-label><input matInput formControlName="origem" [readonly]="avisoPadrao()" /></mat-form-field>
        <div formArrayName="secoes">
          @for (secao of secoes.controls; track $index; let i = $index) {
            <div class="bloco" [formGroupName]="i">
              <div class="linha">
                <mat-form-field appearance="outline" class="grow"><mat-label>Seção</mat-label><input matInput formControlName="titulo" [readonly]="avisoPadrao()" /></mat-form-field>
                <button mat-icon-button type="button" (click)="removerSecao(i)" [disabled]="avisoPadrao()"><mat-icon>delete</mat-icon></button>
              </div>
              <mat-form-field appearance="outline" class="grow"><mat-label>Descrição</mat-label><textarea matInput rows="2" formControlName="descricao" [readonly]="avisoPadrao()"></textarea></mat-form-field>
              <div formArrayName="perguntas">
                @for (p of perguntasDe(i).controls; track $index; let j = $index) {
                  <div class="linha" [formGroupName]="j">
                    <mat-form-field appearance="outline" class="grow"><mat-label>Pergunta (texto curto)</mat-label><input matInput formControlName="enunciado" [readonly]="avisoPadrao()" /></mat-form-field>
                    <button mat-icon-button type="button" (click)="removerPergunta(i, j)" [disabled]="avisoPadrao()"><mat-icon>close</mat-icon></button>
                  </div>
                }
              </div>
              <button mat-stroked-button type="button" (click)="adicionarPergunta(i)" [disabled]="avisoPadrao()">Adicionar pergunta</button>
            </div>
          }
        </div>
        <button mat-stroked-button type="button" (click)="adicionarSecao()" [disabled]="avisoPadrao()">Adicionar seção</button>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      @if (avisoPadrao()) {
        <button mat-flat-button type="button" (click)="clonar()">Clonar</button>
      } @else {
        <button mat-flat-button type="button" (click)="salvar()">Salvar</button>
      }
    </mat-dialog-actions>
  `,
  styles: `
    .aviso { color: var(--gd-alerta, #c47f00); margin-bottom: 12px; }
    .bloco { border: 1px solid var(--gd-borda, #e0e0e0); border-radius: 8px; padding: 12px; margin: 8px 0; }
    .linha { display: flex; gap: 8px; align-items: flex-start; }
    .grow { flex: 1; }
  `,
})
export class BriefingEditorDialog {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly dados = inject<Briefing | null>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<BriefingEditorDialog, boolean>);
  protected readonly avisoPadrao = signal(!!this.dados?.padrao);
  protected readonly form = this.fb.group({
    nome: [this.dados?.nome ?? '', Validators.required],
    origem: [this.dados?.origem ?? 'GestaoDeEventos'],
    secoes: this.fb.array([]),
  });

  protected get secoes(): FormArray {
    return this.form.get('secoes') as FormArray;
  }

  constructor() {
    for (const secao of this.dados?.secoes ?? []) {
      const grupo = this.novaSecao(secao.titulo, secao.descricao ?? '');
      const perguntas = grupo.get('perguntas') as FormArray;
      for (const pergunta of secao.perguntas) {
        perguntas.push(this.fb.nonNullable.group({ enunciado: pergunta.enunciado, tipo: 'TextoCurto' }));
      }
      this.secoes.push(grupo);
    }
  }

  protected perguntasDe(i: number): FormArray {
    return this.secoes.at(i).get('perguntas') as FormArray;
  }

  protected adicionarSecao(): void {
    this.secoes.push(this.novaSecao('', ''));
  }

  protected removerSecao(i: number): void {
    this.secoes.removeAt(i);
  }

  protected adicionarPergunta(i: number): void {
    this.perguntasDe(i).push(this.fb.nonNullable.group({ enunciado: '', tipo: 'TextoCurto' }));
  }

  protected removerPergunta(i: number, j: number): void {
    this.perguntasDe(i).removeAt(j);
  }

  protected salvar(): void {
    const valor = this.form.getRawValue() as {
      nome: string | null;
      origem: string | null;
      secoes: { titulo: string; descricao: string; perguntas: { enunciado: string; tipo: string }[] }[];
    };
    const pedido = {
      nome: valor.nome,
      origem: valor.origem,
      secoes: (valor.secoes ?? []).map((secao) => ({
        titulo: secao.titulo,
        descricao: secao.descricao || null,
        perguntas: (secao.perguntas ?? [])
          .filter((p) => p.enunciado?.trim())
          .map((p) => ({ enunciado: p.enunciado, tipo: 'TextoCurto' })),
      })),
    };
    const req = this.dados
      ? this.negocio.atualizarBriefing(this.dados.id, pedido)
      : this.negocio.criarBriefing(pedido);
    req.subscribe({
      next: () => {
        this.toast.sucesso('Briefing salvo.');
        this.ref.close(true);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected clonar(): void {
    if (!this.dados) {
      return;
    }
    this.negocio.clonarBriefing(this.dados.id).subscribe({
      next: () => {
        this.toast.sucesso('Briefing clonado.');
        this.ref.close(true);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  private novaSecao(titulo: string, descricao: string) {
    return this.fb.group({
      titulo: [titulo, Validators.required],
      descricao: [descricao],
      perguntas: this.fb.array([]),
    });
  }
}

@Component({
  selector: 'app-modelos-briefing',
  imports: [MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule, GdDataPipe],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Modelos de briefing</h1>
        <p>Biblioteca de questionários para eventos.</p>
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
        <p class="gd-vazio">Não há modelo de briefing.</p>
      } @else {
        <table class="gd-grade" mat-table [dataSource]="lista()">
          <ng-container matColumnDef="nome">
            <th mat-header-cell *matHeaderCellDef>Nome</th>
            <td mat-cell *matCellDef="let item">
              {{ item.nome }}
              @if (item.padrao) { <span class="gd-sub">Padrão</span> }
            </td>
          </ng-container>
          <ng-container matColumnDef="origem">
            <th mat-header-cell *matHeaderCellDef>Origem</th>
            <td mat-cell *matCellDef="let item">{{ item.origem }}</td>
          </ng-container>
          <ng-container matColumnDef="data">
            <th mat-header-cell *matHeaderCellDef>Data</th>
            <td mat-cell *matCellDef="let item">{{ item.criadoEm | gdData }}</td>
          </ng-container>
          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
            <td mat-cell *matCellDef="let item" class="gd-acoes">
              <button mat-icon-button type="button" aria-label="Editar" (click)="abrir(item)"><mat-icon>edit</mat-icon></button>
              <button mat-icon-button type="button" aria-label="Clonar" (click)="clonar(item)"><mat-icon>content_copy</mat-icon></button>
              <button mat-icon-button type="button" aria-label="Excluir" (click)="excluir(item)" [disabled]="item.padrao"><mat-icon>delete</mat-icon></button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="colunas"></tr>
          <tr mat-row *matRowDef="let row; columns: colunas"></tr>
        </table>
      }
    </section>
  `,
})
export class ModelosBriefingPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly lista = signal<Briefing[]>([]);
  protected readonly busca = signal('');
  protected readonly colunas = ['nome', 'origem', 'data', 'acoes'];

  ngOnInit(): void {
    this.carregar();
  }

  protected buscar(evento: Event): void {
    this.busca.set((evento.target as HTMLInputElement).value);
    this.carregar();
  }

  protected abrir(item?: Briefing): void {
    const dados = item ?? null;
    if (item) {
      this.negocio.briefing(item.id).subscribe({
        next: (detalhe) => this.dialog.open(BriefingEditorDialog, { ...DIALOG, data: detalhe }).afterClosed().subscribe((ok) => ok && this.carregar()),
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
      return;
    }
    this.dialog.open(BriefingEditorDialog, { ...DIALOG, data: dados }).afterClosed().subscribe((ok) => ok && this.carregar());
  }

  protected clonar(item: Briefing): void {
    this.negocio.clonarBriefing(item.id).subscribe({
      next: () => {
        this.toast.sucesso('Briefing clonado.');
        this.carregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected excluir(item: Briefing): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir briefing', texto: `Confirma a exclusão de "${item.nome}"?`, confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (!confirmou) {
        return;
      }
      this.negocio.excluirBriefing(item.id).subscribe({
        next: () => {
          this.toast.sucesso('Briefing excluído.');
          this.carregar();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  private carregar(): void {
    this.negocio.briefings(this.busca()).subscribe({
      next: (lista) => this.lista.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
