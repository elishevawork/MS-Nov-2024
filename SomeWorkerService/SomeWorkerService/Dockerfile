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
#EXPOSE 8080
WORKDIR /app
COPY --from=build-env /app/out .

#ENV ASPNETCORE_ENVIRONMENT=Production

#EXPOSE 8080
ENTRYPOINT [ "dotnet", "SomeWorkerService.dll" ]
