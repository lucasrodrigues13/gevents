# 005. Perfil da assessoria

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-005. Perfil da assessoria.

## 2. Objetivo

O Organizer guarda os dados da empresa que foram vistos na tela de perfil da assessoria. Este é o perfil do Organizer.

## 3. Requisitos relacionados

RF-044. A conta e o e-mail vêm da SPEC-001.

## 4. Atores e permissões

O Organizer vê e edita o próprio perfil. Não vê o perfil de outra conta. Admin não tem tela de perfil de assessoria nesta spec. Client não edita este perfil.

## 5. Pré-condições

Organizer autenticado com e-mail confirmado.

## 6. Fluxo principal

1. O Organizer abre o perfil.
2. O e-mail da conta aparece e não pode ser editado.
3. Ele informa nome da empresa, responsável, CPF ou CNPJ, celular, data de nascimento e endereço.
4. Salva.
5. Ao reabrir, os dados gravados continuam lá.

## 7. Fluxos alternativos

- Primeira abertura, sem perfil gravado: o e-mail aparece e os outros campos vêm vazios.
- Salvar sem preencher os campos opcionais: grava o perfil só com o e-mail.
- Data de nascimento inválida: não grava.
- Outra conta: não existe caminho para o perfil alheio.

## 8. Regras de negócio

- Há um perfil por conta de Organizer.
- O e-mail é o da autenticação e permanece somente leitura, como na tela vista.
- CPF ou CNPJ é um único campo de documento. Não foi vista validação de dígito.
- Endereço é um único texto. A exploração não separou CEP, rua, cidade ou estado.
- Estes dados não criam evento, contato de cliente nem conta Identity.

## 9. Validações

Data de nascimento, se preenchida, é uma data válida e não futura. Os demais textos aceitam vazio. Tamanho máximo e máscara de celular ou documento não foram vistos.

## 10. Estados

Sem perfil gravado e perfil gravado. Salvar de novo substitui os campos editáveis e mantém o e-mail.

## 11. Dados necessários

E-mail da conta, nome da empresa, responsável, documento, celular, nascimento e endereço.

## 12. Interface

Uma página autenticada, e-mail visível e bloqueado, os outros campos editáveis e uma ação de salvar. Sem foto, assistente ou notificação.

## 13. Erros

Data inválida indica o campo e não salva. Falha ao gravar mantém o perfil anterior. Mensagem sem detalhe interno.

## 14. Critérios de aceite

1. O e-mail da conta aparece e não muda por esta tela.
2. Salvar e reabrir devolve os mesmos campos.
3. Salvar com os campos vazios não apaga o e-mail.
4. Data futura ou inválida não grava.
5. Outra conta não lê esse perfil.

## 15. Cenários de teste

Primeiro salvamento. Segundo salvamento substituindo o nome. Documento e endereço com texto livre. Data futura recusada. Duas contas com perfis diferentes.

## 16. Dependências

SPEC-001. Nenhuma dependência de evento.

## 17. Premissas e pontos não definidos

Quais campos além do e-mail são obrigatórios. Validação de CPF e CNPJ. Partes do endereço. Troca de e-mail, que esta tela não faz e a SPEC-001 também não define.
