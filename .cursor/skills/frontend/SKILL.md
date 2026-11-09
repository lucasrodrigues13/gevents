---
name: frontend
description: Aplica o design system AdminLTE 4 do gevents em Razor Pages. Use ao criar ou alterar página, layout, partial, CSS, JavaScript de interface, formulário, tabela, modal, alerta ou menu visual.
---

# Frontend

Leia `docs/frontend/README.md` e o documento que o pedido exigir. Não copie o catálogo para a resposta.

## Faça

1. Confira se o componente já está em `docs/frontend/component-catalog.md`. Reutilize o partial, a classe ou o Tag Helper.
2. Use a referência AdminLTE 4 de `docs/frontend/adminlte-references.md`. Não misture exemplo do AdminLTE 3 nem invente classe.
3. Mantenha Razor Pages, Tag Helpers (`asp-for`, `asp-validation-for`, `asp-validation-summary`, `asp-page`) e o POST com antiforgery.
4. Validação visual acompanha o `ModelState`. O unobtrusive não substitui o servidor.
5. Listagem, formulário, modal e estado vazio seguem `docs/frontend/page-patterns.md`.
6. Cor, raio, sombra e botão vêm das variáveis do Bootstrap/AdminLTE. Sem hexadecimal novo e sem biblioteca que o pacote não tenha.
7. Confira vazio, erro, sucesso e desabilitado. Em tela estreita, o bloco empilha e a tabela fica em `table-responsive`.
8. Label visível, foco do Bootstrap e nome acessível em botão de ícone.
9. Ao criar componente reutilizável, atualize o catálogo e `docs/frontend/rastreabilidade.md`.

## Não faça

- Não troque Razor Pages por outro framework de UI.
- Não crie partial para um único input.
- Não paginar nem ordenar a lista inteira no navegador.
- Não altere Domain, Application ou Infrastructure por causa de estilo.
