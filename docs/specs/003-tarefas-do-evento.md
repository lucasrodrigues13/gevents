# 003. Tarefas do evento

**Status: aprovada.** Plano técnico e código dependem de aprovação própria.

## 1. Identificador e nome

SPEC-003. Tarefas do evento.

## 2. Objetivo

A assessoria registra tarefas no evento, muda a situação e consulta as tarefas de todos os seus eventos.

## 3. Requisitos relacionados

RF-061, no corte do escopo. Fora: RF-062, quadro por dia, prazo relativo, checklist, perguntas, comentários, anexos, serviço e lembrete de aniversário.

## 4. Atores e permissões

O User da conta dona do evento cria, lista, edita e muda a situação. Outro User não vê. Visitante e convidado não acessam.

O responsável é a própria conta. Não há atribuição a outra pessoa.

## 5. Pré-condições

Evento da conta, SPEC-002. Tarefa em evento concluído ou cancelado continua não definida.

## 6. Fluxo principal

1. No evento, o usuário informa título e, se quiser, descrição e prazo.
2. A tarefa nasce pendente, responsável a conta atual.
3. A lista do evento e o quadro por situação mostram a tarefa.
4. O usuário edita título, descrição e prazo.
5. Muda a situação entre pendente, em andamento, concluída e atrasada.
6. A lista de todos os eventos da conta mostra a mesma tarefa, com o nome do evento.

## 7. Fluxos alternativos

- Título vazio: não grava.
- Prazo vazio: a tarefa fica sem prazo.
- Evento de outra conta: a lista não abre.
- Conta sem tarefas: as duas listas informam que não há tarefa.
- Não há arrastar entre dias nem filtro por serviço.

## 8. Regras de negócio

- A tarefa pertence a um evento e à conta desse evento.
- Situação inicial: pendente.
- As quatro situações observadas podem ser escolhidas. Não há cálculo automático de atrasada, porque essa passagem não foi vista.
- Prazo é uma data. Não é texto do tipo "6 meses antes".
- Mudar a data do evento não muda o prazo.
- O quadro tem uma coluna por situação, não por dia.
- A lista geral não mostra tarefa de outra conta.

## 9. Validações

Título obrigatório, sem ficar só com espaços. Prazo, se preenchido, é data válida. Situação é uma das quatro. Tamanho máximo de título e descrição não foi definido.

## 10. Estados

Criação vai para pendente. Dali, e das outras três, o usuário pode ir para qualquer uma das quatro: pendente, em andamento, concluída e atrasada.

## 11. Dados necessários

Título, descrição opcional, prazo opcional, situação, conta responsável e evento. Sem checklist, pergunta, comentário, anexo ou serviço.

## 12. Interface

No evento: lista e quadro por situação, com título, prazo, situação e responsável. Inclusão e edição no evento. Fora do evento: lista de todas as tarefas da conta, com o nome do evento, abrindo a tarefa no evento de origem.

## 13. Erros

Validação indica o campo. Item de outra conta ou inexistente fica indisponível, sem revelar conteúdo. Falha ao gravar mantém a situação anterior.

## 14. Critérios de aceite

1. A tarefa criada no evento aparece pendente na lista, no quadro e na lista geral.
2. Sem título, nada é gravado.
3. A situação muda entre as quatro e as três visões acompanham.
4. A lista geral não mistura outra conta nem omite o evento.
5. Alterar a data do evento não altera o prazo.
6. Não existe coluna por dia nem prazo relativo.

## 15. Cenários de teste

Criar com e sem prazo. Passar pelas quatro situações. Dois eventos da mesma conta na lista geral. Segunda conta sem acesso. Editar a data do evento e reler o prazo. Descrição vazia e descrição preenchida.

## 16. Dependências

SPEC-001 e SPEC-002. A contagem da home usa as situações desta spec.

## 17. Premissas e pontos não definidos

Se atrasada deveria ser automática. Remoção. Ordem da lista. Tamanho dos textos. Destaque da semana: a referência mostra, a fronteira da semana não foi vista, então não entra. Tarefa depois de o evento ser encerrado.
