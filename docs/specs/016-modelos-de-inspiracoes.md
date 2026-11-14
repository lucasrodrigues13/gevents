# 016. Modelos de inspirações

**Status: aprovada.** O código segue esta spec. Fonte: [inventario-referencia.md](../inventario-referencia.md) §6 e leitura VIP (`/modelo_inspiracoes`). Inspirações no evento permanecem em RF-072.

## 1. Identificador e nome

SPEC-016. Modelos de inspirações.

## 2. Objetivo

O Organizer mantém a biblioteca de galerias de inspiração: listar, criar galeria com nome, editar conteúdo (imagens/referências), clonar quando disponível e excluir.

## 3. Requisitos relacionados

RF-054 (biblioteca de inspiração). RF-072 (inspirações / galerias no contexto do evento e visão do Client).

Fora: RF-079. Assistente. Fornecedor com login. Aba Inspirações do modelo de evento (vínculo na SPEC-012).

## 4. Atores e permissões

| Ator | Pode |
|---|---|
| Organizer | CRUD das galerias da própria conta. |
| Client | Não edita a biblioteca. No evento, vê inspirações se a flag (RF-056 / RF-072) permitir. |
| Admin | Acessa galerias de qualquer conta. |
| Visitante | Não acessa a biblioteca. |

## 5. Pré-condições

Conta autenticada, SPEC-001.

## 6. Fluxo principal

1. O Organizer abre Modelos de Inspirações.
2. Se não houver galeria, vê o estado vazio e a ação "Criar nova galeria".
3. Cria uma galeria informando o nome e salva.
4. Edita a galeria: adiciona, reordena ou remove imagens/referências.
5. Salva.
6. Pode clonar uma galeria existente.
7. Pode excluir uma galeria própria, com confirmação.

## 7. Fluxos alternativos

- Sem galerias: texto "Sem galerias cadastradas" (ou equivalente visto: mensagem orientando a clicar em "Criar nova galeria").
- Nome vazio: não grava.
- Galeria sem imagens: permitida; lista a galeria vazia.
- Galeria de outra conta: Organizer não vê.
- Uso no evento: RF-072, fora do miolo desta biblioteca.

## 8. Regras de negócio

- A biblioteca de inspiração é uma das bibliotecas de RF-054; o item é a galeria (RF-072).
- Estado vazio mostra orientação e a ação de criar.
- Clonar gera cópia editável na conta.
- Excluir remove a galeria da biblioteca; referências já usadas em evento mantêm o que foi copiado/anexado no evento.
- Isolamento por conta do Organizer.
- Upload de imagem segue as regras gerais de arquivo do produto (DA de arquivo), sem antimalware obrigatório neste corte se ainda não estiver no Must.

## 9. Validações

Nome da galeria obrigatório, sem só espaços. Arquivo de imagem em tipo/tamanho aceitos pelo produto. Galeria alheia rejeitada sem vazar dado.

## 10. Estados

Biblioteca sem galerias; biblioteca com uma ou mais galerias. Sem situação adicional por galeria.

## 11. Dados necessários

Lista: nome da galeria.

Criar / editar: nome; imagens ou itens de referência (arquivo/URL conforme a UI).

Clonar / excluir: identificador.

## 12. Interface

Tela com "Criar nova galeria". Estado vazio com texto de ausência de galerias. Lista de galerias e editor de conteúdo. Confirmação ao excluir.

## 13. Erros

Campo ou arquivo inválido permanece no formulário. Falha ao salvar mantém o anterior. Galeria alheia não revela dado.

## 14. Critérios de aceite

1. Sem galeria, a biblioteca mostra o estado vazio e "Criar nova galeria".
2. Criar com nome grava a galeria; sem nome, não grava.
3. Editar permite adicionar e remover imagens/referências.
4. Clonar gera cópia editável.
5. Excluir remove da biblioteca.
6. Outra conta não vê a galeria.

## 15. Cenários de teste

Abrir conta sem galeria e ver o vazio. Criar galeria com nome. Adicionar uma imagem. Clonar. Excluir a cópia. Conta B não vê a galeria da conta A.

## 16. Dependências

SPEC-001. RF-054. RF-072. SPEC-012 pode vincular na aba Inspirações.

## 17. Premissas e pontos não definidos

- Se a referência aceita só upload, só URL ou ambos.
- Limite de imagens por galeria e tamanho máximo por arquivo.
- Texto exato do estado vazio (a referência varia entre "Sem galerias cadastradas" e frase com instrução).
