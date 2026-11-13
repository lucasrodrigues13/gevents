import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { NegocioClient } from '../../core/negocio.client';

@Component({
  selector: 'app-perfil',
  imports: [ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
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
            <input matInput [value]="email()" readonly />
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
            <input matInput formControlName="documento" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Celular</mat-label>
            <input matInput formControlName="celular" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Nascimento</mat-label>
            <input matInput type="date" formControlName="nascimento" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Endereço</mat-label>
            <textarea matInput rows="3" formControlName="endereco"></textarea>
          </mat-form-field>
          @if (aviso()) { <p class="aviso" role="alert">{{ aviso() }}</p> }
          <button mat-flat-button type="submit">Salvar</button>
        </form>
      </mat-card-content>
    </mat-card>
  `,
})
export class PerfilPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  protected readonly email = signal('');
  protected readonly aviso = signal('');
  protected readonly formulario = this.fb.nonNullable.group({
    nomeEmpresa: '',
    responsavel: '',
    documento: '',
    celular: '',
    nascimento: '',
    endereco: '',
  });

  ngOnInit(): void {
    this.negocio.perfil().subscribe({
      next: (perfil) => {
        this.email.set(perfil.email);
        this.formulario.patchValue({
          nomeEmpresa: perfil.nomeEmpresa ?? '',
          responsavel: perfil.responsavel ?? '',
          documento: perfil.documento ?? '',
          celular: perfil.celular ?? '',
          nascimento: perfil.nascimento ?? '',
          endereco: perfil.endereco ?? '',
        });
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }

  protected salvar(): void {
    const valor = this.formulario.getRawValue();
    this.negocio.salvarPerfil({
      nomeEmpresa: valor.nomeEmpresa,
      responsavel: valor.responsavel,
      documento: valor.documento,
      celular: valor.celular,
      nascimento: valor.nascimento || null,
      endereco: valor.endereco,
    }).subscribe({
      next: (perfil) => {
        this.email.set(perfil.email);
        this.aviso.set('Perfil salvo.');
      },
      error: (erro: unknown) => this.aviso.set(mensagemDeErro(erro)),
    });
  }
}
