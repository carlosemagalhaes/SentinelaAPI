# ============================================================
# Stage 1 — restore
# Copia apenas os .csproj e restaura dependências em camada
# separada para aproveitar o cache do Docker quando o código
# muda mas as dependências não mudam.
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

COPY SentinelaAPI/SentinelaAPI.Domain/SentinelaAPI.Domain.csproj           SentinelaAPI.Domain/
COPY SentinelaAPI/SentinelaAPI.Application/SentinelaAPI.Application.csproj SentinelaAPI.Application/
COPY SentinelaAPI/SentinelaAPI.Infrastructure/SentinelaAPI.Infrastructure.csproj SentinelaAPI.Infrastructure/
COPY SentinelaAPI/SentinelaAPI.Api/SentinelaAPI.Api.csproj                 SentinelaAPI.Api/
COPY SentinelaAPI/SentinelaAPI.Tests/SentinelaAPI.Tests.csproj             SentinelaAPI.Tests/

RUN dotnet restore SentinelaAPI.Api/SentinelaAPI.Api.csproj

# ============================================================
# Stage 2 — build e publish
# Copia o código-fonte, builda em Release e publica em /app/publish.
# ============================================================
FROM restore AS build
WORKDIR /src

COPY SentinelaAPI/SentinelaAPI.Domain/           SentinelaAPI.Domain/
COPY SentinelaAPI/SentinelaAPI.Application/      SentinelaAPI.Application/
COPY SentinelaAPI/SentinelaAPI.Infrastructure/   SentinelaAPI.Infrastructure/
COPY SentinelaAPI/SentinelaAPI.Api/              SentinelaAPI.Api/
COPY SentinelaAPI/SentinelaAPI.Tests/            SentinelaAPI.Tests/

RUN dotnet publish SentinelaAPI.Api/SentinelaAPI.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ============================================================
# Stage 3 — runtime (imagem final)
# Usa apenas o runtime (muito menor que o SDK — ~220 MB vs ~750 MB).
# Não tem compilador, não tem dotnet CLI: superfície de ataque reduzida.
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Boa prática de segurança: não rodar como root dentro do container.
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

WORKDIR /app

COPY --from=build /app/publish .

# Garante que apenas o dono do processo pode ler os arquivos da aplicação.
RUN chown -R appuser:appgroup /app

USER appuser

# Porta padrão do ASP.NET Core em containers.
EXPOSE 8080

# Variáveis de ambiente seguras para produção.
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "SentinelaAPI.Api.dll"]
