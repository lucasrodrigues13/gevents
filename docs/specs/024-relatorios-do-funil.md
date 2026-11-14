# 024. Relatórios do funil

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-024. Relatórios do funil.

## 2. Objetivo

O Organizer consulta e exporta o catálogo de relatórios do funil no período escolhido. O dashboard de quantidade e valor por estágio permanece na SPEC-007.

## 3. Requisitos relacionados

RF-077, no recorte do catálogo de funil. RF-075 alimenta os dados. RF-082 e SPEC-007 são outra tela. Catálogo financeiro, impressões de evento e RF-078 ficam fora. RF-079 fica fora.

## 4. Atores e permissões

O `Organizer` lê e exporta só relatórios da própria conta. Outro `Organizer` não vê. `Admin` acessa qualquer conta. `Client` não acessa. Assistente e fornecedor com login ficam fora.

## 5. Pré-condições

Conta autenticada, SPEC-001. Oportunidades e descarte vêm da SPEC-020. Leads RSVP vêm da SPEC-021.

## 6. Fluxo principal

1. O Organizer abre Relatórios no funil.
2. Escolhe o período.
3. Consulta os nove relatórios do catálogo.
4. Exporta o relatório aberto quando quiser levar os números para fora da tela.

## 7. Fluxos alternativos

- Período sem movimento: os relatórios abrem zerados ou sem linhas, sem erro.
- Trocar o período: os nove recalculam.
- Conta sem oportunidade: tudo zerado.

## 8. Regras de negócio

- Os nove relatórios, nesta ordem:
  1. Negócios fechados x perdidos (período pela entrada no estágio; fechado = Contratado; perdido = Descartado).
  2. Por fonte.
  3. Por serviço/produto.
  4. Por estágio (período pela criação).
  5. Por região (tabelas UF e Cidade, com quantidade e total em reais).
  6. Por responsável.
  7. Por data prevista.
  8. Por motivos de descarte.
  9. Oportunidades de RSVP (visualizações, cliques no botão, contatados, descartados, geradas, conversão e conversões anteriores).
- Só entram dados da própria conta.
- Esta tela não cria nem edita oportunidade.
- Estágios são os quatro da SPEC-007 / SPEC-020.

## 9. Validações

Período com início e fim válidos; fim não anterior ao início.

## 10. Estados

Relatório aberto no período selecionado. Não há estado de rascunho de relatório.

## 11. Dados necessários

Oportunidades (estágio, valor, fonte, serviço/produto, responsável, data prevista, região, motivo de descarte, datas de criação e de entrada no estágio) e métricas de oportunidades de RSVP.

## 12. Interface

Catálogo com os nove relatórios e controle de período. Exportação do relatório visível. Angular Material, DA-044. Sem gráfico obrigatório além do necessário para Oportunidades de RSVP.

## 13. Erros

Período inválido indica o controle. Falha ao carregar mostra mensagem genérica. Sem movimento não é erro.

## 14. Critérios de aceite

1. Os nove relatórios listados na seção 8 estão disponíveis.
2. O período altera os números.
3. Sem movimento, os relatórios ficam zerados ou sem linha.
4. Fechados e perdidos usam Contratado e Descartado.
5. Por região mostra UF e Cidade com quantidade e total.
6. Exportação gera arquivo só com dados da própria conta.
7. Outra conta não altera os números.
8. Dashboard da SPEC-007 permanece tela distinta.

## 15. Cenários de teste

Período sem movimento. Conta com oportunidades nos quatro estágios. Descarte com motivo. Lead RSVP. Duas contas. Período inválido. Exportação.

## 16. Dependências

SPEC-001, SPEC-007, SPEC-020 e SPEC-021.

## 17. Premissas e pontos não definidos

Formato do arquivo exportado (planilha ou equivalente). Intervalo padrão inicial do período além de “período escolhido pelo Organizer”.
