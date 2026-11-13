import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { AuthService } from '../core/auth.service';
import { ContaClient } from '../core/conta.client';

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
        <div class="gd-grupo">Gestão</div>
        <nav>
          <a class="gd-item" routerLink="/painel" routerLinkActive="ativo"><mat-icon>dashboard</mat-icon>Dashboards</a>
          <a class="gd-item" routerLink="/perfil" routerLinkActive="ativo"><mat-icon>apartment</mat-icon>Perfil</a>
          <a class="gd-item" routerLink="/eventos" routerLinkActive="ativo"><mat-icon>celebration</mat-icon>Eventos</a>
          <a class="gd-item" routerLink="/tarefas" routerLinkActive="ativo"><mat-icon>task_alt</mat-icon>Tarefas</a>
          <a class="gd-item" routerLink="/clientes" routerLinkActive="ativo"><mat-icon>group</mat-icon>Clientes</a>
          <a class="gd-item" routerLink="/convidados" routerLinkActive="ativo"><mat-icon>mail</mat-icon>Convidados</a>
          @if (auth.ehAdmin()) {
            <a class="gd-item" routerLink="/admin" routerLinkActive="ativo"><mat-icon>admin_panel_settings</mat-icon>Administração</a>
          }
        </nav>
      </mat-sidenav>
      <mat-sidenav-content>
        <header class="gd-topo">
          <span>{{ auth.email() }}</span>
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
