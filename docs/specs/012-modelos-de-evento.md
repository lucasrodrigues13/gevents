# 012. Modelos de evento

**Status: rascunho.** RF-053 está Observado. Esta spec não o promove a aprovado e não autoriza código. O corte aprovado em [mvp-scope.md](../mvp-scope.md) deixa RF-053 de fora. A criação do evento com modelo obrigatório permanece na SPEC-008.

Fonte desta leitura: a lista de modelos e o editor de um modelo já existente. Criar, clonar, ordenar, salvar e excluir não foram acionados.

## 1. Identificador e nome

SPEC-012. Modelos de evento.

## 2. Objetivo

A assessoria consulta a biblioteca de modelos e abre o editor de um modelo existente, no que a leitura mostrou: tipo na lista, abas do editor e checklist do serviço.

## 3. Requisitos relacionados

RF-053, status Observado.

Permanece na SPEC-008, sem reescrita: modelo obrigatório na criação do evento, tipo definido por esse modelo, e o evento nascendo com os serviços e as tarefas do modelo. RF-049 continua dono desse formulário.

Fora desta spec: bibliotecas de briefing, roteiro, cronograma, inspiração e mensagem (RF-054), tarefas do evento já aprovadas (SPEC-003, RF-061), permissões do portal do cliente (RF-056), loja de modelos (RF-079).

## 4. Atores e permissões

O ator visto é a assessoria, na conta autenticada. A role `User` continua sem ser papel de negócio, RF-040. Assistente e a matriz de RF-045 não entram.

O cliente não abre esta biblioteca. O que as permissões do modelo mudam para ele está sem RF e fica não definido.

## 5. Pré-condições

Conta autenticada, SPEC-001. Há um modelo existente para abrir o editor. Não está definido se os modelos dos cinco tipos nascem com a conta, vêm da plataforma ou são gravados pela assessoria.

## 6. Fluxo principal

1. O usuário abre a biblioteca de modelos.
2. A lista mostra modelos. O tipo de cada um é um dos vistos em RF-053: 15 anos, casamento, aniversário, corporativo, formatura.
3. O usuário abre um modelo que já existe.
4. O editor mostra as abas Checklist, Tarefas, Briefings, Roteiros, Inspirações, RSVP e Mensagens, e Permissões. RF-053.
5. Na Checklist, o serviço aparece com tarefas, fornecedores, alinhamentos, comparativos e valor estimado. RF-053.

## 7. Fluxos alternativos

- Biblioteca sem modelo: não foi vista. Não definido.
- Modelo de outra conta: não foi visto. Não definido.
- Criar, clonar, ordenar, salvar e excluir: Pendente. O controle não foi acionado. Não há passos, validação nem resultado.

## 8. Regras de negócio

- A lista exibe modelos dos cinco tipos de RF-053. Não está definido se a tela separa seções por tipo ou se o tipo é só o rótulo de cada modelo.
- A checklist do modelo lista, por serviço, tarefas, fornecedores, alinhamentos, comparativos e valor estimado. RF-053. O valor zero visto num modelo é dado daquele modelo, não regra de valor inicial.
- Aplicar o modelo na criação do evento permanece na SPEC-008.
- Grupos de tarefas, prazo “N dias antes”, marca flexível, vínculos das outras abas e o efeito de cada permissão sobre o cliente foram vistos na tela e não têm RF. A regra não está definida. Não alteram a SPEC-003, em que o prazo da tarefa do evento é uma data.

## 9. Validações

Nenhuma gravação foi feita. RF-053 registra que criar pede Nome e Tipo. Obrigatoriedade, lista fechada de Tipo, tamanho, duplicidade e o que o salvar rejeita continuam não definidos. A SPEC-008 já deixou a lista fechada de Tipo em aberto.

## 10. Estados

Não foi vista situação do modelo. Não definido.

## 11. Dados necessários

Na lista, o que a leitura sustenta com RF-053: nome do modelo e tipo, entre os cinco tipos.

Na checklist: serviço, tarefas, fornecedores, alinhamentos, comparativos e valor estimado.

Grupo de tarefas, quantidade de dias antes do evento, marca flexível, vínculo de briefing, roteiro, inspiração, RSVP e mensagem, e o conteúdo de cada grupo de permissão: vistos na tela, sem RF. Não entram como dado decidido.

## 12. Interface

Biblioteca com os modelos e o tipo de cada um. Editor com as sete abas de RF-053. Checklist com o serviço e os cinco itens da seção 11.

Nesta leitura, a aba Tarefas mostrou grupos e prazo relativo no formato “N dias antes”, com marca flexível. As abas Briefings, Roteiros, Inspirações e RSVP e Mensagens mostraram vínculos. A aba Permissões mostrou grupos que tratam do que o cliente vê e edita: gerais, planejamento, tarefas, serviços e fornecedores, convites. Isso é registro da tela. Sem RF, não é regra.

Criar, clonar, ordenar, salvar e excluir não têm tela de fluxo nesta spec.

## 13. Erros

Nenhuma falha foi provocada. Mensagem de erro, modelo inexistente e modelo de outra conta: não definido.

## 14. Critérios de aceite

Enquanto o status for rascunho e RF-053 for Observado, o critério descreve a leitura. Não autoriza implementação.

1. A biblioteca lista modelos cujo tipo é 15 anos, casamento, aniversário, corporativo ou formatura.
2. O editor de um modelo existente mostra as abas Checklist, Tarefas, Briefings, Roteiros, Inspirações, RSVP e Mensagens, e Permissões.
3. Na checklist, o serviço mostra tarefas, fornecedores, alinhamentos, comparativos e valor estimado.
4. O formulário de criar evento continua o da SPEC-008.
5. Criar, clonar, ordenar, salvar e excluir não têm critério de aceite. O fluxo está pendente.

## 15. Cenários de teste

Abrir a biblioteca e ler o tipo de cada modelo. Abrir um modelo existente e ler a checklist de um serviço. Confirmar as sete abas. Não inclui criar, clonar, ordenar, salvar nem excluir.

## 16. Dependências

SPEC-001. SPEC-008 consome o modelo na criação do evento e não muda com esta spec. SPEC-003 permanece a regra aprovada da tarefa do evento.

RF-054, RF-056, RF-061, RF-065, RF-069, RF-071 e RF-072 nomeiam peças que aparecem como aba ou vínculo. O miolo dessas peças não é desta spec.

## 17. Premissas e pontos não definidos

Pendente. O fluxo não foi acionado; não há passos para especificar:

- Criar modelo. O pedido de Nome e Tipo está em RF-053. As opções de Tipo não foram abertas e o salvar não foi acionado.
- Clonar modelo.
- Ordenar.
- Salvar o editor.
- Excluir modelo.

Não definido. Visto nesta leitura, sem RF; a regra não foi decidida:

- Tarefas do modelo em grupos, prazo “N dias antes” e marca flexível. O que “flexível” altera, e como o prazo vira data no evento, não foi decidido.
- Vínculos de Briefings, Roteiros, Inspirações e RSVP e Mensagens. Não foi decidido o que o vínculo guarda nem se aponta para a biblioteca de RF-054.
- Permissões do modelo nos grupos gerais, planejamento, tarefas, serviços e fornecedores, e convites. Não foi decidido o que o cliente vê e o que edita em cada grupo. Essa lista não é a matriz de RF-056.
- Se a lista separa seções por tipo.
- Se os modelos dos cinco tipos pertencem à conta ou à plataforma.
- Efeito de alterar um modelo sobre eventos já criados.
- Busca na biblioteca. Não está em RF-053 e não entra como regra.
