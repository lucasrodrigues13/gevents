# Planos técnicos

**Status: proposta.** Nenhum plano está aprovado. Não autorizam código.

A spec correspondente já está aprovada. O plano não acrescenta comportamento. O que a spec marcou como não definido fica de fora da implementação.

Ordem, por dependência:

1. [001-autenticacao-e-conta.md](001-autenticacao-e-conta.md) cria a solution, a conta e o banco.
2. [005-perfil-da-assessoria.md](005-perfil-da-assessoria.md) e [002-eventos.md](002-eventos.md) dependem só da conta.
3. [003-tarefas-do-evento.md](003-tarefas-do-evento.md), [004-convites-e-rsvp.md](004-convites-e-rsvp.md) e [006-clientes.md](006-clientes.md) dependem do evento.
4. [007-dashboards.md](007-dashboards.md) depende do evento. O funil aparece zerado até existir spec de oportunidade.

A migration de cada plano entra depois da anterior. Duas migrations não nascem em paralelo no mesmo modelo.
