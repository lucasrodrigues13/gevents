# 013. Modelos de briefing

**Status: rascunho.** Fora do MVP aprovado. RF-054 e RF-071 continuam Observado. Esta spec não autoriza código.

## 1. Identificador e nome

SPEC-013. Modelos de briefing.

## 2. Objetivo

A assessoria abre a biblioteca de modelos de briefing, a mesma lista no menu de Gestão e no menu do Funil, e lê um modelo com seções, descrição e perguntas.

## 3. Requisitos relacionados

RF-095 e RF-096, Observado, ainda sem aprovação. RF-054 registra a biblioteca e que o editor não tinha sido aberto; o editor de briefing foi aberto depois, e esse trecho fica nestes dois RF. O briefing do evento, o status de resposta, o link público e a impressão permanecem em RF-071.

Roteiro, cronograma, inspiração e mensagem permanecem em RF-054.

## 4. Atores e permissões

A assessoria, na conta autenticada, consulta a biblioteca. Isolamento entre contas não foi medido.

## 5. Pré-condições

Conta autenticada, SPEC-001. O fluxo está fora do corte Must de [mvp-scope.md](../mvp-scope.md).

## 6. Fluxo principal

1. A assessoria abre Modelos de briefing pela Gestão ou pelo Funil.
2. Os dois menus mostram a mesma lista.
3. Cada item mostra a origem e uma data. A origem vista é Gestão de eventos ou Funil.
4. Abrir um modelo mostra seções, descrição e perguntas.
5. A pergunta vista é do tipo Texto de resposta curta.
6. Um modelo padrão mostra um aviso para clonar antes de editar.
7. Abrir o modelo Conhecendo o cliente mostra a seção Conhecendo você e perguntas de resposta curta.

## 7. Fluxos alternativos

Clonar, criar, salvar e excluir não foram acionados. O resultado desses atos não está definido. Outro tipo de pergunta não foi visto.

## 8. Regras de negócio

- A lista da Gestão e a lista do Funil são a mesma biblioteca (RF-095).
- Cada item mostra origem e data (RF-095). Origens vistas: Gestão de eventos e Funil.
- O editor mostra seções, descrição e perguntas (RF-096).
- O tipo de pergunta visto é Texto de resposta curta (RF-096).
- Um modelo padrão avisa para clonar antes de editar (RF-096).
- O modelo Conhecendo o cliente contém a seção Conhecendo você, com perguntas de resposta curta (RF-096).

## 9. Validações

Nenhuma validação foi exercida. Campo obrigatório, tamanho de texto e formato da data não estão definidos.

## 10. Estados

A biblioteca, neste recorte, é só consulta. Estado da pergunta e status de resposta permanecem em RF-071.

## 11. Dados necessários

Modelo: nome, origem, data. Editor: seções, descrição e perguntas. Pergunta vista: tipo Texto de resposta curta. Seção vista em Conhecendo o cliente: Conhecendo você.

O lugar da descrição, no modelo ou na seção, não está definido. O significado da data, criação ou alteração, não está definido.

## 12. Interface

Dois menus, Gestão e Funil, abrem a mesma lista. A lista mostra origem e data. O editor mostra seções, descrição e perguntas. O modelo padrão mostra o aviso para clonar.

## 13. Erros

Nenhum erro foi provocado. Mensagem de falha ao gravar não está definida.

## 14. Critérios de aceite

1. Gestão e Funil mostram a mesma lista.
2. Cada item mostra origem Gestão de eventos ou Funil, e uma data.
3. Abrir um modelo mostra seções, descrição e perguntas.
4. A pergunta exibida é Texto de resposta curta.
5. O modelo padrão exibe aviso para clonar antes de editar.
6. Conhecendo o cliente abre a seção Conhecendo você com perguntas de resposta curta.

## 15. Cenários de teste

Abrir a lista pelos dois menus e comparar os itens. Abrir um modelo e ler seções, descrição e o tipo da pergunta. Abrir um modelo padrão e ler o aviso. Abrir Conhecendo o cliente e ler a seção Conhecendo você.

## 16. Dependências

SPEC-001. RF-054 nomeia a biblioteca entre os outros modelos. RF-095 e RF-096 detalham a lista e o editor. RF-071 fica para o briefing aplicado ao evento.

## 17. Premissas e pontos não definidos

- Clonar, criar, salvar e excluir, inclusive o que o aviso do modelo padrão faz além de avisar.
- Texto exato do aviso.
- Outros tipos de pergunta.
- Como a origem é gravada.
- Se a data é de criação ou de alteração, e o formato.
- Se a descrição é do modelo ou da seção.
- Se o produto inclui o modelo Conhecendo o cliente como conteúdo inicial.
- Ordenação, busca e modelo vazio.
- Isolamento entre contas.
- Aplicar o modelo num briefing, status de resposta, link público e impressão.
