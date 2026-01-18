FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj files
COPY TubixChat.DataLayer/TubixChat.DataLayer.csproj TubixChat.DataLayer/
COPY TubixChat.BizLogicLayer/TubixChat.BizLogicLayer.csproj TubixChat.BizLogicLayer/
COPY TubixChat.Server/TubixChat.Server.csproj TubixChat.Server/

# Restore
RUN dotnet restore TubixChat.Server/TubixChat.Server.csproj

# Copy source code (TO‘G‘RI JOYLASHUV)
COPY TubixChat.DataLayer TubixChat.DataLayer/
COPY TubixChat.BizLogicLayer TubixChat.BizLogicLayer/
COPY TubixChat.Server TubixChat.Server/

# Publish
RUN dotnet publish TubixChat.Server/TubixChat.Server.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =====================
# Runtime
# =====================
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "TubixChat.Server.dll"]
