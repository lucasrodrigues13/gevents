import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { LeadRsvp, NegocioClient } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { GdDataPipe } from '../../shared/gd-data.pipe';

@Component({
  selector: 'app-leads-rsvp',
  imports: [MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule, GdDataPipe],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Oportunidades de RSVP</h1>
        <p>Leads com intenção de realizar evento nos próximos dois anos.</p>
      </div>
    </header>
    <section class="gd-painel">
      <div class="gd-barra">
        <span></span>
        <mat-form-field appearance="outline" subscriptSizing="dynamic">
          <mat-label>Buscar por convite ou evento</mat-label>
          <input matInput [value]="busca()" (input)="buscar($event)" />
        </mat-form-field>
      </div>
      @if (lista().length === 0) {
        <p class="gd-vazio">Não há lead de RSVP.</p>
      } @else {
        <table class="gd-grade" mat-table [dataSource]="lista()">
          <ng-container matColumnDef="convite">
            <th mat-header-cell *matHeaderCellDef>Convite</th>
            <td mat-cell *matCellDef="let item">{{ item.convite }}</td>
          </ng-container>
          <ng-container matColumnDef="evento">
            <th mat-header-cell *matHeaderCellDef>Evento</th>
            <td mat-cell *matCellDef="let item">{{ item.evento }}</td>
          </ng-container>
          <ng-container matColumnDef="criadoEm">
            <th mat-header-cell *matHeaderCellDef>Criado em</th>
            <td mat-cell *matCellDef="let item">{{ item.criadoEm | gdData }}</td>
          </ng-container>
          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
            <td mat-cell *matCellDef="let item" class="gd-acoes">
              <button mat-icon-button type="button" aria-label="Gerar oportunidade" (click)="gerar(item)"><mat-icon>trending_up</mat-icon></button>
              <button mat-icon-button type="button" aria-label="Contatar" (click)="contatar(item)"><mat-icon>phone</mat-icon></button>
              <button mat-icon-button type="button" aria-label="Descartar" (click)="descartar(item)"><mat-icon>delete</mat-icon></button>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="colunas"></tr>
          <tr mat-row *matRowDef="let row; columns: colunas"></tr>
        </table>
      }
    </section>
  `,
})
export class LeadsRsvpPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly lista = signal<LeadRsvp[]>([]);
  protected readonly busca = signal('');
  protected readonly colunas = ['convite', 'evento', 'criadoEm', 'acoes'];

  ngOnInit(): void {
    this.carregar();
  }

  protected buscar(evento: Event): void {
    this.busca.set((evento.target as HTMLInputElement).value);
    this.carregar();
  }

  protected gerar(item: LeadRsvp): void {
    this.negocio.gerarOportunidadeLead(item.id).subscribe({
      next: () => {
        this.toast.sucesso('Oportunidade gerada.');
        this.carregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected descartar(item: LeadRsvp): void {
    this.negocio.descartarLead(item.id).subscribe({
      next: () => {
        this.toast.sucesso('Lead descartado.');
        this.carregar();
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected contatar(item: LeadRsvp): void {
    this.negocio.contatarLead(item.id).subscribe({
      next: () => {
        if (item.telefone) {
          window.open(`https://wa.me/${item.telefone.replace(/\D/g, '')}`, '_blank');
        }
        this.toast.sucesso('Contato registrado.');
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  private carregar(): void {
    this.negocio.leadsRsvp(this.busca()).subscribe({
      next: (lista) => this.lista.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
