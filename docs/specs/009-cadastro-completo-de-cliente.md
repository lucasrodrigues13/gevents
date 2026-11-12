# 009. Cadastro completo de cliente

**Status: aprovada.** O código segue esta spec. Incremento sobre a SPEC-006. Fonte observada: [inventario-referencia.md](../inventario-referencia.md) §1.7 e §5.

## 1. Identificador e nome

SPEC-009. Cadastro completo de cliente.

## 2. Objetivo

O cadastro de cliente passa a ter os campos vistos na referência. O vínculo com o evento continua nascendo no evento, não na ficha do cliente.

## 3. Requisitos relacionados

RF-055. RF-056 fica fora: o portal e as permissões ainda não foram usados de ponta a ponta. E-mail de primeiro acesso dispara na referência; o envio real não foi executado neste teste.

## 4. Atores e permissões

O User da conta cria, lista e edita os próprios clientes. Outro User não vê. O cliente não tem login neste incremento.

## 5. Pré-condições

Conta autenticada. Para vincular, evento da mesma conta, SPEC-002.

## 6. Fluxo principal

1. Na lista da conta, o usuário abre Adicionar cliente.
2. Preenche os campos da seção 11, em três abas.
3. Salva. A lista mostra nome, telefone, e-mail, eventos vinculados e último acesso.
4. No evento, Gerenciar clientes oferece três caminhos: link para o próprio cliente se cadastrar, selecionar existente, cadastrar novo já vinculado.
5. O cartão do cliente na lista aponta para os eventos vinculados.

## 7. Fluxos alternativos

- Nome, papel, e-mail ou celular vazios: não grava.
- E-mail inválido: não grava.
- CEP completo: preenche endereço, estado, cidade e bairro.
- Sem evento vinculado: o cartão diz "Nenhum evento vinculado".
- Cadastrar pelo evento: a referência avisa que o cliente será notificado do primeiro acesso.

## 8. Regras de negócio

- Cliente pertence à conta. O evento só guarda o vínculo, como na SPEC-006.
- O casal gravado no evento (SPEC-008) não cria cliente sozinho.
- Papel é lista fechada, vista na tela: Anfitrião, Aniversariante, Avó, Avô, Comissão, Formando(a), Mãe, Noiva, Noivo, Pai, Promotor, Representante, Tia, Tio.
- DDI padrão `+55`.
- Um cliente pode estar em mais de um evento da conta.
- Excluir: a lista tem lixeira; a consequência da exclusão nos vínculos não foi executada.

## 9. Validações

Nome completo, papel, e-mail, DDI e celular obrigatórios. E-mail com formato válido. CPF, CNPJ e CEP, se preenchidos, seguem a máscara da tela. Tamanho máximo dos textos não foi definido.

## 10. Estados

Cliente cadastrado, com zero ou mais eventos. Sem situação de convite.

## 11. Dados necessários

Dados pessoais: foto, nome completo, papel, nascimento, e-mail, DDI, celular, telefone fixo, CPF, RG, CNPJ, profissão, inscrição municipal, religião, observação.

Redes: Facebook, Instagram.

Endereço: país, CEP, endereço, número, estado, cidade, bairro, complemento.

Lista: eventos vinculados e último acesso. Último acesso depende do portal, RF-056, e permanece sem regra própria aqui.

## 12. Interface

Lista com busca por nome. Modal de três abas. No evento, modal Gerenciar clientes com Clientes do evento, Permissões (vazia sem cliente) e Cadastrar novo cliente nas três opções da seção 6.

## 13. Erros

Campo inválido permanece no modal. Cliente ou evento de outra conta não revela dado. Falha ao gravar mantém o anterior.

## 14. Critérios de aceite

1. Sem nome, papel, e-mail ou celular, nada grava.
2. Cadastro válido aparece só na lista da própria conta.
3. O formulário do cliente não pede evento.
4. Vincular pelo evento mostra o evento no cartão e o cliente na home.
5. Desvincular tira os dois lados.
6. Papel só aceita um valor da lista da seção 8.
7. Não existe login de cliente neste incremento.

## 15. Cenários de teste

Cadastro só com os obrigatórios. E-mail inválido. CEP que completa o endereço. Vínculo e desvínculo. Um cliente em dois eventos. Isolamento entre contas.

## 16. Dependências

SPEC-001, SPEC-006, SPEC-002 para o vínculo.

## 17. Premissas e pontos não definidos

Obrigatoriedade de foto. Duplicidade de e-mail na mesma conta. Conteúdo e disparo do e-mail de primeiro acesso. Matriz da aba Permissões. Exclusão e o destino dos vínculos. Último acesso.
