/** Converte `yyyy-MM-dd` (API) em `Date` local sem deslocar o dia. */
export function deIsoParaData(iso: string | null | undefined): Date | null {
  if (!iso) {
    return null;
  }

  const trecho = iso.slice(0, 10);
  const partes = trecho.match(/^(\d{4})-(\d{2})-(\d{2})$/);
  if (!partes) {
    return null;
  }

  const ano = Number(partes[1]);
  const mes = Number(partes[2]) - 1;
  const dia = Number(partes[3]);
  const data = new Date(ano, mes, dia);
  return Number.isNaN(data.getTime()) ? null : data;
}

/** Converte `Date` local em `yyyy-MM-dd` para a API. */
export function deDataParaIso(data: Date | null | undefined): string | null {
  if (!data || Number.isNaN(data.getTime())) {
    return null;
  }

  const ano = data.getFullYear();
  const mes = String(data.getMonth() + 1).padStart(2, '0');
  const dia = String(data.getDate()).padStart(2, '0');
  return `${ano}-${mes}-${dia}`;
}

/** Exibe data ISO ou Date como `dd/MM/yyyy`. */
export function formatarDataBr(valor: string | Date | null | undefined): string {
  if (!valor) {
    return '';
  }

  const data = valor instanceof Date ? valor : deIsoParaData(valor);
  if (!data) {
    return typeof valor === 'string' ? valor : '';
  }

  const dia = String(data.getDate()).padStart(2, '0');
  const mes = String(data.getMonth() + 1).padStart(2, '0');
  const ano = data.getFullYear();
  return `${dia}/${mes}/${ano}`;
}

/** Interpreta texto `dd/MM/yyyy` digitado no campo. */
export function parseDataBr(texto: string): Date | null {
  const limpo = texto.trim();
  const partes = limpo.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
  if (!partes) {
    return null;
  }

  const dia = Number(partes[1]);
  const mes = Number(partes[2]) - 1;
  const ano = Number(partes[3]);
  const data = new Date(ano, mes, dia);
  if (data.getFullYear() !== ano || data.getMonth() !== mes || data.getDate() !== dia) {
    return null;
  }

  return data;
}
