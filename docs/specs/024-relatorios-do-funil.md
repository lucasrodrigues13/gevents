# 024. Relatórios do funil

**Status: rascunho.** RF-075 e RF-077 continuam Observado. Esta spec não os aprova e não entra no código.

## 1. Identificador e nome

SPEC-024. Relatórios do funil.

## 2. Objetivo

Registrar o catálogo de relatórios do funil visto na conta de referência, no período padrão, com os nove relatórios abertos e sem movimento.

## 3. Requisitos relacionados

RF-077, Observado. O texto cita o catálogo de funil, o catálogo financeiro e as impressões. Não descreve os nove relatórios, o período nem as colunas. Não está aprovado.

RF-075, Observado. Cobre o funil, o descarte com motivo e o quadro de quantidade e valor. Não descreve esta tela. Não está aprovado.

RF-082, Definido, e a SPEC-007. Quantidade e valor nos quatro estágios do dashboard. Esta tela é outra.

RF-078, Observado. A aba Estatísticas da lista de eventos fica de fora, como o próprio RF-077 aponta.

RF-035, Pendente. O comportamento aprovado para implementação segue as specs 001 a 010. Este arquivo é rascunho.

O fluxo não está no Must. Em [mvp-scope.md](../mvp-scope.md), RF-077 fica de fora do corte. De RF-075 entra só o que RF-082 usa no dashboard.

Não há RF para o intervalo do período padrão, para a base do período, para os nomes internos do catálogo nem para as colunas e métricas. Esses pontos ficam não definidos na seção 17. Nenhum RF novo é criado.

Fora deste arquivo: catálogo financeiro, impressões de convidados, custos, roteiro, cronograma, briefing, alinhamento, tarefas, resumo e agenda.

## 4. Atores e permissões

RF-077 nomeia a assessoria. A visita ocorreu numa conta autenticada da referência.

Quem abre a tela neste produto, e se outra conta vê os mesmos relatórios, não foi visto. Não definido. RF-040 impede criar papel novo para nomear a assessoria.

## 5. Pré-condições

Conta autenticada, SPEC-001, na visita à referência.

O cadastro da oportunidade ainda não tem spec. A visita não precisou dele: o período estava sem movimento.

## 6. Fluxo principal

1. A conta abre o catálogo de relatórios do funil da referência. A rota vista foi `https://assessoriavip.com.br/funnel/reports`. A rota deste produto não está definida.
2. Há um período em vigor. Na visita era o período padrão. O intervalo e o controle para trocá-lo não foram lidos.
3. Nove relatórios aparecem abertos. Nenhum mostra movimento no período.
4. A estrutura lida em cada um está na seção 12. Onde a visita não mostrou coluna, filtro ou número, o fluxo para.

## 7. Fluxos alternativos

- Período sem movimento: não há linha, total numérico nem ponto de gráfico para registrar.
- Trocar o período: não foi visto.
- Fechar um relatório: não foi visto. Na visita os nove estavam abertos.
- Relatório com movimento: não foi visto. A causa “conta sem oportunidade” e a causa “período sem lançamento” não foram separadas.
- Outra conta na mesma tela: não foi visto.

## 8. Regras de negócio

- O arquivo cobre o catálogo de relatórios do funil. RF-077, Observado, não aprovado.
- A aba Estatísticas da lista de eventos fica neste outro requisito. RF-078, Observado. RF-077 só aponta para ela.
- Quantidade e valor dos estágios contato inicial, orçamento enviado, contratado e descartado pertencem ao dashboard. RF-082.
- Estágios configuráveis, filtro, exportação, formulário público de lead, checagem de data, contratos, orçamentos e tarefas de reunião, ligação, e-mail ou tarefa não apareceram nesta visita. RF-075 os cita no funil. Não entram nesta spec.
- Os nove relatórios, a legenda do período e as tabelas da seção 12 foram vistos. O texto de RF-077 e de RF-075 não os fixa. Adotá-los como regra do produto fica não definido até a revisão desta spec.
- Quais estágios contam como fechado ou perdido não foi visto. Não definido. Os quatro estágios de RF-082 não são copiados para este relatório.
- A lista de motivos de descarte não foi vista. RF-075 diz que o descarte tem motivo e não lista os motivos desta tela. Não definido.

## 9. Validações

Não há formulário nesta visita. Validação de período, de filtro ou de exportação não foi vista. Não definido.

## 10. Estados

Na visita, o período era o padrão e os nove relatórios estavam abertos, sem movimento.

Estado de relatório fechado, período alterado e relatório com movimento não foi visto. Não definido.

## 11. Dados necessários

A visita não exibiu lançamento. Não há número para guardar.

O que a estrutura deixa implícito, e que ainda não é regra:

- Por região: quantidade e total em reais, numa tabela de UF e noutra de Cidade.
- Oportunidades de RSVP: os rótulos visualizações, cliques no botão, contatados, descartados, geradas, conversão e Conversões anteriores.

Os campos dos outros sete relatórios não apareceram. Não definido.

## 12. Interface

Fonte da visita: `https://assessoriavip.com.br/funnel/reports`. Período padrão. Sem movimento. A rota e o componente deste produto ficam não definidos.

Os nove relatórios, na ordem vista, todos abertos:

1. Negócios fechados x perdidos. O período desse relatório é pela entrada no estágio.
2. Por fonte. Sem coluna, agrupamento ou total além do relatório aberto e sem movimento.
3. Por serviço/produto. Sem coluna, agrupamento ou total além do relatório aberto e sem movimento.
4. Por estágio. O período desse relatório é pela criação.
5. Por região. Duas tabelas: UF e Cidade. Cada uma tem quantidade e total em reais. Sem linha de movimento.
6. Por responsável. Sem coluna, agrupamento ou total além do relatório aberto e sem movimento.
7. Por data prevista. Sem coluna, agrupamento ou total além do relatório aberto e sem movimento.
8. Por motivos de descarte. Sem lista de motivos, porque não houve movimento.
9. Oportunidades de RSVP. Gráfico e tabela. Os itens vistos são visualizações, cliques no botão, contatados, descartados, geradas e conversão. Há ainda o rótulo Conversões anteriores. O que esse rótulo lista não foi lido. Nenhum número foi lido. O tipo do gráfico não foi identificado.

Não foi visto outro filtro além do período padrão. Não foi visto o intervalo desse período. Não foi visto número de quantidade, de valor nem de conversão.

## 13. Erros

Falha de carga, período inválido e relatório vazio com mensagem própria não foram vistos. Não definido. Ausência de movimento nesta visita não é erro.

## 14. Critérios de aceite

Estes critérios conferem o registro da visita. Não aprovam RF-075 nem RF-077.

1. O registro lista os nove relatórios da seção 12, abertos, no período padrão e sem movimento.
2. Negócios fechados x perdidos fica com o período pela entrada no estágio.
3. Por estágio fica com o período pela criação.
4. Por região fica com as tabelas UF e Cidade, quantidade e total em reais, sem linha.
5. Oportunidades de RSVP fica com gráfico, tabela e os rótulos da seção 12, inclusive Conversões anteriores, sem número.
6. O registro não acrescenta filtro, coluna, motivo, estágio, tipo de gráfico nem valor que a visita não mostrou.
7. Catálogo financeiro, impressões e a aba Estatísticas ficam fora deste arquivo.

## 15. Cenários de teste

Visita com período padrão e sem movimento: os nove abertos, sem número copiado.

Por região sem linha: as duas tabelas mostram quantidade e total em reais.

Oportunidades de RSVP sem movimento: os rótulos estão visíveis e nenhum valor entra na spec.

Troca de período, outro filtro, relatório fechado, relatório com movimento e duas contas: não definidos. Ficam fora do cenário.

## 16. Dependências

SPEC-001, para a conta autenticada da visita.

SPEC-007 e RF-082, só para separar o dashboard deste catálogo.

O cadastro da oportunidade ainda não tem spec. Sem ele, este catálogo não tem de onde sair um movimento. A estrutura vazia da visita não depende desse cadastro.

O fluxo continua fora do corte de [mvp-scope.md](../mvp-scope.md) enquanto esta spec estiver em rascunho.

## 17. Premissas e pontos não definidos

- RF-075 e RF-077 permanecem Observado. RF-035 permanece Pendente. Nada disso vira regra aprovada por este arquivo.
- Intervalo do período padrão e o modo de alterá-lo.
- Base do período nos sete relatórios em que a visita não a mostrou: por fonte, por serviço/produto, por região, por responsável, por data prevista, por motivos de descarte e Oportunidades de RSVP.
- Fechar ou recolher um relatório.
- Colunas e agrupamentos dos relatórios que a visita mostrou só abertos e sem movimento.
- Estágios que entram em fechados e em perdidos.
- Motivos listados em por motivos de descarte.
- Tipo do gráfico de Oportunidades de RSVP e o conteúdo de Conversões anteriores.
- Texto de vazio, zero e formato do real além da expressão “total em reais” na região.
- Filtros que não sejam o período padrão.
- Exportação, impressão e abertura de um item do relatório.
- Rota deste produto.
- Isolamento entre contas nesta tela.
- Nenhum RF de RF-160 a RF-169. Onde não há RF, a regra fica não definida.
