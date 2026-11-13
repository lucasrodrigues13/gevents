import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Cliente, EventoDetalhe, NegocioClient, ROTULO_PAPEL } from '../../core/negocio.client';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { DIALOG } from '../../shared/dialog';
import { ClienteFormDialog } from '../clientes/cliente-form.dialog';
import { EventoFormPage } from './evento-form.page';

@Component({
  selector: 'app-evento-home',
  imports: [RouterLink, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatIconModule],
  template: `
    <a mat-button routerLink="/eventos">Eventos</a>
    @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
    @if (evento(); as atual) {
      <header class="gd-cabecalho">
        <div>
          <h1>{{ atual.nome }}</h1>
          <p>{{ atual.data }} @if (atual.hora) { às {{ atual.hora }} }</p>
        </div>
        <button mat-flat-button type="button" (click)="editar()"><mat-icon>edit</mat-icon> Editar</button>
      </header>
      @if (atual.diasRestantes !== null && atual.diasRestantes >= 0) { <p>Faltam {{ atual.diasRestantes }} dias</p> }
      <p>{{ atual.cerimonia.nome }}</p>
      @if (mapa(atual.cerimonia); as link) { <p><a [href]="link" target="_blank" rel="noopener">Mapa da cerimônia</a></p> }
      <p>{{ atual.recepcao.nome }}</p>
      @if (mapa(atual.recepcao); as link) { <p><a [href]="link" target="_blank" rel="noopener">Mapa da recepção</a></p> }
      <p>{{ atual.tarefasConcluidas }} de {{ atual.tarefas }} tarefas concluídas</p>
      <p>Convidados: {{ atual.convidadosConfirmados }} confirmados, {{ atual.convidadosPendentes }} pendentes, {{ atual.convidadosRecusados }} não comparecerão.</p>
      @for (servico of atual.servicos; track servico.id) {
        <p>{{ servico.nome }} @if (servico.contratado) { — contratado, {{ servico.valorContratado }}, {{ servico.parcelas.length }} parcelas }</p>
      }
      <p>
        <a mat-button [routerLink]="['/eventos', atual.id, 'tarefas']">Tarefas</a>
        <a mat-button [routerLink]="['/eventos', atual.id, 'convidados']">Convidados</a>
      </p>
      @if (atual.situacao === 'EmAndamento') {
        <button mat-stroked-button type="button" (click)="encerrar('concluir')">Concluir</button>
        <button mat-stroked-button type="button" (click)="encerrar('cancelar')">Cancelar</button>
      }
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
})
export class EventoHomePage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  private readonly dialog = inject(MatDialog);
  protected readonly evento = signal<EventoDetalhe | null>(null);
  protected readonly clientes = signal<Cliente[]>([]);
  protected readonly todos = signal<Cliente[]>([]);
  protected readonly aviso = signal('');
  private id = '';

  ngOnInit(): void {
    this.id = this.rota.snapshot.paramMap.get('id') ?? '';
    this.recarregar();
  }

  protected mapa(local: EventoDetalhe['cerimonia']): string | null {
    const texto = [local.endereco, local.numero, local.bairro, local.cidade, local.estado].filter(Boolean).join(', ');
    return texto ? `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(texto)}` : null;
  }

  protected rotulo(papel: string): string {
    return ROTULO_PAPEL[papel] ?? papel;
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
        next: (evento) => this.evento.set(evento),
        error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
      });
    });
  }

  protected vincular(clienteId: string): void {
    if (!clienteId) {
      return;
    }
    this.negocio.vincular(this.id, clienteId).subscribe({
      next: () => this.recarregar(),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected desvincular(clienteId: string): void {
    this.negocio.desvincular(this.id, clienteId).subscribe({
      next: () => this.recarregar(),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected editar(): void {
    this.dialog.open(EventoFormPage, { ...DIALOG, data: { id: this.id } }).afterClosed().subscribe((salvo) => {
      if (salvo) {
        this.recarregar();
      }
    });
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
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
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
