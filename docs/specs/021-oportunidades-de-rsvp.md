# 021. Oportunidades de RSVP

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-021. Oportunidades de RSVP.

## 2. Objetivo

O Organizer consulta leads gerados pela confirmação de presença quando o convidado declara pretender realizar um evento nos próximos dois anos, e decide descartar o lead ou gerar oportunidade no funil.

## 3. Requisitos relacionados

RF-075, no recorte de oportunidades de RSVP e integração do RSVP com o funil. Presença, link e convite permanecem na SPEC-004 e na SPEC-010. O quadro de oportunidades é a SPEC-020. Relatório de oportunidades de RSVP é a SPEC-024. RF-079 fica fora.

## 4. Atores e permissões

O `Organizer` lista e age sobre os leads RSVP da própria conta. Outro `Organizer` não vê. `Admin` acessa qualquer conta. `Client` não acessa. Quem responde o link de presença permanece na SPEC-004, sem login de Organizer. Assistente e fornecedor com login ficam fora.

## 5. Pré-condições

Conta autenticada, SPEC-001. Evento e convite existem, SPEC-002 e SPEC-004. A configuração do RSVP da conta permite a pergunta de intenção de realizar evento e o envio ao funil.

## 6. Fluxo principal

1. O Organizer abre Oportunidades de RSVP.
2. A lista mostra convidados que disseram pretender realizar evento nos próximos dois anos.
3. Cada linha mostra o convite, o evento e a data de criação do lead.
4. O Organizer busca por convite ou por evento.
5. A partir da linha, gera uma oportunidade no funil (estágio Contato inicial) ou descarta o lead, ou abre o contato telefônico já informado no RSVP.

## 7. Fluxos alternativos

- Lista vazia: a tela informa que não há lead.
- Busca sem correspondência: a lista fica sem linhas, sem erro.
- Gerar oportunidade: cria registro na SPEC-020 e remove ou marca o lead como convertido, sem duplicar na lista ativa.
- Descartar lead: tira o item da lista ativa; não cria oportunidade.
- Conta de outro Organizer: leads não aparecem.

## 8. Regras de negócio

- A lista reúne só quem respondeu a intenção nos próximos dois anos no fluxo de RSVP da própria conta.
- Gerar oportunidade cria item em Contato inicial na SPEC-020, com nome derivado do contato/convidado quando disponível.
- Esta lista não é o quadro de estágios; o quadro continua na SPEC-020.
- Descarte aqui é do lead RSVP, distinto do descarte com motivo de uma oportunidade já no quadro.
- Telefone na linha usa o dado já coletado; não inventa contato.

## 9. Validações

Não há formulário de criação manual nesta tela. Ação sobre lead inexistente ou de outra conta não grava.

## 10. Estados

Lead ativo na lista, convertido em oportunidade, ou descartado. Convertido e descartado não reaparecem na lista ativa.

## 11. Dados necessários

Convite, evento, data de criação do lead, dados de contato disponíveis do RSVP.

## 12. Interface

Lista com busca por convite ou evento. Em cada linha: convite, evento, data de criação e ações de descartar, gerar oportunidade e contato. Angular Material, DA-044.

## 13. Erros

Lead de outra conta fica indisponível. Falha ao gerar oportunidade mantém o lead na lista. Lista vazia não é erro.

## 14. Critérios de aceite

1. A lista mostra só leads RSVP da própria conta com intenção nos próximos dois anos.
2. A linha mostra convite, evento e data de criação.
3. A busca filtra por convite ou evento.
4. Gerar oportunidade cria item em Contato inicial na SPEC-020.
5. Descartar remove o lead da lista ativa sem criar oportunidade.
6. Outra conta não vê os leads.

## 15. Cenários de teste

Lista vazia. Lead após RSVP com intenção. Busca por convite e por evento. Gerar oportunidade e conferir SPEC-020. Descartar. Duas contas.

## 16. Dependências

SPEC-001, SPEC-002, SPEC-004, SPEC-010 e SPEC-020. SPEC-024 lê métricas deste fluxo.

## 17. Premissas e pontos não definidos

Texto exato da pergunta no RSVP público. Se o lead convertido permanece arquivado para auditoria além de sair da lista ativa.
