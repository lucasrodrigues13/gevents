# 027. Modelo de orçamento do funil

**Status: rascunho.** Fora do Must. RF-075 e RF-077 permanecem Observado e não autorizam implementação.

## 1. Identificador e nome

SPEC-027. Modelo de orçamento do funil.

## 2. Objetivo

Descrever o modelo de orçamento do funil no limite do que a visita mostrou. A lista de orçamentos fica na SPEC-025. O comportamento de produto deste modelo permanece não definido.

## 3. Requisitos relacionados

RF-075 cita orçamentos do funil. Status Observado. O texto não descreve modelo, cores, parâmetros nem texto de proposta. RF-077 cita catálogos e impressões do funil e do financeiro. Status Observado. O texto não descreve este modelo. RF-082, Definido, mostra quantidade e valor nos quatro estágios do dashboard e não cria modelo.

Nenhum RF aprovado cobre este fluxo. Regra sem RF fica não definida.

O corte em [mvp-scope.md](../mvp-scope.md) deixa o restante do funil de fora. De RF-075 entra só quantidade e valor, pelo RF-082. No [brief.md](../brief.md), modelos de orçamento do funil estão na fase depois. Isso não confirma o requisito.

## 4. Atores e permissões

A visita ocorreu na conta autenticada da referência. Quem abre o modelo no produto, inclusive `User` e `Admin`, fica não definido. Lead, cliente, assistente e fornecedor não tiveram esta tela aberta.

## 5. Pré-condições

Conta autenticada na referência. A SPEC-001 só situa a conta; este fluxo não entra no corte aprovado. O cadastro da oportunidade ainda não tem spec. A SPEC-025 cobre a lista de orçamentos.

## 6. Fluxo principal

Visita à referência, em `https://assessoriavip.com.br/funnel/models/budget`. Isto não é rota do produto.

1. A tela abre um modelo padrão.
2. Esse modelo está não editável.
3. O modelo mostra cores, parâmetros e o texto da proposta.
4. O único token transcrito é `#nome_oportunidade`. Também há parâmetros de datas, horários, locais, quantidades e valor. Os tokens desses grupos não foram transcritos.
5. A tela tem o controle "Mostrar mais parâmetros". Nesta visita o controle não expandiu.

## 7. Fluxos alternativos

- "Mostrar mais parâmetros" não revelou a lista. Os parâmetros ocultos ficam pendentes.
- Edição, criação, exclusão, duplicação e uso do modelo numa oportunidade não abriram. Ficam pendentes.
- Outro modelo além do padrão não foi visto. Fica pendente.

## 8. Regras de negócio

- RF-075 e RF-077 estão Observado. Nenhum dos dois vira regra aprovada aqui.
- O modelo padrão não editável, as cores, os parâmetros visíveis e o texto da proposta são o registro da visita. Sem RF aprovado, isso não é obrigatório no produto.
- Entra como token só `#nome_oportunidade`. Datas, horários, locais, quantidades e valor entram como grupos vistos, sem nome inventado.
- A lista atrás de "Mostrar mais parâmetros" não foi lida. Nenhum parâmetro oculto entra nesta spec.
- A troca de `#nome_oportunidade` por um dado da oportunidade não foi vista. Fica não definida.
- O texto da proposta foi visto. O conteúdo não foi transcrito e não entra aqui.
- As cores foram vistas. Quais cores, e o que elas colorem, não foi transcrito. Fica não definido.
- O modelo de orçamento do funil é distinto da lista de orçamentos da SPEC-025, do budget do evento, do orçamento de fornecedor e do relatório do funil.

## 9. Validações

Nenhuma validação foi vista. Obrigatoriedade, tamanho e formato ficam não definidos.

## 10. Estados

O modelo padrão foi visto não editável. Outro estado não apareceu. Fica não definido se "não editável" é um estado, uma trava só do padrão ou a ausência de ação naquela tela.

## 11. Dados necessários

A visita mostrou um modelo padrão, cores, o token `#nome_oportunidade`, parâmetros visíveis de datas, horários, locais, quantidades e valor, e um texto de proposta. Dono do modelo, cópia por conta ou modelo único do produto, e se o texto fica gravado ficam não definidos.

## 12. Interface

Tela do modelo de orçamento do funil, distinta da lista de orçamentos. Mostra o modelo padrão não editável, cores, os parâmetros visíveis, o texto da proposta e o controle "Mostrar mais parâmetros". A rota do produto fica não definida.

## 13. Erros

Nenhum erro foi visto. A mensagem de falha fica não definida.

## 14. Critérios de aceite

1. Com RF-075 e RF-077 em Observado, esta spec não serve de base para implementação.
2. O modelo descrito aqui permanece distinto da lista de orçamentos da SPEC-025.
3. O registro da visita contém um modelo padrão não editável, cores, `#nome_oportunidade`, os grupos datas, horários, locais, quantidades e valor, e texto da proposta.
4. A spec não nomeia parâmetro que estivesse atrás de "Mostrar mais parâmetros".
5. A spec não descreve uma tela de edição.

## 15. Cenários de teste

- Condição: abrir o modelo padrão na referência. Resultado observado: modelo não editável, com cores, parâmetros visíveis e texto da proposta, inclusive `#nome_oportunidade`. Resultado de produto: não definido.
- Condição: acionar "Mostrar mais parâmetros". Resultado observado: a lista não abriu. Resultado de produto: pendente.
- Condição: procurar editar o modelo padrão. Resultado observado: a edição não abriu. Resultado de produto: pendente.

## 16. Dependências

SPEC-001, apenas como conta autenticada da visita. RF-082 e a SPEC-007 não incluem este modelo. A lista de orçamentos é a SPEC-025. Saber se `#nome_oportunidade` substitui algum valor depende de spec de oportunidade, que ainda não existe.

## 17. Premissas e pontos não definidos

- Parâmetros ocultos de "Mostrar mais parâmetros".
- Tokens de datas, horários, locais, quantidades e valor.
- Conteúdo e formatação do texto da proposta.
- Quais cores existem e o que colorem.
- Edição, criação, exclusão, duplicação e outro modelo além do padrão.
- Aplicar o modelo a uma oportunidade, pré-visualizar com dados, enviar, imprimir ou gerar arquivo.
- Quem pode ver o modelo, e se ele é da conta ou único do produto.
- Validações, erros e estados além do não editável visto.
