# 018. Serviços e fornecedores

**Status: rascunho.** Fora do corte do MVP. RF-057 e RF-058 continuam Observado. Esta spec não autoriza implementação.

Fonte da visita: três abas da referência, sem gravar. Catálogo em `https://assessoriavip.com.br/suppliers`. Solicitação de dados em `https://assessoriavip.com.br/suppliers/data-request`. Lista de serviços em `https://assessoriavip.com.br/suppliers/services`. Adicionar, editar, excluir e baixar não foram acionados.

## 1. Identificador e nome

SPEC-018. Serviços e fornecedores.

## 2. Objetivo

A assessoria consulta o catálogo de fornecedores, a solicitação de dados e a lista de serviços da conta. Nesta visita nada foi gravado. O catálogo de modelos de serviço do funil, em `/funnel/models/service`, é a SPEC-026 e fica de fora deste arquivo.

## 3. Requisitos relacionados

RF-057, Observado. Nomeia busca, filtro, exportação, mescla, os status completo, incompleto ou dados solicitados, e as abas de catálogo, solicitação de dados e lista de serviços. O mesmo RF descreve o formulário de oito abas e trata nome e serviços como obrigatórios. Este formulário não foi aberto nesta visita. O status Observado não aprova esta spec.

RF-058, Observado. No evento, cada serviço tem fornecedor, tarefas e valor, e a home compara estimado e contratado. A nota desse RF diz que a lista de categorias vista é a de um modelo de casamento. Esta visita não abriu o evento. RF-058 não descreve a lista em `/suppliers/services` e não aprova esta spec.

O corte aprovado em [mvp-scope.md](../mvp-scope.md), seção 5, deixa RF-057 e RF-058 de fora. O fluxo não está no Must e não foi confirmado para o produto.

Não há RF novo. Contagens, a ação "Ver todos", os grupos de colunas da solicitação e a marca Padrão foram vistos e não estão fechados em RF-057 nem em RF-058. Sem RF, o efeito desses itens permanece não definido.

## 4. Atores e permissões

A tela foi vista na conta autenticada da assessoria. Cliente, assistente e fornecedor não foram exercitados nestas rotas. Quem mais abre, cria ou altera permanece não definido. A role `User` não é papel de negócio, RF-040.

## 5. Pré-condições

Conta autenticada, SPEC-001. A visita abriu as três rotas e não gravou cadastro, serviço nem solicitação.

## 6. Fluxo principal

1. A assessoria abre o catálogo.
2. O catálogo mostra nome, serviços, cadastro completo ou incompleto, contagens e a ação "Ver todos".
3. A assessoria abre a solicitação de dados.
4. A solicitação mostra colunas de incompletos e colunas de preenchimento solicitado.
5. A assessoria abre a lista de serviços.
6. A lista mostra o nome do serviço. Alguns serviços estão marcados como Padrão.
7. A visita termina sem gravar.

## 7. Fluxos alternativos

- Acionar "Ver todos": a ação foi vista e não foi acionada. O destino permanece não definido.
- Adicionar, editar, excluir e baixar: não foram acionados. Resultado, formulário e arquivo permanecem pendentes.
- Busca, filtro, exportação e mescla: RF-057 os nomeia. Nesta visita não foram exercidos. O comportamento aqui permanece não definido.
- Formulário do fornecedor: não foi aberto. As oito abas e a obrigatoriedade de nome e serviços continuam só no texto de RF-057.
- Serviço dentro do evento: não foi aberto. Permanece em RF-058.

## 8. Regras de negócio

- O catálogo, a solicitação de dados e a lista de serviços são as três áreas vistas deste fluxo. RF-057, Observado.
- No catálogo, o fornecedor aparece com nome, serviços e cadastro completo ou incompleto. RF-057, Observado. O terceiro status citado nesse RF, dados solicitados, não foi lido como rótulo nesta lista.
- A solicitação de dados mostra colunas de incompletos e colunas de preenchimento solicitado. Isso é o que a aba mostrou. RF-057 nomeia dados solicitados. A equivalência entre esse status e o preenchimento solicitado não foi exercida e permanece não definida.
- Contagens e "Ver todos" aparecem no catálogo. Não há RF para o que cada contagem soma nem para o destino da ação. Ficam como observação da tela. O efeito permanece não definido.
- A lista de serviços mostra o nome do serviço. Alguns itens carregam a marca Padrão. Não há RF para essa marca. O que ela permite ou impede permanece não definido.
- Esta lista e o catálogo `/funnel/models/service` são catálogos distintos. A relação entre eles, se houver, não foi vista e permanece não definida. SPEC-026.
- Fornecedor, tarefas e valor do serviço no evento, e a comparação de estimado e contratado, permanecem em RF-058, Observado, fora do que esta visita abriu.

## 9. Validações

Nenhuma validação foi exercida. O formulário não abriu e nada foi gravado. Obrigatório, tamanho, formato e duplicidade permanecem não definidos nesta visita. A obrigatoriedade de nome e serviços citada em RF-057 não foi revalidada.

## 10. Estados

Estados vistos no catálogo: cadastro completo e cadastro incompleto.

Estado visto na solicitação de dados: colunas de incompletos e colunas de preenchimento solicitado. A passagem de um para o outro não foi acionada.

Estado visto na lista de serviços: nome do serviço, com a marca Padrão em alguns itens.

Outro estado do fornecedor, da solicitação ou do serviço permanece não definido.

## 11. Dados necessários

No catálogo, o item visto traz nome, serviços, situação do cadastro e contagens. O que cada contagem mede não foi registrado.

Na solicitação de dados, a tela agrupa incompletos e preenchimento solicitado. O título de cada coluna não foi registrado.

Na lista de serviços, o dado visto é o nome do serviço, com a marca Padrão em parte dos itens.

Demais campos do fornecedor e do serviço permanecem não definidos neste arquivo.

## 12. Interface

Três rotas vistas na referência:

- `/suppliers`: nome, serviços, cadastro completo ou incompleto, contagens e "Ver todos".
- `/suppliers/data-request`: colunas de incompletos e colunas de preenchimento solicitado.
- `/suppliers/services`: nome do serviço, alguns marcados como Padrão.

Adicionar, editar, excluir e baixar estavam fora do que foi acionado. Formulário, diálogo e arquivo não foram vistos.

## 13. Erros

Nenhum erro foi visto. Mensagem de falha, de permissão e de validação permanece não definida.

## 14. Critérios de aceite

1. O catálogo mostra nome, serviços, cadastro completo ou incompleto, contagens e a ação "Ver todos".
2. A solicitação de dados mostra colunas de incompletos e colunas de preenchimento solicitado.
3. A lista de serviços mostra o nome do serviço, e alguns serviços com a marca Padrão.
4. As três telas permanecem distintas do catálogo de modelos de serviço do funil, SPEC-026.
5. O aceite cobre essa consulta. Adicionar, editar, excluir e baixar ficam pendentes.

Enquanto o status for rascunho e RF-057 e RF-058 forem Observado, o critério descreve a visita. Não autoriza implementação.

## 15. Cenários de teste

Conta de referência abre `/suppliers`. Resultado: lista com nome, serviços, cadastro completo ou incompleto, contagens e "Ver todos".

A mesma conta abre `/suppliers/data-request`. Resultado: colunas de incompletos e colunas de preenchimento solicitado.

A mesma conta abre `/suppliers/services`. Resultado: nome do serviço, com a marca Padrão em alguns itens.

O cenário não aciona "Ver todos", adicionar, editar, excluir nem baixar, e não grava dado.

## 16. Dependências

SPEC-001. SPEC-026 é o outro catálogo, em `/funnel/models/service`. RF-057 e RF-058 seguem Observado. Negociação do fornecedor no evento (RF-059) e link público do fornecedor (RF-060) ficam fora deste arquivo e também estão Observado.

## 17. Premissas e pontos não definidos

- O fluxo está fora do Must. RF-057 e RF-058 são Observado e não aprovam esta spec.
- Adicionar, editar, excluir e baixar: pendentes. Não foram acionados.
- Destino de "Ver todos" e o que as contagens medem: não definidos.
- Títulos das colunas da solicitação de dados: não registrados.
- Marca Padrão: vista em alguns serviços. Efeito não definido.
- Busca, filtro, exportação, mescla e o formulário de oito abas: texto de RF-057, não exercidos nesta visita.
- Serviço no evento, com fornecedor, tarefas, valor e comparação de estimado e contratado: RF-058, não aberto.
- Ligação com a negociação (RF-059) e com o link do fornecedor (RF-060): não vista.
- Isolamento entre contas: não exercido.
- Abertura sem sessão: não testada.
