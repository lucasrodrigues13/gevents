import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Modelo, NegocioClient } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { CampoDataComponent } from '../../shared/campo-data';
import { deDataParaIso, deIsoParaData } from '../../shared/data';
import { MascaraDirective } from '../../shared/mascara.directive';
import { aplicarMascara } from '../../shared/mascaras';

export interface EventoFormDados {
  id?: string;
}

@Component({
  selector: 'app-evento-form',
  imports: [
    ReactiveFormsModule, RouterLink, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatCheckboxModule, MatTabsModule, MatDialogModule, CampoDataComponent, MascaraDirective,
  ],
  template: `
    <div [class.gd-dialog-miolo]="!!dialogRef">
    @if (dialogRef) {
      <h1>Novo evento</h1>
    } @else {
      <a mat-button [routerLink]="editando() ? ['/eventos', id] : '/eventos'">Voltar</a>
      <header class="gd-cabecalho">
        <div>
          <h1>{{ editando() ? 'Editar evento' : 'Novo evento' }}</h1>
          <p>{{ editando() ? 'Informações, detalhes e locais do evento.' : 'Escolha o modelo e preencha os dados.' }}</p>
        </div>
      </header>
    }
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
        </mat-card-content>
      </mat-card>
    }
    @if (modeloId() || editando()) {
      <form class="formulario" [formGroup]="formulario" (ngSubmit)="salvar()">
        @if (editando()) {
          <mat-tab-group>
            <mat-tab label="Informações gerais">
              <label>Foto <input type="file" accept="image/jpeg,image/png,image/webp,.jpg,.jpeg,.png,.webp" (change)="escolherFoto($event)" /></label>
              @if (tipo() === 'Casamento') {
                <mat-form-field appearance="outline"><mat-label>Nome da noiva</mat-label><input matInput formControlName="primeiroNome" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Nome do noivo</mat-label><input matInput formControlName="segundoNome" /></mat-form-field>
              } @else {
                <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="primeiroNome" /></mat-form-field>
              }
              <app-campo-data rotulo="Data" formControlName="data" />
              <mat-form-field appearance="outline"><mat-label>Horário do convite</mat-label><input matInput type="time" formControlName="hora" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Budget</mat-label><input matInput type="number" min="0" formControlName="orcamentoEstimado" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Convidados estimados</mat-label><input matInput type="number" min="0" formControlName="convidadosEstimados" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>URL do site</mat-label><input matInput [value]="urlSite()" readonly /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Senha do site</mat-label><input matInput formControlName="senhaSite" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Tags</mat-label><input matInput formControlName="tags" placeholder="Assessoria completa" /></mat-form-field>
              <mat-checkbox formControlName="notificarAssessor">Notificar assessor(a)</mat-checkbox>
              <mat-checkbox formControlName="notificarAssistentes">Notificar assistentes</mat-checkbox>
              <mat-checkbox formControlName="notificarClientes">Notificar clientes</mat-checkbox>
              <mat-checkbox formControlName="salvarNaAgenda">Salvar na agenda</mat-checkbox>
              @if (formulario.controls.salvarNaAgenda.value) { <p>O vínculo com o Google fica vazio.</p> }
            </mat-tab>
            <mat-tab label="Detalhes do evento">
              <mat-form-field appearance="outline"><mat-label>Tempo de festa</mat-label><input matInput type="time" formControlName="tempoFesta" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Início da cerimônia</mat-label><input matInput type="time" formControlName="inicioCerimonia" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Início da recepção</mat-label><input matInput type="time" formControlName="inicioRecepcao" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Padrinhos</mat-label><input matInput type="number" min="0" formControlName="quantidadePadrinhos" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Damas e pajens</mat-label><input matInput type="number" min="0" formControlName="quantidadeDamas" /></mat-form-field>
            </mat-tab>
            <mat-tab label="Cerimônia">
              <mat-form-field appearance="outline"><mat-label>Local</mat-label><input matInput formControlName="cerimoniaNome" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>CEP</mat-label><input matInput formControlName="cerimoniaCep" gdMascara="cep" (blur)="completarCep('cerimonia')" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>País</mat-label><input matInput formControlName="cerimoniaPais" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Endereço</mat-label><input matInput formControlName="cerimoniaEndereco" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Número</mat-label><input matInput formControlName="cerimoniaNumero" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Estado</mat-label><input matInput formControlName="cerimoniaEstado" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Cidade</mat-label><input matInput formControlName="cerimoniaCidade" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Bairro</mat-label><input matInput formControlName="cerimoniaBairro" /></mat-form-field>
            </mat-tab>
            <mat-tab label="Recepção">
              <mat-form-field appearance="outline"><mat-label>Local</mat-label><input matInput formControlName="recepcaoNome" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>CEP</mat-label><input matInput formControlName="recepcaoCep" gdMascara="cep" (blur)="completarCep('recepcao')" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>País</mat-label><input matInput formControlName="recepcaoPais" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Endereço</mat-label><input matInput formControlName="recepcaoEndereco" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Número</mat-label><input matInput formControlName="recepcaoNumero" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Estado</mat-label><input matInput formControlName="recepcaoEstado" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Cidade</mat-label><input matInput formControlName="recepcaoCidade" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Bairro</mat-label><input matInput formControlName="recepcaoBairro" /></mat-form-field>
            </mat-tab>
          </mat-tab-group>
        } @else {
          <label>Foto <input type="file" accept="image/jpeg,image/png,image/webp,.jpg,.jpeg,.png,.webp" (change)="escolherFoto($event)" /></label>
          @if (tipo() === 'Casamento') {
            <mat-form-field appearance="outline"><mat-label>Nome da noiva</mat-label><input matInput formControlName="primeiroNome" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Nome do noivo</mat-label><input matInput formControlName="segundoNome" /></mat-form-field>
          } @else {
            <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="primeiroNome" /></mat-form-field>
          }
          <app-campo-data rotulo="Data" formControlName="data" />
          <mat-form-field appearance="outline"><mat-label>Horário do convite</mat-label><input matInput type="time" formControlName="hora" /></mat-form-field>
          <mat-form-field appearance="outline"><mat-label>Budget</mat-label><input matInput type="number" min="0" formControlName="orcamentoEstimado" /></mat-form-field>
          <mat-form-field appearance="outline"><mat-label>Convidados estimados</mat-label><input matInput type="number" min="0" formControlName="convidadosEstimados" /></mat-form-field>
          <h2>Local da cerimônia</h2>
          <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="cerimoniaNome" /></mat-form-field>
          <h2>Local da recepção</h2>
          <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="recepcaoNome" /></mat-form-field>
          <mat-form-field appearance="outline"><mat-label>Tags</mat-label><input matInput formControlName="tags" placeholder="Assessoria completa" /></mat-form-field>
          <mat-checkbox formControlName="notificarAssessor">Notificar assessor(a)</mat-checkbox>
          <mat-checkbox formControlName="notificarAssistentes">Notificar assistentes</mat-checkbox>
          <mat-checkbox formControlName="notificarClientes">Notificar clientes</mat-checkbox>
          <mat-checkbox formControlName="salvarNaAgenda">Salvar na agenda</mat-checkbox>
          @if (formulario.controls.salvarNaAgenda.value) { <p>O vínculo com o Google fica vazio.</p> }
        }
        <button mat-flat-button type="submit">Salvar</button>
        @if (dialogRef) {
          <button mat-button type="button" (click)="dialogRef.close()">Cancelar</button>
        } @else if (editando()) {
          <a mat-button [routerLink]="['/eventos', id]">Cancelar</a>
        } @else {
          <a mat-button routerLink="/eventos">Cancelar</a>
        }
      </form>
    }
    </div>
  `,
})
export class EventoFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly rota = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly dadosDialogo = inject<EventoFormDados | null>(MAT_DIALOG_DATA, { optional: true });
  protected readonly dialogRef = inject(MatDialogRef<EventoFormPage, string>, { optional: true });
  private readonly toast = inject(ToastService);
  protected readonly modelos = signal<Modelo[]>([]);
  protected readonly modeloId = signal('');
  protected readonly tipo = signal('');
  protected readonly editando = signal(false);
  protected readonly urlSite = signal('');
  private foto: File | null = null;
  protected id = '';

  protected readonly formulario = this.fb.group({
    primeiroNome: '',
    segundoNome: '',
    data: this.fb.control<Date | null>(null),
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

  ngOnInit(): void {
    this.id = this.dadosDialogo ? (this.dadosDialogo.id ?? '') : (this.rota.snapshot.paramMap.get('id') ?? '');
    this.editando.set(!!this.id);
    this.negocio.modelos().subscribe({
      next: (modelos) => this.modelos.set(modelos),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
    if (this.id) {
      this.negocio.evento(this.id).subscribe({
        next: (evento) => {
          this.tipo.set(evento.tipo);
          this.urlSite.set(evento.urlSite ?? '');
          this.formulario.patchValue({
            primeiroNome: evento.primeiroNome ?? '',
            segundoNome: evento.segundoNome ?? '',
            data: deIsoParaData(evento.data),
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
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    }
  }

  protected escolherModelo(id: string): void {
    this.modeloId.set(id);
    const modelo = this.modelos().find((item) => item.id === id);
    this.tipo.set(modelo?.tipo ?? '');
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
          [`${grupo}Pais`]: endereco.pais || 'Brasil',
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
          this.toast.sucesso(this.editando() ? 'Evento atualizado.' : 'Evento criado.');
          this.dialogRef?.close();
          void this.router.navigate(['/eventos', evento.id]);
        };
        if (!this.foto) {
          seguir();
          return;
        }
        this.negocio.imagemEvento(evento.id, this.foto).subscribe({
          next: seguir,
          error: (erro: unknown) => {
            this.toast.erro(mensagemDeErro(erro));
            seguir();
          },
        });
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  private pedido() {
    const valor = this.formulario.getRawValue();
    return {
      primeiroNome: valor.primeiroNome ?? '',
      segundoNome: valor.segundoNome ?? '',
      data: deDataParaIso(valor.data),
      hora: this.hora(valor.hora ?? ''),
      orcamentoEstimado: this.numero(valor.orcamentoEstimado ?? ''),
      convidadosEstimados: this.numero(valor.convidadosEstimados ?? ''),
      tags: (valor.tags ?? '').split(',').map((item) => item.trim()).filter(Boolean),
      senhaSite: valor.senhaSite ?? '',
      notificarAssessor: !!valor.notificarAssessor,
      notificarAssistentes: !!valor.notificarAssistentes,
      notificarClientes: !!valor.notificarClientes,
      salvarNaAgenda: !!valor.salvarNaAgenda,
      tempoFesta: this.hora(valor.tempoFesta ?? ''),
      inicioCerimonia: this.hora(valor.inicioCerimonia ?? ''),
      inicioRecepcao: this.hora(valor.inicioRecepcao ?? ''),
      quantidadePadrinhos: this.numero(valor.quantidadePadrinhos ?? ''),
      quantidadeDamas: this.numero(valor.quantidadeDamas ?? ''),
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
      [`${grupo}Cep`]: aplicarMascara(local.cep ?? '', 'cep'),
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
