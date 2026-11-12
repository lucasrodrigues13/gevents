# Requisitos e decisões arquiteturais

Documento de referência desta etapa. Aqui ficam a visão do sistema, os requisitos já fechados, a arquitetura e o que ainda precisa de confirmação.

Nenhum código, solution, entidade, migration ou arquivo de infraestrutura faz parte deste documento.

## Como ler o status

| Status | Significado |
|---|---|
| **Definido** | Decisão confirmada. A implementação deve seguir este texto. |
| **Proposto** | Recomendação técnica. Ainda não vale como regra do projeto. |
| **Pendente** | Pergunta em aberto. Não escolher um caminho na implementação antes de fechar. |

## 1. Visão geral

Aplicação web para gestão de cerimonial e assessoria de eventos, com a interface em Angular e os casos de uso numa API .NET. A primeira entrega técnica é a base: contas, autenticação, autorização e a estrutura da solution. As funcionalidades de evento, convidado, fornecedor e financeiro entram em specifications posteriores.

A API, o build estático do Angular, o PostgreSQL e os arquivos ficam na mesma VPS, sob Docker Compose, com o Caddy na frente. A primeira versão não usa microsserviço, mensageria, fila de e-mail nem infraestrutura distribuída.

O material de domínio já levantado está em [docs/brief.md](brief.md). Esse brief descreve o produto observado na plataforma de referência e o recorte sugerido de MVP. Ele ainda não foi convertido em requisitos funcionais deste documento. O cliente também ainda não informou as melhorias que pretende pedir.

## 2. Requisitos funcionais

Nos requisitos de conta, o status **Definido** significa **Confirmado** para este produto. Requisitos de domínio usam também **Observado**: comportamento visto na plataforma de referência e ainda não aprovado para o nosso sistema.

### 2.1. Cadastro público

| ID | Requisito | Status |
|---|---|---|
| RF-001 | Qualquer visitante pode criar uma conta com e-mail e senha, sem convite. | Definido |
| RF-002 | O e-mail é obrigatório e único. A comparação usa a normalização do ASP.NET Core Identity. | Definido |
| RF-003 | A senha é obrigatória, tem no mínimo 8 caracteres e segue DA-028. O Identity faz a validação e o hash. | Definido |
| RF-004 | Os dados do cadastro são validados no servidor antes de criar a conta. | Definido |
| RF-005 | O login só é permitido depois da confirmação do e-mail. | Definido |
| RF-006 | O cadastro tem proteção contra tentativas automatizadas, com os recursos nativos do ASP.NET Core, inclusive o limite de RF-017. | Definido |
| RF-007 | Falhas de cadastro são informadas sem expor dados sensíveis nem detalhes internos. | Definido |
| RF-008 | Toda conta criada pelo cadastro público recebe somente a role `User`. | Definido |
| RF-009 | O formulário de cadastro não oferece campo de role. O visitante não escolhe o próprio privilégio. | Definido |
| RF-010 | O cadastro público não atribui a role `Admin`. O primeiro visitante que se cadastra não vira administrador. | Definido |

### 2.2. Login e logout

| ID | Requisito | Status |
|---|---|---|
| RF-011 | O usuário autentica com e-mail e senha pelo ASP.NET Core Identity. | Definido |
| RF-012 | A sessão da API é um JWT de acesso, enviado em `Authorization: Bearer`. A aplicação não usa cookie de autenticação nem opera servidor OAuth2. | Definido |
| RF-013 | O logout faz o cliente deixar de enviar o token. O token já emitido continua válido até expirar. Não há lista de revogação. | Definido |
| RF-014 | Após 5 tentativas inválidas de login, a conta fica bloqueada por 15 minutos pelo lockout do Identity. | Definido |
| RF-015 | As mensagens de erro de login não revelam se o e-mail existe, se a senha está errada ou se a conta está bloqueada de forma que ajude um ataque. | Definido |
| RF-016 | A proteção contra força bruta usa os recursos nativos do Identity e do ASP.NET Core. A aplicação não implementa hash nem autenticador próprios. O lockout por conta convive com o rate limiting por IP. | Definido |
| RF-017 | Cadastro, login, recuperação de senha e reenvio de confirmação passam pelo rate limiting nativo, com os limites de DA-030. | Definido |

### 2.3. Recuperação de senha e e-mail

| ID | Requisito | Status |
|---|---|---|
| RF-018 | O usuário solicita a recuperação informando o e-mail. | Definido |
| RF-019 | O token de redefinição é gerado pelo ASP.NET Core Identity. A aplicação não gera token próprio. | Definido |
| RF-020 | O link de redefinição de senha vale 1 hora. | Definido |
| RF-021 | A senha só é trocada depois que o token é validado pelo Identity. | Definido |
| RF-022 | A resposta da solicitação é a mesma exista ou não uma conta para aquele e-mail. Falha de SMTP não muda essa resposta. | Definido |
| RF-023 | O token deixa de valer depois do uso bem-sucedido, conforme o comportamento do Identity. | Definido |
| RF-036 | Confirmação de cadastro e recuperação de senha são enviadas pelo serviço de e-mail. | Definido |
| RF-037 | O usuário pode pedir o reenvio do e-mail de confirmação. | Definido |
| RF-038 | Falha de envio não grava senha nem token em log e não marca o e-mail como confirmado. No cadastro, a tela informa que o envio falhou. Na recuperação, vale RF-022. | Definido |
| RF-041 | O link de confirmação de e-mail vale 24 horas. Token expirado ou inválido é rejeitado. O usuário pede outro pelo reenvio, dentro do limite de RF-017. | Definido |
| RF-042 | Upload rejeitado recebe mensagem clara, sem detalhe interno de disco, caminho ou exceção. | Definido |
| RF-043 | Arquivo privado só é entregue pela aplicação, depois de autenticação e da autorização daquele arquivo. O caminho físico não aparece na resposta. | Definido |

### 2.4. Autorização

| ID | Requisito | Status |
|---|---|---|
| RF-024 | Existem duas roles iniciais: `Admin` e `User`. | Definido |
| RF-025 | Página privada exige usuário autenticado. | Definido |
| RF-026 | Página administrativa exige a role `Admin`. | Definido |
| RF-027 | Usuário com role `User` não acessa recurso administrativo. | Definido |
| RF-028 | A autorização é verificada no servidor com `[Authorize]`, roles e policies do ASP.NET Core. Esconder um link na interface não protege a página. | Definido |
| RF-040 | A role `User` identifica a conta comum. Ela não representa, por si só, assessoria, casal, fornecedor ou outra entidade de negócio. Nenhuma role nova será criada até um requisito funcional justificar. | Definido |

### 2.5. Bootstrap do administrador inicial

| ID | Requisito | Status |
|---|---|---|
| RF-029 | Na inicialização, quando o bootstrap está habilitado, a aplicação garante as roles `Admin` e `User`, cria o administrador inicial se ele ainda não existe e atribui a role `Admin`. | Definido |
| RF-030 | A operação é idempotente: não duplica role nem usuário e não altera a senha de um usuário que já existe. | Definido |
| RF-031 | Se já existir um usuário comum com o e-mail configurado para o administrador, o bootstrap interrompe a operação e não promove esse usuário. | Definido |
| RF-032 | O administrador criado pelo bootstrap consegue entrar sem passar pelo fluxo público de confirmação de e-mail. A confirmação é registrada de forma segura durante o bootstrap. | Definido |
| RF-033 | O bootstrap fica desligado por padrão. Só roda quando `Bootstrap__Enabled` está habilitado. Credencial ausente ou inválida faz a inicialização falhar, sem gravar senha ou segredo em log. | Definido |
| RF-034 | Não existe endpoint público para disparar o bootstrap. Ele não usa `HasData()` do EF Core, senha fixa no código nem seed genérico de usuários. | Definido |

O comportamento detalhado está na seção 8.4.

### 2.6. Domínio de negócio

| ID | Requisito | Status |
|---|---|---|
| RF-035 | O comportamento aprovado para implementação é o das specs 001 a 010. O restante desta seção continua observado e fica de fora até existir spec própria. | Pendente |

A fonte é [docs/brief.md](brief.md), leitura da plataforma de referência, sem nova exploração. O corte aprovado está em [docs/mvp-scope.md](mvp-scope.md). Os RF abaixo continuam observados. A spec aprovada é que limita o que entra no código.

### 2.7. Requisitos observados na referência

Nada desta seção está confirmado. Regras que não foram vistas não foram completadas.

| ID | Nome | Comportamento observado | Ator | Regras vistas | Depende de | Status |
|---|---|---|---|---|---|---|
| RF-044 | Perfil da assessoria | Cadastro da empresa com nome, responsável, e-mail, CPF ou CNPJ, celular, nascimento e endereço. | Assessoria | E-mail da conta não é editável na tela vista. | RF-011 | Observado |
| RF-045 | Assistentes | A assessoria cadastra assistentes e limita módulos. | Assessoria, assistente | Cada permissão é independente. A referência tem mais papéis do que `Admin` e `User`. | RF-040, DP-016 | Observado |
| RF-046 | Busca global | Campo para buscar eventos ou ferramentas. | Assessoria | Não foi medido o que a busca devolve. | RF-048 | Observado |
| RF-047 | Notificações | Sino com lista e ação de marcar como lidas. | Assessoria | O conteúdo de cada aviso não foi aberto. | — | Observado |
| RF-048 | Lista de eventos | Abas em andamento, concluído e cancelado. Busca por nome. Filtros de tipo, equipe, assistente, período, modelo e tags. Card com data, contagem, modelo, tarefas e convidados. Ações de editar, cancelar, concluir, remover e imprimir a lista. | Assessoria | Tags vistas: assessoria completa, parcial e final. São dados da conta, não uma lista fechada. Atalhos no topo: pagamentos, tarefas da semana, anexos, equipe escalada, aniversários. Inventário em [inventario-referencia.md](inventario-referencia.md) §1.1. | RF-049 | Observado |
| RF-049 | Cadastro do evento | Cria evento com imagem, modelo, casal, nome, data, hora, orçamento estimado, número estimado de convidados e locais de cerimônia e recepção. | Assessoria | Na referência o modelo é obrigatório e define o tipo. Depois do salvar pede valor da assessoria (total, parcelas, entrada). Edição tem quatro abas, com horário de convite, tempo de festa, horários de cerimônia e recepção, padrinhos, damas e endereço completo de cada local. Campos em [inventario-referencia.md](inventario-referencia.md) §1.3 e §1.5. | RF-053 | Observado |
| RF-050 | Home do evento | Contagem regressiva, código, atalhos de budget, pagamentos, tarefas, convidados, serviços por categoria e pendências do mês. | Assessoria | Também mostra equipe do dia, gerenciar clientes, planejamento de budget, observações gerais, lista de serviços do modelo, compromissos e despesas. Código visto no formato `12729CXQT`. | RF-048, RF-058, RF-061, RF-063, RF-068 | Observado |
| RF-051 | Equipe do dia | Ação para escalar quem trabalha no evento. | Assessoria | Modal com Minha equipe e Mural de recados. Membro tem Dados, Endereço, Dados bancários e Compromissos. Escalar pergunta se notifica por e-mail. Conflito de agenda aparece em vermelho. Campos em [inventario-referencia.md](inventario-referencia.md) §1.8. | RF-045, RF-050 | Observado |
| RF-052 | Agrupar eventos | Há rotas de eventos agrupados. | Assessoria | O fluxo não foi aberto. | RF-048 | Pendente |
| RF-053 | Modelo de evento | Biblioteca de modelos. Tipos vistos: casamento, 15 anos, aniversário, corporativo e formatura. Aplicar o modelo na criação do evento. | Assessoria | Criar pede Nome e Tipo. O editor tem abas Checklist, Tarefas, Briefings, Roteiros, Inspirações, RSVP e Mensagens, Permissões. Checklist lista serviços com tarefas, fornecedores, alinhamentos, comparativos e valor estimado. Miolo das outras abas ainda não aberto. | — | Observado |
| RF-054 | Outros modelos | Bibliotecas de briefing, roteiro, cronograma, inspiração e mensagem. | Assessoria | Cada biblioteca foi vista no menu, não no editor. | RF-069, RF-070, RF-071, RF-072, RF-065 | Observado |
| RF-055 | Clientes da assessoria | Lista com busca. Cartão com nome, telefone, e-mail, eventos vinculados e último acesso. | Assessoria | Formulário em três abas. Obrigatórios: nome, papel, e-mail, DDI, celular. Papéis vistos: Anfitrião, Aniversariante, Avó, Avô, Comissão, Formando(a), Mãe, Noiva, Noivo, Pai, Promotor, Representante, Tia, Tio. Endereço dividido. Vínculo nasce no evento, não neste formulário. Campos em [inventario-referencia.md](inventario-referencia.md) §5. | RF-049 | Observado |
| RF-056 | Acesso do cliente | O cliente entra só no próprio evento. Há flags de permissão para convidados, tarefas, anexos, parcelas, RSVP, mapa e contrato. | Cliente, assessoria | As flags vieram do frontend. O portal do cliente não foi usado. | RF-045, RF-055 | Observado |
| RF-057 | Catálogo de fornecedores | Busca, filtro, exportação, mescla e status completo, incompleto ou dados solicitados. Há abas de catálogo, solicitação de dados e lista de serviços. | Assessoria | Formulário com oito abas: Dados, Redes sociais, Endereço, Dados bancários, Orçamentos, Diferencial, Inf. gerais, Portfólio. Nome e serviços obrigatórios. Telefones e e-mails são listas. Campos em [inventario-referencia.md](inventario-referencia.md) §1.11. | — | Observado |
| RF-058 | Serviços e budget | No evento, cada serviço tem fornecedor, tarefas e valor. A home compara estimado e contratado. | Assessoria | A lista de categorias vista é a de um modelo de casamento, não um catálogo fixo do produto. | RF-049, RF-057, RF-061 | Observado |
| RF-059 | Negociação | Estágios vistos: recomendado, orçamento solicitado, orçamento recebido, contratado, contrato solicitado, contrato recebido. | Assessoria | No hub do evento a trilha tem quatro etapas empilhadas, com seta para voltar uma: Recomendado, Orçamento solicitado, Orçamento recebido, Contratado. Solicitar contrato é ação do contratado, não uma quinta coluna na trilha. | RF-058 | Observado |
| RF-060 | Link do fornecedor | Há rotas para o fornecedor completar dados, responder orçamento e enviar contrato sem a conta da assessoria. | Fornecedor | Os formulários públicos não foram preenchidos. | RF-057, RF-059 | Observado |
| RF-061 | Tarefas | Situações pendente, em andamento, concluída e atrasada. Visões por situação, responsável, prazo e serviço. Lista, quadro por situação e quadro por dia. Prazo também relativo ao evento. Tarefa com descrição, checklist, perguntas, comentários e anexos. Há lista de todos os eventos e destaque da semana. | Assessoria, cliente | Cliente só age se a permissão existir. O conjunto exato de campos obrigatórios não foi fechado. | RF-049, RF-058 | Observado |
| RF-062 | Redatar tarefas | Ao mudar a data do evento, os prazos das tarefas podem ser recalculados. | Assessoria | Visto como aviso no produto, não executado. | RF-049, RF-061 | Observado |
| RF-063 | Convites e convidados | Convite agrupa convidados. Grupos configuráveis. Situação: pendente, confirmado ou não comparecerá. Faixa etária, custo, gênero, celular, observação e transfer. Visão por convite ou por convidado. Busca, filtro, criação, importação, exportação e impressão. | Assessoria | Convite: nome obrigatório, DDI, celular, grupo, observação, transfer e origem. Convidado: nome obrigatório, RSVP, mesa, gênero, faixa, custo, RG, CPF. Telefone fica no convite. Planilha padrão em 11 colunas. Campos em [inventario-referencia.md](inventario-referencia.md) §2. | RF-049 | Observado |
| RF-064 | RSVP por link | Link por convite e link único do evento. Histórico: enviada, acessada, respondida. | Convidado, assessoria | Há também QR Code do convite, envio por WhatsApp e configuração da página (fonte, data, cor, contato). O formulário público ainda não foi respondido. | RF-063 | Observado |
| RF-065 | RSVP automático | Mensagens e disparos automáticos, com configuração. Mudar a data do evento pode desligar envios. | Assessoria | A configuração não foi concluída. Depende de envio em volume, acima do uso atual do Gmail. | RF-064, RNF-020 | Observado |
| RF-066 | Mapa de mesas | Item de menu, com trava de data. | Assessoria | Cadastro da mesa: nome ou número obrigatório, quantidade de lugares, observação até 1000 caracteres, convidados. Filtro do seletor: sem mesa e só confirmados. | RF-063 | Observado |
| RF-067 | Checklist de presença | Item de menu e relatório com presentes, ausentes, mesa e contagem. Há menção a uso offline. | Assessoria | Tela com Ler QR Code, Acesso offline, totais pendentes, presentes e não compareceram. O check-in de uma pessoa não foi executado. | RF-063 | Observado |
| RF-068 | Parcelas do evento | Parcelas por serviço e fornecedor. Visão por mês ou por fornecedor. Filtros de período, pagador, status e forma de pagamento. Campos de vencimento, valor, juros, número da parcela e pago. Impressão de custos. | Assessoria | Parcela no hub: Status (Pago, Pendente), vencimento, pagamento, valor, juros, pago por, forma (Cartão de crédito, Cartão de débito, Boleto, TED, DOC, Cheque, Dinheiro, Transferência, Pix, Permuta, Outros), observação, comprovantes. | RF-058 | Observado |
| RF-069 | Roteiro | Seções ordenáveis, observação, anexo, clonagem, impressão e permissão de ver ou editar para o cliente. | Assessoria, cliente | Rótulos vistos. O editor não foi percorrido. | RF-049, RF-054 | Observado |
| RF-070 | Cronograma | Data, hora, responsável, recado a fornecedores, salvar como modelo, impressão e compartilhamento. | Assessoria | Rótulos vistos. A grade não foi percorrida. | RF-049, RF-054 | Observado |
| RF-071 | Briefing | Do zero ou de modelo, com seções, perguntas, status de resposta, link público e impressão. | Assessoria, cliente | Rótulos vistos. Um briefing respondido não foi aberto. | RF-054 | Observado |
| RF-072 | Inspirações | Galerias de referência. | Assessoria, cliente | Menu e rótulo de galeria. O miolo não foi aberto. | RF-054 | Observado |
| RF-073 | Anexos | Por fornecedor, serviço, tarefa e observação geral, mais uma caixa de todos os eventos. | Assessoria | As abas foram vistas na home e nas rotas. O fluxo de upload do produto de referência não foi especificado. | RF-049, DA-035 | Observado |
| RF-074 | Agenda | Mês, semana, dia e lista. Evento com data pode gerar compromisso. Confirmação de presença em compromisso por link. Há permissão de ver só os próprios compromissos. | Assessoria, assistente | Compromisso exige data. O formulário do compromisso não foi aberto. | RF-049 | Observado |
| RF-075 | Funil de vendas | Quadro com quantidade e valor. Estágios vistos na conta: contato inicial, orçamento enviado, contratado e descartado. Estágios configuráveis. Descarte com motivo. Filtro, exportação, formulário público de lead, checagem de data, contratos, orçamentos e tarefas de reunião, ligação, e-mail ou tarefa. | Assessoria, lead | Não foi visto se a checagem de data bloqueia o cadastro ou só avisa. | RF-074 | Observado |
| RF-076 | Financeiro da assessoria | Caixa da empresa, separado do budget do evento. Transações de recebimento, despesa e transferência. Contas, categorias, recibo, saldo realizado e previsto, pendências e fluxo de caixa. | Assessoria | A integração automática com a parcela do evento foi vista como configuração, não comprovada num lançamento. | RF-068 | Observado |
| RF-077 | Relatórios e impressões | Catálogos de funil e do financeiro, mais impressões de convidados, custos, roteiro, cronograma, briefing, alinhamento, tarefas, resumo e agenda. | Assessoria | A aba Estatísticas da lista de eventos não entra aqui. Ver RF-078. | Vários, conforme o relatório | Observado |
| RF-078 | Estatísticas de eventos | Existe uma aba com esse nome na lista de eventos. | Assessoria | Total, concluídos, em andamento e cancelados com percentual. Eventos por mês no ano atual. Ranking de serviços no orçamento (eventos concluídos), fornecedores mais contratados e serviços mais utilizados. | RF-048 | Observado |
| RF-079 | Peças da plataforma de referência | VIP Pay, site e vitrine Casar.com, loja de modelos, MeLink, indicação e vídeos explicativos aparecem na referência. | Assessoria | São produtos da plataforma observada, não pedidos do nosso cliente. | — | Observado |

### 2.8. Tela inicial

Decisão deste produto. A referência abre um painel de menus. Aqui a primeira tela autenticada mostra dashboards.

| ID | Requisito | Status |
|---|---|---|
| RF-080 | Depois do login, o `User` cai numa tela de dashboards da própria conta. Essa tela não é uma grade de atalhos dos módulos. O menu lateral leva aos módulos. | Definido |
| RF-081 | O dashboard de eventos mostra a quantidade em andamento, concluída e cancelada, e lista os eventos em andamento com nome e data. | Definido |
| RF-082 | O dashboard do funil mostra quantidade e valor nos estágios já vistos: contato inicial, orçamento enviado, contratado e descartado. Não cria estágio novo. | Definido |

## 3. Requisitos não funcionais

| ID | Requisito | Status |
|---|---|---|
| RNF-001 | Segredos e credenciais ficam fora do controle de versão. Configuração por `appsettings`, variáveis de ambiente, User Secrets em desenvolvimento e arquivo protegido em produção. | Definido |
| RNF-002 | Desenvolvimento e produção usam configurações separadas. | Definido |
| RNF-003 | Os dados de negócio e as contas do Identity persistem no PostgreSQL. O banco só é alcançado pela rede interna do Docker. A porta não fica publicada no host. | Definido |
| RNF-004 | Arquivos enviados ficam no volume `/storage`, fora do build estático do frontend e fora da camada gravável do container. O volume sobrevive à recriação do container. O Caddy não publica esse diretório. | Definido |
| RNF-005 | Erros HTTP são tratados de forma centralizada. A resposta ao usuário não expõe stack trace nem detalhe interno em produção. | Definido |
| RNF-006 | Logging estruturado com `ILogger`, recurso nativo do .NET. Senha, token, senha SMTP, connection string e chave do Data Protection não entram no log. | Definido |
| RNF-007 | Toda entrada relevante é validada no servidor. | Definido |
| RNF-008 | Acesso a banco, disco, SMTP e rede usa operações assíncronas. | Definido |
| RNF-009 | A evolução do schema usa EF Core Migrations. Produção não aplica migration na subida da aplicação. | Definido |
| RNF-010 | A VPS sobe com Docker Compose: API, PostgreSQL e Caddy. O Caddy publica o build estático do Angular. Sem Kubernetes e sem outro orquestrador. | Definido |
| RNF-011 | Senhas de usuário são protegidas exclusivamente pelo ASP.NET Core Identity. | Definido |
| RNF-012 | Endpoints de autenticação têm proteção contra força bruta e abuso, conforme RF-014, RF-016 e RF-017. | Definido |
| RNF-013 | Pedido que altera estado exige o JWT no cabeçalho `Authorization`. Não há cookie de autenticação nem proteção antiforgery de formulário. | Definido |
| RNF-014 | Produção atende somente por HTTPS. O Caddy redireciona HTTP para HTTPS e renova o certificado. | Definido |
| RNF-015 | As chaves do ASP.NET Core Data Protection ficam em volume persistente da VPS, com acesso restrito, e sobrevivem à recriação do container. | Definido |
| RNF-016 | O JWT de acesso dura 8 horas e não se renova sozinho. Não há sliding expiration, refresh token nem lista de revogação. O cliente guarda o token em `sessionStorage`. | Definido |
| RNF-017 | A criação do administrador inicial é idempotente e não registra segredo em log, conforme RF-030 e RF-033. | Definido |
| RNF-018 | Esta versão não implementa backup, ferramenta nem rotina de cópia. Backup e restauração passam a ser obrigatórios antes de dados reais em produção. Até lá, perda de disco significa perda do banco e dos arquivos. | Definido |
| RNF-019 | Testes unitários rodam em todo pull request. Testes de integração rodam no pipeline de validação, com PostgreSQL descartável, antes da aprovação do deploy. Falha em teste obrigatório interrompe o pipeline. Não há meta percentual de cobertura nem teste de navegador nesta versão. | Definido |
| RNF-020 | O e-mail transacional de confirmação e de recuperação sai por SMTP do Gmail, com MailKit, na Infrastructure. | Definido |
| RNF-021 | Ao exceder o rate limit, a resposta é HTTP 429. Quando o middleware puder, a resposta inclui `Retry-After`. | Definido |
| RNF-022 | O rate limiting desta versão é em memória e vale para uma instância. Os limites ficam no `appsettings.json` e mudam sem recompilar. Não há Redis nem outro armazenamento distribuído de contagem. | Definido |
| RNF-023 | O IP usado no rate limiting é o do cliente, lido atrás do Caddy. A aplicação só confia em proxies configurados de forma explícita. | Definido |
| RNF-024 | Migration de produção roda por comando explícito no deploy, depois de autorização. Havendo migration pendente sem essa autorização, o deploy para. | Definido |
| RNF-025 | Volumes persistentes guardam os dados do PostgreSQL, os arquivos enviados e as chaves do Data Protection. | Definido |
| RNF-026 | O nome físico do arquivo é gerado no servidor. O nome enviado pelo usuário não entra no caminho. Caminho informado pelo cliente é recusado, o que impede path traversal. | Definido |
| RNF-027 | O binário não entra no PostgreSQL. O banco guarda identificador, nome original sanitizado, nome físico, caminho relativo, MIME validado, tamanho, data de criação e o usuário do upload, quando houver. | Definido |
| RNF-028 | Tamanho, quantidade, total da requisição e formatos de upload são validados no servidor e permanecem configuráveis sem recompilar. O mesmo teto de corpo vale no Caddy e no ASP.NET Core. | Definido |
| RNF-029 | Arquivo vazio, extensão fora da lista, assinatura incompatível com o formato e requisição acima do limite são recusados. A aplicação não confia só no Content-Type do navegador. | Definido |
| RNF-030 | Disco e PostgreSQL não formam uma transação única. Falha depois da gravação remove o arquivo daquela tentativa no mesmo fluxo e registra o erro. Não há job de limpeza nesta versão. | Definido |

Não há meta numérica de disponibilidade, throughput ou latência nesta versão.

Não há cookie de autenticação. DA-019 registra isso.

## 4. Decisões arquiteturais

| ID | Decisão | Status |
|---|---|---|
| DA-001 | A aplicação é um produto com dois processos: a API e o build estático do Angular. Não é microsserviço. A interface não roda dentro do processo da API. | Definido |
| DA-002 | A arquitetura é Clean Architecture simplificada, com quatro projetos .NET de produção. O frontend Angular fica em `frontend/` e não entra na solution. Os testes ficam em `tests/`, separados em unitário e integração. | Definido |
| DA-003 | A interface é Angular em `frontend/`. A API é ASP.NET Core em `Project.Api`, com rotas em `/api/v1`. Erro HTTP vai em JSON, com código e mensagem, sem stack trace em produção. Swagger é configurável e fica ligado em desenvolvimento. CORS só libera a origem local do `ng serve`. Em produção a origem é a do Caddy. Não há Razor Pages. | Definido |
| DA-004 | .NET 10 LTS, C#, ASP.NET Core. | Definido |
| DA-005 | PostgreSQL, Entity Framework Core, Code First e Migrations. O provider é Npgsql. | Definido |
| DA-006 | Autenticação com ASP.NET Core Identity para senha, normalização, lockout, roles e tokens de e-mail. A sessão da API é um JWT de acesso. Sem cookie de autenticação, sem refresh token, sem lista de revogação, sem servidor OAuth2 próprio e sem provedor externo de identidade. | Definido |
| DA-007 | Autorização com o sistema nativo do ASP.NET Core, baseada nas roles `Admin` e `User`. | Definido |
| DA-008 | O Domain não referencia Identity, EF Core nem ASP.NET Core. Nenhuma entidade de domínio herda de `IdentityUser`. | Definido |
| DA-009 | `ApplicationUser`, se um campo extra for realmente necessário, fica na Infrastructure. Enquanto o `IdentityUser` padrão atender, essa classe não será criada. | Definido |
| DA-010 | Login e as operações que o Identity já resolve usam `UserManager` e `SignInManager.CheckPasswordSignIn` nos endpoints de conta da Api. Não haverá serviço só para encapsular essas APIs. A assinatura do JWT fica na Infrastructure, atrás de interface na Application. O logout não invalida o token no servidor. | Definido |
| DA-011 | Usuários e roles não ganham repositório próprio. `UserManager` e `RoleManager` são a API de persistência dessas contas. | Definido |
| DA-012 | Não haverá Generic Repository nem Unit of Work adicional. O `DbContext` já cumpre esses papéis. Repositório específico de um agregado só nasce quando isolar a consulta ou a escrita trouxer clareza real. | Definido |
| DA-013 | Casos de uso ficam na Application, organizados por funcionalidade. Endpoints delegam a regra de negócio para essa camada. Endpoint de negócio não usa `DbContext`. | Definido |
| DA-014 | Não entram CQRS, MediatR, AutoMapper nem biblioteca de abstração sem um problema concreto que o recurso nativo não resolva. As exceções já registradas são MailKit (DA-021) e Testcontainers (DA-042). | Definido |
| DA-015 | Injeção de dependência, logging, Options e `IConfiguration` usam os recursos nativos do ASP.NET Core. | Definido |
| DA-016 | O bootstrap do administrador é um serviço específico da Infrastructure, chamado de forma explícita na inicialização, usando `UserManager` e `RoleManager`. | Definido |
| DA-017 | Arquivos da primeira versão ficam no disco da mesma VPS, em volume Docker. Sem S3, Azure Blob, MinIO ou outro armazenamento externo nesta versão. | Definido |
| DA-018 | A primeira versão não usa microsserviço, barramento de mensagens, fila de e-mail nem cache distribuído. | Definido |
| DA-019 | Não há cookie de autenticação, então `SameSite` não se aplica. O cliente guarda o JWT em `sessionStorage` e envia `Authorization: Bearer`. | Definido |
| DA-020 | Lockout do Identity: 5 falhas e bloqueio de 15 minutos. | Definido |
| DA-021 | E-mail transacional por SMTP do Gmail, biblioteca MailKit, implementação na Infrastructure, registrada no contrato de e-mail do Identity. Os casos de uso não conhecem o Gmail. | Definido |
| DA-022 | O envio de e-mail acontece na requisição. Esta versão não cria fila nem worker para isso. | Definido |
| DA-023 | Deploy com Docker Compose na VPS única. Serviços: API ASP.NET Core, PostgreSQL e Caddy. O Caddy publica o build estático do Angular e encaminha `/api` para a API. Sem processo Node, sem SSR e sem service worker. | Definido |
| DA-024 | O Caddy é o reverse proxy, emite e renova o HTTPS e redireciona HTTP para HTTPS. | Definido |
| DA-025 | O PostgreSQL permanece na rede interna do Compose. A porta do banco não é publicada. | Definido |
| DA-026 | Volumes persistentes para dados do PostgreSQL, arquivos enviados e chaves do Data Protection. | Definido |
| DA-027 | Dado não sensível no `appsettings.json`. Segredo em variável de ambiente, User Secrets no desenvolvimento ou arquivo protegido fora do repositório. | Definido |
| DA-028 | Senha com mínimo de 8 caracteres. A política não exige número, maiúscula nem caractere especial. Hash e validação são os do Identity. | Definido |
| DA-029 | JWT de acesso com duração de 8 horas, sem renovação deslizante e sem refresh token. Segredo, issuer, audience e prazo vêm de configuração. | Definido |
| DA-030 | Rate limiting nativo, em memória, por IP, com os limites iniciais da seção 8.6. Resposta 429 e `Retry-After` quando o middleware informar a espera. | Definido |
| DA-031 | Token de recuperação de senha com validade de 1 hora, gerado e validado pelo Identity. | Definido |
| DA-032 | Migration de produção é explícita no deploy, de preferência por script SQL idempotente do EF Core. A aplicação não chama `Database.Migrate()` na inicialização nem ao subir o container. | Definido |
| DA-033 | Backup fica fora desta versão. Vira obrigação antes de dados reais em produção. Nenhuma ferramenta de backup entra agora. | Definido |
| DA-034 | `User` não é entidade de negócio. Novas roles esperam requisito funcional. | Definido |
| DA-035 | Arquivos ficam em `/storage/{categoria}/{ano}/{mês}/{guid}.{extensão}`, com a raiz configurável. A extensão segue o tipo validado. O nome original só existe no banco, sanitizado. | Definido |
| DA-036 | Metadados do arquivo ficam no PostgreSQL. O conteúdo binário fica só no volume. | Definido |
| DA-037 | Arquivo privado é lido pela aplicação, com autenticação e autorização. O Caddy não serve o volume. A resposta usa o MIME validado, `Content-Disposition: attachment` com o nome sanitizado e `X-Content-Type-Options: nosniff`. | Definido |
| DA-038 | O contrato de armazenamento, quando um caso de uso precisar, fica na Application. A Infrastructure grava e lê com o I/O nativo do .NET. Sem abstração de storage distribuído. | Definido |
| DA-039 | Limites iniciais: imagem 5 MB, documento 10 MB, até 5 arquivos e 50 MB por requisição. Formatos: PDF, JPG/JPEG, PNG e WebP. | Definido |
| DA-040 | Upload usa `IFormFile` e o pipeline nativo do ASP.NET Core. Sem streaming próprio e sem job de limpeza. A gravação evita carregar o arquivo inteiro em um array na memória. | Definido |
| DA-041 | Esta versão não inclui antivírus. O risco de malware fica registrado e precisa ser reavaliado antes de liberar upload de usuário não confiável. | Definido |
| DA-042 | Testes com xUnit. Integração HTTP com `WebApplicationFactory`. PostgreSQL real com Testcontainers e Npgsql. O provider InMemory não valida o comportamento do PostgreSQL. E-mail de teste substitui o Gmail. | Definido |
| DA-043 | Confirmação de e-mail vale 24 horas e recuperação de senha vale 1 hora. Prazos diferentes usam providers de token do próprio Identity, sem criptografia própria. | Definido |
| DA-044 | A interface usa Angular Material em `frontend/`, no tema padrão do schematic, com componentes standalone. A casca autenticada tem barra lateral, barra superior e cartões na área principal. As telas do produto são só as das specs, com textos em português. Formulário, tabela, diálogo e aviso usam o Material. Componente próprio só nasce quando o Material não cobre a tela, e pode se inspirar no visual de outro template sem trazer o código dele. O miolo do dashboard continua em cartões e lista. Gráfico não entra enquanto um RF não pedir a série. | Definido |

## 5. Stack tecnológica

| Tema | Escolha | Status |
|---|---|---|
| Linguagem de servidor | C# | Definido |
| Plataforma | .NET 10 LTS | Definido |
| API | ASP.NET Core Web API em `Project.Api` | Definido |
| Interface | Angular 22 e Angular Material, build estático | Definido |
| Banco | PostgreSQL | Definido |
| Acesso a dados | Entity Framework Core + Npgsql | Definido |
| Schema | Code First e EF Core Migrations | Definido |
| Contas | ASP.NET Core Identity | Definido |
| Sessão | JWT de acesso, 8 horas, sem refresh | Definido |
| Autorização | Roles `Admin` e `User` | Definido |
| E-mail | Gmail SMTP + MailKit | Definido |
| Arquitetura | Clean Architecture simplificada | Definido |
| Execução | API e frontend estático na mesma VPS | Definido |
| Hospedagem | Uma VPS, Docker Compose | Definido |
| Proxy | Caddy | Definido |
| Arquivos | Volume local `/storage` na VPS | Definido |
| Testes | xUnit, WebApplicationFactory, Testcontainers | Definido |
| Fora desta versão | Microsserviço, mensageria, fila, Redis, Kubernetes, refresh token, Razor Pages, pacote AdminLTE, template pago de dashboard, OAuth2 próprio, login social, MFA, SSR, backup automatizado, antivírus, object storage, teste de navegador | Definido |

## 6. Organização dos projetos

```text
frontend/

src/
├── Project.Api/
├── Project.Application/
├── Project.Domain/
└── Project.Infrastructure/

tests/
├── Project.Tests/
└── Project.IntegrationTests/

docs/
├── brief.md
└── requirements.md
```

A solution tem quatro projetos de produção .NET. `frontend/` é o Angular e não entra na solution. Os testes são dois porque a suíte unitária não sobe banco e a de integração sobe PostgreSQL descartável. Novo projeto de produção só com justificativa registrada neste documento.

### 6.1. Project.Domain

Camada central. Não depende de nenhum outro projeto da solution.

Contém entidades, value objects quando houver valor real, enums, invariantes e comportamento de negócio.

Modelo rico quando a regra justificar encapsular comportamento na entidade. CRUD sem regra permanece simples, sem abstração em volta.

Não referencia ASP.NET Core, Entity Framework Core, Identity nem PostgreSQL.

### 6.2. Project.Application

Casos de uso e orquestração. Depende só de Domain.

Contém serviços de aplicação, DTOs de entrada e saída, validação de aplicação e interfaces para recursos externos. Contratos de persistência também ficam aqui, e somente quando um caso de uso precisar deles.

Organização preferencial por funcionalidade.

Contrato ligado à identidade do usuário só aparece quando um caso de uso de negócio precisar consultar quem está autenticado. Operações de login, cadastro e senha não passam por um serviço intermediário nesta camada. A assinatura do JWT é recurso externo: a interface fica aqui e a implementação na Infrastructure.

O envio de confirmação e de recuperação usa o contrato de e-mail do Identity. A Application não referencia MailKit nem a configuração SMTP.

Contrato de armazenamento só nasce quando um caso de uso precisar gravar ou ler arquivo. Esse contrato fala em fluxo e metadados, não em caminho absoluto nem em volume Docker.

Não depende de Infrastructure.

### 6.3. Project.Infrastructure

Implementações de recursos externos. Depende de Application e Domain.

Contém `DbContext`, configuração Fluent API, migrations, acesso ao PostgreSQL, leitura e gravação de arquivos no volume, implementações das interfaces da Application, `IdentityDbContext`, a assinatura do JWT, o envio SMTP com MailKit e o bootstrap do administrador.

`ApplicationUser`, se existir, fica aqui.

Esta camada não contém regra de negócio.

### 6.4. Project.Api

ASP.NET Core Web API. Depende de Application e de Infrastructure.

Infrastructure entra aqui para composição: registro de dependências, Identity, JWT, CORS, rate limiting, DbContext, e-mail e bootstrap. Os endpoints de negócio não usam `DbContext` nem tipos concretos de persistência.

Contém endpoints em `/api/v1`, validação de entrada, cadastro, login, logout, confirmação, reenvio e recuperação de senha, middleware, tratamento de erro HTTP e `Program.cs`.

Endpoints ficam finos. A exceção definida é o uso direto de `UserManager` e `SignInManager.CheckPasswordSignIn` nos endpoints de conta.

### 6.5. Testes

`Project.Tests` cobre regras, invariantes, value objects, validação e serviços de aplicação, sem banco, sem rede e sem SMTP real.

`Project.IntegrationTests` cobre EF Core, constraints, migrations, a API, autenticação, autorização e arquivos, com PostgreSQL do Testcontainers e um diretório temporário isolado.

O detalhe está na seção 10.1.

## 7. Regras de dependência

```text
Project.Domain
    não referencia outros projetos

Project.Application
    → Project.Domain

Project.Infrastructure
    → Project.Application
    → Project.Domain

Project.Api
    → Project.Application
    → Project.Infrastructure
```

A seta é referência de compilação. Application não referencia Infrastructure. `frontend/` não referencia os projetos .NET.

Interfaces de persistência e de serviços externos são declaradas na Application e implementadas na Infrastructure. O registro concreto acontece no `Program.cs` da Api.

O fluxo de uma requisição de negócio pode percorrer Api, Application e Infrastructure. A dependência de código continua apontando para dentro.

Identity segue a mesma regra: persistência, bootstrap, SMTP e assinatura do JWT na Infrastructure, endpoints de conta na Api, Domain sem referência ao Identity.

### 7.1. O que cada endpoint pode usar

| Endpoint | Pode usar | Não usa |
|---|---|---|
| Cadastro, login, logout, confirmação de e-mail, reenvio, recuperação de senha | `UserManager`, `SignInManager`, `RoleManager` quando a operação for do Identity | Serviço de aplicação criado só para repetir essas APIs; cliente SMTP |
| Demais endpoints | Serviços da Application | `DbContext`, repositório concreto, SQL |

## 8. Autenticação, autorização e sessão

### 8.1. Cadastro, login e senha

Coberto por RF-001 a RF-023 e RF-036 a RF-038.

O hash da senha, a normalização do e-mail e a geração dos tokens de confirmação e de redefinição são os do ASP.NET Core Identity. O JWT de acesso é emitido pela Infrastructure. Não há implementação paralela de senha nem de token de e-mail.

Política inicial de senha, DA-028:

- tamanho mínimo de 8 caracteres;
- número não é obrigatório;
- letra maiúscula não é obrigatória;
- caractere especial não é obrigatório.

A exigência de ao menos uma letra minúscula não foi alterada. O padrão do Identity continua ligado nesse ponto. Ver seção 14.

Confirmação de e-mail antes do login é obrigatória para quem veio do cadastro público. O link vale 24 horas, RF-041. A recuperação de senha continua em 1 hora, DA-031. Os dois prazos usam providers de token do Identity, DA-043. O administrador criado pelo bootstrap é a exceção de confirmação, em RF-032.

### 8.2. Roles

| Role | Quem recebe | Acesso |
|---|---|---|
| `User` | Todo cadastro público | Páginas autenticadas comuns. Não é um papel de negócio. |
| `Admin` | Somente o bootstrap, ou um procedimento administrativo futuro explícito | Páginas administrativas |

Não há tela em que o próprio usuário eleve o próprio privilégio.

Assistente, casal, fornecedor e convidado não viram role nesta etapa. Isso espera RF-035 e DP-016.

### 8.3. Sessão

| Diretriz | Status |
|---|---|
| JWT de acesso com duração de 8 horas | Definido (DA-029, RNF-016) |
| Sem sliding expiration, sem refresh e sem lista de revogação | Definido (DA-029) |
| Cliente guarda o token em `sessionStorage` e envia `Authorization: Bearer` | Definido (DA-019, RNF-016) |
| Não há cookie de autenticação | Definido (DA-019) |
| HTTPS obrigatório em produção | Definido (RNF-014) |
| Pedido que altera estado exige o Bearer. Não há antiforgery de formulário | Definido (RNF-013) |
| CORS só para a origem local do `ng serve`. Em produção a origem é a do Caddy | Definido (DA-003) |
| Chaves do Data Protection em volume persistente, para os tokens de e-mail do Identity | Definido (RNF-015, DA-026) |

### 8.4. Bootstrap

Configuração por variável de ambiente, ou mecanismo seguro equivalente, nunca por valor real em arquivo versionado:

```text
Bootstrap__Enabled
Bootstrap__AdminEmail
Bootstrap__AdminPassword
```

`Bootstrap__Enabled` permanece desligado, salvo na janela do primeiro deploy.

Quando habilitado, a ordem é:

1. Conferir se e-mail e senha foram informados e se a senha cumpre DA-028. Se faltar ou for inválida, a inicialização falha sem escrever o segredo no log.
2. Criar a role `Admin` se ela não existir.
3. Criar a role `User` se ela não existir.
4. Se não existir usuário com o e-mail configurado, criar o administrador pelo `UserManager`, marcar o e-mail como confirmado e atribuir `Admin`.
5. Se o usuário já existir e já tiver a role `Admin`, não alterar nada. A senha permanece como está.
6. Se o usuário já existir como usuário comum, interromper o bootstrap. A resolução é manual.
7. Repetir a inicialização não duplica role nem usuário.

Depois que o administrador entrou pela primeira vez, `Bootstrap__Enabled` volta a desligado. A aplicação não apaga a variável sozinha. Quem opera a VPS desliga.

Restrições já fechadas:

- Sem senha administrativa escrita no repositório ou em `HasData()`.
- Sem endpoint HTTP para rodar o bootstrap.
- Sem redefinir senha administrativa a cada startup.
- Sem promover cadastro público a `Admin`, inclusive o primeiro cadastro.
- Sem framework genérico de seeds. O código deste bootstrap serve só para as duas roles e para o primeiro administrador.

### 8.5. E-mail transacional

Definido em DA-021, DA-022, RF-036 e RNF-020.

Servidor inicial: `smtp.gmail.com`, porta `587`, STARTTLS. A autenticação usa senha de aplicativo do Google, gerada uma vez. A aplicação não abre login interativo no Google em tempo de execução. O MailKit usa as credenciais já configuradas.

Não haverá servidor SMTP próprio nem relay autenticado por IP fixo.

O contrato consumido pelo Identity no .NET 10 é `IEmailSender<TUser>`, em `Microsoft.AspNetCore.Identity`. A Infrastructure implementa esse contrato com MailKit e o registra na composição. Não será criada outra abstração de e-mail na Application, e o pacote de UI padrão do Identity não entra só para obter o `IEmailSender` não genérico. Esse contrato do framework cobre confirmação de cadastro e recuperação de senha. Ele não é uma caixa de saída genérica do domínio.

Trocar o Gmail por outro SMTP no futuro altera a configuração e, se a API do provedor mudar, a implementação na Infrastructure. Os casos de uso e os endpoints permanecem.

Configuração não sensível no `appsettings.json`. O arquivo versionado guarda placeholder, não a caixa real:

```json
{
  "Email": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "seuemail@gmail.com",
    "FromAddress": "seuemail@gmail.com",
    "FromName": "Minha Aplicação",
    "UseStartTls": true
  }
}
```

A senha fica em `Email__Password`, em User Secrets no desenvolvimento, ou em arquivo protegido fora do repositório. O binding usa Options e `IConfiguration`. Endereço e nome reais de produção podem ser sobrescritos pelas variáveis `Email__Username`, `Email__FromAddress` e `Email__FromName`.

Limites publicados pelo Google para o SMTP do Gmail, em janela móvel de cerca de 24 horas:

| Conta | Mensagens |
|---|---|
| Gmail pessoal, ou Workspace em teste | até 500 |
| Google Workspace pago | até 2.000 |

Outras restrições que acompanham essa escolha:

- A senha de aplicativo exige verificação em duas etapas na conta Google. A senha normal da conta não autentica no SMTP.
- Pelo SMTP, cada mensagem aceita até cerca de 100 destinatários. Confirmação e recuperação desta versão usam um destinatário.
- Estourar o limite faz o Google recusar o envio por um período que pode chegar a 24 horas.
- O limite é da caixa, compartilhado com outros clientes que usem a mesma conta.
- Mensagem considerada suspeita pode ser filtrada ou rejeitada.
- O remetente efetivo tende a ser a conta autenticada.
- Este canal não serve para disparo em massa, como RSVP para centenas de convidados.

Fontes: [limites de envio do Gmail](https://support.google.com/mail/answer/22839) e [SMTP do Gmail](https://developers.google.com/workspace/gmail/imap/imap-smtp).

### 8.6. Rate limiting

Definido em DA-030, RF-017, RNF-021, RNF-022 e RNF-023.

Limites iniciais, por IP:

| Operação | Limite |
|---|---|
| Cadastro público | 5 requisições por hora |
| Login | 10 requisições por minuto |
| Recuperação de senha | 3 requisições por hora |
| Reenvio de confirmação de e-mail | 3 requisições por hora |

O contador é em memória, de uma instância. Várias pessoas atrás do mesmo endereço, como uma rede de escritório, dividem o mesmo limite. Os números ficam no `appsettings.json` para ajuste sem recompilar. O lockout de 15 minutos continua por conta, separado do limite por IP.

Atrás do Caddy, o IP do cliente vem do cabeçalho encaminhado. A aplicação só aceita esse cabeçalho de proxies colocados na configuração. O restante da internet não pode inventar o IP.

## 9. Padrões de desenvolvimento

Estas regras estão **definidas**.

- Preferir o recurso nativo do ASP.NET Core, do EF Core e do Identity antes de adicionar pacote. As exceções já decididas são MailKit, para SMTP, e Testcontainers, para PostgreSQL real nos testes.
- Injeção de dependência nativa. Interface só quando existir mais de uma implementação real ou quando a camada interna precisar depender de um contrato.
- Logging com `ILogger`. Senha, token, senha SMTP, connection string e chave do Data Protection não entram no log.
- Configuração em `appsettings` por ambiente. Segredo por User Secrets, variável de ambiente ou arquivo protegido.
- Fluent API para o mapeamento das entidades quando a convenção do EF Core não expressar a restrição.
- I/O assíncrono até o banco, o disco e o SMTP.
- Validação no servidor, mesmo quando a tela também validar no navegador.
- Endpoint de negócio fino. Regra de negócio na Application ou no Domain, conforme o tipo da regra. A tela Angular não substitui essa validação.
- Entidade de domínio rica só onde houver invariante. Lista e cadastro simples não ganham camada extra.
- Sem Generic Repository, sem Unit of Work manual, sem repositório de usuário ou role.
- Sem CQRS, MediatR, AutoMapper e sem fila até existir um caso em que o código nativo fique pior de manter.
- A solution permanece nos projetos da seção 6.

## 10. Persistência e migrations

**Definido**, junto com DA-005, DA-012, DA-032 e RNF-024.

- Um `DbContext` de aplicação na Infrastructure, integrado ao Identity por `IdentityDbContext`.
- PostgreSQL como único banco desta versão, acessível só pela rede interna do Compose.
- Migrations versionadas no projeto Infrastructure.
- Consultas e comandos de negócio podem usar o `DbContext` diretamente a partir de um serviço da Infrastructure que implemente um contrato da Application.
- Repositório por agregado só quando a consulta for reutilizada ou quando esconder o EF Core simplificar o caso de uso.
- Arquivos em volume persistente, conforme RNF-004 e DA-026.

Produção não chama `Database.Migrate()` na inicialização e não aplica schema quando o container sobe.

O caminho preferido é o script SQL idempotente:

```bash
dotnet ef migrations script --idempotent
```

O script é revisado antes de rodar em produção. Não haverá ferramenta própria de migration.

Fluxo do deploy quando o schema muda:

1. O pull request é aprovado.
2. A pipeline aponta migrations novas.
3. Quem desenvolve informa que o banco precisa ser atualizado.
4. O deploy recebe aprovação para essa atualização.
5. A migration roda de forma explícita.
6. Se ela terminar bem, a nova versão da aplicação pode ser publicada.
7. Se ela falhar, a publicação para.

Se a pipeline alcançar o banco de destino e encontrar migration pendente sem autorização, o deploy para.

Migration que apaga ou reescreve dado recebe revisão própria. Enquanto RNF-018 valer, não existe cópia para desfazer esse tipo de mudança. Durante a troca de versão, o schema e a aplicação que ainda estiver no ar precisam continuar compatíveis. Por isso a ordem é migration bem-sucedida e só então a publicação.

## 10.1. Arquivos e uploads

Definido em DA-017, DA-035 a DA-041, RF-042, RF-043 e RNF-004, RNF-026 a RNF-030.

Não há S3, Azure Blob nem MinIO. O volume do container é a raiz de armazenamento, configurável no `appsettings.json`. O valor inicial no container é `/storage`.

```text
/storage/
├── documents/
│   └── 2026/
│       └── 11/
│           └── {guid}.pdf
├── images/
│   └── 2026/
│       └── 11/
│           └── {guid}.webp
└── temp/
```

`documents` e `images` ilustram a organização. A categoria real acompanha o caso de uso, quando o domínio existir. Ano e mês são os da gravação. `temp` recebe o arquivo enquanto a validação e a persistência não terminam.

Regras de nome e caminho:

- O nome físico é um GUID mais a extensão do tipo validado.
- O nome original é guardado no banco, sem caminho, sem caractere de controle e sem sequência que atravesse diretório.
- O destino é sempre relativo à raiz configurada. Entrada do usuário não escolhe pasta.
- A gravação final usa criação exclusiva, para não sobrescrever um arquivo existente.
- O diretório não é servido como site e não tem permissão de execução.

Quem pode ver cada arquivo depende do requisito funcional daquele caso. Enquanto isso não existir, a regra técnica é: arquivo privado só sai pela aplicação, para um usuário autenticado e autorizado àquele recurso. A resposta traz o MIME validado, disposição `attachment` com o nome sanitizado e `nosniff`. O corpo não inclui o caminho do volume.

Limites iniciais, todos configuráveis:

| Tipo | Limite |
|---|---|
| Imagem | 5 MB por arquivo |
| Documento | 10 MB por arquivo |
| Quantidade | 5 arquivos por requisição |
| Total da requisição | 50 MB |

Formatos aceitos: PDF, JPG/JPEG, PNG e WebP. Executável, script e arquivo compactado ficam de fora até um requisito funcional pedir outro formato.

A validação no servidor confere tamanho, extensão, arquivo vazio e a assinatura do conteúdo. JPEG, PNG, WebP e PDF são reconhecidos pelo início do arquivo, não pelo cabeçalho que o navegador declara. Extensão e assinatura precisam concordar.

O upload usa `IFormFile`. O arquivo segue para `temp` e só vai para a pasta final depois da validação. O código não copia o conteúdo inteiro para um `byte[]` sem necessidade. O Kestrel e o Caddy usam o mesmo teto de 50 MB para o corpo dessa requisição, também configurável. Se o envelope multipart passar um pouco desse teto, a margem entra na mesma configuração, sem um segundo limite escondido no código.

Disco e banco não commitam juntos. A ordem é gravar o arquivo final, persistir o metadado e, se o banco falhar, apagar o arquivo daquela tentativa no mesmo fluxo e registrar o erro com `ILogger`. Arquivo parcial em `temp` também é removido quando a validação ou a gravação falha. Se o processo morrer no meio, pode sobrar um arquivo sem linha no banco. Esta versão não cria job para varrer isso.

Upload de pessoa não confiável pode trazer malware. Não há antivírus nesta etapa. Antes de liberar essa função para uso real, essa proteção precisa ser avaliada de novo. DA-041.

## 10.2. Testes

Definido em DA-042, DA-002 e RNF-019.

| Suíte | Onde | O que prova |
|---|---|---|
| Unitária | `tests/Project.Tests` | Regra de negócio, invariante, value object, validação e serviço de aplicação com lógica. Também os casos de erro. |
| Integração | `tests/Project.IntegrationTests` | Mapeamento do EF Core, consulta, constraint, migration, fluxo HTTP da API, autenticação, autorização e arquivo. |

A suíte unitária não acessa banco, rede nem Gmail, e cabe em todo pull request. Não se cria teste só para aparecer em métrica de cobertura. Não há percentual obrigatório. O foco é autenticação, autorização, persistência e arquivo.

A suíte de integração sobe PostgreSQL com Testcontainers e usa Npgsql. O banco é descartável e não aponta para desenvolvimento nem produção. `WebApplicationFactory` exercita o HTTP sem um servidor à parte. O provider InMemory do EF Core não substitui esses cenários.

Autenticação que a integração precisa cobrir: cadastro válido, e-mail duplicado, login válido e inválido, confirmação de e-mail, recuperação de senha, bloqueio após as tentativas, página administrativa negada ao `User`, página administrativa permitida ao `Admin` e bootstrap idempotente. O `IEmailSender<TUser>` dos testes é uma implementação local. Nenhum teste envia mensagem pelo Gmail.

Arquivo que a integração precisa cobrir: upload válido, extensão recusada, arquivo acima do limite, arquivo vazio, assinatura incompatível, tentativa de path traversal, acesso sem autorização, falha ao gravar o metadado e limpeza do temporário quando o fluxo quebra. O diretório usado é temporário e exclusivo do teste.

Biblioteca de mock ou de assertion só entra se o xUnit e um dublê simples não derem conta. Essa necessidade ainda não existe.

Não há teste de ponta a ponta com navegador nesta versão.

## 11. Hospedagem

Definido em DA-023, DA-024, DA-025, DA-026 e DA-027.

Uma VPS. Três serviços no Compose:

| Serviço | Função |
|---|---|
| Caddy | Certificado HTTPS, redirecionamento de HTTP para HTTPS, publicação do build estático do Angular, encaminhamento de `/api` para a API e o mesmo teto de corpo dos uploads. Não publica `/storage`. Não sobe processo Node. |
| API ASP.NET Core | Web API, Identity, JWT, rate limiting, SMTP e acesso ao PostgreSQL pela rede interna |
| PostgreSQL | Dados. Sem porta publicada no host |

Volumes que sobrevivem à recriação dos containers: dados do PostgreSQL, arquivos enviados e chaves do Data Protection.

Segredos que não entram no Git: senha SMTP, senha do administrador de bootstrap, connection string com credencial, segredo de assinatura do JWT e qualquer chave privada. Em desenvolvimento, User Secrets. Em produção, variável de ambiente ou arquivo protegido.

## 12. Decisões pendentes

Itens já fechados saíram desta lista. O rastro está na seção 15.

### DP-016. Aprovação dos requisitos de domínio

O catálogo observado está na seção 2.7, RF-044 a RF-079. Falta aprovar o que entra no produto. Melhorias pedidas pelo cliente ainda não existem.

Enquanto isso, a autorização implementada é `Admin` e `User`, e `User` não é papel de negócio, conforme RF-040. RF-045, RF-056 e RF-060 não podem ser implementados como roles novas sem uma decisão que altere RF-040.

## 13. Atividades futuras

Não são pendências desta etapa e não entram na implementação agora.

- Estratégia de backup e restauração, obrigatória antes de dados reais em produção, RNF-018 e DA-033.
- Troca do Gmail por outro SMTP quando o volume passar dos limites da seção 8.5. A troca fica na Infrastructure.
- Verificação antimalware, antes de liberar upload de usuário não confiável, DA-041.
- Migração do volume local para object storage, se o disco da VPS deixar de ser suficiente.
- Autenticação multifator.
- Login social.
- Fila ou worker, só se um caso concreto deixar a requisição lenta. E-mail desta versão não justifica.
- Monitoramento além do log nativo.
- Teste de ponta a ponta com navegador, se um fluxo crítico pedir.
- Roles de negócio, só depois de RF que as justifiquem.

## 14. Conflitos e pontos de atenção

Decisões anteriores que este texto altera ou que precisam de cuidado:

1. **Lockout.** DA-020 deixou de ser a proposta de 5 minutos e passou a ser 15 minutos, definido.
2. **E-mail.** DP-010 estava aberto e bloqueava o primeiro acesso. O canal agora é Gmail SMTP. O cadastro continua dependente desse envio, e a caixa tem o teto da seção 8.5.
3. **Backup.** A pendência de escolher a rotina foi substituída pela decisão de não ter backup nesta versão. O risco de perda total permanece e fica explícito em RNF-018.
4. **Migrations.** A proposta de comando explícito virou DA-032. `Database.Migrate()` na subida deixa de ser opção.
5. **Cookie.** A duração de 8 horas chegou a ser fechada em DA-029 como cookie com sliding expiration. Essa forma foi substituída pelo JWT. O registro da troca está no item 17.
6. **MailKit e a regra de poucos pacotes.** DA-014 evita biblioteca sem necessidade. MailKit entra porque o `SmtpClient` de `System.Net.Mail` não é o caminho recomendado para SMTP. Não entram outros pacotes de e-mail.
7. **Nome `IEmailSender`.** No .NET 10 o Identity chama `IEmailSender<TUser>`. A implementação segue esse contrato. Não será adotado o `IEmailSender` não genérico do pacote de UI.
8. **Um prazo para dois tokens.** A recuperação permanece em 1 hora e a confirmação passou a 24 horas, DA-043. O provider padrão do Identity não carrega os dois prazos. A implementação usa dois providers nativos, sem criptografia própria. DP-013 fecha com isso.
9. **Letra minúscula na senha.** DA-028 desliga número, maiúscula e caractere especial. Não fala de minúscula. O padrão do Identity, que exige uma minúscula, permanece. Uma senha de 8 dígitos continua inválida por causa disso.
10. **`User` e o brief.** O brief descreve assessoria, assistente, casal, fornecedor e convidado sem conta. RF-040 impede tratar a role `User` como esses papéis. O convidado por link e a conta pública só convivem se forem coisas diferentes, o que DP-016 ainda precisa escrever.
11. **IP compartilhado.** Cinco cadastros por hora no mesmo IP podem atingir uma rede inteira. O número está definido e pode ser ajustado no `appsettings.json`. O lockout por conta não substitui esse limite.
12. **Migration sem backup.** DA-032 pede revisão especial do que é destrutivo. Sem cópia do banco, essa revisão é o único freio.
13. **Api referencia Infrastructure.** Continua intencional para o `Program.cs`. Endpoint de negócio não usa `DbContext` nem MailKit. A regra da seção 7.1 permanece.
14. **Bootstrap sem role `Admin`.** Está definido o que fazer se o e-mail não existe, se já é `Admin` e se já é usuário comum. Não está definido o que fazer se o e-mail do bootstrap existe e a role `Admin` foi removida depois. A resolução continua manual.
15. **Quem vê qual arquivo.** A entrega técnica está fechada: autenticação, autorização e resposta segura. A regra de qual usuário acessa qual arquivo continua no domínio, DP-016. Sem esse requisito, não há tela de upload de negócio.
16. **Testcontainers.** A suíte de integração precisa de Docker para subir o PostgreSQL. Isso é intencional: o provider InMemory não reproduz constraint nem migration. A suíte unitária segue sem Docker, para o pull request continuar leve.
17. **Interface e sessão.** A interface deixou de ser Razor Pages com AdminLTE, DA-003 e DA-044, e passou a ser o build estático do Angular, com a API em `Project.Api`. A sessão deixou de ser cookie, RF-012, DA-006, DA-029, RNF-013 e RNF-016, e passou a ser JWT de acesso de 8 horas, sem renovação, sem refresh e sem lista de revogação. DA-019 deixa de propor `SameSite`. O logout do cliente não invalida um token já emitido.
18. **Template da interface.** O Apex foi descartado porque é pago. DA-044 passa a ser Angular Material, no tema padrão. Componente próprio entra quando o Material não cobrir a tela. A sessão do item 17 permanece.

## 15. Pendências já resolvidas

| ID antigo | Decisão que o substitui |
|---|---|
| DP-001 | DA-023, Docker Compose |
| DP-002 | DA-024, Caddy |
| DP-003 | DA-033 e RNF-018, sem backup nesta versão |
| DP-007 | DA-022, e-mail na requisição, sem fila |
| DP-008 | DA-032, migration explícita no deploy |
| DP-009 | DA-027, DA-026 e RNF-001, segredos e volumes |
| DP-010 | DA-021 e RNF-020, Gmail SMTP com MailKit |
| DP-011 | DA-030, limites da seção 8.6 |
| DP-012 | DA-028, senha mínima de 8 |
| DP-014 | DA-029, duração de 8 horas. A forma deixou de ser cookie com sliding expiration e passou a ser JWT sem renovação. Ver seção 14, item 17. |
| DP-015 | Seção 13, MFA e login social ficam para o futuro |
| DP-004 | DA-035 e DA-036, árvore `/storage`, GUID e metadados |
| DP-005 | DA-039 e DA-040, limites, formatos e `IFormFile` |
| DP-006 | DA-042 e RNF-019, xUnit, WebApplicationFactory e Testcontainers |
| DP-013 | DA-043 e RF-041, confirmação em 24 horas, com provider distinto da recuperação |
