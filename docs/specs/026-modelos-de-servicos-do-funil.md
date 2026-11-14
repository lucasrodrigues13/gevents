# 026. Modelos de serviços do funil

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-026. Modelos de serviços do funil.

## 2. Objetivo

O Organizer mantém o catálogo de serviços/produtos do funil usados nas oportunidades: lista, cria, edita e exclui.

## 3. Requisitos relacionados

RF-075 e RF-077, no recorte de catálogos do funil. Uso do serviço/produto na oportunidade é a SPEC-020. A lista de serviços de fornecedor (`/suppliers/services`, SPEC-018) é catálogo distinto. RF-079 fica fora.

## 4. Atores e permissões

O `Organizer` opera o catálogo da própria conta. Outro `Organizer` não vê. `Admin` acessa qualquer conta. `Client` não acessa. Assistente e fornecedor com login ficam fora.

## 5. Pré-condições

Conta autenticada, SPEC-001.

## 6. Fluxo principal

1. O Organizer abre Modelos de Serviços/Produtos no funil.
2. Vê a lista da conta.
3. Cria item com nome do serviço/produto, descrição opcional e valor opcional.
4. O item aparece na lista e fica disponível na SPEC-020.
5. Edita nome, descrição ou valor.
6. Exclui o item que não estiver em uso obrigatório; se estiver referenciado só como texto histórico em oportunidade, a exclusão remove o modelo do catálogo sem apagar a oportunidade.

## 7. Fluxos alternativos

- Lista vazia: a tela informa que não há serviço/produto cadastrado.
- Nome vazio: não grava.
- Descrição acima de 200 caracteres: não grava.
- Catálogo de fornecedor da SPEC-018: não aparece aqui e vice-versa.

## 8. Regras de negócio

- Modelo de serviço/produto do funil pertence à conta do Organizer.
- Distinto do serviço de fornecedor da SPEC-018.
- Valor vazio é permitido; a oportunidade pode informar valor próprio.
- RF-079 (loja de modelos da referência) fica fora.

## 9. Validações

Nome obrigatório, sem ficar só com espaços. Descrição, se preenchida, até 200 caracteres. Valor, se preenchido, não negativo.

## 10. Estados

Item cadastrado ou excluído. Não há situação intermediária.

## 11. Dados necessários

Nome do serviço/produto, descrição opcional, valor opcional.

## 12. Interface

Lista com ação Novo serviço/produto, edição e exclusão. Formulário com nome, descrição e valor. Angular Material, DA-044.

## 13. Erros

Campo inválido indica o campo. Item de outra conta fica indisponível. Lista vazia não é erro.

## 14. Critérios de aceite

1. Criar com nome coloca o item na lista da própria conta.
2. Sem nome, nada é gravado.
3. Editar e excluir alteram só a própria conta.
4. O item criado aparece como opção de serviço/produto na SPEC-020.
5. A lista é distinta da SPEC-018.
6. Outra conta não vê o item.

## 15. Cenários de teste

Lista vazia. Criar, editar e excluir. Descrição longa demais. Duas contas. Uso na criação de oportunidade.

## 16. Dependências

SPEC-001 e SPEC-020. SPEC-018 é o outro catálogo.

## 17. Premissas e pontos não definidos

Se a exclusão de modelo ainda referenciado por oportunidade antiga exige confirmação extra além de remover do catálogo.
