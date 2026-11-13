# gevents

API .NET e interface Angular do mesmo produto. O Caddy publica o build estático e encaminha `/api`.

## Onde fica cada coisa

```text
src/frontend/                 Angular
src/backend/gevents.slnx      solution só do backend
src/backend/Project.Api/      API
src/backend/Project.Application/
src/backend/Project.Domain/
src/backend/Project.Infrastructure/
tests/                         testes da API, incluídos na solution
deploy/                        script SQL da base local
docs/                          requisitos e specs
```

Abra `src/backend/gevents.slnx` no Visual Studio. Ela não inclui o Angular. Os testes ficam fora de `src/`, separados da produção.

## Subir local

```text
just up
```

Abre em `http://localhost:8080`. `just down` encerra os containers.

A senha do PostgreSQL e o segredo do JWT no `compose.yaml` valem só para essa rede local.
