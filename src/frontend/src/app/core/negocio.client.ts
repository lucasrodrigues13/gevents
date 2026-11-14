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

export interface ItemServicoModelo {
  id: string;
  tipo: string;
  texto: string;
}

export interface ServicoModelo {
  id: string;
  nome: string;
  valorEstimado: number | null;
  itens: ItemServicoModelo[];
}

export interface TarefaModelo {
  id: string;
  titulo: string;
  diasAntes: number | null;
}

export interface VinculoModelo {
  id: string;
  tipo: string;
  bibliotecaId: string;
}

export interface PermissoesModelo {
  gerais: boolean;
  planejamento: boolean;
  tarefas: boolean;
  servicosFornecedores: boolean;
  convites: boolean;
}

export interface Modelo {
  id: string;
  nome: string;
  tipo: string;
  servicos: ServicoModelo[];
  tarefas: TarefaModelo[];
  permissoes: PermissoesModelo;
  vinculos: VinculoModelo[];
}

export interface EstatisticasEventos {
  total: number;
  situacoes: { situacao: string; quantidade: number; percentual: number }[];
  eventosPorMes: { mes: number; quantidade: number }[];
  rankingOrcamento: { posicao: number; servico: string; percentual: number }[];
  rankingFornecedores: { posicao: number; fornecedor: string; quantidade: number }[];
  rankingServicos: { posicao: number; servico: string; quantidade: number }[];
  resumo: {
    tarefasSemana: number;
    aniversariosHoje: number;
    rotuloPagamentos: string;
    rotuloTarefas: string;
    rotuloAnexos: string;
    rotuloEquipe: string;
    rotuloAniversarios: string;
    contadorTarefas: string;
    contadorAniversarios: string;
  };
}

export interface Briefing {
  id: string;
  nome: string;
  origem: string;
  padrao: boolean;
  criadoEm: string;
  secoes: {
    id: string;
    titulo: string;
    descricao: string | null;
    ordem: number;
    perguntas: { id: string; enunciado: string; tipo: string; ordem: number }[];
  }[];
}

export interface Roteiro {
  id: string;
  nome: string;
  secoes: {
    id: string;
    titulo: string | null;
    horario: string | null;
    musica: string | null;
    ordem: number;
    itens: { id: string; texto: string; ordem: number }[];
  }[];
}

export interface Cronograma {
  id: string;
  titulo: string;
  descricao: string | null;
  itens: {
    id: string;
    rotulo: string | null;
    data: string | null;
    hora: string | null;
    responsavel: string | null;
    recadoFornecedores: string | null;
    ordem: number;
  }[];
}

export interface Inspiracao {
  id: string;
  nome: string;
  itens: { id: string; arquivoId: string | null; url: string | null; ordem: number }[];
}

export interface MensagemModelo {
  id: string;
  nome: string;
  texto: string;
  parametros: string[];
}

export interface ServicoConta {
  id: string;
  nome: string;
  padrao: boolean;
}

export interface FornecedorResumo {
  id: string;
  nome: string;
  servicos: string[];
  completo: boolean;
}

export interface Fornecedor {
  id: string;
  nome: string;
  fotoId: string | null;
  razaoSocial: string | null;
  cnpj: string | null;
  cpf: string | null;
  nomeResponsavel: string | null;
  precoDe: number | null;
  precoAte: number | null;
  site: string | null;
  facebook: string | null;
  instagram: string | null;
  youtube: string | null;
  pinterest: string | null;
  pais: string | null;
  cep: string | null;
  endereco: string | null;
  numero: string | null;
  estado: string | null;
  cidade: string | null;
  bairro: string | null;
  complemento: string | null;
  banco: string | null;
  agencia: string | null;
  contaBancaria: string | null;
  titularConta: string | null;
  tipoConta: string | null;
  documentosBancarios: string | null;
  pixTipo: string | null;
  pixChave: string | null;
  diferencial: string | null;
  diferencialVisivelCliente: boolean;
  informacoesGerais: string | null;
  portfolio: string | null;
  completo: boolean;
  servicoContaIds: string[];
  servicos: string[];
  telefones: { id: string; responsavel: string | null; ddi: string | null; numero: string; movel: boolean }[];
  emails: { id: string; endereco: string }[];
  orcamentos: { id: string; titulo: string; link: string | null; anexoId: string | null; visivelClientes: boolean }[];
}

export interface BudgetEvento {
  budget: number;
  estimado: number;
  contratado: number;
  provaReal: number;
  custoPorConvidado: number | null;
  servicos: { id: string; nome: string; estimado: number; contratado: number }[];
}

export const ETAPAS_NEGOCIACAO = [
  { valor: 'Recomendado', rotulo: 'Recomendado' },
  { valor: 'OrcamentoSolicitado', rotulo: 'Orçamento solicitado' },
  { valor: 'OrcamentoRecebido', rotulo: 'Orçamento recebido' },
  { valor: 'Contratado', rotulo: 'Contratado' },
];

export const TIPOS_ITEM_CHECKLIST = [
  { valor: 'Tarefa', rotulo: 'Tarefa' },
  { valor: 'Fornecedor', rotulo: 'Fornecedor' },
  { valor: 'Alinhamento', rotulo: 'Alinhamento' },
  { valor: 'Comparativo', rotulo: 'Comparativo' },
];

export const MESES = [
  'Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez',
];

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
  servicos: EventoServico[];
  convidadosPendentes: number;
  convidadosRecusados: number;
}

export interface EventoServico {
  id: string;
  nome: string;
  contratado: boolean;
  valorEstimado: number | null;
  valorContratado: number | null;
  ocultoParaClient: boolean;
  ordem: number;
  servicoContaId: string | null;
  parcelas: { numero: number; vencimento: string; valor: number; entrada: boolean; situacao: string }[];
  subitens: { id: string; descricao: string; quantidade: number; valorUnitario: number; total: number }[];
  fornecedores: {
    id: string;
    fornecedorId: string;
    fornecedor: string;
    etapa: string;
    valorOrcamento: number | null;
    observacoes: string | null;
  }[];
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

export interface PermissoesCliente {
  convidados: boolean;
  tarefas: boolean;
  anexos: boolean;
  parcelas: boolean;
  rsvp: boolean;
  mapa: boolean;
  contrato: boolean;
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
  acessoPlataforma: boolean;
  usuarioId: string | null;
  permissoes: PermissoesCliente;
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

  modelos(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<Modelo[]>(`${this.base}/api/v1/modelos`, { params });
  }

  modelo(id: string) {
    return this.http.get<Modelo>(`${this.base}/api/v1/modelos/${id}`);
  }

  criarModelo(pedido: { nome: string; tipo: string; servicos?: string[]; tarefas?: string[] }) {
    return this.http.post<Modelo>(`${this.base}/api/v1/modelos`, pedido);
  }

  atualizarModelo(id: string, pedido: unknown) {
    return this.http.put<Modelo>(`${this.base}/api/v1/modelos/${id}`, pedido);
  }

  excluirModelo(id: string) {
    return this.http.delete(`${this.base}/api/v1/modelos/${id}`);
  }

  clonarModelo(id: string) {
    return this.http.post<Modelo>(`${this.base}/api/v1/modelos/${id}/clonar`, {});
  }

  estatisticas() {
    return this.http.get<EstatisticasEventos>(`${this.base}/api/v1/eventos/estatisticas`);
  }

  briefings(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<Briefing[]>(`${this.base}/api/v1/modelos-briefing`, { params });
  }

  briefing(id: string) {
    return this.http.get<Briefing>(`${this.base}/api/v1/modelos-briefing/${id}`);
  }

  criarBriefing(pedido: unknown) {
    return this.http.post<Briefing>(`${this.base}/api/v1/modelos-briefing`, pedido);
  }

  atualizarBriefing(id: string, pedido: unknown) {
    return this.http.put<Briefing>(`${this.base}/api/v1/modelos-briefing/${id}`, pedido);
  }

  excluirBriefing(id: string) {
    return this.http.delete(`${this.base}/api/v1/modelos-briefing/${id}`);
  }

  clonarBriefing(id: string) {
    return this.http.post<Briefing>(`${this.base}/api/v1/modelos-briefing/${id}/clonar`, {});
  }

  roteiros(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<Roteiro[]>(`${this.base}/api/v1/modelos-roteiro`, { params });
  }

  roteiro(id: string) {
    return this.http.get<Roteiro>(`${this.base}/api/v1/modelos-roteiro/${id}`);
  }

  criarRoteiro(pedido: unknown) {
    return this.http.post<Roteiro>(`${this.base}/api/v1/modelos-roteiro`, pedido);
  }

  atualizarRoteiro(id: string, pedido: unknown) {
    return this.http.put<Roteiro>(`${this.base}/api/v1/modelos-roteiro/${id}`, pedido);
  }

  excluirRoteiro(id: string) {
    return this.http.delete(`${this.base}/api/v1/modelos-roteiro/${id}`);
  }

  clonarRoteiro(id: string) {
    return this.http.post<Roteiro>(`${this.base}/api/v1/modelos-roteiro/${id}/clonar`, {});
  }

  cronogramas(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<Cronograma[]>(`${this.base}/api/v1/modelos-cronograma`, { params });
  }

  cronograma(id: string) {
    return this.http.get<Cronograma>(`${this.base}/api/v1/modelos-cronograma/${id}`);
  }

  criarCronograma(pedido: unknown) {
    return this.http.post<Cronograma>(`${this.base}/api/v1/modelos-cronograma`, pedido);
  }

  atualizarCronograma(id: string, pedido: unknown) {
    return this.http.put<Cronograma>(`${this.base}/api/v1/modelos-cronograma/${id}`, pedido);
  }

  excluirCronograma(id: string) {
    return this.http.delete(`${this.base}/api/v1/modelos-cronograma/${id}`);
  }

  clonarCronograma(id: string) {
    return this.http.post<Cronograma>(`${this.base}/api/v1/modelos-cronograma/${id}/clonar`, {});
  }

  inspiracoes(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<Inspiracao[]>(`${this.base}/api/v1/modelos-inspiracoes`, { params });
  }

  inspiracao(id: string) {
    return this.http.get<Inspiracao>(`${this.base}/api/v1/modelos-inspiracoes/${id}`);
  }

  criarInspiracao(pedido: unknown) {
    return this.http.post<Inspiracao>(`${this.base}/api/v1/modelos-inspiracoes`, pedido);
  }

  atualizarInspiracao(id: string, pedido: unknown) {
    return this.http.put<Inspiracao>(`${this.base}/api/v1/modelos-inspiracoes/${id}`, pedido);
  }

  excluirInspiracao(id: string) {
    return this.http.delete(`${this.base}/api/v1/modelos-inspiracoes/${id}`);
  }

  clonarInspiracao(id: string) {
    return this.http.post<Inspiracao>(`${this.base}/api/v1/modelos-inspiracoes/${id}/clonar`, {});
  }

  mensagens(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<MensagemModelo[]>(`${this.base}/api/v1/modelos-mensagens`, { params });
  }

  mensagem(id: string) {
    return this.http.get<MensagemModelo>(`${this.base}/api/v1/modelos-mensagens/${id}`);
  }

  criarMensagem(pedido: unknown) {
    return this.http.post<MensagemModelo>(`${this.base}/api/v1/modelos-mensagens`, pedido);
  }

  atualizarMensagem(id: string, pedido: unknown) {
    return this.http.put<MensagemModelo>(`${this.base}/api/v1/modelos-mensagens/${id}`, pedido);
  }

  excluirMensagem(id: string) {
    return this.http.delete(`${this.base}/api/v1/modelos-mensagens/${id}`);
  }

  clonarMensagem(id: string) {
    return this.http.post<MensagemModelo>(`${this.base}/api/v1/modelos-mensagens/${id}/clonar`, {});
  }

  servicosConta(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<ServicoConta[]>(`${this.base}/api/v1/servicos-conta`, { params });
  }

  criarServicoConta(nome: string) {
    return this.http.post<ServicoConta>(`${this.base}/api/v1/servicos-conta`, { nome });
  }

  excluirServicoConta(id: string) {
    return this.http.delete(`${this.base}/api/v1/servicos-conta/${id}`);
  }

  fornecedores(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<FornecedorResumo[]>(`${this.base}/api/v1/fornecedores`, { params });
  }

  fornecedor(id: string) {
    return this.http.get<Fornecedor>(`${this.base}/api/v1/fornecedores/${id}`);
  }

  criarFornecedor(pedido: unknown) {
    return this.http.post<Fornecedor>(`${this.base}/api/v1/fornecedores`, pedido);
  }

  atualizarFornecedor(id: string, pedido: unknown) {
    return this.http.put<Fornecedor>(`${this.base}/api/v1/fornecedores/${id}`, pedido);
  }

  excluirFornecedor(id: string) {
    return this.http.delete(`${this.base}/api/v1/fornecedores/${id}`);
  }

  adicionarServicoEvento(eventoId: string, nome: string) {
    return this.http.post(`${this.base}/api/v1/eventos/${eventoId}/servicos`, { nome });
  }

  atualizarEstimado(eventoId: string, servicoId: string, pedido: unknown) {
    return this.http.put(`${this.base}/api/v1/eventos/${eventoId}/servicos/${servicoId}/estimado`, pedido);
  }

  adicionarFornecedorServico(eventoId: string, servicoId: string, fornecedorId: string, etapa: string) {
    return this.http.post(`${this.base}/api/v1/eventos/${eventoId}/servicos/${servicoId}/fornecedores`, {
      fornecedorId,
      etapa,
    });
  }

  avancarEtapa(eventoId: string, servicoId: string, vinculoId: string) {
    return this.http.post(
      `${this.base}/api/v1/eventos/${eventoId}/servicos/${servicoId}/fornecedores/${vinculoId}/avancar`,
      {},
    );
  }

  voltarEtapa(eventoId: string, servicoId: string, vinculoId: string) {
    return this.http.post(
      `${this.base}/api/v1/eventos/${eventoId}/servicos/${servicoId}/fornecedores/${vinculoId}/voltar`,
      {},
    );
  }

  contratarFornecedor(eventoId: string, servicoId: string, vinculoId: string, pedido: unknown) {
    return this.http.post(
      `${this.base}/api/v1/eventos/${eventoId}/servicos/${servicoId}/fornecedores/${vinculoId}/contratar`,
      pedido,
    );
  }

  budget(eventoId: string) {
    return this.http.get<BudgetEvento>(`${this.base}/api/v1/eventos/${eventoId}/budget`);
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
    return this.http.get<{ endereco: string; estado: string; cidade: string; bairro: string; pais: string }>(
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

  responderRsvp(
    token: string,
    respostas: { convidadoId: string; situacao: string }[],
    pretendeRealizarEvento?: boolean,
  ) {
    return this.http.post<{ convite: string; convidados: { id: string; nome: string; situacao: string }[] }>(
      `${this.base}/api/v1/rsvp/${token}`,
      { respostas, pretendeRealizarEvento },
    );
  }

  criarLeadPublico(
    contaId: string,
    pedido: { nome: string; contatoNome: string; telefone?: string | null; email?: string | null; fonte?: string | null },
  ) {
    return this.http.post(`${this.base}/api/v1/public/leads/${contaId}`, pedido);
  }

  agenda(de?: string, ate?: string) {
    let params = new HttpParams();
    if (de) params = params.set('de', de);
    if (ate) params = params.set('ate', ate);
    return this.http.get<CompromissoAgenda[]>(`${this.base}/api/v1/agenda`, { params });
  }

  criarCompromisso(pedido: { descricao: string; data: string; hora?: string | null }) {
    return this.http.post<CompromissoAgenda>(`${this.base}/api/v1/agenda`, pedido);
  }

  atualizarCompromisso(id: string, pedido: { descricao: string; data: string; hora?: string | null }) {
    return this.http.put<CompromissoAgenda>(`${this.base}/api/v1/agenda/${id}`, pedido);
  }

  excluirCompromisso(id: string) {
    return this.http.delete(`${this.base}/api/v1/agenda/${id}`);
  }

  oportunidades(filtro: Record<string, string | undefined> = {}) {
    let params = new HttpParams();
    for (const [chave, valor] of Object.entries(filtro)) {
      if (valor) params = params.set(chave, valor);
    }
    return this.http.get<Oportunidade[]>(`${this.base}/api/v1/oportunidades`, { params });
  }

  oportunidade(id: string) {
    return this.http.get<Oportunidade>(`${this.base}/api/v1/oportunidades/${id}`);
  }

  criarOportunidade(pedido: unknown) {
    return this.http.post<Oportunidade>(`${this.base}/api/v1/oportunidades`, pedido);
  }

  atualizarOportunidade(id: string, pedido: unknown) {
    return this.http.put<Oportunidade>(`${this.base}/api/v1/oportunidades/${id}`, pedido);
  }

  moverOportunidade(id: string, pedido: { estagio: string; motivoDescarte?: string }) {
    return this.http.post<Oportunidade>(`${this.base}/api/v1/oportunidades/${id}/mover`, pedido);
  }

  excluirOportunidade(id: string) {
    return this.http.delete(`${this.base}/api/v1/oportunidades/${id}`);
  }

  checarDataOportunidade(data: string) {
    return this.http.get<{ temCompromisso: boolean }>(`${this.base}/api/v1/oportunidades/checar-data`, {
      params: new HttpParams().set('data', data),
    });
  }

  exportarOportunidades(filtro: Record<string, string | undefined> = {}) {
    let params = new HttpParams();
    for (const [chave, valor] of Object.entries(filtro)) {
      if (valor) params = params.set(chave, valor);
    }
    return this.http.get(`${this.base}/api/v1/oportunidades/exportar`, { params, responseType: 'blob' });
  }

  leadsRsvp(busca = '') {
    const params = busca ? new HttpParams().set('busca', busca) : undefined;
    return this.http.get<LeadRsvp[]>(`${this.base}/api/v1/leads-rsvp`, { params });
  }

  gerarOportunidadeLead(id: string) {
    return this.http.post<Oportunidade>(`${this.base}/api/v1/leads-rsvp/${id}/gerar-oportunidade`, {});
  }

  descartarLead(id: string) {
    return this.http.post(`${this.base}/api/v1/leads-rsvp/${id}/descartar`, {});
  }

  contatarLead(id: string) {
    return this.http.post(`${this.base}/api/v1/leads-rsvp/${id}/contatar`, {});
  }

  contratosFunil(filtro: Record<string, string | undefined> = {}) {
    let params = new HttpParams();
    for (const [chave, valor] of Object.entries(filtro)) {
      if (valor) params = params.set(chave, valor);
    }
    return this.http.get<ContratoFunil[]>(`${this.base}/api/v1/contratos-funil`, { params });
  }

  criarContratoFunil(pedido: unknown) {
    return this.http.post<ContratoFunil>(`${this.base}/api/v1/contratos-funil`, pedido);
  }

  atualizarContratoFunil(id: string, pedido: unknown) {
    return this.http.put<ContratoFunil>(`${this.base}/api/v1/contratos-funil/${id}`, pedido);
  }

  enviarContratoFunil(id: string) {
    return this.http.post<ContratoFunil>(`${this.base}/api/v1/contratos-funil/${id}/enviar`, {});
  }

  assinarContratoFunil(id: string) {
    return this.http.post<ContratoFunil>(`${this.base}/api/v1/contratos-funil/${id}/assinar`, {});
  }

  cancelarContratoFunil(id: string) {
    return this.http.post<ContratoFunil>(`${this.base}/api/v1/contratos-funil/${id}/cancelar`, {});
  }

  excluirContratoFunil(id: string) {
    return this.http.delete(`${this.base}/api/v1/contratos-funil/${id}`);
  }

  orcamentosFunil(filtro: Record<string, string | undefined> = {}) {
    let params = new HttpParams();
    for (const [chave, valor] of Object.entries(filtro)) {
      if (valor) params = params.set(chave, valor);
    }
    return this.http.get<OrcamentoFunil[]>(`${this.base}/api/v1/orcamentos-funil`, { params });
  }

  criarOrcamentoFunil(pedido: unknown) {
    return this.http.post<OrcamentoFunil>(`${this.base}/api/v1/orcamentos-funil`, pedido);
  }

  atualizarOrcamentoFunil(id: string, pedido: unknown) {
    return this.http.put<OrcamentoFunil>(`${this.base}/api/v1/orcamentos-funil/${id}`, pedido);
  }

  enviarOrcamentoFunil(id: string, moverParaOrcamentoEnviado = false) {
    return this.http.post<OrcamentoFunil>(`${this.base}/api/v1/orcamentos-funil/${id}/enviar`, {
      moverParaOrcamentoEnviado,
    });
  }

  responderOrcamentoFunil(id: string) {
    return this.http.post<OrcamentoFunil>(`${this.base}/api/v1/orcamentos-funil/${id}/responder`, {});
  }

  cancelarOrcamentoFunil(id: string) {
    return this.http.post<OrcamentoFunil>(`${this.base}/api/v1/orcamentos-funil/${id}/cancelar`, {});
  }

  excluirOrcamentoFunil(id: string) {
    return this.http.delete(`${this.base}/api/v1/orcamentos-funil/${id}`);
  }

  tarefasFunil(filtro: Record<string, string | undefined> = {}) {
    let params = new HttpParams();
    for (const [chave, valor] of Object.entries(filtro)) {
      if (valor) params = params.set(chave, valor);
    }
    return this.http.get<TarefaFunil[]>(`${this.base}/api/v1/tarefas-funil`, { params });
  }

  criarTarefaFunil(pedido: unknown) {
    return this.http.post<TarefaFunil>(`${this.base}/api/v1/tarefas-funil`, pedido);
  }

  atualizarTarefaFunil(id: string, pedido: unknown) {
    return this.http.put<TarefaFunil>(`${this.base}/api/v1/tarefas-funil/${id}`, pedido);
  }

  concluirTarefaFunil(id: string) {
    return this.http.post<TarefaFunil>(`${this.base}/api/v1/tarefas-funil/${id}/concluir`, {});
  }

  reabrirTarefaFunil(id: string) {
    return this.http.post<TarefaFunil>(`${this.base}/api/v1/tarefas-funil/${id}/reabrir`, {});
  }

  excluirTarefaFunil(id: string) {
    return this.http.delete(`${this.base}/api/v1/tarefas-funil/${id}`);
  }

  relatoriosFunil(inicio: string, fim: string) {
    return this.http.get<CatalogoRelatoriosFunil>(`${this.base}/api/v1/relatorios-funil`, {
      params: new HttpParams().set('inicio', inicio).set('fim', fim),
    });
  }

  exportarRelatoriosFunil(inicio: string, fim: string) {
    return this.http.get(`${this.base}/api/v1/relatorios-funil/exportar`, {
      params: new HttpParams().set('inicio', inicio).set('fim', fim),
      responseType: 'blob',
    });
  }

  modelosServicoFunil(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<ModeloServicoFunil[]>(`${this.base}/api/v1/modelos-servicos-funil`, { params });
  }

  criarModeloServicoFunil(pedido: unknown) {
    return this.http.post<ModeloServicoFunil>(`${this.base}/api/v1/modelos-servicos-funil`, pedido);
  }

  atualizarModeloServicoFunil(id: string, pedido: unknown) {
    return this.http.put<ModeloServicoFunil>(`${this.base}/api/v1/modelos-servicos-funil/${id}`, pedido);
  }

  excluirModeloServicoFunil(id: string) {
    return this.http.delete(`${this.base}/api/v1/modelos-servicos-funil/${id}`);
  }

  modelosOrcamentoFunil(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<ModeloOrcamentoFunil[]>(`${this.base}/api/v1/modelos-orcamento-funil`, { params });
  }

  criarModeloOrcamentoFunil(pedido: unknown) {
    return this.http.post<ModeloOrcamentoFunil>(`${this.base}/api/v1/modelos-orcamento-funil`, pedido);
  }

  atualizarModeloOrcamentoFunil(id: string, pedido: unknown) {
    return this.http.put<ModeloOrcamentoFunil>(`${this.base}/api/v1/modelos-orcamento-funil/${id}`, pedido);
  }

  excluirModeloOrcamentoFunil(id: string) {
    return this.http.delete(`${this.base}/api/v1/modelos-orcamento-funil/${id}`);
  }

  modelosContratoFunil(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<ModeloContratoFunil[]>(`${this.base}/api/v1/modelos-contrato-funil`, { params });
  }

  criarModeloContratoFunil(pedido: unknown) {
    return this.http.post<ModeloContratoFunil>(`${this.base}/api/v1/modelos-contrato-funil`, pedido);
  }

  atualizarModeloContratoFunil(id: string, pedido: unknown) {
    return this.http.put<ModeloContratoFunil>(`${this.base}/api/v1/modelos-contrato-funil/${id}`, pedido);
  }

  excluirModeloContratoFunil(id: string) {
    return this.http.delete(`${this.base}/api/v1/modelos-contrato-funil/${id}`);
  }

  arquivo(id: string) {
    return `${this.base}/api/v1/arquivos/${id}`;
  }
}

export interface CompromissoAgenda {
  id: string;
  descricao: string;
  data: string;
  hora: string | null;
  eventoId: string | null;
  deEvento: boolean;
}

export interface ContatoOportunidade {
  id: string;
  nome: string;
  telefone: string | null;
  email: string | null;
}

export interface Oportunidade {
  id: string;
  nome: string;
  estagio: string;
  fonte: string | null;
  valorProposta: number | null;
  responsavel: string;
  servicoProduto: string | null;
  modeloServicoFunilId: string | null;
  tipoEvento: string | null;
  quantidadeConvidados: number | null;
  dataPrevista: string | null;
  horarios: string | null;
  quantidadePadrinhos: number | null;
  quantidadeDamas: number | null;
  localCerimonia: string | null;
  enderecoCerimonia: string | null;
  localRecepcao: string | null;
  enderecoRecepcao: string | null;
  uf: string | null;
  cidade: string | null;
  motivoDescarte: string | null;
  criadoEm: string;
  entradaNoEstagioEm: string;
  contatos: ContatoOportunidade[];
}

export interface LeadRsvp {
  id: string;
  convite: string;
  evento: string;
  criadoEm: string;
  telefone: string | null;
  conviteId: string;
  eventoId: string;
}

export interface ContratoFunil {
  id: string;
  oportunidadeId: string;
  oportunidadeNome: string;
  conteudo: string;
  situacao: string;
  criadoEm: string;
  enviadoEm: string | null;
  assinadoEm: string | null;
  modeloId: string | null;
}

export interface OrcamentoFunil {
  id: string;
  oportunidadeId: string;
  oportunidadeNome: string;
  conteudo: string;
  valor: number | null;
  situacao: string;
  criadoEm: string;
  enviadoEm: string | null;
  respondidoEm: string | null;
  modeloId: string | null;
}

export interface TarefaFunil {
  id: string;
  oportunidadeId: string;
  oportunidadeNome: string;
  titulo: string;
  observacao: string | null;
  tipo: string;
  responsavel: string;
  prazo: string;
  completa: boolean;
  anexoArquivoId: string | null;
}

export interface CatalogoRelatoriosFunil {
  fechadosPerdidos: { chave: string; quantidade: number; valor: number }[];
  porFonte: { chave: string; quantidade: number; valor: number }[];
  porServicoProduto: { chave: string; quantidade: number; valor: number }[];
  porEstagio: { chave: string; quantidade: number; valor: number }[];
  porRegiao: { uf: string; cidade: string; quantidade: number; valor: number }[];
  porResponsavel: { chave: string; quantidade: number; valor: number }[];
  porDataPrevista: { chave: string; quantidade: number; valor: number }[];
  porMotivosDescarte: { chave: string; quantidade: number; valor: number }[];
  oportunidadesRsvp: {
    visualizacoes: number;
    cliquesBotao: number;
    contatados: number;
    descartados: number;
    geradas: number;
    conversao: number;
    conversoesAnteriores: number;
  };
}

export interface ModeloServicoFunil {
  id: string;
  nome: string;
  descricao: string | null;
  valor: number | null;
}

export interface ModeloOrcamentoFunil {
  id: string;
  nome: string;
  texto: string;
  corPrimaria: string | null;
  corSecundaria: string | null;
  parametros: string[];
}

export interface ModeloContratoFunil {
  id: string;
  nome: string;
  texto: string;
  parametros: string[];
}

export const ESTAGIOS_FUNIL = [
  'Contato inicial',
  'Orçamento enviado',
  'Contratado',
  'Descartado',
];

export const TIPOS_TAREFA_FUNIL = [
  { valor: 'Reuniao', rotulo: 'Reunião' },
  { valor: 'Ligacao', rotulo: 'Ligação' },
  { valor: 'Email', rotulo: 'E-mail' },
  { valor: 'Tarefa', rotulo: 'Tarefa' },
];
