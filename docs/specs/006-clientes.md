# 006. Clientes

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-006. Clientes.

## 2. Objetivo

A assessoria cadastra o cliente e vincula essa pessoa a um evento. O cliente não entra no sistema.

## 3. Requisitos relacionados

RF-055. RF-056 fica fora: o portal e as permissões não foram usados na exploração.

## 4. Atores e permissões

O User da conta cria, lista, edita e vincula os próprios clientes. Outro User não vê. O cliente não tem login neste corte.

## 5. Pré-condições

Conta autenticada. Para vincular, o evento precisa ser da mesma conta, SPEC-002.

## 6. Fluxo principal

1. O usuário cadastra nome, telefone e e-mail.
2. A lista da conta mostra o cliente e permite busca por nome.
3. No evento, o usuário vincula um cliente já cadastrado.
4. O cliente passa a mostrar esse evento, e o evento mostra esse cliente.
5. O usuário pode desfazer o vínculo e editar nome, telefone e e-mail.

## 7. Fluxos alternativos

- Nome vazio: não cadastra.
- Telefone e e-mail vazios: o cliente existe só com o nome.
- E-mail preenchido com formato inválido: não grava.
- Vincular cliente de outra conta ou evento de outra conta: não vincula.
- O mesmo cliente em mais de um evento da conta: permitido. A exploração mostrou eventos vinculados, no plural.
- Repetir o mesmo vínculo: a lista não duplica.

## 8. Regras de negócio

- Cliente pertence à conta, não ao evento. O evento só guarda o vínculo.
- O cadastro do casal dentro do evento, SPEC-002, não cria cliente e não é alterado por este vínculo.
- Não há último acesso. Esse dado depende do portal, que está fora.
- Não há permissão por módulo para o cliente.
- Excluir o cliente não foi visto. Não entra.

## 9. Validações

Nome obrigatório, sem ficar só com espaços. E-mail, se preenchido, tem formato válido. Telefone não tem máscara definida. Tamanho máximo dos textos não foi definido.

## 10. Estados

Cliente cadastrado, com zero ou mais eventos da mesma conta. Não há situação de convite nem de usuário.

## 11. Dados necessários

Nome, telefone, e-mail e a lista de eventos vinculados.

## 12. Interface

Lista com busca por nome. Formulário com os três campos. No evento, escolha de um cliente da conta e a lista dos já vinculados, com ação de desvincular. Sem tela de login do cliente.

## 13. Erros

Nome vazio ou e-mail inválido indica o campo. Cliente ou evento inacessível não revela dado de outra conta. Falha ao gravar mantém o vínculo anterior.

## 14. Critérios de aceite

1. Cadastrar com nome coloca o cliente na lista da própria conta.
2. Sem nome, nada é gravado.
3. A busca por nome não retorna cliente de outra conta.
4. Vincular mostra o evento no cliente e o cliente no evento.
5. Desvincular tira os dois lados.
6. O mesmo par cliente-evento não aparece duplicado.
7. Um cliente pode estar em dois eventos da conta.
8. Não existe entrada de login para o cliente.

## 15. Cenários de teste

Cadastro só com nome. E-mail inválido. Busca entre duas contas. Vínculo, desvínculo e tentativa de vincular de novo. Um cliente em dois eventos. Conferir que o casal gravado na SPEC-002 permanece um dado do evento.

## 16. Dependências

SPEC-001 e, para o vínculo, SPEC-002. Não depende de tarefa nem de RSVP.

## 17. Premissas e pontos não definidos

Obrigatoriedade de telefone e e-mail. Cliente com o mesmo nome ou o mesmo e-mail. Remoção do cliente e o que acontece com os vínculos. Último acesso e qualquer ação do cliente dentro do evento.
