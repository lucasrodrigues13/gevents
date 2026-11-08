---
name: implementation-planning
description: Planeja a implementação de uma spec já aprovada do gevents, por camadas, testes e tarefas. Use quando o usuário pedir plano técnico depois da aprovação da spec. Não escreve código.
---

# Plano de implementação

## Antes de planejar

A spec precisa estar marcada como aprovada. Sem isso, pare.

## Faça

1. Leia a spec e os RF citados. Não acrescente comportamento.
2. Liste entidades, casos de uso, páginas Razor, autorização, migration e testes. Página de negócio não usa `DbContext`.
3. Quebre em tarefas verificáveis, cada uma com critério de aceite da spec.
4. Aponte o que muda em Domain, Application, Infrastructure e Web. Recuse fila, MediatR, AutoMapper e repositório genérico.
5. Mostre o plano e espere aprovação antes de implementar.

## Não faça

- Não crie projeto, migration nem código.
- Não mova cartão do Trello para Concluído.
