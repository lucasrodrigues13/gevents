# 002. Eventos

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-002. Eventos.

## 2. Objetivo

A assessoria cria um evento com os dados vistos no formulário, encontra-o na própria lista e acompanha, na home, tarefas e convidados.

## 3. Requisitos relacionados

RF-048, RF-049 e a parte de RF-050 coberta por evento, tarefa e convidado.

Fora: aba Estatísticas (RF-078), agrupar (RF-052), equipe do dia (RF-051), impressão, remover, modelo (RF-053) e os blocos de budget, pagamento e serviço.

## 4. Atores e permissões

| Ator | Pode |
|---|---|
| Organizer | Criar, listar, filtrar, abrir, editar, concluir e cancelar os próprios eventos. Ver a imagem do próprio evento. |
| Outro Organizer | Não vê evento, home nem imagem. |
| Admin | Admin geral; fora do miolo desta spec de operação da conta. |
| Client | Não opera este cadastro. Acesso limitado a evento/módulo, se existir, fica em RF-056. |
| Visitante | Não acessa. |

## 5. Pré-condições

`Organizer` autenticado, SPEC-001. Não há biblioteca de modelos.

## 6. Fluxo principal

1. O Organizer informa o nome e, se quiser, os demais dados da seção 11.
2. O sistema grava o evento na conta do Organizer, em andamento.

3. A aba em andamento mostra o card.
4. Busca por nome e filtros de tipo, período e tag reduzem a lista.
5. Ao abrir, a home mostra os dados, a contagem regressiva se houver data, as tarefas concluídas e os convidados por situação.
6. Editar grava os mesmos campos.
7. Concluir ou cancelar, depois de confirmação, tira o evento de em andamento.

## 7. Fluxos alternativos

- Nome vazio: não grava.
- Lista vazia ou busca sem resultado: a tela diz que não há evento para mostrar.
- Evento de outra conta ou inexistente: não mostra conteúdo nem revela se existe.
- Sem data: não há contagem regressiva. O filtro por período não inclui esse evento.
- Sem tarefas ou convidados: a home mostra zero.
- Mudar a data não recalcula tarefa nem altera RSVP.
- Imagem ausente, grande demais ou de tipo não aceito: o evento pode ser gravado sem imagem, e a imagem rejeitada é informada. Limites de RF de arquivo: imagem até 5 MB, JPG, JPEG, PNG ou WebP.

## 8. Regras de negócio

- O evento pertence ao Organizer autenticado.

- Situação inicial: em andamento.
- Encerrar é concluir ou cancelar.
- Modelo não é pedido. A obrigatoriedade da referência fica de fora junto com RF-053.
- Tags são textos do evento. Não há cadastro separado de tags. Exemplos vistos: assessoria completa, parcial e final.
- Tipos vistos, e únicos neste corte: casamento, 15 anos, aniversário, corporativo e formatura.
- A home não inventa indicador de módulo que está fora do corte.
- Remover foi visto no card e não entra. A consequência da exclusão não foi vista.

## 9. Validações

- Nome obrigatório, sem ficar só com espaços.
- Data e hora, se preenchidas, são válidas.
- Orçamento estimado e número estimado de convidados, se preenchidos, são números maiores ou iguais a zero.
- E-mail do casal, se preenchido, tem formato válido.
- Tipo, se preenchido, é um dos cinco.
- Imagem segue a validação já definida para arquivo.
- Tamanho máximo dos textos: não definido.

## 10. Estados

| De | Para |
|---|---|
| — | Em andamento |
| Em andamento | Concluído |
| Em andamento | Cancelado |

Reabrir, editar depois de encerrar e aceitar tarefa ou RSVP depois de encerrar não estão definidos.

## 11. Dados necessários

Nome, situação, data, hora, tipo, tags, casal (nome, papel, e-mail, celular), orçamento estimado, número estimado de convidados, local da cerimônia, local da recepção e imagem opcional.

O card mostra nome, situação, data, contagem regressiva, tags e as contagens de tarefas concluídas e de convidados confirmados quando esses módulos existirem.

O casal do formulário não cria cliente. O vínculo de cliente é a SPEC-006.

## 12. Interface

Três abas, busca por nome, filtros de tipo, período e tag. Formulário com os dados da seção 11. Home com dados, contagem regressiva, resumo de tarefas e resumo de convidados, mais os links para SPEC-003, SPEC-004 e SPEC-006. Concluir e cancelar só em andamento.

## 13. Erros

Campo inválido permanece no formulário. Evento inacessível usa a mensagem de indisponível. Falha ao gravar mantém o dado anterior. Imagem inválida não apaga o restante do evento.

## 14. Critérios de aceite

1. Criar com nome coloca o evento em andamento, na própria conta.
2. Sem nome, nada é gravado.
3. Busca por nome, tipo, período e tag encontram só eventos da conta que batem o critério.
4. Outra conta não vê o evento nem a imagem.
5. A home mostra zero ou as contagens reais de tarefas e convidados.
6. Concluir e cancelar mudam de aba depois da confirmação. Desistir não muda.
7. O formulário não pede modelo.
8. Mudar a data não altera prazo de tarefa nem link de RSVP.

## 15. Cenários de teste

Criar só com nome. Criar com todos os campos e uma imagem válida. Rejeitar imagem acima de 5 MB e arquivo que não é imagem. Filtrar por cada tipo e por um período que exclui evento sem data. Duas contas isoladas. Concluir, cancelar e desistir. Home com e sem tarefas e convidados.

## 16. Dependências

SPEC-001. Contagens dependem de SPEC-003 e SPEC-004. Imagem usa as regras de arquivo já confirmadas. SPEC-006 vincula cliente sem ser obrigatória para criar o evento.

## 17. Premissas e pontos não definidos

Tamanho máximo dos textos. Edição após concluir ou cancelar. Exclusão. Código do evento, visto na home da referência e sem regra de formação. Filtros de assistente e equipe. Impressão da lista.
