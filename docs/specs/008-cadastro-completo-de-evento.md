# 008. Cadastro completo de evento

**Status: aprovada.** O código segue esta spec. Incremento sobre a SPEC-002. Fonte observada: [inventario-referencia.md](../inventario-referencia.md) §1.3 e §1.5.

## 1. Identificador e nome

SPEC-008. Cadastro completo de evento.

## 2. Objetivo

O formulário de criar e editar evento passa a ter os campos vistos na referência, inclusive modelo, endereço dos locais e o valor da assessoria logo após o salvar.

## 3. Requisitos relacionados

RF-049, RF-053. RF-050 só no que a home já mostra a partir desses dados. Fora: site Casar.com (RF-079), equipe do dia (RF-051), modelo editor interno além de aplicar na criação.

## 4. Atores e permissões

O User da conta cria e edita os próprios eventos em andamento. Outro User não vê.

## 5. Pré-condições

Conta autenticada, SPEC-001. Existe pelo menos um modelo na conta, RF-053. A SPEC-002 continua valendo para lista, abas, concluir e cancelar.

## 6. Fluxo principal

1. O usuário escolhe o modelo. O restante do formulário só aparece depois dessa escolha. O modelo define o tipo.
2. Informa foto opcional, nomes das pessoas do evento, data, horário, budget, convidados estimados, local da cerimônia, local da recepção, tags, notificações e se grava na agenda.
3. Salva. O sistema pede o valor do serviço da própria assessoria: valor total, quantidade de parcelas, data da primeira parcela e, se marcado, entrada.
4. O evento nasce em andamento, com os serviços e tarefas do modelo, e com o serviço de assessoria contratado se o valor foi informado.
5. Na edição, quatro abas: Informações gerais, Detalhes do evento, Cerimônia, Recepção.

## 7. Fluxos alternativos

- Sem modelo: não grava.
- Nomes das pessoas vazios: não grava.
- Interruptor de cadastrar cliente ligado: abre os blocos de cliente da SPEC-009. Sem e-mail válido, não foi testado o envio.
- Agenda marcada sem Google: o evento entra na agenda da conta; o vínculo Google fica vazio.
- Desistir do valor da assessoria: não definido. Na referência o passo aparece depois do salvar.

## 8. Regras de negócio

- Modelo é obrigatório na referência. A SPEC-002 deixou isso de fora; este incremento adota o que foi visto.
- O nome do evento no casamento nasce da junção dos dois nomes com `&`.
- Tags são da conta, com criação livre. Vistas: Assessoria completa, Assessoria parcial, Assessoria final.
- Notificações por e-mail: assessor(a), assistentes, clientes. Vêm ligadas.
- Horário do cadastro é o horário do convite. Detalhes guardam também tempo de festa, início da cerimônia e início da recepção.
- CEP completo preenche endereço, estado, cidade e bairro.
- O valor da assessoria cria o serviço "Assessoria e Organização" com a própria conta como fornecedor contratado e gera parcelas, RF-068.

## 9. Validações

Modelo obrigatório. Nomes das pessoas obrigatórios, sem só espaços. Data e hora, se preenchidas, válidas. Budget e convidados, se preenchidos, maiores ou iguais a zero. E-mail de cliente, se o interruptor estiver ligado, formato válido. Valor total da assessoria, se informado, maior que zero. Quantidade de parcelas, se o valor existe, inteiro maior que zero.

## 10. Estados

Não muda a máquina da SPEC-002: nasce em andamento.

## 11. Dados necessários

Criar: foto, modelo, nomes (rótulo conforme o tipo), data, horário, budget, convidados estimados, local da cerimônia, local da recepção, tags, três notificações, salvar na agenda, vincular Google.

Depois do salvar: incluir valor de entrada, base (valor ou porcentagem), valor de entrada, data de entrada, valor total, quantidade de parcelas, data da primeira parcela.

Editar, Informações gerais: foto, nomes, data, horário (convite), budget estimado, URL do site (somente leitura, RF-079), senha do site, tags, três notificações.

Editar, Detalhes: quantidade estimada de convidados, tempo de festa, horário de início da cerimônia, horário de início da recepção, quantidade de padrinhos, quantidade de damas e pajens.

Editar, Cerimônia e Recepção, cada uma: local, país, CEP, endereço, número, estado, cidade, bairro.

## 12. Interface

Formulário de criação com modelo no topo. Modal de valor da assessoria. Edição em quatro abas e botão Salvar. A home monta o link do mapa a partir do endereço.

## 13. Erros

Campo inválido permanece no formulário. Evento de outra conta não revela dado. Falha ao gravar o valor da assessoria não apaga o evento já criado; o estado desse passo não foi visto com falha real.

## 14. Critérios de aceite

1. Sem modelo, o evento não grava.
2. Com modelo e nomes, o evento entra em andamento na conta, com os serviços do modelo.
3. Dois nomes de casamento viram o nome do evento com `&`.
4. O passo do valor da assessoria gera o serviço contratado e as parcelas informadas.
5. A edição grava horário de convite, tempo de festa, horários de cerimônia e recepção e o endereço completo de cada local.
6. CEP preenchido completa endereço, estado, cidade e bairro.
7. Outra conta não vê o evento.

## 15. Cenários de teste

Criar sem modelo. Criar com o modelo de casamento da conta, data, budget e tag. Informar valor total e quatro parcelas. Editar os endereços via CEP. Conferir isolamento entre contas.

## 16. Dependências

SPEC-001, SPEC-002 (lista e situações), RF-053 para existir modelo, RF-068 para as parcelas da assessoria.

## 17. Premissas e pontos não definidos

Lista fechada de Tipo ao criar modelo. O que acontece se o usuário fecha o modal de valor da assessoria. Campos obrigatórios de endereço. Tamanho máximo dos textos. Integração Google Agenda.
