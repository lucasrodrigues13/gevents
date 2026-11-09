# Plano 008. Cadastro completo de evento

**Status: aprovada.** Spec [008-cadastro-completo-de-evento.md](../specs/008-cadastro-completo-de-evento.md). Depende dos planos 001 e 002 já implementados.

## Fora deste plano

Editor completo do modelo: briefings, roteiros, inspirações, RSVP, permissões, fornecedores do modelo, alinhamentos, comparativos, valor estimado do modelo e prazo relativo da tarefa. Equipe do dia, site Casar.com, Google Agenda, envio das três notificações e o cadastro de cliente no interruptor da criação. Esse interruptor fica para o plano 009. Catálogo de fornecedor com as oito abas. Tela de pagamentos do evento. Agenda com mês, semana e dia.

Fechar o passo do valor sem informar um valor válido não apaga o evento e não cria serviço. A spec não define esse fechamento como obrigatório.

## Camadas

| Camada | O que entra |
|---|---|
| Domain | `Modelo` da conta, serviço e tarefa do modelo, cópia para o evento, endereço dos dois locais, detalhes de horário, serviço contratado da assessoria e parcelas. Nome do casamento nasce dos dois nomes. |
| Application | Criar modelo mínimo, criar e editar evento com modelo, gravar o valor da assessoria, consultar CEP. A consulta é interface; o teste usa uma falsa. |
| Infrastructure | Mapeamento, uma migration e o cliente HTTP da consulta de CEP, com endereço configurável. |
| Web | Páginas Razor. PageModel não usa `DbContext`. |

Sem MediatR, AutoMapper, repositório genérico e fila.

## Modelo mínimo

`Modelo` guarda conta, nome e tipo. O tipo reutiliza os cinco já aceitos no evento. A lista que a referência mostra ao criar modelo não foi aberta; este plano não cria um sexto tipo.

No modelo, cada serviço tem nome. Cada tarefa do serviço tem título. Isso existe só para a cópia na criação. Não é o editor da referência.

## Evento

O formulário de criar pede o modelo antes do resto. Sem modelo, não grava. O tipo do evento é o tipo do modelo.

Casamento exige dois nomes. O nome do evento fica `{primeiro} & {segundo}`. Os outros quatro tipos exigem um nome, e esse nome é o nome do evento.

Permanecem data, hora, budget, convidados estimados, locais em texto, tags e foto. Entram tempo de festa, início da cerimônia, início da recepção, quantidade de padrinhos, quantidade de damas e pajens, três marcações de notificação e a marcação de agenda. As três notificações nascem ligadas e só são gravadas. Nenhum e-mail sai por causa delas.

Cada local, cerimônia e recepção, ganha país, CEP, endereço, número, estado, cidade e bairro, além do nome do local que a SPEC-002 já grava. CEP com oito dígitos consulta o serviço e preenche endereço, estado, cidade e bairro. CEP incompleto ou desconhecido não preenche e não impede gravar o evento.

A marcação de agenda, com data, grava um compromisso da conta com o nome, a data e a hora do evento. Sem data, só a marcação fica gravada. Vínculo com Google não é chamado e permanece vazio.

URL do site fica vazia e somente leitura. Senha do site, se preenchida, é gravada. Não há integração com Casar.com.

Os campos de casal da SPEC-002 continuam na entidade. Este formulário não os apaga.

## Valor da assessoria

Depois que o evento existe, a página pede valor total, quantidade de parcelas, data da primeira parcela e, se marcado, entrada. A entrada tem base em valor ou porcentagem, o valor e a data.

Valor informado maior que zero e quantidade inteira maior que zero criam o serviço "Assessoria e Organização" no evento, contratado, com a própria conta como fornecedor. O saldo depois da entrada, se houver, divide em parcelas iguais. A primeira parcela usa a data informada. As seguintes caem no mesmo dia dos meses seguintes. A entrada, se houver, é uma parcela à parte na data da entrada.

Falha nesse passo não remove o evento.

## Páginas

| Página | Autorização |
|---|---|
| Lista e criação de modelo, com serviço e tarefa só por nome e título | `User` |
| Criar evento, com modelo antes dos demais campos | `User` |
| Valor da assessoria, no evento recém-criado | `User` |
| Editar evento em quatro abas: Informações gerais, Detalhes, Cerimônia, Recepção | `User`, evento em andamento da própria conta |
| Home, com o link do mapa a partir do endereço e o compromisso quando existir | `User` |

## Persistência

Uma migration, depois das anteriores: modelo, serviço e tarefa do modelo, cópia no evento, endereço, detalhes, compromisso, serviço contratado e parcela. Índice de modelo por conta.

## Testes

Unitário: evento sem modelo, casamento sem um dos nomes, nome com `&`, valor não positivo, quantidade de parcelas inválida. Integração: criar modelo de casamento com um serviço e uma tarefa, criar o evento e ver a cópia, gravar quatro parcelas, editar endereço por CEP falso, CEP desconhecido sem apagar o evento, fechar o valor sem gravar serviço, outra conta sem ver o evento.

## Tarefas

1. Modelo mínimo e cópia de serviços e tarefas na criação do evento.
2. Formulário de criar e as quatro abas de editar, com endereço e CEP.
3. Valor da assessoria, serviço contratado e parcelas.
4. Migration e testes da spec.

## Pendência que permanece

Lista de tipos do modelo na referência. Obrigatoriedade de cada campo de endereço. Tamanho máximo dos textos. Envio real das notificações. Google Agenda. O que a tela faz se o usuário abandona o valor no meio.
