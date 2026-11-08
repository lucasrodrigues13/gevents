FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Project.Domain/Project.Domain.csproj src/Project.Domain/
COPY src/Project.Application/Project.Application.csproj src/Project.Application/
COPY src/Project.Infrastructure/Project.Infrastructure.csproj src/Project.Infrastructure/
COPY src/Project.Web/Project.Web.csproj src/Project.Web/
RUN dotnet restore src/Project.Web/Project.Web.csproj
COPY src/ src/
RUN dotnet publish src/Project.Web/Project.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM build AS migrate
RUN dotnet tool install --global dotnet-ef --version 10.0.14
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT ["dotnet", "ef", "database", "update", "--project", "src/Project.Infrastructure/Project.Infrastructure.csproj", "--startup-project", "src/Project.Web/Project.Web.csproj", "--configuration", "Release"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY deploy/entrypoint.sh /entrypoint.sh
USER root
RUN sed -i 's/\r$//' /entrypoint.sh \
    && chmod +x /entrypoint.sh \
    && mkdir -p /storage /keys \
    && chown -R app:app /app /storage /keys
EXPOSE 8080
ENTRYPOINT ["/entrypoint.sh"]
