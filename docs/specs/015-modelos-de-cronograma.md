# 015. Modelos de cronograma

**Status: rascunho.** O fluxo está fora do Must de [mvp-scope.md](../mvp-scope.md). RF-054 e RF-070 são Observado. Esta spec não autoriza implementação.

## 1. Identificador e nome

SPEC-015. Modelos de cronograma.

## 2. Objetivo

Registrar a biblioteca de modelos de cronograma no que a leitura sustenta e deixar o restante pendente. Evento permanece na SPEC-002 e na SPEC-008. O cronograma do evento permanece em RF-070.

## 3. Requisitos relacionados

RF-054 nomeia a biblioteca de cronograma junto com briefing, roteiro, inspiração e mensagem. A nota diz que cada biblioteca foi vista no menu, não no editor. Status Observado.

RF-070 descreve o cronograma do evento: data, hora, responsável, recado a fornecedores, salvar como modelo, impressão e compartilhamento. A nota diz que os rótulos foram vistos e a grade não foi percorrida. Status Observado.

Não há RF Definido para título, descrição, criar, salvar ou excluir um modelo de cronograma. Onde falta RF, a regra fica não definida.

Modelo de evento é RF-053, já coberto pela SPEC-008.

## 4. Atores e permissões

RF-054 indica a assessoria como ator da biblioteca. Quem lista, cria, altera ou exclui um modelo de cronograma não está definido. Cliente, assistente e fornecedor não têm regra neste fluxo. A role `User` continua sem ser papel de negócio, RF-040.

## 5. Pré-condições

Conta autenticada, SPEC-001. A página vista é a biblioteca. Vínculo com um evento não foi condição desta leitura.

## 6. Fluxo principal

Leitura de `https://assessoriavip.com.br/modelos_cronograma`:

1. A biblioteca mostra itens.
2. Cada item mostra título e descrição.

Isso é o que apareceu na referência. Título e descrição ainda não são regra aprovada do produto.

## 7. Fluxos alternativos

Criar, salvar e excluir não foram acionados. O resultado de cada ação fica pendente.

Lista vazia, busca, filtro, ordenação, abrir o item e aplicar o modelo num evento não entraram nesta leitura. Ficam pendentes.

O rótulo "salvar como modelo" de RF-070 não foi executado. O efeito dele sobre esta biblioteca fica pendente.

## 8. Regras de negócio

- A existência da biblioteca acompanha RF-054, status Observado. O texto desse RF não descreve o miolo.
- Título e descrição foram vistos no item. Sem RF Definido, a regra do produto permanece não definida.
- Data, hora, responsável, recado a fornecedores, impressão e compartilhamento são rótulos de RF-070. A grade não foi percorrida. Esses rótulos não viram campos do modelo nesta spec.
- SPEC-002 e SPEC-008 permanecem como estão.

## 9. Validações

Pendentes. Obrigatoriedade, tamanho e formato de título e descrição não foram vistos.

## 10. Estados

Não definidos. Nenhum estado do modelo foi visto.

## 11. Dados necessários

No item visto há título e descrição. Se a descrição pode ficar vazia, se o título se repete e quais outros dados o modelo guarda permanecem não definidos.

## 12. Interface

Lista de itens com título e descrição. Formulário, botões e editor não foram abertos. O layout da referência não entra como regra.

## 13. Erros

Não definidos.

## 14. Critérios de aceite

Com status rascunho e com RF-054 e RF-070 em Observado, esta spec fica fora da implementação.

O que a leitura permite conferir na referência:

1. A biblioteca mostra itens.
2. O item visível traz título e descrição.
3. Criar, salvar e excluir permanecem sem resultado esperado.

## 15. Cenários de teste

Condição vista: biblioteca com itens. Resultado visto: cada item exibe título e descrição.

Criar, salvar e excluir ficam sem cenário. Cenário de produto só cabe depois de RF aprovado para a ação correspondente.

## 16. Dependências

SPEC-001, para a conta autenticada. RF-070 permanece dependência em aberto se o modelo passar a alimentar o cronograma do evento. Esse vínculo não foi visto.

## 17. Pendências

- Corte: fora do Must. RF-054 e RF-070 seguem Observado.
- Criar: pendente. A ação não foi acionada.
- Salvar: pendente. A ação não foi acionada.
- Excluir: pendente. A ação não foi acionada.
- Título e descrição como campos do produto: não definido. Falta RF.
- Obrigatoriedade, unicidade, tamanho, ordenação, busca e lista vazia: não definido.
- Conteúdo interno do modelo, inclusive horários, responsáveis e recados: não definido. A grade de RF-070 não foi percorrida.
- Quem mais acessa, isolamento entre contas e o efeito de excluir um modelo já usado num evento: não definido.
- Impressão e compartilhamento citados em RF-070 e RF-077 ficam fora deste fluxo.
