import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/auth.guard';

export const routes: Routes = [
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
      { path: 'conta', loadComponent: () => import('./pages/conta/area-usuario.page').then((m) => m.AreaUsuarioPage) },
      {
        path: 'admin',
        canActivate: [adminGuard],
        loadComponent: () => import('./pages/admin/area-admin.page').then((m) => m.AreaAdminPage),
      },
    ],
  },
];
