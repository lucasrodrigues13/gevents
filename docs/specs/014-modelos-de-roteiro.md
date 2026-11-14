# 014. Modelos de roteiro

**Status: aprovada.** O código segue esta spec. Fonte: [inventario-referencia.md](../inventario-referencia.md) §6 e leitura VIP da biblioteca. Roteiro do evento permanece em RF-069.

## 1. Identificador e nome

SPEC-014. Modelos de roteiro.

## 2. Objetivo

O Organizer mantém a biblioteca de modelos de roteiro: listar, criar com nome, editar seções (horário, música, itens de checklist), clonar e excluir.

## 3. Requisitos relacionados

RF-054 (biblioteca de roteiro). RF-069 (roteiro no evento: observação, anexo, impressão, permissão do Client) fica como destino de aplicação, fora do miolo desta biblioteca.

Fora: RF-079. Assistente. Fornecedor com login.

## 4. Atores e permissões

| Ator | Pode |
|---|---|
| Organizer | CRUD dos modelos de roteiro da própria conta. |
| Client | Não edita a biblioteca. No evento, ver ou editar o roteiro só com flag (RF-056 / RF-069). |
| Admin | Acessa modelos de qualquer conta. |
| Visitante | Não acessa. |

## 5. Pré-condições

Conta autenticada, SPEC-001.

## 6. Fluxo principal

1. O Organizer abre Modelos de Roteiro.
2. A lista mostra os modelos da conta pelo nome.
3. Cria um novo modelo com nome e salva.
4. Abre o modelo e edita seções com horário, música e itens de checklist.
5. Salva.
6. Pode clonar um modelo existente.
7. Pode excluir um modelo próprio, com confirmação.

## 7. Fluxos alternativos

- Lista vazia: ação "Criar novo modelo".
- Nome vazio: não grava.
- Seção sem horário, sem música ou sem item: permitido; campos opcionais salvo o nome do modelo.
- Modelo de outra conta: Organizer não vê.
- Aplicar ao evento: RF-069, fora desta spec.

## 8. Regras de negócio

- A biblioteca de roteiro é uma das bibliotecas de RF-054.
- O conteúdo do modelo é organizado em seções; cada seção pode ter horário, música e itens de checklist.
- Clonar gera cópia editável na conta.
- Excluir remove da biblioteca; roteiros já aplicados em eventos mantêm o conteúdo copiado.
- Isolamento por conta do Organizer.
- Ordenação de seções no editor é permitida (RF-069 nomeia seções ordenáveis no evento; o modelo espelha a mesma estrutura).

## 9. Validações

Nome do modelo obrigatório, sem só espaços. Horário, se informado, em formato válido. Modelo de outra conta rejeitado sem vazar dado.

## 10. Estados

Modelo na biblioteca. Sem situação adicional.

## 11. Dados necessários

Lista: nome do modelo.

Criar / editar: nome; seções com horário, música, itens de checklist (texto).

Clonar / excluir: identificador.

## 12. Interface

Lista com "Criar novo modelo" e painel ao selecionar. Editor de seções. Confirmação ao excluir. Sem VIP Store.

## 13. Erros

Campo inválido permanece no editor. Falha ao salvar mantém o anterior. Modelo alheio não revela dado.

## 14. Critérios de aceite

1. O Organizer lista só os próprios modelos.
2. Criar com nome grava; sem nome, não grava.
3. O editor mostra seções com horário, música e itens de checklist.
4. Clonar gera cópia editável.
5. Excluir remove da biblioteca sem apagar roteiros já no evento.
6. Outra conta não vê o modelo.

## 15. Cenários de teste

Criar modelo com uma seção (horário, música, um item). Clonar. Excluir a cópia. Conta B não vê o modelo da conta A.

## 16. Dependências

SPEC-001. RF-054. RF-069 para uso no evento. SPEC-012 pode vincular na aba Roteiros.

## 17. Premissas e pontos não definidos

- Campos obrigatórios além do nome do modelo.
- Anexo e observação no modelo (RF-069 os cita no evento; no modelo da biblioteca não foram confirmados).
- Impressão a partir da biblioteca (impressão no evento fica em RF-069 / RF-077).
