import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API_BASE_URL } from './api-base-url';
import { Sessao } from './auth.service';

export interface MensagemResposta {
  mensagem: string;
  confirmacaoEnviada?: boolean;
}

export interface AreaResposta {
  email: string;
  papeis: string[];
}

@Injectable({ providedIn: 'root' })
export class ContaClient {
  private readonly http = inject(HttpClient);
  private readonly base = inject(API_BASE_URL);

  cadastrar(email: string, senha: string, confirmacao: string) {
    return this.http.post<MensagemResposta>(`${this.base}/api/v1/conta/cadastro`, { email, senha, confirmacao });
  }

  entrar(email: string, senha: string) {
    return this.http.post<Sessao>(`${this.base}/api/v1/conta/entrar`, { email, senha });
  }

  confirmar(usuarioId: string, token: string) {
    return this.http.post<MensagemResposta>(`${this.base}/api/v1/conta/confirmar-email`, { usuarioId, token });
  }

  reenviar(email: string) {
    return this.http.post<MensagemResposta>(`${this.base}/api/v1/conta/reenviar-confirmacao`, { email });
  }

  esqueciSenha(email: string) {
    return this.http.post<MensagemResposta>(`${this.base}/api/v1/conta/esqueci-senha`, { email });
  }

  redefinir(usuarioId: string, token: string, senha: string, confirmacao: string) {
    return this.http.post<MensagemResposta>(`${this.base}/api/v1/conta/redefinir-senha`, {
      usuarioId,
      token,
      senha,
      confirmacao,
    });
  }

  sair() {
    return this.http.post<void>(`${this.base}/api/v1/conta/sair`, null);
  }

  areaUsuario() {
    return this.http.get<AreaResposta>(`${this.base}/api/v1/conta/area`);
  }

  areaAdmin() {
    return this.http.get<AreaResposta>(`${this.base}/api/v1/admin/area`);
  }
}
