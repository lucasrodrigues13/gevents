import { TestBed } from '@angular/core/testing';
import { provideRouter, Router, UrlTree } from '@angular/router';
import { AuthService } from './auth.service';
import { adminGuard, authGuard } from './auth.guard';

describe('guardas de conta', () => {
  beforeEach(() => {
    sessionStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideRouter([])],
    });
  });

  it('redireciona quem não está autenticado', () => {
    const resultado = TestBed.runInInjectionContext(() => authGuard({} as never, {} as never));
    const router = TestBed.inject(Router);

    expect(resultado).toBeInstanceOf(UrlTree);
    expect(router.serializeUrl(resultado as UrlTree)).toBe('/entrar');
  });

  it('bloqueia User na rota de Admin', () => {
    TestBed.inject(AuthService).guardar({
      token: 'token-de-teste',
      email: 'pessoa@gevents.test',
      papeis: ['User'],
    });

    const resultado = TestBed.runInInjectionContext(() => adminGuard({} as never, {} as never));
    const router = TestBed.inject(Router);

    expect(resultado).toBeInstanceOf(UrlTree);
    expect(router.serializeUrl(resultado as UrlTree)).toBe('/conta');
  });
});
