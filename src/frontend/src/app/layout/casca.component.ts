import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../core/auth.service';
import { ContaClient } from '../core/conta.client';

@Component({
  selector: 'app-casca',
  imports: [RouterOutlet, RouterLink, MatSidenavModule, MatToolbarModule, MatButtonModule],
  styles: `
    mat-sidenav-container {
      height: 100%;
    }

    mat-sidenav {
      width: 220px;
      padding: 12px;
    }

    mat-toolbar {
      gap: 12px;
    }

    .espaco {
      flex: 1;
    }

    main {
      padding: 24px;
    }

    nav {
      display: flex;
      flex-direction: column;
      align-items: stretch;
    }
  `,
  template: `
    <mat-sidenav-container>
      <mat-sidenav mode="side" opened>
        <nav>
          <a mat-button routerLink="/painel">Dashboards</a>
          <a mat-button routerLink="/perfil">Perfil</a>
          <a mat-button routerLink="/eventos">Eventos</a>
          <a mat-button routerLink="/tarefas">Tarefas</a>
          <a mat-button routerLink="/clientes">Clientes</a>
          <a mat-button routerLink="/convidados">Convidados</a>
          @if (auth.ehAdmin()) {
            <a mat-button routerLink="/admin">Administração</a>
          }
        </nav>
      </mat-sidenav>
      <mat-sidenav-content>
        <mat-toolbar>
          <span>{{ auth.email() }}</span>
          <span class="espaco"></span>
          <button mat-button type="button" (click)="sair()">Sair</button>
        </mat-toolbar>
        <main>
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
