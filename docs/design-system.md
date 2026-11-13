# Design system

A interface do gevents é Angular Material. A referência visual é a demo pública [Apex Dashboard](https://apex-angular.dashboardpack.com), em especial a lista em [Orders](https://apex-angular.dashboardpack.com/orders), os cartões do [Dashboard](https://apex-angular.dashboardpack.com/dashboard) e a lista de [Customers](https://apex-angular.dashboardpack.com/customers).

O código desse template não entra no repositório. As cores abaixo foram lidas da demo. O que a visita não abriu não vira componente.

## Tema

Há modo claro e modo escuro. A escolha fica em `localStorage`, chave `gevents-tema`, com os valores `claro` e `escuro`. Sem escolha salva, o primeiro carregamento segue `prefers-color-scheme`. O botão de tema fica fixo no canto superior direito e vale na casca e nas telas públicas.

A classe `escuro` no `html` liga o modo escuro. O modo claro é a ausência dessa classe.

A fonte da demo é Geist Sans. O gevents usa a pilha `ui-sans-serif, system-ui, sans-serif`, que no Windows cai em Segoe UI. Não se adiciona pacote de fonte.

## Cores

Valores em `oklch`, os mesmos da demo.

| Token | Claro | Escuro |
| --- | --- | --- |
| `--gd-background` | `oklch(0.985 0.002 230)` | `oklch(0.075 0 0)` |
| `--gd-foreground` | `oklch(0.155 0.015 230)` | `oklch(0.922 0.005 230)` |
| `--gd-card` | `oklch(1 0 0)` | `oklch(0.19 0 0)` |
| `--gd-primary` | `oklch(0.55 0.175 160)` | `oklch(0.55 0.19 160)` |
| `--gd-primary-foreground` | `oklch(0.985 0.002 230)` | `oklch(1 0 0)` |
| `--gd-muted` | `oklch(0.96 0.005 230)` | `oklch(0.235 0 0)` |
| `--gd-muted-foreground` | `oklch(0.556 0.015 230)` | `oklch(0.556 0.015 230)` |
| `--gd-border` | `oklch(0.895 0.005 230)` | `oklch(0.28 0 0)` |
| `--gd-success` | `oklch(0.62 0.17 145)` | `oklch(0.62 0.17 145)` |
| `--gd-warning` | `oklch(0.75 0.175 75)` | `oklch(0.75 0.175 75)` |
| `--gd-destructive` | `oklch(0.577 0.245 27.325)` | `oklch(0.577 0.245 27.325)` |
| `--gd-sidebar` | `oklch(0.14 0.015 170)` | `oklch(0.11 0 0)` |
| `--gd-sidebar-foreground` | `oklch(0.87 0.005 230)` | `oklch(0.9 0.005 230)` |
| `--gd-sidebar-accent` | `oklch(0.2 0.015 170)` | `oklch(0.2 0 0)` |

A barra lateral permanece escura nos dois modos. O miolo troca com o tema.

O primário do Angular Material (`--mat-sys-primary` e a superfície) aponta para esses tokens. Botão preenchido, campo e cartão do Material herdam daí. Não se cria uma segunda biblioteca de componentes.

## Casca

- Barra lateral de 260px, fundo `--gd-sidebar`, texto `--gd-sidebar-foreground`.
- Marca no topo: quadrado verde de 28px com raio 8px e o nome do produto.
- Grupos com rótulo em caixa alta, 11px, opacidade baixa.
- Item de 36px, raio 10px, ícone e texto a 14px. O item da rota atual usa o verde do primário sobre um fundo misturado a 18%.
- Barra superior de 64px, fundo do miolo, borda inferior `--gd-border`. À direita, o e-mail e Sair. O alternador de tema fica no canto da janela.
- Conteúdo com 24px de respiro.

## Cabeçalho de página

O bloco `.gd-cabecalho` repete o topo de Orders:

- Título a 24px e peso 700.
- Linha de apoio a 14px em `--gd-muted-foreground`.
- Ação principal à direita: botão preenchido verde, altura 32px, raio 8px, ícone `add` e o rótulo do cadastro (`Novo evento`, `Novo cliente`, e o equivalente de cada lista).

## Tabelas

Lista de registros usa `.gd-painel`: cartão de raio 14px, borda `--gd-border`, fundo `--gd-card`.

Dentro do cartão:

- Abas em pílula (`.gd-abas`), quando a tela já tem filtro de situação. A aba ativa tem fundo `--gd-background`.
- Busca à direita, quando a tela já tem busca.
- Tabela em largura total. Cabeçalho a 12px, peso 500, cor `--gd-muted-foreground`. Linha com divisória suave e realce no hover.
- Situação em cápsula (`.gd-status`): verde para concluído ou confirmado, âmbar para pendente ou em andamento, vermelho para cancelado ou recusa.

A última coluna é Ações, alinhada à direita. Ela contém, nesta ordem, Editar (`edit`) e Excluir (`delete`), como botões de ícone. Editar abre o diálogo do cadastro sobre a tela atual. Excluir pede confirmação. A remoção só segue quando a API daquele fluxo já oferece a operação. Sem essa operação, a confirmação não chama a API e a tela avisa que a exclusão ainda não está disponível.

Não se desenha minigráfico na coluna, como a coluna Trend da demo. O gevents não tem essa série.

## Cadastros

Novo e Editar de evento, cliente e tarefa abrem `MatDialog` sobre a tela que já está aberta. Novo convite abre painel modal sobre a lista de convites. A rota da lista não muda.

O diálogo usa a classe `gd-dialog`: superfície `--gd-card`, raio 14px, borda `--gd-border`, largura até 840px e altura até 90% da janela, com o miolo rolando. O fundo escurece com `gd-backdrop`.

O formulário continua em Angular Material, campos `outline`, uma coluna. Salvar é o botão preenchido. Cancelar fecha o diálogo sem gravar.

Perfil da assessoria permanece página, porque não é um cadastro aberto a partir de uma lista.

## Cartões do painel

Os números do dashboard ficam em `.gd-numero`: rótulo discreto, valor em 28px e um ícone num quadrado de 36px com o verde a 15%. Vários cartões na mesma linha, com quebra. Sem gráfico, enquanto um requisito não pedir a série.

Cartão comum do Material usa o mesmo raio, borda e fundo `--gd-card`.

## Telas públicas

Login, cadastro, confirmação, reenvio, senha e RSVP ficam num cartão centrado sobre `--gd-background`. O alternador de tema continua visível.
