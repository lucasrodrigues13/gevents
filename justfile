set windows-shell := ["powershell.exe", "-NoLogo", "-Command"]

default:
    @just --list

# Sobe a API, o frontend e o PostgreSQL no Docker.
up:
    docker compose up --build

# Encerra os containers desse compose.
down:
    docker compose down
