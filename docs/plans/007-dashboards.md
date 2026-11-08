# Plano 007. Dashboards

**Status: proposta.** Spec [007-dashboards.md](../specs/007-dashboards.md). Depende dos planos 001 e 002.

## Fora deste plano

Gráfico, biblioteca nova, grade de atalhos no miolo, estágios que não estão na spec e cadastro de oportunidade. Sem oportunidade, os quatro cartões do funil mostram quantidade zero e valor zero.

## Camadas

| Camada | O que entra |
|---|---|
| Domain | Nada novo. As situações do evento já existem. |
| Application | Consulta de contagem e da lista em andamento da conta. Consulta do funil devolve zeros enquanto a entidade não existir. |
| Infrastructure | A consulta usa o mapeamento do evento. Sem migration neste plano. |
| Web | Layout autenticado e a página inicial. |

O layout segue a DA-044: lateral, barra superior e cartões, com AdminLTE 4 sobre Bootstrap. Não copia o tema da Adminmart.

## Páginas

Depois do login, o `User` cai nesta página. A lateral, neste plano, leva a dashboards, perfil, eventos, tarefas, clientes e convidados. O link só aparece quando a página de destino já existir. O miolo tem os dois dashboards.

Cartões de evento: em andamento, concluído e cancelado. A lista abaixo mostra nome e data dos eventos em andamento. O clique abre o evento. O clique na quantidade abre a lista filtrada.

Funil, nos textos da spec: contato inicial, orçamento enviado, contratado e descartado.

## Persistência

Nenhuma, até uma spec futura criar a oportunidade. Aí a consulta deixa de devolver zero e uma migration própria nasce naquele plano.

## Testes

Integração: login abre esta tela, conta vazia, um evento em cada situação, evento sem data, duas contas com números diferentes, funil zerado, ausência dos estágios rejeitados pela spec.

## Tarefas

1. Layout autenticado e troca do destino do login.
2. Dashboard de eventos sobre os dados do plano 002.
3. Dashboard do funil zerado, com os quatro estágios.
4. Testes da spec.

## Pendência que permanece

Ordem da lista, moeda e a soma de valor vazio. Essas três esperam a spec da oportunidade antes de virarem código com dado real.
