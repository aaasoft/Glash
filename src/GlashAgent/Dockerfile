FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the entire source code into the container
COPY .. ..

# Build the project
RUN dotnet publish GlashAgent.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Set default timezone to Shanghai
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Set default ASP.NET Core environment
ENV ASPNETCORE_ENVIRONMENT=Production
# Set default listening port
ENV ASPNETCORE_URLS=http://+:6002
# Set default database file path
ENV GLASH_DB_FILE_PATH=Config.litedb

# Support passing additional arguments to the application
ENTRYPOINT ["dotnet", "GlashAgent.dll"]
