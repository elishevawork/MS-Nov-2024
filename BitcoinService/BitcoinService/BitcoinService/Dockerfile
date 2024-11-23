# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY . ./

# Publish the app in Release mode to the 'out' directory
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
#EXPOSE 5001
WORKDIR /app
# Copy the published app from the build stage
COPY --from=build-env /app/out .

# Entry point for the Worker Service
ENTRYPOINT [ "dotnet", "BitcoinService.dll" ]
