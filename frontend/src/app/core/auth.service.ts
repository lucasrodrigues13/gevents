import { Injectable, signal } from '@angular/core';

const CHAVE = 'gevents.sessao';

export interface Sessao {
  token: string;
  email: string;
  papeis: string[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly sessao = signal<Sessao | null>(this.ler());

  autenticado(): boolean {
    return !!this.sessao()?.token;
  }

  ehAdmin(): boolean {
    return this.sessao()?.papeis.includes('Admin') ?? false;
  }

  email(): string {
    return this.sessao()?.email ?? '';
  }

  token(): string | null {
    return this.sessao()?.token ?? null;
  }

  guardar(sessao: Sessao): void {
    sessionStorage.setItem(CHAVE, JSON.stringify(sessao));
    this.sessao.set(sessao);
  }

  sair(): void {
    sessionStorage.removeItem(CHAVE);
    this.sessao.set(null);
  }

  private ler(): Sessao | null {
    const texto = sessionStorage.getItem(CHAVE);
    if (!texto) {
      return null;
    }

    try {
      const sessao = JSON.parse(texto) as Sessao;
      return sessao?.token ? sessao : null;
    } catch {
      return null;
    }
  }
}
