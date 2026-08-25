# Use multi-platform aware base images
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETPLATFORM
ARG BUILDPLATFORM

WORKDIR /src

# Copy project files first for better layer caching
COPY Glash/Glash.csproj Glash/
COPY Glash.Blazor.Agent/Glash.Blazor.Agent.csproj Glash.Blazor.Agent/
COPY GlashAgent/GlashAgent.csproj GlashAgent/

# Restore dependencies first for better caching
RUN dotnet restore GlashAgent/GlashAgent.csproj

# Copy the entire source code
COPY Glash/ Glash/
COPY Glash.Blazor.Agent/ Glash.Blazor.Agent/
COPY GlashAgent/ GlashAgent/

# Build the project for the target platform
RUN dotnet publish GlashAgent/GlashAgent.csproj -c Release -o /app --no-restore

# Use multi-platform aware runtime image
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Set default timezone to Shanghai
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Set default language to Chinese
ENV LANG=zh_CN.UTF-8
ENV LANGUAGE=zh_CN:zh
ENV LC_ALL=zh_CN.UTF-8

# Set default ASP.NET Core environment
ENV ASPNETCORE_ENVIRONMENT=Production
# Set default listening port
ENV HTTP_PORTS=6002
# Set default database file path
ENV GLASH_DB_FILE_PATH=Config.litedb
# Set default admin password (empty)
ENV GLASH_ADMIN_PASSWORD=""

# Support passing additional arguments to the application
ENTRYPOINT ["dotnet", "GlashAgent.dll"]
