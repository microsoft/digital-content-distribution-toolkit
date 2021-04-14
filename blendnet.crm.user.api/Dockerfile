#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["blendnet.crm.user.api/blendnet.crm.user.api.csproj", "blendnet.crm.user.api/"]
COPY ["blendnet.common.infrastructure/blendnet.common.infrastructure.csproj", "blendnet.common.infrastructure/"]
COPY ["blendnet.common.dto/blendnet.common.dto.csproj", "blendnet.common.dto/"]
RUN dotnet restore "blendnet.crm.user.api/blendnet.crm.user.api.csproj"
COPY . .
WORKDIR "/src/blendnet.crm.user.api"
RUN dotnet build "blendnet.crm.user.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "blendnet.crm.user.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blendnet.crm.user.api.dll"]