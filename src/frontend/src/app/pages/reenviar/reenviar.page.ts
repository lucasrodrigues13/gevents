import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ContaClient } from '../../core/conta.client';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { ToastService } from '../../core/toast.service';

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
  private readonly toast = inject(ToastService);
  protected readonly enviando = signal(false);
  protected readonly formulario = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  protected enviar(): void {
    if (this.formulario.invalid) {
      this.toast.erro('Informe um e-mail válido.');
      return;
    }

    this.enviando.set(true);
    this.conta.reenviar(this.formulario.getRawValue().email).subscribe({
      next: (resposta) => {
        this.toast.sucesso(resposta.mensagem);
        this.enviando.set(false);
      },
      error: (erro: unknown) => {
        this.toast.erro(mensagemDeErro(erro));
        this.enviando.set(false);
      },
    });
  }
}
