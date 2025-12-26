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
RUN dotnet publish GlashAgentWorker/GlashAgentWorker.csproj -c Release -o /app --no-restore

# Use multi-platform aware runtime image
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Set default timezone to Shanghai
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Set default language to Chinese
ENV LANG=zh_CN.UTF-8
ENV LANGUAGE=zh_CN:zh
ENV LC_ALL=zh_CN.UTF-8

# Set default environment variables
ENV GLASH_SERVER_URL=qp.wss://my_galsh_server.cnb.run/glash?Password=5a79cf67400c48b3afcf33d0e23e5541
ENV GLASH_AGENT_NAME=agent
ENV GLASH_AGENT_PASSWORD=agent

# Support passing additional arguments to the application
ENTRYPOINT ["dotnet", "GlashAgentWorker.dll"]
