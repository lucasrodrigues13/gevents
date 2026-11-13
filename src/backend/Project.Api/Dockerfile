FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY nuget.config ./
COPY src/backend/Project.Domain/Project.Domain.csproj src/backend/Project.Domain/
COPY src/backend/Project.Application/Project.Application.csproj src/backend/Project.Application/
COPY src/backend/Project.Infrastructure/Project.Infrastructure.csproj src/backend/Project.Infrastructure/
COPY src/backend/Project.Api/Project.Api.csproj src/backend/Project.Api/
RUN dotnet restore src/backend/Project.Api/Project.Api.csproj
COPY src/backend/ src/backend/
RUN dotnet publish src/backend/Project.Api/Project.Api.csproj -c Release -o /app --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Project.Api.dll"]
