import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { CampoDataComponent } from '../../shared/campo-data';
import { deDataParaIso, deIsoParaData } from '../../shared/data';
import { MascaraDirective } from '../../shared/mascara.directive';
import { aplicarMascara } from '../../shared/mascaras';

@Component({
  selector: 'app-perfil',
  imports: [
    ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule,
    MascaraDirective, CampoDataComponent,
  ],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Perfil</h1>
        <p>Dados da assessoria.</p>
      </div>
    </header>
    <mat-card>
      <mat-card-header><mat-card-title>Perfil da assessoria</mat-card-title></mat-card-header>
      <mat-card-content>
        <form class="formulario" [formGroup]="formulario" (ngSubmit)="salvar()">
          <mat-form-field appearance="outline">
            <mat-label>E-mail</mat-label>
            <input matInput [value]="email" readonly />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Nome da empresa</mat-label>
            <input matInput formControlName="nomeEmpresa" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Responsável</mat-label>
            <input matInput formControlName="responsavel" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>CPF ou CNPJ</mat-label>
            <input matInput formControlName="documento" gdMascara="documento" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Celular</mat-label>
            <input matInput formControlName="celular" gdMascara="celular" />
          </mat-form-field>
          <app-campo-data rotulo="Nascimento" formControlName="nascimento" />
          <mat-form-field appearance="outline">
            <mat-label>Endereço</mat-label>
            <textarea matInput rows="3" formControlName="endereco"></textarea>
          </mat-form-field>
          <button mat-flat-button type="submit">Salvar</button>
        </form>
      </mat-card-content>
    </mat-card>
  `,
})
export class PerfilPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected email = '';
  protected readonly formulario = this.fb.group({
    nomeEmpresa: '',
    responsavel: '',
    documento: '',
    celular: '',
    nascimento: this.fb.control<Date | null>(null),
    endereco: '',
  });

  ngOnInit(): void {
    this.negocio.perfil().subscribe({
      next: (perfil) => {
        this.email = perfil.email;
        this.formulario.patchValue({
          nomeEmpresa: perfil.nomeEmpresa ?? '',
          responsavel: perfil.responsavel ?? '',
          documento: aplicarMascara(perfil.documento ?? '', 'documento'),
          celular: aplicarMascara(perfil.celular ?? '', 'celular'),
          nascimento: deIsoParaData(perfil.nascimento),
          endereco: perfil.endereco ?? '',
        });
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected salvar(): void {
    const valor = this.formulario.getRawValue();
    this.negocio.salvarPerfil({
      nomeEmpresa: valor.nomeEmpresa ?? '',
      responsavel: valor.responsavel ?? '',
      documento: valor.documento ?? '',
      celular: valor.celular ?? '',
      nascimento: deDataParaIso(valor.nascimento),
      endereco: valor.endereco ?? '',
    }).subscribe({
      next: (perfil) => {
        this.email = perfil.email;
        this.toast.sucesso('Perfil salvo.');
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
