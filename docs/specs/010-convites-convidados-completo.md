# 010. Convites e convidados completo

**Status: aprovada.** O código segue esta spec. Incremento sobre a SPEC-004. Fonte observada: [inventario-referencia.md](../inventario-referencia.md) §2. O telefone do convite é escolha deste produto: aponta para o convidado principal.

## 1. Identificador e nome

SPEC-010. Convites e convidados completo.

## 2. Objetivo

Convite e convidado passam a ter todos os campos vistos na referência, inclusive importação, grupos da conta, transfer, QR Code e mesa no cadastro da pessoa.

## 3. Requisitos relacionados

RF-063, RF-064. Fora deste arquivo: RSVP automático (RF-065), mapa operacional além do campo mesa (RF-066), checklist de portaria (RF-067) e o formulário público, que ainda não foi respondido.

## 4. Atores e permissões

O User da conta mantém grupos, convites e convidados do próprio evento. Quem tem o link de RSVP continua no recorte da SPEC-004, sem os campos novos.

## 5. Pré-condições

Evento da conta, SPEC-002. A SPEC-004 continua valendo para situações, totais e o link por convite.

## 6. Fluxo principal

1. A conta cria grupos pelo nome.
2. Cria um convite com nome e, se quiser, grupo e observação.
3. Inclui pelo menos um convidado: nome obrigatório; DDI, celular, RSVP, mesa, gênero, faixa, custo, RG e CPF opcionais.
4. No convite, escolhe o telefone de um convidado deste convite. Esse convidado fica principal e o telefone do convite passa a ser o dele.
5. Com transfer ativo no evento, o convite pergunta se usa o serviço e, se sim, o local de origem.
6. A lista filtra por situação, telefone, transfer, grupo e observação.
7. Importação usa a planilha padrão de 11 colunas.
8. Na edição, o convite tem Histórico de mensagens e QR Code, com download e envio por WhatsApp.

## 7. Fluxos alternativos

- Convite sem nome ou sem convidado: a tela exige pelo menos um convidado para salvar o convite novo.
- Escolher como principal um convidado sem telefone, ou de outro convite: não grava essa escolha.
- Transfer Sim sem local de origem: o campo aparece; gravar sem local não foi forçado.
- Importar arquivo que não é `.xlsx` ou `.xls`: o seletor não oferece outro tipo.
- Filtro "só confirmados" no mapa: convidado pendente some da lista da mesa.

## 8. Regras de negócio

- Grupo pertence à conta. Padrão visto: Amigos, Amigos da Noiva, Amigos do Noivo, Família da Noiva, Família do Noivo. Criar grupo novo é só o nome.
- Convidado e convite têm telefone. O do convidado é o cadastro da pessoa, com DDI e celular. O do convite não é digitado à parte: a conta escolhe um convidado deste convite que já tenha telefone. Esse convidado é o principal, e o único. O telefone mostrado no convite é o dele. Trocar a escolha troca o principal. Convidado sem telefone não pode ser escolhido. Sem escolha, o convite fica sem telefone e sem principal. Se o telefone do principal mudar, o convite mostra o número novo. Se o principal sair do convite ou perder o telefone, o convite fica sem telefone e sem principal.
- Situações na lista: Pendente, Confirmado, Não comparecerá. No ícone do convidado a recusa aparece como Cancelado.
- Faixas vistas nos totais: Adulto, Idoso, Adolescente, Criança, Criança de colo.
- Custos: Inteira, Meia, Gratuita. A planilha também traz Free.
- Gêneros na tela: Masculino, Feminino, Não binário. A planilha usa `M` e `F`.
- Transfer é do evento. Local é um nome. Só então o convite mostra Sim/Não e origem.
- Mesa no convidado aceita valor novo pela busca.

Colunas da planilha, nesta ordem: Nome do convite *, DDI, DDD + Telefone para confirmação de presença, Grupo do convite, Observação do convite, Nome dos convidados *, Gênero, Faixa etária, Custo/pagamento, Situação, Mesa.

## 9. Validações

Nome do convite e nome do convidado obrigatórios. Situação, faixa, custo e gênero, se preenchidos, só os valores da seção 8. O principal precisa ser um convidado deste convite e ter telefone. Transfer Sim exige local quando o campo está visível. Arquivo de importação só `.xlsx` e `.xls`.

## 10. Estados

Convidado nasce pendente, salvo importação com outra situação. Transfer do evento: desligado ou ligado, com zero ou mais locais.

## 11. Dados necessários

Grupo: nome. Local de transfer: nome. Convite: nome, convidado principal, grupo, observação, usa transfer, local de origem, QR Code. O telefone do convite é o do principal. Convidado: nome, DDI, celular, situação, mesa, gênero, faixa, custo, RG, CPF.

## 12. Interface

Duas visões. Filtros da seção 2.1 do inventário. Modais de convite, convidado, grupos, importação e transfer. Edição do convite com três abas.

## 13. Erros

Validação no campo. Convite de outra conta não revela dado. Planilha inválida: o resultado do erro de importação não foi visto.

## 14. Critérios de aceite

1. Convite novo sem convidado não grava.
2. O convidado grava o próprio celular. No convite, escolher esse telefone marca o convidado como principal e a lista do convite mostra o número dele.
3. Grupo criado na conta aparece no convite.
4. Com transfer desligado, o convite não pergunta origem.
5. Com transfer ligado e Sim, o local de origem é o cadastrado no evento.
6. A planilha padrão tem as 11 colunas da seção 8.
7. QR Code existe na edição do convite, com baixar e enviar por WhatsApp (DDI, celular, mensagem).
8. Outra conta não vê convite nem convidado.

## 15. Cenários de teste

Criar convite com dois convidados, um com celular. Escolher esse celular e conferir o principal. Trocar a escolha. Ativar transfer, cadastrar local, marcar Sim e origem. Criar grupo. Abrir a planilha padrão e conferir o cabeçalho. Isolamento entre contas. Importação com arquivo válido ainda não foi executada. Na importação, o telefone da linha do convite grava no convidado daquela linha e esse convidado nasce principal.

## 16. Dependências

SPEC-001, SPEC-002, SPEC-004.

## 17. Premissas e pontos não definidos

Formulário público de RSVP. Resultado de uma importação real. Envio efetivo do WhatsApp do QR Code. O que a aba Histórico mostra depois de um disparo. Excluir todos os convites.
