# Plano 005. Perfil da assessoria

**Status: proposta.** Spec [005-perfil-da-assessoria.md](../specs/005-perfil-da-assessoria.md). Depende do plano 001.

## Fora deste plano

Troca de e-mail, validação de CPF ou CNPJ, endereço dividido, foto, assistente e perfil de outra conta.

## Camadas

| Camada | O que entra |
|---|---|
| Domain | `Perfil`. Um por conta. Data de nascimento, se houver, não é futura. |
| Application | Obter e salvar o perfil da conta autenticada. |
| Infrastructure | Mapeamento e uma migration. |
| Web | Uma página do `User`. O e-mail é lido do Identity e renderizado desabilitado. O PageModel não grava o e-mail. |

Os campos de empresa não entram em `ApplicationUser`.

## Páginas

`/perfil`, autorização `User`. Primeira visita mostra o e-mail e o restante vazio. Salvar substitui só os campos editáveis.

## Persistência

Uma migration. Índice único pela conta.

## Testes

Unitário: data futura recusada. Integração: primeiro salvamento, segundo salvamento, documento e endereço livres, data inválida sem gravar, duas contas isoladas, e-mail inalterado.

## Tarefas

1. Entidade e casos de uso.
2. Página com e-mail somente leitura.
3. Migration e testes.

## Pendência que permanece

Obrigatoriedade dos campos além do e-mail. Tamanho dos textos e máscara de celular ou documento.
