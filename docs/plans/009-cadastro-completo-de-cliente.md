# Plano 009. Cadastro completo de cliente

**Status: aprovada.** Spec [009-cadastro-completo-de-cliente.md](../specs/009-cadastro-completo-de-cliente.md). Depende dos planos 002, 006 e 008.

## Fora deste plano

Login do cliente, último acesso, matriz de permissões, exclusão e o e-mail de primeiro acesso. A aba Permissões aparece vazia. A lixeira da referência não entra.

O interruptor "cadastrar clientes" da criação do evento passa a abrir estes campos. O casal gravado no evento continua sem virar cliente.

## Camadas

| Camada | O que entra |
|---|---|
| Domain | `Cliente` com papel obrigatório, contato obrigatório e endereço. Papel só aceita a lista da spec. |
| Application | Cadastrar, editar, listar, vincular, desvincular e cadastrar pelo link do evento. Reusa a consulta de CEP do plano 008. |
| Infrastructure | Mapeamento, uma migration e a foto no fluxo de arquivo já definido. |
| Web | Lista, formulário em três abas, gerência no evento e página pública do link. PageModel não usa `DbContext`. |

## Cliente

Obrigatórios: nome, papel, e-mail, DDI e celular. DDI nasce `+55`. E-mail inválido não grava. Telefone fixo, nascimento, CPF, RG, CNPJ, profissão, inscrição municipal, religião e observação são opcionais.

CPF, CNPJ e CEP, se preenchidos, precisam ter 11, 14 e 8 dígitos. A máscara fica na página. Este plano não valida dígito verificador.

Papéis, e só estes: Anfitrião, Aniversariante, Avó, Avô, Comissão, Formando(a), Mãe, Noiva, Noivo, Pai, Promotor, Representante, Tia, Tio.

Redes: Facebook e Instagram. Endereço: país, CEP, endereço, número, estado, cidade, bairro e complemento. CEP com oito dígitos preenche endereço, estado, cidade e bairro pela mesma consulta do plano 008.

Foto opcional, mesmas regras do arquivo de imagem do evento. O formulário da conta não pede evento.

A lista mostra nome, telefone, e-mail e os eventos vinculados. Sem evento, o texto é "Nenhum evento vinculado". Não há último acesso.

## Vínculo no evento

Três caminhos, todos no evento:

| Caminho | O que faz |
|---|---|
| Selecionar existente | O vínculo que a SPEC-006 já tem. |
| Cadastrar novo | O formulário completo e, ao gravar, o vínculo com esse evento. |
| Link | Gera um identificador opaco. A página pública, sem login, usa os mesmos campos e já grava o cliente vinculado ao evento. A tela autenticada só copia o link. |

Não há envio de e-mail nem de WhatsApp. O mesmo par cliente-evento não duplica. Um cliente pode estar em dois eventos da conta. Cliente ou evento de outra conta não vincula.

## Páginas

Autorização `User` nas páginas da conta e do evento. A página do link é anônima e só mostra o formulário daquele evento.

## Persistência

Uma migration nos campos novos do cliente, inclusive a foto. O vínculo existente não muda de tabela. O link guarda o evento e o identificador opaco.

## Testes

Unitário: obrigatório ausente, e-mail inválido, papel fora da lista, CPF com quantidade errada de dígitos. Integração: cadastro mínimo, CEP falso, vínculo, desvínculo, um cliente em dois eventos, link público gravando o vínculo, link de outra conta sem revelar dado, casal do evento inalterado.

## Tarefas

1. Campos, três abas e lista.
2. Os três caminhos de vínculo no evento.
3. Migration e testes da spec.

## Pendência que permanece

Foto obrigatória ou não. Cliente com o mesmo e-mail. Texto e disparo do e-mail de primeiro acesso. Permissões. Exclusão e o efeito nos vínculos. Último acesso.
