# 004. Convites e RSVP

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-004. Convites e RSVP.

## 2. Objetivo

A assessoria monta a lista como ela foi vista: convite, convidados e os campos observados. Copia um link. Quem tem o link informa, sem conta, quem comparece.

## 3. Requisitos relacionados

RF-063 e RF-064, no corte do escopo.

Fora: importação, exportação, impressão, mapa de mesas, checklist de portaria, QR Code, RSVP automático, link único do evento e o valor de histórico "enviada".

## 4. Atores e permissões

| Ator | Pode |
|---|---|
| Organizer | Manter grupos, convites e convidados. Copiar o link. Ver situações e totais. |
| Quem tem o link | Ver os convidados daquele convite e informar a presença de cada um. Sem conta Identity. |
| Outro Organizer ou quem não tem o link | Não vê a lista. |
| Admin | Admin geral; fora do miolo desta spec. |
| Client | Não gerencia a lista neste corte. Acesso a convidados/RSVP via flags fica em RF-056. |

## 5. Pré-condições

Evento em andamento da conta, SPEC-002. O link não é enviado pelo sistema.

## 6. Fluxo principal

1. A conta cria os grupos que quiser usar.
2. Cria um convite com nome e inclui convidados com nome.
3. Cada convidado nasce pendente.
4. Preenche, se quiser, faixa, custo, gênero, celular, observação, grupo e transfer.
5. A lista pode ser vista por convite ou por convidado, com busca e filtros.
6. Os totais do evento mostram situação, faixa e custo.
7. O convite tem um link. A assessoria copia.
8. A página pública lista os nomes daquele convite.
9. A pessoa marca cada um como confirmado ou não comparecerá e envia.
10. A situação muda e o histórico fica respondido. A assessoria vê a lista e os totais atualizados.

## 7. Fluxos alternativos

- Nome de convite, convidado ou grupo vazio: esse item não grava.
- Transfer marcado sem local de origem: não grava o transfer.
- Convite sem convidado: existe, e a página pública diz que não há quem responder.
- Link inválido: não mostra nomes.
- Abrir o link válido marca acessada, se ainda não estava respondido.
- Fechar sem enviar não muda situação.
- Segunda resposta, incluir pessoa pelo link e a assessoria corrigir a situação depois: não definidos.

## 8. Regras de negócio

- Convite e convidado pertencem ao evento da conta.
- Grupos pertencem à conta e podem ser reutilizados. Os nomes vistos foram família da noiva, família do noivo, padrinhos, amigos e outros. Não são uma lista obrigatória.
- Situações: pendente, confirmado e não comparecerá.
- Faixas vistas: adulto, adolescente, criança, criança de colo, idoso e não informado.
- Custos vistos: inteira, meia, gratuita e não informado.
- Gêneros vistos: masculino, feminino, não binário e não informado.
- Transfer exige local de origem.
- O link é de um convite. Não exige conta e não mostra outro convite.
- Totais usam só os convidados do evento.
- A página pública altera a situação. Não altera faixa, custo, gênero, celular, observação, grupo nem transfer, porque esse formulário não foi visto.

## 9. Validações

Nomes obrigatórios, sem ficar só com espaços. Faixa, custo, gênero e situação só aceitam os valores da seção 8. Celular não tem máscara definida. Transfer sem origem é recusado. O envio público só mexe nos convidados daquele convite e só nas situações confirmado e não comparecerá.

## 10. Estados

O convidado nasce pendente. No envio público passa a confirmado ou não comparecerá.

Histórico neste corte: acessada na abertura válida; respondida no envio gravado. "Enviada" não é usada.

## 11. Dados necessários

Grupo: nome e conta. Convite: nome, evento e link. Convidado: nome, situação, faixa, custo, gênero, celular, observação, grupo, transfer e local de origem. Histórico: acessada e respondida.

## 12. Interface

Grupos da conta. No evento, as duas visões, busca, filtros por situação, celular, transfer, grupo e observação, totais e copiar link. Página pública sem menu da conta, com o nome de cada convidado e a escolha da presença.

## 13. Erros

Validação no campo. Link inválido ou convite de outra conta não revela dados. Falha no envio público avisa que não salvou e mantém a situação anterior.

## 14. Critérios de aceite

1. A conta cria grupo, convite e convidado no próprio evento.
2. Convidado novo fica pendente.
3. Transfer sem origem não é gravado.
4. As duas visões, a busca, os filtros e os totais usam só a conta e o evento.
5. O link abre sem login e mostra só aquele convite.
6. O envio muda a presença e os totais.
7. A página pública não oferece os outros campos do convidado.
8. Outra conta e um link inválido não mostram a lista.

## 15. Cenários de teste

Convite com dois convidados e respostas diferentes. Transfer com e sem origem. Filtro por grupo e por situação. Totais por situação, faixa e custo. Sessão anônima. Link alterado. Segunda conta. Envio interrompido sem mudança.

## 16. Dependências

SPEC-001 e SPEC-002. Não envia e-mail. A home do evento lê os totais por situação.

## 17. Premissas e pontos não definidos

O formulário público da referência não foi respondido. Esta spec só deixa a presença nas pessoas já cadastradas. Continuam sem definição: acrescentar convidado pelo link, segunda visita, correção pela assessoria, remover convite ou convidado, e o que acontece com o link depois da remoção. Máscara de celular e tamanho dos textos. Link único do evento.
