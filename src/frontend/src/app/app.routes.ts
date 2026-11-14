import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: 'rsvp/:token', loadComponent: () => import('./pages/rsvp/rsvp.page').then((m) => m.RsvpPage) },
  {
    path: 'lead/:contaId',
    loadComponent: () => import('./pages/lead-publico/lead-publico.page').then((m) => m.LeadPublicoPage),
  },
  { path: '', pathMatch: 'full', redirectTo: 'entrar' },
  { path: 'entrar', loadComponent: () => import('./pages/entrar/entrar.page').then((m) => m.EntrarPage) },
  { path: 'cadastro', loadComponent: () => import('./pages/cadastro/cadastro.page').then((m) => m.CadastroPage) },
  {
    path: 'confirmar-email',
    loadComponent: () => import('./pages/confirmar-email/confirmar-email.page').then((m) => m.ConfirmarEmailPage),
  },
  {
    path: 'reenviar-confirmacao',
    loadComponent: () => import('./pages/reenviar/reenviar.page').then((m) => m.ReenviarPage),
  },
  {
    path: 'esqueci-senha',
    loadComponent: () => import('./pages/esqueci-senha/esqueci-senha.page').then((m) => m.EsqueciSenhaPage),
  },
  {
    path: 'redefinir-senha',
    loadComponent: () => import('./pages/redefinir-senha/redefinir-senha.page').then((m) => m.RedefinirSenhaPage),
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/casca.component').then((m) => m.CascaComponent),
    children: [
      { path: 'painel', loadComponent: () => import('./pages/painel/painel.page').then((m) => m.PainelPage) },
      { path: 'perfil', loadComponent: () => import('./pages/perfil/perfil.page').then((m) => m.PerfilPage) },
      { path: 'eventos', loadComponent: () => import('./pages/eventos/eventos.page').then((m) => m.EventosPage) },
      { path: 'eventos/novo', loadComponent: () => import('./pages/eventos/evento-form.page').then((m) => m.EventoFormPage) },
      { path: 'eventos/:id', loadComponent: () => import('./pages/eventos/evento-home.page').then((m) => m.EventoHomePage) },
      { path: 'eventos/:id/editar', loadComponent: () => import('./pages/eventos/evento-form.page').then((m) => m.EventoFormPage) },
      { path: 'eventos/:id/tarefas', loadComponent: () => import('./pages/tarefas/tarefas.page').then((m) => m.TarefasPage) },
      { path: 'eventos/:id/convidados', loadComponent: () => import('./pages/convidados/convidados.page').then((m) => m.ConvidadosPage) },
      { path: 'tarefas', loadComponent: () => import('./pages/tarefas/tarefas.page').then((m) => m.TarefasPage) },
      { path: 'clientes', loadComponent: () => import('./pages/clientes/clientes.page').then((m) => m.ClientesPage) },
      { path: 'convidados', loadComponent: () => import('./pages/convidados/convidados.page').then((m) => m.ConvidadosPage) },
      { path: 'pagamentos', loadComponent: () => import('./pages/atalhos/em-breve.page').then((m) => m.EmBrevePage) },
      { path: 'anexos', loadComponent: () => import('./pages/atalhos/em-breve.page').then((m) => m.EmBrevePage) },
      { path: 'equipe', loadComponent: () => import('./pages/atalhos/em-breve.page').then((m) => m.EmBrevePage) },
      { path: 'aniversarios', loadComponent: () => import('./pages/atalhos/em-breve.page').then((m) => m.EmBrevePage) },
      {
        path: 'modelos-evento',
        loadComponent: () => import('./pages/modelos-evento/modelos-evento.page').then((m) => m.ModelosEventoPage),
      },
      {
        path: 'modelos-evento/:id',
        loadComponent: () => import('./pages/modelos-evento/modelo-evento-editor.page').then((m) => m.ModeloEventoEditorPage),
      },
      {
        path: 'modelos-briefing',
        loadComponent: () => import('./pages/modelos-briefing/modelos-briefing.page').then((m) => m.ModelosBriefingPage),
      },
      {
        path: 'modelos-roteiro',
        loadComponent: () => import('./pages/modelos-roteiro/modelos-roteiro.page').then((m) => m.ModelosRoteiroPage),
      },
      {
        path: 'modelos-cronograma',
        loadComponent: () => import('./pages/modelos-cronograma/modelos-cronograma.page').then((m) => m.ModelosCronogramaPage),
      },
      {
        path: 'modelos-inspiracoes',
        loadComponent: () => import('./pages/modelos-inspiracoes/modelos-inspiracoes.page').then((m) => m.ModelosInspiracoesPage),
      },
      {
        path: 'modelos-mensagens',
        loadComponent: () => import('./pages/modelos-mensagens/modelos-mensagens.page').then((m) => m.ModelosMensagensPage),
      },
      {
        path: 'fornecedores',
        loadComponent: () => import('./pages/fornecedores/fornecedores.page').then((m) => m.FornecedoresPage),
      },
      { path: 'agenda', loadComponent: () => import('./pages/agenda/agenda.page').then((m) => m.AgendaPage) },
      {
        path: 'oportunidades',
        loadComponent: () => import('./pages/oportunidades/oportunidades.page').then((m) => m.OportunidadesPage),
      },
      {
        path: 'leads-rsvp',
        loadComponent: () => import('./pages/leads-rsvp/leads-rsvp.page').then((m) => m.LeadsRsvpPage),
      },
      {
        path: 'contratos-funil',
        loadComponent: () => import('./pages/contratos-funil/contratos-funil.page').then((m) => m.ContratosFunilPage),
      },
      {
        path: 'orcamentos-funil',
        loadComponent: () => import('./pages/orcamentos-funil/orcamentos-funil.page').then((m) => m.OrcamentosFunilPage),
      },
      {
        path: 'tarefas-funil',
        loadComponent: () => import('./pages/tarefas-funil/tarefas-funil.page').then((m) => m.TarefasFunilPage),
      },
      {
        path: 'relatorios-funil',
        loadComponent: () => import('./pages/relatorios-funil/relatorios-funil.page').then((m) => m.RelatoriosFunilPage),
      },
      {
        path: 'modelos-servicos-funil',
        loadComponent: () => import('./pages/modelos-funil/modelos-funil.page').then((m) => m.ModelosServicosFunilPage),
      },
      {
        path: 'modelos-orcamento-funil',
        loadComponent: () => import('./pages/modelos-funil/modelos-funil.page').then((m) => m.ModelosOrcamentoFunilPage),
      },
      {
        path: 'modelos-contrato-funil',
        loadComponent: () => import('./pages/modelos-funil/modelos-funil.page').then((m) => m.ModelosContratoFunilPage),
      },
      { path: 'conta', loadComponent: () => import('./pages/conta/area-usuario.page').then((m) => m.AreaUsuarioPage) },
      {
        path: 'admin',
        canActivate: [adminGuard],
        loadComponent: () => import('./pages/admin/area-admin.page').then((m) => m.AreaAdminPage),
      },
    ],
  },
];
