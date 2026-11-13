# 023. Tarefas do funil

**Status: rascunho.** Não autoriza código. RF-075 e RF-077 continuam Observado.

## 1. Identificador e nome

SPEC-023. Tarefas do funil.

## 2. Objetivo

Registrar a lista de tarefas do funil de vendas como fluxo próprio, separado das tarefas do evento. A observação não fechou o uso da lista.

## 3. Requisitos relacionados

RF-075, Observado. O texto cita tarefas de reunião, ligação, e-mail ou tarefa, junto do restante do funil. Não está aprovado. No corte do MVP, de RF-075 entra só o que RF-082 usa: quantidade e valor nos quatro estágios. O resto do funil fica de fora.

RF-077, Observado. Catálogos e impressões do funil. Esta spec não cobre relatório nem impressão. Não está aprovado.

RF-061 e a SPEC-003 são tarefas do evento. Não descrevem esta lista.

Não há RF Definido para filtro, aba, tipo ou formulário desta tela. Onde falta RF, a regra fica não definida.

## 4. Atores e permissões

A tela foi vista na conta autenticada da referência. Quem cria, quem filtra e quem pode ser responsável não foi definido. O lead consta no ator de RF-075, e o uso desta lista por lead não foi visto. Outra conta não foi comparada.

## 5. Pré-condições

O fluxo não está no Must. A conta autenticada é a da referência. A SPEC-001 cobre a sessão já aprovada e não abre este módulo. Oportunidade do funil ainda não tem spec. Não se viu se esta lista exige oportunidade.

## 6. Fluxo principal

O que deu para ver, sem gravação:

1. A lista de tarefas do funil mostra filtros de prazo, de tipo e de responsável.
2. O filtro de tipo lista Reunião, Ligação, Email e Tarefa.
3. A lista tem as abas Incompletas e Completas.
4. As duas abas estavam vazias.
5. A ação Adicionar tarefa está na tela e não foi usada.

Não há passo de salvar. O formulário não foi aberto.

## 7. Fluxos alternativos

- Adicionar tarefa: pendente. O formulário não entra nesta spec.
- Prazo e responsável: o filtro aparece. Opções e efeito sobre a lista não foram vistos.
- Abas sem item: não se viu texto de vazio nem coluna de uma tarefa.
- Tarefa do evento: não se viu esta lista misturada com a SPEC-003. Se o mesmo registro aparece nos dois lugares, não está definido.
- Relatório ou impressão do funil: fora. RF-077 permanece Observado.

## 8. Regras de negócio

- Esta lista não segue a SPEC-003. Situação de tarefa do evento não é regra daqui. RF-061 não cobre este fluxo.
- Os quatro rótulos de tipo foram vistos. RF-075, Observado, cita reunião, ligação, e-mail ou tarefa. Isso não aprova o conjunto, a obrigatoriedade nem a grafia Email ou e-mail. Regra do tipo: não definida.
- O efeito de filtrar por prazo, tipo ou responsável: não definido. Não havia item na lista.
- Incompleta e Completa são abas vistas. A passagem de uma para a outra: não definida.
- Vínculo com estágio, oportunidade, evento ou agenda: não definido.
- Quem é o responsável: não definido. Não vale, aqui, a regra da SPEC-003 de que o responsável é a própria conta.
- Esta spec não altera o dashboard do funil, RF-082, nem cria estágio.

## 9. Validações

Nenhuma validação foi vista. O formulário não foi aberto. Obrigatório, formato de prazo e tamanho de texto: não definidos.

## 10. Estados

Incompletas e Completas, as duas vazias na observação. Não se viu tarefa em nenhuma aba. Não há mapeamento definido para pendente, em andamento, concluída ou atrasada.

## 11. Dados necessários

Rótulos vistos no filtro de tipo: Reunião, Ligação, Email e Tarefa. Os demais dados de uma tarefa do funil não foram vistos. Não há checklist, comentário, anexo nem serviço nesta spec, e também não se afirma que existam.

## 12. Interface

Filtros de prazo, tipo e responsável. Tipo com os quatro rótulos acima. Abas Incompletas e Completas. Ação Adicionar tarefa, não acionada. Coluna, ordem, paginação, busca e texto de lista vazia: não vistos.

## 13. Erros

Nenhum erro foi visto. Mensagem de validação, falha ao gravar e lista de outra conta: não definidas.

## 14. Critérios de aceite

1. A spec não descreve a tarefa do funil com as regras da SPEC-003.
2. Ficam registrados apenas os filtros de prazo, tipo e responsável, os rótulos Reunião, Ligação, Email e Tarefa, e as abas Incompletas e Completas vazias.
3. Não há critério para criar, editar, concluir, filtrar com resultado ou imprimir.
4. O formulário de Adicionar tarefa permanece de fora.
5. RF-075 e RF-077 seguem Observado. O status desta spec segue rascunho.

## 15. Cenários de teste

Não há cenário de produto. A lista estava vazia e o formulário não foi aberto. Enquanto isso, a revisão confere se a spec não inventa campo, não copia situação da SPEC-003 e não trata Observado como aprovado.

## 16. Dependências

SPEC-001, só como conta autenticada já especificada. SPEC-007 mostra quantidade e valor do funil e não fornece tarefa. SPEC-003 não é fonte desta lista. Cadastro de oportunidade continua sem spec.

## 17. Premissas e pontos não definidos

Fora do Must. A observação é a tela de tarefas do funil da referência, com as duas abas vazias. Adicionar tarefa não foi usado.

Pendente: campos e obrigatoriedade do formulário; opções do filtro de prazo; opções do filtro de responsável; efeito dos três filtros; como uma tarefa nasce e como passa a completa; se incompleta e completa esgotam os estados; vínculo com oportunidade, estágio, evento e agenda; isolamento entre contas; edição; exclusão; ordenação; texto de vazio; se o tipo Email envia mensagem; relatório e impressão. Nenhum desses pontos vira regra sem RF Definido.

Esta spec não altera RF-035. Spec aprovada é que limita o código. Este arquivo ainda não é essa spec.
