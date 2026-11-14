import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

const TITULOS: Record<string, string> = {
  pagamentos: 'Pagamentos',
  anexos: 'Anexos',
  equipe: 'Equipe escalada',
  aniversarios: 'Aniversários',
};

@Component({
  selector: 'app-em-breve',
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>{{ titulo }}</h1>
        <p>Em breve — módulo em construção</p>
      </div>
    </header>
    <section class="gd-painel">
      <p class="gd-vazio">Em breve — módulo em construção</p>
    </section>
  `,
})
export class EmBrevePage {
  private readonly rota = inject(ActivatedRoute);
  protected readonly titulo =
    TITULOS[this.rota.snapshot.routeConfig?.path ?? ''] ?? 'Módulo';
}
