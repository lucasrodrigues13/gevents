# Escopo do MVP

**Status: aprovado.** Vale o comportamento das specs 001 a 007. O que a spec deixou de fora continua fora. Observação da referência que a spec não repetiu não virou requisito. A fonte da exploração é [brief.md](brief.md) e a seção 2.7 de [requirements.md](requirements.md).

Entra o que a exploração fechou: tela vista, campos vistos e regra vista. Fica de fora o que parou em menu, rota, rótulo ou formulário não aberto.

## 1. Objetivo

A assessoria entra na própria conta e cai numa tela de dashboards. Dali opera perfil, eventos, tarefas, clientes e convidados até o RSVP por link.

## 2. Público

Assessoria, na conta autenticada, e convidado, que responde sem usuário.

Cliente com login, assistente e fornecedor ficam de fora. A role `User` continua sem ser papel de negócio, RF-040.

## 3. O que a exploração fechou

| Módulo | Entra neste corte | Ficou de fora porque não fechou |
|---|---|---|
| Conta | Cadastro, confirmação, login, bloqueio, senha, logout e bootstrap. SPEC-001 aprovada. | A conta deste corte está fechada na SPEC-001. |
| Dashboards | Primeira tela do `User`. Menu na lateral. Miolo com dashboard de eventos e dashboard do funil. RF-080, RF-081 e RF-082. Referência visual: Angular Material, DA-044. | A grade de menus da referência. Estágios que não foram vistos. Gráfico que exigiria biblioteca nova. |
| Perfil | Nome da empresa, responsável, e-mail somente leitura, CPF ou CNPJ, celular, nascimento e endereço. RF-044. | Não foi vista a divisão do endereço em CEP, rua e cidade. |
| Eventos | Abas em andamento, concluído e cancelado. Busca por nome. Filtros de tipo, período e tags. Card com data, contagem, tags, tarefas e convidados. Criar e editar com os campos do formulário. Concluir e cancelar. Imagem opcional do evento. RF-048 e RF-049. | Aba Estatísticas, agrupar, equipe do dia, impressão, remover e modelo. O miolo do modelo não foi inventariado, então a obrigatoriedade vista na referência não entra. |
| Home do evento | Contagem regressiva, dados do evento, tarefas concluídas e convidados por situação. | Budget, pagamentos, serviços, pendências do mês e equipe. Dependem de módulos que não fecharam. |
| Clientes | Lista com busca, cadastro com nome, telefone e e-mail, vínculo com evento. RF-055. | Portal do cliente e a matriz de permissões. Vieram de flags, sem uso do portal. Último acesso depende desse portal. |
| Tarefas | Do evento e de todos os eventos da conta. Lista e quadro por situação. Título, descrição, prazo em data, situação e responsável da conta. RF-061. | Quadro por dia, prazo relativo, checklist, perguntas, comentários, anexos, serviço, recálculo e aniversário. O recálculo foi um aviso, não uma execução. |
| Convidados | Convite, convidados, grupos da conta, situação, faixa, custo, gênero, celular, observação e transfer com origem. Visão por convite ou por convidado. Busca, filtros e totais. RF-063. | Importação, exportação, impressão, mapa, portaria, QR Code e RSVP automático. |
| RSVP | Link por convite, sem conta. Histórico acessada e respondida. A pessoa informa quem comparece. RF-064. | Formulário público não foi respondido. Link único do evento. Valor "enviada", porque este corte não dispara e-mail. |

## 4. Requisitos incluídos

| ID | Corte |
|---|---|
| RF-001 a RF-034, RF-036 a RF-038, RF-040 a RF-043 | Conta já confirmada. RF-039 não existe. |
| RF-080, RF-081, RF-082 | Tela inicial e os dois dashboards. |
| RF-044 | Perfil, nos campos vistos. |
| RF-048, RF-049 | Lista e cadastro, sem os itens da tabela acima. |
| RF-050 | Só os blocos da home cujo dado vem de evento, tarefa ou convidado. |
| RF-055 | Cadastro e vínculo. Sem RF-056. |
| RF-061 | Lista, quadro por situação e visão de todos os eventos. Sem RF-062. |
| RF-063, RF-064 | Lista completa observada e link por convite. |

## 5. Fora deste corte

RF-045, RF-046, RF-047, RF-051, RF-052, RF-053, RF-054, RF-056, RF-057, RF-058, RF-059, RF-060, RF-062, RF-065, RF-066, RF-067, RF-068, RF-069, RF-070, RF-071, RF-072, RF-073, RF-074, RF-076, RF-077, RF-078 e RF-079. De RF-075 entra só o que RF-082 usa: quantidade e valor nos quatro estágios vistos. O restante do funil continua de fora.

Motivo comum: a exploração não abriu o fluxo por dentro, ou a regra depende de papel diferente de `Admin` e `User`.

## 6. Dependências

1. Conta, SPEC-001.
2. Perfil da mesma conta, sem bloquear o evento.
3. Evento, antes de tarefa, cliente vinculado e convite.
4. Convite, antes do link de RSVP.
5. Arquivo já definido, só para a imagem opcional do evento. Quem vê é a conta dona.

## 7. Riscos e premissas

- O corte e as specs 001 a 007 estão aprovados. O código segue a spec, sem plano técnico separado.
- Campos obrigatórios que a exploração não marcou continuam pendentes em cada spec.
- Modelo obrigatório, assistente, portal do cliente, fornecedor, parcela, agenda e financeiro não entram neste corte.
- O funil completo também não. O dashboard usa os quatro estágios vistos. Cadastro da oportunidade, descarte com motivo, formulário de lead e checagem de data continuam sem spec.
- RSVP público continua o maior buraco: sabe-se o link, a ausência de conta e as três situações. Não se sabe o formulário que a referência mostra.
- Não há backup nesta versão.

## 8. Conclusão

O MVP está pronto quando:

1. A assessoria entra, vê os dashboards de eventos e do funil, e grava o perfil.
2. Cria um evento, filtra a própria lista e conclui ou cancela.
3. Vê na home a contagem de tarefas e de convidados.
4. Cria tarefas no evento e as encontra também na lista de todos os eventos.
5. Cadastra um cliente e vincula ao evento.
6. Monta convite e convidados com os campos vistos e copia o link.
7. Alguém sem conta informa quem comparece, e a conta dona vê a situação.
8. Outra conta não vê esses dados.

As specs estão aprovadas. Os planos técnicos estão em [plans/README.md](plans/README.md), ainda como proposta. A implementação de cada uma começa depois da aprovação do plano correspondente.

## 9. Specs

| Spec | Status |
|---|---|
| [001-autenticacao-e-conta.md](specs/001-autenticacao-e-conta.md) | Aprovada |
| [002-eventos.md](specs/002-eventos.md) | Aprovada |
| [003-tarefas-do-evento.md](specs/003-tarefas-do-evento.md) | Aprovada |
| [004-convites-e-rsvp.md](specs/004-convites-e-rsvp.md) | Aprovada |
| [005-perfil-da-assessoria.md](specs/005-perfil-da-assessoria.md) | Aprovada |
| [006-clientes.md](specs/006-clientes.md) | Aprovada |
| [007-dashboards.md](specs/007-dashboards.md) | Aprovada |
| [008-cadastro-completo-de-evento.md](specs/008-cadastro-completo-de-evento.md) | Aprovada. Incremento sobre a SPEC-002. |
| [009-cadastro-completo-de-cliente.md](specs/009-cadastro-completo-de-cliente.md) | Aprovada. Incremento sobre a SPEC-006. |
| [010-convites-convidados-completo.md](specs/010-convites-convidados-completo.md) | Aprovada. Incremento sobre a SPEC-004. |
