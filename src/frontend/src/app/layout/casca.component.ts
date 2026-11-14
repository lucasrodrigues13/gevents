import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { AuthService } from '../core/auth.service';
import { ContaClient } from '../core/conta.client';

interface ItemMenu {
  rotulo: string;
  icone: string;
  rota?: string;
  emBreve?: boolean;
}

interface GrupoMenu {
  rotulo: string;
  itens: ItemMenu[];
}

@Component({
  selector: 'app-casca',
  host: { class: 'gd-casca' },
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatSidenavModule, MatButtonModule, MatIconModule],
  template: `
    <mat-sidenav-container>
      <mat-sidenav class="gd-sidebar" mode="side" opened>
        <div class="gd-marca">
          <span class="gd-marca-selo">G</span>
          <span>gevents</span>
        </div>
        @for (grupo of grupos; track grupo.rotulo) {
          <div class="gd-grupo">{{ grupo.rotulo }}</div>
          <nav>
            @for (item of grupo.itens; track item.rotulo) {
              @if (item.rota && !item.emBreve) {
                <a class="gd-item" [routerLink]="item.rota" routerLinkActive="ativo">
                  <mat-icon>{{ item.icone }}</mat-icon>
                  <span>{{ item.rotulo }}</span>
                </a>
              } @else {
                <span class="gd-item gd-item-breve" title="Em breve" aria-disabled="true">
                  <mat-icon>{{ item.icone }}</mat-icon>
                  <span>{{ item.rotulo }}</span>
                  <small>Em breve</small>
                </span>
              }
            }
          </nav>
        }
        @if (auth.ehAdmin()) {
          <div class="gd-grupo">Sistema</div>
          <nav>
            <a class="gd-item" routerLink="/admin" routerLinkActive="ativo">
              <mat-icon>admin_panel_settings</mat-icon>
              <span>Administração</span>
            </a>
          </nav>
        }
      </mat-sidenav>
      <mat-sidenav-content>
        <header class="gd-topo">
          <div class="gd-topo-titulo">
            <strong>Painel</strong>
            <span>{{ auth.email() }}</span>
          </div>
          <span class="espaco"></span>
          <button mat-button type="button" (click)="sair()">Sair</button>
        </header>
        <main class="gd-conteudo">
          <router-outlet />
        </main>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
})
export class CascaComponent {
  protected readonly auth = inject(AuthService);
  private readonly conta = inject(ContaClient);
  private readonly router = inject(Router);

  protected readonly grupos: GrupoMenu[] = [
    {
      rotulo: 'Principal',
      itens: [
        { rotulo: 'Dashboards', icone: 'dashboard', rota: '/painel' },
        { rotulo: 'Perfil', icone: 'apartment', rota: '/perfil' },
      ],
    },
    {
      rotulo: 'Gestão de eventos',
      itens: [
        { rotulo: 'Eventos', icone: 'celebration', rota: '/eventos' },
        { rotulo: 'Tarefas', icone: 'task_alt', rota: '/tarefas' },
        { rotulo: 'Clientes', icone: 'group', rota: '/clientes' },
        { rotulo: 'Convidados', icone: 'mail', rota: '/convidados' },
        { rotulo: 'Modelos de evento', icone: 'content_copy', rota: '/modelos-evento' },
        { rotulo: 'Modelos de briefing', icone: 'quiz', rota: '/modelos-briefing' },
        { rotulo: 'Modelos de roteiro', icone: 'schedule', rota: '/modelos-roteiro' },
        { rotulo: 'Modelos de cronograma', icone: 'view_timeline', rota: '/modelos-cronograma' },
        { rotulo: 'Modelos de inspirações', icone: 'photo_library', rota: '/modelos-inspiracoes' },
        { rotulo: 'Modelos de mensagens', icone: 'sms', rota: '/modelos-mensagens' },
        { rotulo: 'Serviços e fornecedores', icone: 'storefront', rota: '/fornecedores' },
      ],
    },
    {
      rotulo: 'Agenda',
      itens: [{ rotulo: 'Minha agenda', icone: 'calendar_month', rota: '/agenda' }],
    },
    {
      rotulo: 'Funil de vendas',
      itens: [
        { rotulo: 'Oportunidades', icone: 'trending_up', rota: '/oportunidades' },
        { rotulo: 'Oportunidades de RSVP', icone: 'rsvp', rota: '/leads-rsvp' },
        { rotulo: 'Contratos', icone: 'description', rota: '/contratos-funil' },
        { rotulo: 'Orçamentos', icone: 'request_quote', rota: '/orcamentos-funil' },
        { rotulo: 'Tarefas do funil', icone: 'checklist', rota: '/tarefas-funil' },
        { rotulo: 'Relatórios', icone: 'analytics', rota: '/relatorios-funil' },
        { rotulo: 'Modelos de serviços', icone: 'category', rota: '/modelos-servicos-funil' },
        { rotulo: 'Modelo de orçamento', icone: 'receipt_long', rota: '/modelos-orcamento-funil' },
        { rotulo: 'Modelos de contrato', icone: 'article', rota: '/modelos-contrato-funil' },
      ],
    },
  ];

  protected sair(): void {
    this.conta.sair().subscribe({
      next: () => this.encerrar(),
      error: () => this.encerrar(),
    });
  }

  private encerrar(): void {
    this.auth.sair();
    void this.router.navigateByUrl('/entrar');
  }
}
