import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { EventoResumo, NegocioClient, TIPOS } from '../../core/negocio.client';
import { DIALOG } from '../../shared/dialog';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { EventoFormPage } from './evento-form.page';

@Component({
  selector: 'app-eventos',
  imports: [
    ReactiveFormsModule, RouterLink, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule,
    MatIconModule, MatTableModule,
  ],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Eventos</h1>
        <p>Acompanhe os eventos da assessoria.</p>
      </div>
      <button mat-flat-button type="button" (click)="abrir()">
        <mat-icon>add</mat-icon>
        Novo evento
      </button>
    </header>
    @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
    <section class="gd-painel">
      <div class="gd-barra">
        <div class="gd-abas">
          @for (aba of abas; track aba.valor; let indice = $index) {
            <button type="button" [class.ativa]="indice === abaAtual()" (click)="trocarAba(indice)">{{ aba.rotulo }}</button>
          }
        </div>
        <form class="filtros" [formGroup]="filtro" (ngSubmit)="carregar()">
          <mat-form-field appearance="outline" subscriptSizing="dynamic"><mat-label>Nome</mat-label><input matInput formControlName="nome" /></mat-form-field>
          <mat-form-field appearance="outline" subscriptSizing="dynamic">
            <mat-label>Tipo</mat-label>
            <mat-select formControlName="tipo">
              <mat-option value="">Todos</mat-option>
              @for (tipo of tipos; track tipo.valor) { <mat-option [value]="tipo.valor">{{ tipo.rotulo }}</mat-option> }
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline" subscriptSizing="dynamic"><mat-label>De</mat-label><input matInput type="date" formControlName="de" /></mat-form-field>
          <mat-form-field appearance="outline" subscriptSizing="dynamic"><mat-label>Até</mat-label><input matInput type="date" formControlName="ate" /></mat-form-field>
          <mat-form-field appearance="outline" subscriptSizing="dynamic"><mat-label>Tag</mat-label><input matInput formControlName="tag" /></mat-form-field>
          <button mat-stroked-button type="submit">Filtrar</button>
        </form>
      </div>
      @if (eventos().length === 0) {
        <p class="gd-vazio">Não há evento para mostrar.</p>
      } @else {
        <table class="gd-grade" mat-table [dataSource]="eventos()">
          <ng-container matColumnDef="nome">
            <th mat-header-cell *matHeaderCellDef>Evento</th>
            <td mat-cell *matCellDef="let evento">
              <a [routerLink]="['/eventos', evento.id]">{{ evento.nome }}</a>
              <span class="gd-sub">{{ rotuloTipo(evento.tipo) }}</span>
            </td>
          </ng-container>
          <ng-container matColumnDef="data">
            <th mat-header-cell *matHeaderCellDef>Data</th>
            <td mat-cell *matCellDef="let evento">
              {{ evento.data ?? '—' }}
              @if (evento.diasRestantes !== null && evento.diasRestantes >= 0) {
                <span class="gd-sub">Faltam {{ evento.diasRestantes }} dias</span>
              }
            </td>
          </ng-container>
          <ng-container matColumnDef="situacao">
            <th mat-header-cell *matHeaderCellDef>Situação</th>
            <td mat-cell *matCellDef="let evento"><span [class]="classe(evento.situacao)">{{ rotuloSituacao(evento.situacao) }}</span></td>
          </ng-container>
          <ng-container matColumnDef="tarefas">
            <th mat-header-cell *matHeaderCellDef>Tarefas</th>
            <td mat-cell *matCellDef="let evento">{{ evento.tarefasConcluidas }} de {{ evento.tarefas }}</td>
          </ng-container>
          <ng-container matColumnDef="convidados">
            <th mat-header-cell *matHeaderCellDef>Convidados</th>
            <td mat-cell *matCellDef="let evento">{{ evento.convidadosConfirmados }} de {{ evento.convidados }}</td>
          </ng-container>
          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
            <td mat-cell *matCellDef="let evento" class="gd-acoes">
              <button mat-icon-button type="button" aria-label="Editar" (click)="abrir(evento.id)"><mat-icon>edit</mat-icon></button>
              <button mat-icon-button type="button" aria-label="Excluir" (click)="excluir()"><mat-icon>delete</mat-icon></button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="colunas"></tr>
          <tr mat-row *matRowDef="let row; columns: colunas"></tr>
        </table>
      }
    </section>
  `,
  styles: `.filtros { flex-direction: row; align-items: center; } .filtros mat-form-field { width: 140px; }`,
})
export class EventosPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  protected readonly tipos = TIPOS;
  protected readonly eventos = signal<EventoResumo[]>([]);
  protected readonly aviso = signal('');
  protected readonly abaAtual = signal(0);
  protected readonly colunas = ['nome', 'data', 'situacao', 'tarefas', 'convidados', 'acoes'];
  protected readonly abas = [
    { valor: 'EmAndamento', rotulo: 'Em andamento' },
    { valor: 'Concluido', rotulo: 'Concluído' },
    { valor: 'Cancelado', rotulo: 'Cancelado' },
  ];
  protected readonly filtro = this.fb.nonNullable.group({ nome: '', tipo: '', de: '', ate: '', tag: '' });

  ngOnInit(): void {
    this.rota.queryParamMap.subscribe((params) => {
      const situacao = params.get('situacao') ?? 'EmAndamento';
      this.abaAtual.set(situacao === 'Concluido' ? 1 : situacao === 'Cancelado' ? 2 : 0);
      this.carregar();
    });
  }

  protected trocarAba(indice: number): void {
    void this.router.navigate(['/eventos'], { queryParams: { situacao: this.abas[indice].valor } });
  }

  protected abrir(id?: string): void {
    this.dialog.open(EventoFormPage, { ...DIALOG, data: { id } }).afterClosed().subscribe((salvo) => {
      if (salvo) {
        this.carregar();
      }
    });
  }

  protected excluir(): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir evento', texto: 'Confirma a exclusão deste evento?', confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (confirmou) {
        this.aviso.set('A exclusão ainda não está disponível neste fluxo.');
      }
    });
  }

  protected carregar(): void {
    const valor = this.filtro.getRawValue();
    this.negocio.eventos({ situacao: this.abas[this.abaAtual()].valor, ...valor }).subscribe({
      next: (eventos) => {
        this.eventos.set(eventos);
        this.aviso.set('');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected rotuloTipo(tipo: string): string {
    return this.tipos.find((item) => item.valor === tipo)?.rotulo ?? tipo;
  }

  protected rotuloSituacao(situacao: string): string {
    return this.abas.find((aba) => aba.valor === situacao)?.rotulo ?? situacao;
  }

  protected classe(situacao: string): string {
    if (situacao === 'Concluido') {
      return 'gd-status ok';
    }
    if (situacao === 'Cancelado') {
      return 'gd-status erro';
    }
    return 'gd-status alerta';
  }
}
