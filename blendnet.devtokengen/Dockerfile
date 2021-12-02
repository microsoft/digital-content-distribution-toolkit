FROM mcr.microsoft.com/dotnet/sdk:6.0-cbl-mariner1.0 AS publish
WORKDIR /src
COPY . .
RUN dotnet build "blendnet.devtokengen.csproj" -c Release -o /app/build
RUN dotnet publish "blendnet.devtokengen.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0-cbl-mariner1.0-distroless AS final

WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "blendnet.devtokengen.dll"]
