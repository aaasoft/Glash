# Glash Server

Glash 是一个反向代理服务器，允许你将位于 NAT 或防火墙后面的本地服务器暴露给外部访问。

## 快速开始

```bash
docker run -d \
  --name glash-server \
  -p 6000:6000 \
  -v /path/to/data:/app/data \
  -e GLASH_CONNECTION_PASSWORD=your_password \
  -e GLASH_ADMIN_PASSWORD=admin_password \
  aaasoft/glash-server:main
```

## 环境变量

| 变量名 | 说明 | 默认值 |
|-------|------|-------|
| `GLASH_CONNECTION_PASSWORD` | 连接密码（留空则自动生成） | 自动生成 |
| `GLASH_ADMIN_PASSWORD` | 管理员密码 | 空 |
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |
| `GLASH_SERVER_PATH` | WebSocket 服务路径 | `/glash` |
| `HTTP_PORTS` | 监听端口 | `6000` |

## 架构说明

Glash 采用 C/S 架构，包含三个组件：

- **Server（本镜像）**：中转服务器，负责协调 Agent 和 Client 之间的连接
- **Agent**：代理端，部署在内网环境中，负责转发本地服务流量
- **Client**：客户端，用于管理代理规则和隧道

## 配置示例

### Docker Compose

```yaml
version: '3.8'

services:
  glash-server:
    image: aaasoft/glash-server:main
    container_name: glash-server
    ports:
      - "6000:6000"
    volumes:
      - ./data:/app/data
    environment:
      - GLASH_CONNECTION_PASSWORD=my_secure_password
      - GLASH_ADMIN_PASSWORD=admin_password
    restart: unless-stopped
```

## 更多信息

- [GitHub 仓库](https://github.com/aaasoft/Glash)
- [Docker 部署指南](https://github.com/aaasoft/Glash/blob/main/DOCKER.md)
