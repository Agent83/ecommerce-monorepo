# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the UserService project files into the container
COPY ["Services/UserService/UserService.csproj", "UserService/"]

# Restore the project
RUN dotnet restore "UserService/UserService.csproj"

# Copy all the files from the UserService directory
COPY Services/UserService/ ./UserService/

# Set the working directory to the UserService folder
WORKDIR /src/UserService

# Build the project
RUN dotnet build "UserService.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish

# Final stage, copy the built app to the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.dll"]
