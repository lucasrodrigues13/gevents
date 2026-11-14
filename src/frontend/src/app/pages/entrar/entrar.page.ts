import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../../core/auth.service';
import { ContaClient } from '../../core/conta.client';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-entrar',
  imports: [ReactiveFormsModule, RouterLink, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
    <section class="pagina-publica">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Entrar</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="formulario" (ngSubmit)="enviar()">
            <mat-form-field appearance="outline">
              <mat-label>E-mail</mat-label>
              <input matInput type="email" formControlName="email" autocomplete="username" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Senha</mat-label>
              <input matInput type="password" formControlName="senha" autocomplete="current-password" />
            </mat-form-field>
            <button mat-flat-button type="submit" [disabled]="formulario.invalid || enviando()">Entrar</button>
          </form>
          <p><a routerLink="/esqueci-senha">Esqueci a senha</a></p>
          <p><a routerLink="/reenviar-confirmacao">Reenviar confirmação de e-mail</a></p>
          <p><a routerLink="/cadastro">Criar conta</a></p>
        </mat-card-content>
      </mat-card>
    </section>
  `,
})
export class EntrarPage {
  private readonly fb = inject(FormBuilder);
  private readonly conta = inject(ContaClient);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  protected readonly enviando = signal(false);
  protected readonly formulario = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    senha: ['', Validators.required],
  });

  protected enviar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      this.toast.erro('Informe e-mail e senha.');
      return;
    }

    this.enviando.set(true);
    const { email, senha } = this.formulario.getRawValue();
    this.conta.entrar(email, senha).subscribe({
      next: (sessao) => {
        this.auth.guardar(sessao);
        this.toast.sucesso('Login realizado.');
        const destino = sessao.papeis.includes('Admin') ? '/admin' : '/painel';
        void this.router.navigateByUrl(destino);
      },
      error: (erro: unknown) => {
        this.toast.erro(mensagemDeErro(erro));
        this.enviando.set(false);
      },
    });
  }
}
