---
name: feature-implementation
description: Implementa uma funcionalidade do gevents já coberta por spec aprovada. Use quando o usuário pedir para implementar, codar ou testar uma feature aprovada. Não amplia o escopo.
---

# Implementação

## Antes de codar

Exija spec aprovada. Se a spec estiver em rascunho ou não existir, pare e diga qual. Não peça plano técnico.

## Faça

1. Implemente só o que a spec cobre.
2. Siga `.cursor/rules/project-architecture.mdc` e `.cursor/rules/coding-standards.mdc`. Na API, siga `.cursor/skills/backend/SKILL.md`. Na interface, siga `.cursor/skills/frontend/SKILL.md`.
3. Escreva o teste dos critérios de aceite e dos cenários da spec. Unitário sem banco. Integração com Testcontainers quando o fluxo passa por banco, HTTP ou arquivo. Teste de tela quando a spec descreve a interface.
4. Confira os critérios de aceite um a um.
5. Resuma arquivos alterados, testes executados e o que ficou pendente. Não invente o comportamento que faltou.

## Não faça

- Não altere RF aprovado para acomodar o código.
- Não rode migration em produção nem faça deploy.
