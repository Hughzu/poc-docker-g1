FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app
EXPOSE 7230
ENV ASPNETCORE_URLS=http://+:7230

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY Docker.sln .
COPY ["/Docker.Domain/Docker.Domain.csproj", "Docker.Domain/"]
COPY ["/Docker.Data/Docker.Data.csproj", "Docker.Data/"]
COPY ["/Docker.Logic/Docker.Logic.csproj", "Docker.Logic/"]
COPY ["/Docker.WebApi/Docker.WebApi.csproj", "Docker.WebApi/"]
RUN dotnet restore
COPY . ./
RUN dotnet build "Docker.WebApi/Docker.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Docker.WebApi/Docker.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Docker.WebApi.dll"]