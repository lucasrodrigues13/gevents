# Padrões de página

## Listagem

Referência: [tabelas do AdminLTE 4](https://adminlte.io/themes/v4/tables/data.html). Sem DataTables.

Estrutura:

1. Título no `h1` do layout e trilha.
2. Card.
3. Abas no `card-header`, quando a spec tiver abas.
4. Botão primário de cadastro.
5. Filtro em `method="get"`, com `label` visível.
6. `table-responsive` > `table table-striped table-hover align-middle`.
7. Badge de status.
8. Coluna Ações com `ge-acoes`, alinhada à direita.
9. Rodapé com a quantidade devolvida pela consulta.
10. Vazio: parágrafo com a frase já usada pela funcionalidade e classe `ge-vazio`.
11. Erro: `<adminlte-alert theme="danger">`.

A consulta continua no serviço. Não há paginação nem ordenação na Application; a tela não simula as duas coisas no navegador. Quando o contrato ganhar página e ordem, a tabela recebe os controles no mesmo card, ainda por GET.

Convite com formulário próprio permanece card. A visão por convidado é tabela.

## Formulário

Referência: [validação do AdminLTE 4](https://adminlte.io/themes/v4/forms/validation.html), no exemplo de validação customizada, adaptada ao Razor Pages.

- `label` visível, `form-control` ou `form-select`.
- Grupo de campos num `card` ou `fieldset`.
- `asp-validation-summary="ModelOnly"` com `role="alert"`. O CSS de `.validation-summary-errors` segue o alerta de perigo.
- `asp-validation-for` com `invalid-feedback`.
- `.input-validation-error` ganha borda de inválido. `site.js` acrescenta `is-invalid` e `aria-invalid`.
- Valor reexibido pelo binding. Não limpar o campo depois do erro.
- Botões no fim do card: primário e, quando existe destino, `btn btn-outline-secondary`.
- Grade `row` para caber em tela estreita.
- Validação no servidor permanece a fonte. O unobtrusive só antecipa o que já está anotado no input model.

Data e hora: `input` nativo (`date`, `time` ou o tipo que o Tag Helper gera para `DateOnly` e `TimeOnly`). O pacote não inclui seletor.

Arquivo: `input type="file"` com `form-control` e `accept` do requisito. Sem plugin de upload.

Formulário condicional: o bloco extra fica no HTML e some com `d-none` enquanto o checkbox `data-ge-toggle` estiver desligado. O POST continua valendo no servidor.

## Criação de evento

1. A página mostra os modelos em cards (`ge-modelo-card`).
2. O clique escolhe o modelo por GET `?modelo=`.
3. O callout identifica o modelo. O tipo vai oculto, porque o modelo o define.
4. O restante do formulário só aparece depois dessa escolha, ou de novo se o POST voltar com erro.
5. "Trocar modelo" retorna aos cards.
6. "Cadastrar cliente" abre os campos da SPEC-009.

## Edição de evento

Quatro abas Bootstrap, dentro do mesmo formulário: Informações gerais, Detalhes, Cerimônia, Recepção. O botão da aba é `type="button"` para não enviar o formulário.

## Modal de confirmação

Base: modal do Bootstrap 5, partial `Pages/Shared/_ModalConfirmacao.cshtml`.

Conteúdo: título, consequência, nome do registro quando existe, Voltar e botão de confirmação. Cancelar evento e desvincular usam `btn-danger`. Concluir usa `btn-primary`.

O formulário é POST, com antiforgery e o campo que o handler já espera (`confirmado=sim` ou `clienteId`). Não há confirmação por GET. `?confirmar=` só abre o modal já existente, para o mesmo POST de antes.

`site.js` desabilita o botão no processamento. Erro de regra continua no `ModelState` da página.

## Notificação

Mensagem que precisa continuar na tela: `<adminlte-alert>` com `success`, `danger`, `warning` ou `info`.

Depois de redirect: `TempData["Aviso"]` vira toast Bootstrap no layout autenticado; `TempData["Erro"]` vira alerta. Não usar `alert()` do navegador.

## Estados

| Estado | Quando |
|---|---|
| Vazio | A consulta voltou lista vazia. A frase funcional permanece. |
| Erro | Falha de validação, regra ou carga. |
| Sucesso | Alert ou toast. |
| Desabilitado | Submit em andamento, ou campo somente leitura. |
| Carregando | Só no botão que está em POST. A página Razor não usa spinner em GET. |

## Acessibilidade

Label ligada ao campo. Trilha com `aria-current="page"`. Modal com `aria-labelledby`, botão Fechar e foco tratado pelo Bootstrap. Badge não é a única informação: o texto da situação está dentro dele. Contraste é o do tema Bootstrap nos pares `text-bg-*` e `btn-*`.
