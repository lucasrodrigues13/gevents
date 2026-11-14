export type TipoMascara = 'cpf' | 'cnpj' | 'celular' | 'telefone' | 'cep' | 'documento';

export function soDigitos(valor: string): string {
  return valor.replace(/\D/g, '');
}

export function aplicarMascara(valor: string, tipo: TipoMascara): string {
  const digitos = soDigitos(valor);

  switch (tipo) {
    case 'cpf':
      return mascarar(digitos.slice(0, 11), [3, 6, 9], ['.', '.', '-']);
    case 'cnpj':
      return mascarar(digitos.slice(0, 14), [2, 5, 8, 12], ['.', '.', '/', '-']);
    case 'celular':
      return mascararTelefone(digitos.slice(0, 11), true);
    case 'telefone':
      return mascararTelefone(digitos.slice(0, 10), false);
    case 'cep':
      return mascarar(digitos.slice(0, 8), [5], ['-']);
    case 'documento':
      return digitos.length > 11
        ? aplicarMascara(digitos, 'cnpj')
        : aplicarMascara(digitos, 'cpf');
    default:
      return valor;
  }
}

function mascarar(digitos: string, cortes: number[], separadores: string[]): string {
  let resultado = '';
  let cursor = 0;

  for (let i = 0; i < digitos.length; i++) {
    if (cursor < cortes.length && i === cortes[cursor]) {
      resultado += separadores[cursor];
      cursor += 1;
    }
    resultado += digitos[i];
  }

  return resultado;
}

function mascararTelefone(digitos: string, celular: boolean): string {
  if (!digitos) {
    return '';
  }

  if (digitos.length <= 2) {
    return `(${digitos}`;
  }

  const ddd = digitos.slice(0, 2);
  const resto = digitos.slice(2);

  if (!celular) {
    if (resto.length <= 4) {
      return `(${ddd}) ${resto}`;
    }
    return `(${ddd}) ${resto.slice(0, 4)}-${resto.slice(4)}`;
  }

  if (resto.length <= 5) {
    return `(${ddd}) ${resto}`;
  }

  return `(${ddd}) ${resto.slice(0, 5)}-${resto.slice(5)}`;
}
