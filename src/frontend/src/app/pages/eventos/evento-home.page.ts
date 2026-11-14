import { DecimalPipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { mensagemDeErro } from '../../core/mensagem-erro';
import {
  BudgetEvento,
  Cliente,
  ETAPAS_NEGOCIACAO,
  EventoDetalhe,
  EventoServico,
  FornecedorResumo,
  NegocioClient,
  ROTULO_PAPEL,
} from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { CampoDataComponent } from '../../shared/campo-data';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { deDataParaIso } from '../../shared/data';
import { DIALOG } from '../../shared/dialog';
import { GdDataPipe } from '../../shared/gd-data.pipe';
import { ClienteFormDialog } from '../clientes/cliente-form.dialog';

@Component({
  selector: 'app-contratar-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatCheckboxModule, CampoDataComponent],
  template: `
    <h2 mat-dialog-title>Contratar fornecedor</h2>
    <mat-dialog-content>
      <form class="formulario" [formGroup]="form">
        <mat-form-field appearance="outline"><mat-label>Valor</mat-label><input matInput type="number" min="0" formControlName="valor" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Parcelas</mat-label><input matInput type="number" min="1" formControlName="quantidadeParcelas" /></mat-form-field>
        <app-campo-data rotulo="Primeira parcela" formControlName="primeiraParcela" />
        <mat-checkbox formControlName="comEntrada">Incluir entrada</mat-checkbox>
        @if (form.controls.comEntrada.value) {
          <mat-form-field appearance="outline">
            <mat-label>Base</mat-label>
            <mat-select formControlName="baseEntrada">
              <mat-option value="Valor">Valor</mat-option>
              <mat-option value="Porcentagem">Porcentagem</mat-option>
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline"><mat-label>Valor de entrada</mat-label><input matInput type="number" formControlName="valorEntrada" /></mat-form-field>
          <app-campo-data rotulo="Data de entrada" formControlName="dataEntrada" />
        }
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="confirmar()">Contratar</button>
    </mat-dialog-actions>
  `,
})
export class ContratarDialog {
  private readonly fb = inject(FormBuilder);
  protected readonly ref = inject(MatDialogRef<ContratarDialog, unknown>);
  protected readonly form = this.fb.group({
    valor: ['', Validators.required],
    quantidadeParcelas: ['1', Validators.required],
    primeiraParcela: this.fb.control<Date | null>(null),
    comEntrada: false,
    baseEntrada: 'Valor',
    valorEntrada: '',
    dataEntrada: this.fb.control<Date | null>(null),
  });

  protected confirmar(): void {
    const valor = this.form.getRawValue();
    this.ref.close({
      valor: Number(valor.valor),
      quantidadeParcelas: Number(valor.quantidadeParcelas),
      primeiraParcela: deDataParaIso(valor.primeiraParcela),
      comEntrada: !!valor.comEntrada,
      baseEntrada: valor.comEntrada ? valor.baseEntrada : null,
      valorEntrada: valor.comEntrada ? Number(valor.valorEntrada) : null,
      dataEntrada: valor.comEntrada ? deDataParaIso(valor.dataEntrada) : null,
    });
  }
}

@Component({
  selector: 'app-nome-servico-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  template: `
    <h2 mat-dialog-title>Adicionar serviço</h2>
    <mat-dialog-content>
      <mat-form-field appearance="outline" class="cheia">
        <mat-label>Nome do serviço</mat-label>
        <input matInput [formControl]="nome" />
      </mat-form-field>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="ok()">Adicionar</button>
    </mat-dialog-actions>
  `,
  styles: `.cheia { width: 100%; }`,
})
export class NomeServicoDialog {
  private readonly fb = inject(FormBuilder);
  protected readonly ref = inject(MatDialogRef<NomeServicoDialog, string>);
  protected readonly nome = this.fb.nonNullable.control('', Validators.required);

  protected ok(): void {
    const valor = this.nome.value.trim();
    if (!valor) {
      this.nome.markAsTouched();
      return;
    }
    this.ref.close(valor);
  }
}

@Component({
  selector: 'app-subitens-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule],
  template: `
    <h2 mat-dialog-title>Subitens — {{ dados.nome }}</h2>
    <mat-dialog-content>
      <form class="formulario" [formGroup]="form">
        <mat-form-field appearance="outline">
          <mat-label>Valor estimado (sem subitens)</mat-label>
          <input matInput type="number" min="0" formControlName="valorEstimado" />
        </mat-form-field>
        <div formArrayName="subitens">
          @for (ctrl of subitens.controls; track $index; let i = $index) {
            <div class="linha" [formGroupName]="i">
              <mat-form-field appearance="outline"><mat-label>Descrição</mat-label><input matInput formControlName="descricao" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Qtd</mat-label><input matInput type="number" min="1" formControlName="quantidade" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Unitário</mat-label><input matInput type="number" min="0" formControlName="valorUnitario" /></mat-form-field>
              <button mat-icon-button type="button" aria-label="Remover" (click)="remover(i)"><mat-icon>delete</mat-icon></button>
            </div>
          }
        </div>
        <button mat-stroked-button type="button" (click)="adicionar()"><mat-icon>add</mat-icon> Subitem</button>
        <p class="gd-sub">Com subitens, o estimado do serviço vira a soma (quantidade × unitário).</p>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="salvar()">Salvar</button>
    </mat-dialog-actions>
  `,
  styles: `
    .linha { display: flex; gap: 8px; flex-wrap: wrap; align-items: center; margin-bottom: 8px; }
    .linha mat-form-field { flex: 1; min-width: 120px; }
  `,
})
export class SubitensDialog {
  private readonly fb = inject(FormBuilder);
  protected readonly dados = inject<{ nome: string; valorEstimado: number | null; subitens: { descricao: string; quantidade: number; valorUnitario: number }[] }>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<SubitensDialog, { valorEstimado: number | null; subitens: { descricao: string; quantidade: number; valorUnitario: number }[] }>);
  protected readonly form = this.fb.group({
    valorEstimado: [this.dados.valorEstimado],
    subitens: this.fb.array(
      this.dados.subitens.map((item) => this.fb.group({
        descricao: [item.descricao, Validators.required],
        quantidade: [item.quantidade, Validators.required],
        valorUnitario: [item.valorUnitario, Validators.required],
      })),
    ),
  });

  protected get subitens() {
    return this.form.controls.subitens;
  }

  protected adicionar(): void {
    this.subitens.push(this.fb.group({
      descricao: ['', Validators.required],
      quantidade: [1, Validators.required],
      valorUnitario: [0, Validators.required],
    }));
  }

  protected remover(indice: number): void {
    this.subitens.removeAt(indice);
  }

  protected salvar(): void {
    const valor = this.form.getRawValue();
    this.ref.close({
      valorEstimado: valor.valorEstimado === null || valor.valorEstimado === undefined ? null : Number(valor.valorEstimado),
      subitens: (valor.subitens ?? [])
        .filter((item) => !!item.descricao?.trim())
        .map((item) => ({
          descricao: item.descricao!.trim(),
          quantidade: Number(item.quantidade) || 1,
          valorUnitario: Number(item.valorUnitario) || 0,
        })),
    });
  }
}

@Component({
  selector: 'app-budget-dialog',
  imports: [DecimalPipe, MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Planejamento de budget</h2>
    <mat-dialog-content>
      @if (budget) {
        <div class="gd-numeros">
          <div class="gd-numero"><span>Budget</span><strong>{{ budget.budget | number:'1.2-2' }}</strong></div>
          <div class="gd-numero"><span>Estimado</span><strong>{{ budget.estimado | number:'1.2-2' }}</strong></div>
          <div class="gd-numero"><span>Contratado</span><strong>{{ budget.contratado | number:'1.2-2' }}</strong></div>
          <div class="gd-numero"><span>Prova real</span><strong>{{ budget.provaReal | number:'1.2-2' }}</strong></div>
          @if (budget.custoPorConvidado !== null) {
            <div class="gd-numero"><span>Por convidado</span><strong>{{ budget.custoPorConvidado | number:'1.2-2' }}</strong></div>
          }
        </div>
        <table class="gd-grade">
          <thead><tr><th>Serviço</th><th>Estimado</th><th>Contratado</th></tr></thead>
          <tbody>
            @for (s of budget.servicos; track s.id) {
              <tr>
                <td>{{ s.nome }}</td>
                <td>{{ s.estimado | number:'1.2-2' }}</td>
                <td>{{ s.contratado | number:'1.2-2' }}</td>
              </tr>
            }
          </tbody>
        </table>
      }
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" mat-dialog-close>Fechar</button>
    </mat-dialog-actions>
  `,
})
export class BudgetDialog {
  protected readonly budget = inject<BudgetEvento>(MAT_DIALOG_DATA);
}

@Component({
  selector: 'app-evento-home',
  imports: [
    RouterLink, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatIconModule,
    GdDataPipe, ReactiveFormsModule,
  ],
  template: `
    <a mat-button routerLink="/eventos">Eventos</a>
    @if (evento(); as atual) {
      <header class="gd-cabecalho">
        <div>
          <h1>{{ atual.nome }}</h1>
          <p>{{ atual.data | gdData }} @if (atual.hora) { às {{ atual.hora }} }</p>
        </div>
        <div class="acoes">
          <button mat-stroked-button type="button" (click)="abrirBudget()"><mat-icon>request_quote</mat-icon> Budget</button>
          <button mat-flat-button type="button" (click)="editar()"><mat-icon>edit</mat-icon> Editar</button>
        </div>
      </header>
      @if (atual.diasRestantes !== null && atual.diasRestantes >= 0) { <p>Faltam {{ atual.diasRestantes }} dias</p> }
      <p>{{ atual.cerimonia.nome }}</p>
      @if (mapa(atual.cerimonia); as link) { <p><a [href]="link" target="_blank" rel="noopener">Mapa da cerimônia</a></p> }
      <p>{{ atual.recepcao.nome }}</p>
      @if (mapa(atual.recepcao); as link) { <p><a [href]="link" target="_blank" rel="noopener">Mapa da recepção</a></p> }
      <p>{{ atual.tarefasConcluidas }} de {{ atual.tarefas }} tarefas concluídas</p>
      <p>Convidados: {{ atual.convidadosConfirmados }} confirmados, {{ atual.convidadosPendentes }} pendentes, {{ atual.convidadosRecusados }} não comparecerão.</p>
      <p>
        <a mat-button [routerLink]="['/eventos', atual.id, 'tarefas']">Tarefas</a>
        <a mat-button [routerLink]="['/eventos', atual.id, 'convidados']">Convidados</a>
      </p>
      @if (atual.situacao === 'EmAndamento') {
        <button mat-stroked-button type="button" (click)="encerrar('concluir')">Concluir</button>
        <button mat-stroked-button type="button" (click)="encerrar('cancelar')">Cancelar</button>
      }

      <section class="gd-painel servicos">
        <div class="gd-barra">
          <h2>Serviços e fornecedores</h2>
          <button mat-stroked-button type="button" (click)="adicionarServico()"><mat-icon>add</mat-icon> Adicionar serviço</button>
        </div>
        @for (servico of atual.servicos; track servico.id) {
          <article class="servico">
            <header>
              <strong>{{ servico.nome }}</strong>
              <span>Estimado: {{ servico.valorEstimado ?? '—' }} · Contratado: {{ servico.valorContratado ?? '—' }}</span>
            </header>
            <div class="trilha">
              @for (etapa of etapas; track etapa.valor) {
                <span class="etapa" [class.ativa]="temEtapa(servico, etapa.valor)">{{ etapa.rotulo }}</span>
                @if (!$last) { <span class="seta">→</span> }
              }
            </div>
            @for (forn of servico.fornecedores; track forn.id) {
              <div class="forn">
                <span>{{ forn.fornecedor }} — {{ rotuloEtapa(forn.etapa) }}</span>
                @if (forn.valorOrcamento !== null) { <span class="gd-sub">{{ forn.valorOrcamento }}</span> }
                <button mat-button type="button" (click)="voltar(servico, forn.id)">Voltar</button>
                <button mat-button type="button" (click)="avancar(servico, forn.id)">Avançar</button>
                <button mat-flat-button type="button" (click)="contratar(servico, forn.id)">Contratar</button>
              </div>
            }
            <div class="linha">
              <mat-form-field appearance="outline" subscriptSizing="dynamic">
                <mat-label>Adicionar fornecedor</mat-label>
                <mat-select (valueChange)="adicionarFornecedor(servico, $event)">
                  @for (f of fornecedores(); track f.id) {
                    <mat-option [value]="f.id">{{ f.nome }}</mat-option>
                  }
                </mat-select>
              </mat-form-field>
              <button mat-stroked-button type="button" (click)="editarSubitens(servico)">Estimado / subitens</button>
            </div>
          </article>
        }
      </section>

      <mat-card>
        <mat-card-header><mat-card-title>Gerenciar clientes</mat-card-title></mat-card-header>
        <mat-card-content>
          <h2>Clientes do evento</h2>
          @if (clientes().length === 0) { <p>Nenhum cliente vinculado.</p> }
          @for (cliente of clientes(); track cliente.id) {
            <p>{{ cliente.nome }} — {{ rotulo(cliente.papel) }}
              <button mat-button type="button" (click)="desvincular(cliente.id)">Desvincular</button>
            </p>
          }
          <h2>Selecionar existente</h2>
          <mat-form-field appearance="outline">
            <mat-label>Cliente da conta</mat-label>
            <mat-select (valueChange)="vincular($event)">
              @for (cliente of todos(); track cliente.id) {
                <mat-option [value]="cliente.id">{{ cliente.nome }} — {{ rotulo(cliente.papel) }}</mat-option>
              }
            </mat-select>
          </mat-form-field>
          <h2>Cadastrar novo cliente</h2>
          <button mat-flat-button type="button" (click)="criarCliente()"><mat-icon>add</mat-icon> Novo cliente</button>
          <h2>Permissões</h2>
          <p>Nenhum cliente selecionado para permissões.</p>
        </mat-card-content>
      </mat-card>
    }
  `,
  styles: `
    .acoes { display: flex; gap: 8px; }
    .servicos { margin: 16px 0; }
    .servico { border-top: 1px solid var(--gd-borda, #e0e0e0); padding: 12px 0; }
    .servico header { display: flex; flex-direction: column; gap: 4px; margin-bottom: 8px; }
    .trilha { display: flex; flex-wrap: wrap; align-items: center; gap: 4px; margin-bottom: 8px; font-size: 0.85rem; }
    .etapa { opacity: 0.45; }
    .etapa.ativa { opacity: 1; font-weight: 600; color: var(--gd-primaria, #2ecc71); }
    .forn { display: flex; flex-wrap: wrap; gap: 8px; align-items: center; margin: 4px 0; }
    .linha { display: flex; gap: 8px; flex-wrap: wrap; margin-top: 8px; }
  `,
})
export class EventoHomePage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly evento = signal<EventoDetalhe | null>(null);
  protected readonly clientes = signal<Cliente[]>([]);
  protected readonly todos = signal<Cliente[]>([]);
  protected readonly fornecedores = signal<FornecedorResumo[]>([]);
  protected readonly etapas = ETAPAS_NEGOCIACAO;
  private id = '';

  ngOnInit(): void {
    this.id = this.rota.snapshot.paramMap.get('id') ?? '';
    this.recarregar();
    this.negocio.fornecedores().subscribe({
      next: (lista) => this.fornecedores.set(lista),
      error: () => this.fornecedores.set([]),
    });
  }

  protected mapa(local: EventoDetalhe['cerimonia']): string | null {
    const texto = [local.endereco, local.numero, local.bairro, local.cidade, local.estado].filter(Boolean).join(', ');
    return texto ? `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(texto)}` : null;
  }

  protected rotulo(papel: string): string {
    return ROTULO_PAPEL[papel] ?? papel;
  }

  protected rotuloEtapa(etapa: string): string {
    return this.etapas.find((item) => item.valor === etapa)?.rotulo ?? etapa;
  }

  protected temEtapa(servico: EventoServico, etapa: string): boolean {
    return servico.fornecedores.some((item) => item.etapa === etapa);
  }

  protected adicionarServico(): void {
    this.dialog.open(NomeServicoDialog, { ...DIALOG, width: 'min(420px, calc(100vw - 32px))' }).afterClosed().subscribe((nome) => {
      if (!nome) {
        return;
      }
      this.negocio.adicionarServicoEvento(this.id, nome).subscribe({
        next: () => {
          this.toast.sucesso('Serviço adicionado.');
          this.recarregar();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  protected adicionarFornecedor(servico: EventoServico, fornecedorId: string): void {
    if (!fornecedorId) {
      return;
    }
    this.negocio.adicionarFornecedorServico(this.id, servico.id, fornecedorId, 'Recomendado').subscribe({
      next: () => {
        this.toast.sucesso('Fornecedor adicionado.');
        this.recarregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected avancar(servico: EventoServico, vinculoId: string): void {
    this.negocio.avancarEtapa(this.id, servico.id, vinculoId).subscribe({
      next: () => {
        this.toast.sucesso('Etapa avançada.');
        this.recarregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected voltar(servico: EventoServico, vinculoId: string): void {
    this.negocio.voltarEtapa(this.id, servico.id, vinculoId).subscribe({
      next: () => {
        this.toast.sucesso('Etapa retornada.');
        this.recarregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected contratar(servico: EventoServico, vinculoId: string): void {
    this.dialog.open(ContratarDialog, { ...DIALOG, width: 'min(480px, calc(100vw - 32px))' }).afterClosed().subscribe((pedido) => {
      if (!pedido) {
        return;
      }
      this.negocio.contratarFornecedor(this.id, servico.id, vinculoId, pedido).subscribe({
        next: () => {
          this.toast.sucesso('Fornecedor contratado.');
          this.recarregar();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  protected editarSubitens(servico: EventoServico): void {
    this.dialog.open(SubitensDialog, {
      ...DIALOG,
      data: {
        nome: servico.nome,
        valorEstimado: servico.valorEstimado,
        subitens: servico.subitens.map((item) => ({
          descricao: item.descricao,
          quantidade: item.quantidade,
          valorUnitario: item.valorUnitario,
        })),
      },
    }).afterClosed().subscribe((pedido) => {
      if (!pedido) {
        return;
      }
      this.negocio.atualizarEstimado(this.id, servico.id, pedido).subscribe({
        next: () => {
          this.toast.sucesso('Estimado atualizado.');
          this.recarregar();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  protected abrirBudget(): void {
    this.negocio.budget(this.id).subscribe({
      next: (budget) => this.dialog.open(BudgetDialog, { ...DIALOG, data: budget }),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected encerrar(acao: 'concluir' | 'cancelar'): void {
    const ref = this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: {
        titulo: acao === 'concluir' ? 'Concluir evento' : 'Cancelar evento',
        texto: 'Confirma a mudança de situação?',
        confirmar: acao === 'concluir' ? 'Concluir' : 'Cancelar evento',
      },
    });
    ref.afterClosed().subscribe((confirmou) => {
      if (!confirmou) {
        return;
      }
      const pedido = acao === 'concluir' ? this.negocio.concluir(this.id) : this.negocio.cancelar(this.id);
      pedido.subscribe({
        next: (evento) => {
          this.evento.set(evento);
          this.toast.sucesso(acao === 'concluir' ? 'Evento concluído.' : 'Evento cancelado.');
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  protected vincular(clienteId: string): void {
    if (!clienteId) {
      return;
    }
    this.negocio.vincular(this.id, clienteId).subscribe({
      next: () => {
        this.toast.sucesso('Cliente vinculado.');
        this.recarregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected desvincular(clienteId: string): void {
    this.negocio.desvincular(this.id, clienteId).subscribe({
      next: () => {
        this.toast.sucesso('Cliente desvinculado.');
        this.recarregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected editar(): void {
    void this.router.navigate(['/eventos', this.id, 'editar']);
  }

  protected criarCliente(): void {
    this.dialog.open(ClienteFormDialog, { ...DIALOG, data: null }).afterClosed().subscribe((cliente) => {
      if (cliente) {
        this.vincular(cliente.id);
      }
    });
  }

  private recarregar(): void {
    this.negocio.evento(this.id).subscribe({
      next: (evento) => this.evento.set(evento),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
    this.negocio.clientesDoEvento(this.id).subscribe({
      next: (clientes) => this.clientes.set(clientes),
      error: () => this.clientes.set([]),
    });
    this.negocio.clientes().subscribe({
      next: (clientes) => this.todos.set(clientes),
      error: () => this.todos.set([]),
    });
  }
}
