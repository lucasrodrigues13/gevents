import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { CompromissoAgenda, NegocioClient } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { CampoDataComponent } from '../../shared/campo-data';
import { ConfirmaDialog } from '../../shared/confirma-dialog';
import { deDataParaIso, deIsoParaData } from '../../shared/data';
import { DIALOG } from '../../shared/dialog';
import { GdDataPipe } from '../../shared/gd-data.pipe';

@Component({
  selector: 'app-compromisso-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, CampoDataComponent],
  template: `
    <h2 mat-dialog-title>{{ dados ? 'Editar compromisso' : 'Novo compromisso' }}</h2>
    <mat-dialog-content>
      <form class="formulario" [formGroup]="form">
        <mat-form-field appearance="outline"><mat-label>Descrição</mat-label><input matInput formControlName="descricao" /></mat-form-field>
        <app-campo-data formControlName="data" rotulo="Data" />
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="salvar()">Salvar</button>
    </mat-dialog-actions>
  `,
})
export class CompromissoDialog {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly dados = inject<CompromissoAgenda | null>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<CompromissoDialog, boolean>);
  protected readonly form = this.fb.nonNullable.group({
    descricao: [this.dados?.descricao ?? '', Validators.required],
    data: this.fb.control<Date | null>(deIsoParaData(this.dados?.data), Validators.required),
  });

  protected salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toast.erro('Informe descrição e data.');
      return;
    }
    const valor = this.form.getRawValue();
    const pedido = { descricao: valor.descricao, data: deDataParaIso(valor.data)! };
    const req = this.dados
      ? this.negocio.atualizarCompromisso(this.dados.id, pedido)
      : this.negocio.criarCompromisso(pedido);
    req.subscribe({
      next: () => {
        this.toast.sucesso('Compromisso salvo.');
        this.ref.close(true);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}

@Component({
  selector: 'app-agenda',
  imports: [MatButtonModule, MatButtonToggleModule, MatIconModule, GdDataPipe],
  template: `
    <header class="gd-cabecalho">
      <div>
        <h1>Minha agenda</h1>
        <p>Compromissos da conta no período selecionado.</p>
      </div>
      <button mat-flat-button type="button" (click)="abrir()"><mat-icon>add</mat-icon> Novo compromisso</button>
    </header>
    <section class="gd-painel">
      <div class="gd-barra">
        <div class="nav">
          <button mat-stroked-button type="button" (click)="navegar(-1)">Anterior</button>
          <button mat-stroked-button type="button" (click)="hoje()">Hoje</button>
          <button mat-stroked-button type="button" (click)="navegar(1)">Próxima</button>
          <strong>{{ rotuloPeriodo() }}</strong>
        </div>
        <mat-button-toggle-group [value]="vista()" (change)="mudarVista($event.value)">
          <mat-button-toggle value="mes">Mês</mat-button-toggle>
          <mat-button-toggle value="semana">Semana</mat-button-toggle>
          <mat-button-toggle value="dia">Dia</mat-button-toggle>
          <mat-button-toggle value="lista">Lista</mat-button-toggle>
        </mat-button-toggle-group>
      </div>
      @if (visiveis().length === 0) {
        <p class="gd-vazio">Nenhum compromisso neste período.</p>
      } @else {
        <ul class="lista">
          @for (item of visiveis(); track item.id) {
            <li>
              <div>
                <strong>{{ item.descricao }}</strong>
                <span class="gd-sub">{{ item.data | gdData }} @if (item.deEvento) { · evento }</span>
              </div>
              @if (!item.deEvento) {
                <span class="gd-acoes">
                  <button mat-icon-button type="button" (click)="abrir(item)"><mat-icon>edit</mat-icon></button>
                  <button mat-icon-button type="button" (click)="excluir(item)"><mat-icon>delete</mat-icon></button>
                </span>
              }
            </li>
          }
        </ul>
      }
    </section>
  `,
  styles: `
    .nav { display: flex; gap: 8px; align-items: center; flex-wrap: wrap; }
    .lista { list-style: none; margin: 0; padding: 0; }
    .lista li { display: flex; justify-content: space-between; gap: 12px; padding: 12px 0; border-bottom: 1px solid var(--gd-border); }
  `,
})
export class AgendaPage implements OnInit {
  private readonly negocio = inject(NegocioClient);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  protected readonly vista = signal<'mes' | 'semana' | 'dia' | 'lista'>('mes');
  protected readonly ancora = signal(new Date());
  protected readonly itens = signal<CompromissoAgenda[]>([]);
  protected readonly periodo = computed(() => calcularPeriodo(this.vista(), this.ancora()));
  protected readonly rotuloPeriodo = computed(() => this.periodo().rotulo);
  protected readonly visiveis = computed(() => this.itens());

  ngOnInit(): void {
    this.carregar();
  }

  protected mudarVista(valor: 'mes' | 'semana' | 'dia' | 'lista'): void {
    this.vista.set(valor);
    this.carregar();
  }

  protected navegar(delta: number): void {
    const atual = this.ancora();
    const proxima = new Date(atual);
    if (this.vista() === 'mes') proxima.setMonth(proxima.getMonth() + delta);
    else if (this.vista() === 'semana') proxima.setDate(proxima.getDate() + delta * 7);
    else proxima.setDate(proxima.getDate() + delta);
    this.ancora.set(proxima);
    this.carregar();
  }

  protected hoje(): void {
    this.ancora.set(new Date());
    this.carregar();
  }

  protected abrir(item?: CompromissoAgenda): void {
    this.dialog.open(CompromissoDialog, { ...DIALOG, data: item ?? null }).afterClosed().subscribe((ok) => ok && this.carregar());
  }

  protected excluir(item: CompromissoAgenda): void {
    this.dialog.open(ConfirmaDialog, {
      ...DIALOG,
      data: { titulo: 'Excluir compromisso', texto: `Confirma a exclusão de "${item.descricao}"?`, confirmar: 'Excluir' },
    }).afterClosed().subscribe((confirmou) => {
      if (!confirmou) return;
      this.negocio.excluirCompromisso(item.id).subscribe({
        next: () => {
          this.toast.sucesso('Compromisso excluído.');
          this.carregar();
        },
        error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
      });
    });
  }

  private carregar(): void {
    const { de, ate } = this.periodo();
    this.negocio.agenda(de, ate).subscribe({
      next: (lista) => this.itens.set(lista),
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}

function calcularPeriodo(vista: string, ancora: Date): { de: string; ate: string; rotulo: string } {
  const y = ancora.getFullYear();
  const m = ancora.getMonth();
  const d = ancora.getDate();
  if (vista === 'dia') {
    const iso = deDataParaIso(ancora)!;
    return { de: iso, ate: iso, rotulo: iso };
  }
  if (vista === 'semana') {
    const inicio = new Date(ancora);
    inicio.setDate(d - ((inicio.getDay() + 6) % 7));
    const fim = new Date(inicio);
    fim.setDate(inicio.getDate() + 6);
    return { de: deDataParaIso(inicio)!, ate: deDataParaIso(fim)!, rotulo: `${deDataParaIso(inicio)} — ${deDataParaIso(fim)}` };
  }
  const inicio = new Date(y, m, 1);
  const fim = new Date(y, m + 1, 0);
  return {
    de: deDataParaIso(inicio)!,
    ate: deDataParaIso(fim)!,
    rotulo: inicio.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' }),
  };
}
