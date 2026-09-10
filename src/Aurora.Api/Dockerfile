FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Aurora.Api/Aurora.Api.csproj", "src/Aurora.Api/"]
COPY ["src/Aurora.Application/Aurora.Application.csproj", "src/Aurora.Application/"]
COPY ["src/Aurora.Infrastructure/Aurora.Infrastructure.csproj", "src/Aurora.Infrastructure/"]
COPY ["src/Aurora.Domain/Aurora.Domain.csproj", "src/Aurora.Domain/"]
COPY ["src/Aurora.Contracts/Aurora.Contracts.csproj", "src/Aurora.Contracts/"]

RUN dotnet restore "src/Aurora.Api/Aurora.Api.csproj"

COPY . .
RUN dotnet publish "src/Aurora.Api/Aurora.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aurora.Api.dll"]
