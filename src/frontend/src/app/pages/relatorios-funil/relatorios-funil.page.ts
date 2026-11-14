import { DecimalPipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { CatalogoRelatoriosFunil, NegocioClient } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { CampoDataComponent } from '../../shared/campo-data';
import { deDataParaIso } from '../../shared/data';

@Component({
  selector: 'app-relatorios-funil',
  imports: [DecimalPipe, ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatIconModule, MatTableModule, CampoDataComponent],
  template: `
    <header class="gd-cabecalho">
      <div><h1>Relatórios do funil</h1><p>Catálogo de métricas comerciais da conta.</p></div>
      <button mat-stroked-button type="button" (click)="exportar()"><mat-icon>download</mat-icon> Exportar</button>
    </header>
    <section class="gd-painel">
      <form class="gd-barra" [formGroup]="periodo" (ngSubmit)="carregar()">
        <app-campo-data formControlName="inicio" rotulo="Início" />
        <app-campo-data formControlName="fim" rotulo="Fim" />
        <button mat-flat-button type="submit">Atualizar</button>
      </form>
      @if (dados(); as catalogo) {
        <div class="catalogo">
          @for (bloco of blocos(catalogo); track bloco.titulo) {
            <article>
              <h2>{{ bloco.titulo }}</h2>
              @if (bloco.linhas.length === 0) {
                <p class="gd-vazio">Sem movimento no período.</p>
              } @else {
                <table class="gd-grade" mat-table [dataSource]="bloco.linhas">
                  <ng-container matColumnDef="chave">
                    <th mat-header-cell *matHeaderCellDef>Item</th>
                    <td mat-cell *matCellDef="let item">{{ item.chave }}</td>
                  </ng-container>
                  <ng-container matColumnDef="quantidade">
                    <th mat-header-cell *matHeaderCellDef>Qtd</th>
                    <td mat-cell *matCellDef="let item">{{ item.quantidade }}</td>
                  </ng-container>
                  <ng-container matColumnDef="valor">
                    <th mat-header-cell *matHeaderCellDef>Valor</th>
                    <td mat-cell *matCellDef="let item">{{ item.valor | number: '1.2-2' }}</td>
                  </ng-container>
                  <tr mat-header-row *matHeaderRowDef="colunas"></tr>
                  <tr mat-row *matRowDef="let row; columns: colunas"></tr>
                </table>
              }
            </article>
          }
          <article>
            <h2>Oportunidades de RSVP</h2>
            <div class="gd-numeros">
              <div class="gd-numero"><span>Visualizações</span><strong>{{ catalogo.oportunidadesRsvp.visualizacoes }}</strong></div>
              <div class="gd-numero"><span>Cliques</span><strong>{{ catalogo.oportunidadesRsvp.cliquesBotao }}</strong></div>
              <div class="gd-numero"><span>Contatados</span><strong>{{ catalogo.oportunidadesRsvp.contatados }}</strong></div>
              <div class="gd-numero"><span>Descartados</span><strong>{{ catalogo.oportunidadesRsvp.descartados }}</strong></div>
              <div class="gd-numero"><span>Geradas</span><strong>{{ catalogo.oportunidadesRsvp.geradas }}</strong></div>
              <div class="gd-numero"><span>Conversão %</span><strong>{{ catalogo.oportunidadesRsvp.conversao }}</strong></div>
              <div class="gd-numero"><span>Conversões anteriores</span><strong>{{ catalogo.oportunidadesRsvp.conversoesAnteriores }}</strong></div>
            </div>
          </article>
        </div>
      }
    </section>
  `,
  styles: `
    .catalogo { display: grid; gap: 16px; }
    article h2 { margin: 0 0 8px; font-size: 1rem; }
  `,
})
export class RelatoriosFunilPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);
  protected readonly dados = signal<CatalogoRelatoriosFunil | null>(null);
  protected readonly colunas = ['chave', 'quantidade', 'valor'];
  protected readonly periodo = this.fb.nonNullable.group({
    inicio: this.fb.control<Date | null>(primeiroDoMes(), Validators.required),
    fim: this.fb.control<Date | null>(new Date(), Validators.required),
  });

  ngOnInit(): void {
    this.carregar();
  }

  protected blocos(catalogo: CatalogoRelatoriosFunil): { titulo: string; linhas: { chave: string; quantidade: number; valor: number }[] }[] {
    return [
      { titulo: '1. Negócios fechados x perdidos', linhas: catalogo.fechadosPerdidos },
      { titulo: '2. Por fonte', linhas: catalogo.porFonte },
      { titulo: '3. Por serviço/produto', linhas: catalogo.porServicoProduto },
      { titulo: '4. Por estágio', linhas: catalogo.porEstagio },
      {
        titulo: '5. Por região',
        linhas: catalogo.porRegiao.map((r) => ({ chave: `${r.uf} / ${r.cidade}`, quantidade: r.quantidade, valor: r.valor })),
      },
      { titulo: '6. Por responsável', linhas: catalogo.porResponsavel },
      { titulo: '7. Por data prevista', linhas: catalogo.porDataPrevista },
      { titulo: '8. Por motivos de descarte', linhas: catalogo.porMotivosDescarte },
    ];
  }

  protected carregar(): void {
    if (this.periodo.invalid) {
      this.toast.erro('Informe um período válido.');
      return;
    }
    const v = this.periodo.getRawValue();
    const inicio = deDataParaIso(v.inicio)!;
    const fim = deDataParaIso(v.fim)!;
    this.negocio.relatoriosFunil(inicio, fim).subscribe({
      next: (dados) => this.dados.set(dados),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected exportar(): void {
    const v = this.periodo.getRawValue();
    const inicio = deDataParaIso(v.inicio);
    const fim = deDataParaIso(v.fim);
    if (!inicio || !fim) return;
    this.negocio.exportarRelatoriosFunil(inicio, fim).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'relatorios-funil.csv';
        a.click();
        URL.revokeObjectURL(url);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}

function primeiroDoMes(): Date {
  const hoje = new Date();
  return new Date(hoje.getFullYear(), hoje.getMonth(), 1);
}
