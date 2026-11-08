---
name: feature-implementation
description: Implementa uma funcionalidade do gevents já coberta por spec e plano aprovados. Use quando o usuário pedir para implementar, codar ou testar uma feature aprovada. Não amplia o escopo.
---

# Implementação

## Antes de codar

Exija spec aprovada e plano aprovado. Se faltar um dos dois, pare e diga qual.

## Faça

1. Implemente só o que os dois documentos cobrem.
2. Siga `.cursor/rules/project-architecture.mdc` e `.cursor/rules/coding-standards.mdc`.
3. Escreva o teste que o plano pediu. Unitário sem banco. Integração com Testcontainers quando o plano incluir.
4. Confira os critérios de aceite um a um.
5. Resuma arquivos alterados, testes executados e o que ficou pendente. Não invente o comportamento que faltou.

## Não faça

- Não altere RF aprovado para acomodar o código.
- Não rode migration em produção nem faça deploy.
