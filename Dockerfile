FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY . .
RUN dotnet publish MyApp.csproj -c Release -o publish

FROM base AS final
WORKDIR /app

ENV STORAGE_ACCOUNT_NAME=
ENV FUNCTION_NAME=

COPY --from=build /src/publish .
ENTRYPOINT ["dotnet", "WebAppBlob.dll"]