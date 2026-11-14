# 015. Modelos de cronograma

**Status: aprovada.** O código segue esta spec. Fonte: [inventario-referencia.md](../inventario-referencia.md) §6 e padrão de biblioteca dos demais modelos. Cronograma do evento permanece em RF-070.

## 1. Identificador e nome

SPEC-015. Modelos de cronograma.

## 2. Objetivo

O Organizer mantém a biblioteca de modelos de cronograma: listar, criar com título, editar conteúdo (descrição e itens de grade), clonar e excluir. "Salvar como modelo" a partir do cronograma do evento (RF-070) grava nesta biblioteca.

## 3. Requisitos relacionados

RF-054 (biblioteca de cronograma). RF-070 (cronograma do evento: data, hora, responsável, recado a fornecedores, salvar como modelo, impressão e compartilhamento).

Fora: RF-079. Assistente. Fornecedor com login.

## 4. Atores e permissões

| Ator | Pode |
|---|---|
| Organizer | CRUD dos modelos de cronograma da própria conta. |
| Client | Não edita a biblioteca. Acesso ao cronograma do evento só com flag (RF-056). |
| Admin | Acessa modelos de qualquer conta. |
| Visitante | Não acessa. |

## 5. Pré-condições

Conta autenticada, SPEC-001.

## 6. Fluxo principal

1. O Organizer abre Modelos de Cronograma.
2. A lista mostra itens com título e descrição.
3. Cria um novo modelo informando título (e descrição opcional) e salva.
4. Abre o modelo e edita a grade de itens (data/hora relativa ou rótulo, responsável, recado a fornecedores, conforme o espelho de RF-070).
5. Salva.
6. Pode clonar um modelo.
7. Pode excluir um modelo próprio, com confirmação.

## 7. Fluxos alternativos

- Lista vazia: ação de criar.
- Título vazio: não grava.
- "Salvar como modelo" no cronograma do evento (RF-070): cria item nesta biblioteca com o conteúdo atual do evento.
- Modelo de outra conta: Organizer não vê.
- Aplicar modelo a um evento: copia o conteúdo para o cronograma do evento (RF-070); detalhe de UI do evento fora desta spec.

## 8. Regras de negócio

- A biblioteca existe conforme RF-054.
- Item da lista exibe título e descrição.
- Clonar gera cópia editável na conta.
- Excluir remove da biblioteca; cronogramas já no evento mantêm o que foi copiado.
- Isolamento por conta do Organizer.
- Impressão e compartilhamento do cronograma no evento permanecem em RF-070 / RF-077, não nesta biblioteca.

## 9. Validações

Título obrigatório, sem só espaços. Datas e horas, se informadas nos itens, válidas. Modelo alheio rejeitado sem vazar dado.

## 10. Estados

Modelo na biblioteca. Sem situação adicional.

## 11. Dados necessários

Lista: título, descrição.

Criar / editar: título, descrição; itens com data/hora (ou referência), responsável, recado a fornecedores.

Clonar / excluir: identificador.

## 12. Interface

Lista com "Criar novo modelo", itens com título e descrição, editor da grade. Confirmação ao excluir.

## 13. Erros

Campo inválido permanece no formulário. Falha ao salvar mantém o anterior. Modelo alheio não revela dado.

## 14. Critérios de aceite

1. O Organizer lista só os próprios modelos, com título e descrição.
2. Criar com título grava; sem título, não grava.
3. Editar e salvar atualiza o conteúdo da grade.
4. Clonar gera cópia editável.
5. Excluir remove da biblioteca sem apagar cronogramas já no evento.
6. Outra conta não vê o modelo.

## 15. Cenários de teste

Criar modelo com título e um item. Clonar. Excluir a cópia. Conta B não vê o modelo da conta A.

## 16. Dependências

SPEC-001. RF-054. RF-070 para o cronograma do evento e "salvar como modelo".

## 17. Premissas e pontos não definidos

- Formato exato da grade do modelo (espelho completo de RF-070) quando a referência não foi percorrida item a item.
- Se data/hora no modelo é absoluta ou relativa ao evento.
- Busca e ordenação da lista.
