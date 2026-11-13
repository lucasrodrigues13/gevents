# 017. Modelos de mensagens

**Status: rascunho.** Fonte observada: a página `https://assessoriavip.com.br/modelos_mensagens`, aberta vazia. RF-054 está Observado e fora do corte aprovado em [mvp-scope.md](../mvp-scope.md), seção 5. O fluxo não está no Must e não foi confirmado na conta.

## 1. Identificador e nome

SPEC-017. Modelos de mensagens.

## 2. Objetivo

Descrever a biblioteca de modelos de mensagem no único estado visto: lista vazia, com o texto e a ação que apareceram na página.

## 3. Requisitos relacionados

RF-054, somente a biblioteca de mensagem. O status dele continua Observado.

Fora deste arquivo: briefing, roteiro, cronograma e inspiração, que também estão em RF-054; modelo de evento (RF-053); RSVP automático (RF-065); roteiro do evento (RF-069); cronograma (RF-070); briefing do evento (RF-071); inspirações (RF-072).

## 4. Atores e permissões

Assessoria, na conta autenticada. A role `User` identifica a conta e não é papel de negócio, RF-040. Cliente, assistente e fornecedor ficam de fora deste fluxo.

## 5. Pré-condições

Conta autenticada, SPEC-001. A página vista não pediu evento aberto.

## 6. Fluxo principal

1. A assessoria abre a biblioteca de modelos de mensagens.
2. Sem modelos cadastrados, a tela mostra o texto "Sem modelos de mensagens cadastrados".
3. A tela mostra a ação "Novo Modelo".

A ação não foi acionada. O fluxo para nesse ponto.

## 7. Fluxos alternativos

Biblioteca com modelo já cadastrado: não visto. Destino de "Novo Modelo": não visto.

## 8. Regras de negócio

- A assessoria tem uma biblioteca de mensagem. RF-054, Observado.
- O editor dessa biblioteca não foi aberto. RF-054 registra a biblioteca no menu, sem o editor.
- O texto "Sem modelos de mensagens cadastrados" e o rótulo "Novo Modelo" foram vistos na página vazia. Não há RF que fixe esse texto ou esse rótulo. Ficam como observação da página, sem virar regra decidida.

## 9. Validações

Nenhuma validação foi vista. Campo, obrigatoriedade e formato do modelo: não definido.

## 10. Estados

Estado visto: biblioteca sem modelos, com o texto e a ação da seção 6. Biblioteca com modelos: não definido. Editor aberto: não definido.

## 11. Dados necessários

Não definido. A página vazia não mostrou campo de modelo.

## 12. Interface

Biblioteca vazia com o texto "Sem modelos de mensagens cadastrados" e a ação "Novo Modelo". Composição do editor, lista preenchida, busca e demais ações: não definido.

## 13. Erros

Nenhum erro foi visto nesta página. Mensagem de falha ao abrir ou ao criar: não definido.

## 14. Critérios de aceite

1. Sem modelos cadastrados, a biblioteca mostra "Sem modelos de mensagens cadastrados".
2. A mesma tela mostra a ação "Novo Modelo".
3. O que ocorre ao acionar "Novo Modelo" fica fora do aceite enquanto o editor não for visto.

Estes critérios descrevem a página vista. RF-054 segue Observado.

## 15. Cenários de teste

Abrir a biblioteca sem modelos e conferir o texto e a ação. Não acionar "Novo Modelo". Cenário com modelo existente e cenário do editor ficam sem resultado esperado até haver observação.

## 16. Dependências

SPEC-001, para a conta autenticada. RF-054 aponta RF-069, RF-070, RF-071, RF-072 e RF-065; essas peças não são pré-requisito do estado vazio visto.

## 17. Premissas e pontos não definidos

- Editor, campos, gravação, edição, exclusão, clonagem e ordenação.
- Aparência da lista quando já existe modelo.
- Dono do dado: conta, evento ou outro vínculo. A página vazia não mostrou essa ligação.
- Uso do modelo em envio, WhatsApp, fornecedor, parcela, RSVP ou na aba Mensagens do modelo de evento.
- Permissão de cliente para ver modelo de mensagem.
- Isolamento entre contas nesta biblioteca. A página vazia não foi comparada com outra conta.
- Resultado de acionar "Novo Modelo".
