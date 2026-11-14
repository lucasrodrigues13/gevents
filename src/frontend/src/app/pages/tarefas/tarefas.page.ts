import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { AuthService } from '../../core/auth.service';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient, SITUACOES_TAREFA, Tarefa } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { DIALOG } from '../../shared/dialog';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { GdDataPipe } from '../../shared/gd-data.pipe';
import { TarefaFormDialog } from './tarefa-form.dialog';

@Component({
  selector: 'app-tarefas',
  imports: [RouterLink, MatCardModule, MatButtonModule, MatButtonToggleModule, MatIconModule, MatTableModule, GdDataPipe],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Tarefas</h1>
        <p>Pendências dos eventos da assessoria.</p>
      </div>
      @if (eventoId()) {
        <button mat-flat-button type="button" (click)="abrir()">
          <mat-icon>add</mat-icon>
          Nova tarefa
        </button>
      }
    </header>
    @if (eventoId()) { <a mat-button [routerLink]="['/eventos', eventoId()]">Voltar ao evento</a> }
    <mat-button-toggle-group [value]="visao()" (change)="visao.set($event.value)">
      <mat-button-toggle value="lista">Lista</mat-button-toggle>
      @if (eventoId()) { <mat-button-toggle value="quadro">Quadro</mat-button-toggle> }
    </mat-button-toggle-group>
    @if (visao() === 'lista') {
      <section class="gd-painel">
        @if (tarefas().length === 0) {
          <p class="gd-vazio">Não há tarefa.</p>
        } @else {
          <table class="gd-grade" mat-table [dataSource]="tarefas()">
            <ng-container matColumnDef="titulo">
              <th mat-header-cell *matHeaderCellDef>Tarefa</th>
              <td mat-cell *matCellDef="let tarefa">
                {{ tarefa.titulo }}
                @if (tarefa.descricao) { <span class="gd-sub">{{ tarefa.descricao }}</span> }
              </td>
            </ng-container>
            <ng-container matColumnDef="evento">
              <th mat-header-cell *matHeaderCellDef>Evento</th>
              <td mat-cell *matCellDef="let tarefa">
                @if (eventoId()) {
                  {{ tarefa.evento }}
                } @else {
                  <a [routerLink]="['/eventos', tarefa.eventoId, 'tarefas']">{{ tarefa.evento }}</a>
                }
              </td>
            </ng-container>
            <ng-container matColumnDef="prazo">
              <th mat-header-cell *matHeaderCellDef>Prazo</th>
              <td mat-cell *matCellDef="let tarefa">{{ tarefa.prazo | gdData }}</td>
            </ng-container>
            <ng-container matColumnDef="situacao">
              <th mat-header-cell *matHeaderCellDef>Situação</th>
              <td mat-cell *matCellDef="let tarefa"><span class="gd-status alerta">{{ rotulo(tarefa.situacao) }}</span></td>
            </ng-container>
            <ng-container matColumnDef="responsavel">
              <th mat-header-cell *matHeaderCellDef>Responsável</th>
              <td mat-cell *matCellDef="let tarefa">{{ email() }}</td>
            </ng-container>
            <ng-container matColumnDef="acoes">
              <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
              <td mat-cell *matCellDef="let tarefa" class="gd-acoes">
                @if (eventoId()) {
                  <button mat-icon-button type="button" aria-label="Editar" (click)="abrir(tarefa)"><mat-icon>edit</mat-icon></button>
                  <button mat-icon-button type="button" aria-label="Excluir" (click)="excluir()"><mat-icon>delete</mat-icon></button>
                }
              </td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="colunas"></tr>
            <tr mat-row *matRowDef="let row; columns: colunas"></tr>
          </table>
        }
      </section>
    } @else {
      <div class="quadro">
        @for (coluna of situacoes; track coluna.valor) {
          <section class="gd-painel">
            <h2>{{ coluna.rotulo }}</h2>
            @for (tarefa of daColuna(coluna.valor); track tarefa.id) {
              <mat-card><mat-card-content>{{ tarefa.titulo }} @if (tarefa.prazo) { — {{ tarefa.prazo | gdData }} }</mat-card-content></mat-card>
            }
          </section>
        }
      </div>
    }
  `,
  styles: `.quadro { display: grid; grid-template-columns: repeat(4, minmax(160px, 1fr)); gap: 12px; margin-top: 16px; } mat-button-toggle-group { margin-bottom: 16px; }`,
})
export class TarefasPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  private readonly auth = inject(AuthService);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly situacoes = SITUACOES_TAREFA;
  protected readonly tarefas = signal<Tarefa[]>([]);
  protected readonly visao = signal('lista');
  protected readonly eventoId = signal('');
  protected readonly email = this.auth.email.bind(this.auth);
  protected readonly colunas = ['titulo', 'evento', 'prazo', 'situacao', 'responsavel', 'acoes'];

  ngOnInit(): void {
    this.eventoId.set(this.rota.snapshot.paramMap.get('id') ?? '');
    this.carregar();
  }

  protected rotulo(situacao: string): string {
    return this.situacoes.find((item) => item.valor === situacao)?.rotulo ?? situacao;
  }

  protected daColuna(situacao: string): Tarefa[] {
    return this.tarefas().filter((tarefa) => tarefa.situacao === situacao);
  }

  protected abrir(tarefa?: Tarefa): void {
    this.dialog.open(TarefaFormDialog, { ...DIALOG, data: { eventoId: this.eventoId(), tarefa } }).afterClosed().subscribe((salvo) => {
      if (salvo) {
        this.carregar();
      }
    });
  }

  protected excluir(): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir tarefa', texto: 'Confirma a exclusão desta tarefa?', confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (confirmou) {
        this.toast.erro('A exclusão ainda não está disponível neste fluxo.');
      }
    });
  }

  private carregar(): void {
    const pedido = this.eventoId() ? this.negocio.tarefasDoEvento(this.eventoId()) : this.negocio.tarefas();
    pedido.subscribe({
      next: (tarefas) => this.tarefas.set(tarefas),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
