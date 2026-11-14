#!/usr/bin/env node
/**
 * Auto-allow local build/test/run commands for gevents agents.
 * Destructive git/deploy still asks.
 */
import { stdin } from 'node:process';

const chunks = [];
for await (const chunk of stdin) {
  chunks.push(chunk);
}

let input = {};
try {
  input = JSON.parse(Buffer.concat(chunks).toString('utf8') || '{}');
} catch {
  input = {};
}

const command = String(input.command ?? input.cmd ?? '');
const lower = command.toLowerCase();

const deny =
  /git\s+push\s+--force|git\s+reset\s+--hard|rm\s+-rf\s+[\/\\]|docker\s+compose\s+down\s+-v|drop\s+database|format\s+c:/.test(
    lower,
  );

if (deny) {
  process.stdout.write(
    JSON.stringify({
      permission: 'ask',
      user_message: 'Comando destrutivo — confirme antes de continuar.',
      agent_message: 'Hook pediu revisão humana para comando potencialmente destrutivo.',
    }),
  );
  process.exit(0);
}

const allow =
  /\b(dotnet|npm|npx|ng|node|tsc|vitest|docker|git\s+(status|diff|log|add|commit|branch|checkout|fetch|pull)|pwsh|powershell)\b/.test(
    lower,
  ) ||
  /\b(test|build|restore|publish|serve|compose\s+up|compose\s+build|ef\s+migrations)\b/.test(lower);

process.stdout.write(
  JSON.stringify({
    permission: allow ? 'allow' : 'ask',
  }),
);
process.exit(0);
