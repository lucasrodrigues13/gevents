---
name: requirements-analysis
description: Consolida requisitos do gevents a partir de docs/brief.md e docs/requirements.md, separando confirmado, observado, proposto e pendente. Use quando o usuário pedir levantamento, consolidação de RF ou atualização de requisitos sem nova entrevista.
---

# Análise de requisitos

## Faça

1. Leia `docs/requirements.md` e `docs/brief.md`. Não abra o site de referência de novo.
2. Liste só funcionalidade que já está escrita. Marque a fonte.
3. Grave RF novo com o próximo ID livre. Não renumere.
4. Use Observado para a referência, Proposto para sugestão, Pendente quando faltar dado, Definido só para o que o usuário já aprovou.
5. Aponte conflito com roles `Admin`/`Organizer`/`Client`, e-mail, arquivo ou backup. Não resolva sozinho.

## Não faça

- Não invente campo obrigatório, estado ou permissão.
- Não transforme o recorte do brief em escopo aprovado.
- Não escreva spec nem código nesta skill.
