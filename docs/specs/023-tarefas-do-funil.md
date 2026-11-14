# 023. Tarefas do funil

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-023. Tarefas do funil.

## 2. Objetivo

O Organizer gerencia tarefas comerciais do funil — reunião, ligação, e-mail e tarefa — distintas das tarefas do evento da SPEC-003: cria, edita, conclui, reabre, exclui e filtra.

## 3. Requisitos relacionados

RF-075, no recorte de tarefas de reunião, ligação, e-mail ou tarefa. Oportunidade é a SPEC-020. SPEC-003 e RF-061 não descrevem esta lista. RF-079 fica fora.

## 4. Atores e permissões

O `Organizer` opera as tarefas comerciais da própria conta. Outro `Organizer` não vê. `Admin` acessa qualquer conta. `Client` não acessa. Assistente e fornecedor com login ficam fora. Lead não usa esta lista.

## 5. Pré-condições

Conta autenticada, SPEC-001. A tarefa comercial exige oportunidade da mesma conta, SPEC-020.

## 6. Fluxo principal

1. O Organizer abre Tarefas no funil.
2. Vê filtros de prazo, tipo e responsável, as abas Incompletas e Completas, e a ação Adicionar tarefa.
3. Cria tarefa com título, oportunidade, tipo, responsável e prazo; observação e anexo são opcionais.
4. A tarefa nasce incompleta e aparece na aba Incompletas.
5. Edita os campos e salva.
6. Conclui a tarefa; ela passa à aba Completas.
7. Reabre uma completa; volta a Incompletas.
8. Exclui quando não precisar mais dela.

## 7. Fluxos alternativos

- Abas vazias: a tela informa que não há tarefa.
- Título, oportunidade, tipo, responsável ou prazo vazios: não grava.
- Filtro por prazo, tipo ou responsável: restringe a lista da própria conta.
- Tipo E-mail: não envia mensagem sozinho; é só classificação da tarefa.
- Integração com Google Agenda fica fora deste corte.
- Tarefa do evento da SPEC-003 não aparece nesta lista e vice-versa.

## 8. Regras de negócio

- Esta lista é comercial e vinculada a oportunidade. Não usa situações pendente/em andamento/concluída/atrasada da SPEC-003.
- Tipos: Reunião, Ligação, E-mail e Tarefa.
- Estados de lista: incompleta e completa.
- Responsável, neste corte, é a conta do Organizer (ou identificador interno da mesma conta).
- Tarefa pertence à conta e à oportunidade informada.

## 9. Validações

Título, oportunidade, tipo, responsável e prazo obrigatórios. Tipo é um dos quatro. Prazo é data válida. Oportunidade da mesma conta.

## 10. Estados

Incompleta e Completa. Exclusão remove o registro.

## 11. Dados necessários

Título, observação opcional, oportunidade, tipo, responsável, prazo, anexo opcional, situação incompleta/completa.

## 12. Interface

Filtros de prazo, tipo e responsável. Abas Incompletas e Completas. Formulário Nova tarefa. Angular Material, DA-044. Sem vínculo Google.

## 13. Erros

Campo inválido indica o campo. Item de outra conta fica indisponível. Falha ao gravar mantém a situação anterior. Lista vazia não é erro.

## 14. Critérios de aceite

1. A lista de tarefas do funil é distinta da SPEC-003.
2. Criar com título, oportunidade, tipo, responsável e prazo coloca a tarefa em Incompletas.
3. Tipos disponíveis: Reunião, Ligação, E-mail e Tarefa.
4. Concluir move para Completas; reabrir volta a Incompletas.
5. Editar e excluir alteram só a própria conta.
6. Filtros não mostram tarefa de outra conta.
7. Sem os obrigatórios, nada é gravado.

## 15. Cenários de teste

Abas vazias. Criar cada tipo. Concluir e reabrir. Filtrar. Editar e excluir. Duas contas. Confirmar que tarefa de evento não aparece.

## 16. Dependências

SPEC-001 e SPEC-020. SPEC-003 permanece o fluxo de tarefa de evento.

## 17. Premissas e pontos não definidos

Comportamento do lembrete além do prazo gravado. Texto exato da lista vazia.
