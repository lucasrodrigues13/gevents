import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ContaClient } from '../../core/conta.client';
import { mensagemDeErro } from '../../core/mensagem-erro';

@Component({
  selector: 'app-reenviar',
  imports: [ReactiveFormsModule, RouterLink, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
    <section class="pagina-publica">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Reenviar confirmação</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="formulario" (ngSubmit)="enviar()">
            <mat-form-field appearance="outline">
              <mat-label>E-mail</mat-label>
              <input matInput type="email" formControlName="email" autocomplete="username" />
            </mat-form-field>
            @if (aviso()) {
              <p class="aviso" role="status">{{ aviso() }}</p>
            }
            <button mat-flat-button type="submit" [disabled]="formulario.invalid || enviando()">Enviar</button>
          </form>
          <p><a routerLink="/entrar">Voltar ao login</a></p>
        </mat-card-content>
      </mat-card>
    </section>
  `,
})
export class ReenviarPage {
  private readonly fb = inject(FormBuilder);
  private readonly conta = inject(ContaClient);
  protected readonly aviso = signal('');
  protected readonly enviando = signal(false);
  protected readonly formulario = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  protected enviar(): void {
    if (this.formulario.invalid) {
      return;
    }

    this.enviando.set(true);
    this.conta.reenviar(this.formulario.getRawValue().email).subscribe({
      next: (resposta) => {
        this.aviso.set(resposta.mensagem);
        this.enviando.set(false);
      },
      error: (erro: unknown) => {
        this.aviso.set(mensagemDeErro(erro));
        this.enviando.set(false);
      },
    });
  }
}
