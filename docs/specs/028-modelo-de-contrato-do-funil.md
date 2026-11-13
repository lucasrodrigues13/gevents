# 028. Modelo de contrato do funil

**Status: rascunho.**

## 1. Identificador e nome

SPEC-028. Modelo de contrato do funil.

## 2. Objetivo

Registrar a biblioteca de modelos de contrato do funil. A lista de contratos permanece na SPEC-022. Nesta visita a lista estava vazia. Criar e editar não foram acionados. O fluxo está fora do Must aprovado em [mvp-scope.md](../mvp-scope.md).

## 3. Requisitos relacionados

RF-075 cita contratos no funil e está Observado. Não descreve biblioteca de modelo, campo nem cláusula, e não aprova esta spec.

RF-077 cita catálogos de funil e impressões e está Observado. Não descreve esta biblioteca e não aprova esta spec.

Não há RF Definido para modelo de contrato do funil. Regra sem RF fica não definida.

## 4. Atores e permissões

A lista vazia foi vista na conta da referência. Qual role deste produto abre a biblioteca não está definido. Lead não aparece nesta tela. `User` continua sem ser papel de negócio, RF-040.

## 5. Pré-condições

A visita ocorreu com a conta da referência já autenticada. A lista vazia não exigiu evento, oportunidade nem contrato da SPEC-022. Vínculo com oportunidade não foi visto.

## 6. Fluxo principal

1. A conta abre a biblioteca de modelos de contrato do funil.
2. Sem modelo, a lista aparece vazia.

Fonte desta visita: `https://assessoriavip.com.br/funnel/models/contract`.

## 7. Fluxos alternativos

- Criar modelo: não acionado. Não definido.
- Editar modelo: não acionado. Não definido.
- Lista com um ou mais modelos: não vista. Não definido.

## 8. Regras de negócio

- O que esta spec cobre é a biblioteca de modelos. A lista de contratos é a SPEC-022.
- Nenhuma cláusula foi vista. Texto, variável e estrutura de cláusula ficam não definidos.
- Campo, obrigatoriedade, exclusão, cópia, ordem, impressão e uso do modelo numa oportunidade ficam não definidos. Não há RF para essas regras.
- RF-075 e RF-077 permanecem Observados. Nenhuma regra deles entra aqui como confirmada.

## 9. Validações

Nenhuma validação foi vista. Não definido.

## 10. Estados

Estado visto: lista vazia. Modelo gravado, modelo em edição e lista com itens não foram vistos. Não definido.

## 11. Dados necessários

Nenhum campo foi aberto. Nome, texto e cláusulas do modelo não foram vistos. Não definido.

## 12. Interface

A página vista mostrou a lista vazia. Rótulo, coluna, botão e formulário de criar ou editar não foram registrados. Não definido.

## 13. Erros

Nenhum erro foi visto. A lista vazia é o estado visto, não uma falha. A resposta quando a leitura falha não foi vista. Não definido.

## 14. Critérios de aceite

1. Sem modelo, a biblioteca mostra a lista vazia.
2. O conteúdo desta tela é a biblioteca de modelos. A lista de contratos continua na SPEC-022.
3. Aceite não inclui cláusula, campo ou ação de criar e editar.

## 15. Cenários de teste

Abrir a biblioteca sem modelo e ver a lista vazia. Não há cenário de criar, editar ou cláusula: o formulário não foi aberto.

## 16. Dependências

SPEC-001, quando o acesso autenticado deste produto for confirmado. SPEC-022 cobre a lista de contratos e não esta biblioteca. SPEC-007 e RF-082 mostram quantidade e valor do funil e não usam modelo de contrato.

## 17. Premissas e pontos não definidos

- O fluxo está fora do Must. RF-075 e RF-077 estão Observados e não aprovam esta spec.
- Criar e editar não foram acionados.
- Cláusula não foi vista e não é inventada.
- Campos, validações, exclusão, cópia, impressão e vínculo com oportunidade ou com um contrato da SPEC-022 permanecem pendentes.
- Texto, coluna e botão da lista vazia não foram registrados.
- O caminho no menu deste produto não está definido.
