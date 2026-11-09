# Plano 010. Convites e convidados completo

**Status: aprovada.** Spec [010-convites-convidados-completo.md](../specs/010-convites-convidados-completo.md). Depende do plano 004 já implementado.

## Fora deste plano

RSVP automático, formulário público além do que a SPEC-004 já grava, mapa de mesas, checklist de presença, impressão, exportação, envio em massa e exclusão de todos os convites. A aba Histórico de mensagens existe e fica vazia. O que ela mostraria depois de um disparo não foi definido.

Não há semente com os cinco grupos vistos. Grupo continua sendo só um nome criado pela conta.

## Camadas

| Camada | O que entra |
|---|---|
| Domain | Telefone no convidado e no convite. O do convite aponta para o convidado principal. Transfer do evento com locais. Convidado com mesa, RG e CPF. Convite novo exige um convidado. |
| Application | Manter esses campos, ligar o transfer, importar a planilha e montar o QR Code a partir do link que já existe. |
| Infrastructure | Mapeamento, uma migration, leitura de `.xlsx` e `.xls` e a imagem do QR Code. |
| Web | Os formulários do evento e o download da planilha modelo. PageModel não usa `DbContext`. |

A página pública da SPEC-004 não ganha campo. Continua só a presença.

## Convite e convidado

O convidado continua com DDI e celular. O convite não tem telefone digitado à parte. A conta escolhe um convidado deste convite que já tenha telefone. Esse convidado fica principal, um só, e o telefone exibido no convite é o dele. Convidado sem telefone, ou de outro convite, não entra nessa escolha. Sem escolha, o convite fica sem telefone e sem principal. Mudar o celular do principal atualiza o que o convite mostra. Tirar o principal do convite, ou apagar o celular dele, limpa os dois.

Busca e filtro de telefone olham o telefone do convite e o de cada convidado.

O grupo do formulário fica no convite. O filtro de grupo lê esse campo.

Convite novo sem nome ou sem ao menos um convidado não grava. Convidado exige nome. Situação, mesa, gênero, faixa, custo, RG e CPF são opcionais. Situação omitida nasce pendente. A assessoria pode gravar pendente, confirmado ou não comparecerá. Na tela, a recusa continua rotulada Cancelado no ícone e Não comparecerá na lista. É o mesmo valor.

Mesa é texto. Um nome novo é aceito.

## Transfer

O evento tem um interruptor. Desligado, o convite não mostra transfer nem origem. Ligado, a conta cadastra locais só com nome. No convite, Sim exige um desses locais. Não grava sem origem.

## Importação

A planilha modelo tem, nesta ordem: Nome do convite, DDI, DDD + telefone, Grupo do convite, Observação do convite, Nome dos convidados, Gênero, Faixa etária, Custo/pagamento, Situação, Mesa. O arquivo oferecido para baixar traz esse cabeçalho e nenhuma linha de exemplo.

`M` e `F` viram masculino e feminino. `Free` vira gratuita. Linha seguinte sem nome de convite continua no convite da linha anterior. O telefone da linha do convite grava no convidado dessa linha, e esse convidado nasce principal. Valor fora das listas, ou arquivo que não seja `.xlsx` ou `.xls`, não grava nada. Não há gravação parcial.

## QR Code

A edição do convite mostra o QR Code do link já existente, para baixar. Enviar para WhatsApp pede DDI, celular e mensagem e abre `wa.me` com esses dados. Não há API de WhatsApp.

## Páginas

| Página | Quem |
|---|---|
| Convites, convidados, grupos, transfer, importação, QR Code | `User` |
| Presença | Quem tem o link, como na SPEC-004 |

## Persistência

Uma migration: convidado principal no convite, local de transfer, mesa, RG e CPF. O celular do convidado permanece. O identificador do link não muda.

## Testes

Unitário: convite sem convidado, principal sem telefone, principal de outro convite, transfer ligado e Sim sem local, gênero ilegível na planilha. Integração: dois convidados e a escolha do principal, troca da escolha, telefone do principal alterado, transfer desligado sem o campo, transfer ligado com origem, planilha em que o telefone da linha do convite vira o principal, arquivo inválido sem gravar, QR Code do link, segunda conta sem ver o convite, página pública ainda só com a presença.

## Tarefas

1. Campos do convite e do convidado, com o celular nos dois e o principal no convite.
2. Transfer do evento e origem no convite.
3. Planilha modelo, importação e QR Code.
4. Migration e testes da spec.

## Pendência que permanece

Resultado visual de uma planilha rejeitada na referência. Conteúdo do histórico depois de um disparo. Envio que não seja o link `wa.me`. Excluir todos os convites.
