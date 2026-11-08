# Brief do produto

Documento de referência para o MVP. O levantamento foi feito em leitura sobre uma plataforma de gestão de cerimonial já em uso pelo cliente. Nenhum dado foi criado, alterado ou apagado. Nomes, contatos e documentos das pessoas da conta não entram aqui.

O cliente ainda não passou as melhorias. Elas entram depois, no backlog, sem mudar a estrutura deste brief.

## O que o produto é

Sistema de gestão para cerimonial e assessoria de eventos. O centro é o evento. Em volta dele ficam tarefas, fornecedores, orçamento, convidados, RSVP, pagamentos, roteiro e o comercial da assessoria.

O primeiro uso real é casamento. O modelo de evento também precisa nascer preparado para outros tipos: 15 anos, aniversário, corporativo e formatura.

## Quem usa

| Papel | O que faz |
|---|---|
| Assessoria | Dona da conta. Opera a plataforma inteira. |
| Assistente | Acessa só os módulos que a assessoria liberar. |
| Cliente | Casal ou contratante. Entra no próprio evento, com permissão limitada. |
| Fornecedor | Recebe link para completar dados, enviar orçamento e contrato. |
| Convidado | Confirma presença por link. Não tem conta na plataforma. |

## Mapa de módulos

**Gestão de eventos:** lista, home do evento, clientes, fornecedores, modelos.

**Dentro do evento:** Home, Convidados, Pagamentos, Tarefas, Roteiro, Cronograma, Briefing, Inspirações, Anexos.

**Convidados se divide em:** convites e convidados, RSVP e mensagens, mapeamento de mesa, checklist de presença.

**Operação da assessoria:** agenda, funil de vendas, gestão financeira.

**Fica de fora do produto:** cobrança automática de terceiros, site de casamento de marketplace, loja de modelos, link-in-bio, programa de indicação e área de vídeos da plataforma de referência.

## Regras já observadas

- Evento novo exige um modelo.
- Tarefas podem ser redatadas quando a data do evento muda.
- RSVP automático pode desligar envios se a data do evento mudar.
- Cliente e assistente só enxergam o que a permissão libera.
- Transfer de convidado exige local de origem.
- Compromisso na agenda exige data.
- Convidado responde por link, sem login.

## Recorte

**MVP**

1. Login, perfil da assessoria e assistentes com permissão.
2. Eventos, modelos de evento e home do evento.
3. Clientes vinculados ao evento.
4. Serviços, fornecedores e budget.
5. Tarefas, inclusive a visão de todos os eventos.
6. Convites, convidados e RSVP por link.
7. Parcelas do evento.
8. Agenda.

**Depois do MVP**

Funil de vendas, financeiro da empresa, roteiro, cronograma, briefing, inspirações, anexos, mapa de mesas, checklist de portaria, RSVP automático e relatórios.

**Fora, até o cliente pedir**

Cobrança por carteira de terceiros, vitrine e site de casamento de parceiro, loja de modelos, link-in-bio e programa de indicação.

---

## Épicos e features

Cada feature abaixo vira um card. As histórias ficam no checklist do card.

### 1. Acesso, perfil e equipe

Fase: MVP

- Login com e-mail e senha, recuperação de senha e cadastro da assessoria.
- Perfil da empresa: nome, responsável, e-mail, CPF ou CNPJ, celular, data de nascimento e endereço.
- Assistentes com permissão por módulo.
- Busca de eventos e ferramentas.
- Notificações internas.

Histórias:

- Como assessora, quero entrar com e-mail e senha para acessar só a minha operação.
- Como assessora, quero cadastrar assistentes e limitar o que cada um vê.
- Como assessora, quero manter os dados da empresa para contratos e comunicação saírem corretos.

### 2. Gestão de eventos

Fase: MVP

- Lista com abas: em andamento, concluído, cancelado e estatísticas.
- Busca por nome.
- Filtros: tipo, equipe presente, assistente, período, modelo e tags.
- Card com data, contagem regressiva, modelo, tags, tarefas feitas e convidados confirmados.
- Abrir, editar, cancelar, concluir, remover e imprimir a lista.
- Criar evento com imagem, modelo, casal (nome, papel, e-mail, celular), nome, data, hora, orçamento estimado, número estimado de convidados, local da cerimônia e local da recepção.
- Tags de pacote, como assessoria completa, parcial e final.
- Home do evento: contagem regressiva, código, equipe do dia, clientes, budget estimado e contratado, pagamentos pagos e atrasados, tarefas concluídas e atrasadas, convidados por situação, serviços por categoria e pendências do mês.
- Agrupar eventos.

Histórias:

- Como assessora, quero criar um evento a partir de um modelo para ele já nascer com serviços e tarefas.
- Como assessora, quero filtrar eventos por status e data para priorizar o que está próximo.
- Como assessora, quero a home do evento com budget, tarefas e convidados para ver o atraso sem abrir cada módulo.
- Como assessora, quero escalar a equipe do dia.

### 3. Modelos

Fase: MVP para modelo de evento. Os demais ficam para depois.

- Modelos de evento.
- Modelos de briefing.
- Modelos de roteiro.
- Modelos de cronograma.
- Modelos de inspirações.
- Modelos de mensagens.

Tipos vistos na referência: casamento, 15 anos, aniversário, corporativo e formatura.

Histórias:

- Como assessora, quero manter um modelo por tipo de evento para não remontar o checklist a cada contrato.
- Como assessora, quero que as tarefas do modelo se desloquem quando a data do evento for definida.

### 4. Clientes

Fase: MVP

- Lista com busca.
- Cadastro com nome, telefone e e-mail.
- Eventos vinculados e último acesso.
- O cliente entra só no próprio evento.
- Permissões individuais: ver convidados, importar convites, excluir convites, RSVP somente leitura, enviar RSVP, travar data de RSVP, ver mapa de mesas, ver checklist de presença, editar tarefas, mudar situação da tarefa, ver anexos, ver modelos de mensagem e de observação, editar alinhamentos, incluir, editar, excluir e baixar parcelas, pedir contrato.

Histórias:

- Como assessora, quero vincular o casal ao evento para eles acompanharem tarefas e convidados.
- Como cliente, quero entrar só no meu evento.
- Como assessora, quero escolher o que o cliente pode editar.

### 5. Serviços, fornecedores e budget

Fase: MVP

- Catálogo de fornecedores, com busca, filtro, exportação e mescla.
- Status do cadastro: completo, incompleto ou dados solicitados.
- Lista de serviços da assessoria.
- No evento, cada serviço tem fornecedor, tarefas e valor.
- Negociação: recomendado, orçamento solicitado, orçamento recebido, contratado, contrato solicitado, contrato recebido.
- Budget estimado, contratado e comparativo.
- Link para o fornecedor completar dados, enviar orçamento e enviar contrato.
- Chat e alinhamento com fornecedor ficam para depois.
- Impressão do alinhamento fica para depois.

Histórias:

- Como assessora, quero um catálogo único de fornecedores para reutilizar em vários eventos.
- Como assessora, quero vincular fornecedor, valor e tarefas a cada serviço.
- Como assessora, quero comparar o estimado com o contratado.
- Como fornecedor, quero receber um link para enviar orçamento e contrato sem criar uma conta completa.

### 6. Tarefas

Fase: MVP

- Visões por situação, responsável, prazo e serviço.
- Situações: pendente, em andamento, concluída e atrasada.
- Formatos: lista, quadro por situação e quadro por dia.
- Prazo relativo ao evento, como "6 meses antes".
- Filtros: com checklist, com perguntas, sem prazo, sem serviço.
- Tarefa com título, prazo, serviço, responsável, descrição, checklist, perguntas, comentários e anexos.
- Lista de tarefas de todos os eventos, com destaque da semana.
- Recalcular prazos quando a data do evento muda.
- Lembrete de aniversário.

Histórias:

- Como assessora, quero ver as tarefas atrasadas de todos os eventos para montar a semana.
- Como assessora, quero um checklist que nasce do modelo e se desloca com a data do evento.
- Como cliente, quero marcar as tarefas que são minhas.

### 7. Convidados e RSVP

Fase: MVP para convite, convidado e RSVP por link. Mapa de mesas, checklist de portaria e RSVP automático ficam para depois.

- Convite (família ou grupo) com convidados dentro.
- Grupos configuráveis: família da noiva, família do noivo, padrinhos, amigos e outros.
- Situação: pendente, confirmado, não comparecerá.
- Faixa: adulto, adolescente, criança, criança de colo, idoso, não informado.
- Custo: inteira, meia, gratuita, não informado.
- Gênero: masculino, feminino, não binário, não informado.
- Celular, observação e transfer com local de origem.
- Visão por convite ou por convidado.
- Busca e filtros: situação, telefone, transfer, grupo, observação.
- Criar convite, importar planilha, exportar e imprimir.
- Link de RSVP por convite e link único do evento.
- Totais por situação, faixa e custo.
- Histórico do RSVP: enviada, acessada, respondida.
- Mapa de mesas, com trava de data.
- Checklist de presença, inclusive offline.
- QR Code do convite.
- RSVP automático e mensagens programadas.

Histórias:

- Como assessora, quero importar a lista e organizar por convite e grupo.
- Como assessora, quero enviar um link e ver quem confirmou, recusou ou não respondeu.
- Como convidado, quero confirmar presença pelo link, dizendo quem vai.
- Como assessora, quero o mapa de mesas e a lista de portaria no dia do evento.

### 8. Pagamentos do evento

Fase: MVP

- Parcelas por serviço e fornecedor.
- Visão por mês ou por fornecedor.
- Filtros: período, pagador, status e forma de pagamento.
- Vencimento, valor, juros, número da parcela, pagador e status de pago.
- Imprimir custos e enviar o resumo por e-mail.

Histórias:

- Como assessora, quero lançar as parcelas de cada fornecedor para ver o que está pago e o que está atrasado.
- Como assessora, quero imprimir os custos do evento para alinhar com o casal.

### 9. Roteiro, cronograma, briefing, inspirações e anexos

Fase: depois

- Roteiro em seções ordenáveis, com observação, anexo, clonagem, impressão e permissão de ver ou editar para o cliente.
- Cronograma com data, hora, responsável, recado aos fornecedores, salvar como modelo, impressão e compartilhamento.
- Briefing do zero ou de modelo, com seções, perguntas, status de resposta, link público e impressão.
- Galerias de inspiração.
- Anexos por fornecedor, serviço, tarefa e observações gerais, mais uma caixa de todos os eventos.

Histórias:

- Como assessora, quero o roteiro do dia em seções para alinhar cerimônia e recepção.
- Como assessora, quero um cronograma com responsável e horário para entregar aos fornecedores.
- Como cliente, quero responder o briefing por link.

### 10. Agenda

Fase: MVP

- Calendário em mês, semana, dia e lista.
- Eventos e compromissos no mesmo calendário.
- Confirmação de presença em compromisso por link.
- Ver só os próprios compromissos ou os de toda a equipe.
- Evento com data pode gerar o compromisso.

Histórias:

- Como assessora, quero a agenda com visitas, eventos e reuniões para não aceitar duas festas no mesmo dia.

### 11. Funil de vendas

Fase: depois

- Quadro de oportunidades com estágios configuráveis.
- Cada coluna mostra quantidade e valor.
- Adicionar, filtrar, exportar e descartar com motivo.
- Verificar se a data está livre.
- Formulário público de lead.
- Oportunidades de RSVP.
- Contratos.
- Tarefas comerciais: reunião, ligação, e-mail e tarefa.
- Orçamentos.
- Modelos de serviço, orçamento, contrato e briefing do funil.

Estágios vistos na conta de referência: contato inicial, orçamento enviado, contratado, descartado.

Histórias:

- Como assessora, quero um funil para saber o que está em orçamento e o que já fechou.
- Como assessora, quero checar a data antes de aceitar um lead.
- Como lead, quero preencher um formulário e cair no funil.

### 12. Gestão financeira da assessoria

Fase: depois

Caixa da empresa, separado do budget do evento.

- Dashboard: recebimentos e despesas por categoria, pendências de hoje, do mês e em atraso, balanço mensal e fluxo de caixa.
- Transações: todas, recebimentos, despesas e transferências.
- Colunas: pago, vencimento, descrição, conta, forma de pagamento e valor.
- Resumo de saldo, receitas, despesas e balanço, realizado e previsto.
- Contas bancárias, categorias e modelos de recibo.
- Integração com as parcelas do evento, para não lançar duas vezes.
- Exportar e imprimir.

Histórias:

- Como assessora, quero lançar o que entra e o que sai da empresa.
- Como assessora, quero uma parcela do evento refletida no financeiro.

### 13. Relatórios

Fase: depois, exceto as impressões já citadas no MVP (lista de eventos, lista de convidados e custos do evento).

#### Funil

- Negócios fechados e perdidos
- Oportunidades por fonte
- Oportunidades por serviço ou produto
- Oportunidades por estágio
- Oportunidades por região
- Oportunidades por responsável
- Oportunidades por data prevista
- Oportunidades por motivo de descarte
- Oportunidades de RSVP

#### Financeiro da assessoria

Cada um com visão resumida ou detalhada e filtro de período:

- Fluxo de caixa: data, saldo inicial, recebimentos, despesas, balanço e saldo final
- Despesas por categoria
- Recebimentos por categoria
- Despesas por evento
- Recebimentos por evento
- Despesas por conta bancária
- Recebimentos por conta bancária
- Extrato de transações
- Recibo, inclusive em duas vias

#### Evento, para impressão ou PDF

Convidados:

- Lista de convites, com situação, tipo, mesa, transfer e observação
- Lista de convidados, com custo
- Convidados por mesa, inclusive quem está sem mesa
- Transfers
- RSVP automático
- Checklist de presença: presentes, ausentes, mesa e contagem

Dinheiro do evento:

- Planejamento de budget
- Custos e pagamentos, por período, status e forma de pagamento
- Pagamentos por mês
- Despesas do evento

Operação:

- Roteiro completo e roteiro de uma seção
- Cronograma
- Briefing respondido
- Alinhamento de um fornecedor e alinhamento de todos
- Observações gerais
- Galeria de inspirações
- Tarefas do evento
- Resumo do evento
- Lista geral de eventos
- Compromissos da agenda

A lista de eventos também tem uma aba Estatísticas na tela. O conteúdo interno dessa aba ainda não foi aberto.

Exportações para planilha, que não são relatórios: transações, fornecedores e oportunidades.

---

## Como o trabalho fica no Trello

Um quadro só. As colunas são o fluxo, não os módulos.

1. **Índice** — parada. Um card por épico, só para navegação.
2. **Backlog** — o que existe e ainda não entrou na semana.
3. **Pronto** — o que já foi escolhido para fazer em seguida.
4. **Fazendo** — poucas features ao mesmo tempo.
5. **Feito**

O card é a feature. A user story fica no checklist, não vira card.

Etiquetas:

- Uma por épico: Acesso, Eventos, Modelos, Clientes, Fornecedores, Tarefas, Convidados, Pagamentos, Documentos, Agenda, Funil, Financeiro, Relatórios.
- Fase: MVP, Depois, Fora.
- Melhoria, quando o pedido vier do cliente.

Card de feature:

- Título curto, por exemplo `RSVP por link`.
- Etiquetas do épico e da fase.
- Descrição com o que a feature cobre e a regra principal.
- Checklist **Histórias**.
- Checklist **Pronto quando**.

O índice não se move. Melhoria nova entra no Backlog com a etiqueta do épico e a etiqueta Melhoria.

## O que ainda falta do cliente

Lista de melhorias em relação à plataforma que ele já usa. Cada pedido vira uma feature no épico correspondente, com a etiqueta Melhoria.
