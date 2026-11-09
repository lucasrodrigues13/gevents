# Referências AdminLTE 4

Pacote em uso: `ColorlibHQ.AdminLTE.AspNetCore` 0.3.1. Documentação do pacote: <https://docs.adminlte.io/aspnet/introduction>. Demonstração do tema: <https://adminlte.io/themes/v4/>.

## O que a demonstração tem e o pacote acompanha

| Necessidade | Referência | No gevents |
|---|---|---|
| Layout, sidebar, header, footer | <https://adminlte.io/themes/v4/index.html> | `_LayoutAutenticado.cshtml`, `<adminlte-sidebar />` |
| Dashboard | a mesma página, small-box e info-box | `Pages/Dashboards/Index.cshtml` e a home do evento |
| Tabela | <https://adminlte.io/themes/v4/tables/data.html> | Card + tabela Bootstrap. Sem DataTables |
| Formulário e validação | <https://adminlte.io/themes/v4/forms/validation.html> | Tag Helpers + CSS de `.validation-summary-errors` e `.input-validation-error` |
| Login | <https://adminlte.io/themes/v4/examples/login.html> | `_LayoutConta.cshtml`, classes `login-page` e `login-box` |
| Alerta | Tag Helper do pacote e alerta Bootstrap | `<adminlte-alert>` |
| Callout | Tag Helper do pacote | Modelo selecionado na criação do evento |
| Modal | Bootstrap 5, documentado junto do AdminLTE 4 | `_ModalConfirmacao.cshtml` |
| Badge | Bootstrap 5 | `_BadgeEvento.cshtml`, `_BadgeTarefa.cshtml` |
| Toast | Bootstrap 5 | `TempData["Aviso"]` no layout autenticado |
| Abas | Bootstrap 5 / AdminLTE | Edição de evento e visões de convite |
| Ícones | Bootstrap Icons no pacote | `bi bi-*` |

## O que a demonstração mostra e este corte não copia

| Item da demonstração | Substituição |
|---|---|
| DataTables e botões de exportação | Tabela Bootstrap. A ordenação e a página ficam para quando o serviço as oferecer |
| Chart.js no dashboard de exemplo | Small-box e cards. A spec pede contagem e funil, não gráfico |
| Tempus Dominus | `input` de data e hora nativo. O pacote não distribui o seletor |
| jsGrid, FullCalendar, mapa, caixa de entrada | Sem requisito correspondente |
| AdminLTE 3 (`adminlte.io/themes/v3`) | Não usar classe, plugin nem exemplo dessa versão |

Os caminhos exatos de exemplos secundários (upload, calendário, 404 temático) mudam no site. Quando a URL não existir, vale o componente Bootstrap 5.3 já carregado pelo pacote, registrado neste arquivo.
