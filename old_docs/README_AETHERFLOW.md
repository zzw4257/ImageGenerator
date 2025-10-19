# Aetherflow - AIGC 资源协作与交易平台

> **让AIGC资源流动起来 🌊**

基于 ImageGenerator 开发的 AIGC 资源协作与交易平台 MVP 版本。通过建立三边市场（资源提供者、效率追求者、技术贡献者），利用标准化的 Credits 实现资源的按需使用和价值流动。

---

## 🎯 项目愿景

构建一个基于通用积分（Credits）的 AIGC 资源协作与交易平台。我们不制造工具，我们构建一个经济体，旨在盘活全球闲置的 AIGC 算力与创意资产，让价值高效流动，最终成为 AIGC 领域的"价值交换层"和"资产管理层"。

---

## 🚀 核心价值

### 三边市场模型

```
┌─────────────────┐
│  资源提供者      │  闲置API额度 → 收益
│  (Miners)       │
└────────┬────────┘
         │
         │  提供算力
         ▼
┌─────────────────┐
│  Credits池      │  平台价值交换层
│  (Platform)     │
└────────┬────────┘
         │
         │  按需消费
         ▼
┌─────────────────┐
│  效率追求者      │  灵活使用 → 效率
│  (Creators)     │
└─────────────────┘
```

### MVP 核心功能

✅ **用户系统**: 邮箱注册/登录，JWT认证  
✅ **钱包系统**: Credits充值、余额查询、交易流水  
✅ **资源接入**: 资源提供者托管合规API密钥（Stability AI）  
✅ **智能调度**: 自动选择最优资源进行图片生成  
✅ **交易闭环**: 自动扣费、分账、佣金计算  
✅ **争议处理**: 基础的退款和补偿机制  

---

## 🛠️ 技术栈

### 后端
- **框架**: ASP.NET Core 8
- **ORM**: Entity Framework Core
- **数据库**: SQLite (开发) / PostgreSQL (生产)
- **认证**: JWT
- **API文档**: Scalar/OpenAPI

### 前端
- **框架**: Vue 3 + TypeScript
- **UI库**: Vuetify 3 (Material Design)
- **状态管理**: Pinia
- **构建工具**: Vite

### 基础设施
- **容器化**: Docker + Docker Compose
- **CI/CD**: GitHub Actions
- **密钥管理**: AES-256 加密（生产环境使用Key Vault）

---

## 📚 文档导航

### 快速开始
- 📖 [**快速启动指南**](./AETHERFLOW_QUICKSTART.md) - 5分钟上手，环境配置和一键启动
- 🚀 [启动脚本](./start.sh) (macOS/Linux) / [start.bat](./start.bat) (Windows)

### 规划与设计
- 📋 [**完整开发计划**](./AETHERFLOW_PLAN.md) - 技术架构、模块设计、数据库设计
- 👥 [**团队任务分工**](./TEAM_TASKS.md) - 7人团队的详细任务清单（2-3周）
- 🔍 [**调研模板**](./RESEARCH_TEMPLATE.md) - AIGC平台调研框架

### 原始文档
- 📄 [ImageGenerator README](./README.md) - 原项目说明
- 🌐 [English README](./README.en.md)

---

## 👥 团队分工（7人）

| 角色 | 人数 | 主要职责 |
|------|------|----------|
| 项目负责人 (PM) | 1人 | 整体协调、需求把控、API设计 |
| 后端开发 (BE) | 2人 | 用户/钱包系统、资源调度、API开发 |
| 前端开发 (FE) | 2人 | 用户界面、资源管理、状态管理 |
| 调研与测试 (QA) | 1人 | AIGC平台调研、合规性分析、功能测试 |
| DevOps (OPS) | 1人 | 部署配置、CI/CD、问题修复 |

**详细任务分工**: 查看 [TEAM_TASKS.md](./TEAM_TASKS.md)

---

## 📅 开发计划（3周）

### Week 1 (10月19-25日): 调研 + 架构搭建
- ✅ AIGC平台深度调研（10+平台）
- ✅ 用户注册/登录
- ✅ 钱包系统
- ✅ 资源接入
- ✅ 端到端文生图流程

### Week 2 (10月26-11月1日): 核心功能开发
- ⏳ 完善交易流水
- ⏳ 支付集成（支付宝沙箱）
- ⏳ 资源池同步
- ⏳ 管理后台
- ⏳ 争议处理

### Week 3 (11月2-8日): 测试优化与上线
- ⏳ 全面测试（功能+安全+性能）
- ⏳ Bug修复
- ⏳ 生产环境部署
- ⏳ 答辩准备

**详细计划**: 查看 [AETHERFLOW_PLAN.md](./AETHERFLOW_PLAN.md)

---

## 🏗️ 系统架构

```
┌─────────────────────────────────────────┐
│         前端 (Vue 3 + Vuetify)          │
│  注册登录 | 资源接入 | 图片生成 | 钱包  │
└──────────────────┬──────────────────────┘
                   │ REST API
┌──────────────────┴──────────────────────┐
│       后端 (ASP.NET Core 8)             │
│  ┌──────────┐  ┌──────────┐  ┌────────┐│
│  │Identity  │  │Wallet    │  │Resource││
│  │Service   │  │Service   │  │Gateway ││
│  └──────────┘  └──────────┘  └────────┘│
│  ┌──────────────────────────────────┐  │
│  │  Generation Service (智能调度)   │  │
│  └──────────────────────────────────┘  │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────┴──────────────────────┐
│  PostgreSQL  │  Redis   │  Blob Storage │
└─────────────────────────────────────────┘
                   │
┌──────────────────┴──────────────────────┐
│  Stability AI  │  OpenAI  │  其他APIs   │
└─────────────────────────────────────────┘
```

---

## 🚀 快速开始

### 前置要求
- .NET 8 SDK
- Node.js 18+
- pnpm
- SQLite (内置) 或 PostgreSQL

### 一键启动

**macOS/Linux**:
```bash
chmod +x start.sh
./start.sh
```

**Windows**:
```cmd
start.bat
```

### 手动启动

**后端**:
```bash
cd ImageGenerator
dotnet restore
dotnet ef database update
dotnet run
# 访问: http://localhost:5000
```

**前端**:
```bash
cd WebUI
pnpm install
pnpm dev
# 访问: http://localhost:5173
```

**详细说明**: 查看 [AETHERFLOW_QUICKSTART.md](./AETHERFLOW_QUICKSTART.md)

---

## 📊 数据模型

### 核心实体

```
Users (用户)
  ├─ Email, PasswordHash, Role
  └─ WalletId (1:1)

Wallets (钱包)
  ├─ Balance (Credits余额)
  └─ Transactions[] (1:N)

ApiResources (API资源)
  ├─ UserId, Provider, ApiKeyEncrypted
  ├─ AvailableCredits, Status
  └─ PricePerCredit

GenerationTasks (生成任务)
  ├─ UserId, ApiResourceId
  ├─ Prompt, Params
  └─ Status, CreditsCost, ResultUrl

Transactions (交易记录)
  ├─ WalletId, Type, Amount
  └─ BalanceAfter, Description
```

---

## 🔐 安全性

- **API密钥加密**: AES-256 静态加密，环境变量管理主密钥
- **JWT认证**: 24小时过期，HTTPS传输
- **SQL注入防护**: 使用参数化查询（EF Core）
- **XSS防护**: 前端输入过滤，CSP策略
- **限流**: 基于IP和用户的请求限流
- **审计日志**: 完整记录所有交易和API调用

---

## ✅ 成功标准

### MVP验收标准
- ✅ 用户可以注册、登录、充值
- ✅ 资源提供者可以接入Stability AI并验证
- ✅ 效率追求者可以生成图片并自动扣费
- ✅ 交易流水清晰可查
- ✅ 管理员可以处理争议

### 技术指标
- API响应时间 P95 < 500ms
- 单元测试覆盖率 > 70%
- 无高危安全漏洞
- 系统可用性 > 99%

---

## 📝 开发规范

### Git提交规范
```
feat: 新增功能
fix: Bug修复
docs: 文档更新
refactor: 代码重构
test: 测试相关
chore: 构建/工具变动
```

### 分支策略
- `main` - 生产分支（保护）
- `aetherflow/mvp` - MVP开发主分支
- `feature/*` - 功能分支
- `bugfix/*` - Bug修复分支

---

## 🎯 里程碑

- [x] **M0**: 项目启动，文档完成 (10/19)
- [ ] **M1**: 架构搭建完成，端到端可用 (10/25)
- [ ] **M2**: MVP功能完整 (11/1)
- [ ] **M3**: 生产就绪，答辩准备 (11/8)

---

## 🤝 贡献指南

1. Fork本仓库
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'feat: add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 提交Pull Request

**Code Review要点**:
- 安全性（密钥、注入、XSS）
- 性能（N+1查询、循环调用）
- 可读性（命名、注释）
- 测试覆盖

---

## 📞 联系方式

- **项目看板**: GitHub Projects
- **代码仓库**: [github.com/zzw4257/ImageGenerator](https://github.com/zzw4257/ImageGenerator)
- **分支**: `aetherflow/mvp`
- **文档更新**: 2024-10-19

---

## 📄 许可证

本项目基于 [原始ImageGenerator项目](https://github.com/zzw4257/ImageGenerator) 开发。

---

## 🙏 致谢

- 基于 [ImageGenerator](https://github.com/zzw4257/ImageGenerator) 项目
- Stability AI 提供的优质API
- .NET 和 Vue.js 社区

---

**Aetherflow MVP - 让AIGC资源流动起来！🌊**

*最后更新: 2024-10-19 | 版本: 1.0.0*