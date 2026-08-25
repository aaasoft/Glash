# Docker 部署指南

## 概述

Glash 项目提供了三个 Docker 镜像，分别用于部署服务端、客户端和代理端。

## 镜像说明

| 镜像名称 | 用途 | 默认端口 |
|---------|------|---------|
| `glash-server` | Glash 服务端 | 6000 |
| `glash-client` | Glash 客户端 | 6001 |
| `glash-agent` | Glash 代理端 | 6002 |

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
docker pull yourusername/glash-server:latest

# 拉取客户端镜像
docker pull yourusername/glash-client:latest

# 拉取代理端镜像
docker pull yourusername/glash-agent:latest
```

## 运行容器

### 服务端

```bash
docker run -d \
  --name glash-server \
  -p 6000:6000 \
  -v /path/to/data:/app/data \
  -e GLASH_CONNECTION_PASSWORD=your_password \
  -e GLASH_ADMIN_PASSWORD=admin_password \
  yourusername/glash-server:latest
```

### 客户端

```bash
docker run -d \
  --name glash-client \
  -p 6001:6001 \
  -v /path/to/data:/app/data \
  -e GLASH_SERVER_URL=ws://your-server:6000/glash \
  yourusername/glash-client:latest
```

### 代理端

```bash
docker run -d \
  --name glash-agent \
  -p 6002:6002 \
  -v /path/to/data:/app/data \
  -e GLASH_SERVER_URL=ws://your-server:6000/glash \
  -e GLASH_AGENT_NAME=your_agent_name \
  -e GLASH_AGENT_PASSWORD=your_agent_password \
  yourusername/glash-agent:latest
```

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
| `GLASH_SERVER_URL` | 服务器地址 | - |
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |

### 代理端环境变量

| 变量名 | 说明 | 默认值 |
|-------|------|-------|
| `GLASH_SERVER_URL` | 服务器地址 | - |
| `GLASH_AGENT_NAME` | 代理名称 | - |
| `GLASH_AGENT_PASSWORD` | 代理密码 | - |
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |

## Docker Compose 示例

```yaml
version: '3.8'

services:
  glash-server:
    image: yourusername/glash-server:latest
    container_name: glash-server
    ports:
      - "6000:6000"
    volumes:
      - ./server-data:/app/data
    environment:
      - GLASH_CONNECTION_PASSWORD=your_password
      - GLASH_ADMIN_PASSWORD=admin_password
    restart: unless-stopped

  glash-client:
    image: yourusername/glash-client:latest
    container_name: glash-client
    ports:
      - "6001:6001"
    volumes:
      - ./client-data:/app/data
    environment:
      - GLASH_SERVER_URL=ws://glash-server:6000/glash
    depends_on:
      - glash-server
    restart: unless-stopped

  glash-agent:
    image: yourusername/glash-agent:latest
    container_name: glash-agent
    ports:
      - "6002:6002"
    volumes:
      - ./agent-data:/app/data
    environment:
      - GLASH_SERVER_URL=ws://glash-server:6000/glash
      - GLASH_AGENT_NAME=your_agent_name
      - GLASH_AGENT_PASSWORD=your_agent_password
    depends_on:
      - glash-server
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
docker logs glash-server
docker logs glash-client
docker logs glash-agent
```

### 进入容器

```bash
docker exec -it glash-server /bin/bash
docker exec -it glash-client /bin/bash
docker exec -it glash-agent /bin/bash
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
