import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTabsModule } from '@angular/material/tabs';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Cliente, NegocioClient, PAPEIS, ROTULO_PAPEL } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { CampoDataComponent } from '../../shared/campo-data';
import { deDataParaIso, deIsoParaData } from '../../shared/data';
import { MascaraDirective } from '../../shared/mascara.directive';
import { aplicarMascara } from '../../shared/mascaras';

@Component({
  selector: 'app-cliente-form',
  imports: [
    ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatTabsModule, MatSlideToggleModule, MatCheckboxModule, MascaraDirective, CampoDataComponent,
  ],
  template: `
    <h2 mat-dialog-title>{{ dados ? 'Editar cliente' : 'Novo cliente' }}</h2>
    <mat-dialog-content>
      <form id="cliente-form" class="gd-dialog-miolo" [formGroup]="formulario" (ngSubmit)="salvar()">
        <mat-tab-group>
          <mat-tab label="Dados pessoais">
            <mat-form-field appearance="outline"><mat-label>Nome completo</mat-label><input matInput formControlName="nome" /></mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Papel</mat-label>
              <mat-select formControlName="papel">
                @for (papel of papeis; track papel) { <mat-option [value]="papel">{{ rotulo(papel) }}</mat-option> }
              </mat-select>
            </mat-form-field>
            <app-campo-data rotulo="Nascimento" formControlName="nascimento" />
            <mat-form-field appearance="outline"><mat-label>E-mail</mat-label><input matInput type="email" formControlName="email" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>DDI</mat-label><input matInput formControlName="ddi" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Celular</mat-label><input matInput formControlName="celular" gdMascara="celular" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Telefone fixo</mat-label><input matInput formControlName="telefoneFixo" gdMascara="telefone" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>CPF</mat-label><input matInput formControlName="cpf" gdMascara="cpf" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>RG</mat-label><input matInput formControlName="rg" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>CNPJ</mat-label><input matInput formControlName="cnpj" gdMascara="cnpj" /></mat-form-field>
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
            <mat-form-field appearance="outline"><mat-label>CEP</mat-label><input matInput formControlName="cep" gdMascara="cep" (blur)="completarCep()" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>País</mat-label><input matInput formControlName="pais" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Endereço</mat-label><input matInput formControlName="endereco" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Número</mat-label><input matInput formControlName="numero" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Estado</mat-label><input matInput formControlName="estado" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Cidade</mat-label><input matInput formControlName="cidade" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Bairro</mat-label><input matInput formControlName="bairro" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Complemento</mat-label><input matInput formControlName="complemento" /></mat-form-field>
          </mat-tab>
          <mat-tab label="Acesso">
            <mat-slide-toggle formControlName="acessoPlataforma">Acesso à plataforma</mat-slide-toggle>
            <p class="gd-apoio">O cliente recebe link para definir a senha e entra com a role Client vinculada à sua conta.</p>
            <div formGroupName="permissoes" class="gd-flags">
              <mat-checkbox formControlName="convidados">Convidados</mat-checkbox>
              <mat-checkbox formControlName="tarefas">Tarefas</mat-checkbox>
              <mat-checkbox formControlName="anexos">Anexos</mat-checkbox>
              <mat-checkbox formControlName="parcelas">Parcelas</mat-checkbox>
              <mat-checkbox formControlName="rsvp">RSVP</mat-checkbox>
              <mat-checkbox formControlName="mapa">Mapa</mat-checkbox>
              <mat-checkbox formControlName="contrato">Contrato</mat-checkbox>
            </div>
          </mat-tab>
        </mat-tab-group>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="submit" form="cliente-form">Salvar</button>
    </mat-dialog-actions>
  `,
  styles: `
    .gd-flags { display: grid; gap: 0.35rem; margin-top: 1rem; }
    .gd-apoio { margin: 0.75rem 0 0; color: var(--gd-texto-suave, #667); font-size: 0.875rem; }
    mat-slide-toggle { display: block; margin-top: 0.5rem; }
  `,
})
export class ClienteFormDialog {
  protected readonly dados = inject<Cliente | null>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<ClienteFormDialog, Cliente>);
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly papeis = PAPEIS;
  protected readonly formulario = this.fb.nonNullable.group({
    nome: [this.dados?.nome ?? '', Validators.required],
    papel: [this.dados?.papel ?? 'Noiva', Validators.required],
    nascimento: this.fb.control<Date | null>(deIsoParaData(this.dados?.nascimento)),
    email: [this.dados?.email ?? '', Validators.required],
    ddi: this.dados?.ddi ?? '+55',
    celular: [aplicarMascara(this.dados?.celular ?? '', 'celular'), Validators.required],
    telefoneFixo: aplicarMascara(this.dados?.telefoneFixo ?? '', 'telefone'),
    cpf: aplicarMascara(this.dados?.cpf ?? '', 'cpf'),
    rg: this.dados?.rg ?? '',
    cnpj: aplicarMascara(this.dados?.cnpj ?? '', 'cnpj'),
    profissao: this.dados?.profissao ?? '',
    inscricaoMunicipal: this.dados?.inscricaoMunicipal ?? '',
    religiao: this.dados?.religiao ?? '',
    observacao: this.dados?.observacao ?? '',
    facebook: this.dados?.facebook ?? '',
    instagram: this.dados?.instagram ?? '',
    pais: this.dados?.pais ?? '',
    cep: aplicarMascara(this.dados?.cep ?? '', 'cep'),
    endereco: this.dados?.endereco ?? '',
    numero: this.dados?.numero ?? '',
    estado: this.dados?.estado ?? '',
    cidade: this.dados?.cidade ?? '',
    bairro: this.dados?.bairro ?? '',
    complemento: this.dados?.complemento ?? '',
    acessoPlataforma: this.dados?.acessoPlataforma ?? false,
    permissoes: this.fb.nonNullable.group({
      convidados: this.dados?.permissoes?.convidados ?? false,
      tarefas: this.dados?.permissoes?.tarefas ?? false,
      anexos: this.dados?.permissoes?.anexos ?? false,
      parcelas: this.dados?.permissoes?.parcelas ?? false,
      rsvp: this.dados?.permissoes?.rsvp ?? false,
      mapa: this.dados?.permissoes?.mapa ?? false,
      contrato: this.dados?.permissoes?.contrato ?? false,
    }),
  });

  protected rotulo(papel: string): string {
    return ROTULO_PAPEL[papel] ?? papel;
  }

  protected completarCep(): void {
    const digitos = this.formulario.controls.cep.value.replace(/\D/g, '');
    if (digitos.length !== 8) {
      return;
    }
    this.negocio.cep(digitos).subscribe({
      next: (endereco) => this.formulario.patchValue({
        pais: endereco.pais || 'Brasil',
        endereco: endereco.endereco,
        estado: endereco.estado,
        cidade: endereco.cidade,
        bairro: endereco.bairro,
      }),
      error: () => undefined,
    });
  }

  protected salvar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      this.toast.erro('Confira os campos obrigatórios.');
      return;
    }
    const valor = this.formulario.getRawValue();
    const pedido = {
      ...valor,
      nascimento: deDataParaIso(valor.nascimento),
    };
    const destino = this.dados
      ? this.negocio.atualizarCliente(this.dados.id, pedido)
      : this.negocio.criarCliente(pedido);
    destino.subscribe({
      next: (cliente) => {
        this.toast.sucesso(this.dados ? 'Cliente atualizado.' : 'Cliente criado.');
        this.ref.close(cliente);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
