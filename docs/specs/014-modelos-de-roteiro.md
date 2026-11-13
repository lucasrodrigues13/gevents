# 014. Modelos de roteiro

**Status: rascunho.** Fora do corte aprovado em [mvp-scope.md](../mvp-scope.md). RF-054 e RF-069 permanecem Observado.

## 1. Identificador e nome

SPEC-014. Modelos de roteiro.

## 2. Objetivo

Descrever a biblioteca de modelos de roteiro no trecho visto: seções com horário, música e itens de checklist.

## 3. Requisitos relacionados

RF-054, somente a biblioteca de roteiro. RF-069 fica neste arquivo como dependência observada do roteiro do evento: rótulos vistos, editor não percorrido. Os dois permanecem Observado.

## 4. Atores e permissões

Assessoria, ator de RF-054. Acesso de outra pessoa à biblioteca: não definido. A permissão de o cliente ver ou editar é rótulo de RF-069 e não foi vista nesta biblioteca.

## 5. Pré-condições

Conta autenticada, SPEC-001. Esta biblioteca está fora do corte aprovado do MVP.

## 6. Fluxo principal

1. A assessoria abre a biblioteca de modelos de roteiro.
2. O conteúdo aparece em seções.
3. A seção mostra horário, música e itens de checklist.

## 7. Fluxos alternativos

Criar, salvar e excluir: Pendente. O acionamento não ocorreu. O resultado desses comandos permanece não definido.

Modelo sem seção, seção sem horário, sem música ou sem item de checklist: não definido.

## 8. Regras de negócio

- A biblioteca de roteiro é uma das bibliotecas de RF-054. O status desse RF é Observado.
- A seção vista exibe horário, música e itens de checklist. Obrigatório, editável ou somente leitura: não definido. Não há RF para essa regra.
- Ordenação, observação, anexo, clonagem, impressão e permissão do cliente são rótulos de RF-069. Permanecem fora das regras desta biblioteca.

## 9. Validações

Não definidas. Nenhum campo foi submetido.

## 10. Estados

Não definidos.

## 11. Dados necessários

Seção, horário, música e itens de checklist, no que a tela exibiu. Nome do modelo, ordem da seção, quantidade e vínculo com evento: não definidos.

## 12. Interface

Biblioteca de modelos de roteiro. Seções com horário, música e itens de checklist.

## 13. Erros

Não definidos.

## 14. Critérios de aceite

Enquanto o status for rascunho, o aceite cobre só o que foi visto:

1. A biblioteca mostra seções.
2. A seção mostra horário, música e itens de checklist.

## 15. Cenários de teste

Abrir a biblioteca e ler uma seção com horário, música e itens de checklist. Criar, salvar e excluir não têm cenário.

## 16. Dependências

SPEC-001. RF-054. A aba Roteiros do modelo de evento (RF-053) não foi aberta e não é esta biblioteca. O roteiro dentro do evento continua em RF-069, Observado.

## 17. Premissas e pontos não definidos

- RF-054 e RF-069 permanecem Observado.
- Criar, salvar e excluir: Pendente.
- Nome do modelo e se a biblioteca lista vários modelos.
- Horário, música e item de checklist como campo obrigatório, texto ou lista.
- Ordem das seções e seção vazia.
- Aplicar o modelo a um evento.
- Isolamento entre contas nesta tela.
- Tamanho dos textos.
