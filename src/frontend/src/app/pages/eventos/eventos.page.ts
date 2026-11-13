import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { EventoResumo, NegocioClient, TIPOS } from '../../core/negocio.client';

@Component({
  selector: 'app-eventos',
  imports: [ReactiveFormsModule, RouterLink, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatTabsModule],
  template: `
    <header class="topo">
      <h1>Eventos</h1>
      <a mat-flat-button routerLink="/eventos/novo">Novo evento</a>
    </header>
    <form class="filtros" [formGroup]="filtro" (ngSubmit)="carregar()">
      <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="nome" /></mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Tipo</mat-label>
        <mat-select formControlName="tipo">
          <mat-option value="">Todos</mat-option>
          @for (tipo of tipos; track tipo.valor) { <mat-option [value]="tipo.valor">{{ tipo.rotulo }}</mat-option> }
        </mat-select>
      </mat-form-field>
      <mat-form-field appearance="outline"><mat-label>De</mat-label><input matInput type="date" formControlName="de" /></mat-form-field>
      <mat-form-field appearance="outline"><mat-label>Até</mat-label><input matInput type="date" formControlName="ate" /></mat-form-field>
      <mat-form-field appearance="outline"><mat-label>Tag</mat-label><input matInput formControlName="tag" /></mat-form-field>
      <button mat-stroked-button type="submit">Filtrar</button>
    </form>
    <mat-tab-group [selectedIndex]="aba()" (selectedIndexChange)="trocarAba($event)">
      <mat-tab label="Em andamento" />
      <mat-tab label="Concluído" />
      <mat-tab label="Cancelado" />
    </mat-tab-group>
    @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
    @if (eventos().length === 0 && !aviso()) {
      <p>Não há evento para mostrar.</p>
    }
    <div class="cards">
      @for (evento of eventos(); track evento.id) {
        <mat-card>
          <mat-card-header>
            <mat-card-title><a [routerLink]="['/eventos', evento.id]">{{ evento.nome }}</a></mat-card-title>
            <mat-card-subtitle>{{ rotuloTipo(evento.tipo) }}</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            @if (evento.data) { <p>{{ evento.data }}</p> }
            @if (evento.diasRestantes !== null && evento.diasRestantes >= 0) { <p>Faltam {{ evento.diasRestantes }} dias</p> }
            @if (evento.tags.length) { <p>{{ evento.tags.join(', ') }}</p> }
            <p>{{ evento.tarefasConcluidas }} de {{ evento.tarefas }} concluídas</p>
            <p>{{ evento.convidadosConfirmados }} de {{ evento.convidados }} confirmados</p>
          </mat-card-content>
        </mat-card>
      }
    </div>
  `,
  styles: `
    .topo, .filtros, .cards { display: flex; gap: 12px; flex-wrap: wrap; align-items: center; }
    .cards { margin-top: 16px; }
    mat-card { width: min(320px, 100%); }
  `,
})
export class EventosPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly tipos = TIPOS;
  protected readonly eventos = signal<EventoResumo[]>([]);
  protected readonly aviso = signal('');
  protected readonly aba = signal(0);
  protected readonly filtro = this.fb.nonNullable.group({ nome: '', tipo: '', de: '', ate: '', tag: '' });

  ngOnInit(): void {
    this.rota.queryParamMap.subscribe((params) => {
      const situacao = params.get('situacao') ?? 'EmAndamento';
      this.aba.set(situacao === 'Concluido' ? 1 : situacao === 'Cancelado' ? 2 : 0);
      this.carregar();
    });
  }

  protected trocarAba(indice: number): void {
    const situacao = ['EmAndamento', 'Concluido', 'Cancelado'][indice];
    void this.router.navigate(['/eventos'], { queryParams: { situacao } });
  }

  protected carregar(): void {
    const situacao = ['EmAndamento', 'Concluido', 'Cancelado'][this.aba()];
    const valor = this.filtro.getRawValue();
    this.negocio.eventos({ situacao, ...valor }).subscribe({
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
}
