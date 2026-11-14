# 020. Oportunidades do funil

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-020. Oportunidades do funil.

## 2. Objetivo

O Organizer gerencia o quadro de oportunidades do funil: cria, edita, exclui, filtra, exporta e move entre os quatro estágios, inclusive descarte com motivo. Os números alimentam o dashboard da SPEC-007.

## 3. Requisitos relacionados

RF-075, no recorte de oportunidades, quadro, filtro, exportação, descarte com motivo, formulário público de lead e checagem de data. RF-082 e SPEC-007 para quantidade e valor nos mesmos quatro estágios. Contratos, orçamentos e tarefas comerciais ficam nas SPEC-022, SPEC-025 e SPEC-023. RF-079 fica fora.

## 4. Atores e permissões

O `Organizer` cria, lista, edita, exclui, filtra, exporta e muda estágio das oportunidades da própria conta. Outro `Organizer` não vê. `Admin` acessa qualquer conta. `Client` não acessa o funil neste corte. Lead preenche só o formulário público, sem login. Assistente e fornecedor com login ficam fora.

## 5. Pré-condições

Conta autenticada, SPEC-001. O dashboard da SPEC-007 reflete as mesmas oportunidades.

## 6. Fluxo principal

1. O Organizer abre Oportunidades no funil de vendas.
2. Vê o quadro com quatro colunas, nesta ordem: Contato inicial, Orçamento enviado, Contratado e Descartado. Cada coluna mostra quantidade e valor da conta.
3. Cria oportunidade com nome, estágio, e ao menos um contato; opcionalmente fonte, valor da proposta, responsável, serviço/produto, dados do evento previsto, locais e demais contatos.
4. A oportunidade aparece no cartão da coluna do estágio escolhido; quantidade e valor da coluna e do dashboard atualizam.
5. Edita os mesmos campos e salva.
6. Move a oportunidade entre estágios (exceto o descarte, que exige motivo).
7. Exporta a lista filtrada da conta.
8. Exclui a oportunidade quando não precisar mais dela.

## 7. Fluxos alternativos

- Conta sem oportunidade: as quatro colunas ficam em 0 e R$ 0,00.
- Nome vazio ou sem contato: não grava.
- Mover para Descartado: exige motivo; sem motivo não grava o descarte.
- Filtro por responsável, tipo de evento, data prevista e ordenação: restringe o quadro à própria conta.
- Busca por nome da oportunidade: restringe os cartões visíveis.
- Formulário público de lead: o visitante envia dados mínimos; a oportunidade nasce em Contato inicial na conta dona do link.
- Checagem de data: ao informar data prevista, o sistema informa se a agenda da conta já tem compromisso naquela data; o Organizer decide se grava mesmo assim.
- Estágios configuráveis e “gerenciar estágios” da referência ficam fora: os quatro estágios são fixos, alinhados à SPEC-007.

## 8. Regras de negócio

- Os estágios são só os quatro da SPEC-007 e de RF-075 vistos na conta: contato inicial, orçamento enviado, contratado e descartado. Rótulo de produto: Orçamento enviado (não “Enviado orçamento”).
- Quantidade é o número de oportunidades da conta na coluna. Valor é a soma dos valores informados na coluna. Oportunidade sem valor não soma.
- Oportunidade pertence à conta do Organizer.
- Descarte exige motivo. Motivo fica gravado e alimenta o relatório de motivos da SPEC-024.
- Serviço/produto usa o catálogo da SPEC-026 quando existir item; o Organizer também pode informar texto livre na criação.
- Responsável, neste corte sem assistente, é a própria conta do Organizer ou outro identificador interno da mesma conta; não há atribuição a outra conta.
- Exportação cobre as oportunidades da conta no filtro atual.
- RF-079 e peças da loja/vitrine/MeLink ficam fora.

## 9. Validações

Nome da oportunidade obrigatório. Estágio obrigatório e um dos quatro. Ao menos um contato com nome. Valor, se preenchido, é número não negativo. Data prevista, se preenchida, é data válida. Motivo obrigatório no descarte.

## 10. Estados

Contato inicial, Orçamento enviado, Contratado e Descartado. Criação pode nascer em qualquer estágio, salvo o formulário público, que nasce em Contato inicial. Exclusão remove o registro.

## 11. Dados necessários

Nome, estágio, fonte opcional, valor opcional, responsável, serviço/produto, contatos (nome e, se houver, telefone e e-mail), tipo do evento, quantidade estimada de convidados, data prevista, horários e quantidades de padrinhos/damas quando informados, local e endereço de cerimônia e de recepção, motivo de descarte quando aplicável.

## 12. Interface

Quadro em quatro colunas com quantidade e valor. Busca, filtro, adicionar, editar, excluir e exportar. Formulário em blocos: sobre a oportunidade, sobre o evento, cerimônia, recepção e contatos. Angular Material, DA-044. Sem gerenciar estágios.

Textos dos estágios: Contato inicial, Orçamento enviado, Contratado e Descartado.

## 13. Erros

Campo inválido indica o campo. Descarte sem motivo não grava. Item de outra conta fica indisponível. Falha ao gravar mantém o estágio anterior. Quadro vazio não é erro.

## 14. Critérios de aceite

1. O quadro mostra os quatro estágios com quantidade e valor da própria conta.
2. Sem oportunidade, cada coluna mostra 0 e R$ 0,00.
3. Criar com nome, estágio e contato coloca o cartão na coluna e atualiza o dashboard da SPEC-007.
4. Sem nome ou sem contato, nada é gravado.
5. Editar e excluir alteram só a conta do Organizer.
6. Mover para Descartado exige motivo.
7. Filtro e exportação não incluem outra conta.
8. Lead pelo formulário público nasce em Contato inicial.
9. Não há coluna além dos quatro estágios nem tela de gerenciar estágios.

## 15. Cenários de teste

Quadro vazio. Criar, editar, excluir. Mudar entre os três estágios ativos. Descartar com e sem motivo. Filtrar e exportar. Duas contas. Lead público. Data prevista com compromisso na agenda. Conferir SPEC-007.

## 16. Dependências

SPEC-001, SPEC-007 e SPEC-019 para a checagem de data. SPEC-026 para serviço/produto cadastrado. SPEC-021, SPEC-022, SPEC-023, SPEC-024 e SPEC-025 complementam o funil.

## 17. Premissas e pontos não definidos

Lista fechada de motivos de descarte. Se a checagem de data um dia passar a bloquear em vez de só avisar.
