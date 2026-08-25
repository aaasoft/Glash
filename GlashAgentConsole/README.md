# Glash Agent Console

Glash Agent Console 是 Glash 反向代理系统的代理端控制台版本，无 Web UI，通过环境变量配置，适合容器化和自动化部署场景。

## 快速开始

```bash
docker run -d \
  --name glash-agent-console \
  -e GLASH_SERVER_URL=ws://your-server:6000/glash \
  -e GLASH_AGENT_NAME=my_agent \
  -e GLASH_AGENT_PASSWORD=my_password \
  aaasoft/glash-agent-console:main
```

## 环境变量

| 变量名 | 说明 | 是否必须 |
|--------|------|---------|
| `GLASH_SERVER_URL` | 服务端连接 URL（如 `ws://server:6000/glash`） | 是 |
| `GLASH_AGENT_NAME` | 代理名称 | 是 |
| `GLASH_AGENT_PASSWORD` | 代理密码 | 是 |

## 与 GlashAgentWeb 的区别

| 特性 | GlashAgentConsole | GlashAgentWeb |
|------|-------------------|---------------|
| Web UI | 无 | 有（Blazor Server） |
| 配置方式 | 环境变量 | Web 界面 |
| 适用场景 | 容器化、自动化部署 | 手动管理、可视化操作 |
| 镜像大小 | 更小（runtime 基础镜像） | 较大（aspnet 基础镜像） |

## 架构说明

Glash 采用 C/S 架构，包含三个组件：

- **Server**：中转服务器，负责协调 Agent 和 Client 之间的连接
- **Agent（本程序）**：代理端，部署在内网环境中，负责转发本地服务流量
- **Client**：客户端，用于管理代理规则和隧道

## 功能特性

- 无 UI，纯控制台运行
- 断线自动重连（5 秒后重试）
- 支持 Ctrl+C 优雅退出
- 支持 Docker 部署

## 配置示例

### Docker Compose

```yaml
version: '3.8'

services:
  glash-server:
    image: aaasoft/glash-server-web:main
    container_name: glash-server
    ports:
      - "6000:6000"
    environment:
      - GLASH_CONNECTION_PASSWORD=my_secure_password
    restart: unless-stopped

  glash-agent-console:
    image: aaasoft/glash-agent-console:main
    container_name: glash-agent-console
    environment:
      - GLASH_SERVER_URL=ws://glash-server:6000/glash
      - GLASH_AGENT_NAME=my_agent
      - GLASH_AGENT_PASSWORD=my_password
    depends_on:
      - glash-server
    restart: unless-stopped
```

## 源码构建

```bash
dotnet run --project GlashAgentConsole
```

需要设置环境变量后运行：

```bash
export GLASH_SERVER_URL=ws://your-server:6000/glash
export GLASH_AGENT_NAME=my_agent
export GLASH_AGENT_PASSWORD=my_password
dotnet run --project GlashAgentConsole
```

## 更多信息

- [GitHub 仓库](https://github.com/aaasoft/Glash)
- [Docker 部署指南](https://github.com/aaasoft/Glash/blob/main/DOCKER.md)
