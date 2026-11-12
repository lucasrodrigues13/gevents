---
name: implementation-planning
description: Só use quando o usuário pedir explicitamente um plano técnico. O gevents implementa a partir da spec aprovada, sem plano separado.
---

# Plano de implementação

O código segue a spec aprovada. Não crie plano técnico por conta própria e não espere um plano para implementar.

Se o usuário pedir o plano:

1. A spec precisa estar aprovada. Sem isso, pare.
2. Leia a spec e os RF citados. Não acrescente comportamento.
3. Liste entidades, casos de uso, endpoints da API, telas Angular, autorização, migration e testes. Endpoint de negócio não usa `DbContext`.
4. Aponte o que muda em Domain, Application, Infrastructure e Api, e na tela em `frontend/` quando a spec tiver interface. Recuse fila, MediatR, AutoMapper, repositório genérico, refresh token e Razor Pages.
5. Mostre o plano. Não escreva código nessa resposta, a menos que o usuário também peça a implementação.
