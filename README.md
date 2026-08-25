# Glash

[![NuGet](https://img.shields.io/nuget/v/Glash.svg)](https://www.nuget.org/packages/Glash/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Glash.svg)](https://www.nuget.org/packages/Glash/)
[![Docker](https://img.shields.io/badge/Docker-Hub-blue?logo=docker)](https://hub.docker.com/u/aaasoft)
[![GitHub](https://img.shields.io/badge/GitHub-aaasoft%2FGlash-black?logo=github)](https://github.com/aaasoft/Glash)
[![License](https://img.shields.io/github/license/aaasoft/Glash)](LICENSE)

Glash 是一个轻量级的反向代理系统，可以将位于 NAT 或防火墙后面的本地服务器安全地暴露到外部网络。基于 [Quick.Protocol](https://github.com/QuickProtocol/Quick.Protocol) 实现高效通信，支持 WebSocket 协议传输。

## 功能特性

- **反向代理** — 将内网服务通过中转服务器暴露到外部
- **多代理类型** — 内置 SSH、RDP、Web、数据库等常见代理类型，支持自定义扩展
- **Web 管理界面** — 基于 Blazor 的服务端管理面板，支持 Agent/Client/隧道管理
- **桌面客户端** — 基于 Avalonia 的跨平台桌面客户端（Windows），支持一键连接
- **多语言支持** — 内置中文本地化支持
- **Docker 部署** — 提供官方 Docker 镜像，支持 linux/amd64 和 linux/arm64
- **CI/CD 自动化** — GitHub Actions 自动构建并发布到 Docker Hub

## 系统架构

Glash 采用 **Server-Agent-Client** 三层架构：

```
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│              │         │              │         │              │
│    Client    │◄───────►│    Server    │◄───────►│     Agent    │
│  (管理端)     │         │  (中转服务器)  │         │  (代理端)     │
│              │         │              │         │              │
└──────┬───────┘         └──────────────┘         └──────┬───────┘
       │                                                  │
       │         ┌──────────────┐                         │
       │         │              │                         │
       └────────►│  本地服务端口  │◄────────────────────────┘
                 │  (RDP/SSH等)  │
                 │              │
                 └──────────────┘
```

| 组件 | 说明 | 默认端口 |
|------|------|---------|
| **Server** | 中转服务器，协调 Agent 和 Client 之间的连接与隧道创建 | 6000 |
| **Agent** | 部署在内网环境，接收 Server 指令，连接本地服务并转发流量 | 6002 |
| **Client** | 管理端，通过 Web 界面管理代理规则、创建隧道 | 6001 |

### 工作流程

1. **Agent** 启动后连接到 **Server**，注册自己的名称
2. **Client** 连接到 **Server**，获取可用 Agent 列表
3. **Client** 通过 Web 界面创建代理规则（指定 Agent、目标主机和端口）
4. **Client** 请求创建隧道，**Server** 协调 **Agent** 建立到目标服务的连接
5. 数据通过 **Server** 在 **Client** 和 **Agent** 之间双向转发

## 项目结构

```
Glash/
├── Glash/                    # 核心协议库（NuGet 包）
│   ├── Agent/                # Agent 端协议与实现
│   ├── Client/               # Client 端协议与实现
│   ├── Server/               # Server 端协议与实现
│   └── Core/                 # 共享核心类（隧道、加密等）
│
├── GlashServer/              # Server 可执行程序（ASP.NET Core Web）
├── GlashClient/              # Client 可执行程序（ASP.NET Core Web）
├── GlashAgent/               # Agent 可执行程序（ASP.NET Core Web）
│
├── Glash.Blazor.Server/      # Server 端 Blazor UI 组件库
├── Glash.Blazor.Client/      # Client 端 Blazor UI 组件库
├── Glash.Blazor.Agent/       # Agent 端 Blazor UI 组件库
│
├── GlashClientDesktop/       # Avalonia 桌面客户端（Windows）
│
└── .github/workflows/        # GitHub Actions CI/CD
```

## 快速开始

### 方式一：Docker 部署（推荐）

使用 Docker Compose 一键部署：

```yaml
version: '3.8'

services:
  glash-server:
    image: aaasoft/glash-server:latest
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
    image: aaasoft/glash-client:latest
    container_name: glash-client
    ports:
      - "6001:6001"
    volumes:
      - ./client-data:/app/data
    depends_on:
      - glash-server
    restart: unless-stopped

  glash-agent:
    image: aaasoft/glash-agent:latest
    container_name: glash-agent
    ports:
      - "6002:6002"
    volumes:
      - ./agent-data:/app/data
    depends_on:
      - glash-server
    restart: unless-stopped
```

启动后通过各自的 Web 界面配置 Server URL：
- Client：`http://localhost:6001`
- Agent：`http://localhost:6002`
```

```bash
docker-compose up -d
```

### 方式二：源码构建

```bash
# 克隆仓库
git clone https://github.com/aaasoft/Glash.git
cd Glash

# 构建并运行 Server
dotnet run --project GlashServer

# 构建并运行 Client
dotnet run --project GlashClient

# 构建并运行 Agent
dotnet run --project GlashAgent
```

### 方式三：NuGet 集成

将 Glash 作为库集成到自己的项目中：

```bash
dotnet add package Glash
```

## 环境变量

### Server

| 变量名 | 说明 | 默认值 |
|--------|------|--------|
| `GLASH_CONNECTION_PASSWORD` | 连接密码（留空则自动生成） | 自动生成 |
| `GLASH_ADMIN_PASSWORD` | 管理员密码 | 空 |
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |
| `GLASH_SERVER_PATH` | WebSocket 服务路径 | `/glash` |
| `HTTP_PORTS` | 监听端口 | `6000` |

### Client

| 变量名 | 说明 | 默认值 |
|--------|------|--------|
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |
| `HTTP_PORTS` | 监听端口 | `6001` |

### Agent

| 变量名 | 说明 | 默认值 |
|--------|------|--------|
| `GLASH_ADMIN_PASSWORD` | 管理员密码 | 空 |
| `GLASH_DB_FILE_PATH` | 数据库文件路径 | `Config.litedb` |
| `HTTP_PORTS` | 监听端口 | `6002` |

## 桌面客户端

GlashClientDesktop 是基于 [Avalonia UI](https://avaloniaui.net/) 的跨平台桌面客户端，提供以下功能：

- **连接管理** — 添加、编辑、删除 Server 连接
- **代理规则管理** — 可视化配置代理规则
- **内置代理类型**：
  - **SSH** — 一键启动 PuTTY 终端，支持 WinSCP 文件传输
  - **RDP** — 一键启动远程桌面连接
  - **Web** — 一键打开浏览器访问内网 Web 服务
  - **Database** — 支持数据库连接代理
- **实时监控** — 查看隧道状态、流量统计

## 技术栈

| 技术 | 用途 |
|------|------|
| .NET 10.0 | 运行时框架 |
| ASP.NET Core | Web 服务端 |
| Blazor Server | 服务端管理 UI |
| Avalonia UI | 跨平台桌面客户端 |
| Quick.Protocol | 自定义协议通信 |
| Quick.LiteDB.Plus | 轻量级数据库存储 |
| Docker | 容器化部署 |
| GitHub Actions | CI/CD 自动化 |

## Docker 镜像

所有镜像支持 `linux/amd64` 和 `linux/arm64` 平台：

| 镜像 | 用途 |
|------|------|
| `aaasoft/glash-server` | 服务端 |
| `aaasoft/glash-client` | 客户端 |
| `aaasoft/glash-agent` | 代理端 |

```bash
docker pull aaasoft/glash-server:latest
docker pull aaasoft/glash-client:latest
docker pull aaasoft/glash-agent:latest
```

## 相关链接

- [GitHub 仓库](https://github.com/aaasoft/Glash)
- [NuGet 包](https://www.nuget.org/packages/Glash/)
- [Docker Hub](https://hub.docker.com/u/aaasoft)
- [Docker 部署指南](DOCKER.md)

## 许可证

本项目基于 MIT 许可证开源。详见 [LICENSE](LICENSE) 文件。
