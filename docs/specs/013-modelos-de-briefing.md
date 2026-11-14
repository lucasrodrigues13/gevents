# 013. Modelos de briefing

**Status: aprovada.** O código segue esta spec. Fonte: [inventario-referencia.md](../inventario-referencia.md) §6 (menu) e leitura VIP da biblioteca. Briefing aplicado ao evento permanece em RF-071.

## 1. Identificador e nome

SPEC-013. Modelos de briefing.

## 2. Objetivo

O Organizer mantém a biblioteca de modelos de briefing: listar, criar com nome, editar seções e perguntas, clonar e excluir. O mesmo acervo aparece no menu de Gestão de eventos.

## 3. Requisitos relacionados

RF-054 (biblioteca de briefing). RF-071 (briefing do evento: resposta, link público, impressão) fica fora do miolo desta biblioteca, como destino de aplicação.

Fora: RF-079. Assistente. Fornecedor com login.

## 4. Atores e permissões

| Ator | Pode |
|---|---|
| Organizer | CRUD dos modelos de briefing da própria conta. |
| Client | Não edita a biblioteca. Pode responder briefing do evento se a flag de permissão (RF-056) e o fluxo RF-071 permitirem. |
| Admin | Acessa modelos de qualquer conta. |
| Visitante | Não acessa a biblioteca. |

## 5. Pré-condições

Conta autenticada, SPEC-001.

## 6. Fluxo principal

1. O Organizer abre Modelos de Briefing.
2. A lista mostra os modelos da conta, com nome, origem (Gestão de eventos ou Funil, quando houver) e data de criação.
3. Cria um novo modelo informando o nome e salva.
4. Abre o modelo: editor com seções, descrição e perguntas.
5. Edita seções e perguntas (tipo visto: Texto de resposta curta) e salva.
6. Pode clonar um modelo (inclusive padrão, que avisa para clonar antes de editar).
7. Pode excluir um modelo próprio, com confirmação.

## 7. Fluxos alternativos

- Lista vazia: ação de criar.
- Modelo padrão da plataforma: aviso para clonar antes de editar; edição direta do padrão não grava sobre o original.
- Nome vazio: não grava.
- Modelo de outra conta: Organizer não vê.
- Aplicar o modelo a um briefing de evento: RF-071 / fluxo do evento, fora desta spec.

## 8. Regras de negócio

- A biblioteca de briefing é uma das bibliotecas de RF-054.
- Cada item pode mostrar origem (Gestão de eventos ou Funil) e data.
- O editor organiza conteúdo em seções com descrição e perguntas. RF-071 descreve o uso no evento; aqui o conteúdo é o modelo.
- Tipo de pergunta mínimo suportado: Texto de resposta curta.
- Modelo padrão exibe aviso para clonar antes de editar.
- Clonar gera cópia editável na conta do Organizer.
- Excluir remove o modelo da biblioteca; briefings de evento já criados a partir dele mantêm o conteúdo copiado.
- Isolamento por conta do Organizer.

## 9. Validações

Nome do modelo obrigatório, sem só espaços. Pergunta sem enunciado não grava. Modelo inexistente ou de outra conta: rejeitado sem vazar dado.

## 10. Estados

Modelo na biblioteca. Status de resposta do briefing no evento permanece em RF-071.

## 11. Dados necessários

Lista: nome, origem, data.

Criar / editar: nome; seções (título, descrição); perguntas (enunciado, tipo).

Clonar / excluir: identificador do modelo.

## 12. Interface

Lista com busca, "Criar novo modelo" e painel de leitura ao selecionar. Editor de seções e perguntas. Aviso no modelo padrão. Confirmação ao excluir.

## 13. Erros

Campo inválido permanece no editor. Falha ao salvar mantém o anterior. Modelo de outra conta não revela dado.

## 14. Critérios de aceite

1. O Organizer lista só os próprios modelos (Admin, a conta escolhida).
2. Criar com nome grava; sem nome, não grava.
3. Abrir um modelo mostra seções, descrição e perguntas.
4. Clonar gera cópia editável; modelo padrão avisa para clonar antes de editar.
5. Excluir remove da biblioteca sem apagar briefings já aplicados em eventos.
6. Outra conta não vê o modelo.

## 15. Cenários de teste

Criar modelo com uma seção e uma pergunta de resposta curta. Clonar. Editar a cópia. Excluir a cópia. Conta B não vê o modelo da conta A. Abrir modelo padrão e ver o aviso de clonar.

## 16. Dependências

SPEC-001. RF-054. RF-071 para aplicação no evento. SPEC-012 pode vincular briefing na aba Briefings do modelo de evento.

## 17. Premissas e pontos não definidos

- Demais tipos de pergunta além de texto curto.
- Unificação obrigatória da lista Gestão × Funil neste produto (a referência mostra origem mista; o gevents trata como biblioteca da conta do Organizer).
- Texto exato do aviso do modelo padrão.
