# 011. Estatísticas e resumo dos eventos

**Status: aprovada.** O código segue esta spec. Incremento da lista de eventos, ao lado da SPEC-002 e da SPEC-008. Fonte: [inventario-referencia.md](../inventario-referencia.md) §1.1 e §1.2.

## 1. Identificador e nome

SPEC-011. Estatísticas e resumo dos eventos.

## 2. Objetivo

Na lista de eventos, o Organizer vê a aba Estatísticas (somente leitura) e o bloco Resumo de todos os eventos com atalhos de navegação para módulos relacionados. Lista, busca, filtros, card e cadastro continuam nas specs já aprovadas.

## 3. Requisitos relacionados

RF-078 (estatísticas de eventos). RF-048 somente nos atalhos e no bloco de resumo do topo; abas de situação, busca, filtros, card e cadastro permanecem na SPEC-002 e na SPEC-008.

Fora: RF-079 (marketplace e peças da plataforma de referência). Impressão da lista. Atalho "+" do card. CRUD sobre as estatísticas em si (não há criação nem edição de números).

## 4. Atores e permissões

| Ator | Pode |
|---|---|
| Organizer | Ver totais, gráfico, rankings e o resumo dos próprios eventos. Usar os atalhos para navegar aos módulos relacionados da própria conta. |
| Client | Não acessa esta aba nem o resumo da lista de eventos do Organizer, salvo se flag de permissão futura o autorizar; neste corte, fora. |
| Admin | Acessa as estatísticas e o resumo de qualquer conta. |
| Visitante | Não acessa. |

Assistente e fornecedor com login ficam fora deste corte.

## 5. Pré-condições

Conta autenticada, SPEC-001. Eventos e as três situações vêm da SPEC-002. Serviços e fornecedores da conta alimentam os rankings (SPEC-018).

## 6. Fluxo principal

1. O Organizer abre a lista de eventos da própria conta.
2. Vê o bloco Resumo de todos os eventos e o bloco Aniversários.
3. Abre a aba Estatísticas.
4. Vê o total, as três situações com quantidade e percentual, o gráfico de eventos por mês no ano atual e os três rankings.
5. Pode clicar num atalho do resumo e navegar ao módulo correspondente.
6. Volta para uma aba de situação. A situação dos eventos permanece a da SPEC-002.

## 7. Fluxos alternativos

- Conta sem evento: total e as três quantidades ficam em zero; percentuais ficam em zero; gráfico e rankings ficam vazios.
- Conta sem evento concluído: o ranking de orçamento aparece vazio.
- Conta sem serviço ou sem fornecedor: os rankings afetados aparecem vazios.
- Evento de outra conta: não entra em total, situação, mês nem ranking.
- Busca ou filtro ativos na lista de situações: a aba Estatísticas e o resumo continuam sobre todos os eventos da conta, sem herdar o filtro da lista.

## 8. Regras de negócio

- A lista tem a aba Estatísticas junto das abas Em andamento, Concluído e Cancelado. RF-078.
- A aba mostra o total de eventos da conta e, com percentual, concluídos, em andamento e cancelados. RF-078. Percentual = quantidade da situação / total, com total zero resultando em 0%.
- A aba mostra eventos por mês no ano atual, usando a data do evento. RF-078. Evento sem data não entra no gráfico.
- A aba mostra o ranking de serviços no orçamento, calculado só sobre eventos concluídos: posição, serviço e percentual de participação no orçamento. RF-078.
- A aba mostra o ranking dos fornecedores mais contratados (posição, fornecedor, quantidade) e o dos serviços mais utilizados (posição, serviço, quantidade). RF-078. "Contratado" segue a etapa Contratado da SPEC-018. "Utilizado" conta serviço presente em evento da conta.
- Os números são só da conta do Organizer autenticado, salvo Admin.
- No topo da lista há o bloco Resumo: Pagamentos, Tarefas (contador "Para essa semana"), Anexos e Equipe escalada; ao lado, Aniversários com "Lembretes de aniversários" e contador "Para hoje". RF-048.
- Os atalhos são navegação, não CRUD: Pagamentos → módulo de pagamentos dos eventos; Tarefas → tarefas com recorte da semana; Anexos → anexos da conta/eventos; Equipe escalada → escala/equipe do dia; Aniversários → lembretes de aniversário do dia. O miolo de cada destino permanece na spec do módulo.
- Não há gravação, exclusão nem edição de estatística. CRUD das estatísticas: N/A.

## 9. Validações

Não há campo editável nesta spec. Não há gravação para validar.

## 10. Estados

A aba não tem situação própria. Lê Em andamento, Concluído e Cancelado da SPEC-002. Nenhum bloco desta spec muda essa situação.

## 11. Dados necessários

Eventos da conta, com situação e data, para total, percentuais e gráfico.

Ranking de orçamento (eventos concluídos): posição, nome do serviço, percentual.

Ranking de fornecedores: posição, nome do fornecedor, quantidade de contratações.

Ranking de serviços mais utilizados: posição, nome do serviço, quantidade.

Resumo: rótulos dos atalhos; contador de tarefas da semana; contador de aniversários de hoje.

## 12. Interface

Quatro abas na lista: Em andamento, Concluído, Cancelado e Estatísticas. As três primeiras seguem a SPEC-002.

Na aba Estatísticas: total; três situações com quantidade e percentual; gráfico "Eventos por mês (ano atual)"; três rankings com as colunas da seção 11.

Acima das abas: Resumo de todos os eventos (Pagamentos, Tarefas, Anexos, Equipe escalada) e Aniversários. Sem formulário de gravação nesta tela.

## 13. Erros

Falha ao carregar não revela evento de outra conta. Lista sem evento não é erro: quantidades e percentuais em zero; rankings vazios.

## 14. Critérios de aceite

1. A lista da conta mostra a aba Estatísticas junto das três abas da SPEC-002.
2. A aba mostra o total da conta e a quantidade em cada situação, com percentual; com total zero, percentuais são 0%.
3. A aba mostra "Eventos por mês (ano atual)" e os três rankings, com as colunas da seção 11. O ranking de orçamento só usa eventos concluídos.
4. O resumo mostra Pagamentos, Tarefas com "Para essa semana", Anexos e Equipe escalada; Aniversários mostra "Lembretes de aniversários" com "Para hoje".
5. Clicar num atalho navega ao módulo relacionado da própria conta.
6. Evento de outra conta não entra nesses números.
7. Abrir a aba não muda situação, não grava e não imprime.
8. Busca, filtros, card e cadastro continuam os da SPEC-002 e da SPEC-008.

## 15. Cenários de teste

Conta com um evento em andamento, um concluído e um cancelado: total 3, quantidade 1 em cada situação, percentuais presentes.

Conta sem evento: total e quantidades em zero; rankings vazios.

Duas contas com quantidades diferentes: cada Organizer vê só os próprios totais; Admin vê a conta escolhida.

Evento concluído de outra conta: não entra no total nem no ranking de orçamento.

Clicar em Tarefas no resumo: abre o módulo de tarefas no recorte da semana.

## 16. Dependências

SPEC-001, SPEC-002. SPEC-008 dona do cadastro completo. SPEC-018 para serviço e fornecedor nos rankings. Destinos dos atalhos: pagamentos (RF-068), tarefas (RF-061 / SPEC-003), anexos (RF-073), equipe (RF-051).

## 17. Premissas e pontos não definidos

- Fórmula exata do percentual do ranking de orçamento (base do denominador) quando há vários serviços.
- Quantidade máxima de linhas exibidas em cada ranking e desempate.
- Início da semana do contador "Para essa semana" e fuso horário dos contadores.
- Conteúdo exato do lembrete de aniversário (qual pessoa: cliente, membro da equipe).
- Impressão da lista (RF-048) permanece fora desta spec.
