# 025. Orçamentos do funil

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-025. Orçamentos do funil.

## 2. Objetivo

O Organizer gerencia orçamentos comerciais do funil ligados a oportunidades: cria, edita, envia, registra resposta, cancela e consulta nas abas Em andamento, Respondidos e Cancelados.

## 3. Requisitos relacionados

RF-075, no recorte de orçamentos do funil. Modelo de orçamento é a SPEC-027. Oportunidade é a SPEC-020. Orçamento de fornecedor no evento e budget do evento ficam fora. RF-079 fica fora.

## 4. Atores e permissões

O `Organizer` opera os orçamentos da própria conta. Outro `Organizer` não vê. `Admin` acessa qualquer conta. `Client` não acessa o funil neste corte. Assistente e fornecedor com login ficam fora.

## 5. Pré-condições

Conta autenticada, SPEC-001. Para criar, existe oportunidade da mesma conta, SPEC-020. Modelo opcional vem da SPEC-027.

## 6. Fluxo principal

1. O Organizer abre Orçamentos no funil.
2. Vê filtro de criação, busca e as abas Em andamento, Respondidos e Cancelados.
3. Cria orçamento vinculado a uma oportunidade, com conteúdo a partir do modelo ou texto próprio, e salva em Em andamento.
4. Edita o orçamento em andamento.
5. Envia ao contato da oportunidade.
6. Registra a resposta; o item passa à aba Respondidos.
7. Cancela um orçamento em andamento; o item passa à aba Cancelados.

## 7. Fluxos alternativos

- Aba vazia: a tela informa que não há orçamento.
- Busca ou filtro sem correspondência: lista vazia, sem erro.
- Excluir orçamento em andamento ainda não respondido: remove o registro.
- Oportunidade de outra conta: não vincula.
- Ao enviar orçamento, o Organizer pode mover a oportunidade para Orçamento enviado na SPEC-020; a mudança de estágio não é automática se o Organizer não a confirmar.

## 8. Regras de negócio

- Orçamento do funil pertence à conta e a uma oportunidade da mesma conta.
- Situações: Em andamento, Respondido e Cancelado.
- Enviar não responde; responder não cancela.
- O modelo da SPEC-027, quando usado, copia parâmetros e texto; depois o orçamento tem vida própria.
- Distinto do budget do evento e do orçamento de fornecedor.

## 9. Validações

Oportunidade obrigatória e da mesma conta. Conteúdo obrigatório para salvar. Situação só nas três previstas.

## 10. Estados

Em andamento → Respondido. Em andamento → Cancelado. Exclusão só de Em andamento neste corte.

## 11. Dados necessários

Oportunidade, conteúdo, valor quando informado, data de criação, situação, data de envio e de resposta quando existirem, referência ao modelo usado se houver.

## 12. Interface

Filtro de criação, busca, abas Em andamento, Respondidos e Cancelados. Ações criar, editar, enviar, responder, cancelar e excluir quando couber. Angular Material, DA-044.

## 13. Erros

Campo inválido indica o campo. Item de outra conta fica indisponível. Falha ao gravar mantém a situação anterior. Lista vazia não é erro.

## 14. Critérios de aceite

1. A lista mostra filtro, busca e as três abas.
2. Criar com oportunidade e conteúdo coloca o orçamento em Em andamento.
3. Enviar mantém Em andamento e registra o envio.
4. Registrar resposta move para Respondidos.
5. Cancelar move para Cancelados.
6. Outra conta não vê o orçamento.
7. Budget do evento e orçamento de fornecedor não aparecem nesta lista.

## 15. Cenários de teste

Abas vazias. Criar, editar, enviar, responder e cancelar. Busca. Duas contas. Sem oportunidade.

## 16. Dependências

SPEC-001, SPEC-020 e SPEC-027.

## 17. Premissas e pontos não definidos

Canal exato do envio além do registro de que foi enviado. Campos detalhados da resposta do lead além da mudança de aba.
