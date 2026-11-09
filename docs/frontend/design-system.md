# Design system

## Decisão de integração

Fica o pacote `ColorlibHQ.AdminLTE.AspNetCore` 0.3.1, já referenciado em `Project.Web`.

Motivo: é a integração oficial do AdminLTE 4 para Razor Pages no .NET 10. Entrega Bootstrap 5.3.8, Bootstrap Icons, OverlayScrollbars, o CSS e o JS do AdminLTE 4, e Tag Helpers (`adminlte-styles`, `adminlte-scripts`, `adminlte-sidebar`, `adminlte-alert`, `adminlte-callout`). O menu sai de `appsettings.json`. Não pede npm nem jQuery para o tema.

A cópia manual dos arquivos do repositório ColorlibHQ/AdminLTE foi descartada. Duplicaria asset, versão e o menu que o Tag Helper já resolve. Os componentes Blazor do mesmo pacote não entram: a UI é Razor Pages.

jQuery permanece só para `jquery.validate` e `jquery.validate.unobtrusive`. O AdminLTE 4 não depende dele. Interação visual nova vai em `wwwroot/js/site.js`, sem `alert()` e sem biblioteca extra.

## Tokens

Não criar cor, raio ou sombra fora das variáveis do Bootstrap e do AdminLTE. O pacote define, entre outras:

| Token | Papel |
|---|---|
| `--bs-primary` | Ação principal. No pacote, `#0d6efd` |
| `--bs-secondary` | Ação secundária e texto de apoio |
| `--bs-success` | Sucesso e concluído |
| `--bs-warning` | Atenção |
| `--bs-danger` | Erro, cancelado e ação destrutiva |
| `--bs-info` | Informação |
| `--bs-body-font-family`, `--bs-body-font-size` | Texto |
| `--bs-border-radius` | Cantos, 0.375 rem no AdminLTE 4 |
| `--bs-box-shadow-sm` | Elevação leve. Card, small-box e info-box já trazem a sombra do tema |
| `--bs-focus-ring-color` | Foco |

Tema claro (`data-bs-theme="light"`). O seletor de modo está desligado em `AdminLte:ColorModeToggle`.

### Tipografia

Um `h1` por tela, no cabeçalho do layout autenticado, com classe `h3`. Título de card é `h2.h5`. Título de bloco interno é `h3.h6` ou `h5`. Não repetir o mesmo título no corpo.

### Espaçamento e densidade

Formulário: `mb-3` entre campos, `row` / `g-2` ou `g-3` na grade. Tabela: `table table-striped table-hover align-middle`. Botão de tabela: `btn btn-sm`. Input e select usam a altura padrão do Bootstrap (`form-control` / `form-select`). Não fixar altura em pixel.

### Breakpoints

Os do Bootstrap 5: sm 576, md 768, lg 992, xl 1200, xxl 1400. Sidebar `sidebar-expand-lg`. Filtro e formulário empilham abaixo de `md`.

### Estados

Hover, focus, active e disabled são os do Bootstrap. Foco visível não é removido. Botão de envio recebe `aria-busy` e fica desabilitado no envio seguinte para não postar duas vezes. O primeiro envio não é cancelado.

### Ícones

Bootstrap Icons, classe `bi bi-*`, como no menu. Ícone decorativo leva `aria-hidden="true"`. Botão só de ícone leva `aria-label`.

### Botões

| Uso | Classe |
|---|---|
| Ação principal da tela ou do formulário | `btn btn-primary` |
| Ação secundária, voltar, filtrar | `btn btn-outline-secondary` ou `btn btn-outline-primary` |
| Destrutiva | `btn btn-danger` ou `btn btn-outline-danger` |
| Ação de linha | `btn btn-sm` na coluna `ge-acoes` |

### Campos obrigatórios

Label com `ge-obrigatorio` e o campo com `aria-required="true"`. O asterisco usa `--bs-danger`. Isso não substitui a validação do servidor. Não usar o atributo HTML `required` para não bloquear o POST antes do `ModelState`.

### Status

| Situação do evento | Badge |
|---|---|
| Em andamento | `text-bg-primary` |
| Concluído | `text-bg-success` |
| Cancelado | `text-bg-danger` |

Tarefa: pendente `secondary`, em andamento `primary`, concluída `success`, atrasada `warning`. Tag livre: `text-bg-secondary`.

## O que não entra

DataTables, chart.js, Tempus Dominus, Select2, SweetAlert e qualquer CSS com cor hexadecimal nova. Paginação no navegador sobre a lista inteira também não.
