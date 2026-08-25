# Docker 部署指南

## 概述

Glash 项目提供了四个 Docker 镜像，分别用于部署服务端、客户端和代理端。

## 镜像说明

| 镜像名称 | 用途 | 默认端口 |
|---------|------|---------|
| `glash-server-web` | Glash 服务端 | 6000 |
| `glash-client-web` | Glash 客户端 | 6001 |
| `glash-agent-web` | Glash 代理端 | 6002 |
| `glash-agent-console` | Glash 代理端（控制台版，无 UI） | - |

## 快速开始

### 1. 配置 GitHub Secrets

在 GitHub 仓库的 Settings > Secrets and variables > Actions 中添加以下 secrets：

- `DOCKERHUB_USERNAME`: Docker Hub 用户名
- `DOCKERHUB_TOKEN`: Docker Hub 访问令牌

### 2. 构建和发布

当代码推送到 `main` 分支时，GitHub Actions 会自动构建并发布 Docker 镜像。

### 3. 拉取镜像

```bash
# 拉取服务端镜像
docker pull aaasoft/glash-server-web:main

# 拉取客户端镜像
docker pull aaasoft/glash-client-web:main

# 拉取代理端镜像
docker pull aaasoft/glash-agent-web:main

# 拉取代理端控制台镜像
docker pull aaasoft/glash-agent-console:main
```

## 运行容器

### 服务端

```bash
docker run -d \
  --name glash-server-web \
  -p 6000:6000 \
  -e GLASH_CONNECTION_PASSWORD=your_password \
  -e GLASH_ADMIN_PASSWORD=admin_password \
  aaasoft/glash-server-web:main
```

### 客户端

```bash
docker run -d \
  --name glash-client-web \
  -p 6001:6001 \
  aaasoft/glash-client-web:main
```

启动后通过 Web 界面（默认端口 6001）配置 Server URL。

### 代理端

```bash
docker run -d \
  --name glash-agent-web \
  -p 6002:6002 \
  aaasoft/glash-agent-web:main
```

启动后通过 Web 界面（默认端口 6002）配置 Server URL 和代理信息。

### 代理端（控制台版）

```bash
docker run -d \
  --name glash-agent-console \
  -e GLASH_SERVER_URL=ws://your-server:6000/glash \
  -e GLASH_AGENT_NAME=my_agent \
  -e GLASH_AGENT_PASSWORD=my_password \
  aaasoft/glash-agent-console:main
```

无 Web UI，通过环境变量配置，适合容器化和自动化部署。

## 环境变量

### 服务端环境变量

| 变量名 | 说明 | 默认值 |
|-------|------|-------|
| `GLASH_CONNECTION_PASSWORD` | 连接密码 | 自动生成 |
| `GLASH_ADMIN_PASSWORD` | 管理员密码 | 空 |
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |
| `GLASH_SERVER_PATH` | 服务器路径 | `/glash` |

### 客户端环境变量

| 变量名 | 说明 | 默认值 |
|-------|------|-------|
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |
| `ASPNETCORE_HTTP_PORTS` | 监听端口 | `6001` |

### 代理端环境变量

| 变量名 | 说明 | 默认值 |
|-------|------|-------|
| `GLASH_ADMIN_PASSWORD` | 管理员密码 | 空 |
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |
| `ASPNETCORE_HTTP_PORTS` | 监听端口 | `6002` |

### 代理端控制台环境变量

| 变量名 | 说明 | 是否必须 |
|-------|------|---------|
| `GLASH_SERVER_URL` | 服务端连接 URL（如 `ws://server:6000/glash`） | 是 |
| `GLASH_AGENT_NAME` | 代理名称 | 是 |
| `GLASH_AGENT_PASSWORD` | 代理密码 | 是 |

## Docker Compose 示例

```yaml
version: '3.8'

services:
  glash-server-web:
    image: aaasoft/glash-server-web:main
    container_name: glash-server-web
    ports:
      - "6000:6000"
    environment:
      - GLASH_CONNECTION_PASSWORD=your_password
      - GLASH_ADMIN_PASSWORD=admin_password
    restart: unless-stopped

  glash-client-web:
    image: aaasoft/glash-client-web:main
    container_name: glash-client-web
    ports:
      - "6001:6001"
    depends_on:
      - glash-server-web
    restart: unless-stopped

  glash-agent-web:
    image: aaasoft/glash-agent-web:main
    container_name: glash-agent-web
    ports:
      - "6002:6002"
    depends_on:
      - glash-server-web
    restart: unless-stopped

  glash-agent-console:
    image: aaasoft/glash-agent-console:main
    container_name: glash-agent-console
    environment:
      - GLASH_SERVER_URL=ws://glash-server-web:6000/glash
      - GLASH_AGENT_NAME=my_agent
      - GLASH_AGENT_PASSWORD=my_password
    depends_on:
      - glash-server-web
    restart: unless-stopped
```

## 多平台支持

所有镜像都支持以下平台：
- `linux/amd64`
- `linux/arm64`

## 版本标签

镜像支持以下标签：
- `main`: 分支最新提交（滚动更新）
- `v1.0.0`: 语义化版本标签
- `1.0`: 主次版本标签

## 故障排除

### 查看日志

```bash
docker logs glash-server-web
docker logs glash-client-web
docker logs glash-agent-web
docker logs glash-agent-console
```

### 进入容器

```bash
docker exec -it glash-server-web /bin/bash
docker exec -it glash-client-web /bin/bash
docker exec -it glash-agent-web /bin/bash
docker exec -it glash-agent-console /bin/bash
```

### 健康检查

```bash
# 检查服务端是否运行
curl http://localhost:6000/health

# 检查客户端是否运行
curl http://localhost:6001/health

# 检查代理端是否运行
curl http://localhost:6002/health
```
