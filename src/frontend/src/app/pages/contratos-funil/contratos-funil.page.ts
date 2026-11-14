import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { ContratoFunil, ModeloContratoFunil, NegocioClient, Oportunidade } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { DIALOG } from '../../shared/dialog';
import { GdDataPipe } from '../../shared/gd-data.pipe';

@Component({
  selector: 'app-contrato-funil-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  template: `
    <h2 mat-dialog-title>{{ dados ? 'Editar contrato' : 'Novo contrato' }}</h2>
    <mat-dialog-content>
      <form class="formulario" [formGroup]="form">
        @if (!dados) {
          <mat-form-field appearance="outline">
            <mat-label>Oportunidade</mat-label>
            <mat-select formControlName="oportunidadeId">
              @for (o of oportunidades(); track o.id) { <mat-option [value]="o.id">{{ o.nome }}</mat-option> }
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Modelo</mat-label>
            <mat-select formControlName="modeloId" (selectionChange)="aplicarModelo($event.value)">
              <mat-option [value]="null">Sem modelo</mat-option>
              @for (m of modelos(); track m.id) { <mat-option [value]="m.id">{{ m.nome }}</mat-option> }
            </mat-select>
          </mat-form-field>
        }
        <mat-form-field appearance="outline"><mat-label>Conteúdo</mat-label><textarea matInput rows="8" formControlName="conteudo"></textarea></mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="salvar()">Salvar</button>
    </mat-dialog-actions>
  `,
})
export class ContratoFunilDialog implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly dados = inject<ContratoFunil | null>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<ContratoFunilDialog, boolean>);
  protected readonly oportunidades = signal<Oportunidade[]>([]);
  protected readonly modelos = signal<ModeloContratoFunil[]>([]);
  protected readonly form = this.fb.nonNullable.group({
    oportunidadeId: [this.dados?.oportunidadeId ?? '', Validators.required],
    modeloId: [this.dados?.modeloId ?? null as string | null],
    conteudo: [this.dados?.conteudo ?? '', Validators.required],
  });

  ngOnInit(): void {
    this.negocio.oportunidades().subscribe({ next: (lista) => this.oportunidades.set(lista) });
    this.negocio.modelosContratoFunil().subscribe({ next: (lista) => this.modelos.set(lista) });
  }

  protected aplicarModelo(id: string | null): void {
    const modelo = this.modelos().find((m) => m.id === id);
    if (!modelo) return;
    const opp = this.oportunidades().find((o) => o.id === this.form.controls.oportunidadeId.value);
    this.form.controls.conteudo.setValue(modelo.texto.replace(/#nome_oportunidade/gi, opp?.nome ?? ''));
  }

  protected salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const valor = this.form.getRawValue();
    const req = this.dados
      ? this.negocio.atualizarContratoFunil(this.dados.id, valor)
      : this.negocio.criarContratoFunil(valor);
    req.subscribe({
      next: () => {
        this.toast.sucesso('Contrato salvo.');
        this.ref.close(true);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}

@Component({
  selector: 'app-contratos-funil',
  imports: [MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatTableModule, GdDataPipe],
  template: `
    <header class="gd-cabecalho">
      <div><h1>Contratos</h1><p>Contratos comerciais do funil.</p></div>
      <button mat-flat-button type="button" (click)="abrir()"><mat-icon>add</mat-icon> Novo contrato</button>
    </header>
    <section class="gd-painel">
      <div class="gd-barra">
        <div class="gd-abas">
          @for (aba of abas; track aba.valor) {
            <button type="button" [class.ativa]="situacao() === aba.valor" (click)="situacao.set(aba.valor); carregar()">{{ aba.rotulo }}</button>
          }
        </div>
        <mat-form-field appearance="outline" subscriptSizing="dynamic">
          <mat-label>Buscar oportunidade</mat-label>
          <input matInput [value]="busca()" (input)="buscar($event)" />
        </mat-form-field>
      </div>
      @if (lista().length === 0) {
        <p class="gd-vazio">Não há contrato nesta aba.</p>
      } @else {
        <table class="gd-grade" mat-table [dataSource]="lista()">
          <ng-container matColumnDef="oportunidade">
            <th mat-header-cell *matHeaderCellDef>Oportunidade</th>
            <td mat-cell *matCellDef="let item">{{ item.oportunidadeNome }}</td>
          </ng-container>
          <ng-container matColumnDef="criadoEm">
            <th mat-header-cell *matHeaderCellDef>Criado em</th>
            <td mat-cell *matCellDef="let item">{{ item.criadoEm | gdData }}</td>
          </ng-container>
          <ng-container matColumnDef="acoes">
            <th mat-header-cell *matHeaderCellDef class="gd-acoes">Ações</th>
            <td mat-cell *matCellDef="let item" class="gd-acoes">
              @if (item.situacao === 'EmAndamento') {
                <button mat-icon-button type="button" (click)="abrir(item)"><mat-icon>edit</mat-icon></button>
                <button mat-icon-button type="button" (click)="enviar(item)"><mat-icon>send</mat-icon></button>
                <button mat-icon-button type="button" (click)="assinar(item)"><mat-icon>draw</mat-icon></button>
                <button mat-icon-button type="button" (click)="cancelar(item)"><mat-icon>cancel</mat-icon></button>
                <button mat-icon-button type="button" (click)="excluir(item)"><mat-icon>delete</mat-icon></button>
              } @else if (item.situacao === 'Assinado') {
                <button mat-icon-button type="button" (click)="cancelar(item)"><mat-icon>cancel</mat-icon></button>
              }
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="colunas"></tr>
          <tr mat-row *matRowDef="let row; columns: colunas"></tr>
        </table>
      }
    </section>
  `,
})
export class ContratosFunilPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly abas = [
    { valor: 'EmAndamento', rotulo: 'Em andamento' },
    { valor: 'Assinado', rotulo: 'Assinados' },
    { valor: 'Cancelado', rotulo: 'Cancelados' },
  ];
  protected readonly situacao = signal('EmAndamento');
  protected readonly busca = signal('');
  protected readonly lista = signal<ContratoFunil[]>([]);
  protected readonly colunas = ['oportunidade', 'criadoEm', 'acoes'];

  ngOnInit(): void {
    this.carregar();
  }

  protected buscar(evento: Event): void {
    this.busca.set((evento.target as HTMLInputElement).value);
    this.carregar();
  }

  protected abrir(item?: ContratoFunil): void {
    this.dialog.open(ContratoFunilDialog, { ...DIALOG, data: item ?? null }).afterClosed().subscribe((ok) => ok && this.carregar());
  }

  protected enviar(item: ContratoFunil): void {
    this.negocio.enviarContratoFunil(item.id).subscribe({
      next: () => { this.toast.sucesso('Contrato enviado.'); this.carregar(); },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected assinar(item: ContratoFunil): void {
    this.negocio.assinarContratoFunil(item.id).subscribe({
      next: () => { this.toast.sucesso('Contrato assinado.'); this.carregar(); },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected cancelar(item: ContratoFunil): void {
    this.negocio.cancelarContratoFunil(item.id).subscribe({
      next: () => { this.toast.sucesso('Contrato cancelado.'); this.carregar(); },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }

  protected excluir(item: ContratoFunil): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir contrato', texto: 'Confirma a exclusão?', confirmar: 'Excluir' },
    }).afterClosed().subscribe((ok) => {
      if (!ok) return;
      this.negocio.excluirContratoFunil(item.id).subscribe({
        next: () => { this.toast.sucesso('Contrato excluído.'); this.carregar(); },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  protected carregar(): void {
    this.negocio.contratosFunil({ situacao: this.situacao(), busca: this.busca() || undefined }).subscribe({
      next: (lista) => this.lista.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
