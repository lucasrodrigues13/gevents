# Escopo do MVP

**Status: aprovado.** Vale o comportamento das specs **001 a 028**. O que a spec deixou de fora continua fora. Peças de marketplace (RF-079), assistente (RF-045), login de fornecedor (RF-060) e financeiro da empresa (RF-076) ficam fora deste corte. A fonte é [brief.md](brief.md), [inventario-referencia.md](inventario-referencia.md) e a seção 2.7 de [requirements.md](requirements.md).

## 1. Objetivo

O organizador entra na própria conta, vê dashboards e opera gestão de eventos, convidados, modelos, funil, agenda, serviços e fornecedores, com CRUD fechado nas specs aprovadas.

## 2. Público e roles

Três roles Identity:

| Role | Significado |
|---|---|
| `Admin` | Admin geral: tudo e todas as contas. |
| `Organizer` | Comprador do sistema; organiza eventos e dados do próprio tenant. Cadastro público cria `Organizer`. |
| `Client` | Subusuário sempre vinculado a um `Organizer`; acesso limitado às flags concedidas (convidados, tarefas, anexos, parcelas, RSVP, mapa, contrato). |

Convidado responde RSVP por link, sem conta. Assistente e fornecedor com login ficam **fora** deste corte.

Contato de negócio chamado “cliente” (cadastro na lista de clientes) não é a role `Client`; a role nasce quando o organizador concede login de plataforma.

## 3. Agrupamento de menu (produto)

Navegação lateral alinhada à Assessoria VIP. O agente de frontend implementa a casca; este documento fixa os grupos pretendidos:

| Grupo | Itens / specs |
|---|---|
| Início | Dashboards (SPEC-007) |
| Gestão de eventos | Lista e cadastro (002, 008), estatísticas e resumo (011), home do evento, serviços/fornecedores no evento (018), tarefas (003), clientes do evento (006, 009) |
| Convidados | Convites, RSVP, grupos, transfer (004, 010) |
| Modelos | Evento (012), briefing (013), roteiro (014), cronograma (015), inspirações (016), mensagens (017) |
| Funil de vendas | Oportunidades (020), oportunidades de RSVP (021), contratos (022), tarefas comerciais (023), relatórios (024), orçamentos (025), modelos de serviço/orçamento/contrato (026–028) |
| Agenda | Minha agenda (019) |
| Serviços e fornecedores | Catálogo da conta (018) |
| Conta | Perfil do organizador (005), autenticação (001) |

Fora do menu deste corte: assistentes, financeiro da empresa, VIP Pay / Casar.com / loja / MeLink (RF-079).

## 4. O que entra (Must)

| Módulo | Entra | Specs |
|---|---|---|
| Conta | Cadastro público → `Organizer`, confirmação, login, lockout, senha, logout, bootstrap das três roles | 001 |
| Dashboards | Eventos e funil (quatro estágios) | 007 |
| Perfil | Dados da empresa do Organizer | 005 |
| Eventos | Lista, abas, CRUD, home, estatísticas e resumo | 002, 008, 011 |
| Clientes (contato) | Lista, cadastro completo, vínculo ao evento; login `Client` com flags | 006, 009 |
| Tarefas do evento | Lista, quadro por situação, CRUD | 003 |
| Convidados / RSVP | Convite, convidados, grupos, transfer, link | 004, 010 |
| Modelos | Seis bibliotecas com CRUD | 012–017 |
| Serviços e fornecedores | Catálogo, serviço no evento, negociação, budget, parcelas | 018 |
| Agenda | Visões e CRUD de compromisso | 019 |
| Funil | Oportunidades, RSVP, contratos, tarefas, relatórios, orçamentos e modelos | 020–028 |

## 5. Requisitos incluídos

| ID | Corte |
|---|---|
| RF-001 a RF-034, RF-036 a RF-038, RF-040 a RF-043 | Conta e autorização com três roles. |
| RF-080 a RF-082 | Tela inicial e dashboards. |
| RF-083 a RF-086 | Tenant do Organizer; vínculo e permissões do Client. |
| RF-044, RF-048 a RF-050, RF-053 a RF-059, RF-061, RF-063, RF-064, RF-068 a RF-072, RF-074, RF-075, RF-077, RF-078 | Domínio coberto pelas specs 002–028 no que cada spec descreve. |
| RF-056 | Flags de acesso do Client, sem matriz inventada além do inventário. |

## 6. Fora deste corte

| ID | Motivo |
|---|---|
| RF-045 | Assistente fora. |
| RF-046, RF-047 | Busca global e notificações sem spec. |
| RF-051, RF-052 | Equipe do dia e agrupar eventos. |
| RF-060 | Login / link de fornecedor fora. |
| RF-062, RF-065 | Redatar tarefas; RSVP automático em volume. |
| RF-066, RF-067 | Mapa operacional completo e portaria (além do campo mesa já nas specs de convidados). |
| RF-073 | Caixa global de anexos ainda Observado. |
| RF-076 | Financeiro da assessoria (caixa da empresa). |
| RF-079 | Marketplace / peças da plataforma de referência. |

## 7. Dependências

1. Conta e roles (SPEC-001) antes de qualquer módulo autenticado.
2. Perfil não bloqueia evento.
3. Modelo de evento (012) antes de criar evento com checklist (008).
4. Evento antes de tarefa, convidado, serviço no evento e Client vinculado.
5. Funil (020+) independente do evento, mas agendas e checagem de data cruzam com 019.
6. Arquivo técnico já definido: imagem do evento, anexos de fornecedor/serviço conforme 018.

## 8. Riscos e premissas

- Specs 001–028 estão **aprovadas**; implementação segue a spec, sem plano técnico separado.
- Pendências explícitas em cada spec §17 não bloqueiam o restante do CRUD fechado.
- Canal de e-mail continua Gmail SMTP; disparo em massa (RSVP automático) fora.
- Sem backup nesta versão.

## 9. Conclusão

O MVP expandido está pronto para implementação quando:

1. Organizer entra, vê dashboards e grava o perfil.
2. Mantém modelos, cria eventos, estatísticas e home.
3. Opera tarefas, clientes (contato), convidados e RSVP.
4. Opera serviços, fornecedores, negociação e parcelas no evento.
5. Opera agenda e funil completo (oportunidades até modelos).
6. Pode criar `Client` vinculado com flags; Admin vê todas as contas.
7. Outro Organizer não vê os dados do tenant.

## 10. Specs

| Spec | Status |
|---|---|
| [001-autenticacao-e-conta.md](specs/001-autenticacao-e-conta.md) | Aprovada |
| [002-eventos.md](specs/002-eventos.md) | Aprovada |
| [003-tarefas-do-evento.md](specs/003-tarefas-do-evento.md) | Aprovada |
| [004-convites-e-rsvp.md](specs/004-convites-e-rsvp.md) | Aprovada |
| [005-perfil-da-assessoria.md](specs/005-perfil-da-assessoria.md) | Aprovada |
| [006-clientes.md](specs/006-clientes.md) | Aprovada |
| [007-dashboards.md](specs/007-dashboards.md) | Aprovada |
| [008-cadastro-completo-de-evento.md](specs/008-cadastro-completo-de-evento.md) | Aprovada |
| [009-cadastro-completo-de-cliente.md](specs/009-cadastro-completo-de-cliente.md) | Aprovada |
| [010-convites-convidados-completo.md](specs/010-convites-convidados-completo.md) | Aprovada |
| [011-estatisticas-e-resumo-dos-eventos.md](specs/011-estatisticas-e-resumo-dos-eventos.md) | Aprovada |
| [012-modelos-de-evento.md](specs/012-modelos-de-evento.md) | Aprovada |
| [013-modelos-de-briefing.md](specs/013-modelos-de-briefing.md) | Aprovada |
| [014-modelos-de-roteiro.md](specs/014-modelos-de-roteiro.md) | Aprovada |
| [015-modelos-de-cronograma.md](specs/015-modelos-de-cronograma.md) | Aprovada |
| [016-modelos-de-inspiracoes.md](specs/016-modelos-de-inspiracoes.md) | Aprovada |
| [017-modelos-de-mensagens.md](specs/017-modelos-de-mensagens.md) | Aprovada |
| [018-servicos-e-fornecedores.md](specs/018-servicos-e-fornecedores.md) | Aprovada |
| [019-minha-agenda.md](specs/019-minha-agenda.md) | Aprovada |
| [020-oportunidades-do-funil.md](specs/020-oportunidades-do-funil.md) | Aprovada |
| [021-oportunidades-de-rsvp.md](specs/021-oportunidades-de-rsvp.md) | Aprovada |
| [022-contratos-do-funil.md](specs/022-contratos-do-funil.md) | Aprovada |
| [023-tarefas-do-funil.md](specs/023-tarefas-do-funil.md) | Aprovada |
| [024-relatorios-do-funil.md](specs/024-relatorios-do-funil.md) | Aprovada |
| [025-orcamentos-do-funil.md](specs/025-orcamentos-do-funil.md) | Aprovada |
| [026-modelos-de-servicos-do-funil.md](specs/026-modelos-de-servicos-do-funil.md) | Aprovada |
| [027-modelo-de-orcamento-do-funil.md](specs/027-modelo-de-orcamento-do-funil.md) | Aprovada |
| [028-modelo-de-contrato-do-funil.md](specs/028-modelo-de-contrato-do-funil.md) | Aprovada |
