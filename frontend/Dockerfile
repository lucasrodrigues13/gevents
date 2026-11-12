FROM node:22-alpine AS build
WORKDIR /app
COPY frontend/ /app/
RUN if [ -f package.json ]; then \
      corepack enable; \
      if [ -f pnpm-lock.yaml ]; then \
        corepack prepare pnpm@10 --activate && pnpm install --frozen-lockfile && pnpm build; \
      elif [ -f package-lock.json ]; then \
        npm ci && npm run build; \
      else \
        npm install && npm run build; \
      fi; \
    else \
      mkdir -p /app/dist/browser; \
    fi

FROM caddy:2-alpine
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=build /app/dist /tmp/dist
RUN set -eu; \
    src="$(find /tmp/dist -type d -name browser | head -n 1)"; \
    if [ -z "$src" ]; then src=/tmp/dist; fi; \
    mkdir -p /srv/frontend; \
    cp -a "$src"/. /srv/frontend/
EXPOSE 80
