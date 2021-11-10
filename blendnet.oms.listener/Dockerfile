#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-cbl-mariner1.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-cbl-mariner1.0 AS build
WORKDIR /src
COPY ["blendnet.oms.listener/blendnet.oms.listener.csproj", "blendnet.oms.listener/"]
COPY ["blendnet.common.dto/blendnet.common.dto.csproj", "blendnet.common.dto/"]
COPY ["blendnet.common.infrastructure/blendnet.common.infrastructure.csproj", "blendnet.common.infrastructure/"]
COPY ["blendnet.api.proxy/blendnet.api.proxy.csproj", "blendnet.api.proxy/"]
RUN dotnet restore "blendnet.oms.listener/blendnet.oms.listener.csproj"
COPY . .
WORKDIR "/src/blendnet.oms.listener"
RUN dotnet build "blendnet.oms.listener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "blendnet.oms.listener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blendnet.oms.listener.dll"]