# 022. Contratos do funil

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-022. Contratos do funil.

## 2. Objetivo

O Organizer gerencia contratos comerciais do funil ligados a oportunidades: cria, edita, envia, assina, cancela e consulta nas abas Em andamento, Assinados e Cancelados.

## 3. Requisitos relacionados

RF-075, no recorte de contratos do funil. Modelo de contrato é a SPEC-028. Oportunidade é a SPEC-020. Contrato pedido ao fornecedor no evento permanece em RF-059 e RF-060, fora desta tela. RF-079 fica fora.

## 4. Atores e permissões

O `Organizer` opera os contratos da própria conta. Outro `Organizer` não vê. `Admin` acessa qualquer conta. `Client` não acessa o funil neste corte. Assistente e fornecedor com login ficam fora.

## 5. Pré-condições

Conta autenticada, SPEC-001. Para criar, existe oportunidade da mesma conta, SPEC-020. Modelo opcional vem da SPEC-028.

## 6. Fluxo principal

1. O Organizer abre Contratos no funil.
2. Vê filtro por criação, busca por oportunidade e as abas Em andamento, Assinados e Cancelados.
3. Cria contrato vinculado a uma oportunidade, com conteúdo a partir do modelo ou texto próprio, e salva em Em andamento.
4. Edita o contrato em andamento.
5. Envia o contrato ao contato da oportunidade.
6. Registra a assinatura; o item passa à aba Assinados.
7. Cancela um contrato em andamento; o item passa à aba Cancelados.

## 7. Fluxos alternativos

- Aba vazia: a tela informa que não há item.
- Busca ou filtro sem correspondência: a lista fica vazia, sem erro.
- Excluir contrato em andamento ainda não assinado: remove o registro.
- Contrato assinado: não volta a Em andamento; cancelamento após assinatura, se necessário, vai para Cancelados.
- Oportunidade de outra conta: não vincula.

## 8. Regras de negócio

- Contrato do funil pertence à conta e a uma oportunidade da mesma conta.
- Situações: Em andamento, Assinado e Cancelado, espelhadas nas abas.
- Enviar não assina; assinar não cancela.
- O modelo da SPEC-028, quando usado, copia o texto para o contrato; depois o contrato tem vida própria.
- Este fluxo é distinto do contrato de fornecedor no evento.

## 9. Validações

Oportunidade obrigatória e da mesma conta. Conteúdo do contrato obrigatório para salvar. Situação só nas três previstas.

## 10. Estados

Em andamento → Assinado. Em andamento → Cancelado. Assinado → Cancelado quando o Organizer cancelar após assinatura. Exclusão só de Em andamento neste corte.

## 11. Dados necessários

Oportunidade, conteúdo do contrato, data de criação, situação, data de envio e de assinatura quando existirem, referência ao modelo usado se houver.

## 12. Interface

Filtro Criado em, busca por oportunidade, abas Em andamento, Assinados e Cancelados. Ações criar, editar, enviar, assinar, cancelar e excluir quando couber. Angular Material, DA-044.

## 13. Erros

Campo inválido indica o campo. Item de outra conta fica indisponível. Falha ao gravar mantém a situação anterior. Lista vazia não é erro.

## 14. Critérios de aceite

1. A lista mostra filtro, busca e as três abas.
2. Criar com oportunidade e conteúdo coloca o contrato em Em andamento.
3. Enviar mantém Em andamento e registra o envio.
4. Assinar move para Assinados.
5. Cancelar move para Cancelados.
6. Outra conta não vê o contrato.
7. Contrato de fornecedor do evento não aparece nesta lista.

## 15. Cenários de teste

Abas vazias. Criar, editar, enviar, assinar e cancelar. Busca por oportunidade. Duas contas. Tentativa sem oportunidade.

## 16. Dependências

SPEC-001, SPEC-020 e SPEC-028.

## 17. Premissas e pontos não definidos

Canal exato do envio (e-mail ou outro) além do registro de que foi enviado. Assinatura eletrônica com validade jurídica fica fora deste corte.
