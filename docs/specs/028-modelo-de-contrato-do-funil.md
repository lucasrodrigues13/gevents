# 028. Modelo de contrato do funil

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-028. Modelo de contrato do funil.

## 2. Objetivo

O Organizer mantém a biblioteca de modelos de contrato do funil e os aplica ao criar contratos na SPEC-022.

## 3. Requisitos relacionados

RF-075 e RF-077, no recorte de contratos e catálogos do funil. Lista de contratos é a SPEC-022. Oportunidade é a SPEC-020. RF-079 fica fora.

## 4. Atores e permissões

O `Organizer` lista, cria, edita e exclui modelos da própria conta. Outro `Organizer` não vê. `Admin` acessa qualquer conta. `Client` não acessa. Assistente e fornecedor com login ficam fora.

## 5. Pré-condições

Conta autenticada, SPEC-001.

## 6. Fluxo principal

1. O Organizer abre a biblioteca de modelos de contrato do funil.
2. Cria modelo com nome e texto/cláusulas.
3. O modelo aparece na lista da conta.
4. Edita nome ou texto e salva.
5. Ao criar contrato na SPEC-022, escolhe o modelo; o sistema copia o conteúdo para o contrato.
6. Exclui modelo que não queira mais na biblioteca.

## 7. Fluxos alternativos

- Lista vazia: a tela informa que não há modelo.
- Nome ou texto vazios: não grava.
- Excluir modelo não apaga contratos já gerados na SPEC-022.
- Contrato de fornecedor no evento não usa esta biblioteca.

## 8. Regras de negócio

- Modelo pertence à conta do Organizer.
- A biblioteca de modelos é distinta da lista de contratos (SPEC-022).
- Tokens de oportunidade, quando usados no texto, seguem o mesmo padrão mínimo de `#nome_oportunidade` da SPEC-027.
- RF-079 (loja de modelos da referência) fica fora.

## 9. Validações

Nome obrigatório. Texto do modelo obrigatório.

## 10. Estados

Modelo cadastrado ou excluído.

## 11. Dados necessários

Nome e texto/cláusulas do modelo.

## 12. Interface

Lista com criar, editar e excluir. Editor de texto do modelo. Angular Material, DA-044.

## 13. Erros

Campo inválido indica o campo. Item de outra conta fica indisponível. Lista vazia não é erro.

## 14. Critérios de aceite

1. Sem modelo, a biblioteca mostra lista vazia.
2. Criar com nome e texto coloca o modelo na própria conta.
3. Editar e excluir alteram só a própria conta.
4. Aplicar o modelo na SPEC-022 copia o conteúdo para o contrato.
5. Excluir o modelo não apaga contratos já criados.
6. A tela permanece distinta da lista de contratos.

## 15. Cenários de teste

Lista vazia. Criar, editar e excluir. Aplicar em contrato. Duas contas. Tentativa sem nome ou sem texto.

## 16. Dependências

SPEC-001, SPEC-020 e SPEC-022.

## 17. Premissas e pontos não definidos

Estrutura rígida de cláusulas numeradas além do texto livre do modelo.
