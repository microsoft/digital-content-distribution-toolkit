FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["blendnet.crm.contentprovider.api/blendnet.crm.contentprovider.api.csproj", "blendnet.crm.contentprovider.api/"]
RUN dotnet restore "blendnet.crm.contentprovider.api/blendnet.crm.contentprovider.api.csproj"
COPY . .
WORKDIR "/src/blendnet.crm.contentprovider.api"
RUN dotnet build "blendnet.crm.contentprovider.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "blendnet.crm.contentprovider.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blendnet.crm.contentprovider.api.dll"]
