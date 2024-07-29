# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the RealEstate project file and restore dependencies
COPY RealState.csproj .
RUN dotnet restore

# Copy the remaining files and build the RealEstate project
COPY . .
RUN dotnet publish -c Release -o /app/build


# Stage 2: Setup runtime environment and download dependencies
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the built files from the build stage
COPY --from=build /app/build .

# Install dotnet-ef tool in runtime container as well
RUN dotnet tool install --global dotnet-ef

# Expose port
EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "RealState.dll"]
