# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY TubixChat.DataLayer/TubixChat.DataLayer.csproj TubixChat.DataLayer/
COPY TubixChat.BizLogicLayer/TubixChat.BizLogicLayer.csproj TubixChat.BizLogicLayer/
COPY TubixChat.Server/TubixChat.Server.csproj TubixChat.Server/

# Restore dependencies for Server project (it will restore dependencies automatically)
WORKDIR /src/TubixChat.Server
RUN dotnet restore

# Copy all source files
COPY . ./

# Clean obj folders before build to avoid duplicate attribute errors
RUN find . -type d -name "obj" -exec rm -rf {} + 2>/dev/null || true
RUN find . -type d -name "bin" -exec rm -rf {} + 2>/dev/null || true

# Build the application
WORKDIR /src/TubixChat.Server
RUN dotnet build TubixChat.Server.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
WORKDIR /src/TubixChat.Server
RUN dotnet publish TubixChat.Server.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Expose port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

# Run the application
ENTRYPOINT ["dotnet", "TubixChat.Server.dll"]
