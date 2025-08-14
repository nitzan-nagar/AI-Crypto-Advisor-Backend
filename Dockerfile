FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AI.CryptoAdvisor.Api.csproj ./
RUN dotnet restore AI.CryptoAdvisor.Api.csproj

COPY AI.CryptoAdvisor.Api/. ./
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["dotnet", "AI.CryptoAdvisor.Api.dll"]
