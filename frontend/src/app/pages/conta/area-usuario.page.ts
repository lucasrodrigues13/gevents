import { Component, inject, OnInit, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { ContaClient } from '../../core/conta.client';
import { mensagemDeErro } from '../../core/mensagem-erro';

@Component({
  selector: 'app-area-usuario',
  imports: [MatCardModule],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Área do usuário</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <p>{{ aviso() }}</p>
      </mat-card-content>
    </mat-card>
  `,
})
export class AreaUsuarioPage implements OnInit {
  private readonly conta = inject(ContaClient);
  protected readonly aviso = signal('Carregando...');

  ngOnInit(): void {
    this.conta.areaUsuario().subscribe({
      next: (area) => this.aviso.set(`Você entrou como ${area.email}.`),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }
}
