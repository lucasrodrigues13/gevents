# Auditoria de frontend

Data da leitura: 8 de novembro de 2026. Escopo: Razor Pages em `src/Project.Web`, estilos, scripts e o menu em `appsettings.json`.

## O que já estava certo

- .NET 10, Razor Pages, Tag Helpers e o pacote oficial AdminLTE 4 para ASP.NET Core.
- Layout autenticado com `app-wrapper`, sidebar por configuração e `data-lte-toggle="sidebar"`.
- Páginas de conta no `login-box` do AdminLTE.
- Alertas com `<adminlte-alert>`.
- Cookie, POST e antiforgery nas saídas, confirmações e desvínculos.
- Textos funcionais cobertos pelos testes de integração.

## Problemas

| Problema | Onde aparecia |
|---|---|
| Listagens em `<ul>` ou cards soltos, cada uma com outro espaçamento | Eventos, clientes, tarefas, modelos, grupos, convidados do convite |
| Formulário de evento inteiro antes da escolha do modelo | `Pages/Eventos/Criar.cshtml` |
| Edição de evento sem as quatro áreas da SPEC-008 | `Pages/Eventos/_Campos.cshtml` |
| Confirmação de concluir e cancelar como parágrafo e link | `Pages/Eventos/Home.cshtml` |
| Desvincular cliente por botão direto | Home do evento |
| `asp-validation-summary` com `text-danger`, sem o visual de alerta do Bootstrap | Formulários em geral |
| Campo inválido sem a borda `is-invalid` | Validação server-side e unobtrusive |
| jQuery apontando para `jquery.min.js` e `jquery.validate.min.js`, arquivos que não estão em `wwwroot` | `_Scripts` e `_ValidationScriptsPartial` |
| Scripts de validação não entravam no layout | Nenhuma página incluía o partial |
| Cabeçalho sem trilha | `_LayoutAutenticado` |
| Dashboard com cards genéricos, fora do small-box | `Pages/Dashboards/Index.cshtml` |
| Modelos e grupos existiam e não estavam no menu | `AdminLte:Menu` |
| Botão Sair repetido e fora do dropdown da conta | Layout, `/User`, `/Admin` |
| Vários `h1` na mesma tela autenticada | Home, convites, tarefas |
| Estado vazio era um parágrafo sem classe comum | Listagens |
| Filtros só com placeholder, sem label | Eventos e convites |
| Radios do RSVP sem `form-check` | `/Convite` |
| CSS próprio quase só na validação; o resto era classe solta | `wwwroot/css/site.css` |

Não havia modal, toast, DataTables, gráfico nem seletor de data de terceiros. Também não havia paginação nem ordenação no serviço: as listas já vinham filtradas do servidor, inteiras.

## Correções aplicadas

- Layout, trilha, menu da conta e toasts de `TempData`.
- Small-box no dashboard, tabela nas listagens simples, cards onde o item carrega formulário.
- Criação de evento começa pelos cards de modelo.
- Edição de evento em abas Informações gerais, Detalhes, Cerimônia e Recepção.
- Modal Bootstrap para concluir, cancelar e desvincular, sempre por POST.
- Validação visual ligada ao `ModelState` e ao jQuery Unobtrusive, com os arquivos que existem no repositório.
- Modelos e grupos no menu já configurado do AdminLTE.

## Fora deste corte

- Paginação e ordenação server-side. Exigiriam contrato novo em Application. A tela mostra a quantidade recebida e não pagina no navegador.
- Gráfico. A spec do dashboard pede cartões e funil, não série temporal.
- Seletor de data de biblioteca. O pacote não traz um. O campo é `input` nativo.
- Toasts disparados pelos handlers. O layout já lê `TempData["Aviso"]` e `TempData["Erro"]`; os fluxos atuais ainda devolvem a mensagem na própria página.
