FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY AkkaIoTIngress/AkkaIoTIngress.csproj AkkaIoTIngress/
RUN dotnet restore AkkaIoTIngress/AkkaIoTIngress.csproj
COPY . .
WORKDIR /src/AkkaIoTIngress
RUN dotnet build AkkaIoTIngress.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AkkaIoTIngress.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AkkaIoTIngress.dll"]
