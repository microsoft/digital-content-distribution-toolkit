#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-cbl-mariner1.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-cbl-mariner1.0 AS build
WORKDIR /src
COPY ["blendnet.device.listener/blendnet.device.listener.csproj", "blendnet.device.listener/"]
COPY ["blendnet.device.repository/blendnet.device.repository.csproj", "blendnet.device.repository/"]
COPY ["blendnet.common.dto/blendnet.common.dto.csproj", "blendnet.common.dto/"]
COPY ["blendnet.common.infrastructure/blendnet.common.infrastructure.csproj", "blendnet.common.infrastructure/"]
COPY ["blendnet.api.proxy/blendnet.api.proxy.csproj", "blendnet.api.proxy/"]
RUN dotnet restore "blendnet.device.listener/blendnet.device.listener.csproj"
COPY . .
WORKDIR "/src/blendnet.device.listener"
RUN dotnet build "blendnet.device.listener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "blendnet.device.listener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blendnet.device.listener.dll"]