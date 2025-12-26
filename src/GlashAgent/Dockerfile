# Use multi-platform aware base images
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETPLATFORM
ARG BUILDPLATFORM

WORKDIR /src

# Copy the entire source code into the container
COPY src/ .

# Restore dependencies first for better caching
RUN dotnet restore

# Build the project for the target platform
RUN dotnet publish GlashAgent/GlashAgent.csproj -c Release -o /app --no-restore

# Use multi-platform aware runtime image
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Set default timezone to Shanghai
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Set default ASP.NET Core environment
ENV ASPNETCORE_ENVIRONMENT=Production
# Set default listening port
ENV HTTP_PORTS=6002
# Set default database file path
ENV GLASH_DB_FILE_PATH=Config.litedb

# Support passing additional arguments to the application
ENTRYPOINT ["dotnet", "GlashAgent.dll"]
