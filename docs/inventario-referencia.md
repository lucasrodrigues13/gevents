# Inventário da plataforma de referência

**Status: observado.** Leitura tela a tela da plataforma que o cliente usa hoje, feita em 08/11/2026 com autorização para cadastro completo. Nada aqui é requisito aprovado. O que virar regra do gevents entra em [requirements.md](requirements.md) com ID próprio e depois numa spec.

Convenção: `*` marca campo obrigatório na tela. Listas fechadas vêm na ordem em que a tela mostra. Nomes, contatos e documentos de pessoas reais da conta não foram copiados. O evento de teste chama-se `TESTE LUCÃO`.

## 1. Gestão de eventos

### 1.1. Lista de eventos

Rota `/gestao_eventos`.

Atalhos no topo: Adicionar novo evento, Serviços e Fornecedores, Meus clientes, Modelos (submenu: evento, roteiro, briefing), Assistentes (abre a aba de assistentes do perfil).

Bloco "Resumo de todos os eventos": Pagamentos, Tarefas (com contador "Para essa semana"), Anexos, Equipe escalada. Bloco "Aniversários": Lembretes de aniversários (com contador "Para hoje").

Abas, cada uma com a contagem: Em andamento, Concluído, Cancelado, Estatísticas.

Busca: "Buscar evento pelo nome". Botão "Mais filtros" abre: Tipo do evento, Membros da equipe presente, Assistente, Período do evento, Modelo de evento, Tags. Todos com busca e "limpar seleção".

Card do evento: tipo, nome, "Criado em" com data e hora, modelo, tags, contagem regressiva ("Faltam N dias"), data, dia da semana, "X de Y concluídas" (tarefas), "X de Y confirmados" (convidados). Ações: Editar, Cancelar, Remover. O card inteiro abre a home do evento.

### 1.2. Aba Estatísticas

- Total de eventos.
- Concluídos, em andamento e cancelados, cada um com quantidade e percentual.
- Gráfico "Eventos por mês (ano atual)".
- "Ranking dos serviços que mais consomem as partes do orçamento dos seus eventos", calculado sobre eventos concluídos: posição, serviço, percentual.
- "Ranking dos fornecedores mais contratados": posição, fornecedor, quantidade.
- "Ranking dos serviços mais utilizados": posição, serviço, quantidade.

### 1.3. Adicionar novo evento

Rota `/gestao_eventos/create`. Até escolher o modelo, só a foto e o campo de modelo aparecem. O restante do formulário depende do modelo, que define o tipo do evento.

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Foto | Arquivo | Não | Botões Inserir, Alterar, Remover foto. |
| Modelo de evento | Lista | Sim | Vistos: Checklist 15 Anos, Checklist de Casamento, Checklist de Aniversário, Checklist Corporativo, Checklist de Formatura, e o modelo próprio da conta "Cronograma de Tarefas Casamento - Assessoria DN". |
| Cadastrar clientes no evento | Interruptor | Não | Desligado por padrão. Ligado, abre dois blocos de cliente (abaixo). Aviso: "Seu cliente será notificado sobre o primeiro acesso à plataforma." |
| Nome da(o) noiva(o) | Texto | Sim | Dois campos no casamento. Em outros tipos o rótulo é "Nome". O nome do evento nasce da junção: `TESTE` e `LUCÃO` viraram "TESTE & LUCÃO", e o título mostrou "Casamento de TESTE & LUCÃO". |
| Data | Data | Não | Calendário, formato dd/mm/aaaa. |
| Horário | Hora | Não | hh:mm. |
| Budget | Moeda | Não | Máscara R$, digitação em centavos. |
| Convidados | Número | Não | "Insira o estimado". |
| Local da cerimônia | Texto | Não | |
| Local da recepção | Texto | Não | |
| Tags do evento | Lista múltipla | Não | Sugestões da conta: Assessoria completa, Assessoria parcial, Assessoria final. Aceita digitar tag nova. |
| Notificações via e-mail sobre esse evento | Três marcações | Não | enviar para assessor(a), enviar para assistentes, enviar para clientes. Todas vêm marcadas. |
| Salvar evento na minha agenda | Marcação | Não | Ao marcar, aparece "Vincular com agenda do google?" (lista; vazia na conta sem Google). |

Bloco de cliente, quando o interruptor está ligado. Dois blocos, um por pessoa:

| Campo | Cliente 1 | Cliente 2 |
|---|---|---|
| Nome da(o) noiva(o) | Obrigatório | Obrigatório |
| Esse(a) cliente é | Obrigatório. No casamento: Noiva, Noivo, Representante. | Obrigatório |
| E-mail | Obrigatório | Opcional |
| DDI | Lista de países, `+55 - Brasil` padrão | Idem |
| Celular | Obrigatório, máscara (00) 00000 0000 | Opcional |

Ao salvar, antes de abrir o evento, a plataforma pede o valor do serviço da própria assessoria:

| Campo | Tipo | Obrigatório |
|---|---|---|
| Incluir valor de entrada | Marcação | Não. Ligada, abre Base de cálculo (Valor, Porcentagem), Valor de entrada e Data de entrada. |
| Valor total | Moeda | Sim |
| Qtd. de parcelas | Número | Sim. Fica desabilitado até informar o valor total. |
| Data da 1ª parcela | Data | Preenchida com hoje. |

Esse passo cria o serviço "Assessoria e Organização" no evento, com a própria assessoria como fornecedor contratado e o valor nas parcelas. Depois vem a tela "Evento criado com sucesso!" com o botão "Acessar evento".

Dados gravados no teste: modelo DN, data 20/06/2027 16:00, budget R$ 150.000,00, 150 convidados, Igreja Teste, Espaço Teste, tag Assessoria completa, agenda marcada, assessoria R$ 15.000,00 com entrada R$ 3.000,00 e 4 parcelas a partir de 10/12/2026. Cliente não cadastrado no evento para não disparar e-mail a terceiro.

### 1.4. Home do evento

Rota `/gestao_eventos/view2/{id}`. Menu do evento: Home, Convidados (Convites e Convidados, RSVP e Mensagens, Mapeamento de mesa, Checklist de presença), Pagamentos, Tarefas, Roteiro, Mais (Anexos com abas Fornecedores, Serviços, Tarefas, Observações gerais; Inspirações; Briefing; Cronograma). Botão Voltar.

Cabeçalho: imagem (Adicionar imagem), contagem regressiva em dias, horas, minutos e segundos, nome do evento, data por extenso com hora e dia da semana, Cerimônia, Recepção, "Cód. do evento" (exemplo de formato: `12729CXQT`), botões Escalar equipe do dia e Gerenciar clientes.

Menu de ações da home: Editar evento, Gerenciar clientes, Permissões dos clientes, Escalar equipe do dia, Planejamento de budget, Site de casamento, Observações gerais, Adicionar ou remover serviços, Agrupar serviços (pacotes), Ordenar serviços, Ocultar serviços, Solicitações aos fornecedores, Alinhamento de todos os fornecedores, Enviar mensagem para fornecedores contratados.

Cartões:

- Budget: valor estimado (do formulário), valor contratado (soma dos serviços contratados), link Planejamento de budget.
- Pagamentos: Pago, Atrasado, link Acessar pagamentos.
- Tarefas: "X de Y completas", "N atrasadas", link Acessar tarefas.
- Convidados: Total, Est. (estimado do formulário), pendentes, confirmados, não comparecerão, link Acessar convidados.

Lista "Serviços e Fornecedores", com colunas Serviços, Fornecedores, Tarefas, Valores. Cada serviço do modelo vira uma linha com botão "Adicionar fornecedor", "X de Y concluídas" das tarefas do serviço, link Ver tarefas e valor. O serviço "Assessoria e Organização" já nasce com a própria conta como fornecedor, situação Contratado, percentual de tarefas, valor orçado e valor contratado, e ícones info, delete e hub. Ações do bloco: Fornecedores (Solicitação aos fornecedores, Alinhamento de todos fornecedores, Enviar mensagem para fornecedores contratados) e Serviços (Adicionar ou remover, Agrupar em pacotes, Ocultar para cliente, Ordenar).

Serviços criados pelo modelo DN no teste (65): Assessoria e Organização, Acessórios e jóias, Banda, Bar de drinks, Barbearia e dia do noivo, Beleza e dia da noiva, Bem-casados, Bolo, Buffet, Carro da noiva, Cartório e casamento civil, Celebrante, Convites e papelaria, Coreógrafa, Decoração, DJ, Doces, ECAD, Filmagem, Forminhas de doces, Fotografia, Gerador de energia, Igreja, Som e iluminação, Local da recepção, Lua de mel e viagens, Manobrista, Músicos para cerimônia, Pajens e damas, Planejamento, Presente de padrinhos, Recreação infantil, Trajes masculinos, Vestido, Recepcionista, Material de higiene para toaletes, Kit toilet, Segurança, Limpeza de toillet, Cerimônia, Site e lista de presentes, Calígrafa, Chinelos, Whisky, Espumante, Quebras Buffet, Quebras Bar, Alimentação staff, RSVP, Mapeamento de salão, Som para cerimônia, Identidade Visual, Rider de Banda, Tendas e coberturas, Sapato da noiva, Hospedagem atração, Limpeza Pré Festa, Bolo Cenográfico, Alianças, Porta-alianças, Robes, Locação de materiais, Pista de dança, Carpete, Atração, Beleza das madrinhas e convidadas, Mesa posta, Motorista.

Bloco "Site de casamento": peça da plataforma, fora do produto (RF-079).

Bloco "Pendências do evento": abas Tarefas e Pagamentos, "do mês ou em atraso". Cada tarefa mostra título, "Até dd/mm/aaaa" e o serviço. As 165 tarefas do modelo nasceram com prazo calculado a partir da data do evento; três já estavam atrasadas no dia da criação.

Bloco "Compromissos do evento": abas Próximos e Anteriores. O evento salvo na agenda aparece como compromisso com data, hora, "em 7 meses" e nome.

Bloco "Despesas com o evento": "Nenhuma despesa cadastrada!".

### 1.5. Editar evento

Modal aberto pelo lápis do cabeçalho. Botão Salvar no topo. Quatro abas.

Aba Informações gerais:

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Foto | Imagem (Inserir foto) | Não | |
| Noiva(o) | Texto | Sim | Rótulo muda conforme o modelo do evento. |
| Noivo(a) | Texto | Sim | |
| Data | Data | Não | |
| Horário (Convite) | Hora `__:__` | Não | É a hora que aparece no convite e no cabeçalho. |
| Budget estimado | Moeda | Não | |
| URL do site | Texto, somente leitura | | "Esse campo é automaticamente preenchido quando o evento for integrado com o Casar.com!" Fora do produto (RF-079). |
| Senha do site | Texto | Não | Idem. |
| Tags do evento | Múltipla escolha com criação | Não | Mesmas tags do cadastro. |
| Notificações via e-mail / push | Três marcações | | Assessor(a), Assistentes, Clientes. |

Aba Detalhes do evento:

| Campo | Tipo | Obrigatório |
|---|---|---|
| Qtd de convidados estimado | Número | Não |
| Tempo de festa | Hora `__:__` | Não |
| Horário de início da cerimônia | Hora `__:__` | Não |
| Horário de início da recepção | Hora `__:__` | Não |
| Qtde de padrinhos | Número | Não |
| Qtde de damas e pajens | Número | Não |

Abas Cerimônia e Recepção, com os mesmos campos:

| Campo | Tipo | Observação |
|---|---|---|
| Local | Texto | Nome do local (igreja, espaço). |
| País | Busca, padrão Brasil | |
| CEP | Máscara `_____-___` | Ao completar, preenche Endereço, Estado, Cidade e Bairro automaticamente. |
| Endereço | Texto | |
| Número | Texto | |
| Estado | Busca | |
| Cidade | Busca, depende do estado | |
| Bairro | Texto | |

A home monta link para o Google Maps a partir do endereço. Dados gravados no teste: tempo de festa 06:00, cerimônia 16:00, recepção 18:00, 10 padrinhos, 4 damas e pajens; cerimônia na Avenida Afonso Pena, 100, Centro, Belo Horizonte, MG, CEP 30130-000; recepção no CEP 30140-000, número 200, Savassi.

### 1.6. Observações gerais do evento

Modal aberto pelo terceiro botão do cabeçalho ou pelo menu de ações. Lista de observações com busca ("Pesquisar"); cada observação mostra data, hora e autor ("08/11/2026 às 13:40 por {conta}"), o texto formatado e os botões compartilhar, imprimir, excluir e editar. Botão compartilhar também no topo do modal.

Formulário "Adicionar observação":

| Campo | Tipo | Observação |
|---|---|---|
| Observação | Texto rico (editor Froala: negrito, itálico, sublinhado, listas, alinhamento, link, imagem, tabela, fonte, tamanho, cores, cabeçalhos, emoticons, linha horizontal, tela cheia) | Placeholder "Digite algo". |
| Modelos de observação | Botão | Abre "Selecionar modelo" com a lista de modelos da conta e botão "Novo modelo". |
| Adicionar anexo | Botão | Arquivo anexado à observação. |

Formulário "Adicionar modelo" (modelo de observação): Nome do modelo (texto) e Observação (texto rico). Botão Salvar. A conta de teste não tinha modelos.

Dados gravados no teste: uma observação "Observação geral de teste do evento TESTE & LUCÃO.".

### 1.7. Gerenciar clientes

Modal com abas "Clientes do evento" e "Permissões" e botão "Cadastrar novo cliente". Sem cliente, mostra "Nenhum cliente adicionado ao evento!" e "Para que os clientes do evento tenham acesso à plataforma e ao aplicativo, clique no botão acima." A aba Permissões fica vazia enquanto não houver cliente.

"Cadastrar novo cliente" abre três opções:

| Opção | O que faz |
|---|---|
| Via link | "Envie o link para que o próprio cliente se cadastre e automaticamente seja adicionado ao evento." Abre "Enviar via" com dois canais: WhatsApp (campos DDI, padrão +55; Celular com DDD; Mensagem, textarea pré-preenchida com saudação, link único `/createOwnerByLink/{token}` e texto promocional do Casar.com; botões Cancelar e Enviar) e Link (campo somente leitura com a URL, botões Fechar e Copiar). |
| Selecionar existente | "Selecione um cliente já cadastrado anteriormente para ser adicionado ao evento." Abre "Selecionar cliente já cadastrado" com busca "Digite o nome do cliente que deseja adicionar" e lista alfabética agrupada por letra, cada linha com nome e papel (Noiva, Noivo...). |
| Cadastrar novo | "Cadastre seu cliente manualmente para adicioná-lo ao evento. Ele receberá o acesso via e-mail e você também poderá enviar..." Abre o mesmo formulário de cliente da seção 7 (abas Dados pessoais, Redes sociais, Endereço) com título "Adicionar cliente"; ao salvar, o cliente já fica vinculado ao evento. |

O vínculo cliente-evento é sempre feito a partir do evento. O cadastro de cliente em `/clientes` não escolhe evento.

Não foi criado cliente no teste porque a plataforma envia e-mail de acesso. Para completar o teste é preciso um e-mail real do usuário.

### 1.8. Escalar equipe do dia

Modal com abas "Minha equipe" e "Mural de recados" e botão "Adicionar novo membro".

Aba Minha equipe: tabela Nome, E-mail, Escalar. A coluna Escalar é um círculo de marcação; ao marcar, a plataforma pergunta "Deseja enviar notificação para o email do membro da equipe escalada?" com botões Enviar e Não notificar. Membro escalado ganha ícone de WhatsApp. Se o membro tiver compromisso na data do evento, aparece o aviso em vermelho "Possui um compromisso nessa data". A equipe é da conta (reaproveitada entre eventos); a escala é por evento.

Aba Mural de recados: lista de recados e textarea "Deixe um recado para sua equipe do dia! Eles receberão no e-mail".

Formulário "Adicionar membro", abas Dados, Endereço, Dados bancários, Compromissos; botão Salvar.

Aba Dados:

| Campo | Tipo | Obrigatório |
|---|---|---|
| Nome | Texto | Sim |
| E-mail | E-mail | Sim |
| Celular | Telefone | Sim |
| Telefone | Telefone | Não |
| Data de nascimento | Data | Não |
| CPF | Máscara | Não |
| RG | Texto | Não |
| Outras informações | Texto longo | Não |

Aba Endereço: País (busca, padrão Brasil), CEP `_____-___`, Endereço, Número, Estado (busca), Cidade (busca), Bairro, Complemento. Mesmo bloco de endereço do cliente.

Aba Dados bancários: lista de contas ("Nenhuma conta bancária cadastrada!") e botões "Adicionar conta" e "Adicionar Pix".

- Adicionar dados bancários: Banco, Agência, Conta, Nome do titular, Tipo de conta (Corrente, Poupança), Documentos (CPF ou CNPJ). Botão Adicionar.
- Adicionar Pix: escolha do tipo de chave (CNPJ, CPF, Celular, E-mail, Chave aleatória) e o campo da chave, com placeholder "{tipo} de quem irá receber" e máscara por tipo. Botão Adicionar.

Aba Compromissos: lista e botão "Adicionar compromisso", que abre "Adicionar novo compromisso" com Descrição ("Descreva o compromisso") e Data ("Digite a data"). Botão Salvar. Serve para avisar conflito de agenda na escala.

Dados gravados no teste: membro "Equipe Teste Lucao", e-mail `equipe.teste.lucao@example.invalid`, celular (31) 99999-0001, telefone (31) 3333-0001, nascimento 01/01/1990, CPF 000.000.000-00, RG MG-00.000.000, escalado no evento sem notificação. A máscara do telefone fixo aceitou o valor como `(31) 33330-001`, isto é, o campo usa máscara de celular.

### 1.9. Planejamento de budget

Modal grande aberto pelo cartão Budget ou pelo menu. Botão "Exportar para excel".

Resumo no topo:

| Indicador | Cálculo observado |
|---|---|
| Budget | Budget estimado do evento (R$ 150.000,00). |
| Estimado | Soma dos valores estimados dos serviços (R$ 8.000,00 após o teste). |
| Contratado | Soma dos valores contratados dos fornecedores (R$ 15.000,00). |
| Custo estimado por convidado | (Estimado + Contratado) / convidados estimados. Com 150 convidados: R$ 100,00 antes, R$ 153,33 depois do teste. |
| Prova real | Estimado + Contratado (R$ 23.000,00). |

Busca "Buscar serviço". Tabela com colunas Serviço, Valor Estimado, Valor Contratado. Uma linha por serviço do evento:

- Valor Estimado: "Clique para editar" abre o mini-modal "Salvar valor" com um campo moeda e botão Salvar.
- Valor Contratado: "-" quando não há fornecedor contratado; senão o valor e o nome do fornecedor.
- "Adicionar subitem": abre "Adicionar subitem" com Descrição, Quantidade e Valor unitário (moeda). O subitem aparece sob o serviço com descrição, quantidade, valor unitário, valor total (quantidade × unitário) e botões editar e excluir. Subitens não alteram o Valor Estimado do serviço; são detalhamento.

Dados gravados no teste: Banda com estimado R$ 8.000,00 e subitem "Cachê banda (teste)", 2 × R$ 1.500,00 = R$ 3.000,00. Depois de recarregar a home, o estimado da Banda passou a R$ 3.000,00: quando existem subitens, o valor estimado do serviço é a soma deles.

### 1.10. Linha de serviço na home

Cada serviço do evento ocupa uma linha com quatro colunas:

- Serviços: nome e, ao passar o mouse, botões "Resumo do serviço" (info), "Remover" (lixeira) e "Agrupar" (sitemap).
- Fornecedores: trilha de quatro etapas com avatares dos fornecedores em cada uma: Recomendado, Orçamento solicitado, Orçamento recebido, Contratado. Botão "Adicionar fornecedor".
- Tarefas: "X de Y concluídas", barra de progresso e link "Ver tarefas".
- Valores: Estimado (lápis abre "Editar valor" com um campo moeda e botão Editar) e, se houver fornecedor contratado, Contratado em verde e "% em relação ao budget" (R$ 10.000,00 de R$ 150.000,00 = 6.7%).

"Resumo do serviço" abre um modal com o nome do serviço e quatro abas:

| Aba | Conteúdo |
|---|---|
| Observações do serviço | Lista de observações e caixa "Digite uma observação aqui" com menção (@) e enviar. |
| Fornecedores | Botão "Gerenciar fornecedores" e lista de fornecedores do serviço com etapa e valor (ex.: Banda Teste Lucão, Contratado, R$ 10.000,00). |
| Comparativo | Sub-abas Comparativos e Comentários. Comparativos é uma matriz: linhas são critérios (botão "Adicionar comparativo", campo texto único; cada linha com editar e excluir), colunas são os fornecedores do serviço; cada célula aceita sim (check), não (X), em análise (lupa) ou texto livre (lápis abre "Adicionar" com textarea). Botões "Ordenar comparativos", "Ocultar fornecedores", compartilhar e imprimir (`/printEventServiceComparative/{uuid}`). Comentários é um chat com anexo. |
| Anexos | "Adicionar anexo". |

"Agrupar" abre "Adicionar agrupamento": "Serviço principal: {serviço}", busca "Buscar serviço", marcação "Mostrar todos os serviços" e lista dos serviços do evento para marcar; serviço já agrupado mostra "Já está sendo utilizado em outro agrupamento". Botão Adicionar. O menu Serviços tem "Agrupar serviços (Pacotes)", que abre "Gerenciar serviços agrupados" com a lista de pacotes e botão "Criar agrupamento".

"Ver tarefas" abre "Tarefas do serviço {serviço}": botão "Adicionar tarefa" e lista de tarefas do serviço. Cada item tem bolinha de situação (amarela pendente, azul em andamento, verde finalizada; clicar troca), título, ícones de perguntas, checklist, comentários e anexos, prazo, serviço e fornecedor contratado, avatares dos responsáveis, botões editar e excluir. Formulário de tarefa na seção 2.4.

Dados gravados no teste: comparativo "Comparativo de bandas (teste)" no serviço Banda; tarefa "Tarefa de teste da banda" com prazo 15/01/2027.

### 1.11. Gerenciar fornecedores de um serviço

Botão "Adicionar fornecedor" abre "Gerenciar fornecedores de {serviço}": botões "Adicionar novo fornecedor" e "Notificar clientes"; busca "Buscar pelo nome do fornecedor..."; filtros UF (27 estados) e Cidade ("Selecione um estado antes!"). Lista alfabética de todos os fornecedores da conta que oferecem o serviço (124 para Banda), cada um com foto, nome, "Cidade - Estado" e dois botões: Recomendar e Contratar.

- Recomendar: fornecedor vira "Recomendado" com X para desfazer e link "Ver fornecedor".
- Contratar: vira "Contratado" direto, sem pedir valor; o valor é informado depois no hub.
- Notificar clientes: abre "Enviar via" WhatsApp com DDI, celular e mensagem pré-preenchida "Segue o link para visualizar recomendações de fornecedores para {serviço}!" com link direto para o modal do serviço (`?modal=eventService&eventServiceId={id}&tab=suppliers`).

Formulário "Adicionar novo fornecedor" (também é o cadastro de fornecedor em `/suppliers`): botão Salvar no topo; oito abas.

Aba Dados:

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Foto | Imagem | Não | |
| Nome do fornecedor | Texto | Sim | |
| Oferece o(s) serviço(s) de | Múltipla escolha com busca | Sim | Catálogo global da plataforma com 663 serviços; vem pré-marcado com o serviço de origem. |
| Telefone | Lista | Não | "Clique aqui para adicionar um telefone" abre "Adicionar telefone": Nome do responsável (opcional), DDI (+55), Número com DDD, tipo Móvel ou Fixo. Cada telefone na lista tem editar e remover e aparece como "Nome \| +55 número \| Móvel". |
| E-mail | Lista | Não | "Adicionar e-mail": um campo E-mail. Lista com editar e remover. |
| Razão social | Texto | Não | |
| CNPJ | Máscara `__.___.___/____-__` | Não | |
| CPF | Máscara `___.___.___-__` | Não | |
| Nome do responsável | Texto | Não | |
| Faixa de preço | Duas moedas ("de" e "até") | Não | |

Aba Redes sociais: Site, Facebook, Instagram, Youtube, Pinterest.

Aba Endereço: mesmo bloco (País, CEP, Endereço, Número, Estado, Cidade, Bairro, Complemento). Ao salvar sem estado, a plataforma abre "Adicionar estado" e exige escolher um.

Aba Dados bancários: mesmas opções do membro da equipe (Adicionar conta; Adicionar Pix).

Aba Orçamentos ("Área restrita apenas para assessor(a) e assistente(s)!"): lista e "Adicionar orçamento" com Título do orçamento (ex.: "Tabela de variação dos docinhos"), modo ANEXO (botão Adicionar anexo) ou LINK (campo "Insira o link aqui", aviso "Lembre-se de colocar o link com HTTP ou HTTPS no inicio."), marcação "Deixar visível aos clientes". São orçamentos genéricos do fornecedor, não de um evento.

Aba Diferencial: textarea "Insira abaixo os diferenciais que seus clientes terão com esse fornecedor" (ex.: "20% de desconto e um brinde"). Visível ao cliente.

Aba Inf. gerais (restrita): textarea de observações internas (ex.: capacidade, ar condicionado, duração).

Aba Portfólio (restrita): Links ("Adicionar link": Título, URL), Anexos ("Adicionar anexos"), Galeria ("Adicionar imagens").

Ao salvar, a plataforma pergunta "Gostaria de enviar uma solicitação para o fornecedor completar o preenchimento dos dados faltantes?" (Sim/Não) e depois "Gostaria de compartilhar esse fornecedor?" (X/check). A solicitação de preenchimento vai por WhatsApp com link `/supplierRegistration/{token}` e texto padrão.

Ao editar um fornecedor existente ("mais infos" no hub), o título é "Editar fornecedor", há o botão "Solicitar preenchimento ao fornecedor" e uma aba extra Eventos (restrita): filtros Status do evento (Em andamento, Concluído, Cancelado) e Negociação (Recomendado, Orçamento enviado, Orçamento recebido, Contratado), lista de eventos com data, serviço, negociação e valor.

Dados gravados no teste: fornecedor "Banda Teste Lucão", serviço Banda, telefone móvel (31) 99999-0002 com contato "Contato Banda Teste", e-mail `banda.teste@example.invalid`, razão social "Banda Teste Lucão Ltda", CNPJ 00.000.000/0001-00, responsável "Responsável Teste", faixa R$ 5.000,00 a R$ 12.000,00, estado Minas Gerais. Contratado no evento.

### 1.12. Hub do fornecedor no evento

"Ver fornecedor" (ou o avatar na trilha) abre "{Fornecedor} - {Serviço}", um modal com painel lateral e seis abas.

Painel lateral: foto, botão "mais infos" (abre Editar fornecedor), Diferenciais, Orçamentos fixados, Dados bancários, Pix, Outras informações (Responsável, Email, Telefone com link "Chamar no WhatsApp" `wa.me/55...`, Redes sociais, Endereço).

Aba Orçamento: funil de negociação em quatro etapas empilhadas, com seta para voltar uma etapa:

| Etapa | Ações |
|---|---|
| Fornecedor me interessa (Recomendado) | "Já tenho o orçamento", "Solicitar orçamento". |
| Orçamento solicitado | "Alterar para orçamento recebido", "Descartar fornecedor". |
| Orçamento recebido | Campo moeda do valor, "Alterar para contratado", "Descartar orçamento". |
| Contratado | "Contratado: R$ X" com lápis, "Solicitar contrato para o fornecedor", bloco de parcelas. |

Lápis do contratado abre "Reajustar o valor contratado": valor atual, operador (+ ou -), Valor, resultado "= R$ X", "Dividir o valor somado em quantas parcelas?" (número) com botão Adicionar, botão Salvar.

Bloco de parcelas do contratado:

| Campo | Tipo | Observação |
|---|---|---|
| Pagamento com entrada | Marcação | Abre Base (Valor, Porcentagem), valor da entrada (moeda ou `% ##,##`) e "Data da entrada". |
| Qtde de parcelas | Número | |
| Data da 1ª parcela | Data | |
| Gerar parcelas | Botão | Gera entrada + N parcelas mensais iguais. |

Cada parcela: situação (Pendente), vencimento, valor, "Método de pgto" (Cartão de crédito, Cartão de débito, Boleto, TED, DOC, Cheque, Dinheiro, Transferência, Pix, Permuta, Outros), botões compartilhar (lembrete por WhatsApp com "Modelos de mensagem"), comprovantes ("Comprovantes" com botão Anexar) e excluir. Clicar na situação abre "Parcela": Status (Pago, Pendente), Data de vencimento*, Data de pagamento, Valor*, Valor de juros, Pago por (clientes do evento), Forma de pagamento, Observação. Botão Salvar. Abaixo: "Remover todas as parcelas" e quatro marcações ligadas por padrão: "Permitir que cliente check parcelas", "Permitir que cliente adicione parcelas", "Permitir que cliente remova parcelas", "Permitir que cliente edite as parcelas".

"Solicitar contrato para o fornecedor" abre "Enviar solicitação de contrato": Enviar por WhatsApp (DDI, Celular com DDD, atalhos com os telefones cadastrados) ou E-mail (campo com "separe por vírgula (,) para enviar para mais de um e-mail", atalhos com os e-mails), marcação "cliente ser notificado da resposta", "Mensagem para fornecedor" com "Gerar mensagem automática" e "Modelos de mensagem", "Adicionar anexo", Enviar.

Aba Chat: mensagens com o fornecedor, anexo, ajuda e enviar.

Aba Observações: observações internas, com autor e data/hora ("{conta} - 08/11/2026 às 13:56"), caixa com menção (@) e enviar.

Aba Anexos: busca "Pesquisar anexo..." e "Adicionar anexo" com dois tipos: Contrato (somente PDF) e Outros documentos (múltiplos).

Aba Alinhamentos: "Adicionar alinhamento" com Alinhamento (textarea "Descreva o alinhamento...") e Resposta (texto rico). Lista mostra "Aguardando confirmação do fornecedor", "Compartilhe com o fornecedor para que confirme as informações!", botões compartilhar, imprimir, ordenar, anexar; cada alinhamento tem situação (ampulheta), editar e excluir.

Aba Portfólio: "Nenhum portfólio adicionado!" e "Gerenciar portfólio".

Dados gravados no teste: Banda Teste Lucão contratada por R$ 10.000,00, entrada R$ 2.000,00 em 01/12/2026 e 4 parcelas de R$ 2.000,00 a partir de 10/01/2027; alinhamento "Horário de passagem de som (teste)"; observação "Observação interna sobre a banda (teste).".

### 1.13. Menus Serviços e Fornecedores da home

Menu Serviços:

| Item | Tela |
|---|---|
| Adicionar ou remover serviços | "Gerenciar serviços do evento": busca e lista do catálogo inteiro (663 + os da conta) com marcação; marcados são os serviços do evento. Buscar nome inexistente mostra "Você não possui o serviço “X” cadastrado. Deseja adicionar esse serviço em sua conta?" e "Sim, adicionar", que abre "Adicionar novo serviço" com "Nome do serviço" (até 200 caracteres, com sugestões). Botão Salvar recarrega a home. |
| Agrupar serviços (Pacotes) | "Gerenciar serviços agrupados": lista de pacotes e "Criar agrupamento". |
| Planejamento de budget | Seção 1.9. |
| Ocultar serviços para cliente | "Os serviços que você ocultar não aparecerão para os clientes do evento." Busca e, por serviço, "Oculto para:" com seleção de clientes. |
| Ordenar serviços | "Arraste e solte para ordenar" e botão "Colocar em ordem alfabética". |

Menu Fornecedores:

| Item | Tela |
|---|---|
| Solicitação aos fornecedores | "Solicitações aos fornecedores": abas Orçamentos e Contratos, filtros Aguardando resposta e Respondido, busca por fornecedor. |
| Alinhamento de todos fornecedores | "Alinhamento de todos os fornecedores contratados": busca, Filtro, contadores (alinhamentos, preenchidos, a serem preenchidos, enviados para o cronograma), lista por fornecedor contratado com seus alinhamentos e botões de ação; alinhamento pode ser enviado ao cronograma. |
| Enviar mensagem para fornecedores contratados | Assunto da mensagem, Mensagem com parâmetros `#fornecedor_nome` e `#instagram_fornecedores`, "Modelos de mensagem", Anexos, lista "Fornecedores que receberão a mensagem" com busca e marcação ("2 selecionados de 2", nome e e-mail). Rodapé: "No final da mensagem será inserido automaticamente as seguintes informações do evento: nome, data, horário, local da recepção, local da cerimônia, número de convidados estimado e tempo de festa". |

Dados gravados no teste: serviço personalizado "Serviço Teste Lucão" criado na conta e adicionado ao evento.

## 2. Convidados do evento

Rota base `/gestao_eventos/invitationManagement/{id}`. Sub-rotas: `invitations-and-guests`, `rsvp-and-messages`, `mapping`, `checklist-presence`. Evento de teste: id `394274`.

### 2.1. Convites e convidados

Rota `.../invitations-and-guests`.

Visões: "Visualização por convite" e "Visualização por convidado". Busca: "Pesquise por nome do convite". Botão Filtro.

Filtros:

| Campo | Opções |
|---|---|
| Situação dos convidados | Pendentes, Confirmados, Não comparecerão. Placeholder "Todas respostas". |
| Possui telefone? | Com Telefone, Sem Telefone. |
| Nº de telefone | Texto, "Digite o número do telefone". |
| Possui transfer? | Sim, Não. |
| Transfer | Lista dos locais cadastrados no evento. Sem local: "Nenhuma opção correspondente cadastrada". |
| Grupo | Múltipla. Grupos da conta (padrão: Amigos, Amigos da Noiva, Amigos do Noivo, Família da Noiva, Família do Noivo) mais os criados. |
| Possui observação? | Sim, Não. |

Botões Aplicar filtros e Limpar Filtros.

Menu Adicionar convites: Novo convite, Importar planilha. Menu Imprimir: Por convites (`/printEventInvites/{uuid}/2`), Por convidados (`/printEventGuests/{uuid}/2`). Outras ações: Copiar link único de RSVP, Compartilhar lista de convites, Compartilhar lista de convidados, Gerenciar grupos, Exportar convites, Excluir todos os convites. Botão Transfer abre a seção 2.2.

Tabela por convite: nome, celular, grupo, contadores Pendentes / Confirmados / Não comparecerão. Menu do convite: Adicionar convidado, Copiar link de RSVP, Remover convite.

Rodapé de totais, para cada situação (pendentes, confirmados, não comparecerão) e no geral: Inteira, Meia, Gratuito, Adulto, Idoso, Adolescente, Criança, Criança de colo, e os três gêneros.

### 2.2. Transfer

Modal "Gerenciar transfer". Texto de ajuda descreve transporte até os locais do evento. Interruptor "Ativar o transfer nesse evento?". Desligado, só esse interruptor aparece. Ligado: lista de locais, botão "Adicionar novo local de transfer", painel do local selecionado.

Cadastro do local: um único campo texto (sem rótulo) e Salvar. O local mostra "Total: N convites, N convidados" e ações Editar transfer, Remover transfer, Exportar em PDF, Exportar em Excel.

No convite, com transfer ativado, aparece "Utilizará o serviço de transfer?" (Sim / Não). Sim abre "Local de origem:" (lista dos locais do evento). Sem transfer no evento, esses campos não existem.

Dado gravado: local "Hotel Teste Lucao"; convite Família Teste Lucão marcado Sim.

### 2.3. Grupos

Modal "Gerenciar grupos". Lista os grupos da conta. "Adicionar novo grupo" abre "Adicionar grupo" com "Nome do grupo:" (texto) e Salvar. O grupo fica na conta, não só no evento. Dado gravado: "Grupo Teste Lucao".

### 2.4. Importar planilha

Modal "Importar convites e convidados". Baixar planilha padrão: `/convite_excel/convites.xlsx`, título interno "PLANILHA DE CONVIDADOS (2.0)". Importar aceita `.xlsx` e `.xls`. Vídeo "Como preencher a planilha de importação".

Colunas da planilha, nesta ordem:

| Coluna | Obrigatório | Valores de exemplo na planilha |
|---|---|---|
| Nome do convite * | Sim | Linhas seguintes do mesmo convite deixam em branco. |
| DDI | Não | |
| DDD + Telefone para confirmação de presença | Não | `48 99999-9999` |
| Grupo do convite | Não | Amigos, Família da noiva, Amigos da Noiva, Amigos do Noivo, Família do Noivo, Família da Noiva |
| Observação do convite | Não | |
| Nome dos convidados * | Sim | Uma linha por pessoa |
| Gênero | Não | `M`, `F` (a tela também tem Não binário; a planilha não traz essa opção) |
| Faixa etária | Não | Adulto, Criança, Criança de colo, Idoso (a tela também lista Adolescente) |
| Custo/pagamento | Não | Inteira, Meia, Gratuita, Free |
| Situação | Não | Pendente, Confirmado, Não comparecerá |
| Mesa | Não | Número |

RG, CPF e transfer não entram na planilha.

### 2.5. Formulário do convite

Título "Adicionar convite" ou "Editar convite". Abas na edição: Dados do convite, Histórico de mensagens, QR Code. Aviso: "Nenhum convidado cadastrado. Informe os dados de pelo menos um convidado para este convite."

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Nome do convite | Texto, placeholder "Ex: Joana e Família" | Sim | |
| DDI | Lista, padrão `+55` | | |
| Celular com DDD | Texto, "Número de celular" | Não | |
| A qual grupo pertence | Lista | Não | |
| Utilizará o serviço de transfer? | Sim / Não | Só existe com transfer ativo | |
| Local de origem | Lista | Só aparece se Sim | |
| Observação | Texto longo | Não | |

Botão "Adicionar convidados" abre o formulário da seção 2.6. Na edição, a lista de convidados do convite já aparece (nome, faixa, gênero).

Aba Histórico de mensagens: vazia no convite de teste (nenhum RSVP enviado).

Aba QR Code: "QR Code do convite para apresentar na recepção do evento." Botões Baixar QR Code e Enviar para WhatsApp. Enviar abre "Enviar via" / "Enviar por WhatsApp": DDI, Celular com DDD, Mensagem, Cancelar, Enviar.

### 2.6. Formulário do convidado

Título "Adicionar convidado".

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Nome do convidado | Texto, "Ex: Joana" | Sim | |
| RSVP | Três ícones | | Pendente, Confirmado, Cancelado (o ícone de recusa; totais e filtros usam "Não comparecerão") |
| Mesa | Busca, "Digite ou selecione a mesa" | Não | Aceita mesa nova |
| Gênero | Botões | Não | Masculino, Feminino, Não binário |
| Faixa etária | Lista | Não | Totais da tela: Adulto, Idoso, Adolescente, Criança, Criança de colo |
| Pagamento/custo por convidado | Botões | Não | Inteira, Meia, Gratuita |
| RG | Texto | Não | |
| CPF | Texto | Não | |

Não há celular no convidado: o telefone é do convite. Dado já existente no evento: "Convidado Teste Um", Adulto, Masculino, no convite Família Teste Lucão, celular (31) 99999-0003, grupo Amigos.

### 2.7. RSVP e mensagens

Rota `.../rsvp-and-messages`. Menus laterais: Disparos automáticos (Mensagens de RSVP, `/activate-automatic-rsvp`), Disparos manuais (Envio Personalizado, `/rsvp-personalized`, botão Adicionar mensagem), Ajustes (Configurações, `/rsvp-configuration`).

Envio personalizado: "Envie o link de confirmação de presença para os seus convites." Botões Editar mensagem padrão, Filtro, Copiar link único de RSVP. Contadores Total, Envio pendente, Enviado. Tabela Convite, Contato, Situação, Status, Ações (Copiar link de RSVP).

Configurações das mensagens automáticas:

| Campo | Tipo | Padrão visto |
|---|---|---|
| Data limite para confirmação / Dias antes do evento | Número | 0 |
| Deseja personalizar o seu link de RSVP? | Botão "Personalizar link de RSVP" | Abre modal (abaixo) |
| Terá QR Code na recepção? | Sim / Não | Sim |
| Integração com funil de vendas | Sim / Não | Sim. "Uma pergunta será feita durante o fluxo de confirmação de presença, e os leads coletados irão direto para o seu funil de vendas." |
| O que deseja mostrar? (aumento de visibilidade) | Página exclusiva Vitrine Casar.com; Carta de apresentação; Nenhuma das opções | Vitrine marcada. Fora do produto, RF-079, salvo Carta de apresentação |
| Qual número de contato deseja utilizar? | Número cadastrado na minha conta; Outro número de contato | Conta. Outro abre DDI e DDD + Celular |

Modal "Configuração da página de confirmação" (Personalizar link de RSVP):

| Campo | Observação |
|---|---|
| Fonte do nome do evento | Lista; valor visto Poppins |
| Formatação da data | Lista; valor visto "20 de junho de 2027" |
| Cor da página | Seletor de cor |
| Informações de contato da Assessoria | Marcação: "Deseja que suas informações de contato sejam mostradas aos convidados na página de confirmação de presença?" |
| Pré-visualização | Nome, data por extenso, contagem regressiva (dias, horas, minutos, segundos), Horário, Cerimônia, Recepção, Site, nome da assessoria, aviso de que o formulário de confirmação fica abaixo |

Modal "Configurações da Carta de Apresentação": Selecione um modelo de carta; Usar foto do perfil / Inserir foto; Mensagem da carta de apresentação (textarea); Link (URL); Descrição do botão. Pré-visualização com texto padrão da conta.

A tela de disparos automáticos e o formulário público de RSVP ainda não foram preenchidos nesta sessão. O formulário público continua o maior buraco para copiar o RSVP.

### 2.8. Mapeamento de mesa

Rota `.../mapping`. Visões: Visualização em blocos (há outra no menu). Busca "Busque por mesa, nome do convite ou convidado". Filtro. Totais: convidados por faixa, "N mesas cadastradas", lugares, livres, ocupados, convidados sem lugar.

Adicionar nova mesa:

| Campo | Tipo | Obrigatório |
|---|---|---|
| Nome ou número da mesa | Texto | Sim |
| Quantidade de lugares | Número | Não |
| Observação | Texto, 1000 caracteres, placeholder sobre acessibilidade e andar | Não |
| Convidados | Botão "Adicionar convidado" | Não |

Seletor de convidados: "Selecionar convidados à mesa: N/lugares". Filtros "Mostrar apenas convidados sem mesa" e "Mostrar apenas convidados confirmados" (este último vem ligado; com ele ligado, convidado pendente não aparece). Lista por convite (nome, grupo, N/total).

Adicionar várias mesas: botão visto; o formulário não chegou a abrir nesta sessão.

### 2.9. Checklist de presença

Rota `.../checklist-presence`. Visualização por convite, Filtro, Ler QR Code, Acesso offline. Tabela Convite, Grupo, Ações. Totais de RSVP e "Status de presença": pendentes, presentes, não compareceram. Link Ver relatório. O check-in de um convidado e o modo offline não foram executados.

## 3. Pagamentos do evento

Rota `/gestao_eventos/payment/{id}`. Visões: por mês e por fornecedor. Imprimir custos. Filtros: De, Até, Pago por ("Todos os pagamentos"), Status de pagamento (Pendente estava pré-selecionado), Forma de pagamento ("Todos as formas de pagamentos"). Cada parcela na lista: situação, vencimento, fornecedor, serviço, valor. Agrupamento por mês com "Pago: R$ X de R$ Y" e "Juros incluso". O cadastro da parcela (status Pago/Pendente, datas, juros, pagador, forma, observação, comprovante) está na seção 1.12.

## 4. Modelos de evento

Rota `/modelo_eventos`. Busca "Burcar modelo...". Botão Criar modelo. Link VIP Store (fora, RF-079). Lista vista: Checklist 15 Anos, Checklist de Casamento, Checklist de Aniversário, Checklist Corporativo, Checklist de Formatura, e o modelo da conta "Cronograma de Tarefas Casamento - Assessoria DN". Cada card mostra o tipo.

Criar modelo: Nome (placeholder "Ex: Casamento na praia, 15 anos completo, Casamento básico, etc..") e Tipo. Salvar. As opções da lista Tipo não abriram nesta sessão; os tipos dos cards são 15 Anos, Casamento, Aniversário, Corporativo, Formatura.

Editor do modelo (`/modelo_eventos/view/{id}`): Clonar modelo. Abas Checklist, Tarefas, Briefings, Roteiros, Inspirações, RSVP e Mensagens, Permissões. Aba Checklist: "Adicionar serviços"; cada serviço tem Gerenciar tarefas, Adicionar fornecedores, Adicionar alinhamentos, Adicionar comparativos e valor estimado (R$ 0,00 no modelo DN). O miolo de Tarefas (prazo relativo), Briefings, Roteiros, Inspirações, RSVP e Permissões do modelo ainda não foi aberto.

## 5. Cliente da conta (`/clientes`)

Lista: busca "Pesquisar cliente", Adicionar cliente. Cartão: nome, telefone, e-mail, eventos vinculados (link para o evento), último acesso, lixeira. O formulário do cliente não escolhe evento.

Modal Adicionar / Editar cliente, três abas. Foto: Inserir, Alterar, Remover.

Aba Dados pessoais:

| Campo | Obrigatório | Observação |
|---|---|---|
| Nome completo | Sim | |
| Esse(a) cliente é | Sim | Anfitrião, Aniversariante, Avó, Avô, Comissão, Formando(a), Mãe, Noiva, Noivo, Pai, Promotor, Representante, Tia, Tio |
| Data de nascimento | Não | Máscara `__/__/____` |
| E-mail | Sim | |
| DDI | Sim | Padrão `+55` |
| Celular com DDD | Sim | "Ex: 48 99999-9999" |
| Telefone fixo | Não | A máscara se comportou como celular |
| CPF | Não | `___.___.___-__` |
| RG | Não | |
| CNPJ | Não | `__.___.___/____-__` |
| Profissão | Não | |
| Inscrição municipal | Não | |
| Religião | Não | |
| Observação | Não | Texto longo |

Aba Redes sociais: Facebook, Instagram.

Aba Endereço: País (busca, Brasil), CEP `_____-___` (preenche endereço, estado, cidade, bairro), Endereço, Número, Estado (busca), Cidade (busca), Bairro, Complemento.

Vínculo com evento: seção 1.7. Cadastro pelo evento dispara e-mail de acesso; por isso o teste não gravou cliente real. Para completar falta um e-mail que o usuário puder receber.

## 6. Ainda não percorrido nesta sessão

Roteiro, cronograma, briefing, inspirações e anexos do evento. Tarefas do evento (formulário completo com checklist, perguntas, anexos). Disparos automáticos de RSVP e o formulário público. Formulário de várias mesas. Permissões dos clientes. Modelos de briefing, roteiro, cronograma, inspirações e mensagens. Catálogo global `/suppliers` (o cadastro pelo evento está na seção 1.11). Perfil e assistentes além da aba já vista. Agenda, funil (além do dashboard) e financeiro. MeLink ficou de fora de propósito.


