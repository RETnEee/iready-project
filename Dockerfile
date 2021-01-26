#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
Copy . .
WORKDIR src/iready/iready.api

RUN dotnet restore
RUN dotnet build -c Release -o /app

FROM build as publish
RUN dotnet publish -c Releease -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "iready.api.dll"]
