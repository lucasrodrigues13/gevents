# 011. Estatísticas e resumo dos eventos

**Status: rascunho.** Incremento da lista de eventos, ao lado da SPEC-002 e da SPEC-008. Fonte observada: [inventario-referencia.md](../inventario-referencia.md) §1.1 e §1.2. RF-048 e RF-078 permanecem Observado. Esta spec não os aprova e não altera o corte de [mvp-scope.md](../mvp-scope.md).

## 1. Identificador e nome

SPEC-011. Estatísticas e resumo dos eventos.

## 2. Objetivo

Na lista de eventos, a conta vê a aba Estatísticas e o resumo de todos os eventos, no que a tela mostrou. Lista, busca, filtros, card e cadastro continuam nas specs já escritas.

## 3. Requisitos relacionados

Os dois RF abaixo estão Observado. Nada do que está Observado entra como regra aprovada.

RF-048, Lista de eventos. Comportamento observado: "Abas em andamento, concluído e cancelado. Busca por nome. Filtros de tipo, equipe, assistente, período, modelo e tags. Card com data, contagem, modelo, tarefas e convidados. Ações de editar, cancelar, concluir, remover e imprimir a lista." Regras vistas: "Tags vistas: assessoria completa, parcial e final. São dados da conta, não uma lista fechada. Atalhos no topo: pagamentos, tarefas da semana, anexos, equipe escalada, aniversários." Ator: Assessoria. Depende de RF-049.

Deste RF, esta spec usa só os atalhos do topo. Abas, busca, filtros, card e cadastro permanecem na SPEC-002 e na SPEC-008.

RF-078, Estatísticas de eventos. Comportamento observado: "Existe uma aba com esse nome na lista de eventos." Regras vistas: "Total, concluídos, em andamento e cancelados com percentual. Eventos por mês no ano atual. Ranking de serviços no orçamento (eventos concluídos), fornecedores mais contratados e serviços mais utilizados." Ator: Assessoria. Depende de RF-048.

O fluxo não está no corte aprovado. Em mvp-scope.md, a aba Estatísticas fica de fora da lista de eventos, e RF-078 está fora do corte.

Fora deste arquivo: reescrita da lista, da busca, dos filtros e do cadastro. Destino dos atalhos do resumo. Impressão da lista, nomeada em RF-048, cujo fluxo não foi aberto. Atalho "+" do card, que não foi aberto. Gravação e exclusão.

## 4. Atores e permissões

O User da conta vê totais, mês, rankings e o resumo dos próprios eventos. Outro User não entra nesses números. Visitante não acessa.

Na referência o ator é a Assessoria. Não surge papel de negócio além de `Admin` e `User`.

## 5. Pré-condições

Usuário autenticado, SPEC-001. Eventos e as três situações vêm da SPEC-002. A SPEC-008 continua responsável pelo cadastro completo e não é relida aqui.

Serviço e fornecedor, usados pelos rankings, não têm spec aprovada. Sem esses dados, o que o ranking exibe fica na seção 17.

## 6. Fluxo principal

1. O usuário abre a lista de eventos da própria conta.
2. Vê o bloco Resumo de todos os eventos e o bloco Aniversários.
3. Abre a aba Estatísticas.
4. Vê o total, as três situações com quantidade e percentual, o bloco de eventos por mês no ano atual e os três rankings.
5. Volta para uma aba de situação. A situação dos eventos permanece a da SPEC-002.

## 7. Fluxos alternativos

- Conta sem evento: total e as três quantidades ficam em zero. O percentual com total zero não foi visto.
- Conta sem evento concluído: o ranking de orçamento não tem o recorte que RF-078 nomeia. O que a tela mostra nesse caso não foi visto.
- Conta sem serviço ou sem fornecedor: o conteúdo dos rankings não foi visto.
- Evento de outra conta: não entra em total, situação, mês nem ranking.
- Clicar em Pagamentos, Tarefas, Anexos, Equipe escalada ou Aniversários: o destino não foi aberto.
- Busca ou filtro ativos na lista: não foi visto se a aba e o resumo acompanham esse recorte.

## 8. Regras de negócio

- A lista tem a aba Estatísticas. RF-078.
- A aba mostra o total de eventos e, com percentual, concluídos, em andamento e cancelados. RF-078.
- A aba mostra eventos por mês no ano atual. RF-078.
- A aba mostra o ranking de serviços no orçamento, calculado sobre eventos concluídos. RF-078.
- A aba mostra o ranking dos fornecedores mais contratados e o ranking dos serviços mais utilizados. RF-078.
- No topo da lista há atalhos de pagamentos, tarefas da semana, anexos, equipe escalada e aniversários. RF-048.
- O atalho de tarefas mostra o contador "Para essa semana". O bloco de aniversários mostra "Lembretes de aniversários" com o contador "Para hoje". RF-048, no rótulo visto. A conta que gera cada número não tem RF e fica não definida.
- As três quantidades usam as situações da SPEC-002. O total é a quantidade de eventos da conta.
- Os números são os da conta autenticada. RF-078 fala nos eventos da assessoria.
- Onde o RF não fecha fórmula, ordenação, recorte ou destino, a regra fica não definida. Esta spec não escolhe o valor.

## 9. Validações

Não há campo editável nesta spec. Não há gravação para validar.

## 10. Estados

A aba não tem situação própria. Ela lê Em andamento, Concluído e Cancelado da SPEC-002. Nenhum bloco desta spec muda essa situação.

## 11. Dados necessários

Eventos da conta, com a situação da SPEC-002, para o total e para as três quantidades.

Eventos por mês: o bloco é do ano atual. Qual data coloca o evento no mês não foi definida.

Ranking de orçamento, só eventos concluídos: posição, serviço e percentual. A base do percentual não foi definida.

Ranking de fornecedores: posição, fornecedor e quantidade. O que conta como contratado não foi definido.

Ranking de serviços mais utilizados: posição, serviço e quantidade. O que conta como utilizado não foi definido.

Resumo: os quatro rótulos e os dois contadores. A origem de cada contador não foi definida.

## 12. Interface

Quatro abas na lista: Em andamento, Concluído, Cancelado e Estatísticas. As três primeiras seguem a SPEC-002. A aba Estatísticas foi vista com uma contagem; o que essa contagem numera fica na seção 17.

Na aba Estatísticas:

- Total de eventos.
- Concluídos, em andamento e cancelados, cada um com quantidade e percentual.
- Gráfico "Eventos por mês (ano atual)".
- "Ranking dos serviços que mais consomem as partes do orçamento dos seus eventos", sobre eventos concluídos, com posição, serviço e percentual.
- "Ranking dos fornecedores mais contratados", com posição, fornecedor e quantidade.
- "Ranking dos serviços mais utilizados", com posição, serviço e quantidade.

Acima das abas, o bloco "Resumo de todos os eventos": Pagamentos, Tarefas com o contador "Para essa semana", Anexos e Equipe escalada. Ao lado, o bloco "Aniversários": "Lembretes de aniversários" com o contador "Para hoje".

Não há formulário, botão de gravar, exclusão nem impressão nesta tela, no que foi percorrido.

## 13. Erros

Falha ao carregar não revela evento de outra conta. O texto de erro da referência não foi visto. Lista sem evento não é erro: as quantidades ficam em zero. Percentual e rankings nesse caso seguem a seção 17.

## 14. Critérios de aceite

1. A lista da conta mostra a aba Estatísticas junto das três abas da SPEC-002.
2. A aba mostra o total da conta e a quantidade em cada situação da SPEC-002, com um percentual em cada situação.
3. A aba mostra "Eventos por mês (ano atual)" e os três rankings, com as colunas da seção 12. O ranking de orçamento declara o recorte de eventos concluídos.
4. O resumo mostra Pagamentos, Tarefas com "Para essa semana", Anexos e Equipe escalada. Aniversários mostra "Lembretes de aniversários" com "Para hoje".
5. Evento de outra conta não entra nesses números.
6. Abrir a aba não muda situação, não grava e não imprime.
7. Busca, filtros, card e cadastro continuam os da SPEC-002 e da SPEC-008.

## 15. Cenários de teste

Conta com um evento em andamento, um concluído e um cancelado. Resultado: total 3 e quantidade 1 em cada situação. O percentual aparece. O valor do percentual não é critério.

Conta sem evento. Resultado: total e as três quantidades em zero.

Duas contas com quantidades diferentes. Resultado: cada uma vê só os próprios totais.

Evento concluído de outra conta. Resultado: não aparece no total nem no ranking de orçamento.

Aba Em andamento, com a quarta aba presente. Resultado: a lista dessa aba continua só com eventos em andamento da conta, pela SPEC-002.

Resumo visível. Resultado: os rótulos e os dois contadores aparecem. Clicar neles não tem destino exigido.

## 16. Dependências

SPEC-001 e SPEC-002. A SPEC-008 segue dona do cadastro completo.

Os rankings dependem de serviço e fornecedor (RF-058 e RF-057), ainda sem spec aprovada. O resumo aponta para pagamentos (RF-068), tarefas (RF-061), anexos (RF-073) e equipe (RF-051). O miolo desses atalhos não foi aberto e não é fluxo desta spec. O lembrete de aniversário continua fora da SPEC-003.

RF-077 não cobre esta aba. O próprio RF-077 manda ver RF-078.

## 17. Premissas e pontos não definidos

- RF-048 e RF-078 continuam Observado. Aprovar esta spec é decisão do usuário.
- O corte aprovado deixa a aba Estatísticas e o RF-078 de fora. Esta spec não os traz para o corte.
- Fórmula do percentual, arredondamento e o percentual quando o total é zero.
- Qual data entra em "Eventos por mês (ano atual)". Meses sem evento. Troca de ano.
- O que a contagem da aba Estatísticas numera.
- Se busca e filtros da lista restringem a aba ou o resumo.
- Quantas linhas cada ranking mostra, como empate se ordena e o que o percentual do ranking de orçamento divide.
- O que a quantidade de "mais contratados" e de "mais utilizados" conta.
- O que o ranking mostra sem evento concluído, sem serviço ou sem fornecedor.
- A conta dos contadores "Para essa semana" e "Para hoje": quais tarefas, de quem é o aniversário, início da semana e fuso.
- Destino ao clicar em Pagamentos, Tarefas, Anexos, Equipe escalada e Aniversários.
- Imprimir a lista, nomeado em RF-048, e o atalho "+" do card. Os dois não foram abertos. Não há fluxo de gravação, exclusão nem impressão nesta spec.
