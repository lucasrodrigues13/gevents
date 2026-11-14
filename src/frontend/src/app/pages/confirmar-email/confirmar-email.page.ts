import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { ContaClient } from '../../core/conta.client';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-confirmar-email',
  imports: [RouterLink, MatCardModule],
  template: `
    <section class="pagina-publica">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Confirmar e-mail</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <p role="status">{{ status() }}</p>
          <p><a routerLink="/entrar">Ir para o login</a></p>
        </mat-card-content>
      </mat-card>
    </section>
  `,
})
export class ConfirmarEmailPage implements OnInit {
  private readonly rota = inject(ActivatedRoute);
  private readonly conta = inject(ContaClient);
  private readonly toast = inject(ToastService);
  protected readonly status = signal('Confirmando o e-mail...');

  ngOnInit(): void {
    const usuarioId = this.rota.snapshot.queryParamMap.get('usuarioId');
    const token = this.rota.snapshot.queryParamMap.get('token');
    if (!usuarioId || !token) {
      const mensagem = 'O link é inválido ou expirou.';
      this.status.set(mensagem);
      this.toast.erro(mensagem);
      return;
    }

    this.conta.confirmar(usuarioId, token).subscribe({
      next: (resposta) => {
        this.status.set(resposta.mensagem);
        this.toast.sucesso(resposta.mensagem);
      },
      error: (erro: unknown) => {
        const mensagem = mensagemDeErro(erro);
        this.status.set(mensagem);
        this.toast.erro(mensagem);
      },
    });
  }
}
