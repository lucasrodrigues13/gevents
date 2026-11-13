import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient, Painel } from '../../core/negocio.client';

@Component({
  selector: 'app-painel',
  imports: [RouterLink, MatCardModule, MatListModule],
  template: `
    <h1>Dashboards</h1>
    @if (aviso()) {
      <p class="aviso" role="alert">{{ aviso() }}</p>
    }
    @if (painel(); as dados) {
      <section class="blocos">
        <mat-card>
          <mat-card-header><mat-card-title>Eventos</mat-card-title></mat-card-header>
          <mat-card-content>
            <div class="numeros">
              <a routerLink="/eventos" [queryParams]="{ situacao: 'EmAndamento' }">
                <strong>{{ dados.emAndamento }}</strong>
                <span>Em andamento</span>
              </a>
              <a routerLink="/eventos" [queryParams]="{ situacao: 'Concluido' }">
                <strong>{{ dados.concluidos }}</strong>
                <span>Concluído</span>
              </a>
              <a routerLink="/eventos" [queryParams]="{ situacao: 'Cancelado' }">
                <strong>{{ dados.cancelados }}</strong>
                <span>Cancelado</span>
              </a>
            </div>
            @if (dados.eventosEmAndamento.length === 0) {
              <p>Não há evento.</p>
            } @else {
              <mat-nav-list>
                @for (evento of dados.eventosEmAndamento; track evento.id) {
                  <a mat-list-item [routerLink]="['/eventos', evento.id]">
                    <span matListItemTitle>{{ evento.nome }}</span>
                    @if (evento.data) {
                      <span matListItemLine>{{ evento.data }}</span>
                    }
                  </a>
                }
              </mat-nav-list>
            }
          </mat-card-content>
        </mat-card>
        <mat-card>
          <mat-card-header><mat-card-title>Funil</mat-card-title></mat-card-header>
          <mat-card-content>
            <div class="numeros">
              @for (estagio of dados.funil; track estagio.estagio) {
                <div>
                  <strong>{{ estagio.quantidade }}</strong>
                  <span>{{ estagio.estagio }}</span>
                  <small>{{ estagio.valor }}</small>
                </div>
              }
            </div>
          </mat-card-content>
        </mat-card>
      </section>
    }
  `,
  styles: `
    .blocos, .numeros { display: grid; gap: 16px; }
    .blocos { grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); }
    .numeros { grid-template-columns: repeat(auto-fit, minmax(120px, 1fr)); margin: 16px 0; }
    .numeros a, .numeros div { display: flex; flex-direction: column; text-decoration: none; color: inherit; }
    strong { font-size: 2rem; }
  `,
})
export class PainelPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  protected readonly painel = signal<Painel | null>(null);
  protected readonly aviso = signal('');

  ngOnInit(): void {
    this.negocio.painel().subscribe({
      next: (painel) => this.painel.set(painel),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }
}
