import { Component, inject, OnInit, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ContaClient } from '../../core/conta.client';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-area-usuario',
  imports: [MatCardModule],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Área do usuário</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <p>{{ status() }}</p>
      </mat-card-content>
    </mat-card>
  `,
})
export class AreaUsuarioPage implements OnInit {
  private readonly conta = inject(ContaClient);
  private readonly toast = inject(ToastService);
  protected readonly status = signal('Carregando...');

  ngOnInit(): void {
    this.conta.areaUsuario().subscribe({
      next: (area) => this.status.set(`Você entrou como ${area.email}.`),
      error: (erro: unknown) => {
        const mensagem = mensagemDeErro(erro);
        this.status.set(mensagem);
        this.toast.erro(mensagem);
      },
    });
  }
}
