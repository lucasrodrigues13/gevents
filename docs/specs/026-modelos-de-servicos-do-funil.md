# 026. Modelos de serviços do funil

**Status: rascunho.** Código só depois de aprovação. Fora do corte do MVP. RF-075 e RF-077 continuam Observado.

## 1. Identificador e nome

SPEC-026. Modelos de serviços do funil.

## 2. Objetivo

A assessoria consulta o catálogo de modelos de serviço do funil de vendas. A lista de serviços de fornecedor, em `/suppliers/services`, é a SPEC-018 e fica de fora deste arquivo.

## 3. Requisitos relacionados

RF-075 e RF-077, os dois Observado. RF-075 descreve o quadro do funil, os quatro estágios vistos, descarte, filtro, exportação, formulário público, checagem de data, contratos, orçamentos e tarefas comerciais. RF-077 agrupa catálogos de funil com relatórios e impressões. Nenhum dos dois descreve esta lista nem campo de modelo de serviço.

O corte aprovado não inclui este catálogo. De RF-075 entra só o que RF-082 usa no dashboard. RF-077 está fora do corte. Esta spec não promove esses RF a Definido e não altera RF-082.

Não há RF para a lista, o estado vazio, criar, editar ou os campos. Sem RF, o que não foi visto permanece não definido.

## 4. Atores e permissões

A lista vazia foi vista na conta de referência, pela assessoria. Visitante, lead, cliente, fornecedor e assistente não foram exercitados nesta rota. Quem mais abre, cria ou edita permanece não definido. A role `User` não é papel de negócio, RF-040.

## 5. Pré-condições

A tela foi aberta na conta de referência já em uso. Não foi isolado se a rota exige autenticação; essa pré-condição permanece não definida. A lista vazia apareceu sem oportunidade, orçamento ou contrato cadastrados nesta visita.

## 6. Fluxo principal

1. A conta abre o catálogo de modelos de serviço do funil.
2. A lista é exibida.
3. Na visita, a lista não tinha itens. Esse é o estado vazio visto.

Criar e editar não foram acionados. O que vem depois da lista vazia permanece pendente.

## 7. Fluxos alternativos

- Lista com modelo: não visto. Não definido.
- Acionar criar: não feito. Rótulo, formulário e resultado não definidos.
- Acionar editar: não feito. Não definido.
- Excluir, duplicar, buscar, filtrar, ordenar e paginar: não vistos. Não definidos.

## 8. Regras de negócio

Não há regra fechada para este catálogo. Sem RF, esta spec não decide.

- O modelo de serviço do funil e o serviço de fornecedor da SPEC-018 são catálogos distintos. A relação entre eles, se houver, não foi vista e permanece não definida.
- A lista vazia foi o único estado visto. Não há RF para o conteúdo da lista quando existe modelo, nem para o que outra conta enxerga.

## 9. Validações

Nenhuma foi vista. O formulário não abriu. Obrigatório, tamanho, formato e duplicidade permanecem não definidos.

## 10. Estados

Estado visto: lista vazia. Outro estado do modelo ou da lista permanece não definido.

## 11. Dados necessários

Não definidos. A lista vazia não mostrou coluna nem campo. Nome e qualquer outro dado do serviço ficam de fora até serem vistos.

## 12. Interface

Rota vista na referência: `/funnel/models/service`. O que apareceu foi a lista sem itens. Texto do estado vazio, botões, colunas e formulário não foram registrados.

## 13. Erros

Nenhum erro foi visto. Mensagem de falha, de permissão e de validação permanece não definida. A lista sem itens foi o estado vazio.

## 14. Critérios de aceite

1. Abrir o catálogo na condição vista mostra a lista vazia.
2. A lista sem itens permanece estado vazio.
3. O aceite cobre essa lista. Criar, editar e os campos do serviço ficam pendentes.
4. O catálogo desta spec e o de `/suppliers/services` (SPEC-018) são listas distintas.

## 15. Cenários de teste

Conta de referência abre `/funnel/models/service` sem modelos. Resultado: lista vazia, como estado vazio. Criar e editar ficam fora do cenário.

## 16. Dependências

SPEC-001, se a rota for página autenticada; isso ainda não foi isolado. SPEC-007 mostra quantidade e valor do funil e não cria este catálogo. SPEC-018 é o outro catálogo. Oportunidade, orçamento, contrato e briefing do funil ficam fora deste arquivo.

## 17. Premissas e pontos não definidos

- O fluxo não está no Must. RF-075 e RF-077 são Observado e não aprovam esta spec.
- Campos do modelo de serviço: não definidos.
- Criar, editar e o formulário: pendentes. Não foram acionados.
- Excluir, duplicar, busca, filtro, ordenação, paginação e impressão: não vistos.
- Uso do modelo numa oportunidade, num orçamento ou num contrato: não visto.
- Texto exato do estado vazio: não registrado.
- Isolamento entre contas: não exercido, porque não havia item.
- Ligação deste modelo com a checagem de data, o descarte ou o formulário público de RF-075: não vista.
- Relatório ou impressão a partir desta lista, no âmbito de RF-077: não visto.
