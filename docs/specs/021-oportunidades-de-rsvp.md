# 021. Oportunidades de RSVP

**Status: rascunho.** Fora do Must de [mvp-scope.md](../mvp-scope.md): a seção 5 deixa RF-077 de fora e, de RF-075, só a quantidade e o valor dos quatro estágios no dashboard. RF-075 e RF-077 continuam Observado. Quem responde presença por link continua na SPEC-004 e na SPEC-010.

Fonte da observação: tela do funil, oportunidades de RSVP, na conta de referência. Os três ícones da linha não foram acionados.

## 1. Identificador e nome

SPEC-021. Oportunidades de RSVP.

## 2. Objetivo

A assessoria consulta os convidados que disseram pretender realizar um evento nos próximos dois anos.

## 3. Requisitos relacionados

Nenhum RF aprovado descreve esta lista. RF-075 e RF-077 estão Observado e não entram como regra. A proposta RF-145 está só neste rascunho e no resumo de revisão. Ela ainda não está em `docs/requirements.md` e não está aprovada.

| ID | Nome | Comportamento observado | Ator | Regras vistas | Depende de | Status |
|---|---|---|---|---|---|---|
| RF-145 | Oportunidades de RSVP | Lista de convidados que disseram pretender realizar evento nos próximos dois anos. Há busca por convite ou evento. | Assessoria | Cada linha mostra o convite, o evento e a data de criação, mais os ícones de pasta com menos, pasta com mais e telefone. Os ícones não foram acionados. | — | Observado |

Presença, link e convite permanecem na SPEC-004 e na SPEC-010.

## 4. Atores e permissões

A assessoria consulta a lista na própria conta. Quem responde o link de presença permanece na SPEC-004. A role `User` identifica a conta, RF-040.

Quem mais abre a lista, e se outra conta a vê, fica pendente na seção 17.

## 5. Pré-condições

Conta da assessoria, SPEC-001. A linha mostra evento e convite já existentes, SPEC-002 e SPEC-004. O passo em que o convidado declara a intenção ainda não tem spec.

## 6. Fluxo principal

1. A assessoria abre a lista de oportunidades de RSVP.
2. A lista mostra os convidados que disseram pretender realizar evento nos próximos dois anos.
3. A busca usa convite ou evento.
4. Cada linha mostra o convite, o evento e a data de criação.
5. Cada linha mostra os ícones de pasta com menos, pasta com mais e telefone.

## 7. Fluxos alternativos

Os três ícones ficam sem fluxo até serem acionados. Busca sem correspondência e lista sem linhas não foram vistas.

## 8. Regras de negócio

- A lista reúne quem disse pretender realizar evento nos próximos dois anos. Proposta RF-145, Observado, sem aprovação.
- A busca é por convite ou evento. Proposta RF-145. Fica não definido se a busca é um campo ou dois, e o que conta como acerto.
- A linha mostra o convite, o evento e a data de criação. Proposta RF-145. Fica não definido a qual registro a data pertence.
- Os três ícones aparecem na linha. A ação de cada um não tem RF e fica não definida.
- Presença, link e convite continuam na SPEC-004 e na SPEC-010.
- O vínculo com os quatro estágios do funil e com um item do catálogo de relatórios fica não definido. RF-075 e RF-077 não fecham esse vínculo.

## 9. Validações

Nenhuma validação foi vista.

## 10. Estados

A lista não mostrou estado próprio nem transição de linha.

## 11. Dados necessários

Na linha: convite, evento e data de criação. Os ícones estão na linha. O dado por trás de cada ícone não foi visto.

## 12. Interface

Lista com busca. Em cada linha: convite, evento, data de criação, pasta com menos, pasta com mais e telefone. Os rótulos exatos não foram transcritos. A rota do produto fica pendente.

## 13. Erros

Nenhum erro foi visto.

## 14. Critérios de aceite

1. A lista mostra convidados que disseram pretender realizar evento nos próximos dois anos.
2. A busca é por convite ou evento.
3. A linha mostra o convite, o evento e a data de criação.
4. A linha mostra os ícones de pasta com menos, pasta com mais e telefone.
5. O efeito dos três ícones fica fora deste aceite.

## 15. Cenários de teste

Abrir a lista e conferir quem aparece, a busca por convite, a busca por evento e as três informações da linha. Os ícones permanecem sem acionamento. Conta sem linhas, busca sem acerto e duas contas ainda não têm resultado visto.

## 16. Dependências

SPEC-001, SPEC-002 e SPEC-004. A SPEC-010 permanece a lista completa de convite e convidado. O dashboard do funil, SPEC-007, não passa a ler esta lista.

## 17. Premissas e pontos não definidos

- Efeito do ícone de pasta com menos.
- Efeito do ícone de pasta com mais.
- Efeito do ícone de telefone.
- Se a busca é um campo ou dois, e o critério de correspondência.
- A qual registro a data de criação pertence.
- Rótulos, ordenação, lista vazia e busca sem resultado.
- Se a linha abre outra tela.
- Onde o convidado declara a intenção de realizar evento nos próximos dois anos.
- Se a linha é uma oportunidade nos estágios de RF-075.
- Se a tela é o item Oportunidades de RSVP do catálogo de relatórios de RF-077.
- Acesso sem login e visibilidade para outra conta.
- A proposta RF-145 ainda não está em `docs/requirements.md`.
