import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { API_BASE_URL } from './api-base-url';
import { ContaClient } from './conta.client';

describe('ContaClient', () => {
  it('usa a URL da API configurada', () => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: API_BASE_URL, useValue: 'http://api.teste' },
      ],
    });

    const cliente = TestBed.inject(ContaClient);
    const http = TestBed.inject(HttpTestingController);

    cliente.entrar('pessoa@gevents.test', 'abcdefgh').subscribe();

    const pedido = http.expectOne('http://api.teste/api/v1/conta/entrar');
    expect(pedido.request.method).toBe('POST');
    pedido.flush({ token: 't', email: 'pessoa@gevents.test', papeis: ['User'] });
    http.verify();
  });
});
