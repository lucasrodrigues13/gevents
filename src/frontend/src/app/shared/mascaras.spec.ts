import { aplicarMascara } from './mascaras';

describe('aplicarMascara', () => {
  it('formata CPF', () => {
    expect(aplicarMascara('52998224725', 'cpf')).toBe('529.982.247-25');
  });

  it('formata celular', () => {
    expect(aplicarMascara('11987654321', 'celular')).toBe('(11) 98765-4321');
  });

  it('formata telefone fixo', () => {
    expect(aplicarMascara('1133334444', 'telefone')).toBe('(11) 3333-4444');
  });
});
