# Glash Agent

Glash Agent 是 Glash 反向代理系统的代理端组件，部署在内网环境中，负责将内网服务流量通过 Glash Server 中转到外部。

## 快速开始

```bash
docker run -d \
  --name glash-agent \
  -p 6002:6002 \
  -v /path/to/data:/app/data \
  -e GLASH_CONNECTION_PASSWORD=server_password \
  yourusername/glash-agent:latest
```

## 环境变量

| 变量名 | 说明 | 默认值 |
|-------|------|-------|
| `GLASH_ADMIN_PASSWORD` | 管理员密码 | 空 |
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |
| `HTTP_PORTS` | 监听端口 | `6002` |

## 架构说明

Glash 采用 C/S 架构，包含三个组件：

- **Server**：中转服务器，负责协调 Agent 和 Client 之间的连接
- **Agent（本镜像）**：代理端，部署在内网环境中，负责转发本地服务流量
- **Client**：客户端，用于管理代理规则和隧道

## 工作流程

1. Agent 启动后连接到 Glash Server
2. Agent 在 Server 上注册自己的名称
3. Client 通过 Server 创建隧道，指定 Agent 转发目标
4. Agent 收到隧道请求后，连接本地服务并转发流量

## 配置示例

### Docker Compose

```yaml
version: '3.8'

services:
  glash-server:
    image: yourusername/glash-server:latest
    container_name: glash-server
    ports:
      - "6000:6000"
    environment:
      - GLASH_CONNECTION_PASSWORD=my_secure_password
    restart: unless-stopped

  glash-agent:
    image: yourusername/glash-agent:latest
    container_name: glash-agent
    environment:
      - GLASH_CONNECTION_PASSWORD=my_secure_password
    depends_on:
      - glash-server
    restart: unless-stopped
```

## 更多信息

- [GitHub 仓库](https://github.com/aaasoft/Glash)
- [Docker 部署指南](https://github.com/aaasoft/Glash/blob/main/DOCKER.md)
