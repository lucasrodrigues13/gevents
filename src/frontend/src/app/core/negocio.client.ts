import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API_BASE_URL } from './api-base-url';

export interface Painel {
  emAndamento: number;
  concluidos: number;
  cancelados: number;
  eventosEmAndamento: { id: string; nome: string; data: string | null }[];
  funil: { estagio: string; quantidade: number; valor: number }[];
}

export interface Perfil {
  email: string;
  nomeEmpresa: string | null;
  responsavel: string | null;
  documento: string | null;
  celular: string | null;
  nascimento: string | null;
  endereco: string | null;
}

export interface Modelo {
  id: string;
  nome: string;
  tipo: string;
  servicos: string[];
  tarefas: string[];
}

export interface EventoResumo {
  id: string;
  nome: string;
  situacao: string;
  tipo: string;
  data: string | null;
  diasRestantes: number | null;
  tags: string[];
  tarefasConcluidas: number;
  tarefas: number;
  convidadosConfirmados: number;
  convidados: number;
}

export interface Local {
  nome: string | null;
  pais: string | null;
  cep: string | null;
  endereco: string | null;
  numero: string | null;
  estado: string | null;
  cidade: string | null;
  bairro: string | null;
}

export interface EventoDetalhe extends EventoResumo {
  primeiroNome: string | null;
  segundoNome: string | null;
  hora: string | null;
  orcamentoEstimado: number | null;
  convidadosEstimados: number | null;
  tempoFesta: string | null;
  inicioCerimonia: string | null;
  inicioRecepcao: string | null;
  quantidadePadrinhos: number | null;
  quantidadeDamas: number | null;
  urlSite: string | null;
  senhaSite: string | null;
  notificarAssessor: boolean;
  notificarAssistentes: boolean;
  notificarClientes: boolean;
  salvarNaAgenda: boolean;
  vinculoGoogle: string | null;
  imagemId: string | null;
  transferAtivo: boolean;
  locaisTransfer: string[];
  cerimonia: Local;
  recepcao: Local;
  servicos: { id: string; nome: string; contratado: boolean; valorContratado: number | null; parcelas: { numero: number; vencimento: string; valor: number; entrada: boolean }[] }[];
  convidadosPendentes: number;
  convidadosRecusados: number;
}

export interface Tarefa {
  id: string;
  eventoId: string;
  evento: string;
  titulo: string;
  descricao: string | null;
  prazo: string | null;
  situacao: string;
  responsavelContaId: string;
}

export interface Cliente {
  id: string;
  nome: string;
  papel: string;
  nascimento: string | null;
  email: string;
  ddi: string;
  celular: string;
  telefoneFixo: string | null;
  cpf: string | null;
  rg: string | null;
  cnpj: string | null;
  profissao: string | null;
  inscricaoMunicipal: string | null;
  religiao: string | null;
  observacao: string | null;
  facebook: string | null;
  instagram: string | null;
  pais: string | null;
  cep: string | null;
  endereco: string | null;
  numero: string | null;
  estado: string | null;
  cidade: string | null;
  bairro: string | null;
  complemento: string | null;
  fotoId: string | null;
  ultimoAcesso: string | null;
  eventos: { id: string; nome: string }[];
}

export interface Convidado {
  id: string;
  nome: string;
  ddi: string | null;
  celular: string | null;
  situacao: string;
  situacaoTela: string;
  situacaoIcone: string;
  mesa: string | null;
  genero: string | null;
  faixa: string | null;
  custo: string | null;
  rg: string | null;
  cpf: string | null;
  observacao: string | null;
  principal: boolean;
}

export interface Convite {
  id: string;
  eventoId: string;
  nome: string;
  grupoId: string | null;
  grupo: string | null;
  observacao: string | null;
  usaTransfer: boolean;
  localOrigem: string | null;
  telefone: string | null;
  token: string;
  acessada: boolean;
  respondida: boolean;
  convidados: Convidado[];
}

export interface ListaConvites {
  convites: Convite[];
  porSituacao: { rotulo: string; quantidade: number }[];
  porFaixa: { rotulo: string; quantidade: number }[];
  porCusto: { rotulo: string; quantidade: number }[];
  mesas: string[];
}

export const PAPEIS = [
  'Anfitriao', 'Aniversariante', 'Avo', 'AvoHomem', 'Comissao', 'Formando', 'Mae', 'Noiva', 'Noivo', 'Pai', 'Promotor', 'Representante', 'Tia', 'Tio',
];

export const ROTULO_PAPEL: Record<string, string> = {
  Anfitriao: 'Anfitrião',
  Aniversariante: 'Aniversariante',
  Avo: 'Avó',
  AvoHomem: 'Avô',
  Comissao: 'Comissão',
  Formando: 'Formando(a)',
  Mae: 'Mãe',
  Noiva: 'Noiva',
  Noivo: 'Noivo',
  Pai: 'Pai',
  Promotor: 'Promotor',
  Representante: 'Representante',
  Tia: 'Tia',
  Tio: 'Tio',
};

export const TIPOS = [
  { valor: 'Casamento', rotulo: 'Casamento' },
  { valor: 'QuinzeAnos', rotulo: '15 anos' },
  { valor: 'Aniversario', rotulo: 'Aniversário' },
  { valor: 'Corporativo', rotulo: 'Corporativo' },
  { valor: 'Formatura', rotulo: 'Formatura' },
];

export const SITUACOES_TAREFA = [
  { valor: 'Pendente', rotulo: 'Pendente' },
  { valor: 'EmAndamento', rotulo: 'Em andamento' },
  { valor: 'Concluida', rotulo: 'Concluída' },
  { valor: 'Atrasada', rotulo: 'Atrasada' },
];

@Injectable({ providedIn: 'root' })
export class NegocioClient {
  private readonly http = inject(HttpClient);
  private readonly base = inject(API_BASE_URL);

  painel() {
    return this.http.get<Painel>(`${this.base}/api/v1/painel`);
  }

  perfil() {
    return this.http.get<Perfil>(`${this.base}/api/v1/perfil`);
  }

  salvarPerfil(perfil: Partial<Perfil>) {
    return this.http.put<Perfil>(`${this.base}/api/v1/perfil`, perfil);
  }

  modelos() {
    return this.http.get<Modelo[]>(`${this.base}/api/v1/modelos`);
  }

  criarModelo(pedido: { nome: string; tipo: string; servicos: string[]; tarefas: string[] }) {
    return this.http.post<Modelo>(`${this.base}/api/v1/modelos`, pedido);
  }

  eventos(filtro: Record<string, string>) {
    let params = new HttpParams();
    for (const [chave, valor] of Object.entries(filtro)) {
      if (valor) {
        params = params.set(chave, valor);
      }
    }
    return this.http.get<EventoResumo[]>(`${this.base}/api/v1/eventos`, { params });
  }

  evento(id: string) {
    return this.http.get<EventoDetalhe>(`${this.base}/api/v1/eventos/${id}`);
  }

  criarEvento(pedido: unknown) {
    return this.http.post<EventoDetalhe>(`${this.base}/api/v1/eventos`, pedido);
  }

  atualizarEvento(id: string, pedido: unknown) {
    return this.http.put<EventoDetalhe>(`${this.base}/api/v1/eventos/${id}`, pedido);
  }

  concluir(id: string) {
    return this.http.post<EventoDetalhe>(`${this.base}/api/v1/eventos/${id}/concluir`, {});
  }

  cancelar(id: string) {
    return this.http.post<EventoDetalhe>(`${this.base}/api/v1/eventos/${id}/cancelar`, {});
  }

  assessoria(id: string, pedido: unknown) {
    return this.http.post<EventoDetalhe>(`${this.base}/api/v1/eventos/${id}/assessoria`, pedido);
  }

  transfer(id: string, ativo: boolean, locais: string[]) {
    return this.http.put<EventoDetalhe>(`${this.base}/api/v1/eventos/${id}/transfer`, { ativo, locais });
  }

  imagemEvento(id: string, arquivo: File) {
    const dados = new FormData();
    dados.append('arquivo', arquivo);
    return this.http.post(`${this.base}/api/v1/eventos/${id}/imagem`, dados);
  }

  tarefas() {
    return this.http.get<Tarefa[]>(`${this.base}/api/v1/tarefas`);
  }

  tarefasDoEvento(id: string) {
    return this.http.get<Tarefa[]>(`${this.base}/api/v1/eventos/${id}/tarefas`);
  }

  criarTarefa(eventoId: string, pedido: unknown) {
    return this.http.post<Tarefa>(`${this.base}/api/v1/eventos/${eventoId}/tarefas`, pedido);
  }

  atualizarTarefa(id: string, pedido: unknown) {
    return this.http.put<Tarefa>(`${this.base}/api/v1/tarefas/${id}`, pedido);
  }

  clientes(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<Cliente[]>(`${this.base}/api/v1/clientes`, { params });
  }

  criarCliente(pedido: unknown) {
    return this.http.post<Cliente>(`${this.base}/api/v1/clientes`, pedido);
  }

  atualizarCliente(id: string, pedido: unknown) {
    return this.http.put<Cliente>(`${this.base}/api/v1/clientes/${id}`, pedido);
  }

  clientesDoEvento(id: string) {
    return this.http.get<Cliente[]>(`${this.base}/api/v1/eventos/${id}/clientes`);
  }

  vincular(eventoId: string, clienteId: string) {
    return this.http.post(`${this.base}/api/v1/eventos/${eventoId}/clientes`, { clienteId });
  }

  desvincular(eventoId: string, clienteId: string) {
    return this.http.delete(`${this.base}/api/v1/eventos/${eventoId}/clientes/${clienteId}`);
  }

  cep(cep: string) {
    return this.http.get<{ endereco: string; estado: string; cidade: string; bairro: string }>(
      `${this.base}/api/v1/cep/${cep}`,
    );
  }

  grupos() {
    return this.http.get<{ id: string; nome: string }[]>(`${this.base}/api/v1/grupos`);
  }

  criarGrupo(nome: string) {
    return this.http.post<{ id: string; nome: string }>(`${this.base}/api/v1/grupos`, { nome });
  }

  convites(eventoId: string, filtro: Record<string, string>) {
    let params = new HttpParams();
    for (const [chave, valor] of Object.entries(filtro)) {
      if (valor) {
        params = params.set(chave, valor);
      }
    }
    return this.http.get<ListaConvites>(`${this.base}/api/v1/eventos/${eventoId}/convites`, { params });
  }

  criarConvite(eventoId: string, pedido: unknown) {
    return this.http.post<Convite>(`${this.base}/api/v1/eventos/${eventoId}/convites`, pedido);
  }

  removerConvidado(id: string) {
    return this.http.delete(`${this.base}/api/v1/convidados/${id}`);
  }

  definirPrincipal(conviteId: string, convidadoId: string) {
    return this.http.post<Convite>(`${this.base}/api/v1/convites/${conviteId}/principal`, { convidadoId });
  }

  importar(eventoId: string, arquivo: File) {
    const dados = new FormData();
    dados.append('arquivo', arquivo);
    return this.http.post(`${this.base}/api/v1/eventos/${eventoId}/convites/importar`, dados);
  }

  planilha(eventoId: string) {
    return this.http.get(`${this.base}/api/v1/eventos/${eventoId}/convites/planilha`, { responseType: 'blob' });
  }

  qr(conviteId: string) {
    return this.http.get(`${this.base}/api/v1/convites/${conviteId}/qr`, { responseType: 'blob' });
  }

  whatsapp(conviteId: string) {
    return this.http.get<{ url: string | null }>(`${this.base}/api/v1/convites/${conviteId}/whatsapp`);
  }

  abrirRsvp(token: string) {
    return this.http.get<{ convite: string; convidados: { id: string; nome: string; situacao: string }[] }>(
      `${this.base}/api/v1/rsvp/${token}`,
    );
  }

  responderRsvp(token: string, respostas: { convidadoId: string; situacao: string }[]) {
    return this.http.post<{ convite: string; convidados: { id: string; nome: string; situacao: string }[] }>(
      `${this.base}/api/v1/rsvp/${token}`,
      { respostas },
    );
  }

  arquivo(id: string) {
    return `${this.base}/api/v1/arquivos/${id}`;
  }
}
