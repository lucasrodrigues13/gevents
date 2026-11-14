import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { Observable } from 'rxjs';
import { mensagemDeErro } from '../../core/mensagem-erro';
import {
  ModeloContratoFunil,
  ModeloOrcamentoFunil,
  ModeloServicoFunil,
  NegocioClient,
} from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { DIALOG } from '../../shared/dialog';

type TipoModelo = 'servico' | 'orcamento' | 'contrato';

@Component({
  selector: 'app-modelo-funil-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  template: `
    <h2 mat-dialog-title>{{ dados.item ? 'Editar' : 'Novo' }} modelo</h2>
    <mat-dialog-content>
      <form class="formulario" [formGroup]="form">
        <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="nome" /></mat-form-field>
        @if (dados.tipo === 'servico') {
          <mat-form-field appearance="outline"><mat-label>Descrição</mat-label><textarea matInput rows="3" formControlName="descricao" maxlength="200"></textarea></mat-form-field>
          <mat-form-field appearance="outline"><mat-label>Valor</mat-label><input matInput type="number" formControlName="valor" /></mat-form-field>
        } @else {
          <mat-form-field appearance="outline"><mat-label>Texto</mat-label><textarea matInput rows="8" formControlName="texto" #area></textarea></mat-form-field>
          <div class="params">
            @for (p of parametros; track p) {
              <button mat-stroked-button type="button" (click)="inserir(p, area)">{{ p }}</button>
            }
          </div>
          @if (dados.tipo === 'orcamento') {
            <mat-form-field appearance="outline"><mat-label>Cor primária</mat-label><input matInput formControlName="corPrimaria" placeholder="#112233" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Cor secundária</mat-label><input matInput formControlName="corSecundaria" placeholder="#aabbcc" /></mat-form-field>
          }
        }
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="salvar()">Salvar</button>
    </mat-dialog-actions>
  `,
  styles: `.params { display: flex; flex-wrap: wrap; gap: 6px; margin: 8px 0; }`,
})
export class ModeloFunilDialog {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly dados = inject<{ tipo: TipoModelo; item: ModeloServicoFunil | ModeloOrcamentoFunil | ModeloContratoFunil | null }>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<ModeloFunilDialog, boolean>);
  protected readonly parametros = ['#nome_oportunidade', '#data_prevista', '#horario', '#local_cerimonia', '#local_recepcao', '#quantidade_convidados', '#valor_proposta'];
  protected readonly form = this.fb.nonNullable.group({
    nome: [(this.dados.item as { nome?: string } | null)?.nome ?? '', Validators.required],
    descricao: [(this.dados.item as ModeloServicoFunil | null)?.descricao ?? ''],
    valor: [(this.dados.item as ModeloServicoFunil | null)?.valor ?? null as number | null],
    texto: [(this.dados.item as ModeloOrcamentoFunil | ModeloContratoFunil | null)?.texto ?? ''],
    corPrimaria: [(this.dados.item as ModeloOrcamentoFunil | null)?.corPrimaria ?? ''],
    corSecundaria: [(this.dados.item as ModeloOrcamentoFunil | null)?.corSecundaria ?? ''],
  });

  protected inserir(param: string, area: HTMLTextAreaElement): void {
    const atual = this.form.controls.texto.value;
    const inicio = area.selectionStart ?? atual.length;
    const fim = area.selectionEnd ?? atual.length;
    this.form.controls.texto.setValue(atual.slice(0, inicio) + param + atual.slice(fim));
  }

  protected salvar(): void {
    if (this.form.controls.nome.invalid || (this.dados.tipo !== 'servico' && !this.form.controls.texto.value.trim())) {
      this.form.markAllAsTouched();
      this.toast.erro('Confira os campos obrigatórios.');
      return;
    }
    const v = this.form.getRawValue();
    const id = this.dados.item?.id;
    let req: Observable<ModeloServicoFunil | ModeloOrcamentoFunil | ModeloContratoFunil>;
    if (this.dados.tipo === 'servico') {
      const pedido = { nome: v.nome, descricao: v.descricao || null, valor: v.valor };
      req = id ? this.negocio.atualizarModeloServicoFunil(id, pedido) : this.negocio.criarModeloServicoFunil(pedido);
    } else if (this.dados.tipo === 'orcamento') {
      const pedido = { nome: v.nome, texto: v.texto, corPrimaria: v.corPrimaria || null, corSecundaria: v.corSecundaria || null };
      req = id ? this.negocio.atualizarModeloOrcamentoFunil(id, pedido) : this.negocio.criarModeloOrcamentoFunil(pedido);
    } else {
      const pedido = { nome: v.nome, texto: v.texto };
      req = id ? this.negocio.atualizarModeloContratoFunil(id, pedido) : this.negocio.criarModeloContratoFunil(pedido);
    }
    req.subscribe({
      next: () => {
        this.toast.sucesso('Modelo salvo.');
        this.ref.close(true);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}

@Component({
  selector: 'app-modelos-funil',
  imports: [MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>{{ titulo }}</h1>
        <p>{{ apoio }}</p>
      </div>
      <button mat-flat-button type="button" (click)="abrir()"><mat-icon>add</mat-icon> Novo</button>
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
        <p class="gd-vazio">{{ vazio }}</p>
      } @else {
        <table class="gd-grade" mat-table [dataSource]="lista()">
          <ng-container matColumnDef="nome">
            <th mat-header-cell *matHeaderCellDef>Nome</th>
            <td mat-cell *matCellDef="let item">{{ item.nome }}</td>
          </ng-container>
          <ng-container matColumnDef="detalhe">
            <th mat-header-cell *matHeaderCellDef>Detalhe</th>
            <td mat-cell *matCellDef="let item"><span class="gd-sub">{{ detalhe(item) }}</span></td>
          </ng-container>
          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
            <td mat-cell *matCellDef="let item" class="gd-acoes">
              <button mat-icon-button type="button" (click)="abrir(item)"><mat-icon>edit</mat-icon></button>
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
export class ModelosFunilPage implements OnInit {
  @Input({ required: true }) tipo!: TipoModelo;
  private readonly negocio = inject(NegocioClient);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly lista = signal<Array<ModeloServicoFunil | ModeloOrcamentoFunil | ModeloContratoFunil>>([]);
  protected readonly busca = signal('');
  protected readonly colunas = ['nome', 'detalhe', 'acoes'];

  get titulo(): string {
    return this.tipo === 'servico'
      ? 'Modelos de Serviços/Produtos'
      : this.tipo === 'orcamento'
        ? 'Modelo de orçamento'
        : 'Modelos de contrato';
  }

  get apoio(): string {
    return this.tipo === 'servico'
      ? 'Catálogo usado nas oportunidades do funil.'
      : this.tipo === 'orcamento'
        ? 'Biblioteca de textos para orçamentos do funil.'
        : 'Biblioteca de textos para contratos do funil.';
  }

  get vazio(): string {
    return this.tipo === 'servico' ? 'Não há serviço/produto cadastrado.' : 'Não há modelo cadastrado.';
  }

  ngOnInit(): void {
    this.carregar();
  }

  protected detalhe(item: ModeloServicoFunil | ModeloOrcamentoFunil | ModeloContratoFunil): string {
    if ('descricao' in item) return item.descricao ?? '';
    if ('texto' in item) return item.texto;
    return '';
  }

  protected buscar(evento: Event): void {
    this.busca.set((evento.target as HTMLInputElement).value);
    this.carregar();
  }

  protected abrir(item?: ModeloServicoFunil | ModeloOrcamentoFunil | ModeloContratoFunil): void {
    this.dialog
      .open(ModeloFunilDialog, { ...DIALOG, data: { tipo: this.tipo, item: item ?? null } })
      .afterClosed()
      .subscribe((ok) => ok && this.carregar());
  }

  protected excluir(item: { id: string; nome: string }): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir modelo', texto: `Confirma a exclusão de "${item.nome}"?`, confirmar: 'Excluir' },
    }).afterClosed().subscribe((ok) => {
      if (!ok) return;
      const req: Observable<unknown> =
        this.tipo === 'servico'
          ? this.negocio.excluirModeloServicoFunil(item.id)
          : this.tipo === 'orcamento'
            ? this.negocio.excluirModeloOrcamentoFunil(item.id)
            : this.negocio.excluirModeloContratoFunil(item.id);
      req.subscribe({
        next: () => {
          this.toast.sucesso('Modelo excluído.');
          this.carregar();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  private carregar(): void {
    const req: Observable<Array<ModeloServicoFunil | ModeloOrcamentoFunil | ModeloContratoFunil>> =
      this.tipo === 'servico'
        ? this.negocio.modelosServicoFunil(this.busca())
        : this.tipo === 'orcamento'
          ? this.negocio.modelosOrcamentoFunil(this.busca())
          : this.negocio.modelosContratoFunil(this.busca());
    req.subscribe({
      next: (lista) => this.lista.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}

@Component({ selector: 'app-modelos-servicos-funil', imports: [ModelosFunilPage], template: `<app-modelos-funil tipo="servico" />` })
export class ModelosServicosFunilPage {}

@Component({ selector: 'app-modelos-orcamento-funil', imports: [ModelosFunilPage], template: `<app-modelos-funil tipo="orcamento" />` })
export class ModelosOrcamentoFunilPage {}

@Component({ selector: 'app-modelos-contrato-funil', imports: [ModelosFunilPage], template: `<app-modelos-funil tipo="contrato" />` })
export class ModelosContratoFunilPage {}
