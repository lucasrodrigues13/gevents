# Rastreabilidade visual

Situação: **Conforme** quando o markup segue o catálogo; **Pendente** quando o padrão existe e a tela ainda não o cobre por falta de contrato; **Não aplicável** quando a spec não pede o componente.

Validação desta rodada: `dotnet build src/Project.Web/Project.Web.csproj` com 0 erros. `Project.Tests`: 20 aprovados. `Project.IntegrationTests`: 35 aprovados. Não houve renderização em navegador: a connection string local está vazia e o app não foi aberto no browser.

| Funcionalidade | Página | Componente | Referência | Implementação | Situação | Validação |
|---|---|---|---|---|---|---|
| Conta | `Pages/Shared/_LayoutAutenticado.cshtml` | Layout, sidebar, header, trilha, footer | [index v4](https://adminlte.io/themes/v4/index.html) | Tag Helpers do pacote | Conforme | Revisão de markup |
| Entrar | `Pages/Account/Login.cshtml` | Login e validação | [login v4](https://adminlte.io/themes/v4/examples/login.html), [validação](https://adminlte.io/themes/v4/forms/validation.html) | `_LayoutConta` | Conforme | Revisão de markup |
| Cadastro de conta | `Pages/Account/Register.cshtml` | Formulário | validação v4 | `_LayoutConta` | Conforme | Revisão de markup |
| Esqueci a senha | `Pages/Account/ForgotPassword.cshtml` | Formulário | validação v4 | `_LayoutConta` | Conforme | Revisão de markup |
| Nova senha | `Pages/Account/ResetPassword.cshtml` | Formulário e alerta | validação v4 | `_LayoutConta` | Conforme | Revisão de markup |
| Confirmação de e-mail | `Pages/Account/ConfirmEmail.cshtml` | Alerta | `adminlte-alert` | Tag Helper | Conforme | Revisão de markup |
| Reenviar confirmação | `Pages/Account/ResendConfirmation.cshtml` | Formulário | validação v4 | `_LayoutConta` | Conforme | Revisão de markup |
| Início | `Pages/Index.cshtml` | Botões | Bootstrap | `_Layout` | Conforme | Revisão de markup |
| Erro | `Pages/Error.cshtml` | Mensagem | alerta Bootstrap | `_Layout` | Conforme | Revisão de markup |
| Dashboards | `Pages/Dashboards/Index.cshtml` | Small-box, lista, funil | index v4 | markup small-box | Conforme | Revisão de markup. Gráfico: não aplicável |
| Perfil | `Pages/Perfil/Index.cshtml` | Formulário | validação v4 | card | Conforme | Revisão de markup |
| Eventos | `Pages/Eventos/Index.cshtml` | Abas, filtro, tabela, badge, vazio | [tabelas v4](https://adminlte.io/themes/v4/tables/data.html) | card + `_BadgeEvento` | Conforme | Paginação e ordenação: pendente |
| Novo evento | `Pages/Eventos/Criar.cshtml` | Cards de modelo, formulário condicional | cards v4 + validação | `ge-modelo-card`, `_Campos` | Conforme | Revisão de markup |
| Editar evento | `Pages/Eventos/Editar.cshtml` | Abas e formulário | nav-tabs Bootstrap | `_Campos` | Conforme | Revisão de markup |
| Home do evento | `Pages/Eventos/Home.cshtml` | Info-box, badge, modal | index v4 e modal Bootstrap | `_BadgeEvento`, `_ModalConfirmacao` | Conforme | Revisão de markup |
| Valor da assessoria | `Pages/Eventos/Valor.cshtml` | Formulário condicional | validação v4 | card + `data-ge-toggle` | Conforme | Revisão de markup |
| Imagem indisponível | `Pages/Eventos/Imagem.cshtml` | Alerta | `adminlte-alert` | Tag Helper | Conforme | Arquivo em si não é tela |
| Tarefas | `Pages/Tarefas/Index.cshtml` | Tabela e badge | tabelas v4 | `_BadgeTarefa` | Conforme | Paginação: pendente |
| Tarefas do evento | `Pages/Eventos/Tarefas/Index.cshtml` | Tabela, quadro, formulário | cards v4 | cards por coluna | Conforme | Revisão de markup |
| Tarefa | `Pages/Eventos/Tarefas/Editar.cshtml` | Formulário | validação v4 | card | Conforme | Revisão de markup |
| Modelos | `Pages/Modelos/Index.cshtml` | Tabela | tabelas v4 | card | Conforme | Paginação: pendente |
| Novo modelo | `Pages/Modelos/Criar.cshtml` | Formulário | validação v4 | card | Conforme | Revisão de markup |
| Modelo | `Pages/Modelos/Editar.cshtml` | Lista e dois formulários | cards v4 | cards | Conforme | Revisão de markup |
| Clientes | `Pages/Clientes/Index.cshtml` | Filtro e tabela | tabelas v4 | card | Conforme | Paginação: pendente |
| Novo cliente | `Pages/Clientes/Criar.cshtml` | Grupos de campos | cards v4 | `_Campos` | Conforme | Revisão de markup |
| Cliente | `Pages/Clientes/Editar.cshtml` | Formulário e lista | cards v4 | `_Campos` | Conforme | Revisão de markup |
| Cadastro público | `Pages/Cadastro/Index.cshtml` | Formulário e alerta | validação v4 | `_Campos` | Conforme | Revisão de markup |
| Convidados | `Pages/Convidados/Index.cshtml` | Estado vazio orientado | card Bootstrap | card | Conforme | Lista global: não aplicável |
| Convites | `Pages/Eventos/Convites/Index.cshtml` | Filtro, tabela, cards com form | tabelas v4 + cards | card por convite | Conforme | Paginação: pendente |
| Editar convite | `Pages/Eventos/Convites/Editar.cshtml` | Abas e formulário | nav-tabs | âncoras | Conforme | Revisão de markup |
| RSVP | `Pages/Convite/Index.cshtml` | Radio e alerta | forms Bootstrap | `_LayoutPublico` | Conforme | Revisão de markup |
| Grupos | `Pages/Grupos/Index.cshtml` | Tabela e formulário | tabelas v4 | card | Conforme | Paginação: pendente |
| Área do usuário | `Pages/User/Index.cshtml` | Atalhos | botões Bootstrap | card | Conforme | Revisão de markup |
| Área administrativa | `Pages/Admin/Index.cshtml` | Card | cards v4 | card | Conforme | Sem outra função visual |
| Toast após redirect | layout autenticado | Toast | Bootstrap 5 toast | `TempData` | Pendente | Nenhum handler grava `TempData` ainda |
| Ordenação de tabela | listagens | Cabeçalho ordenável | tables/data v4 | — | Pendente | Serviço não ordena por coluna |
| Página de tabela | listagens | Paginação | tables/data v4 | — | Pendente | Serviço não pagina |
| Date picker | formulários de data | Seletor | forms v4 | `input` nativo | Não aplicável | Pacote não traz o seletor |
| Gráfico | dashboard | Chart | index v4 | small-box | Não aplicável | Spec não pede série |
