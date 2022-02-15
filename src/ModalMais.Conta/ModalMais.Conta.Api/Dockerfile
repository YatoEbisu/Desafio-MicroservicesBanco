#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ModalMais.Conta/ModalMais.Conta.Api/ModalMais.Conta.Api.csproj", "ModalMais.Conta/ModalMais.Conta.Api/"]
COPY ["ModalMais.Conta/ModalMais.Conta.Service/ModalMais.Conta.Service.csproj", "ModalMais.Conta/ModalMais.Conta.Service/"]
COPY ["ModalMais.Conta/ModalMais.Conta.Domain/ModalMais.Conta.Domain.csproj", "ModalMais.Conta/ModalMais.Conta.Domain/"]
COPY ["ModalMais.Conta/ModalMais.Conta.Infra.Data/ModalMais.Conta.Infra.Data.csproj", "ModalMais.Conta/ModalMais.Conta.Infra.Data/"]
RUN dotnet restore "ModalMais.Conta/ModalMais.Conta.Api/ModalMais.Conta.Api.csproj"
COPY . .
WORKDIR "ModalMais.Conta/ModalMais.Conta.Api"
RUN dotnet build "ModalMais.Conta.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ModalMais.Conta.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ModalMais.Conta.Api.dll"]