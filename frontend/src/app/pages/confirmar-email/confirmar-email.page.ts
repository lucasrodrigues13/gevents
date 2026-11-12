import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { ContaClient } from '../../core/conta.client';
import { mensagemDeErro } from '../../core/mensagem-erro';

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
          <p role="status">{{ aviso() }}</p>
          <p><a routerLink="/entrar">Ir para o login</a></p>
        </mat-card-content>
      </mat-card>
    </section>
  `,
})
export class ConfirmarEmailPage implements OnInit {
  private readonly rota = inject(ActivatedRoute);
  private readonly conta = inject(ContaClient);
  protected readonly aviso = signal('Confirmando o e-mail...');

  ngOnInit(): void {
    const usuarioId = this.rota.snapshot.queryParamMap.get('usuarioId');
    const token = this.rota.snapshot.queryParamMap.get('token');
    if (!usuarioId || !token) {
      this.aviso.set('O link é inválido ou expirou.');
      return;
    }

    this.conta.confirmar(usuarioId, token).subscribe({
      next: (resposta) => this.aviso.set(resposta.mensagem),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }
}
