# 020. Oportunidades do funil

**Status: rascunho.** Não autoriza implementação. RF-075 e RF-077 continuam Observado. No brief o funil está fora do MVP; esta spec não trata isso como aprovação e não altera a SPEC-007.

Fonte observada: quadro de oportunidades da referência, com as quatro colunas em 0 e R$ 0,00. As ações gerenciar estágios e adicionar oportunidade foram vistas e não usadas.

## 1. Identificador e nome

SPEC-020. Oportunidades do funil.

## 2. Objetivo

A conta autenticada vê o quadro das oportunidades nos quatro estágios já conhecidos, com quantidade e valor em cada coluna. O cadastro da oportunidade e a edição dos estágios ficam de fora.

## 3. Requisitos relacionados

RF-075, Observado. O quadro, a quantidade, o valor, o filtro e a menção a estágios configuráveis vêm desse RF. Descarte com motivo, exportação, formulário público de lead, checagem de data, contratos, orçamentos e tarefas de reunião, ligação, e-mail ou tarefa continuam no texto do RF e fora deste fluxo: esta visita não os abriu.

RF-082, Definido, cobre só o dashboard da SPEC-007. Esta spec não reescreve essa tela.

RF-077, Observado. Relatórios do funil não entram aqui.

## 4. Atores e permissões

O User autenticado vê o quadro da própria conta. Outro User não vê essas oportunidades. Quem responderia um formulário público de lead não usa esta tela. RF-025 e RF-028.

## 5. Pré-condições

Conta autenticada, SPEC-001. O caminho no produto até este quadro não está definido. A barra lateral da SPEC-007 não ganha item nesta spec.

Não há spec de cadastro da oportunidade. O quadro visto estava vazio, o mesmo caso em que a SPEC-007 mostra o funil zerado.

## 6. Fluxo principal

1. A conta está no quadro de oportunidades.
2. Vê quatro colunas, nesta ordem: Contato inicial, Enviado orçamento, Contratado e Descartado.
3. Cada coluna mostra a quantidade de oportunidades da conta naquele estágio e o valor correspondente.
4. Sem oportunidade, cada coluna mostra 0 e R$ 0,00.
5. A tela mostra filtro por responsável, tipo de evento, data prevista e ordenação.
6. A tela mostra as ações gerenciar estágios e adicionar oportunidade. Usá-las fica fora deste fluxo.

## 7. Fluxos alternativos

- Conta sem oportunidade: as quatro colunas ficam em 0 e R$ 0,00. Foi o único quadro visto.
- Acionar gerenciar estágios ou adicionar oportunidade: não definido. O formulário e a edição de estágios não foram abertos.
- Aplicar filtro ou ordenação: não definido. Os controles foram vistos; o resultado de usá-los, não.
- Oportunidade já gravada, cartão na coluna ou mudança de estágio: não visto. Não definido.

## 8. Regras de negócio

- Os estágios deste quadro são os quatro de RF-075. Esta spec não cria estágio. RF-082 segue valendo para o dashboard: os mesmos quatro estágios, sem estágio novo.
- Quantidade é o número de oportunidades da conta na coluna. Valor é a soma dos valores da coluna, RF-075. Sem oportunidade, o visto foi 0 e R$ 0,00.
- O rótulo da segunda coluna, neste quadro, foi Enviado orçamento. A SPEC-007 escreve Orçamento enviado no dashboard. Esta spec mantém o texto do dashboard. Unificar os dois rótulos: não definido.
- O filtro de RF-075, nesta visita, são os controles responsável, tipo de evento, data prevista e ordenação. As opções de cada controle não foram vistas.
- Gerenciar estágios e adicionar oportunidade aparecem na tela e não foram usadas. O que cada uma grava: não definido.
- O quadro mostra só oportunidades da própria conta. RF-025 e RF-028.
- Descarte com motivo, exportação, formulário público de lead, checagem de data, contratos, orçamentos e tarefas comerciais permanecem no texto de RF-075 e fora deste fluxo.
- Relatório do funil permanece em RF-077 e fora deste fluxo.

## 9. Validações

Não há formulário nesta spec. Não há campo para validar.

## 10. Estados

O único estado visto é o quadro vazio: as quatro colunas existem, cada uma com quantidade 0 e valor R$ 0,00. Passagem de uma coluna para outra: não definida.

## 11. Dados necessários

Conta autenticada. Oportunidade com estágio e valor, quando um cadastro existir. Esse cadastro não foi visto. Responsável, tipo de evento e data prevista aparecem como nome de filtro, sem lista de opções e sem ficha da oportunidade.

## 12. Interface

Quatro colunas. Em cada uma, quantidade e valor. No vazio, 0 e R$ 0,00. Filtros: responsável, tipo de evento, data prevista e ordenação. Ações: gerenciar estágios e adicionar oportunidade.

O miolo do dashboard continua o da SPEC-007. Esta tela não traz gráfico.

## 13. Erros

A mensagem de falha ao carregar este quadro não foi vista: não definida. Dado de outra conta não aparece. RF-028.

## 14. Critérios de aceite

1. O quadro mostra as colunas Contato inicial, Enviado orçamento, Contratado e Descartado.
2. Sem oportunidade, cada coluna mostra 0 e R$ 0,00.
3. A tela mostra os filtros responsável, tipo de evento, data prevista e ordenação.
4. A tela mostra gerenciar estágios e adicionar oportunidade.
5. O dashboard da SPEC-007 segue com os rótulos já aprovados, entre eles Orçamento enviado, e com o funil zerado enquanto não houver cadastro de oportunidade.
6. Outra conta não aparece no quadro.
7. O quadro não mostra coluna de contrato pendente, contrato assinado, descoberta nem orçamento aprovado.

## 15. Cenários de teste

Conta sem oportunidade: quatro colunas em 0 e R$ 0,00, com os quatro filtros e as duas ações visíveis. Duas contas: cada uma vê só o próprio quadro. O dashboard da SPEC-007 permanece como já especificado, com o funil zerado. Abrir o formulário ou editar estágios fica fora do cenário.

## 16. Dependências

SPEC-001. A SPEC-007 permanece aprovada. O funil zerado dos dois lugares é a conta sem oportunidade. Quantidade e valor do dashboard só podem deixar de ser zero quando existir spec de cadastro; esta não é essa spec.

## 17. Premissas e pontos não definidos

- Esta spec é rascunho. RF-075 e RF-077 não passam a Definido. O funil não entra no MVP por este arquivo.
- O caminho da conta até o quadro: não definido. A lateral da SPEC-007 permanece a da spec aprovada.
- Formulário de adicionar oportunidade: não aberto. Campos, obrigatoriedade e o que ocorre ao salvar: não definidos.
- Gerenciar estágios: não aberto. Criar, renomear, reordenar ou remover estágio: não definido.
- Opções de responsável, tipo de evento, data prevista e ordenação: não definidas. Não se viu se a data prevista é um dia ou um intervalo.
- Conteúdo do cartão, arrastar entre colunas e edição da oportunidade: não vistos.
- Valor de oportunidade gravada, arredondamento e oportunidade sem valor continuam em aberto na SPEC-007. Esta visita viu R$ 0,00 só nas colunas vazias.
- Descarte com motivo, exportação, formulário público, checagem de data, contratos, orçamentos, tarefas comerciais e relatórios do funil: não definidos aqui.
