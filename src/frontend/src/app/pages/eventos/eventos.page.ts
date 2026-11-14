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
import {
  EstatisticasEventos,
  EventoResumo,
  MESES,
  NegocioClient,
  TIPOS,
} from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { CampoDataComponent } from '../../shared/campo-data';
import { DIALOG } from '../../shared/dialog';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { deDataParaIso } from '../../shared/data';
import { GdDataPipe } from '../../shared/gd-data.pipe';
import { EventoFormPage } from './evento-form.page';

@Component({
  selector: 'app-eventos',
  imports: [
    ReactiveFormsModule, RouterLink, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule,
    MatIconModule, MatTableModule, CampoDataComponent, GdDataPipe,
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

    @if (estatisticas(); as stats) {
      <section class="gd-numeros resumo">
        <a class="gd-numero" routerLink="/pagamentos">
          <span>{{ stats.resumo.rotuloPagamentos }}</span>
          <span class="gd-icone"><mat-icon>payments</mat-icon></span>
          <strong>—</strong>
        </a>
        <a class="gd-numero" routerLink="/tarefas" [queryParams]="{ semana: 1 }">
          <span>{{ stats.resumo.rotuloTarefas }}</span>
          <span class="gd-icone"><mat-icon>task_alt</mat-icon></span>
          <strong>{{ stats.resumo.tarefasSemana }}</strong>
          <small>{{ stats.resumo.contadorTarefas }}</small>
        </a>
        <a class="gd-numero" routerLink="/anexos">
          <span>{{ stats.resumo.rotuloAnexos }}</span>
          <span class="gd-icone"><mat-icon>attach_file</mat-icon></span>
          <strong>—</strong>
        </a>
        <a class="gd-numero" routerLink="/equipe">
          <span>{{ stats.resumo.rotuloEquipe }}</span>
          <span class="gd-icone"><mat-icon>groups</mat-icon></span>
          <strong>—</strong>
        </a>
        <a class="gd-numero" routerLink="/aniversarios">
          <span>{{ stats.resumo.rotuloAniversarios }}</span>
          <span class="gd-icone"><mat-icon>cake</mat-icon></span>
          <strong>{{ stats.resumo.aniversariosHoje }}</strong>
          <small>{{ stats.resumo.contadorAniversarios }}</small>
        </a>
      </section>
    }

    <section class="gd-painel">
      <div class="gd-barra">
        <div class="gd-abas">
          @for (aba of abas; track aba.valor; let indice = $index) {
            <button type="button" [class.ativa]="indice === abaAtual()" (click)="trocarAba(indice)">{{ aba.rotulo }}</button>
          }
        </div>
        @if (!ehEstatisticas()) {
          <form class="filtros" [formGroup]="filtro" (ngSubmit)="carregar()">
            <mat-form-field appearance="outline" subscriptSizing="dynamic"><mat-label>Nome</mat-label><input matInput formControlName="nome" /></mat-form-field>
            <mat-form-field appearance="outline" subscriptSizing="dynamic">
              <mat-label>Tipo</mat-label>
              <mat-select formControlName="tipo">
                <mat-option value="">Todos</mat-option>
                @for (tipo of tipos; track tipo.valor) { <mat-option [value]="tipo.valor">{{ tipo.rotulo }}</mat-option> }
              </mat-select>
            </mat-form-field>
            <app-campo-data rotulo="De" formControlName="de" />
            <app-campo-data rotulo="Até" formControlName="ate" />
            <mat-form-field appearance="outline" subscriptSizing="dynamic"><mat-label>Tag</mat-label><input matInput formControlName="tag" /></mat-form-field>
            <button mat-stroked-button type="submit">Filtrar</button>
            <button mat-stroked-button type="button" (click)="limparFiltros()">Limpar filtros</button>
          </form>
        }
      </div>

      @if (ehEstatisticas()) {
        @if (estatisticas(); as stats) {
          <div class="stats">
            <p class="total">Total de eventos: <strong>{{ stats.total }}</strong></p>
            <div class="gd-numeros">
              @for (item of stats.situacoes; track item.situacao) {
                <div class="gd-numero">
                  <span>{{ rotuloSituacao(item.situacao) }}</span>
                  <strong>{{ item.quantidade }}</strong>
                  <small>{{ item.percentual }}%</small>
                </div>
              }
            </div>
            <h2>Eventos por mês (ano atual)</h2>
            <div class="barras">
              @for (mes of stats.eventosPorMes; track mes.mes) {
                <div class="barra-item">
                  <div class="barra" [style.height.%]="altura(mes.quantidade, stats.eventosPorMes)"></div>
                  <span>{{ meses[mes.mes - 1] }}</span>
                  <small>{{ mes.quantidade }}</small>
                </div>
              }
            </div>
            <div class="rankings">
              <div>
                <h3>Ranking de orçamento</h3>
                @if (stats.rankingOrcamento.length === 0) {
                  <p class="gd-vazio">Sem dados.</p>
                } @else {
                  <table class="gd-grade" mat-table [dataSource]="stats.rankingOrcamento">
                    <ng-container matColumnDef="posicao"><th mat-header-cell *matHeaderCellDef>#</th><td mat-cell *matCellDef="let r">{{ r.posicao }}</td></ng-container>
                    <ng-container matColumnDef="servico"><th mat-header-cell *matHeaderCellDef>Serviço</th><td mat-cell *matCellDef="let r">{{ r.servico }}</td></ng-container>
                    <ng-container matColumnDef="percentual"><th mat-header-cell *matHeaderCellDef>%</th><td mat-cell *matCellDef="let r">{{ r.percentual }}%</td></ng-container>
                    <tr mat-header-row *matHeaderRowDef="colsOrc"></tr>
                    <tr mat-row *matRowDef="let row; columns: colsOrc"></tr>
                  </table>
                }
              </div>
              <div>
                <h3>Fornecedores mais contratados</h3>
                @if (stats.rankingFornecedores.length === 0) {
                  <p class="gd-vazio">Sem dados.</p>
                } @else {
                  <table class="gd-grade" mat-table [dataSource]="stats.rankingFornecedores">
                    <ng-container matColumnDef="posicao"><th mat-header-cell *matHeaderCellDef>#</th><td mat-cell *matCellDef="let r">{{ r.posicao }}</td></ng-container>
                    <ng-container matColumnDef="fornecedor"><th mat-header-cell *matHeaderCellDef>Fornecedor</th><td mat-cell *matCellDef="let r">{{ r.fornecedor }}</td></ng-container>
                    <ng-container matColumnDef="quantidade"><th mat-header-cell *matHeaderCellDef>Qtd</th><td mat-cell *matCellDef="let r">{{ r.quantidade }}</td></ng-container>
                    <tr mat-header-row *matHeaderRowDef="colsForn"></tr>
                    <tr mat-row *matRowDef="let row; columns: colsForn"></tr>
                  </table>
                }
              </div>
              <div>
                <h3>Serviços mais utilizados</h3>
                @if (stats.rankingServicos.length === 0) {
                  <p class="gd-vazio">Sem dados.</p>
                } @else {
                  <table class="gd-grade" mat-table [dataSource]="stats.rankingServicos">
                    <ng-container matColumnDef="posicao"><th mat-header-cell *matHeaderCellDef>#</th><td mat-cell *matCellDef="let r">{{ r.posicao }}</td></ng-container>
                    <ng-container matColumnDef="servico"><th mat-header-cell *matHeaderCellDef>Serviço</th><td mat-cell *matCellDef="let r">{{ r.servico }}</td></ng-container>
                    <ng-container matColumnDef="quantidade"><th mat-header-cell *matHeaderCellDef>Qtd</th><td mat-cell *matCellDef="let r">{{ r.quantidade }}</td></ng-container>
                    <tr mat-header-row *matHeaderRowDef="colsServ"></tr>
                    <tr mat-row *matRowDef="let row; columns: colsServ"></tr>
                  </table>
                }
              </div>
            </div>
          </div>
        }
      } @else if (eventos().length === 0) {
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
              {{ evento.data | gdData }}
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
              <button mat-icon-button type="button" aria-label="Editar" (click)="editar(evento.id)"><mat-icon>edit</mat-icon></button>
              <button mat-icon-button type="button" aria-label="Excluir" (click)="excluir()"><mat-icon>delete</mat-icon></button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="colunas"></tr>
          <tr mat-row *matRowDef="let row; columns: colunas"></tr>
        </table>
      }
    </section>
  `,
  styles: `
    .resumo { margin-bottom: 16px; }
    .filtros { flex-direction: row; align-items: center; flex-wrap: wrap; }
    .filtros mat-form-field, .filtros app-campo-data { width: 140px; }
    .filtros app-campo-data { display: block; }
    .stats .total { margin: 8px 0 16px; }
    .barras { display: flex; gap: 8px; align-items: flex-end; height: 140px; margin: 16px 0 24px; }
    .barra-item { flex: 1; display: flex; flex-direction: column; align-items: center; gap: 4px; height: 100%; justify-content: flex-end; }
    .barra { width: 100%; max-width: 28px; min-height: 4px; background: var(--gd-primaria, #2ecc71); border-radius: 4px 4px 0 0; }
    .rankings { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 16px; }
    .rankings h3 { margin: 0 0 8px; font-size: 0.95rem; }
  `,
})
export class EventosPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly tipos = TIPOS;
  protected readonly meses = MESES;
  protected readonly eventos = signal<EventoResumo[]>([]);
  protected readonly estatisticas = signal<EstatisticasEventos | null>(null);
  protected readonly abaAtual = signal(0);
  protected readonly colunas = ['nome', 'data', 'situacao', 'tarefas', 'convidados', 'acoes'];
  protected readonly colsOrc = ['posicao', 'servico', 'percentual'];
  protected readonly colsForn = ['posicao', 'fornecedor', 'quantidade'];
  protected readonly colsServ = ['posicao', 'servico', 'quantidade'];
  protected readonly abas = [
    { valor: 'EmAndamento', rotulo: 'Em andamento' },
    { valor: 'Concluido', rotulo: 'Concluído' },
    { valor: 'Cancelado', rotulo: 'Cancelado' },
    { valor: 'Estatisticas', rotulo: 'Estatísticas' },
  ];
  protected readonly filtro = this.fb.group({
    nome: '',
    tipo: '',
    de: this.fb.control<Date | null>(null),
    ate: this.fb.control<Date | null>(null),
    tag: '',
  });

  ngOnInit(): void {
    this.carregarEstatisticas();
    this.rota.queryParamMap.subscribe((params) => {
      const situacao = params.get('situacao') ?? 'EmAndamento';
      const indice = this.abas.findIndex((aba) => aba.valor === situacao);
      this.abaAtual.set(indice >= 0 ? indice : 0);
      if (this.ehEstatisticas()) {
        this.carregarEstatisticas();
      } else {
        this.carregar();
      }
    });
  }

  protected ehEstatisticas(): boolean {
    return this.abas[this.abaAtual()]?.valor === 'Estatisticas';
  }

  protected trocarAba(indice: number): void {
    void this.router.navigate(['/eventos'], { queryParams: { situacao: this.abas[indice].valor } });
  }

  protected abrir(): void {
    this.dialog.open(EventoFormPage, { ...DIALOG, data: {} }).afterClosed().subscribe((salvo) => {
      if (salvo) {
        this.carregar();
        this.carregarEstatisticas();
      }
    });
  }

  protected editar(id: string): void {
    void this.router.navigate(['/eventos', id, 'editar']);
  }

  protected excluir(): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir evento', texto: 'Confirma a exclusão deste evento?', confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (confirmou) {
        this.toast.erro('A exclusão ainda não está disponível neste fluxo.');
      }
    });
  }

  protected limparFiltros(): void {
    this.filtro.reset({ nome: '', tipo: '', de: null, ate: null, tag: '' });
    this.carregar();
  }

  protected carregar(): void {
    const valor = this.filtro.getRawValue();
    this.negocio.eventos({
      situacao: this.abas[this.abaAtual()].valor,
      nome: valor.nome ?? '',
      tipo: valor.tipo ?? '',
      de: deDataParaIso(valor.de) ?? '',
      ate: deDataParaIso(valor.ate) ?? '',
      tag: valor.tag ?? '',
    }).subscribe({
      next: (eventos) => this.eventos.set(eventos),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected carregarEstatisticas(): void {
    this.negocio.estatisticas().subscribe({
      next: (dados) => this.estatisticas.set(dados),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected altura(quantidade: number, lista: { quantidade: number }[]): number {
    const max = Math.max(...lista.map((item) => item.quantidade), 1);
    return Math.max(4, (quantidade / max) * 100);
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
