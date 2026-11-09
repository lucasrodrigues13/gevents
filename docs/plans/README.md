# Planos técnicos

**Status: proposta**, exceto os planos 008, 009 e 010, que estão aprovados.

A spec correspondente já está aprovada. O plano não acrescenta comportamento. O que a spec marcou como não definido fica de fora da implementação.

Ordem, por dependência:

1. [001-autenticacao-e-conta.md](001-autenticacao-e-conta.md) cria a solution, a conta e o banco.
2. [005-perfil-da-assessoria.md](005-perfil-da-assessoria.md) e [002-eventos.md](002-eventos.md) dependem só da conta.
3. [003-tarefas-do-evento.md](003-tarefas-do-evento.md), [004-convites-e-rsvp.md](004-convites-e-rsvp.md) e [006-clientes.md](006-clientes.md) dependem do evento.
4. [007-dashboards.md](007-dashboards.md) depende do evento. O funil aparece zerado até existir spec de oportunidade.
5. [008-cadastro-completo-de-evento.md](008-cadastro-completo-de-evento.md) depende do evento já implementado.
6. [009-cadastro-completo-de-cliente.md](009-cadastro-completo-de-cliente.md) depende do cliente já implementado e da consulta de CEP do plano 008.
7. [010-convites-convidados-completo.md](010-convites-convidados-completo.md) depende do convite já implementado.

A migration de cada plano entra depois da anterior. Duas migrations não nascem em paralelo no mesmo modelo. Os planos 008, 009 e 010 estão aprovados e autorizam a implementação nessa ordem.
