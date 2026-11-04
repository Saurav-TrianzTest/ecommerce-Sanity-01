# =============================================================================
# Stage 1: Build Stage
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

WORKDIR /src

# Copy project file first for dependency caching
COPY ["EcommerceWebApi.csproj", "./"]

# Restore NuGet dependencies
RUN dotnet restore "EcommerceWebApi.csproj"

# Copy remaining source code
COPY . .

# Build and publish in Release configuration
RUN dotnet publish "EcommerceWebApi.csproj" -c Release -o /app/publish --no-restore

# =============================================================================
# Stage 2: Runtime Stage
# =============================================================================
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime

WORKDIR /app

# Create non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup --no-create-home appuser

# Copy published artifacts from build stage
COPY --from=builder /app/publish .

# Create Logs directory and set permissions
RUN mkdir -p /app/Logs && \
    chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Set .NET environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expose application port (Kestrel configured via SIGNALR_PORT env var, default 5000)
EXPOSE 5000

# Entry point
ENTRYPOINT ["dotnet", "EcommerceWebApi.dll"]
