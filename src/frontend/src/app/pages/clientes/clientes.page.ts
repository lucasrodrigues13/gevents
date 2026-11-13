import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Cliente, NegocioClient, ROTULO_PAPEL } from '../../core/negocio.client';
import { DIALOG } from '../../shared/dialog';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { ClienteFormDialog } from './cliente-form.dialog';

@Component({
  selector: 'app-clientes',
  imports: [RouterLink, MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Clientes</h1>
        <p>Contatos da assessoria e os eventos vinculados.</p>
      </div>
      <button mat-flat-button type="button" (click)="abrir()">
        <mat-icon>add</mat-icon>
        Novo cliente
      </button>
    </header>
    @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
    <section class="gd-painel">
      <div class="gd-barra">
        <span></span>
        <mat-form-field appearance="outline" subscriptSizing="dynamic">
          <mat-label>Buscar por nome</mat-label>
          <input matInput [value]="busca()" (input)="buscar($event)" />
        </mat-form-field>
      </div>
      @if (clientes().length === 0) {
        <p class="gd-vazio">Não há cliente para mostrar.</p>
      } @else {
        <table class="gd-grade" mat-table [dataSource]="clientes()">
          <ng-container matColumnDef="nome">
            <th mat-header-cell *matHeaderCellDef>Cliente</th>
            <td mat-cell *matCellDef="let cliente">
              {{ cliente.nome }}
              <span class="gd-sub">{{ rotulo(cliente.papel) }}</span>
            </td>
          </ng-container>
          <ng-container matColumnDef="contato">
            <th mat-header-cell *matHeaderCellDef>Contato</th>
            <td mat-cell *matCellDef="let cliente">
              {{ cliente.email }}
              <span class="gd-sub">{{ cliente.ddi }} {{ cliente.celular }}</span>
            </td>
          </ng-container>
          <ng-container matColumnDef="eventos">
            <th mat-header-cell *matHeaderCellDef>Eventos</th>
            <td mat-cell *matCellDef="let cliente">
              @if (cliente.eventos.length === 0) {
                Nenhum evento vinculado
              } @else {
                @for (evento of cliente.eventos; track evento.id) {
                  <a [routerLink]="['/eventos', evento.id]">{{ evento.nome }}</a>
                }
              }
            </td>
          </ng-container>
          <ng-container matColumnDef="acesso">
            <th mat-header-cell *matHeaderCellDef>Último acesso</th>
            <td mat-cell *matCellDef="let cliente">{{ cliente.ultimoAcesso ?? '—' }}</td>
          </ng-container>
          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
            <td mat-cell *matCellDef="let cliente" class="gd-acoes">
              <button mat-icon-button type="button" aria-label="Editar" (click)="abrir(cliente)"><mat-icon>edit</mat-icon></button>
              <button mat-icon-button type="button" aria-label="Excluir" (click)="excluir()"><mat-icon>delete</mat-icon></button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="colunas"></tr>
          <tr mat-row *matRowDef="let row; columns: colunas"></tr>
        </table>
      }
    </section>
  `,
})
export class ClientesPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly dialog = inject(MatDialog);
  protected readonly clientes = signal<Cliente[]>([]);
  protected readonly aviso = signal('');
  protected readonly busca = signal('');
  protected readonly colunas = ['nome', 'contato', 'eventos', 'acesso', 'acoes'];

  ngOnInit(): void {
    this.carregar();
  }

  protected rotulo(papel: string): string {
    return ROTULO_PAPEL[papel] ?? papel;
  }

  protected buscar(evento: Event): void {
    this.busca.set((evento.target as HTMLInputElement).value);
    this.carregar();
  }

  protected abrir(cliente?: Cliente): void {
    this.dialog.open(ClienteFormDialog, { ...DIALOG, data: cliente ?? null }).afterClosed().subscribe((salvo) => {
      if (salvo) {
        this.carregar();
      }
    });
  }

  protected excluir(): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir cliente', texto: 'Confirma a exclusão deste cliente?', confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (confirmou) {
        this.aviso.set('A exclusão ainda não está disponível neste fluxo.');
      }
    });
  }

  private carregar(): void {
    this.negocio.clientes(this.busca()).subscribe({
      next: (clientes) => {
        this.clientes.set(clientes);
        this.aviso.set('');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }
}
