# 022. Contratos do funil

**Status: rascunho.**

Fonte da visita: a lista em `https://assessoriavip.com.br/funnel/contract`, com as três abas vazias. Criar, assinar, cancelar e enviar não foram acionados. O modelo de contrato fica na SPEC-028. O contrato pedido ao fornecedor no evento fica em RF-059 e RF-060.

## 1. Identificador e nome

SPEC-022. Contratos do funil.

## 2. Objetivo

Descrever a lista de contratos do funil no que a visita mostrou. O restante do fluxo permanece em aberto.

## 3. Requisitos relacionados

RF-075 cita contratos entre os itens do funil de vendas. Status Observado. Esse status não aprova regra desta tela.

RF-077 cita catálogos de funil entre relatórios e impressões. Status Observado. A visita não abriu relatório nem impressão.

O dashboard do funil segue RF-082 e a SPEC-007: quantidade e valor em contato inicial, orçamento enviado, contratado e descartado. As abas desta lista são rótulos vistos na tela de contratos. Elas não entram no dashboard.

Este fluxo está fora do corte em [mvp-scope.md](../mvp-scope.md). De RF-075, o corte aprovado usa só o que RF-082 já cobre.

## 4. Atores e permissões

A tela foi aberta na conta da assessoria. Quem mais acessa, e se o `User` vê só a própria conta, está Pendente.

## 5. Pré-condições

Conta autenticada, SPEC-001. A visita encontrou as três abas vazias. O que precisa existir para um contrato aparecer na lista está Pendente.

## 6. Fluxo principal

1. A assessoria abre a lista de contratos do funil.
2. A tela mostra o filtro "Criado em", a busca por oportunidade e as abas Em andamento, Assinados e Cancelados.
3. Na visita, as três abas estavam vazias.
4. Na aba Cancelados, o texto visível foi "Nenhum orçamento encontrado!".

O fluxo visto para nesse ponto.

## 7. Fluxos alternativos

- Aba Cancelados vazia: o texto visto foi "Nenhum orçamento encontrado!".
- Abas Em andamento e Assinados: estavam vazias. O texto de cada uma não foi registrado.
- Filtro "Criado em" e busca por oportunidade: os controles estavam visíveis. Nenhum valor foi aplicado. O resultado de filtrar ou buscar está Pendente.

## 8. Regras de negócio

Não há RF Definido para esta lista. RF-075 e RF-077 estão Observado e não fecham regra. O que a visita não mostrou fica não definido:

- O que o filtro "Criado em" restringe.
- O que a busca por oportunidade compara.
- Se Em andamento, Assinados e Cancelados são situações do contrato, e o que muda um item de aba.
- Criar, assinar, cancelar e enviar.
- Campos do contrato e o vínculo com oportunidade, orçamento e evento.
- Se o texto "Nenhum orçamento encontrado!" aparece nas outras abas vazias, e o que a palavra orçamento indica nesta lista.

## 9. Validações

Nenhuma validação foi vista. Pendente.

## 10. Estados

Rótulos de aba vistos: Em andamento, Assinados, Cancelados. A passagem entre eles não foi executada. O ciclo de vida fica não definido.

## 11. Dados necessários

Não definido. A lista vazia não mostrou coluna nem campo.

## 12. Interface

Controles vistos: filtro "Criado em", busca por oportunidade, abas Em andamento, Assinados e Cancelados. Na aba Cancelados, com a lista vazia, o texto "Nenhum orçamento encontrado!".

A visita não mostrou cartão, coluna nem formulário. A interface do produto, se esta spec for aprovada, usa Angular Material, DA-044, com esses textos.

## 13. Erros

Nenhum erro foi provocado. Na aba Cancelados vazia, o texto foi "Nenhum orçamento encontrado!". A visita não classificou essa frase como falha.

## 14. Critérios de aceite

O aceite abaixo descreve a visita. Vale depois da aprovação desta spec.

1. A lista mostra o filtro "Criado em", a busca por oportunidade e as abas Em andamento, Assinados e Cancelados.
2. Na aba Cancelados, sem itens, o texto visível é "Nenhum orçamento encontrado!".

Criar, assinar, cancelar e enviar ficam sem critério até serem vistos.

## 15. Cenários de teste

Abrir a lista com as três abas vazias. Na aba Cancelados, ler "Nenhum orçamento encontrado!". Criar, assinar, cancelar e enviar não têm cenário: essas ações não foram acionadas.

## 16. Dependências

SPEC-001. SPEC-007 e RF-082 seguem com os quatro estágios já definidos. O cadastro da oportunidade do funil ainda não tem spec. SPEC-028 é o modelo de contrato. RF-059 e RF-060 são o contrato no fornecedor do evento e estão Observado.

## 17. Premissas e pontos não definidos

Pendente, no sentido de pergunta em aberto, sem caminho escolhido:

- Conteúdo de um contrato quando a lista tem item.
- Ação e efeito de criar, assinar, cancelar e enviar.
- Comportamento do filtro "Criado em" e da busca por oportunidade depois de preenchidos.
- Texto das abas Em andamento e Assinados quando vazias.
- Sentido de "Nenhum orçamento encontrado!" numa aba de contratos.
- Campos, obrigatórios e validações.
- Quem acessa, além da conta em que a tela foi aberta, e o isolamento entre contas.
- Relação com oportunidade, orçamento, evento e com o modelo de contrato da SPEC-028.
- Relatório ou impressão. RF-077 não foi exercido nesta tela.

Não há RF novo. A lacuna fica não definida, como a skill pede quando não existe RF para a regra.
