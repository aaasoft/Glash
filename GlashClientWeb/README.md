# Glash Client

Glash Client 是 Glash 反向代理系统的客户端组件，用于管理代理规则、创建隧道，将内网服务暴露到外部。

## 快速开始

```bash
docker run -d \
  --name glash-client-web \
  -p 6001:6001 \
  aaasoft/glash-client-web:latest
```

启动后通过 Web 界面（默认端口 6001）配置 Server URL。

## 环境变量

| 变量名 | 说明 | 默认值 |
|-------|------|-------|
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |
| `ASPNETCORE_HTTP_PORTS` | 监听端口 | `6001` |

## 架构说明

Glash 采用 C/S 架构，包含三个组件：

- **Server**：中转服务器，负责协调 Agent 和 Client 之间的连接
- **Agent**：代理端，部署在内网环境中，负责转发本地服务流量
- **Client（本镜像）**：客户端，用于管理代理规则和隧道

## 功能特性

- 通过 Web 界面管理代理规则
- 支持多种代理类型（SSH、RDP、Web、数据库等）
- 实时查看代理状态和流量信息
- 支持多 Agent 管理

## 配置示例

### Docker Compose

```yaml
version: '3.8'

services:
  glash-server:
    image: aaasoft/glash-server:latest
    container_name: glash-server
    ports:
      - "6000:6000"
    environment:
      - GLASH_CONNECTION_PASSWORD=my_secure_password
    restart: unless-stopped

  glash-client-web:
    image: aaasoft/glash-client-web:latest
    container_name: glash-client-web
    ports:
      - "6001:6001"
    depends_on:
      - glash-server
    restart: unless-stopped
```

启动后通过 Web 界面（默认端口 6001）配置 Server URL。

## 更多信息

- [GitHub 仓库](https://github.com/aaasoft/Glash)
- [Docker 部署指南](https://github.com/aaasoft/Glash/blob/main/DOCKER.md)
