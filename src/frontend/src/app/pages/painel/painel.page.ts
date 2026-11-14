import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient, Painel } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { GdDataPipe } from '../../shared/gd-data.pipe';

@Component({
  selector: 'app-painel',
  imports: [RouterLink, MatCardModule, MatIconModule, MatListModule, GdDataPipe],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Dashboards</h1>
        <p>O que está em andamento na assessoria.</p>
      </div>
    </header>
    @if (painel(); as dados) {
      <section class="gd-numeros">
        <a class="gd-numero" routerLink="/eventos" [queryParams]="{ situacao: 'EmAndamento' }">
          <span>Em andamento</span>
          <span class="gd-icone"><mat-icon>event</mat-icon></span>
          <strong>{{ dados.emAndamento }}</strong>
        </a>
        <a class="gd-numero" routerLink="/eventos" [queryParams]="{ situacao: 'Concluido' }">
          <span>Concluído</span>
          <span class="gd-icone"><mat-icon>check_circle</mat-icon></span>
          <strong>{{ dados.concluidos }}</strong>
        </a>
        <a class="gd-numero" routerLink="/eventos" [queryParams]="{ situacao: 'Cancelado' }">
          <span>Cancelado</span>
          <span class="gd-icone"><mat-icon>cancel</mat-icon></span>
          <strong>{{ dados.cancelados }}</strong>
        </a>
        @for (estagio of dados.funil; track estagio.estagio) {
          <a class="gd-numero" routerLink="/oportunidades">
            <span>{{ estagio.estagio }}</span>
            <span class="gd-icone"><mat-icon>filter_alt</mat-icon></span>
            <strong>{{ estagio.quantidade }}</strong>
            <small>{{ estagio.valor }}</small>
          </a>
        }
      </section>
      <mat-card>
        <mat-card-header><mat-card-title>Eventos em andamento</mat-card-title></mat-card-header>
        <mat-card-content>
          @if (dados.eventosEmAndamento.length === 0) {
            <p>Não há evento.</p>
          } @else {
            <mat-nav-list>
              @for (evento of dados.eventosEmAndamento; track evento.id) {
                <a mat-list-item [routerLink]="['/eventos', evento.id]">
                  <span matListItemTitle>{{ evento.nome }}</span>
                  @if (evento.data) {
                    <span matListItemLine>{{ evento.data | gdData }}</span>
                  }
                </a>
              }
            </mat-nav-list>
          }
        </mat-card-content>
      </mat-card>
    }
  `,
  styles: `mat-card { margin-top: 16px; }`,
})
export class PainelPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly painel = signal<Painel | null>(null);

  ngOnInit(): void {
    this.negocio.painel().subscribe({
      next: (painel) => this.painel.set(painel),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
