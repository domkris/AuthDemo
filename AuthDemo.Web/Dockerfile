#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["AuthDemo.Web/AuthDemo.Web.csproj", "AuthDemo.Web/"]
COPY ["AuthDemo.Contracts/AuthDemo.Contracts.csproj", "AuthDemo.Contracts/"]
COPY ["AuthDemo.Domain/AuthDemo.Domain.csproj", "AuthDemo.Domain/"]
COPY ["AuthDemo.Infrastructure/AuthDemo.Infrastructure.csproj", "AuthDemo.Infrastructure/"]
COPY ["AuthDemo.Security/AuthDemo.Security.csproj", "AuthDemo.Security/"]
COPY ["AuthDemo.Cache/AuthDemo.Cache.csproj", "AuthDemo.Cache/"]
RUN dotnet restore "./AuthDemo.Web/./AuthDemo.Web.csproj"
COPY . .
WORKDIR "/src/AuthDemo.Web"
RUN dotnet build "./AuthDemo.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "./AuthDemo.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthDemo.Web.dll"]