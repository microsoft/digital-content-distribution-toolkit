#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["blendnet.notification.api/blendnet.notification.api.csproj", "blendnet.notification.api/"]
COPY ["blendnet.api.proxy/blendnet.api.proxy.csproj", "blendnet.api.proxy/"]
COPY ["blendnet.common.dto/blendnet.common.dto.csproj", "blendnet.common.dto/"]
COPY ["blendnet.common.infrastructure/blendnet.common.infrastructure.csproj", "blendnet.common.infrastructure/"]
RUN dotnet restore "blendnet.notification.api/blendnet.notification.api.csproj"
COPY . .
WORKDIR "/src/blendnet.notification.api"
RUN dotnet build "blendnet.notification.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "blendnet.notification.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blendnet.notification.api.dll"]