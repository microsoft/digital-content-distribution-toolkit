#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["blendnet.incentive.listener/blendnet.incentive.listener.csproj", "blendnet.incentive.listener/"]
COPY ["blendnet.incentive.repository/blendnet.incentive.repository.csproj", "blendnet.incentive.repository/"]
COPY ["blendnet.common.dto/blendnet.common.dto.csproj", "blendnet.common.dto/"]
COPY ["blendnet.common.infrastructure/blendnet.common.infrastructure.csproj", "blendnet.common.infrastructure/"]
RUN dotnet restore "blendnet.incentive.listener/blendnet.incentive.listener.csproj"
COPY . .
WORKDIR "/src/blendnet.incentive.listener"
RUN dotnet build "blendnet.incentive.listener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "blendnet.incentive.listener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blendnet.incentive.listener.dll"]