---
name: backend
description: Implementa ou ajusta a API .NET do gevents. Use ao criar ou alterar endpoint, caso de uso, DbContext, migration, JWT, Identity na API ou teste de backend.
---

# Backend

Use esta skill ao implementar ou alterar a API.

## Antes de codar

1. Exija spec aprovada. Se a spec estiver em rascunho ou não existir, pare e diga qual. Não peça plano técnico.
2. Leia `.cursor/rules/project-architecture.mdc`, `.cursor/rules/coding-standards.mdc` e os RF e DA citados na spec, em `docs/requirements.md`.
3. A spec decide o comportamento. Esta skill só diz onde o código fica. Se os quatro projetos da seção 6 ainda não existirem, crie-os ao implementar a primeira spec que precisar deles.

## Onde cada coisa fica

Os quatro projetos e a solution ficam em `src/backend/`. A solution é `src/backend/gevents.slnx` e não inclui o Angular. Os testes ficam em `tests/` e entram nessa solution.

- **Domain.** Entidades, enums e invariantes. Sem EF Core, Identity, ASP.NET Core nem JWT.
- **Application.** Casos de uso, DTOs e interfaces de recurso externo, inclusive a de assinar o JWT. Sem `DbContext` e sem tipo de HTTP.
- **Infrastructure.** EF Core, Npgsql, migrations, `IdentityDbContext`, assinatura do JWT, MailKit, arquivo no volume e bootstrap do administrador.
- **Api.** Endpoints em `/api/v1`, composição no `Program.cs`, CORS, rate limiting e tradução de erro HTTP. O endpoint chama a Application e devolve o status.

## Endpoint

- Endpoint de negócio usa o serviço da Application. Não usa `DbContext`, repositório concreto nem SQL.
- Endpoint de conta pode usar `UserManager`, `SignInManager.CheckPasswordSignIn` e `RoleManager` quando a operação for do Identity. Não crie serviço cujo único trabalho seja repetir essas APIs.
- A senha, a normalização do e-mail, o lockout, as roles e os tokens de confirmação e de redefinição continuam no Identity.
- O JWT de acesso dura 8 horas, sem renovação, sem refresh e sem lista de revogação. Segredo, issuer, audience e prazo vêm de configuração. A assinatura fica na Infrastructure.
- O logout não invalida o token no servidor.
- Roles no token: só `Admin` e `User`.

## Erro

A resposta de erro segue a DA-003: JSON com código e mensagem, status HTTP correspondente, sem stack trace em produção. Logue o erro com contexto. Senha, token, connection string, senha SMTP e chave do Data Protection ficam fora do log.

## Persistência

- Migration explícita no projeto Infrastructure. Produção não chama `Database.Migrate()` nem `EnsureCreated` na subida.
- Repositório só quando isolar a consulta de um agregado deixar o caso de uso mais claro. Sem repositório genérico e sem repositório de usuário ou role.
- Arquivo vai para o volume configurado. O Caddy não publica esse diretório.

## Configuração

- CORS libera só a origem local do `ng serve`. Em produção a origem é a do Caddy.
- Swagger é configurável e fica ligado em desenvolvimento.
- Segredo fica em User Secrets ou variável de ambiente. Não commite senha, connection string com credencial nem segredo do JWT.

## Testes

- Unitário em `Project.Tests`, sem banco, sem rede e sem SMTP real.
- Integração em `Project.IntegrationTests`, com `WebApplicationFactory`, PostgreSQL do Testcontainers e e-mail falso.
- Cubra os critérios de aceite e os cenários da spec. Rode esses testes e corrija a falha antes de parar.

## O que não fazer

- Não acrescente rota, seed de demonstração nem papel que a spec não descreve.
- Não traga MySQL, quatro projetos de teste, empréstimo nem migration na subida de outro repositório.
- Não adicione refresh token, lista de revogação, OAuth próprio, login social, MediatR, AutoMapper, fila nem Redis.
- Não altere RF aprovado para acomodar o código.
- Não rode migration em produção nem faça deploy.
