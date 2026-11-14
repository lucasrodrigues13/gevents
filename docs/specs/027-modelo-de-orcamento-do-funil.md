# 027. Modelo de orçamento do funil

**Status: aprovada.** O código segue esta spec.

## 1. Identificador e nome

SPEC-027. Modelo de orçamento do funil.

## 2. Objetivo

O Organizer mantém o modelo de orçamento do funil (texto, parâmetros e aparência) e o aplica ao criar orçamentos na SPEC-025.

## 3. Requisitos relacionados

RF-075 e RF-077, no recorte de catálogos e orçamentos do funil. Lista de orçamentos é a SPEC-025. Oportunidade é a SPEC-020. RF-079 fica fora.

## 4. Atores e permissões

O `Organizer` lista, cria, edita e exclui modelos da própria conta. Outro `Organizer` não vê. `Admin` acessa qualquer conta. `Client` não acessa. Assistente e fornecedor com login ficam fora.

## 5. Pré-condições

Conta autenticada, SPEC-001.

## 6. Fluxo principal

1. O Organizer abre o modelo de orçamento do funil.
2. Cria ou edita nome, cores, texto da proposta e parâmetros disponíveis, inclusive o token `#nome_oportunidade` e os grupos de datas, horários, locais, quantidades e valor.
3. Salva o modelo na conta.
4. Ao criar orçamento na SPEC-025, escolhe o modelo; o sistema copia o texto e substitui `#nome_oportunidade` pelo nome da oportunidade quando houver.
5. Exclui modelo que não queira mais na biblioteca.

## 7. Fluxos alternativos

- Conta sem modelo: a lista ou a tela padrão indica vazio e permite criar o primeiro.
- Nome ou texto vazios: não grava.
- Modelo padrão somente leitura da referência externa não é regra deste produto: o Organizer edita os próprios modelos.
- Aplicar modelo sem oportunidade selecionada: a SPEC-025 impede gravar o orçamento.

## 8. Regras de negócio

- Modelo pertence à conta do Organizer.
- Distinto da lista de orçamentos (SPEC-025), do budget do evento e do orçamento de fornecedor.
- Token mínimo suportado: `#nome_oportunidade`. Demais parâmetros dos grupos datas, horários, locais, quantidades e valor entram quando o Organizer os incluir no texto.
- Excluir o modelo não apaga orçamentos já gerados.

## 9. Validações

Nome obrigatório. Texto da proposta obrigatório. Cores, se informadas, em formato válido da tela.

## 10. Estados

Modelo cadastrado ou excluído.

## 11. Dados necessários

Nome, cores, texto da proposta, parâmetros/tokens utilizados.

## 12. Interface

Tela de biblioteca/editor do modelo de orçamento do funil, com pré-visualização do texto e parâmetros. Angular Material, DA-044.

## 13. Erros

Campo inválido indica o campo. Item de outra conta fica indisponível. Falha ao gravar mantém o modelo anterior.

## 14. Critérios de aceite

1. Criar e editar modelo grava na própria conta.
2. Sem nome ou sem texto, nada é gravado.
3. Aplicar o modelo na SPEC-025 copia o texto e substitui `#nome_oportunidade`.
4. Excluir remove o modelo da biblioteca e não apaga orçamentos já criados.
5. Outra conta não vê o modelo.
6. A tela permanece distinta da lista de orçamentos.

## 15. Cenários de teste

Criar, editar, excluir. Aplicar em orçamento com oportunidade nomeada. Duas contas. Tentativa sem nome.

## 16. Dependências

SPEC-001, SPEC-020 e SPEC-025.

## 17. Premissas e pontos não definidos

Lista fechada de todos os tokens além de `#nome_oportunidade` e dos grupos já nomeados.
