import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { FornecedorResumo, NegocioClient, ServicoConta } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { DIALOG } from '../../shared/dialog';
import { FornecedorFormDialog } from './fornecedor-form.dialog';

@Component({
  selector: 'app-novo-servico-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  template: `
    <h2 mat-dialog-title>Novo serviço</h2>
    <mat-dialog-content>
      <mat-form-field appearance="outline" class="full">
        <mat-label>Nome</mat-label>
        <input matInput [formControl]="nome" />
      </mat-form-field>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="salvar()">Criar</button>
    </mat-dialog-actions>
  `,
  styles: `.full { width: 100%; display: block; margin-top: 8px; }`,
})
export class NovoServicoDialog {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly ref = inject(MatDialogRef<NovoServicoDialog, boolean>);
  protected readonly nome = this.fb.nonNullable.control('', Validators.required);

  protected salvar(): void {
    if (this.nome.invalid) {
      this.nome.markAsTouched();
      return;
    }
    this.negocio.criarServicoConta(this.nome.value).subscribe({
      next: () => {
        this.toast.sucesso('Serviço criado.');
        this.ref.close(true);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}

@Component({
  selector: 'app-fornecedores',
  imports: [MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Serviços e fornecedores</h1>
        <p>Catálogo da conta e lista de serviços.</p>
      </div>
      <button mat-flat-button type="button" (click)="abrirFornecedor()">
        <mat-icon>add</mat-icon>
        Novo fornecedor
      </button>
    </header>

    <section class="gd-painel">
      <div class="gd-abas">
        <button type="button" [class.ativa]="aba() === 'fornecedores'" (click)="aba.set('fornecedores')">Fornecedores</button>
        <button type="button" [class.ativa]="aba() === 'servicos'" (click)="aba.set('servicos')">Serviços da conta</button>
      </div>

      @if (aba() === 'fornecedores') {
        <div class="gd-barra">
          <span></span>
          <mat-form-field appearance="outline" subscriptSizing="dynamic">
            <mat-label>Buscar por nome</mat-label>
            <input matInput [value]="buscaForn()" (input)="buscarForn($event)" />
          </mat-form-field>
        </div>
        @if (fornecedores().length === 0) {
          <p class="gd-vazio">Não há fornecedor cadastrado.</p>
        } @else {
          <table class="gd-grade" mat-table [dataSource]="fornecedores()">
            <ng-container matColumnDef="nome">
              <th mat-header-cell *matHeaderCellDef>Nome</th>
              <td mat-cell *matCellDef="let item">
                {{ item.nome }}
                <span class="gd-sub">{{ item.completo ? 'Completo' : 'Incompleto' }}</span>
              </td>
            </ng-container>
            <ng-container matColumnDef="servicos">
              <th mat-header-cell *matHeaderCellDef>Serviços</th>
              <td mat-cell *matCellDef="let item">{{ item.servicos.join(', ') || '—' }}</td>
            </ng-container>
            <ng-container matColumnDef="acoes">
              <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
              <td mat-cell *matCellDef="let item" class="gd-acoes">
                <button mat-icon-button type="button" aria-label="Editar" (click)="abrirFornecedor(item)"><mat-icon>edit</mat-icon></button>
                <button mat-icon-button type="button" aria-label="Excluir" (click)="excluirFornecedor(item)"><mat-icon>delete</mat-icon></button>
              </td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="colsForn"></tr>
            <tr mat-row *matRowDef="let row; columns: colsForn"></tr>
          </table>
        }
      } @else {
        <div class="gd-barra">
          <button mat-stroked-button type="button" (click)="criarServico()"><mat-icon>add</mat-icon> Adicionar serviço</button>
          <mat-form-field appearance="outline" subscriptSizing="dynamic">
            <mat-label>Buscar</mat-label>
            <input matInput [value]="buscaServ()" (input)="buscarServ($event)" />
          </mat-form-field>
        </div>
        @if (servicos().length === 0) {
          <p class="gd-vazio">Não há serviço da conta.</p>
        } @else {
          <table class="gd-grade" mat-table [dataSource]="servicos()">
            <ng-container matColumnDef="nome">
              <th mat-header-cell *matHeaderCellDef>Nome</th>
              <td mat-cell *matCellDef="let item">
                {{ item.nome }}
                @if (item.padrao) { <span class="gd-sub">Padrão</span> }
              </td>
            </ng-container>
            <ng-container matColumnDef="acoes">
              <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
              <td mat-cell *matCellDef="let item" class="gd-acoes">
                <button mat-icon-button type="button" aria-label="Excluir" (click)="excluirServico(item)"><mat-icon>delete</mat-icon></button>
              </td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="colsServ"></tr>
            <tr mat-row *matRowDef="let row; columns: colsServ"></tr>
          </table>
        }
      }
    </section>
  `,
})
export class FornecedoresPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly aba = signal<'fornecedores' | 'servicos'>('fornecedores');
  protected readonly fornecedores = signal<FornecedorResumo[]>([]);
  protected readonly servicos = signal<ServicoConta[]>([]);
  protected readonly buscaForn = signal('');
  protected readonly buscaServ = signal('');
  protected readonly colsForn = ['nome', 'servicos', 'acoes'];
  protected readonly colsServ = ['nome', 'acoes'];

  ngOnInit(): void {
    this.carregarFornecedores();
    this.carregarServicos();
  }

  protected buscarForn(evento: Event): void {
    this.buscaForn.set((evento.target as HTMLInputElement).value);
    this.carregarFornecedores();
  }

  protected buscarServ(evento: Event): void {
    this.buscaServ.set((evento.target as HTMLInputElement).value);
    this.carregarServicos();
  }

  protected abrirFornecedor(resumo?: FornecedorResumo): void {
    if (resumo) {
      this.negocio.fornecedor(resumo.id).subscribe({
        next: (detalhe) => this.dialog.open(FornecedorFormDialog, { ...DIALOG, data: detalhe }).afterClosed().subscribe((ok) => ok && this.carregarFornecedores()),
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
      return;
    }
    this.dialog.open(FornecedorFormDialog, { ...DIALOG, data: null }).afterClosed().subscribe((ok) => {
      if (ok) {
        this.carregarFornecedores();
        this.carregarServicos();
      }
    });
  }

  protected excluirFornecedor(item: FornecedorResumo): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir fornecedor', texto: `Confirma a exclusão de "${item.nome}"?`, confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (!confirmou) {
        return;
      }
      this.negocio.excluirFornecedor(item.id).subscribe({
        next: () => {
          this.toast.sucesso('Fornecedor excluído.');
          this.carregarFornecedores();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  protected criarServico(): void {
    this.dialog.open(NovoServicoDialog, { ...DIALOG, width: 'min(420px, calc(100vw - 32px))' }).afterClosed().subscribe((ok) => ok && this.carregarServicos());
  }

  protected excluirServico(item: ServicoConta): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir serviço', texto: `Confirma a exclusão de "${item.nome}"?`, confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (!confirmou) {
        return;
      }
      this.negocio.excluirServicoConta(item.id).subscribe({
        next: () => {
          this.toast.sucesso('Serviço excluído.');
          this.carregarServicos();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  private carregarFornecedores(): void {
    this.negocio.fornecedores(this.buscaForn()).subscribe({
      next: (lista) => this.fornecedores.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  private carregarServicos(): void {
    this.negocio.servicosConta(this.buscaServ()).subscribe({
      next: (lista) => this.servicos.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
