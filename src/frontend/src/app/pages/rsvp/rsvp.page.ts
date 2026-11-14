import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-rsvp',
  imports: [ReactiveFormsModule, MatCardModule, MatButtonModule, MatRadioModule, MatCheckboxModule],
  template: `
    <section class="pagina-publica">
      <mat-card>
        <mat-card-header><mat-card-title>{{ titulo() || 'Confirmação de presença' }}</mat-card-title></mat-card-header>
        <mat-card-content>
          @if (convidados().length === 0 && carregado()) {
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
              <mat-checkbox formControlName="pretendeRealizarEvento">
                Pretendo realizar um evento nos próximos dois anos
              </mat-checkbox>
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
  private readonly toast = inject(ToastService);
  protected readonly titulo = signal('');
  protected readonly carregado = signal(false);
  protected readonly convidados = signal<{ id: string; nome: string; situacao: string }[]>([]);
  protected formulario: FormGroup = this.fb.group({ pretendeRealizarEvento: false });
  private token = '';

  ngOnInit(): void {
    this.token = this.rota.snapshot.paramMap.get('token') ?? '';
    this.negocio.abrirRsvp(this.token).subscribe({
      next: (pagina) => {
        this.titulo.set(pagina.convite);
        this.convidados.set(pagina.convidados);
        this.carregado.set(true);
        const controles: Record<string, FormControl<string | boolean | null>> = {
          pretendeRealizarEvento: this.fb.nonNullable.control(false),
        };
        for (const pessoa of pagina.convidados) {
          controles[pessoa.id] = this.fb.nonNullable.control(
            pessoa.situacao === 'Pendente' ? '' : pessoa.situacao,
          );
        }
        this.formulario = this.fb.group(controles);
      },
      error: (erro: unknown) => {
        this.carregado.set(true);
        this.toast.erro(mensagemDeErro(erro));
      },
    });
  }

  protected enviar(): void {
    const respostas = this.convidados()
      .map((pessoa) => ({ convidadoId: pessoa.id, situacao: String(this.formulario.get(pessoa.id)?.value ?? '') }))
      .filter((resposta) => resposta.situacao === 'Confirmado' || resposta.situacao === 'NaoComparecera');
    if (respostas.length === 0) {
      this.toast.erro('Escolha a presença de ao menos uma pessoa.');
      return;
    }
    const pretende = !!this.formulario.get('pretendeRealizarEvento')?.value;
    this.negocio.responderRsvp(this.token, respostas, pretende || undefined).subscribe({
      next: (pagina) => {
        this.convidados.set(pagina.convidados);
        this.toast.sucesso('Presença registrada.');
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
