# 019. Minha agenda

**Status: rascunho.** RF-074 permanece Observado e fora do corte aprovado em [mvp-scope.md](../mvp-scope.md). A gravação do evento na agenda da conta permanece na SPEC-008.

Fonte desta abertura: a tela de compromissos da referência, sem submenu, com navegação anterior, próxima e Hoje. As vistas Mês, Semana e Lista foram abertas e estavam vazias no período visível. Havia o comando "Integrar minha agenda do Google", sem acionamento. A vista Dia não foi aberta.

## 1. Identificador e nome

SPEC-019. Minha agenda.

## 2. Objetivo

A conta autenticada abre a própria agenda e consulta o período visível em mês, semana e lista.

## 3. Requisitos relacionados

RF-074, status Observado. Não está confirmado e não entra no corte do MVP. Esta spec não o aprova.

A SPEC-008 já grava o evento na agenda da conta quando a marcação é feita no cadastro, e deixa o vínculo Google vazio. Este arquivo não altera essa gravação.

Fora da regra operável desta spec, porque não foram exercidos: vista Dia, criação de compromisso, integração com a agenda do Google, confirmação de presença em compromisso por link e permissão de ver só os próprios compromissos. Impressão da agenda (RF-077) e checagem de data do funil (RF-075) também ficam de fora.

## 4. Atores e permissões

O `User` autenticado consulta a agenda da própria conta. RF-040. Outra conta não foi comparada nesta abertura.

RF-074 cita assessoria e assistente. Assistente não é role deste produto. A permissão de ver só os próprios compromissos não foi exercida.

## 5. Pré-condições

Conta autenticada, SPEC-001. A conta pode já ter evento gravado na agenda pela SPEC-008. No período visível desta abertura, a tela não listou compromisso.

## 6. Fluxo principal

1. O `User` abre a agenda da própria conta.
2. A tela aparece sem submenu.
3. A tela oferece anterior, próxima e Hoje.
4. O `User` abre Mês, Semana e Lista. Em cada uma, o período visível desta abertura não lista compromisso.
5. O comando "Integrar minha agenda do Google" permanece na tela, sem uso.

## 7. Fluxos alternativos

- Vista Dia: o nome consta em RF-074. A vista não foi aberta. O que ela mostra fica não definido.
- Acionar "Integrar minha agenda do Google": não foi feito. O efeito fica não definido. A SPEC-008 permanece como está.
- Criar compromisso a partir desta tela: o formulário não foi aberto. Fica não definido. RF-074 só registra que compromisso exige data.
- Confirmação de presença por link e a visão restrita aos próprios compromissos: constam em RF-074 e não foram exercidos. Ficam não definidos.
- Período que já tenha compromisso, inclusive o evento gravado pela SPEC-008: não apareceu nesta abertura. Não há regra aqui sobre como a tela o mostra.

## 8. Regras de negócio

- RF-074 nomeia as vistas mês, semana, dia e lista. Nesta abertura, mês, semana e lista foram abertas.
- No período visível, essas três vistas não listaram compromisso. Isso descreve a abertura. Não define um texto de vazio nem afirma que a conta inteira está sem compromisso.
- A tela desta abertura não tem submenu. Não há RF para essa ausência. O fato fica registrado e a regra de produto permanece não definida.
- Anterior, próxima e Hoje estão na tela. O quanto cada um desloca o período não foi medido e permanece não definido.
- Compromisso exige data, RF-074. Os demais campos do formulário não foram vistos e permanecem não definidos.
- Evento com data pode gerar compromisso, RF-074. O modo de gravar isso no cadastro do evento é o da SPEC-008. Esta spec não cria outro caminho.
- A confirmação de presença por link e a permissão de ver só os próprios compromissos permanecem não definidas até serem vistas.
- O comando de integrar a agenda do Google foi visto e não foi acionado. O que ele faz permanece não definido.

## 9. Validações

Não há formulário nesta abertura. Não há validação nova. A data obrigatória do compromisso fica sem tela até o formulário ser aberto.

## 10. Estados

Mês, Semana e Lista abertas, com o período visível sem compromisso. Dia fechada. Integração Google não acionada.

## 11. Dados necessários

Nenhum dado novo. O que a SPEC-008 grava ao marcar a agenda do evento continua naquela spec. Os campos de um compromisso criado na agenda não foram vistos.

## 12. Interface

Tela da agenda, sem submenu. Comandos anterior, próxima e Hoje. Vistas Mês, Semana e Lista. Comando "Integrar minha agenda do Google", sem acionamento. A vista Dia não foi mostrada.

## 13. Erros

Nenhum erro foi visto. Período visível sem compromisso não é erro. A mensagem de falha ao carregar a agenda não foi vista e permanece não definida.

## 14. Critérios de aceite

1. O `User` autenticado abre a agenda da própria conta e a tela não apresenta submenu.
2. Mês, Semana e Lista abrem.
3. No período visível desta abertura, as três vistas não listam compromisso.
4. Anterior, próxima e Hoje estão na tela.
5. "Integrar minha agenda do Google" está visível e, sem acionamento, a agenda permanece como a SPEC-008 a deixou.
6. Vista Dia, criação de compromisso e o acionamento do Google ficam fora deste aceite.

## 15. Cenários de teste

Conta autenticada abre a agenda: a tela não tem submenu e mostra anterior, próxima, Hoje e o comando de integrar Google. Abre Mês, Semana e Lista no período visível: nenhuma das três lista compromisso. Sem acionar o Google, a gravação feita pela SPEC-008 permanece a dela. A vista Dia não entra no resultado esperado.

## 16. Dependências

SPEC-001. SPEC-008, sem mudança, para o evento já marcado na agenda da conta. RF-074, ainda Observado.

## 17. Premissas e pontos não definidos

RF-074 não está aprovado. A agenda continua fora do corte do MVP.

Vista Dia. Criação de compromisso e os campos além da data obrigatória citada em RF-074. Efeito de "Integrar minha agenda do Google". Confirmação de presença por link. Permissão de ver só os próprios compromissos, e o papel assistente. Quanto anterior, próxima e Hoje deslocam o período. Texto do período vazio. Como a tela mostra um compromisso quando o período o inclui, inclusive o evento gravado pela SPEC-008.

Submenu ausente, os três comandos de navegação, o período visível vazio e o rótulo do Google foram vistos e não têm RF próprio. Permanecem não definidos como regra confirmada.
