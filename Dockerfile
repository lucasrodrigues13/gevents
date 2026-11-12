FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY nuget.config ./
COPY src/Project.Domain/Project.Domain.csproj src/Project.Domain/
COPY src/Project.Application/Project.Application.csproj src/Project.Application/
COPY src/Project.Infrastructure/Project.Infrastructure.csproj src/Project.Infrastructure/
COPY src/Project.Api/Project.Api.csproj src/Project.Api/
RUN dotnet restore src/Project.Api/Project.Api.csproj
COPY src/ src/
RUN dotnet publish src/Project.Api/Project.Api.csproj -c Release -o /app --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Project.Api.dll"]
