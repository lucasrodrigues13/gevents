import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatRadioModule } from '@angular/material/radio';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient } from '../../core/negocio.client';

@Component({
  selector: 'app-rsvp',
  imports: [ReactiveFormsModule, MatCardModule, MatButtonModule, MatRadioModule],
  template: `
    <section class="pagina-publica">
      <mat-card>
        <mat-card-header><mat-card-title>{{ titulo() || 'Confirmação de presença' }}</mat-card-title></mat-card-header>
        <mat-card-content>
          @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
          @if (convidados().length === 0 && !aviso()) {
            <p>Não há quem responder.</p>
          }
          <form [formGroup]="formulario" (ngSubmit)="enviar()">
            @for (pessoa of convidados(); track pessoa.id) {
              <fieldset>
                <legend>{{ pessoa.nome }}</legend>
                <mat-radio-group [formControlName]="pessoa.id">
                  <mat-radio-button value="Confirmado">Confirmado</mat-radio-button>
                  <mat-radio-button value="NaoComparecera">Não comparecerá</mat-radio-button>
                </mat-radio-group>
              </fieldset>
            }
            @if (convidados().length) {
              <button mat-flat-button type="submit">Enviar</button>
            }
          </form>
        </mat-card-content>
      </mat-card>
    </section>
  `,
})
export class RsvpPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  protected readonly titulo = signal('');
  protected readonly aviso = signal('');
  protected readonly convidados = signal<{ id: string; nome: string; situacao: string }[]>([]);
  protected formulario = this.fb.group({});
  private token = '';

  ngOnInit(): void {
    this.token = this.rota.snapshot.paramMap.get('token') ?? '';
    this.negocio.abrirRsvp(this.token).subscribe({
      next: (pagina) => {
        this.titulo.set(pagina.convite);
        this.convidados.set(pagina.convidados);
        const controles: Record<string, string> = {};
        for (const pessoa of pagina.convidados) {
          controles[pessoa.id] = pessoa.situacao === 'Pendente' ? '' : pessoa.situacao;
        }
        this.formulario = this.fb.group(controles);
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected enviar(): void {
    const respostas = this.convidados()
      .map((pessoa) => ({ convidadoId: pessoa.id, situacao: String(this.formulario.get(pessoa.id)?.value ?? '') }))
      .filter((resposta) => resposta.situacao === 'Confirmado' || resposta.situacao === 'NaoComparecera');
    this.negocio.responderRsvp(this.token, respostas).subscribe({
      next: (pagina) => {
        this.convidados.set(pagina.convidados);
        this.aviso.set('Presença registrada.');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }
}
