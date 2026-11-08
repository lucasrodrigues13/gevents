# Plano 003. Tarefas do evento

**Status: proposta.** Spec [003-tarefas-do-evento.md](../specs/003-tarefas-do-evento.md). Depende do plano 002.

## Fora deste plano

Prazo relativo, quadro por dia, checklist, pergunta, comentário, anexo, serviço, recálculo, aniversário, destaque da semana, remoção e situação atrasada calculada.

## Camadas

| Camada | O que entra |
|---|---|
| Domain | `Tarefa` dentro do evento. Título obrigatório. Situação inicial pendente. As quatro situações são escolha da conta. |
| Application | Criar, editar, mudar situação, listar por evento e listar por conta. |
| Infrastructure | Mapeamento e uma migration. |
| Web | Lista e quadro na página do evento. Lista geral da conta. |

Tarefa só é criada em evento em andamento. Evento concluído ou cancelado não recebe tarefa neste plano, porque a spec deixou esse caso sem definição.

O responsável gravado é o identificador da conta. Não há seleção de outra pessoa.

## Páginas

Autorização `User`. Outra conta não abre a lista. A lista geral mostra o nome do evento e leva à tarefa dentro dele. O quadro tem uma coluna por situação.

A home do evento, do plano 002, passa a ler as contagens reais de concluídas.

## Persistência

Uma migration. Índice por evento e por conta. Prazo nulo é permitido.

## Testes

Unitário: título vazio e situação fora das quatro. Integração: criar com e sem prazo, percorrer as quatro situações, dois eventos na lista geral, segunda conta sem acesso, mudar a data do evento e ver o prazo intacto, descrição vazia e preenchida.

## Tarefas

1. Entidade e casos de uso.
2. Lista, quadro e lista geral.
3. Ligar a contagem na home do evento.
4. Migration e testes.

## Pendência que permanece

Ordem da lista, tamanho dos textos, remoção e tarefa em evento encerrado.
