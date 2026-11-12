import { HttpErrorResponse } from '@angular/common/http';
import { mensagemDeErro } from './mensagem-erro';

describe('mensagemDeErro', () => {
  it('lê o JSON de erro da API', () => {
    const erro = new HttpErrorResponse({
      status: 401,
      error: { code: 'credencial', message: 'E-mail ou senha inválidos.' },
    });

    expect(mensagemDeErro(erro)).toBe('E-mail ou senha inválidos.');
  });

  it('usa uma mensagem genérica quando a resposta não tem o contrato', () => {
    expect(mensagemDeErro(new HttpErrorResponse({ status: 500, error: 'falha' }))).toBe(
      'Não foi possível concluir a ação.',
    );
  });
});
