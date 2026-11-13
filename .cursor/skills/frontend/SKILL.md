---
name: frontend
description: Implementa ou ajusta a interface Angular do gevents com Angular Material. Use ao criar ou alterar tela, layout, formulário, tabela, diálogo, alerta, menu, guarda de rota ou cliente HTTP.
---

# Frontend Angular

Use esta skill ao implementar ou alterar a interface.

## Antes de codar

1. Exija spec aprovada. Se a spec estiver em rascunho ou não existir, pare e diga qual. Não peça plano técnico.
2. Leia a DA-044 em `docs/requirements.md` e `.cursor/rules/frontend.mdc`.
3. Não invente tela, gráfico, papel ou fluxo que a spec não descreve.

## Base

A interface é uma aplicação Angular em `src/frontend/`, com Angular Material.

Se `src/frontend/` ainda não tiver a aplicação, crie com o Angular CLI e adicione o Angular Material pelo schematic oficial. O visual não é o tema padrão do schematic. A fonte de estilo e de layout é `docs/design-system.md`, extraída da demo pública https://apex-angular.dashboardpack.com. O código desse template não entra no repositório.

O build publicado é estático. O Caddy serve esse build e encaminha `/api`. Sem SSR e sem service worker. A tela chama a API real.

## Visual

Siga `docs/design-system.md` em toda tela nova ou alterada.

- Cores iguais às da demo, com modo claro e modo escuro. A classe `escuro` no `html` liga o escuro. A escolha fica em `localStorage` (`gevents-tema`).
- Casca: barra lateral escura de 260px e barra superior de 64px. Item ativo em verde.
- Cabeçalho de lista: título, linha de apoio e botão verde à direita, no padrão de New Order.
- Tabela dentro de `.gd-painel`, com abas em pílula e busca quando a tela já tem esses filtros. A última coluna é Ações, com Editar e Excluir. Excluir só chama a API quando o fluxo já tem remoção.
- Novo e Editar de evento, cliente e tarefa são `MatDialog` (`gd-dialog`) sobre a tela aberta. Novo convite abre painel modal sobre a lista. Não navegue para uma página só para esses formulários.
- Cartões de número do painel usam `.gd-numero`. Sem gráfico enquanto um requisito não pedir a série.
- Formulário, tabela, diálogo, botão, campo e aviso continuam no Angular Material. O tema do Material aponta para os tokens `--gd-*`.

## Stack

Use:

- Angular 22 e componentes standalone
- TypeScript
- Angular Material
- `HttpClient`
- guardas funcionais
- o test runner que o Angular CLI gerar

Evite:

- NgRx, Akita ou outra store global
- segunda biblioteca de componentes, Tailwind como base visual, Bootstrap ou pacote NuGet de tema
- cliente OpenAPI gerado, salvo pedido explícito do usuário
- SSR e PWA como deploy do gevents

## Estrutura esperada

```text
src/frontend/
  src/app/
    core/          auth, interceptor, config, clientes HTTP
    layout/        casca com mat-sidenav e mat-toolbar
    pages/         uma pasta por fluxo da spec, com loadComponent
    shared/        componente próprio, só quando o Material não bastar
```

Formulário, tabela, diálogo, botão, campo e aviso saem do Angular Material. Um componente em `shared/` pode se inspirar no visual de outro template. O código desse template não entra no repositório.

## Configuração

- A URL da API fica num arquivo de ambiente. Componente de funcionalidade não grava host fixo.
- Em desenvolvimento, o `ng serve` fala com a API. Em produção, o Caddy publica o build e encaminha `/api`.
- Não commite segredo. A URL da API fica no arquivo de ambiente.

## Autenticação na tela

- `AuthService` guarda o usuário atual e o JWT em `sessionStorage`.
- O interceptor envia `Authorization: Bearer`.
- Visitante sem token vai para o login.
- `User` não abre rota de `Admin`.
- Login, cadastro, confirmação de e-mail, reenvio, esqueci a senha e redefinição seguem a SPEC-001. Não crie 2FA, lock screen nem botão de Google ou GitHub.
- O logout só descarta o token local. A tela não promete que um token copiado deixou de valer.

## Estado

Estado fica no componente da tela ou num serviço pequeno. Não introduza store global.

## Testes

Quando a spec descrever a tela, cubra o que os critérios de aceite pedirem. O mínimo útil é:

- guarda redireciona quem não está autenticado
- guarda de `Admin` bloqueia `User`
- leitura do erro JSON da API
- URL da API configurável, se der para provar sem fragilidade

Rode `ng test` e `ng build`. Corrija a falha antes de parar.

## O que não fazer

- Não troque o Angular Material por outro framework de UI.
- Não reponha Razor Pages, Tag Helper ou antiforgery de formulário.
- Não altere RF aprovado para acomodar a tela.
