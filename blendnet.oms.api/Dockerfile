#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-cbl-mariner1.0-distroless AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0-cbl-mariner1.0 AS build
WORKDIR /src
COPY ["blendnet.oms.api/blendnet.oms.api.csproj", "blendnet.oms.api/"]
COPY ["blendnet.common.dto/blendnet.common.dto.csproj", "blendnet.common.dto/"]
COPY ["blendnet.common.infrastructure/blendnet.common.infrastructure.csproj", "blendnet.common.infrastructure/"]
COPY ["blendnet.api.proxy/blendnet.api.proxy.csproj", "blendnet.api.proxy/"]
COPY ["blendnet.oms.repository/blendnet.oms.repository.csproj", "blendnet.oms.repository/"]
RUN dotnet restore "blendnet.oms.api/blendnet.oms.api.csproj"
COPY . .
WORKDIR "/src/blendnet.oms.api"
RUN dotnet build "blendnet.oms.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "blendnet.oms.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "blendnet.oms.api.dll"]