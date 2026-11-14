# 012. Modelos de evento

**Status: aprovada.** O código segue esta spec. Fonte: [inventario-referencia.md](../inventario-referencia.md) §4 e §1.3. Aplicar o modelo na criação do evento permanece na SPEC-008.

## 1. Identificador e nome

SPEC-012. Modelos de evento.

## 2. Objetivo

O Organizer mantém a biblioteca de modelos de evento: listar, criar com Nome e Tipo, editar nas abas do editor, clonar e excluir. O modelo alimenta a criação do evento na SPEC-008.

## 3. Requisitos relacionados

RF-053. Aplicação do modelo na criação do evento: SPEC-008 / RF-049.

Fora: RF-079 (VIP Store / loja de modelos). Bibliotecas de briefing, roteiro, cronograma, inspiração e mensagem (SPEC-013 a SPEC-017, RF-054). Assistente (RF-045). Portal do cliente além das flags de permissão do modelo (RF-056).

## 4. Atores e permissões

| Ator | Pode |
|---|---|
| Organizer | CRUD completo dos modelos da própria conta: listar, criar, editar, clonar, excluir. |
| Client | Não abre a biblioteca. As flags da aba Permissões do modelo definem o que o Client vê no evento depois da criação (RF-056); o Client não edita o modelo. |
| Admin | Acessa e mantém modelos de qualquer conta. |
| Visitante | Não acessa. |

Assistente e fornecedor com login ficam fora deste corte.

## 5. Pré-condições

Conta autenticada, SPEC-001. A conta pode nascer sem modelo próprio; criar evento na SPEC-008 exige pelo menos um modelo disponível na conta.

## 6. Fluxo principal

1. O Organizer abre a biblioteca de modelos de evento.
2. A lista mostra os modelos da conta, cada um com nome e tipo.
3. Cria um modelo informando Nome e Tipo e salva.
4. Abre o editor do modelo.
5. Nas abas, configura Checklist (serviços com tarefas, fornecedores, alinhamentos, comparativos e valor estimado), Tarefas, Briefings, Roteiros, Inspirações, RSVP e Mensagens, e Permissões.
6. Salva as alterações.
7. Pode clonar um modelo existente, gerando cópia editável na mesma conta.
8. Pode excluir um modelo da conta, com confirmação.

## 7. Fluxos alternativos

- Biblioteca sem modelo: lista vazia e ação de criar.
- Nome ou Tipo ausentes na criação: não grava.
- Modelo de outra conta: Organizer não vê; Admin sim.
- Excluir modelo já usado em eventos existentes: o modelo some da biblioteca; eventos já criados mantêm os dados copiados na criação (não reaplicam o modelo).
- Busca por nome na biblioteca: reduz a lista.
- VIP Store / loja: fora (RF-079).

## 8. Regras de negócio

- Tipos de modelo: 15 anos, casamento, aniversário, corporativo, formatura. RF-053.
- Criar pede Nome e Tipo. RF-053 / inventário §4.
- O editor tem as abas Checklist, Tarefas, Briefings, Roteiros, Inspirações, RSVP e Mensagens, e Permissões. RF-053.
- Na Checklist, cada serviço lista tarefas, fornecedores, alinhamentos, comparativos e valor estimado. RF-053.
- Clonar gera novo modelo na conta, com o mesmo conteúdo editável, sem alterar o original.
- Excluir remove o modelo da biblioteca da conta. Não apaga eventos já criados.
- Aplicar o modelo na criação do evento (tipo, serviços, tarefas) permanece na SPEC-008.
- Modelos pertencem à conta do Organizer. Isolamento entre contas.
- A aba Permissões configura flags no estilo RF-056 (o que o Client vê/edita no evento): gerais, planejamento, tarefas, serviços e fornecedores, convites. Detalhe fino de cada flag fora do miolo desta spec fica na seção 17 se ainda não fechado.
- Prazo de tarefa no modelo pode ser relativo ("N dias antes"); no evento, a SPEC-003 continua com data concreta após a aplicação.

## 9. Validações

Nome obrigatório, sem só espaços. Tipo obrigatório, um dos cinco tipos. Valor estimado, se informado, maior ou igual a zero. Modelo inexistente ou de outra conta: não grava e não revela dado.

## 10. Estados

Modelo cadastrado na conta. Sem máquina de situação adicional (não há "publicado" / "rascunho" separado além de existir ou não na biblioteca).

## 11. Dados necessários

Criar: Nome, Tipo.

Lista: nome, tipo.

Checklist por serviço: nome do serviço, tarefas, fornecedores, alinhamentos, comparativos, valor estimado.

Demais abas: vínculos ou conteúdo de briefing, roteiro, inspiração, RSVP/mensagem, e grupos de permissão do Client.

Clonar: origem do modelo. Excluir: identificador do modelo.

## 12. Interface

Biblioteca com busca, botão Criar modelo e cards com nome e tipo. Formulário de criação com Nome e Tipo. Editor com Clonar modelo e as sete abas. Confirmação ao excluir. Sem VIP Store neste produto.

## 13. Erros

Campo inválido permanece no formulário. Modelo de outra conta não revela dado. Falha ao salvar mantém o conteúdo anterior. Exclusão sem confirmação não ocorre.

## 14. Critérios de aceite

1. A biblioteca lista só modelos da conta do Organizer (Admin vê a conta escolhida).
2. Criar com Nome e Tipo grava o modelo; sem um dos dois, não grava.
3. O editor mostra as abas Checklist, Tarefas, Briefings, Roteiros, Inspirações, RSVP e Mensagens, e Permissões.
4. Na checklist, o serviço mostra tarefas, fornecedores, alinhamentos, comparativos e valor estimado.
5. Clonar gera cópia editável na mesma conta.
6. Excluir remove o modelo da biblioteca e não apaga eventos já criados.
7. A criação do evento continua a da SPEC-008, consumindo o modelo.
8. Outra conta não vê o modelo.

## 15. Cenários de teste

Criar modelo Casamento com nome. Abrir checklist e gravar um serviço com valor estimado. Clonar e conferir dois itens na lista. Excluir a cópia. Criar evento pela SPEC-008 com o modelo restante. Conta B não vê o modelo da conta A.

## 16. Dependências

SPEC-001. SPEC-008 consome o modelo na criação. SPEC-003 para tarefas do evento após aplicar. SPEC-013 a SPEC-017 para o conteúdo das bibliotecas vinculadas nas abas. SPEC-018 para serviços/fornecedores usados na checklist.

## 17. Premissas e pontos não definidos

- Lista fechada exata de rótulos de Tipo na UI se a referência divergir na capitalização.
- Efeito de alterar um modelo sobre eventos futuros apenas (já definido: não altera eventos passados); reaplicar em evento existente fica fora.
- Semântica fina de cada flag da aba Permissões frente a RF-056.
- Ordenação manual da lista de modelos.
