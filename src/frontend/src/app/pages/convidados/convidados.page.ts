import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { EventoResumo, ListaConvites, NegocioClient } from '../../core/negocio.client';

@Component({
  selector: 'app-convidados',
  imports: [
    ReactiveFormsModule, RouterLink, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonToggleModule, MatCheckboxModule,
  ],
  template: `
    @if (!eventoId()) {
      <h1>Convidados</h1>
      <p>Escolha o evento para ver os convites.</p>
      @for (evento of eventos(); track evento.id) {
        <p><a [routerLink]="['/eventos', evento.id, 'convidados']">{{ evento.nome }}</a></p>
      }
    } @else {
      <a mat-button [routerLink]="['/eventos', eventoId()]">Voltar ao evento</a>
      <h1>Convites e convidados</h1>
      <mat-button-toggle-group [value]="visao()" (change)="visao.set($event.value)">
        <mat-button-toggle value="convite">Por convite</mat-button-toggle>
        <mat-button-toggle value="convidado">Por convidado</mat-button-toggle>
      </mat-button-toggle-group>
      <form class="filtros" [formGroup]="filtro" (ngSubmit)="carregar()">
        <mat-form-field appearance="outline"><mat-label>Busca</mat-label><input matInput formControlName="nome" /></mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Situação</mat-label>
          <mat-select formControlName="situacao">
            <mat-option value="">Todas</mat-option>
            <mat-option value="Pendente">Pendente</mat-option>
            <mat-option value="Confirmado">Confirmado</mat-option>
            <mat-option value="NaoComparecera">Não comparecerá</mat-option>
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Telefone</mat-label><input matInput formControlName="telefone" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Observação</mat-label><input matInput formControlName="observacao" /></mat-form-field>
        <mat-checkbox formControlName="somenteConfirmados">Só confirmados</mat-checkbox>
        <button mat-stroked-button type="submit">Filtrar</button>
      </form>
      <section>
        <h2>Transfer do evento</h2>
        <mat-checkbox [checked]="transferAtivo()" (change)="transferAtivo.set($event.checked)">Transfer ativo</mat-checkbox>
        <mat-form-field appearance="outline"><mat-label>Locais, um por linha</mat-label><textarea matInput rows="2" [value]="locaisTexto()" (input)="locaisTexto.set(texto($event))"></textarea></mat-form-field>
        <button mat-stroked-button type="button" (click)="salvarTransfer()">Salvar transfer</button>
      </section>
      <section>
        <h2>Grupos</h2>
        <mat-form-field appearance="outline"><mat-label>Novo grupo</mat-label><input matInput [value]="grupoNome()" (input)="grupoNome.set(texto($event))" /></mat-form-field>
        <button mat-stroked-button type="button" (click)="criarGrupo()">Criar grupo</button>
        <p>{{ grupos().map((grupo) => grupo.nome).join(', ') }}</p>
      </section>
      <form class="formulario" [formGroup]="convite" (ngSubmit)="criarConvite()">
        <h2>Novo convite</h2>
        <mat-form-field appearance="outline"><mat-label>Nome do convite</mat-label><input matInput formControlName="nome" /></mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Grupo</mat-label>
          <mat-select formControlName="grupoId">
            <mat-option value="">Nenhum</mat-option>
            @for (grupo of grupos(); track grupo.id) { <mat-option [value]="grupo.id">{{ grupo.nome }}</mat-option> }
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Observação</mat-label><input matInput formControlName="observacao" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Nome do convidado</mat-label><input matInput formControlName="convidado" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>DDI</mat-label><input matInput formControlName="ddi" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Celular</mat-label><input matInput formControlName="celular" /></mat-form-field>
        @if (transferAtivo()) {
          <mat-checkbox formControlName="usaTransfer">Usa transfer</mat-checkbox>
          @if (convite.controls.usaTransfer.value) {
            <mat-form-field appearance="outline">
              <mat-label>Origem</mat-label>
              <mat-select formControlName="localOrigem">
                @for (local of locais(); track local) { <mat-option [value]="local">{{ local }}</mat-option> }
              </mat-select>
            </mat-form-field>
          }
        }
        <button mat-flat-button type="submit">Salvar convite</button>
      </form>
      <p>
        <button mat-stroked-button type="button" (click)="baixarPlanilha()">Planilha padrão</button>
        <label>Importar
          <input type="file" accept=".xlsx,.xls" (change)="importar($event)" />
        </label>
      </p>
      @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
      @if (lista(); as dados) {
        <h2>Totais</h2>
        <p>Situação: {{ dados.porSituacao.map((item) => item.rotulo + ' ' + item.quantidade).join(', ') }}</p>
        <p>Faixa: {{ dados.porFaixa.map((item) => item.rotulo + ' ' + item.quantidade).join(', ') }}</p>
        <p>Custo: {{ dados.porCusto.map((item) => item.rotulo + ' ' + item.quantidade).join(', ') }}</p>
        @if (visao() === 'convite') {
          @for (item of dados.convites; track item.id) {
            <mat-card>
              <mat-card-title>{{ item.nome }}</mat-card-title>
              <mat-card-content>
                <p>{{ item.grupo }} {{ item.telefone }}</p>
                <p>Histórico: @if (item.acessada) { acessada } @if (item.respondida) { respondida }</p>
                <button mat-button type="button" (click)="copiar(item.token)">Copiar link</button>
                <button mat-button type="button" (click)="baixarQr(item.id)">Baixar QR Code</button>
                <button mat-button type="button" (click)="whatsapp(item.id)">Enviar por WhatsApp</button>
                @for (pessoa of item.convidados; track pessoa.id) {
                  <p>{{ pessoa.nome }} — {{ pessoa.situacaoIcone }}
                    @if (pessoa.celular) {
                      <button mat-button type="button" (click)="principal(item.id, pessoa.id)">Usar este telefone</button>
                    }
                    @if (pessoa.principal) { <strong>Principal</strong> }
                  </p>
                }
              </mat-card-content>
            </mat-card>
          }
        } @else {
          @for (item of dados.convites; track item.id) {
            @for (pessoa of item.convidados; track pessoa.id) {
              <p>{{ pessoa.nome }} — {{ item.nome }} — {{ pessoa.situacaoTela }} — {{ pessoa.mesa }}</p>
            }
          }
        }
      }
    }
  `,
})
export class ConvidadosPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  protected readonly eventos = signal<EventoResumo[]>([]);
  protected readonly lista = signal<ListaConvites | null>(null);
  protected readonly grupos = signal<{ id: string; nome: string }[]>([]);
  protected readonly locais = signal<string[]>([]);
  protected readonly visao = signal('convite');
  protected readonly aviso = signal('');
  protected readonly eventoId = signal('');
  protected readonly transferAtivo = signal(false);
  protected readonly locaisTexto = signal('');
  protected readonly grupoNome = signal('');
  protected readonly filtro = this.fb.nonNullable.group({
    nome: '', situacao: '', telefone: '', observacao: '', somenteConfirmados: false,
  });
  protected readonly convite = this.fb.nonNullable.group({
    nome: '', grupoId: '', observacao: '', convidado: '', ddi: '+55', celular: '', usaTransfer: false, localOrigem: '',
  });

  ngOnInit(): void {
    const id = this.rota.snapshot.paramMap.get('id') ?? '';
    this.eventoId.set(id);
    if (!id) {
      this.negocio.eventos({ situacao: 'EmAndamento' }).subscribe({
        next: (eventos) => this.eventos.set(eventos),
        error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
      });
      return;
    }
    this.negocio.evento(id).subscribe({
      next: (evento) => {
        this.transferAtivo.set(evento.transferAtivo);
        this.locais.set(evento.locaisTransfer);
        this.locaisTexto.set(evento.locaisTransfer.join('\n'));
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
    this.negocio.grupos().subscribe({ next: (grupos) => this.grupos.set(grupos) });
    this.carregar();
  }

  protected texto(evento: Event): string {
    return (evento.target as HTMLInputElement | HTMLTextAreaElement).value;
  }

  protected carregar(): void {
    const valor = this.filtro.getRawValue();
    this.negocio.convites(this.eventoId(), {
      nome: valor.nome,
      situacao: valor.situacao,
      telefone: valor.telefone,
      observacao: valor.observacao,
      somenteConfirmados: valor.somenteConfirmados ? 'true' : '',
    }).subscribe({
      next: (lista) => {
        this.lista.set(lista);
        this.aviso.set('');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected salvarTransfer(): void {
    const locais = this.locaisTexto().split('\n').map((item) => item.trim()).filter(Boolean);
    this.negocio.transfer(this.eventoId(), this.transferAtivo(), locais).subscribe({
      next: (evento) => {
        this.locais.set(evento.locaisTransfer);
        this.aviso.set('Transfer salvo.');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected criarGrupo(): void {
    this.negocio.criarGrupo(this.grupoNome()).subscribe({
      next: (grupo) => {
        this.grupos.update((lista) => [...lista, grupo]);
        this.grupoNome.set('');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected criarConvite(): void {
    const valor = this.convite.getRawValue();
    this.negocio.criarConvite(this.eventoId(), {
      nome: valor.nome,
      grupoId: valor.grupoId || null,
      observacao: valor.observacao,
      usaTransfer: valor.usaTransfer,
      localOrigem: valor.localOrigem,
      principal: valor.celular ? 0 : null,
      convidados: [{
        nome: valor.convidado,
        ddi: valor.ddi,
        celular: valor.celular,
        situacao: 'Pendente',
      }],
    }).subscribe({
      next: () => {
        this.convite.reset({ ddi: '+55', usaTransfer: false });
        this.carregar();
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected principal(conviteId: string, convidadoId: string): void {
    this.negocio.definirPrincipal(conviteId, convidadoId).subscribe({
      next: () => this.carregar(),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected copiar(token: string): void {
    const link = `${location.origin}/rsvp/${token}`;
    void navigator.clipboard.writeText(link);
    this.aviso.set('Link copiado.');
  }

  protected baixarQr(id: string): void {
    this.negocio.qr(id).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'convite.png';
        link.click();
        URL.revokeObjectURL(url);
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected whatsapp(id: string): void {
    this.negocio.whatsapp(id).subscribe({
      next: (resposta) => {
        if (!resposta.url) {
          this.aviso.set('O convite não tem telefone de principal.');
          return;
        }
        window.open(resposta.url, '_blank', 'noopener');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected baixarPlanilha(): void {
    this.negocio.planilha(this.eventoId()).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'convidados.xlsx';
        link.click();
        URL.revokeObjectURL(url);
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected importar(evento: Event): void {
    const arquivo = (evento.target as HTMLInputElement).files?.[0];
    if (!arquivo) {
      return;
    }
    this.negocio.importar(this.eventoId(), arquivo).subscribe({
      next: () => this.carregar(),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }
}
