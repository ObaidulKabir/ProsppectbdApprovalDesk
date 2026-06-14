# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and csproj files for restoring dependencies
COPY ProspectbdApprovalDesk.sln ./
COPY src/ProspectbdApprovalDesk.Api/ProspectbdApprovalDesk.Api.csproj src/ProspectbdApprovalDesk.Api/
COPY src/ProspectbdApprovalDesk.Application/ProspectbdApprovalDesk.Application.csproj src/ProspectbdApprovalDesk.Application/
COPY src/ProspectbdApprovalDesk.Domain/ProspectbdApprovalDesk.Domain.csproj src/ProspectbdApprovalDesk.Domain/
COPY src/ProspectbdApprovalDesk.Infrastructure/ProspectbdApprovalDesk.Infrastructure.csproj src/ProspectbdApprovalDesk.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy all source files
COPY src/ src/

# Build and publish
WORKDIR /app/src/ProspectbdApprovalDesk.Api
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ProspectbdApprovalDesk.Api.dll"]
