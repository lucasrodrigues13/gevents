# Plano 002. Eventos

**Status: proposta.** Spec [002-eventos.md](../specs/002-eventos.md). Depende do plano 001 aprovado e implementado.

## Fora deste plano

Modelo, estatísticas, agrupar, equipe, impressão, remover, budget, pagamento, serviço, código do evento e edição depois de concluir ou cancelar. Reabrir evento não entra.

## Camadas

| Camada | O que entra |
|---|---|
| Domain | `Evento`, situação, tipo e a regra de nome obrigatório. Situação inicial em andamento. Concluir e cancelar só a partir de em andamento. |
| Application | Casos de uso: criar, editar, concluir, cancelar, listar e obter. O usuário autenticado entra por uma abstração de conta, sem referência ao Identity. |
| Infrastructure | Mapeamento, migration, armazenamento da imagem no volume e metadados no PostgreSQL. |
| Web | Páginas Razor. PageModel não usa `DbContext`. |

A imagem usa o contrato de arquivo já definido: até 5 MB, JPG, JPEG, PNG ou WebP, validação de assinatura, gravação temporária e movimento. Se o metadado falhar, o arquivo é apagado no mesmo fluxo. Sem job de limpeza.

## Entidade

`Evento` guarda a conta dona, nome, situação, data, hora, tipo, tags, casal, orçamento estimado, número estimado de convidados, locais e o identificador opcional do arquivo. Tipo só aceita os cinco valores da spec. Tags são textos no próprio evento.

Conta diferente não lê nem altera. A resposta não revela se o evento existe.

## Páginas

| Página | Autorização |
|---|---|
| Lista, com abas, busca por nome e filtros de tipo, período e tag | `User` |
| Criar e editar | `User`, só evento em andamento na edição |
| Home do evento, com contagem regressiva, tarefas e convidados | `User` |

Concluir e cancelar pedem confirmação. Enquanto as specs 003 e 004 não estiverem implementadas, as contagens da home ficam em zero por ausência de dados, não por número fixo escondido.

Mudar a data não chama tarefa nem RSVP.

## Persistência

Uma migration para evento, tags e metadados de arquivo. Índice por conta e situação. Arquivo não fica no banco.

## Testes

Unitário: nome vazio, transição ilegal, tipo fora da lista. Integração: criar, buscar, filtrar, isolar duas contas, concluir, cancelar, desistir, imagem válida, imagem grande, arquivo que não é imagem, home sem tarefas e sem convidados, data alterada sem efeito colateral.

## Tarefas

1. Entidade, casos de uso e páginas de lista, criação, edição e home.
2. Concluir e cancelar com confirmação.
3. Imagem opcional no fluxo de arquivo já definido.
4. Migration e testes da spec.

## Pendência que permanece

Tamanho máximo dos textos. Evento encerrado não ganha regra nova de edição.
