import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: 'rsvp/:token', loadComponent: () => import('./pages/rsvp/rsvp.page').then((m) => m.RsvpPage) },
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
      { path: 'conta', loadComponent: () => import('./pages/conta/area-usuario.page').then((m) => m.AreaUsuarioPage) },
      {
        path: 'admin',
        canActivate: [adminGuard],
        loadComponent: () => import('./pages/admin/area-admin.page').then((m) => m.AreaAdminPage),
      },
    ],
  },
];
