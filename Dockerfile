# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["PathologyFormApp/PathologyFormApp.csproj", "PathologyFormApp/"]
RUN dotnet restore "PathologyFormApp/PathologyFormApp.csproj"

# Copy all source files
COPY . .
WORKDIR "/src/PathologyFormApp"

# Build application
RUN dotnet build "PathologyFormApp.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "PathologyFormApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN groupadd -r pathologyapp && useradd -r -g pathologyapp pathologyapp

# Create directories for uploads and logs
RUN mkdir -p /app/wwwroot/uploads /app/logs && \
    chown -R pathologyapp:pathologyapp /app

# Copy published application
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R pathologyapp:pathologyapp /app

# Switch to non-root user
USER pathologyapp

# Expose ports
EXPOSE 80
EXPOSE 443

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Entry point
ENTRYPOINT ["dotnet", "PathologyFormApp.dll"]
