import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Modelo, NegocioClient, TIPOS } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { DIALOG } from '../../shared/dialog';

@Component({
  selector: 'app-criar-modelo-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  template: `
    <h2 mat-dialog-title>Criar modelo</h2>
    <mat-dialog-content>
      <form class="formulario" [formGroup]="form" (ngSubmit)="salvar()">
        <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="nome" /></mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Tipo</mat-label>
          <mat-select formControlName="tipo">
            @for (tipo of tipos; track tipo.valor) {
              <mat-option [value]="tipo.valor">{{ tipo.rotulo }}</mat-option>
            }
          </mat-select>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="salvar()">Criar</button>
    </mat-dialog-actions>
  `,
})
export class CriarModeloDialog {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly ref = inject(MatDialogRef<CriarModeloDialog, Modelo>);
  protected readonly tipos = TIPOS;
  protected readonly form = this.fb.nonNullable.group({
    nome: ['', Validators.required],
    tipo: ['Casamento', Validators.required],
  });

  protected salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const valor = this.form.getRawValue();
    this.negocio.criarModelo({ nome: valor.nome, tipo: valor.tipo }).subscribe({
      next: (modelo) => {
        this.toast.sucesso('Modelo criado.');
        this.ref.close(modelo);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}

@Component({
  selector: 'app-modelos-evento',
  imports: [RouterLink, MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Modelos de evento</h1>
        <p>Biblioteca de modelos para criar eventos.</p>
      </div>
      <button mat-flat-button type="button" (click)="criar()">
        <mat-icon>add</mat-icon>
        Criar modelo
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
      @if (modelos().length === 0) {
        <p class="gd-vazio">Não há modelo cadastrado.</p>
      } @else {
        <table class="gd-grade" mat-table [dataSource]="modelos()">
          <ng-container matColumnDef="nome">
            <th mat-header-cell *matHeaderCellDef>Nome</th>
            <td mat-cell *matCellDef="let m">
              <a [routerLink]="['/modelos-evento', m.id]">{{ m.nome }}</a>
            </td>
          </ng-container>
          <ng-container matColumnDef="tipo">
            <th mat-header-cell *matHeaderCellDef>Tipo</th>
            <td mat-cell *matCellDef="let m">{{ rotuloTipo(m.tipo) }}</td>
          </ng-container>
          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
            <td mat-cell *matCellDef="let m" class="gd-acoes">
              <a mat-icon-button aria-label="Editar" [routerLink]="['/modelos-evento', m.id]"><mat-icon>edit</mat-icon></a>
              <button mat-icon-button type="button" aria-label="Clonar" (click)="clonar(m)"><mat-icon>content_copy</mat-icon></button>
              <button mat-icon-button type="button" aria-label="Excluir" (click)="excluir(m)"><mat-icon>delete</mat-icon></button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="colunas"></tr>
          <tr mat-row *matRowDef="let row; columns: colunas"></tr>
        </table>
      }
    </section>
  `,
})
export class ModelosEventoPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  protected readonly modelos = signal<Modelo[]>([]);
  protected readonly busca = signal('');
  protected readonly colunas = ['nome', 'tipo', 'acoes'];
  protected readonly tipos = TIPOS;

  ngOnInit(): void {
    this.carregar();
  }

  protected rotuloTipo(tipo: string): string {
    return this.tipos.find((item) => item.valor === tipo)?.rotulo ?? tipo;
  }

  protected buscar(evento: Event): void {
    this.busca.set((evento.target as HTMLInputElement).value);
    this.carregar();
  }

  protected criar(): void {
    this.dialog.open(CriarModeloDialog, { ...DIALOG, width: 'min(480px, calc(100vw - 32px))' }).afterClosed().subscribe((modelo) => {
      if (modelo) {
        void this.router.navigate(['/modelos-evento', modelo.id]);
      }
    });
  }

  protected clonar(modelo: Modelo): void {
    this.negocio.clonarModelo(modelo.id).subscribe({
      next: () => {
        this.toast.sucesso('Modelo clonado.');
        this.carregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected excluir(modelo: Modelo): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir modelo', texto: `Confirma a exclusão de "${modelo.nome}"?`, confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (!confirmou) {
        return;
      }
      this.negocio.excluirModelo(modelo.id).subscribe({
        next: () => {
          this.toast.sucesso('Modelo excluído.');
          this.carregar();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  private carregar(): void {
    this.negocio.modelos(this.busca()).subscribe({
      next: (lista) => this.modelos.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
