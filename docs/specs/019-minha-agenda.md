# 019. Minha agenda

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-019. Minha agenda.

## 2. Objetivo

O Organizer abre a agenda da própria conta, consulta o período em mês, semana, dia e lista, e cria, edita e exclui compromissos.

## 3. Requisitos relacionados

RF-074. A gravação do evento na agenda da conta permanece na SPEC-008. RF-079 fica fora.

## 4. Atores e permissões

O `Organizer` autentica e opera a agenda da própria conta. Outro `Organizer` não vê esses compromissos. `Admin` acessa qualquer conta. `Client` não acessa esta tela neste corte. Assistente e fornecedor com login ficam fora. Convidado do RSVP não usa a agenda autenticada.

## 5. Pré-condições

Conta autenticada, SPEC-001. Evento com data marcado para a agenda pela SPEC-008 aparece como compromisso quando o período o inclui.

## 6. Fluxo principal

1. O Organizer abre Minha agenda.
2. A tela oferece as vistas Mês, Semana, Dia e Lista, além de anterior, próxima e Hoje.
3. O Organizer cria um compromisso com descrição e data e salva.
4. O compromisso aparece nas vistas do período correspondente.
5. O Organizer edita descrição ou data e salva de novo.
6. O Organizer exclui o compromisso; ele some da agenda.

## 7. Fluxos alternativos

- Período sem compromisso: as vistas abrem vazias, sem erro.
- Compromisso sem data: não grava.
- Descrição vazia: não grava.
- Evento gravado pela SPEC-008: aparece no período da data do evento; a exclusão desse vínculo de evento não é feita nesta tela.
- Conta de outro Organizer: compromissos não aparecem.

## 8. Regras de negócio

- Compromisso exige data, RF-074.
- Compromisso exige descrição.
- Compromisso pertence à conta do Organizer.
- Evento com data pode gerar compromisso, RF-074; o caminho de marcação no cadastro do evento é o da SPEC-008.
- Anterior e próxima deslocam o período conforme a vista ativa (mês, semana ou dia). Hoje centra o período na data atual.
- Integração com a agenda do Google fica fora deste corte.
- Confirmação de presença em compromisso por link fica fora deste corte.
- Permissão de ver só os próprios compromissos de assistente fica fora: não há assistente com login.

## 9. Validações

Data obrigatória e válida. Descrição obrigatória, sem ficar só com espaços. Tamanho máximo da descrição não foi fechado além do necessário para gravar texto curto.

## 10. Estados

Compromisso existente ou excluído. Não há situação intermediária de confirmação neste corte.

## 11. Dados necessários

Descrição, data e, quando vier da SPEC-008, vínculo com o evento da conta. Hora é opcional quando a origem do evento a informar.

## 12. Interface

Tela sem submenu. Comandos anterior, próxima e Hoje. Vistas Mês, Semana, Dia e Lista. Inclusão, edição e exclusão do compromisso. Angular Material, DA-044. Sem o comando de integrar Google neste corte.

## 13. Erros

Data ou descrição inválida indica o campo. Item de outra conta fica indisponível, sem revelar conteúdo. Falha ao gravar mantém o compromisso anterior. Período vazio não é erro.

## 14. Critérios de aceite

1. O Organizer autenticado abre a agenda da própria conta com Mês, Semana, Dia e Lista.
2. Anterior, próxima e Hoje estão na tela e deslocam o período.
3. Criar com descrição e data coloca o compromisso nas vistas do período.
4. Sem data ou sem descrição, nada é gravado.
5. Editar e excluir alteram só a agenda da própria conta.
6. Evento marcado na SPEC-008 aparece no período correspondente.
7. Outra conta não vê o compromisso.
8. Integração Google e confirmação por link não aparecem neste corte.

## 15. Cenários de teste

Abrir as quatro vistas. Criar, editar e excluir compromisso. Tentar salvar sem data e sem descrição. Conta com evento da SPEC-008 no período. Duas contas: isolamento. Período vazio.

## 16. Dependências

SPEC-001 e SPEC-008. SPEC-007 não depende desta tela.

## 17. Premissas e pontos não definidos

Formato de hora quando o compromisso não veio de evento. Texto exato do período vazio.
