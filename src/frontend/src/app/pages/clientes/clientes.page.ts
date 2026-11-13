import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Cliente, NegocioClient, PAPEIS, ROTULO_PAPEL } from '../../core/negocio.client';

@Component({
  selector: 'app-clientes',
  imports: [
    ReactiveFormsModule, RouterLink, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatTabsModule, MatDialogModule,
  ],
  template: `
    <header class="topo">
      <h1>Clientes</h1>
      <button mat-flat-button type="button" (click)="aberto.set(true)">Adicionar cliente</button>
    </header>
    <mat-form-field appearance="outline">
      <mat-label>Buscar por nome</mat-label>
      <input matInput [value]="busca()" (input)="buscar($event)" />
    </mat-form-field>
    @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
    <div class="cards">
      @for (cliente of clientes(); track cliente.id) {
        <mat-card>
          <mat-card-title>{{ cliente.nome }}</mat-card-title>
          <mat-card-content>
            <p>{{ cliente.ddi }} {{ cliente.celular }}</p>
            <p>{{ cliente.email }}</p>
            @if (cliente.eventos.length === 0) {
              <p>Nenhum evento vinculado</p>
            } @else {
              @for (evento of cliente.eventos; track evento.id) {
                <p><a [routerLink]="['/eventos', evento.id]">{{ evento.nome }}</a></p>
              }
            }
            <p>Último acesso: {{ cliente.ultimoAcesso ?? '—' }}</p>
          </mat-card-content>
        </mat-card>
      }
    </div>
    @if (aberto()) {
      <mat-card>
        <mat-card-header><mat-card-title>Adicionar cliente</mat-card-title></mat-card-header>
        <mat-card-content>
          <form [formGroup]="formulario" (ngSubmit)="salvar()">
            <mat-tab-group>
              <mat-tab label="Dados pessoais">
                <mat-form-field appearance="outline"><mat-label>Nome completo</mat-label><input matInput formControlName="nome" /></mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Papel</mat-label>
                  <mat-select formControlName="papel">
                    @for (papel of papeis; track papel) { <mat-option [value]="papel">{{ rotulo(papel) }}</mat-option> }
                  </mat-select>
                </mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Nascimento</mat-label><input matInput type="date" formControlName="nascimento" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>E-mail</mat-label><input matInput type="email" formControlName="email" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>DDI</mat-label><input matInput formControlName="ddi" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Celular</mat-label><input matInput formControlName="celular" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Telefone fixo</mat-label><input matInput formControlName="telefoneFixo" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>CPF</mat-label><input matInput formControlName="cpf" placeholder="000.000.000-00" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>RG</mat-label><input matInput formControlName="rg" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>CNPJ</mat-label><input matInput formControlName="cnpj" placeholder="00.000.000/0000-00" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Profissão</mat-label><input matInput formControlName="profissao" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Inscrição municipal</mat-label><input matInput formControlName="inscricaoMunicipal" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Religião</mat-label><input matInput formControlName="religiao" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Observação</mat-label><textarea matInput rows="2" formControlName="observacao"></textarea></mat-form-field>
              </mat-tab>
              <mat-tab label="Redes">
                <mat-form-field appearance="outline"><mat-label>Facebook</mat-label><input matInput formControlName="facebook" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Instagram</mat-label><input matInput formControlName="instagram" /></mat-form-field>
              </mat-tab>
              <mat-tab label="Endereço">
                <mat-form-field appearance="outline"><mat-label>País</mat-label><input matInput formControlName="pais" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>CEP</mat-label><input matInput formControlName="cep" (blur)="completarCep()" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Endereço</mat-label><input matInput formControlName="endereco" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Número</mat-label><input matInput formControlName="numero" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Estado</mat-label><input matInput formControlName="estado" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Cidade</mat-label><input matInput formControlName="cidade" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Bairro</mat-label><input matInput formControlName="bairro" /></mat-form-field>
                <mat-form-field appearance="outline"><mat-label>Complemento</mat-label><input matInput formControlName="complemento" /></mat-form-field>
              </mat-tab>
            </mat-tab-group>
            <button mat-flat-button type="submit">Salvar</button>
            <button mat-button type="button" (click)="aberto.set(false)">Fechar</button>
          </form>
        </mat-card-content>
      </mat-card>
    }
  `,
  styles: `.topo, .cards { display: flex; gap: 12px; flex-wrap: wrap; } mat-card { width: min(320px, 100%); } mat-form-field { width: 100%; }`,
})
export class ClientesPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  protected readonly papeis = PAPEIS;
  protected readonly clientes = signal<Cliente[]>([]);
  protected readonly aberto = signal(false);
  protected readonly aviso = signal('');
  protected readonly busca = signal('');
  protected readonly formulario = this.fb.nonNullable.group({
    nome: ['', Validators.required],
    papel: ['Noiva', Validators.required],
    nascimento: '',
    email: ['', Validators.required],
    ddi: '+55',
    celular: ['', Validators.required],
    telefoneFixo: '',
    cpf: '',
    rg: '',
    cnpj: '',
    profissao: '',
    inscricaoMunicipal: '',
    religiao: '',
    observacao: '',
    facebook: '',
    instagram: '',
    pais: '',
    cep: '',
    endereco: '',
    numero: '',
    estado: '',
    cidade: '',
    bairro: '',
    complemento: '',
  });

  ngOnInit(): void {
    this.carregar();
  }

  protected rotulo(papel: string): string {
    return ROTULO_PAPEL[papel] ?? papel;
  }

  protected buscar(evento: Event): void {
    this.busca.set((evento.target as HTMLInputElement).value);
    this.carregar();
  }

  protected completarCep(): void {
    const digitos = this.formulario.controls.cep.value.replace(/\D/g, '');
    if (digitos.length !== 8) {
      return;
    }
    this.negocio.cep(digitos).subscribe({
      next: (endereco) => this.formulario.patchValue({
        endereco: endereco.endereco,
        estado: endereco.estado,
        cidade: endereco.cidade,
        bairro: endereco.bairro,
      }),
      error: () => undefined,
    });
  }

  protected salvar(): void {
    const valor = this.formulario.getRawValue();
    this.negocio.criarCliente({ ...valor, nascimento: valor.nascimento || null }).subscribe({
      next: () => {
        this.aberto.set(false);
        this.formulario.reset({ papel: 'Noiva', ddi: '+55' });
        this.carregar();
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  private carregar(): void {
    this.negocio.clientes(this.busca()).subscribe({
      next: (clientes) => {
        this.clientes.set(clientes);
        this.aviso.set('');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }
}
