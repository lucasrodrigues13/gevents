# 016. Modelos de inspirações

**Status: rascunho.** Fora do corte do MVP. RF-054 e RF-072 permanecem Observado. Fonte: a tela `modelo_inspiracoes` da referência, aberta sem galeria. A ação "Criar nova galeria" não foi acionada.

## 1. Identificador e nome

SPEC-016. Modelos de inspirações.

## 2. Objetivo

Descrever a biblioteca de modelos de inspiração quando não há galeria cadastrada.

## 3. Requisitos relacionados

RF-054 e RF-072, os dois Observado. O corte em [mvp-scope.md](../mvp-scope.md) deixa os dois de fora. Nenhum dos dois traz o texto do estado vazio.

Proposta ainda não gravada em `docs/requirements.md`:

| ID | Nome | Comportamento observado | Ator | Regras vistas | Depende de | Status |
|---|---|---|---|---|---|---|
| RF-110 | Estado vazio dos modelos de inspiração | Sem galeria cadastrada, a biblioteca mostra o texto "Sem galerias cadastradas" e a ação "Criar nova galeria". | Assessoria | A ação foi vista e não foi acionada. Conteúdo da galeria e o fluxo de criação não foram vistos. | RF-054, RF-072 | Observado |

Fora deste arquivo: aba Inspirações do modelo de evento (RF-053), inspirações dentro do evento, conteúdo da galeria, destino de "Criar nova galeria" e a visão do cliente citada em RF-072.

## 4. Atores e permissões

A tela foi vista na conta da assessoria. RF-072 também nomeia o cliente. Essa visão não foi aberta. Quem mais acessa a biblioteca fica não definido. A role `User` não vira papel de negócio, RF-040.

## 5. Pré-condições

Conta autenticada, SPEC-001. A biblioteca está sem galeria cadastrada. A abertura sem sessão não foi testada.

## 6. Fluxo principal

1. A assessoria abre a biblioteca de modelos de inspiração.
2. A tela mostra o texto "Sem galerias cadastradas".
3. A mesma tela mostra a ação "Criar nova galeria".
4. A ação segue sem acionamento. O fluxo visto termina aqui.

## 7. Fluxos alternativos

- Acionar "Criar nova galeria" fica não definido. A ação não foi usada.
- Biblioteca com ao menos uma galeria fica não definido. A lista não foi vista.
- O cliente nesta biblioteca fica não definido. A visão não foi aberta.

## 8. Regras de negócio

- Sem galeria cadastrada, a biblioteca mostra "Sem galerias cadastradas" e a ação "Criar nova galeria". RF-110, Observado, proposto nesta spec e ainda ausente de `requirements.md`.
- Efeito da ação, campos da galeria, imagens, ordem, vínculo com evento, vínculo com modelo de evento e acesso do cliente ficam não definidos. Não há RF para isso.

## 9. Validações

Nenhuma validação foi vista. Fica não definido.

## 10. Estados

Estado visto: biblioteca sem galeria. Estado com galeria cadastrada fica não definido.

## 11. Dados necessários

O nome visto para o item é galeria. Nenhum campo da galeria foi visto. Fica não definido.

## 12. Interface

Tela sem galeria, com o texto "Sem galerias cadastradas" e a ação "Criar nova galeria".

## 13. Erros

Nenhum erro foi visto. Fica não definido.

## 14. Critérios de aceite

1. Sem galeria cadastrada, a biblioteca mostra o texto "Sem galerias cadastradas" e a ação "Criar nova galeria".

## 15. Cenários de teste

Abrir a biblioteca numa conta sem galeria. Conferir o texto "Sem galerias cadastradas" e a ação "Criar nova galeria". O cenário não aciona a ação.

## 16. Dependências

SPEC-001. RF-054 e RF-072, ainda Observado. RF-110, proposto e ainda não gravado em `requirements.md`.

## 17. Premissas e pontos não definidos

Destino de "Criar nova galeria". Campos, imagens e nome da galeria. Lista, edição, exclusão e ordenação quando há galerias. Uso no evento e na aba Inspirações do modelo de evento (RF-053). Visão do cliente (RF-072). Isolamento da biblioteca por conta. Abertura sem sessão. Qualquer validação.
