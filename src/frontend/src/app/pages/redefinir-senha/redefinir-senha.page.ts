import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ContaClient } from '../../core/conta.client';
import { mensagemDeErro } from '../../core/mensagem-erro';

@Component({
  selector: 'app-redefinir-senha',
  imports: [ReactiveFormsModule, RouterLink, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
    <section class="pagina-publica">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Redefinir senha</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="formulario" (ngSubmit)="enviar()">
            <mat-form-field appearance="outline">
              <mat-label>Nova senha</mat-label>
              <input matInput type="password" formControlName="senha" autocomplete="new-password" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Confirme a senha</mat-label>
              <input matInput type="password" formControlName="confirmacao" autocomplete="new-password" />
            </mat-form-field>
            @if (aviso()) {
              <p class="aviso" role="status">{{ aviso() }}</p>
            }
            <button mat-flat-button type="submit" [disabled]="formulario.invalid || enviando()">Salvar</button>
          </form>
          <p><a routerLink="/entrar">Ir para o login</a></p>
        </mat-card-content>
      </mat-card>
    </section>
  `,
})
export class RedefinirSenhaPage {
  private readonly fb = inject(FormBuilder);
  private readonly conta = inject(ContaClient);
  private readonly rota = inject(ActivatedRoute);
  protected readonly aviso = signal('');
  protected readonly enviando = signal(false);
  protected readonly formulario = this.fb.nonNullable.group({
    senha: ['', [Validators.required, Validators.minLength(8)]],
    confirmacao: ['', Validators.required],
  });

  protected enviar(): void {
    const usuarioId = this.rota.snapshot.queryParamMap.get('usuarioId');
    const token = this.rota.snapshot.queryParamMap.get('token');
    const { senha, confirmacao } = this.formulario.getRawValue();
    if (!usuarioId || !token) {
      this.aviso.set('O link é inválido ou expirou.');
      return;
    }

    if (senha !== confirmacao) {
      this.aviso.set('A confirmação da senha não confere.');
      return;
    }

    this.enviando.set(true);
    this.conta.redefinir(usuarioId, token, senha, confirmacao).subscribe({
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
