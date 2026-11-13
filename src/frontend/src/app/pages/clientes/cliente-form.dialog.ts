import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Cliente, NegocioClient, PAPEIS, ROTULO_PAPEL } from '../../core/negocio.client';

@Component({
  selector: 'app-cliente-form',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatTabsModule],
  template: `
    <h2 mat-dialog-title>{{ dados ? 'Editar cliente' : 'Novo cliente' }}</h2>
    <mat-dialog-content>
      <form id="cliente-form" class="gd-dialog-miolo" [formGroup]="formulario" (ngSubmit)="salvar()">
        @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
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
            <mat-form-field appearance="outline"><mat-label>CPF</mat-label><input matInput formControlName="cpf" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>RG</mat-label><input matInput formControlName="rg" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>CNPJ</mat-label><input matInput formControlName="cnpj" /></mat-form-field>
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
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="submit" form="cliente-form">Salvar</button>
    </mat-dialog-actions>
  `,
})
export class ClienteFormDialog {
  protected readonly dados = inject<Cliente | null>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<ClienteFormDialog, Cliente>);
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  protected readonly papeis = PAPEIS;
  protected readonly aviso = signal('');
  protected readonly formulario = this.fb.nonNullable.group({
    nome: [this.dados?.nome ?? '', Validators.required],
    papel: [this.dados?.papel ?? 'Noiva', Validators.required],
    nascimento: this.dados?.nascimento ?? '',
    email: [this.dados?.email ?? '', Validators.required],
    ddi: this.dados?.ddi ?? '+55',
    celular: [this.dados?.celular ?? '', Validators.required],
    telefoneFixo: this.dados?.telefoneFixo ?? '',
    cpf: this.dados?.cpf ?? '',
    rg: this.dados?.rg ?? '',
    cnpj: this.dados?.cnpj ?? '',
    profissao: this.dados?.profissao ?? '',
    inscricaoMunicipal: this.dados?.inscricaoMunicipal ?? '',
    religiao: this.dados?.religiao ?? '',
    observacao: this.dados?.observacao ?? '',
    facebook: this.dados?.facebook ?? '',
    instagram: this.dados?.instagram ?? '',
    pais: this.dados?.pais ?? '',
    cep: this.dados?.cep ?? '',
    endereco: this.dados?.endereco ?? '',
    numero: this.dados?.numero ?? '',
    estado: this.dados?.estado ?? '',
    cidade: this.dados?.cidade ?? '',
    bairro: this.dados?.bairro ?? '',
    complemento: this.dados?.complemento ?? '',
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
      return;
    }
    const valor = this.formulario.getRawValue();
    const pedido = { ...valor, nascimento: valor.nascimento || null };
    const destino = this.dados
      ? this.negocio.atualizarCliente(this.dados.id, pedido)
      : this.negocio.criarCliente(pedido);
    destino.subscribe({
      next: (cliente) => this.ref.close(cliente),
      error: (erro: unknown) => {
        this.aviso.set(mensagemDeErro(erro));
      },
    });
  }
}
