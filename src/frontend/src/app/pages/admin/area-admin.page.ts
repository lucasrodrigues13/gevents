import { Component, inject, OnInit, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ContaClient } from '../../core/conta.client';
import { mensagemDeErro } from '../../core/mensagem-erro';

@Component({
  selector: 'app-area-admin',
  imports: [MatCardModule],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Área administrativa</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <p>{{ aviso() }}</p>
      </mat-card-content>
    </mat-card>
  `,
})
export class AreaAdminPage implements OnInit {
  private readonly conta = inject(ContaClient);
  protected readonly aviso = signal('Carregando...');

  ngOnInit(): void {
    this.conta.areaAdmin().subscribe({
      next: (area) => this.aviso.set(`Administrador ${area.email}.`),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }
}
