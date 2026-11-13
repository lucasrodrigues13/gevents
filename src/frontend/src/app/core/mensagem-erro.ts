import { HttpErrorResponse } from '@angular/common/http';

export function mensagemDeErro(erro: unknown): string {
  if (erro instanceof HttpErrorResponse) {
    const corpo = erro.error;
    if (corpo && typeof corpo === 'object' && 'message' in corpo && typeof corpo.message === 'string') {
      return corpo.message;
    }
  }

  return 'Não foi possível concluir a ação.';
}
