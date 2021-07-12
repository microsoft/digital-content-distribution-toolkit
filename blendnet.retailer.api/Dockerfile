#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["blendnet.retailer.api/blendnet.retailer.api.csproj", "blendnet.retailer.api/"]
COPY ["blendnet.cms.repository/blendnet.cms.repository.csproj", "blendnet.cms.repository/"]
COPY ["blendnet.common.dto/blendnet.common.dto.csproj", "blendnet.common.dto/"]
COPY ["blendnet.common.infrastructure/blendnet.common.infrastructure.csproj", "blendnet.common.infrastructure/"]
RUN dotnet restore "blendnet.retailer.api/blendnet.retailer.api.csproj"
COPY . .
WORKDIR "/src/blendnet.retailer.api"
RUN dotnet build "blendnet.retailer.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "blendnet.retailer.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blendnet.retailer.api.dll"]