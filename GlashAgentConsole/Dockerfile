# Use multi-platform aware base images
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETPLATFORM
ARG BUILDPLATFORM

WORKDIR /src

# Copy project files first for better layer caching
COPY Glash/Glash.csproj Glash/
COPY GlashAgentConsole/GlashAgentConsole.csproj GlashAgentConsole/

# Restore dependencies first for better caching
RUN dotnet restore GlashAgentConsole/GlashAgentConsole.csproj

# Copy the entire source code
COPY Glash/ Glash/
COPY GlashAgentConsole/ GlashAgentConsole/

# Build the project for the target platform
RUN dotnet publish GlashAgentConsole/GlashAgentConsole.csproj \
    -c Release \
    -o /app \
    --no-restore \
    -p:DebugType=None \
    -p:DebugSymbols=false \
    -p:PublishTrimmed=true

# Use multi-platform aware runtime image
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Set default timezone and language in one layer
ENV TZ=Asia/Shanghai \
    LANG=zh_CN.UTF-8 \
    LANGUAGE=zh_CN:zh \
    LC_ALL=zh_CN.UTF-8
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Environment variables (no defaults - must be provided)
ENV GLASH_SERVER_URL=""
ENV GLASH_AGENT_NAME=""
ENV GLASH_AGENT_PASSWORD=""

ENTRYPOINT ["dotnet", "GlashAgentConsole.dll"]
