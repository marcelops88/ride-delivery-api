# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copia o arquivo .csproj e restaura as dependências
COPY src/API/*.csproj ./src/API/
RUN dotnet restore ./src/API

# Copia o restante dos arquivos do projeto
COPY ./src ./src

# Faz o build da aplicação
WORKDIR /app/src/API
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Imagem para execução
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Porta que a aplicação irá expor
EXPOSE 5000

# Variável de ambiente para produção
ENV ASPNETCORE_ENVIRONMENT=Production

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "API.dll"]
