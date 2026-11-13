# 025. Orçamentos do funil

**Status: rascunho.** Fonte: visita à lista de orçamentos do funil da referência. O fluxo não está no MVP e não está confirmado.

## 1. Identificador e nome

SPEC-025. Orçamentos do funil.

## 2. Objetivo

Descrever a lista de orçamentos do funil no que a visita mostrou: filtro de criação, busca e as abas Em andamento, Respondidos e Cancelados, as três vazias. Criar, responder, cancelar e enviar não foram acionados. O modelo de orçamento permanece na SPEC-027.

## 3. Requisitos relacionados

RF-075 cita orçamentos entre as peças do funil. RF-077 cita catálogos de funil. Os dois estão Observado e não aprovam regra desta spec. RF-082 cobre quantidade e valor no dashboard e não descreve esta lista. Não há RF Definido para a lista. Onde falta RF, a regra fica não definida.

## 4. Atores e permissões

A tela foi vista na conta autenticada da assessoria. Lead, cliente, assistente e fornecedor não apareceram nesta visita. Quem pode abrir, criar, responder, cancelar ou enviar fica não definido.

## 5. Pré-condições

Conta autenticada na referência, SPEC-001. Não foi visto se a lista exige oportunidade, evento, cliente, fornecedor ou modelo de orçamento.

## 6. Fluxo principal

1. A assessoria abre a lista de orçamentos do funil.
2. A tela mostra filtro de criação, busca e as abas Em andamento, Respondidos e Cancelados.
3. Na visita, as três abas estavam vazias.

## 7. Fluxos alternativos

- Criar: não acionado. Formulário, campos e resultado ficam não definidos.
- Responder: não acionado. Quem responde e o que a ação grava ficam não definidos.
- Cancelar: não acionado. Efeito e pedido de motivo ficam não definidos.
- Enviar: não acionado. Destino e canal ficam não definidos.
- Lista com itens: não vista. Colunas, ordenação e conteúdo ficam não definidos.
- Filtro de criação ou busca com critério preenchido: não exercidos. O que cada um restringe fica não definido.

## 8. Regras de negócio

- Não há RF Definido para esta lista. Nenhuma regra abaixo fecha o produto.
- As abas vistas têm os rótulos Em andamento, Respondidos e Cancelados. A permanência desses nomes no produto fica não definida. RF-075, Observado, não decide isso.
- Filtro de criação e busca foram vistos. O critério de cada um fica não definido.
- Criar, responder, cancelar e enviar não foram acionados. O efeito de cada um fica não definido.
- O modelo de orçamento permanece na SPEC-027. Esta spec não descreve o modelo.
- O quadro do funil e o dashboard da SPEC-007 permanecem como estão. RF-082 não muda com esta lista.
- A relação desta lista com orçamento de fornecedor, negociação do evento e budget do evento fica não definida. RF-057, RF-059 e RF-058 estão Observado e não foram vistos nesta tela.
- A relação desta lista com os relatórios de RF-077 fica não definida. RF-077 está Observado e o relatório não foi aberto nesta visita.

## 9. Validações

Nenhuma validação foi vista. Campo obrigatório, formato e limite ficam não definidos.

## 10. Estados

A visita mostrou três rótulos de aba, sem item em nenhuma. Transição entre Em andamento, Respondidos e Cancelados fica não definida. Nenhum outro estado foi visto.

## 11. Dados necessários

Nenhum campo de orçamento foi visto. A lista estava vazia e criar não foi aberto. Os dados do orçamento ficam não definidos.

## 12. Interface

Três abas com os rótulos Em andamento, Respondidos e Cancelados. Há filtro de criação e busca. O rótulo exato do filtro e o da busca não foram transcritos e ficam não definidos. Sem linha visível, colunas e texto de lista vazia ficam não definidos. O endereço visto na referência não vira rota deste produto.

## 13. Erros

Nenhuma falha foi provocada. Mensagem de erro e o resultado de falha ao criar, responder, cancelar ou enviar ficam não definidos. A lista vazia da visita não foi apresentada como erro.

## 14. Critérios de aceite

Aceite de produto permanece pendente enquanto não houver RF Definido. A visita permite verificar só o seguinte:

1. A lista de orçamentos do funil mostra as abas Em andamento, Respondidos e Cancelados.
2. A mesma tela mostra filtro de criação e busca.
3. Na conta visitada, as três abas ficam sem orçamento.

Criar, responder, cancelar e enviar ficam sem critério de aceite.

## 15. Cenários de teste

Abrir a lista sem orçamento: as três abas aparecem vazias, com filtro de criação e busca. Criar, responder, cancelar e enviar: condição não executada, resultado não definido. Filtro de criação ou busca com valor preenchido: condição não executada, resultado não definido.

## 16. Dependências

SPEC-001, pela conta autenticada. SPEC-007 e RF-082 não dependem desta lista. SPEC-027 cobre o modelo de orçamento e fica fora. O MVP deixa o restante do funil de fora. De RF-075, o corte aprovado usa só o que RF-082 já mostra no dashboard.

## 17. Premissas e pontos não definidos

- Aprovar esta lista para o produto. RF-075 e RF-077 continuam Observado.
- O que o filtro de criação restringe.
- O que a busca compara.
- Rótulos exatos do filtro e da busca.
- Colunas, ordenação, contagem nas abas e texto quando a lista está vazia.
- Campos, validações e resultado de criar.
- Quem responde, o que fica gravado e se a aba muda.
- Efeito de cancelar, inclusive motivo.
- Destino e canal de enviar.
- Se os rótulos das abas são situações com transição.
- Quem, além da conta autenticada da assessoria, acessa a lista.
- Isolamento entre contas nesta tela.
- Ligação com oportunidade, evento, cliente, fornecedor e budget do evento.
- Uso do modelo da SPEC-027 dentro de um orçamento desta lista.
- Ligação com relatório ou impressão.
- Mensagens de erro.
