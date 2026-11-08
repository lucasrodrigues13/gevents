# Plano 004. Convites e RSVP

**Status: proposta.** Spec [004-convites-e-rsvp.md](../specs/004-convites-e-rsvp.md). Depende do plano 002.

## Fora deste plano

Importação, exportação, impressão, mapa, portaria, QR Code, RSVP automático, link único do evento, e-mail de campanha e edição pública de faixa, custo, gênero, celular, observação, grupo ou transfer.

Segunda resposta, inclusão de convidado pelo link, correção pela assessoria e remoção não entram. O primeiro envio grava. Uma abertura depois de respondido mostra o que foi gravado e não grava de novo. Isso não é uma regra de negócio nova. É o limite do que a spec fechou.

## Camadas

| Camada | O que entra |
|---|---|
| Domain | `Grupo`, `Convite` e `Convidado`. Nome obrigatório. Situação inicial pendente. Transfer só com origem. Valores fechados de faixa, custo, gênero e situação. |
| Application | Manter grupos, convites e convidados. Obter o convite pelo link. Registrar acesso e gravar a presença. |
| Infrastructure | Mapeamento, uma migration e geração do identificador do link. O identificador não é sequência adivinhável. |
| Web | Páginas autenticadas do evento e uma página pública sem layout da conta. |

Grupo pertence à conta. Convite e convidado pertencem ao evento da conta. Os nomes de grupo da referência não são semente obrigatória.

## Páginas

| Página | Quem |
|---|---|
| Grupos, convites, convidados, duas visões, busca, filtros, totais e copiar link | `User` |
| Presença | Quem tem o link, sem login |

A home do evento passa a ler os totais por situação.

## Persistência

Uma migration. O link guarda só o identificador opaco do convite. Histórico deste plano: acessada e respondida, com o momento.

## Testes

Unitário: nome vazio, transfer sem origem, valor fora das listas. Integração: dois convidados com respostas diferentes, filtros, totais, sessão anônima, link alterado, segunda conta, envio que falha sem mudar a situação, segunda abertura sem gravar de novo.

## Tarefas

1. Grupos, convites e convidados da conta.
2. Link e página pública só com a presença.
3. Totais na home.
4. Migration e testes.

## Pendência que permanece

Máscara de celular, tamanho dos textos e o que acontece com o link se um dia houver remoção.
