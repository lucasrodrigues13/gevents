# Plano 001. Autenticação e conta

**Status: proposta.** Spec [001-autenticacao-e-conta.md](../specs/001-autenticacao-e-conta.md). Não autoriza código.

## Fora deste plano

Arquivo, evento, perfil, `SameSite` explícito e qualquer role além de `Admin` e `User`. O destino depois do login é uma página autenticada mínima. A SPEC-007 troca esse destino quando for implementada.

## Camadas

Não há caso de uso de cadastro na Application. As páginas de conta usam `UserManager`, `SignInManager` e `RoleManager`.

| Camada | O que entra |
|---|---|
| Domain | Nada. Conta não é entidade de domínio. |
| Application | Nada deste fluxo. |
| Infrastructure | `AppDbContext` com Identity, `ApplicationUser` só se o Identity exigir a classe, MailKit como `IEmailSender<ApplicationUser>`, bootstrap, chaves do Data Protection. |
| Web | Páginas de conta, cookie, rate limit, forwarded headers, composição no `Program.cs`. |
| Testes | Integração com Testcontainers e remetente falso. Unitário não se aplica a este fluxo. |

Sem MediatR, AutoMapper, repositório genérico, fila e `Database.Migrate()` na subida.

## Identity

- E-mail único normalizado. Confirmação obrigatória antes do login.
- Senha: mínimo 8, sem exigir dígito, maiúscula ou símbolo. `RequireLowercase` permanece ligado.
- Bloqueio: 5 falhas, 15 minutos.
- Cookie: 8 horas, renovação deslizante, HttpOnly, Secure em produção. Não gravar `SameSite=Lax` como decisão. DA-019 continua proposta.
- Dois provedores de token do Identity: confirmação 24 horas, redefinição 1 hora.
- Cadastro público só adiciona `User`.
- CSRF nativo. Limites em memória, por IP, configuráveis: cadastro 5/hora, login 10/minuto, redefinição 3/hora, reenvio 3/hora. Resposta 429 com `Retry-After`.
- IP do cliente só a partir do proxy configurado.

## Bootstrap

Serviço interno na subida, desligado por padrão. Variáveis `Bootstrap__Enabled`, `Bootstrap__AdminEmail` e `Bootstrap__AdminPassword`. Cria as duas roles. Cria o admin se o e-mail não existir, confirma o e-mail e não troca senha existente. Se o e-mail já for `User`, a subida falha e não promove. Sem endpoint, sem `HasData()`, sem senha no código. Segredos não vão para log.

## Páginas

| Página | Quem |
|---|---|
| Cadastro, login, confirmar e-mail, reenviar, esqueci a senha, redefinir | Visitante |
| Sair | Autenticado |
| Área do `User` | `User` |
| Área do `Admin` | `Admin` |

`User` recebe 403 na área de `Admin`. Mensagens em português. Falha de login e de redefinição não revela se o e-mail existe. Falha de SMTP no cadastro avisa e não confirma o e-mail.

## Persistência

Primeira migration: tabelas do Identity e das roles. Sem tabela de domínio.

## Testes de integração

Cadastro válido, e-mail duplicado, login certo e errado, confirmação válida e inválida, redefinição válida e inválida, bloqueio por 15 minutos, `User` barrado, `Admin` autorizado, bootstrap idempotente, reenvio, 429, SMTP falso falhando no cadastro, token de 24 horas e token de 1 hora. E-mail real não é enviado.

## Tarefas

1. Criar a solution com os quatro projetos e os dois de teste, nas referências já definidas.
2. Configurar Identity, cookie, rate limit, forwarded headers e páginas de conta.
3. Implementar o remetente MailKit e o remetente falso dos testes.
4. Implementar o bootstrap.
5. Gerar a migration do Identity, sem aplicá-la na subida.
6. Cobrir os testes da spec.

## Pendência que permanece

Segundo campo de confirmação de senha não é obrigatório. E-mail que foi `Admin` e perdeu a role continua resolução manual.
