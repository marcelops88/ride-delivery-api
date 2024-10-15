# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copia os arquivos do projeto e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todo o restante e faz o build da aplicação
COPY . ./
RUN dotnet publish -c Release -o out

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
