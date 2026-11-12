# 007. Dashboards

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-007. Dashboards.

## 2. Objetivo

Depois do login, a assessoria vê a situação da própria conta. A tela mostra dashboards. O menu fica na lateral e não ocupa o miolo.

## 3. Requisitos relacionados

RF-080, RF-081 e RF-082. Estágios do funil vistos em RF-075. Situações de evento da SPEC-002. DA-044 para o layout.

## 4. Atores e permissões

O `User` vê só números e listas da própria conta. Outro `User` não entra nessa conta. O convidado do RSVP não vê esta tela.

## 5. Pré-condições

Usuário autenticado, SPEC-001. Evento vem da SPEC-002. Oportunidade do funil ainda não tem spec de cadastro. Enquanto isso, o dashboard do funil existe e mostra zero.

## 6. Fluxo principal

1. O login bem-sucedido abre esta tela.
2. A barra lateral lista os módulos do corte: dashboards, perfil, eventos, tarefas, clientes e convidados.
3. O miolo mostra dois blocos.
4. Eventos: quantidade em andamento, quantidade concluída e quantidade cancelada. Abaixo, os eventos em andamento, com nome e data, quando houver data.
5. Funil: quantidade e valor em contato inicial, orçamento enviado, contratado e descartado.
6. Clicar num evento em andamento abre esse evento. Clicar numa quantidade de evento abre a lista já filtrada por aquela situação.

## 7. Fluxos alternativos

- Conta sem evento: as três quantidades ficam em zero e a lista de em andamento diz que não há evento.
- Conta sem oportunidade: as quatro quantidades e os quatro valores ficam em zero.
- Evento sem data: aparece pelo nome, sem data.
- Não há cartão de atalho para "abrir o módulo" no miolo. O módulo continua alcançável pela lateral.

## 8. Regras de negócio

- A tela inicial não repete o painel de menus da plataforma de referência.
- Não existe situação "ativo" além de em andamento. Os exemplos descoberta, orçamento pendente, orçamento aprovado, contrato pendente e contrato assinado não foram vistos. Não entram.
- Os estágios do funil são os quatro da conta de referência: contato inicial, orçamento enviado, contratado e descartado.
- Quantidade é o número de oportunidades da conta naquele estágio. Valor é a soma dos valores informados nesse estágio. Oportunidade sem valor não soma. Essa regra de valor vazio não foi vista na referência e fica marcada na seção 17.
- Cada coluna do funil na referência também mostrava valor. Por isso o valor entra, e não só a quantidade.
- Os números não incluem outra conta.
- Esta tela não cria evento nem oportunidade.

## 9. Validações

Não há formulário. Um identificador de evento na query, se um dia for usado por outra página, não vaza dado de outra conta.

## 10. Estados

A tela não tem estado próprio. Ela reflete a situação do evento e o estágio da oportunidade.

## 11. Dados necessários

Eventos da conta, com situação, nome e data. Oportunidades da conta, com estágio e valor, quando o cadastro existir.

## 12. Interface

Referência visual: Angular Material, DA-044. Vale a estrutura: barra lateral, barra superior com a conta e cartões no miolo. Não valem a paleta de comando, o modo escuro, os gráficos nem os widgets de demonstração.

Sem biblioteca de gráfico neste corte. Quantidade em cartão. Funil em quatro cartões, cada um com quantidade e valor. Lista simples dos eventos em andamento.

Textos: Em andamento, Concluído, Cancelado, Contato inicial, Orçamento enviado, Contratado e Descartado.

## 13. Erros

Falha ao carregar mostra mensagem genérica e não mostra dado parcial de outra conta. Lista vazia não é erro.

## 14. Critérios de aceite

1. O login do `User` abre esta tela, não uma grade de módulos.
2. A lateral contém os módulos do corte e o miolo contém os dois dashboards.
3. Com eventos nas três situações, cada cartão mostra a contagem da própria conta.
4. A lista mostra só em andamento, com nome e data.
5. O funil mostra os quatro estágios vistos, com quantidade e valor.
6. Outra conta não altera nem aparece nesses números.
7. Sem oportunidades, o funil mostra zero nos quatro estágios.
8. Não aparecem estágios de contrato pendente, contrato assinado, descoberta ou orçamento aprovado.

## 15. Cenários de teste

Conta sem dados. Conta com um evento em cada situação e outro em andamento sem data. Duas contas com quantidades diferentes. Clique no evento em andamento abre a SPEC-002. Funil zerado antes de existir cadastro de oportunidade.

## 16. Dependências

SPEC-001 e SPEC-002. O valor e a quantidade do funil passam a mudar quando existir spec de oportunidade. Perfil, tarefa, cliente e convite só aparecem na lateral.

## 17. Premissas e pontos não definidos

- Cadastro, edição, descarte com motivo, formulário público e checagem de data do funil não estão especificados. O dashboard não os inventa.
- Não está definido se o valor vazio entra como zero na soma.
- Moeda e formato do valor não foram fechados. A referência mostra valor, sem a regra de arredondamento.
- Ordem da lista de eventos em andamento não foi definida.
- Não há série temporal nem gráfico neste corte.
