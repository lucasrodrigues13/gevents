import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { AuthService } from '../../core/auth.service';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient, SITUACOES_TAREFA, Tarefa } from '../../core/negocio.client';

@Component({
  selector: 'app-tarefas',
  imports: [
    ReactiveFormsModule, RouterLink, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonToggleModule,
  ],
  template: `
    <h1>Tarefas</h1>
    @if (eventoId()) { <a mat-button [routerLink]="['/eventos', eventoId()]">Voltar ao evento</a> }
    <mat-button-toggle-group [value]="visao()" (change)="visao.set($event.value)">
      <mat-button-toggle value="lista">Lista</mat-button-toggle>
      @if (eventoId()) { <mat-button-toggle value="quadro">Quadro</mat-button-toggle> }
    </mat-button-toggle-group>
    @if (eventoId()) {
      <form class="formulario" [formGroup]="formulario" (ngSubmit)="criar()">
        <mat-form-field appearance="outline"><mat-label>Título</mat-label><input matInput formControlName="titulo" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Descrição</mat-label><textarea matInput rows="2" formControlName="descricao"></textarea></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Prazo</mat-label><input matInput type="date" formControlName="prazo" /></mat-form-field>
        <button mat-flat-button type="submit">Adicionar</button>
      </form>
    }
    @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
    @if (tarefas().length === 0) { <p>Não há tarefa.</p> }
    @if (visao() === 'lista') {
      @for (tarefa of tarefas(); track tarefa.id) {
        <mat-card>
          <mat-card-title>{{ tarefa.titulo }}</mat-card-title>
          <mat-card-content>
            <p>{{ tarefa.evento }}</p>
            @if (tarefa.prazo) { <p>Prazo {{ tarefa.prazo }}</p> }
            <p>Responsável: {{ email() }}</p>
            @if (tarefa.descricao) { <p>{{ tarefa.descricao }}</p> }
            @if (eventoId()) {
              <mat-form-field appearance="outline">
                <mat-label>Situação</mat-label>
                <mat-select [value]="tarefa.situacao" (valueChange)="situacao(tarefa, $event)">
                  @for (item of situacoes; track item.valor) { <mat-option [value]="item.valor">{{ item.rotulo }}</mat-option> }
                </mat-select>
              </mat-form-field>
            } @else {
              <a mat-button [routerLink]="['/eventos', tarefa.eventoId, 'tarefas']">Abrir no evento</a>
            }
          </mat-card-content>
        </mat-card>
      }
    } @else {
      <div class="quadro">
        @for (coluna of situacoes; track coluna.valor) {
          <section>
            <h2>{{ coluna.rotulo }}</h2>
            @for (tarefa of daColuna(coluna.valor); track tarefa.id) {
              <mat-card><mat-card-content>{{ tarefa.titulo }} @if (tarefa.prazo) { — {{ tarefa.prazo }} }</mat-card-content></mat-card>
            }
          </section>
        }
      </div>
    }
  `,
  styles: `.quadro { display: grid; grid-template-columns: repeat(4, minmax(160px, 1fr)); gap: 12px; }`,
})
export class TarefasPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  private readonly auth = inject(AuthService);
  protected readonly situacoes = SITUACOES_TAREFA;
  protected readonly tarefas = signal<Tarefa[]>([]);
  protected readonly visao = signal('lista');
  protected readonly aviso = signal('');
  protected readonly eventoId = signal('');
  protected readonly email = this.auth.email.bind(this.auth);
  protected readonly formulario = this.fb.nonNullable.group({ titulo: '', descricao: '', prazo: '' });

  ngOnInit(): void {
    this.eventoId.set(this.rota.snapshot.paramMap.get('id') ?? '');
    this.carregar();
  }

  protected daColuna(situacao: string): Tarefa[] {
    return this.tarefas().filter((tarefa) => tarefa.situacao === situacao);
  }

  protected criar(): void {
    const valor = this.formulario.getRawValue();
    this.negocio.criarTarefa(this.eventoId(), {
      titulo: valor.titulo,
      descricao: valor.descricao,
      prazo: valor.prazo || null,
    }).subscribe({
      next: () => {
        this.formulario.reset({ titulo: '', descricao: '', prazo: '' });
        this.carregar();
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected situacao(tarefa: Tarefa, situacao: string): void {
    this.negocio.atualizarTarefa(tarefa.id, {
      titulo: tarefa.titulo,
      descricao: tarefa.descricao,
      prazo: tarefa.prazo,
      situacao,
    }).subscribe({
      next: () => this.carregar(),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  private carregar(): void {
    const pedido = this.eventoId() ? this.negocio.tarefasDoEvento(this.eventoId()) : this.negocio.tarefas();
    pedido.subscribe({
      next: (tarefas) => {
        this.tarefas.set(tarefas);
        this.aviso.set('');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }
}
