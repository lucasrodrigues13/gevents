import { Component, inject, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { mensagemDeErro } from '../../core/mensagem-erro';
import { Fornecedor, NegocioClient, ServicoConta } from '../../core/negocio.client';
import { ToastService } from '../../core/toast.service';
import { MascaraDirective } from '../../shared/mascara.directive';
import { aplicarMascara } from '../../shared/mascaras';

@Component({
  selector: 'app-fornecedor-form-dialog',
  imports: [
    ReactiveFormsModule, MatDialogModule, MatButtonModule, MatCheckboxModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatTabsModule, MatIconModule, MascaraDirective,
  ],
  template: `
    <h2 mat-dialog-title>{{ dados ? 'Editar fornecedor' : 'Novo fornecedor' }}</h2>
    <mat-dialog-content>
      <form class="formulario" [formGroup]="form">
        <mat-tab-group>
          <mat-tab label="Dados">
            <div class="aba">
              <mat-form-field appearance="outline"><mat-label>Nome</mat-label><input matInput formControlName="nome" /></mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Serviços</mat-label>
                <mat-select formControlName="servicoContaIds" multiple>
                  @for (servico of servicos(); track servico.id) {
                    <mat-option [value]="servico.id">{{ servico.nome }}</mat-option>
                  }
                </mat-select>
              </mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Razão social</mat-label><input matInput formControlName="razaoSocial" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>CNPJ</mat-label><input matInput formControlName="cnpj" gdMascara="cnpj" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>CPF</mat-label><input matInput formControlName="cpf" gdMascara="cpf" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Responsável</mat-label><input matInput formControlName="nomeResponsavel" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Preço de</mat-label><input matInput type="number" formControlName="precoDe" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Preço até</mat-label><input matInput type="number" formControlName="precoAte" /></mat-form-field>
              <div formArrayName="telefones">
                @for (tel of telefones.controls; track $index; let i = $index) {
                  <div class="linha" [formGroupName]="i">
                    <mat-form-field appearance="outline"><mat-label>DDI</mat-label><input matInput formControlName="ddi" /></mat-form-field>
                    <mat-form-field appearance="outline" class="grow"><mat-label>Telefone</mat-label><input matInput formControlName="numero" gdMascara="celular" /></mat-form-field>
                    <mat-checkbox formControlName="movel">Móvel</mat-checkbox>
                    <button mat-icon-button type="button" (click)="removerTelefone(i)"><mat-icon>close</mat-icon></button>
                  </div>
                }
              </div>
              <button mat-stroked-button type="button" (click)="adicionarTelefone()">Adicionar telefone</button>
              <div formArrayName="emails">
                @for (email of emails.controls; track $index; let i = $index) {
                  <div class="linha" [formGroupName]="i">
                    <mat-form-field appearance="outline" class="grow"><mat-label>E-mail</mat-label><input matInput formControlName="endereco" /></mat-form-field>
                    <button mat-icon-button type="button" (click)="removerEmail(i)"><mat-icon>close</mat-icon></button>
                  </div>
                }
              </div>
              <button mat-stroked-button type="button" (click)="adicionarEmail()">Adicionar e-mail</button>
            </div>
          </mat-tab>
          <mat-tab label="Redes sociais">
            <div class="aba">
              <mat-form-field appearance="outline"><mat-label>Site</mat-label><input matInput formControlName="site" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Facebook</mat-label><input matInput formControlName="facebook" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Instagram</mat-label><input matInput formControlName="instagram" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Youtube</mat-label><input matInput formControlName="youtube" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Pinterest</mat-label><input matInput formControlName="pinterest" /></mat-form-field>
            </div>
          </mat-tab>
          <mat-tab label="Endereço">
            <div class="aba">
              <mat-form-field appearance="outline"><mat-label>CEP</mat-label><input matInput formControlName="cep" gdMascara="cep" (blur)="completarCep()" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>País</mat-label><input matInput formControlName="pais" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Endereço</mat-label><input matInput formControlName="endereco" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Número</mat-label><input matInput formControlName="numero" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Estado</mat-label><input matInput formControlName="estado" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Cidade</mat-label><input matInput formControlName="cidade" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Bairro</mat-label><input matInput formControlName="bairro" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Complemento</mat-label><input matInput formControlName="complemento" /></mat-form-field>
            </div>
          </mat-tab>
          <mat-tab label="Dados bancários">
            <div class="aba">
              <mat-form-field appearance="outline"><mat-label>Banco</mat-label><input matInput formControlName="banco" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Agência</mat-label><input matInput formControlName="agencia" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Conta</mat-label><input matInput formControlName="contaBancaria" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Titular</mat-label><input matInput formControlName="titularConta" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Tipo de conta</mat-label><input matInput formControlName="tipoConta" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Documentos</mat-label><input matInput formControlName="documentosBancarios" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Pix tipo</mat-label><input matInput formControlName="pixTipo" /></mat-form-field>
              <mat-form-field appearance="outline"><mat-label>Pix chave</mat-label><input matInput formControlName="pixChave" /></mat-form-field>
            </div>
          </mat-tab>
          <mat-tab label="Orçamentos">
            <div class="aba" formArrayName="orcamentos">
              @for (orc of orcamentos.controls; track $index; let i = $index) {
                <div class="linha" [formGroupName]="i">
                  <mat-form-field appearance="outline" class="grow"><mat-label>Título</mat-label><input matInput formControlName="titulo" /></mat-form-field>
                  <mat-form-field appearance="outline" class="grow"><mat-label>Link</mat-label><input matInput formControlName="link" /></mat-form-field>
                  <mat-checkbox formControlName="visivelClientes">Visível</mat-checkbox>
                  <button mat-icon-button type="button" (click)="removerOrcamento(i)"><mat-icon>close</mat-icon></button>
                </div>
              }
              <button mat-stroked-button type="button" (click)="adicionarOrcamento()">Adicionar orçamento</button>
            </div>
          </mat-tab>
          <mat-tab label="Diferencial">
            <div class="aba">
              <mat-form-field appearance="outline" class="full"><mat-label>Diferencial</mat-label><textarea matInput rows="4" formControlName="diferencial"></textarea></mat-form-field>
              <mat-checkbox formControlName="diferencialVisivelCliente">Visível ao cliente</mat-checkbox>
            </div>
          </mat-tab>
          <mat-tab label="Inf. gerais">
            <div class="aba">
              <mat-form-field appearance="outline" class="full"><mat-label>Informações gerais</mat-label><textarea matInput rows="4" formControlName="informacoesGerais"></textarea></mat-form-field>
            </div>
          </mat-tab>
          <mat-tab label="Portfólio">
            <div class="aba">
              <mat-form-field appearance="outline" class="full"><mat-label>Portfólio</mat-label><textarea matInput rows="4" formControlName="portfolio"></textarea></mat-form-field>
            </div>
          </mat-tab>
        </mat-tab-group>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="ref.close()">Cancelar</button>
      <button mat-flat-button type="button" (click)="salvar()">Salvar</button>
    </mat-dialog-actions>
  `,
  styles: `
    .aba { padding: 12px 0; display: flex; flex-direction: column; gap: 8px; }
    .linha { display: flex; gap: 8px; align-items: center; flex-wrap: wrap; }
    .grow { flex: 1; min-width: 140px; }
    .full { width: 100%; }
  `,
})
export class FornecedorFormDialog implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly negocio = inject(NegocioClient);
  private readonly toast = inject(ToastService);
  protected readonly dados = inject<Fornecedor | null>(MAT_DIALOG_DATA);
  protected readonly ref = inject(MatDialogRef<FornecedorFormDialog, boolean>);
  protected readonly servicos = signal<ServicoConta[]>([]);

  protected readonly form = this.fb.group({
    nome: [this.dados?.nome ?? '', Validators.required],
    servicoContaIds: [this.dados?.servicoContaIds ?? [] as string[], Validators.required],
    razaoSocial: [this.dados?.razaoSocial ?? ''],
    cnpj: [aplicarMascara(this.dados?.cnpj ?? '', 'cnpj')],
    cpf: [aplicarMascara(this.dados?.cpf ?? '', 'cpf')],
    nomeResponsavel: [this.dados?.nomeResponsavel ?? ''],
    precoDe: [this.dados?.precoDe ?? null as number | null],
    precoAte: [this.dados?.precoAte ?? null as number | null],
    site: [this.dados?.site ?? ''],
    facebook: [this.dados?.facebook ?? ''],
    instagram: [this.dados?.instagram ?? ''],
    youtube: [this.dados?.youtube ?? ''],
    pinterest: [this.dados?.pinterest ?? ''],
    pais: [this.dados?.pais ?? ''],
    cep: [aplicarMascara(this.dados?.cep ?? '', 'cep')],
    endereco: [this.dados?.endereco ?? ''],
    numero: [this.dados?.numero ?? ''],
    estado: [this.dados?.estado ?? ''],
    cidade: [this.dados?.cidade ?? ''],
    bairro: [this.dados?.bairro ?? ''],
    complemento: [this.dados?.complemento ?? ''],
    banco: [this.dados?.banco ?? ''],
    agencia: [this.dados?.agencia ?? ''],
    contaBancaria: [this.dados?.contaBancaria ?? ''],
    titularConta: [this.dados?.titularConta ?? ''],
    tipoConta: [this.dados?.tipoConta ?? ''],
    documentosBancarios: [this.dados?.documentosBancarios ?? ''],
    pixTipo: [this.dados?.pixTipo ?? ''],
    pixChave: [this.dados?.pixChave ?? ''],
    diferencial: [this.dados?.diferencial ?? ''],
    diferencialVisivelCliente: [this.dados?.diferencialVisivelCliente ?? false],
    informacoesGerais: [this.dados?.informacoesGerais ?? ''],
    portfolio: [this.dados?.portfolio ?? ''],
    telefones: this.fb.array([]),
    emails: this.fb.array([]),
    orcamentos: this.fb.array([]),
  });

  protected get telefones(): FormArray {
    return this.form.get('telefones') as FormArray;
  }

  protected get emails(): FormArray {
    return this.form.get('emails') as FormArray;
  }

  protected get orcamentos(): FormArray {
    return this.form.get('orcamentos') as FormArray;
  }

  ngOnInit(): void {
    this.negocio.servicosConta().subscribe({
      next: (lista) => this.servicos.set(lista),
      error: () => this.servicos.set([]),
    });
    for (const tel of this.dados?.telefones ?? []) {
      this.telefones.push(this.fb.group({
        responsavel: [tel.responsavel ?? ''],
        ddi: [tel.ddi ?? '55'],
        numero: [aplicarMascara(tel.numero, 'celular')],
        movel: [tel.movel],
      }));
    }
    for (const email of this.dados?.emails ?? []) {
      this.emails.push(this.fb.group({ endereco: [email.endereco] }));
    }
    for (const orc of this.dados?.orcamentos ?? []) {
      this.orcamentos.push(this.fb.group({
        titulo: [orc.titulo],
        link: [orc.link ?? ''],
        visivelClientes: [orc.visivelClientes],
      }));
    }
  }

  protected adicionarTelefone(): void {
    this.telefones.push(this.fb.group({ responsavel: [''], ddi: ['55'], numero: [''], movel: [true] }));
  }

  protected removerTelefone(i: number): void {
    this.telefones.removeAt(i);
  }

  protected adicionarEmail(): void {
    this.emails.push(this.fb.group({ endereco: [''] }));
  }

  protected removerEmail(i: number): void {
    this.emails.removeAt(i);
  }

  protected adicionarOrcamento(): void {
    this.orcamentos.push(this.fb.group({ titulo: [''], link: [''], visivelClientes: [false] }));
  }

  protected removerOrcamento(i: number): void {
    this.orcamentos.removeAt(i);
  }

  protected completarCep(): void {
    const digitos = (this.form.controls.cep.value ?? '').replace(/\D/g, '');
    if (digitos.length !== 8) {
      return;
    }
    this.negocio.cep(digitos).subscribe({
      next: (endereco) => this.form.patchValue({
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
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toast.erro('Informe o nome e ao menos um serviço.');
      return;
    }
    const valor = this.form.getRawValue() as Record<string, unknown> & {
      cnpj: string | null;
      cpf: string | null;
      precoDe: number | string | null;
      precoAte: number | string | null;
      telefones: { numero?: string }[];
      emails: { endereco?: string }[];
      orcamentos: { titulo?: string }[];
    };
    const pedido = {
      ...valor,
      cnpj: valor.cnpj || null,
      cpf: valor.cpf || null,
      precoDe: valor.precoDe === null || valor.precoDe === '' ? null : Number(valor.precoDe),
      precoAte: valor.precoAte === null || valor.precoAte === '' ? null : Number(valor.precoAte),
      telefones: (valor.telefones ?? []).filter((t) => t.numero),
      emails: (valor.emails ?? []).filter((e) => e.endereco),
      orcamentos: (valor.orcamentos ?? []).filter((o) => o.titulo),
    };
    const req = this.dados
      ? this.negocio.atualizarFornecedor(this.dados.id, pedido)
      : this.negocio.criarFornecedor(pedido);
    req.subscribe({
      next: () => {
        this.toast.sucesso('Fornecedor salvo.');
        this.ref.close(true);
      },
      error: (erro: unknown) => this.toast.erro(mensagemDeErro(erro)),
    });
  }
}
