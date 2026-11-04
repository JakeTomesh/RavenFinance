
# Stage 1: Build RavenFinance
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy the project file and restore dependencies
COPY RavenFinance.csproj ./
RUN dotnet restore

# Copy application source code
COPY . .

# Publish RavenFinance
RUN dotnet publish RavenFinance.csproj -c Release -o /app/publish --no-restore



# Stage 2: Run RavenFinance
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "RavenFinance.dll"]
