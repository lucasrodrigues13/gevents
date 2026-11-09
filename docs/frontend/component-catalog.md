# Catálogo de componentes

Cada item lista nome, responsabilidade, referência, variações, estados, regra, implementação, arquivo, acesso e exemplo. Componentes sem arquivo próprio são markup Bootstrap/AdminLTE repetido de propósito: não vale um partial só para um `input`.

## Estrutura

### Layout autenticado

Shell da aplicação: header, sidebar, conteúdo, rodapé. Referência: dashboard v4. Variação única, tema claro, sidebar escura. Estados: menu aberto ou recolhido em viewport menor que `lg`. Usar em toda página logada via `_ViewStart` ou `Layout = "_LayoutAutenticado"`. Arquivo: `Pages/Shared/_LayoutAutenticado.cshtml`. O `h1` é o título. Trilha: Início, seção opcional (`ViewData["Secao"]` e `ViewData["SecaoUrl"]`) e página atual.

### Layout de conta

Caixa central de login. Referência: exemplo de login v4. Arquivo: `Pages/Shared/_LayoutConta.cshtml`. Páginas em `Pages/Account`.

### Layout público

Barra simples e conteúdo. Referência: navbar Bootstrap. Arquivo: `Pages/Shared/_LayoutPublico.cshtml`. RSVP.

### Layout estreito

Cartão central para início e erro. Arquivo: `Pages/Shared/_Layout.cshtml`.

### Sidebar

Menu da conta. Referência: sidebar v4. Itens, ícone, URL e papel saem de `AdminLte:Menu`. Tag Helper `<adminlte-sidebar />`. Não duplicar o menu na página.

### Header

Navbar `app-header`. Botão do menu com `aria-label`. Conta em dropdown: Perfil e Sair por POST.

### Trilha

`ol.breadcrumb` no `app-content-header`. `nav` com `aria-label="Trilha"`.

### Page header

O bloco `app-content-header` já é o cabeçalho. Não criar segundo título igual.

### Footer

`app-footer` com o nome do produto.

### Card

Agrupa filtro, tabela, formulário ou item de convite. Referência: cards v4. Variações: header com ação, body, footer com contagem. Arquivo: markup nas páginas. Não usar card removível nem maximizável.

### Grade

`container-fluid` do layout e `row` / `col-*`. Sem container extra que quebre o alinhamento do AdminLTE.

## Formulários

### Texto, textarea, select, checkbox, radio, data, arquivo

Responsabilidade: editar um dado. Referência: forms/validation v4. Variações: somente leitura, desabilitado, obrigatório (`ge-obrigatorio`). Estados: vazio, preenchido, inválido, desabilitado. Regra: `asp-for`, label visível, sem `required` HTML. Implementação: Tag Helper nativo. Arquivo: a página ou o partial de campos. Acesso: label, `aria-required` no obrigatório, `aria-invalid` depois do erro. Exemplo: `Pages/Perfil/Index.cshtml`.

Data e hora não têm componente próprio. Upload não tem componente próprio.

### Validação

Resumo no topo e mensagem no campo. Referência: custom validation v4, ligada ao `ModelState`. Arquivos: `wwwroot/css/site.css`, `wwwroot/js/site.js`, `Pages/Shared/_ValidationScriptsPartial.cshtml`. O resumo some quando `.validation-summary-valid`.

### Formulário em etapas

Criação de evento: cards de modelo e depois o formulário. Não é um wizard genérico. Valor da assessoria é outra página, depois do salvar.

### Formulário condicional

Checkbox com `data-ge-toggle` aponta o `id` do bloco. Cliente na criação do evento e entrada no valor da assessoria.

### Grupos

`card` ou `fieldset` com legenda. Cliente: Dados pessoais, Redes, Endereço em `Pages/Clientes/_Campos.cshtml`. Evento: `Pages/Eventos/_Campos.cshtml`.

## Dados

### Tabela

Lista registros. Referência: tables/data v4. Variação: com ou sem ações. Estado vazio não usa `<table>` vazia. Ver [page-patterns.md](page-patterns.md).

### Paginação e ordenação

Mapeadas como necessidade. Sem implementação enquanto o serviço não devolver página e ordem. Situação na matriz: pendente.

### Filtro e busca

`form method="get"` no corpo do card. Botão `btn btn-outline-primary`.

### Badge

`Pages/Shared/_BadgeEvento.cshtml` e `_BadgeTarefa.cshtml`. Tag livre usa `badge text-bg-secondary` na página.

### Indicador

Small-box no dashboard. Info-box na home do evento. Referência: widgets da index v4. O número do small-box fica sozinho no `h3`, porque o teste e o tema leem esse valor.

### Card de resumo

Funil do dashboard: card com estágio, `Quantidade:` e `Valor:`.

### Estado vazio

Parágrafo `ge-vazio` com a frase da funcionalidade. Não trocar o texto que a spec ou o teste já fixou.

## Feedback

### Alerta

`<adminlte-alert theme="success|danger|warning|info">`. Referência: Tag Helper do pacote.

### Toast

Layout autenticado, `TempData["Aviso"]`. Bootstrap toast com classe `show`.

### Modal

`Pages/Shared/_ModalConfirmacao.cshtml` e o modal de desvincular na home. Model: `Project.Web.Ui.ModalConfirmacao`.

### Confirmação de exclusão

Não há exclusão de evento. Desvincular cliente usa o modal de perigo. Excluir registro, quando existir, usa o mesmo partial com `Perigo = true`.

### Confirmação de cancelamento

Modal "Cancelar evento" na home. Não é o botão Voltar do formulário.

### Carregamento

`site.js` marca o submit. Sem spinner de página.

### Sucesso e erro

Alerta na página ou `TempData`. Erro de validação no resumo.

## Navegação e ações

### Botões

Ver a tabela em [design-system.md](design-system.md). Não misturar `btn-link` com ação destrutiva.

### Menu de ações

Dropdown só da conta, no header. Linha de tabela usa botões `btn-sm`, não um menu diferente por tela.

### Dropdown

Bootstrap `dropdown` no header. Select de formulário não é dropdown de ação.

### Abas

`nav nav-tabs` com `data-bs-toggle="tab"` dentro de formulário só se o botão for `type="button"`. Âncora quando a seção continua visível, como no convite.

### Links

`asp-page`. Link de texto dentro de tabela para abrir o registro. Ação repetida vai no botão da coluna.

## Domínio de eventos

| Peça | Arquivo |
|---|---|
| Cards de modelo | `Pages/Eventos/Criar.cshtml` |
| Campos do evento | `Pages/Eventos/_Campos.cshtml` |
| Campos do cliente | `Pages/Clientes/_Campos.cshtml` |
| Home | `Pages/Eventos/Home.cshtml` |
| Quadro de tarefas | `Pages/Eventos/Tarefas/Index.cshtml`, colunas em card |
| Convite | `Pages/Eventos/Convites/Index.cshtml` |
| RSVP | `Pages/Convite/Index.cshtml` |
