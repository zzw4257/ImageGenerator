# Aetherflow 快速启动指南

> 基于 ImageGenerator 的 AIGC 资源协作平台 - MVP版本快速部署文档

## 📋 环境要求

- **.NET 8 SDK** - [下载地址](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [下载地址](https://nodejs.org/)
- **pnpm** - 运行 `npm install -g pnpm` 安装
- **SQLite** - 内置支持，无需额外安装

## 🚀 一键启动

### 1. 配置环境变量

在 `ImageGenerator` 目录下创建 `.env` 文件：

```bash
# 数据库配置
DATABASE_PATH=./data/aetherflow.db

# JWT配置
JWT_SECRET=your-super-secret-jwt-key-change-this-in-production
JWT_ISSUER=Aetherflow
JWT_AUDIENCE=AetherflowUsers
JWT_EXPIRY_HOURS=24

# API配置
API_BASE_URL=http://localhost:5000
CORS_ORIGINS=http://localhost:5173,http://localhost:3000

# 图片存储
IMAGE_STORAGE_PATH=./images
MAX_IMAGE_SIZE_MB=10

# Credits配置
DEFAULT_USER_CREDITS=100
DAILY_CREDIT_BONUS=10

# AIGC API配置（示例）
STABILITY_API_KEY=sk-your-stability-api-key
OPENAI_API_KEY=sk-your-openai-api-key
```

### 2. 后端启动

```bash
# 进入后端目录
cd ImageGenerator

# 恢复依赖
dotnet restore

# 创建数据库
dotnet ef database update

# 启动后端服务
dotnet run
```

后端将运行在：`http://localhost:5000`

### 3. 前端启动

打开新终端窗口：

```bash
# 进入前端目录
cd WebUI

# 安装依赖
pnpm install

# 启动开发服务器
pnpm dev
```

前端将运行在：`http://localhost:5173`

## 📝 使用脚本快速启动

### macOS/Linux

在项目根目录创建 `start.sh`：

```bash
#!/bin/bash

# 启动后端
cd ImageGenerator
dotnet run &
BACKEND_PID=$!

# 等待后端启动
sleep 5

# 启动前端
cd ../WebUI
pnpm dev &
FRONTEND_PID=$!

echo "🚀 Aetherflow 已启动！"
echo "后端: http://localhost:5000"
echo "前端: http://localhost:5173"
echo ""
echo "按 Ctrl+C 停止所有服务"

# 等待用户中断
trap "kill $BACKEND_PID $FRONTEND_PID; exit" INT
wait
```

运行：
```bash
chmod +x start.sh
./start.sh
```

### Windows

在项目根目录创建 `start.bat`：

```batch
@echo off
echo Starting Aetherflow...

start "Aetherflow Backend" cmd /k "cd ImageGenerator && dotnet run"
timeout /t 5 /nobreak

start "Aetherflow Frontend" cmd /k "cd WebUI && pnpm dev"

echo Aetherflow is starting...
echo Backend: http://localhost:5000
echo Frontend: http://localhost:5173
```

## 🔧 初始化配置

### 创建初始邀请码

数据库创建后会自动生成一个初始邀请码。查看方式：

```bash
# 使用SQLite查看
sqlite3 ImageGenerator/User.db "SELECT * FROM Invitations;"
```

或通过API：
```bash
curl http://localhost:5000/api/invitation
```

### 注册第一个用户

```bash
curl -X POST http://localhost:5000/api/authentication/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123",
    "invitationCode": "your-invitation-code-here"
  }'
```

## 🎨 测试图片生成

登录后，测试基础图片生成功能：

```bash
# 1. 登录获取token
TOKEN=$(curl -X POST http://localhost:5000/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}' \
  | jq -r '.token')

# 2. 创建对话
CONVERSATION_ID=$(curl -X POST http://localhost:5000/api/conversation \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"测试对话"}' \
  | jq -r '.id')

# 3. 生成图片
curl -X POST http://localhost:5000/api/conversation/$CONVERSATION_ID/generate \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "prompt": "a beautiful sunset over mountains",
    "model": "gemini",
    "width": 512,
    "height": 512
  }'
```

## 📂 项目结构

```
ImageGenerator/
├── Controllers/         # API控制器
├── Services/           # 业务逻辑层
├── Models/             # 数据模型
├── Database/           # EF Core DbContext
├── Dtos/               # 数据传输对象
├── Helpers/            # 工具类
└── appsettings.json    # 配置文件

WebUI/
├── src/
│   ├── components/     # Vue组件
│   ├── pages/          # 页面
│   ├── services/       # API服务
│   ├── stores/         # 状态管理
│   └── router/         # 路由配置
└── package.json
```

## 🐛 常见问题

### 1. 端口被占用

修改端口：
- 后端：编辑 `ImageGenerator/Properties/launchSettings.json`
- 前端：编辑 `WebUI/vite.config.ts` 中的 `server.port`

### 2. 数据库迁移失败

```bash
# 删除旧数据库
rm ImageGenerator/User.db

# 重新迁移
cd ImageGenerator
dotnet ef database update
```

### 3. pnpm 安装失败

尝试清除缓存：
```bash
cd WebUI
pnpm store prune
pnpm install --force
```

### 4. .NET SDK 版本不匹配

检查版本：
```bash
dotnet --version
```

切换到 .NET 8：
```bash
dotnet --list-sdks
# 在 global.json 中指定版本
```

## 📚 下一步

- 查看 [API 文档](http://localhost:5000/scalar/v1)（开发环境）
- 阅读 [完整 SPEC](../AETHERFLOW_SPEC.md)
- 查看 [开发计划](../AETHERFLOW_PLAN.md)

## 💡 提示

- 首次运行建议使用开发模式，便于调试
- 生产环境部署前务必修改 JWT_SECRET
- 定期备份 SQLite 数据库文件
- 监控 `images/` 目录大小，定期清理

---

**Aetherflow MVP** - 让AIGC资源流动起来 🌊