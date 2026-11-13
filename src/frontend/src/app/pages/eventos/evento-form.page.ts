import { NgTemplateOutlet } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Modelo, NegocioClient } from '../../core/negocio.client';

@Component({
  selector: 'app-evento-form',
  imports: [
    NgTemplateOutlet, ReactiveFormsModule, RouterLink, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatCheckboxModule, MatTabsModule,
  ],
  template: `
    <a mat-button routerLink="/eventos">Voltar</a>
    <h1>{{ editando() ? 'Editar evento' : 'Novo evento' }}</h1>
    @if (!editando()) {
      <mat-card>
        <mat-card-header><mat-card-title>Modelo</mat-card-title></mat-card-header>
        <mat-card-content>
          <mat-form-field appearance="outline">
            <mat-label>Modelo de evento</mat-label>
            <mat-select [value]="modeloId()" (valueChange)="escolherModelo($event)">
              @for (modelo of modelos(); track modelo.id) {
                <mat-option [value]="modelo.id">{{ modelo.nome }}</mat-option>
              }
            </mat-select>
          </mat-form-field>
          <form class="formulario" [formGroup]="novoModelo" (ngSubmit)="criarModelo()">
            <mat-form-field appearance="outline"><mat-label>Novo modelo</mat-label><input matInput formControlName="nome" /></mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Tipo</mat-label>
              <mat-select formControlName="tipo">
                <mat-option value="Casamento">Casamento</mat-option>
                <mat-option value="QuinzeAnos">15 anos</mat-option>
                <mat-option value="Aniversario">Aniversário</mat-option>
                <mat-option value="Corporativo">Corporativo</mat-option>
                <mat-option value="Formatura">Formatura</mat-option>
              </mat-select>
            </mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Serviços, um por linha</mat-label><textarea matInput rows="3" formControlName="servicos"></textarea></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Tarefas, uma por linha</mat-label><textarea matInput rows="3" formControlName="tarefas"></textarea></mat-form-field>
            <button mat-stroked-button type="submit">Criar modelo</button>
          </form>
        </mat-card-content>
      </mat-card>
    }
    @if (modeloId() || editando()) {
      <form class="formulario" [formGroup]="formulario" (ngSubmit)="salvar()">
        @if (editando()) {
          <mat-tab-group>
            <mat-tab label="Informações gerais"><ng-container *ngTemplateOutlet="geral" /></mat-tab>
            <mat-tab label="Detalhes do evento"><ng-container *ngTemplateOutlet="detalhes" /></mat-tab>
            <mat-tab label="Cerimônia"><ng-container *ngTemplateOutlet="local; context: { grupo: 'cerimonia' }" /></mat-tab>
            <mat-tab label="Recepção"><ng-container *ngTemplateOutlet="local; context: { grupo: 'recepcao' }" /></mat-tab>
          </mat-tab-group>
        } @else {
          <ng-container *ngTemplateOutlet="geral" />
          <h2>Local da cerimônia</h2>
          <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="cerimoniaNome" /></mat-form-field>
          <h2>Local da recepção</h2>
          <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="recepcaoNome" /></mat-form-field>
        }
        @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
        <button mat-flat-button type="submit">Salvar</button>
      </form>
    }
    <ng-template #geral>
        <label>Foto <input type="file" accept="image/jpeg,image/png,image/webp,.jpg,.jpeg,.png,.webp" (change)="escolherFoto($event)" /></label>
        @if (tipo() === 'Casamento') {
          <mat-form-field appearance="outline"><mat-label>Nome da noiva</mat-label><input matInput formControlName="primeiroNome" /></mat-form-field>
          <mat-form-field appearance="outline"><mat-label>Nome do noivo</mat-label><input matInput formControlName="segundoNome" /></mat-form-field>
        } @else {
          <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="primeiroNome" /></mat-form-field>
        }
        <mat-form-field appearance="outline"><mat-label>Data</mat-label><input matInput type="date" formControlName="data" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Horário do convite</mat-label><input matInput type="time" formControlName="hora" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Budget</mat-label><input matInput type="number" min="0" formControlName="orcamentoEstimado" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Convidados estimados</mat-label><input matInput type="number" min="0" formControlName="convidadosEstimados" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Tags</mat-label><input matInput formControlName="tags" placeholder="Assessoria completa" /></mat-form-field>
        @if (editando()) {
          <mat-form-field appearance="outline"><mat-label>URL do site</mat-label><input matInput [value]="urlSite()" readonly /></mat-form-field>
          <mat-form-field appearance="outline"><mat-label>Senha do site</mat-label><input matInput formControlName="senhaSite" /></mat-form-field>
        }
        <mat-checkbox formControlName="notificarAssessor">Notificar assessor(a)</mat-checkbox>
        <mat-checkbox formControlName="notificarAssistentes">Notificar assistentes</mat-checkbox>
        <mat-checkbox formControlName="notificarClientes">Notificar clientes</mat-checkbox>
        <mat-checkbox formControlName="salvarNaAgenda">Salvar na agenda</mat-checkbox>
        @if (formulario.controls.salvarNaAgenda.value) { <p>O vínculo com o Google fica vazio.</p> }
      </ng-template>
      <ng-template #detalhes>
        <mat-form-field appearance="outline"><mat-label>Tempo de festa</mat-label><input matInput type="time" formControlName="tempoFesta" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Início da cerimônia</mat-label><input matInput type="time" formControlName="inicioCerimonia" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Início da recepção</mat-label><input matInput type="time" formControlName="inicioRecepcao" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Padrinhos</mat-label><input matInput type="number" min="0" formControlName="quantidadePadrinhos" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Damas e pajens</mat-label><input matInput type="number" min="0" formControlName="quantidadeDamas" /></mat-form-field>
      </ng-template>
      <ng-template #local let-grupo="grupo">
        <mat-form-field appearance="outline"><mat-label>Local</mat-label><input matInput [formControlName]="grupo + 'Nome'" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>País</mat-label><input matInput [formControlName]="grupo + 'Pais'" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>CEP</mat-label><input matInput [formControlName]="grupo + 'Cep'" (blur)="completarCep(grupo)" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Endereço</mat-label><input matInput [formControlName]="grupo + 'Endereco'" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Número</mat-label><input matInput [formControlName]="grupo + 'Numero'" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Estado</mat-label><input matInput [formControlName]="grupo + 'Estado'" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Cidade</mat-label><input matInput [formControlName]="grupo + 'Cidade'" /></mat-form-field>
        <mat-form-field appearance="outline"><mat-label>Bairro</mat-label><input matInput [formControlName]="grupo + 'Bairro'" /></mat-form-field>
    </ng-template>
    @if (eventoCriado(); as id) {
      <mat-card>
        <mat-card-header><mat-card-title>Valor da assessoria</mat-card-title></mat-card-header>
        <mat-card-content>
          <form class="formulario" [formGroup]="assessoria" (ngSubmit)="gravarAssessoria(id)">
            <mat-form-field appearance="outline"><mat-label>Valor total</mat-label><input matInput type="number" min="0.01" step="0.01" formControlName="valorTotal" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Quantidade de parcelas</mat-label><input matInput type="number" min="1" formControlName="quantidadeParcelas" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Primeira parcela</mat-label><input matInput type="date" formControlName="primeiraParcela" /></mat-form-field>
            <mat-checkbox formControlName="comEntrada">Incluir entrada</mat-checkbox>
            @if (assessoria.controls.comEntrada.value) {
              <mat-form-field appearance="outline">
                <mat-label>Base</mat-label>
                <mat-select formControlName="baseEntrada">
                  <mat-option value="Valor">Valor</mat-option>
                  <mat-option value="Porcentagem">Porcentagem</mat-option>
                </mat-select>
              </mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Valor de entrada</mat-label><input matInput type="number" formControlName="valorEntrada" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Data de entrada</mat-label><input matInput type="date" formControlName="dataEntrada" /></mat-form-field>
            }
            <button mat-flat-button type="submit">Gerar parcelas</button>
            <a mat-button [routerLink]="['/eventos', id]">Agora não</a>
          </form>
        </mat-card-content>
      </mat-card>
    }
  `,
})
export class EventoFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly modelos = signal<Modelo[]>([]);
  protected readonly modeloId = signal('');
  protected readonly tipo = signal('');
  protected readonly editando = signal(false);
  protected readonly eventoCriado = signal('');
  protected readonly aviso = signal('');
  protected readonly urlSite = signal('');
  private foto: File | null = null;
  private id = '';

  protected readonly novoModelo = this.fb.nonNullable.group({
    nome: ['', Validators.required],
    tipo: ['Casamento', Validators.required],
    servicos: '',
    tarefas: '',
  });

  protected readonly formulario = this.fb.nonNullable.group({
    primeiroNome: '',
    segundoNome: '',
    data: '',
    hora: '',
    orcamentoEstimado: '',
    convidadosEstimados: '',
    tags: '',
    senhaSite: '',
    notificarAssessor: true,
    notificarAssistentes: true,
    notificarClientes: true,
    salvarNaAgenda: false,
    tempoFesta: '',
    inicioCerimonia: '',
    inicioRecepcao: '',
    quantidadePadrinhos: '',
    quantidadeDamas: '',
    cerimoniaNome: '', cerimoniaPais: '', cerimoniaCep: '', cerimoniaEndereco: '', cerimoniaNumero: '', cerimoniaEstado: '', cerimoniaCidade: '', cerimoniaBairro: '',
    recepcaoNome: '', recepcaoPais: '', recepcaoCep: '', recepcaoEndereco: '', recepcaoNumero: '', recepcaoEstado: '', recepcaoCidade: '', recepcaoBairro: '',
  });

  protected readonly assessoria = this.fb.nonNullable.group({
    valorTotal: '',
    quantidadeParcelas: '1',
    primeiraParcela: '',
    comEntrada: false,
    baseEntrada: 'Valor',
    valorEntrada: '',
    dataEntrada: '',
  });

  ngOnInit(): void {
    this.id = this.rota.snapshot.paramMap.get('id') ?? '';
    this.editando.set(!!this.id);
    this.negocio.modelos().subscribe({
      next: (modelos) => this.modelos.set(modelos),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
    if (this.id) {
      this.negocio.evento(this.id).subscribe({
        next: (evento) => {
          this.tipo.set(evento.tipo);
          this.urlSite.set(evento.urlSite ?? '');
          this.formulario.patchValue({
            primeiroNome: evento.primeiroNome ?? '',
            segundoNome: evento.segundoNome ?? '',
            data: evento.data ?? '',
            hora: (evento.hora ?? '').slice(0, 5),
            orcamentoEstimado: evento.orcamentoEstimado?.toString() ?? '',
            convidadosEstimados: evento.convidadosEstimados?.toString() ?? '',
            tags: evento.tags.join(', '),
            senhaSite: evento.senhaSite ?? '',
            notificarAssessor: evento.notificarAssessor,
            notificarAssistentes: evento.notificarAssistentes,
            notificarClientes: evento.notificarClientes,
            salvarNaAgenda: evento.salvarNaAgenda,
            tempoFesta: (evento.tempoFesta ?? '').slice(0, 5),
            inicioCerimonia: (evento.inicioCerimonia ?? '').slice(0, 5),
            inicioRecepcao: (evento.inicioRecepcao ?? '').slice(0, 5),
            quantidadePadrinhos: evento.quantidadePadrinhos?.toString() ?? '',
            quantidadeDamas: evento.quantidadeDamas?.toString() ?? '',
            ...this.localParaForm('cerimonia', evento.cerimonia),
            ...this.localParaForm('recepcao', evento.recepcao),
          });
        },
        error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
      });
    }
  }

  protected escolherModelo(id: string): void {
    this.modeloId.set(id);
    const modelo = this.modelos().find((item) => item.id === id);
    this.tipo.set(modelo?.tipo ?? '');
  }

  protected criarModelo(): void {
    const valor = this.novoModelo.getRawValue();
    this.negocio.criarModelo({
      nome: valor.nome,
      tipo: valor.tipo,
      servicos: valor.servicos.split('\n').map((item) => item.trim()).filter(Boolean),
      tarefas: valor.tarefas.split('\n').map((item) => item.trim()).filter(Boolean),
    }).subscribe({
      next: (modelo) => {
        this.modelos.update((lista) => [...lista, modelo]);
        this.escolherModelo(modelo.id);
        this.aviso.set('');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected escolherFoto(evento: Event): void {
    const entrada = evento.target as HTMLInputElement;
    this.foto = entrada.files?.[0] ?? null;
  }

  protected completarCep(grupo: 'cerimonia' | 'recepcao'): void {
    const controle = this.formulario.get(`${grupo}Cep`);
    const digitos = (controle?.value ?? '').replace(/\D/g, '');
    if (digitos.length !== 8) {
      return;
    }
    this.negocio.cep(digitos).subscribe({
      next: (endereco) => {
        this.formulario.patchValue({
          [`${grupo}Endereco`]: endereco.endereco,
          [`${grupo}Estado`]: endereco.estado,
          [`${grupo}Cidade`]: endereco.cidade,
          [`${grupo}Bairro`]: endereco.bairro,
        });
      },
      error: () => undefined,
    });
  }

  protected salvar(): void {
    const pedido = this.pedido();
    const destino = this.editando()
      ? this.negocio.atualizarEvento(this.id, pedido)
      : this.negocio.criarEvento({ ...pedido, modeloId: this.modeloId() });
    destino.subscribe({
      next: (evento) => {
        const seguir = () => {
          if (this.editando()) {
            void this.router.navigate(['/eventos', evento.id]);
          } else {
            this.eventoCriado.set(evento.id);
          }
        };
        if (!this.foto) {
          seguir();
          return;
        }
        this.negocio.imagemEvento(evento.id, this.foto).subscribe({
          next: seguir,
          error: (erro: unknown) => {
            this.aviso.set(mensagemDeErro(erro));
            seguir();
          },
        });
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected gravarAssessoria(id: string): void {
    const valor = this.assessoria.getRawValue();
    this.negocio.assessoria(id, {
      valorTotal: this.numero(valor.valorTotal),
      quantidadeParcelas: this.numero(valor.quantidadeParcelas),
      primeiraParcela: valor.primeiraParcela || null,
      comEntrada: valor.comEntrada,
      baseEntrada: valor.comEntrada ? valor.baseEntrada : null,
      valorEntrada: valor.comEntrada ? this.numero(valor.valorEntrada) : null,
      dataEntrada: valor.comEntrada ? valor.dataEntrada || null : null,
    }).subscribe({
      next: () => void this.router.navigate(['/eventos', id]),
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  private pedido() {
    const valor = this.formulario.getRawValue();
    return {
      primeiroNome: valor.primeiroNome,
      segundoNome: valor.segundoNome,
      data: valor.data || null,
      hora: this.hora(valor.hora),
      orcamentoEstimado: this.numero(valor.orcamentoEstimado),
      convidadosEstimados: this.numero(valor.convidadosEstimados),
      tags: valor.tags.split(',').map((item) => item.trim()).filter(Boolean),
      senhaSite: valor.senhaSite,
      notificarAssessor: valor.notificarAssessor,
      notificarAssistentes: valor.notificarAssistentes,
      notificarClientes: valor.notificarClientes,
      salvarNaAgenda: valor.salvarNaAgenda,
      tempoFesta: this.hora(valor.tempoFesta),
      inicioCerimonia: this.hora(valor.inicioCerimonia),
      inicioRecepcao: this.hora(valor.inicioRecepcao),
      quantidadePadrinhos: this.numero(valor.quantidadePadrinhos),
      quantidadeDamas: this.numero(valor.quantidadeDamas),
      cerimonia: this.local('cerimonia'),
      recepcao: this.local('recepcao'),
      cadastrarCliente: false,
    };
  }

  private local(grupo: 'cerimonia' | 'recepcao') {
    const valor = this.formulario.getRawValue() as unknown as Record<string, string | boolean | null>;
    const campo = (nome: string) => {
      const item = valor[`${grupo}${nome}`];
      return typeof item === 'string' && item !== '' ? item : null;
    };
    return {
      nome: campo('Nome'), pais: campo('Pais'), cep: campo('Cep'), endereco: campo('Endereco'),
      numero: campo('Numero'), estado: campo('Estado'), cidade: campo('Cidade'), bairro: campo('Bairro'),
    };
  }

  private localParaForm(grupo: 'cerimonia' | 'recepcao', local: { nome: string | null; pais: string | null; cep: string | null; endereco: string | null; numero: string | null; estado: string | null; cidade: string | null; bairro: string | null }) {
    return {
      [`${grupo}Nome`]: local.nome ?? '',
      [`${grupo}Pais`]: local.pais ?? '',
      [`${grupo}Cep`]: local.cep ?? '',
      [`${grupo}Endereco`]: local.endereco ?? '',
      [`${grupo}Numero`]: local.numero ?? '',
      [`${grupo}Estado`]: local.estado ?? '',
      [`${grupo}Cidade`]: local.cidade ?? '',
      [`${grupo}Bairro`]: local.bairro ?? '',
    };
  }

  private numero(valor: string): number | null {
    if (valor === '') {
      return null;
    }
    const numero = Number(valor);
    return Number.isFinite(numero) ? numero : null;
  }

  private hora(valor: string): string | null {
    if (!valor) {
      return null;
    }
    return valor.length === 5 ? `${valor}:00` : valor;
  }
}
