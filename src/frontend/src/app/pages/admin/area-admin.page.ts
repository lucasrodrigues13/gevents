import { Component, inject, OnInit, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ContaClient } from '../../core/conta.client';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-area-admin',
  imports: [MatCardModule],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Área administrativa</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <p>{{ status() }}</p>
      </mat-card-content>
    </mat-card>
  `,
})
export class AreaAdminPage implements OnInit {
  private readonly conta = inject(ContaClient);
  private readonly toast = inject(ToastService);
  protected readonly status = signal('Carregando...');

  ngOnInit(): void {
    this.conta.areaAdmin().subscribe({
      next: (area) => this.status.set(`Administrador ${area.email}.`),
      error: (erro: unknown) => {
        const mensagem = mensagemDeErro(erro);
        this.status.set(mensagem);
        this.toast.erro(mensagem);
      },
    });
  }
}
