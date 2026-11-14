# 017. Modelos de mensagens

**Status: aprovada.** O código segue esta spec. Fonte: [inventario-referencia.md](../inventario-referencia.md) §6 e leitura VIP (`/modelos_mensagens`). Uso em RSVP e envios permanece nos RFs de convite/RSVP.

## 1. Identificador e nome

SPEC-017. Modelos de mensagens.

## 2. Objetivo

O Organizer mantém a biblioteca de modelos de mensagem: listar, criar com nome e texto (com parâmetros), editar, clonar e excluir.

## 3. Requisitos relacionados

RF-054 (biblioteca de mensagem). Consumo em RSVP e envios: RF-065 e fluxos de convite; na aba RSVP e Mensagens do modelo de evento: SPEC-012.

Fora: RF-079. Assistente. Fornecedor com login. Mensagens do funil (SPEC de funil, se houver).

## 4. Atores e permissões

| Ator | Pode |
|---|---|
| Organizer | CRUD dos modelos de mensagem da própria conta. |
| Client | Não edita a biblioteca. |
| Admin | Acessa modelos de qualquer conta. |
| Visitante | Não acessa. |

## 5. Pré-condições

Conta autenticada, SPEC-001.

## 6. Fluxo principal

1. O Organizer abre Modelos de Mensagens.
2. Se não houver modelo, vê o estado vazio e a ação "Novo Modelo".
3. Abre o formulário: Nome do modelo e Texto.
4. Pode inserir parâmetros (#nome_evento, #data_evento, #convite, etc.) no texto.
5. Salva. O modelo entra na lista.
6. Edita ou exclui um modelo existente, com confirmação na exclusão.
7. Pode clonar um modelo, gerando cópia editável.

## 7. Fluxos alternativos

- Sem modelos: texto "Sem modelos de mensagens cadastrados" e ação "Novo Modelo".
- Nome vazio: não grava.
- Texto vazio: não grava.
- Cancelar no formulário: nada é gravado.
- Modelo de outra conta: Organizer não vê.

## 8. Regras de negócio

- A biblioteca de mensagem é uma das bibliotecas de RF-054.
- Criar/editar usa Nome e Texto.
- Parâmetros no texto são substituídos na hora do envio pelos dados do evento/convite. Parâmetros vistos: `#nome_da_empresa`, `#nome_evento`, `#tipo_evento`, `#data_evento`, `#hora_evento`, `#local_cerimonia`, `#local_recepcao`, `#endereco_cerimonia`, `#endereco_recepcao`, `#convite`, `#link_convite`, `#link_unico`, `#nome_convidados`, `#mesa_dos_convidados`, `#site_url`, `#prazo_rsvp`, `#qr_code`.
- Clonar gera cópia na conta.
- Excluir remove da biblioteca; envios já feitos não são alterados.
- Isolamento por conta do Organizer.

## 9. Validações

Nome obrigatório, sem só espaços. Texto obrigatório, sem só espaços. Modelo alheio rejeitado sem vazar dado.

## 10. Estados

Biblioteca vazia; biblioteca com modelos. Sem situação adicional.

## 11. Dados necessários

Lista: nome do modelo.

Criar / editar: nome, texto (com ou sem parâmetros).

Clonar / excluir: identificador.

## 12. Interface

Lista com "Novo Modelo". Estado vazio com texto de ausência. Modal ou formulário com Nome, Texto, lista de parâmetros, Cancelar e Salvar. Confirmação ao excluir.

## 13. Erros

Campo inválido permanece no formulário. Falha ao salvar mantém o anterior. Modelo alheio não revela dado.

## 14. Critérios de aceite

1. Sem modelos, a tela mostra "Sem modelos de mensagens cadastrados" e "Novo Modelo".
2. Criar com nome e texto grava; faltando um dos dois, não grava.
3. Editar atualiza nome e texto.
4. Clonar gera cópia editável.
5. Excluir remove da biblioteca.
6. Outra conta não vê o modelo.

## 15. Cenários de teste

Abrir biblioteca vazia. Criar modelo "RSVP" com texto usando `#nome_evento`. Editar. Clonar. Excluir a cópia. Conta B não vê o modelo da conta A.

## 16. Dependências

SPEC-001. RF-054. RF-065 / SPEC de convites para envio. SPEC-012 para vínculo na aba RSVP e Mensagens.

## 17. Premissas e pontos não definidos

- Subconjunto mínimo de parâmetros obrigatório no produto (a lista vista pode ser reduzida se algum parâmetro depender de módulo fora do corte).
- Pré-visualização com dados de um evento na biblioteca.
