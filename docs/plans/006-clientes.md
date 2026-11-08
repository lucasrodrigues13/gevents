# Plano 006. Clientes

**Status: proposta.** Spec [006-clientes.md](../specs/006-clientes.md). Depende do plano 002.

## Fora deste plano

Login do cliente, permissões, último acesso, remoção e criação automática a partir do casal do evento.

## Camadas

| Camada | O que entra |
|---|---|
| Domain | `Cliente` da conta e o vínculo com evento. Nome obrigatório. O mesmo par não duplica. |
| Application | Cadastrar, editar, listar, vincular e desvincular. |
| Infrastructure | Mapeamento e uma migration. |
| Web | Lista e formulário da conta. Na home do evento, escolher cliente da conta e desvincular. |

E-mail, se preenchido, tem formato válido. Telefone não tem máscara. Cliente de outra conta não vincula.

## Páginas

Autorização `User`. Busca por nome só na própria conta. O casal gravado no evento não é lido nem alterado por este plano.

## Persistência

Uma migration para cliente e para a tabela de vínculo. Único por conta, cliente e evento.

## Testes

Unitário: nome vazio e e-mail inválido. Integração: cadastro, busca entre duas contas, vínculo, desvínculo, vínculo repetido, um cliente em dois eventos e casal do evento preservado.

## Tarefas

1. Cadastro e lista.
2. Vínculo nos dois lados.
3. Migration e testes.

## Pendência que permanece

Cliente duplicado por nome ou e-mail. Remoção e o efeito dela nos vínculos.
