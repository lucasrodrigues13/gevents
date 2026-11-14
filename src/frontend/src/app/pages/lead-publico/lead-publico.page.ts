import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-lead-publico',
  imports: [ReactiveFormsModule, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  template: `
    <section class="pagina-publica">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Solicitar orçamento</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          @if (enviado()) {
            <p>Recebemos o seu contato. Em breve a assessoria retorna.</p>
          } @else {
            <form class="formulario" [formGroup]="form" (ngSubmit)="enviar()">
              <mat-form-field appearance="outline">
                <mat-label>Nome da oportunidade</mat-label>
                <input matInput formControlName="nome" />
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Seu nome</mat-label>
                <input matInput formControlName="contatoNome" />
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Telefone</mat-label>
                <input matInput formControlName="telefone" />
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>E-mail</mat-label>
                <input matInput formControlName="email" />
              </mat-form-field>
              <button mat-flat-button type="submit" [disabled]="!contaId">Enviar</button>
            </form>
          }
        </mat-card-content>
      </mat-card>
    </section>
  `,
})
export class LeadPublicoPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  private readonly toast = inject(ToastService);
  protected contaId = '';
  protected readonly enviado = signal(false);
  protected readonly form = this.fb.nonNullable.group({
    nome: ['', Validators.required],
    contatoNome: ['', Validators.required],
    telefone: [''],
    email: [''],
  });

  ngOnInit(): void {
    this.contaId = this.rota.snapshot.paramMap.get('contaId') ?? '';
  }

  protected enviar(): void {
    if (this.form.invalid || !this.contaId) {
      this.form.markAllAsTouched();
      this.toast.erro('Informe o nome e o contato.');
      return;
    }
    const v = this.form.getRawValue();
    this.negocio.criarLeadPublico(this.contaId, {
      nome: v.nome,
      contatoNome: v.contatoNome,
      telefone: v.telefone || null,
      email: v.email || null,
    }).subscribe({
      next: () => {
        this.enviado.set(true);
        this.toast.sucesso('Contato enviado.');
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
