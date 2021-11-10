#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-cbl-mariner1.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-cbl-mariner1.0 AS build
WORKDIR /src
COPY ["blendnet.cms.listener/blendnet.cms.listener.csproj", "blendnet.cms.listener/"]
COPY ["blendnet.cms.repository/blendnet.cms.repository.csproj", "blendnet.cms.repository/"]
COPY ["blendnet.common.dto/blendnet.common.dto.csproj", "blendnet.common.dto/"]
COPY ["blendnet.common.infrastructure/blendnet.common.infrastructure.csproj", "blendnet.common.infrastructure/"]
COPY ["blendnet.api.proxy/blendnet.api.proxy.csproj", "blendnet.api.proxy/"]
COPY ["MPDParser/MPDParser.csproj", "MPDParser/"]
RUN dotnet restore "blendnet.cms.listener/blendnet.cms.listener.csproj"
COPY . .
WORKDIR "/src/blendnet.cms.listener"
RUN dotnet build "blendnet.cms.listener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "blendnet.cms.listener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blendnet.cms.listener.dll"]