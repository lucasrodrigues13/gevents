# 001. Autenticação e conta

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-001. Autenticação e conta.

## 2. Objetivo

Uma pessoa cria a própria conta, confirma o e-mail, entra, sai e recupera a senha. O primeiro administrador nasce só pelo bootstrap, nunca pelo cadastro público.

## 3. Requisitos relacionados

RF-001 a RF-034, RF-036 a RF-038, RF-040, RF-041, RNF-016, RNF-020, DA-018 a DA-031 e DA-043.

RF-042 e RF-043 existem, mas não há tela de arquivo neste fluxo.

## 4. Atores e permissões

| Ator | Pode |
|---|---|
| Visitante | Cadastrar, confirmar e-mail, pedir novo link, entrar, pedir redefinição. |
| User | Entrar nas áreas autenticadas de usuário. Não acessa área de `Admin`. Não escolhe a própria role. |
| Admin | Acessa a área administrativa. Nasce pelo bootstrap. |
| Sistema | Envia e-mail, bloqueia conta, aplica limite por IP. |

Não há papel de assessoria, assistente, cliente, fornecedor ou convidado nesta spec.

## 5. Pré-condições

- A aplicação está no ar, com PostgreSQL e SMTP configurados.
- O bootstrap, se for criar o primeiro `Admin`, está habilitado só no ambiente e é desligado depois do primeiro acesso.
- O cadastro público não depende do bootstrap.

## 6. Fluxo principal

1. O visitante informa e-mail e senha e envia o cadastro.
2. O sistema grava a conta com e-mail normalizado, sem confirmar, na role `User`.
3. O sistema envia o link de confirmação, válido por 24 horas.
4. O visitante abre o link e o e-mail passa a confirmado.
5. Ele entra com e-mail e senha.
6. O sistema entrega um JWT de acesso e leva à área de `User`.
7. Ele sai. O cliente deixa de enviar o token. O token já emitido continua válido até expirar.

O administrador inicial segue outro caminho: o bootstrap cria as roles, cria o usuário se o e-mail não existir, confirma o e-mail e atribui `Admin`. Ele entra pela mesma tela de login.

## 7. Fluxos alternativos

- E-mail já cadastrado: a mensagem diz que já existe conta. Nada é duplicado.
- E-mail ainda não confirmado: o login não completa e há caminho para reenviar o link.
- Reenvio: novo link de 24 horas. No máximo 3 pedidos por IP a cada hora.
- Esqueceu a senha: informa o e-mail. Se a conta existir, recebe link de 1 hora. A tela não revela se o e-mail existe.
- Token expirado ou inválido: a confirmação ou a troca de senha não ocorre. A pessoa pode pedir outro link.
- Cinco senhas erradas: a conta fica bloqueada por 15 minutos.
- Cadastro acima de 5 por IP a cada hora, login acima de 10 por IP a cada minuto, ou redefinição acima de 3 por IP a cada hora: a ação é recusada com 429 e `Retry-After`.
- Falha no envio do e-mail de cadastro: a conta pode existir, mas o sistema avisa que a confirmação não foi enviada. Não marca o e-mail como confirmado.
- Falha no envio da redefinição: a resposta continua sem revelar se a conta existe.
- Bootstrap com e-mail que já é `User`: a inicialização para e não promove esse usuário.
- Bootstrap repetido com o mesmo administrador: não cria outro e não troca a senha.

## 8. Regras de negócio

- E-mail é único depois da normalização.
- Cadastro público só recebe `User`.
- `User` não é assessoria, casal, fornecedor nem assistente.
- Sem confirmação não há login.
- Confirmação vale 24 horas. Redefinição vale 1 hora. São provedores de token separados do Identity. Não há criptografia própria.
- Bloqueio: 5 falhas, 15 minutos.
- O JWT de acesso vale 8 horas e não se renova sozinho. Não há refresh nem lista de revogação.
- Senha: mínimo 8 caracteres. Não se exige dígito, maiúscula nem símbolo. O Identity continua exigindo minúscula porque isso não foi desligado.
- Limites são por IP, em memória, configuráveis. Só o proxy configurado, o Caddy, define o IP do cliente.
- Não existe endpoint público de bootstrap.
- Pedido que altera estado exige o JWT no cabeçalho Authorization. Não há cookie de autenticação.

## 9. Validações

- E-mail em formato válido e senha com o mínimo e a minúscula ainda exigida.
- Confirmação de senha no cadastro e na redefinição, quando a tela pedir a senha duas vezes. A obrigatoriedade do segundo campo não foi escrita como RF separado. Ver pendências.
- Token ausente, vencido ou de outro usuário é rejeitado.
- Credencial de bootstrap ausente com a flag ligada impede a inicialização.

## 10. Estados

| Estado da conta | Entra |
|---|---|
| E-mail não confirmado | Não |
| Confirmada e desbloqueada | Sim, com a senha certa |
| Bloqueada | Não, até acabar o prazo |
| Senha redefinida | Sim, com a senha nova |

Não há estado de assinatura, plano ou perfil de empresa nesta spec.

## 11. Dados necessários

E-mail, senha, confirmação do e-mail, role, contagem e prazo de bloqueio, JWT de acesso, tokens de confirmação e de redefinição.

Não entram CPF, endereço, nome da empresa nem dados de evento.

## 12. Interface

Páginas públicas: cadastro, login, confirmação, reenvio, esqueci a senha e redefinição.

Página autenticada mínima para `User` e página separada para `Admin`, suficientes para provar a autorização. Sem menu de evento.

Mensagens em português, sem stack trace, senha, token ou connection string.

## 13. Erros

- Dado inválido: a pessoa corrige e envia de novo.
- Credencial errada: mensagem única, sem dizer se o e-mail existe.
- Bloqueio e limite: dizer que a ação não pode ser repetida agora.
- Falha de e-mail no cadastro: dizer que o envio falhou.
- Falha interna: mensagem genérica e registro em log sem segredo.

## 14. Critérios de aceite

1. Cadastro com e-mail novo cria `User` não confirmado e dispara o envio.
2. O mesmo e-mail normalizado não cria segunda conta.
3. Login antes da confirmação falha.
4. Link válido dentro de 24 horas confirma e permite login.
5. Link vencido ou alterado não confirma.
6. Reenvio respeita 3 pedidos por hora por IP.
7. `User` recebe 403 na área de `Admin`.
8. `Admin` entra na área administrativa.
9. Cinco senhas erradas bloqueiam por 15 minutos.
10. Redefinição válida troca a senha. Resposta de pedido não revela se o e-mail existe.
11. Cadastro público não cria `Admin`.
12. Bootstrap desligado não cria usuário. Ligado, cria o admin se não existir e não troca senha existente.
13. E-mail já pertencente a `User` não é promovido a `Admin` pelo bootstrap.
14. Senha, token, senha SMTP e connection string não aparecem em log nem na resposta.

## 15. Cenários de teste

Os cenários de integração de RNF-019 cobrem cadastro válido, e-mail duplicado, login com senha correta e incorreta, confirmação válida e inválida, redefinição válida e inválida, bloqueio, `User` barrado na área administrativa, `Admin` autorizado e bootstrap idempotente.

Também entram: reenvio, limite 429, falha de SMTP no cadastro, token de confirmação com 24 horas e token de redefinição com 1 hora.

Teste de integração não envia e-mail real. Teste unitário não usa banco.

## 16. Dependências

PostgreSQL, Identity, MailKit, JWT de acesso, Data Protection em volume persistente e Caddy como proxy confiável. Nenhuma funcionalidade de evento.

## 17. Premissas e pontos não definidos

- Segundo campo de confirmação de senha não está num RF. A tela pode pedi-lo. A regra fechada é a senha da DA-028 mais a minúscula ainda exigida.
- Depois do login, o `User` vai para a tela de dashboards, SPEC-007. A área mínima desta spec continua servindo só para provar a autorização.
- E-mail de bootstrap que já foi `Admin` e perdeu a role não tem regra. Resolução manual, como já está em DA-026.
- Nome de exibição, foto e perfil da empresa são RF-044, observado, fora desta spec.
