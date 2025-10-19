# Aetherflow 平台计划与架构设计文档

**版本**: 1.0
**创建日期**: 2025年10月19日（第5周周日）
**项目周期**: 2-3周 MVP开发
**团队规模**: 7人
**技术栈**: .NET 8 + Vue 3 + TypeScript + SQLite/PostgreSQL

---

## 📋 目录

1. [项目概述](#项目概述)
2. [技术架构设计](#技术架构设计)
3. [模块设计](#模块设计)
4. [数据库设计](#数据库设计)
5. [团队分工](#团队分工)
6. [开发计划](#开发计划)
7. [里程碑与交付物](#里程碑与交付物)
8. [风险评估](#风险评估)

---

## 项目概述

### 核心价值主张

Aetherflow 是一个基于通用积分（Credits）的 AIGC 资源协作与交易平台，通过建立三边市场（资源提供者、效率追求者、技术贡献者），实现 AIGC 资源的价值流动。

### MVP 目标

**聚焦验证核心假设**：
1. ✅ 用户愿意托管合规的 API 密钥来变现闲置额度
2. ✅ 消费者愿意为按需使用的 AIGC 服务付费
3. ✅ 平台可以安全、稳定地调度和管理 API 资源池

**MVP 范围**：
- 用户注册/登录（邮箱）
- Credits 钱包与充值系统
- 资源提供者接入 Stability AI（主要）
- 效率追求者文生图消费
- 基础交易流水与佣金系统
- 简单的争议处理后台

**不做**（MVP+阶段）：
- 提现功能
- 数字资产市场
- 复杂工作流编辑器
- 多模型并行支持

---

## 技术架构设计

### 整体架构

```
┌─────────────────────────────────────────────────────────────┐
│                        前端层 (Vue 3)                         │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │ 注册登录  │  │ 资源接入  │  │ 图片生成  │  │ 钱包中心  │   │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
└─────────────────────────────────────────────────────────────┘
                            │ HTTPS/REST API
┌─────────────────────────────────────────────────────────────┐
│                    API网关层 (.NET 8)                         │
│              JWT认证 + CORS + 限流 + 日志                     │
└─────────────────────────────────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────────┐
│                      应用服务层 (Controllers + Services)       │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐  ┌────────┐│
│  │ Identity   │  │ Wallet     │  │ Resource   │  │ Task   ││
│  │ Service    │  │ Service    │  │ Gateway    │  │ Queue  ││
│  └────────────┘  └────────────┘  └────────────┘  └────────┘│
└─────────────────────────────────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────────┐
│                      数据持久层                               │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────┐ │
│  │ PostgreSQL/SQLite│  │ Redis (Cache)    │  │ Blob      │ │
│  │ 用户/交易/资源    │  │ 会话/任务队列    │  │ Storage   │ │
│  └──────────────────┘  └──────────────────┘  └───────────┘ │
└─────────────────────────────────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────────┐
│                   外部服务集成层                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │ Stability AI │  │ Payment SDK  │  │ Email/SMS    │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────────────────────────┘
```

### 技术选型说明

| 层级 | 技术 | 理由 |
|------|------|------|
| 前端框架 | Vue 3 + TypeScript | 现有代码基础，快速迭代 |
| UI组件库 | Vuetify 3 | Material Design，组件丰富 |
| 状态管理 | Pinia | 轻量级，易于调试 |
| 后端框架 | ASP.NET Core 8 | 高性能，类型安全，成熟生态 |
| ORM | Entity Framework Core | 代码优先，迁移方便 |
| 数据库 | SQLite (开发) / PostgreSQL (生产) | SQLite快速原型，PostgreSQL生产级 |
| 缓存 | Redis (可选) | 会话、任务队列、限流 |
| 认证 | JWT | 无状态，跨域友好 |
| API文档 | Scalar/OpenAPI | 现有基础，自动生成 |
| 密钥管理 | AES-256 + Key Vault (Azure/本地) | 静态加密，生产用云KMS |
| 支付 | 支付宝SDK / Stripe | 国内首选支付宝，国际Stripe |
| 文件存储 | 本地文件系统 (MVP) / OSS (扩展) | MVP本地够用，后期云存储 |

---

## 模块设计

### 1. Identity Service (身份认证模块)

**职责**：
- 用户注册（邮箱 + 密码）
- 登录（JWT Token生成）
- 个人资料管理
- 角色管理（普通用户、资源提供者、管理员）

**数据模型**：
```csharp
public class User : ModelBase
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public UserRole Role { get; set; }
    public Guid WalletId { get; set; }
    public Wallet Wallet { get; set; }
}

public enum UserRole
{
    Consumer,        // 效率追求者
    ResourceProvider, // 资源提供者
    Admin           // 管理员
}
```

**API端点**：
- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/profile`
- `PUT /api/auth/profile`

---

### 2. Wallet Service (钱包与交易模块)

**职责**：
- Credits 钱包创建与管理
- 法币充值（支付宝集成）
- 交易记录与流水
- 佣金自动计算与扣除
- 余额查询

**数据模型**：
```csharp
public class Wallet : ModelBase
{
    public Guid UserId { get; set; }
    public decimal Balance { get; set; } // Credits余额
    public List<Transaction> Transactions { get; set; }
}

public class Transaction : ModelBase
{
    public Guid WalletId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Description { get; set; }
    public Guid? RelatedTaskId { get; set; } // 关联任务
}

public enum TransactionType
{
    Recharge,       // 充值
    Consume,        // 消费
    Earn,           // 收入（资源提供者）
    Commission,     // 佣金（平台）
    Refund          // 退款
}
```

**API端点**：
- `GET /api/wallet/balance`
- `POST /api/wallet/recharge`
- `GET /api/wallet/transactions`
- `GET /api/wallet/statistics`

---

### 3. Resource Gateway Service (资源网关模块)

**职责**：
- 资源提供者 API 密钥接入
- 密钥安全加密存储
- 密钥有效性验证
- 额度查询与同步
- 资源池管理

**数据模型**：
```csharp
public class ApiResource : ModelBase
{
    public Guid UserId { get; set; } // 资源提供者
    public ApiProvider Provider { get; set; } // Stability, OpenAI等
    public string ApiKeyEncrypted { get; set; } // AES加密
    public string KeyHash { get; set; } // 用于去重
    public ResourceStatus Status { get; set; }
    public decimal AvailableCredits { get; set; } // 可用额度
    public decimal TotalCredits { get; set; } // 总额度
    public decimal PricePerCredit { get; set; } // 每Credit定价
    public DateTime LastSyncAt { get; set; }
    public int DailyLimit { get; set; } // 日消耗上限
    public int MonthlyLimit { get; set; } // 月消耗上限
}

public enum ApiProvider
{
    StabilityAI,
    OpenAI,
    // 后续扩展
}

public enum ResourceStatus
{
    Active,
    Paused,
    Disabled,
    Error
}
```

**API端点**：
- `POST /api/resource/connect` - 接入API
- `GET /api/resource/list` - 我的资源列表
- `PUT /api/resource/{id}/status` - 暂停/启用
- `DELETE /api/resource/{id}` - 删除资源
- `GET /api/resource/{id}/stats` - 资源统计

**安全设计**：
```csharp
public class ApiKeyEncryption
{
    private readonly byte[] _key; // 从环境变量加载

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        // AES-256-CBC加密
        // 返回 Base64(IV + CipherText)
    }

    public string Decrypt(string encrypted)
    {
        // 解密逻辑
    }
}
```

---

### 4. Generation Service (AIGC任务调度模块)

**职责**：
- 接收图片生成请求
- 智能路由选择资源
- 调用外部 API
- 结果处理与存储
- 失败重试机制
- Credits 扣费与分账

**数据模型**：
```csharp
public class GenerationTask : ModelBase
{
    public Guid UserId { get; set; } // 消费者
    public Guid? ApiResourceId { get; set; } // 使用的资源
    public string Prompt { get; set; }
    public GenerationParams Params { get; set; } // JSON
    public TaskStatus Status { get; set; }
    public decimal CreditsCost { get; set; }
    public string ResultUrl { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public enum TaskStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded
}
```

**智能路由策略**：
```csharp
public interface IResourceSelector
{
    ApiResource SelectBestResource(List<ApiResource> pool, GenerationParams params);
}

// 策略：优先选择价格低、成功率高、响应快的资源
public class SmartResourceSelector : IResourceSelector
{
    public ApiResource SelectBestResource(List<ApiResource> pool, GenerationParams params)
    {
        return pool
            .Where(r => r.Status == ResourceStatus.Active)
            .Where(r => r.AvailableCredits >= EstimateCost(params))
            .OrderBy(r => r.PricePerCredit)
            .ThenByDescending(r => r.SuccessRate)
            .FirstOrDefault();
    }
}
```

**API端点**：
- `POST /api/generate/text-to-image`
- `GET /api/generate/task/{id}`
- `GET /api/generate/history`

---

### 5. Marketplace Service (市场服务 - P2阶段)

*暂不实现，预留接口*

---

### 6. Admin Service (后台管理模块)

**职责**：
- 用户管理
- 交易监控
- 争议处理（退款、补偿）
- 系统监控
- 风控规则

**数据模型**：
```csharp
public class Dispute : ModelBase
{
    public Guid UserId { get; set; }
    public Guid TaskId { get; set; }
    public DisputeType Type { get; set; }
    public string Description { get; set; }
    public DisputeStatus Status { get; set; }
    public Guid? HandledBy { get; set; } // 管理员
    public string Resolution { get; set; }
}

public enum DisputeType
{
    GenerationFailed,
    PoorQuality,
    WrongResult,
    Other
}
```

**API端点**：
- `GET /api/admin/users`
- `GET /api/admin/transactions`
- `GET /api/admin/disputes`
- `POST /api/admin/dispute/{id}/resolve`
- `GET /api/admin/statistics`

---

## 数据库设计

### ER图（核心表）

```
┌──────────────┐       ┌──────────────┐       ┌──────────────┐
│    Users     │───┬───│   Wallets    │───────│ Transactions │
└──────────────┘   │   └──────────────┘       └──────────────┘
                   │
                   │   ┌──────────────┐
                   ├───│ ApiResources │
                   │   └──────────────┘
                   │
                   │   ┌──────────────┐
                   └───│GenerationTask│
                       └──────────────┘
```

### 表结构定义

```sql
-- Users 表
CREATE TABLE Users (
    Id UUID PRIMARY KEY,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Salt VARCHAR(255) NOT NULL,
    Role INT NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP NOT NULL
);

-- Wallets 表
CREATE TABLE Wallets (
    Id UUID PRIMARY KEY,
    UserId UUID NOT NULL UNIQUE,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Transactions 表
CREATE TABLE Transactions (
    Id UUID PRIMARY KEY,
    WalletId UUID NOT NULL,
    Type INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    BalanceAfter DECIMAL(18,2) NOT NULL,
    Description TEXT,
    RelatedTaskId UUID,
    CreatedAt TIMESTAMP NOT NULL,
    FOREIGN KEY (WalletId) REFERENCES Wallets(Id)
);

-- ApiResources 表
CREATE TABLE ApiResources (
    Id UUID PRIMARY KEY,
    UserId UUID NOT NULL,
    Provider INT NOT NULL,
    ApiKeyEncrypted TEXT NOT NULL,
    KeyHash VARCHAR(64) NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    AvailableCredits DECIMAL(18,2) NOT NULL DEFAULT 0,
    TotalCredits DECIMAL(18,2) NOT NULL DEFAULT 0,
    PricePerCredit DECIMAL(18,4) NOT NULL DEFAULT 0.01,
    LastSyncAt TIMESTAMP,
    DailyLimit INT DEFAULT 10000,
    MonthlyLimit INT DEFAULT 100000,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- GenerationTasks 表
CREATE TABLE GenerationTasks (
    Id UUID PRIMARY KEY,
    UserId UUID NOT NULL,
    ApiResourceId UUID,
    Prompt TEXT NOT NULL,
    Params JSON,
    Status INT NOT NULL DEFAULT 0,
    CreditsCost DECIMAL(18,2) NOT NULL,
    ResultUrl TEXT,
    ErrorMessage TEXT,
    CompletedAt TIMESTAMP,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ApiResourceId) REFERENCES ApiResources(Id)
);

-- Disputes 表
CREATE TABLE Disputes (
    Id UUID PRIMARY KEY,
    UserId UUID NOT NULL,
    TaskId UUID NOT NULL,
    Type INT NOT NULL,
    Description TEXT NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    HandledBy UUID,
    Resolution TEXT,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (TaskId) REFERENCES GenerationTasks(Id),
    FOREIGN KEY (HandledBy) REFERENCES Users(Id)
);
```

---

## 团队分工

### 团队配置（7人）

| 角色 | 人数 | 主要职责 | 技能要求 |
|------|------|----------|----------|
| **项目负责人 (PM)** | 1人 | 整体协调、需求把控、风险管理 | 技术背景、沟通能力 |
| **后端开发** | 2人 | API开发、业务逻辑、数据库设计 | .NET Core, EF Core, SQL |
| **前端开发** | 2人 | UI/UX实现、状态管理、API集成 | Vue 3, TypeScript, Vuetify |
| **调研与测试** | 1人 | AIGC平台调研、合规性分析、功能测试 | 调研能力、测试思维 |
| **全栈/DevOps** | 1人 | 部署配置、CI/CD、问题修复 | 全栈能力、运维经验 |

### 具体分工

#### 👨‍💼 岗位1: 项目负责人 (PM + 产品)

**姓名**: _________

**主要任务**:
1. **需求管理** (10%)
   - 维护 SPEC 文档，细化用户故事
   - 编写 API 设计文档

2. **项目管理** (40%)
   - 制定并更新开发计划
   - 每日站会主持（15分钟）
   - 周进度跟踪与风险识别

3. **技术支持** (30%)
   - Code Review
   - 协助后端复杂模块开发
   - 数据库设计与优化

4. **对外沟通** (20%)
   - 周报编写
   - 答辩材料准备

**交付物**:
- API 设计文档
- 每周进度报告
- 最终答辩PPT

---

#### 💻 岗位2-3: 后端开发 (Backend Dev)

**成员**: _________ , _________

**职责分工**:

**后端开发1 - 核心业务**:
- Identity Service (认证授权)
- Wallet Service (钱包与交易)
- 数据库迁移与种子数据
- JWT 中间件配置

**后端开发2 - 资源调度**:
- Resource Gateway Service (API接入)
- Generation Service (任务调度)
- 外部 API 客户端封装（Stability AI）
- 密钥加密模块

**共同任务**:
- API 端点开发
- 单元测试编写（核心模块 >70% 覆盖率）
- API 文档维护（Swagger/Scalar）

**技术要点**:
```csharp
// 依赖注入示例
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IResourceGateway, ResourceGateway>();
builder.Services.AddSingleton<IApiKeyEncryption, ApiKeyEncryption>();

// 中间件管道
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiting(); // 限流
app.UseExceptionHandler(); // 统一异常处理
```

**交付物**:
- 完整的 RESTful API
- 单元测试报告
- API 文档

---

#### 🎨 岗位4-5: 前端开发 (Frontend Dev)

**成员**: _________ , _________

**职责分工**:

**前端开发1 - 用户端**:
- 注册/登录页面
- 个人中心（含钱包）
- 图片生成界面（核心功能）
- 生成历史页面

**前端开发2 - 资源端**:
- 资源接入向导
- 资源管理仪表盘
- 收益统计页面
- 管理后台界面（基础版）

**共同任务**:
- 状态管理（Pinia stores）
- API Service 封装
- 响应式设计
- 暗黑模式支持

**技术要点**:
```typescript
// API Service 示例
export const walletService = {
  getBalance: () => api.get<WalletBalance>('/api/wallet/balance'),
  recharge: (amount: number) => api.post('/api/wallet/recharge', { amount }),
  getTransactions: (page: number) => api.get(`/api/wallet/transactions?page=${page}`)
};

// Pinia Store 示例
export const useWalletStore = defineStore('wallet', () => {
  const balance = ref(0);
  const transactions = ref<Transaction[]>([]);

  async function fetchBalance() {
    const res = await walletService.getBalance();
    balance.value = res.data.balance;
  }

  return { balance, transactions, fetchBalance };
});
```

**交付物**:
- 完整的前端应用
- 组件库文档
- UI/UX 截图集

---

#### 🔍 岗位6: 调研与测试 (Research & QA)

**姓名**: _________

**主要任务**:

**第一周 - 深度调研** (60小时):

1. **AIGC 平台调研** (20小时)
   - 调研至少 10 个主流 AIGC 平台：
     - Stability AI
     - OpenAI (DALL-E)
     - Midjourney (仅了解ToS，不集成)
     - Replicate
     - Hugging Face Inference API
     - RunPod
     - Together AI
     - Anthropic (Claude)
     - Google (Gemini)
     - 国内：通义万相、文心一言、智谱清言等

   - 每个平台记录：
     - 官方 API 文档链接
     - 定价模式（按次/按Token/月付）
     - API 限流规则
     - 是否支持第三方集成
     - 示例代码

   - 输出文档：`AIGC_Platforms_Research.md`

2. **合规性调研** (15小时)
   - 研读各平台的 Terms of Service (ToS)
   - 识别禁止行为：
     - 账号共享
     - 自动化滥用
     - 转售限制
   - 合规接入方案：
     - 官方 API Key 托管 ✅
     - 用户自有账号授权 ✅
     - 代理/爬虫 ❌

   - 输出文档：`Compliance_Analysis.md`

3. **可集成性技术验证** (15小时)
   - 编写 Stability AI 集成 Demo（.NET）
   - 编写 OpenAI DALL-E 集成 Demo
   - 测试 API 响应时间、错误处理
   - 验证密钥托管安全性

   - 输出代码：`/demos/` 目录

4. **竞品分析** (10小时)
   - 类似平台调研：
     - Replicate (API marketplace)
     - RunPod (GPU租赁)
     - Together AI (模型托管)
   - 分析定价策略、用户体验

   - 输出文档：`Competitor_Analysis.md`

**第二周 - 测试验证** (40小时):

1. **功能测试** (20小时)
   - 编写测试用例（至少50个）
   - 手动测试所有功能
   - Bug 记录与跟踪（使用 GitHub Issues）

2. **集成测试** (10小时)
   - API 端到端测试
   - 支付流程测试（沙箱环境）
   - 性能测试（JMeter/K6）

3. **安全测试** (10小时)
   - SQL 注入测试
   - XSS 测试
   - API 密钥泄露风险测试
   - OWASP Top 10 检查

**工具**:
- Postman (API测试)
- Selenium/Playwright (E2E测试)
- OWASP ZAP (安全扫描)

**交付物**:
- 调研报告（4份文档）
- 技术验证 Demo
- 测试用例表
- Bug 报告

---

#### 🛠️ 岗位7: 全栈/DevOps (Full Stack / DevOps)

**姓名**: _________

**主要任务**:

1. **开发环境配置** (Week 1, 10小时)
   - Docker Compose 编写（后端+数据库+Redis）
   - 本地开发环境文档完善
   - Git 工作流规范（分支策略）

2. **CI/CD 搭建** (Week 1-2, 15小时)
   - GitHub Actions 配置
   - 自动化测试流水线
   - 自动化部署脚本

3. **问题修复与支持** (Week 2-3, 30小时)
   - 协助前后端解决技术难题
   - Code Review
   - 性能优化
   - Bug 修复

4. **部署上线** (Week 3, 15小时)
   - 生产环境部署（阿里云/腾讯云）
   - 域名配置与 HTTPS
   - 数据库备份策略
   - 监控告警（Grafana/Prometheus）

**技术栈**:
- Docker & Docker Compose
- GitHub Actions
- Nginx
- Let's Encrypt (SSL)
- 云服务（ECS）

**交付物**:
- Docker 配置文件
- CI/CD 流水线
- 部署文档
- 生产环境访问地址

---

## 开发计划

### 时间轴（2025年10月19日 - 11月9日）

```
Week 5 (Oct 19-25): 调研 + 架构搭建
Week 6 (Oct 26-Nov 1): 核心功能开发
Week 7 (Nov 2-8): 集成测试 + 优化
Week 8 (Nov 9): 最终验收与答辩准备
```

---

### 第一周 (Oct 19-25): 调研与架构搭建

**目标**: 完成技术调研、数据库设计、项目脚手架搭建

#### 周日 (Oct 19)
- **所有人**: 项目启动会（2小时）
  - 宣读 SPEC
  - 分工确认
  - 开发环境配置
  - Git 仓库初始化

#### 周一-周二 (Oct 20-21)
- **PM**:
  - 编写 API 设计文档
  - 设计数据库 ER 图
  - 创建 GitHub Project 看板

- **后端团队**:
  - 搭建 .NET 项目结构
  - 配置 EF Core + 数据库迁移
  - 实现 User 和 Wallet 模型
  - 完成 JWT 认证中间件

- **前端团队**:
  - 初始化 Vue 3 项目
  - 配置 Vuetify + 路由
  - 实现登录/注册页面 UI
  - 封装 Axios + 拦截器

- **调研**:
  - 完成 10+ AIGC 平台调研
  - 输出《AIGC_Platforms_Research.md》

- **DevOps**:
  - 编写 Docker Compose
  - 配置 GitHub Actions

**交付**: 项目脚手架、调研文档初稿

#### 周三-周四 (Oct 22-23)
- **后端1**:
  - 完成 Identity Service (注册/登录 API)
  - 完成 Wallet Service (余额查询 API)
  - 单元测试（JWT、密码加密）

- **后端2**:
  - 完成 ApiKeyEncryption 模块
  - 实现 Resource Gateway 接入逻辑
  - Stability AI 客户端封装

- **前端1**:
  - 对接登录/注册 API
  - 实现个人中心页面
  - 钱包余额展示

- **前端2**:
  - 实现资源接入向导界面
  - 资源列表页面

- **调研**:
  - 完成合规性分析文档
  - 编写 Stability AI Demo

- **DevOps**:
  - 协助解决 CORS 问题
  - 配置开发环境文档

**交付**: 认证模块、钱包模块、资源接入 UI

#### 周五-周六 (Oct 24-25)
- **后端1**:
  - 完成 Wallet Recharge API（支付宝沙箱集成）
  - Transaction 记录与查询

- **后端2**:
  - 完成 Generation Service 核心逻辑
  - 智能路由选择算法
  - 任务队列（简单版）

- **前端1**:
  - 实现图片生成界面（核心功能）
  - 结果展示与下载

- **前端2**:
  - 资源仪表盘（统计数据可视化）

- **调研**:
  - OpenAI 集成 Demo
  - 竞品分析文档

- **PM**:
  - 第一周总结会
  - Code Review

**交付**: 端到端文生图流程可运行

---

### 第二周 (Oct 26 - Nov 1): 核心功能开发

**目标**: 完成 MVP 所有核心功能，实现完整业务闭环

#### 周一 (Oct 26)
- **后端1**:
  - 完善交易流水 API
  - 实现佣金自动计算
  - 退款逻辑

- **后端2**:
  - 完善 Generation Service
  - 失败重试机制
  - 资源池同步（定时任务）

- **前端1**:
  - 生成历史页面
  - 交易流水页面

- **前端2**:
  - 资源管理（暂停/启用/删除）
  - 收益统计页面

- **调研→测试**:
  - 转入测试角色
  - 编写测试用例（50+）

#### 周二-周三 (Oct 27-28)
- **后端团队**:
  - Admin Service 基础功能
  - Dispute 争议处理 API
  - 系统监控接口（/health, /metrics）
  - API 文档完善

- **前端团队**:
  - 管理后台界面（用户管理、交易监控）
  - 争议处理界面
  - 响应式优化（移动端适配）

- **测试**:
  - 功能测试（完成 30+ 用例）
  - Bug 记录与反馈

- **DevOps**:
  - 性能优化（SQL 查询、API 响应时间）
  - Redis 集成（可选）

**交付**: 管理后台、争议处理流程

#### 周四-周五 (Oct 29-30)
- **全员**:
  - Bug 修复（高优先级）
  - 功能细节打磨
  - 用户体验优化

- **后端**:
  - 日志系统完善
  - 异常处理统一
  - 安全加固（SQL注入、XSS防护）

- **前端**:
  - Loading 状态完善
  - 错误提示优化
  - 暗黑模式调试

- **测试**:
  - 集成测试（API 端到端）
  - 安全测试（OWASP）

#### 周六-周日 (Oct 31 - Nov 1)
- **后端**:
  - 性能测试与优化
  - 数据库索引优化
  - API 限流配置

- **前端**:
  - UI/UX 最终调整
  - 截图与录屏准备

- **测试**:
  - 回归测试
  - 压力测试（100并发）

- **DevOps**:
  - 预生产环境部署
  - 备份恢复演练

- **PM**:
  - 第二周总结
  - 准备演示 Demo

**交付**: 功能完整的 MVP 系统

---

### 第三周 (Nov 2-8): 测试优化与上线

**目标**: 全面测试、优化、部署到生产环境

#### 周一-周二 (Nov 2-3)
- **测试**:
  - 完整的功能测试（所有用例）
  - 性能测试报告
  - 安全测试报告

- **开发团队**:
  - Bug 修复（中低优先级）
  - 代码重构与优化
  - 文档完善

- **DevOps**:
  - 生产环境部署
  - HTTPS 配置
  - 域名解析

#### 周三-周四 (Nov 4-5)
- **全员**:
  - 生产环境验证测试
  - 性能监控配置
  - 数据备份验证

- **PM**:
  - 编写答辩 PPT
  - 准备演示脚本
  - 整理项目文档

#### 周五 (Nov 6)
- **全员**:
  - 最终 Bug 修复
  - 代码 Freeze（冻结）
  - 答辩彩排

#### 周六-周日 (Nov 7-8)
- **PM + 核心成员**:
  - 答辩材料最终确认
  - Demo 演练
  - Q&A 准备

#### 周一 (Nov 9)
- **全员**: 项目答辩 🎉

---

## 里程碑与交付物

### Milestone 1: 架构搭建完成 (Oct 25)

**验收标准**:
- ✅ 用户可以注册、登录
- ✅ 可以查看钱包余额
- ✅ 可以接入 Stability AI 密钥
- ✅ 可以生成一张图片（端到端）

**交付物**:
- 项目代码（GitHub）
- 数据库迁移脚本
- AIGC 平台调研报告
- API 设计文档

---

### Milestone 2: MVP 功能完整 (Nov 1)

**验收标准**:
- ✅ 完整的用户注册/登录流程
- ✅ Credits 充值（支付宝沙箱）
- ✅ 资源提供者接入 API 并验证
- ✅ 效率追求者生成图片并扣费
- ✅ 交易流水清晰可查
- ✅ 管理后台可处理争议

**交付物**:
- 完整的 Web 应用（前后端）
- API 文档（Scalar/Swagger）
- 测试报告（功能+安全）
- 用户手册

---

### Milestone 3: 生产就绪 (Nov 8)

**验收标准**:
- ✅ 部署到生产环境（有公网访问地址）
- ✅ 所有关键 Bug 已修复
- ✅ 性能达标（API P95 < 500ms）
- ✅ 安全审计通过（无高危漏洞）
- ✅ 监控告警配置完成

**交付物**:
- 生产环境访问地址
- 部署文档
- 运维手册
- 答辩 PPT
- 项目总结报告

---

## 风险评估

### 技术风险

| 风险项 | 概率 | 影响 | 应对措施 |
|--------|------|------|----------|
| **API 密钥加密安全性** | 中 | 高 | 使用 AES-256 + Key Vault，定期安全审计 |
| **支付集成复杂度** | 中 | 中 | 先用沙箱环境，预留 2 天调试时间 |
| **Stability AI 限流** | 低 | 中 | 实现智能重试机制，监控 API 额度 |
| **数据库性能瓶颈** | 低 | 中 | 合理索引设计，考虑 Redis 缓存 |
| **前后端联调问题** | 高 | 低 | 提前约定 API 契约，使用 Mock 数据 |

### 时间风险

| 风险项 | 概率 | 影响 | 应对措施 |
|--------|------|------|----------|
| **功能超范围** | 高 | 高 | 严格遵守 MVP 范围，P2 功能坚决不做 |
| **Bug 修复耗时过长** | 中 | 中 | 优先修复高优先级 Bug，低优先级可延后 |
| **团队成员请假** | 低 | 中 | 交叉培训，每人至少了解 2 个模块 |
| **第三方服务不稳定** | 低 | 低 | 有降级方案（如支付宝不行改微信） |

### 合规风险

| 风险项 | 概率 | 影响 | 应对措施 |
|--------|------|------|----------|
| **违反 AIGC 平台 ToS** | 低 | 高 | 只集成官方 API，不做账号共享/爬虫 |
| **用户数据泄露** | 低 | 高 | 全程加密，定期备份，访问控制 |
| **支付合规问题** | 低 | 中 | MVP 仅充值不提现，避免金融牌照问题 |

---

## 开发规范

### Git 工作流

```bash
main          # 生产分支（保护分支）
  ├── develop # 开发主分支
      ├── feature/auth        # 功能分支
      ├── feature/wallet      # 功能分支
      ├── feature/resource    # 功能分支
      └── bugfix/login-error  # Bug 修复分支
```

**提交规范**:
```
feat: 新增资源接入功能
fix: 修复登录 Token 过期问题
docs: 更新 API 文档
refactor: 重构 Wallet Service
test: 添加 Generation Service 单元测试
chore: 更新依赖包
```

### Code Review 标准

- 所有 PR 必须至少 1 人 Review
- 核心模块（Wallet, Resource Gateway）需 2 人 Review
- Review 重点：
  - 安全性（SQL 注入、XSS、密钥泄露）
  - 性能（N+1 查询、循环调用）
  - 可读性（命名、注释）
  - 测试覆盖率

### 测试策略

**单元测试**:
- 后端核心 Service 覆盖率 > 70%
- 使用 xUnit + Moq

**集成测试**:
- 关键 API 端到端测试
- 使用 WebApplicationFactory

**E2E 测试**:
- 核心用户流程自动化
- 使用 Playwright/Cypress

---

## 成功标准

### 功能完整性
- ✅ 实现 SPEC 中所有 P0 需求（MVP）
- ✅ 用户可以注册、充值、生成图片
- ✅ 资源提供者可以接入 API 并获得收益
- ✅ 管理员可以处理争议

### 技术指标
- ✅ API 响应时间 P95 < 500ms
- ✅ 系统可用性 > 99%（测试期间）
- ✅ 无高危安全漏洞
- ✅ 单元测试覆盖率 > 70%

### 用户体验
- ✅ 界面美观、响应式
- ✅ 关键流程不超过 3 步
- ✅ 错误提示清晰友好
- ✅ Loading 状态明确

### 文档完整性
- ✅ API 文档自动生成
- ✅ 部署文档完整可执行
- ✅ 用户手册简洁易懂
- ✅ 代码关键部分有注释

---

## 附录

### A. 技术选型备选方案

| 模块 | 首选方案 | 备选方案 | 选择理由 |
|------|----------|----------|----------|
| 数据库 | PostgreSQL | MySQL | PostgreSQL JSON 支持更好 |
| 缓存 | Redis | Memcached | Redis 功能更丰富（任务队列） |
| 支付 | 支付宝 | 微信支付 | 国内主流，文档完善 |
| 文件存储 | 本地 | 阿里云 OSS | MVP 本地够用，节省成本 |
| 密钥管理 | AES-256 | Azure Key Vault | MVP 本地加密，生产用云 KMS |

### B. 开发环境配置清单

**后端开发**:
- Visual Studio Code / JetBrains Rider
- .NET 8 SDK
- EF Core Tools: `dotnet tool install --global dotnet-ef`
- PostgreSQL / SQLite

**前端开发**:
- Visual Studio Code
- Node.js 18+
- pnpm
- Vue DevTools (浏览器扩展)

**测试工具**:
- Postman / Insomnia (API 测试)
- OWASP ZAP (安全扫描)
- K6 / JMeter (性能测试)

**DevOps 工具**:
- Docker & Docker Compose
- GitHub CLI
- SSH 客户端

### C. 参考资源

**技术文档**:
- [ASP.NET Core 官方文档](https://docs.microsoft.com/aspnet/core/)
- [Vue 3 官方文档](https://vuejs.org/)
- [Entity Framework Core 文档](https://docs.microsoft.com/ef/core/)
- [Stability AI API 文档](https://platform.stability.ai/docs/api-reference)

**最佳实践**:
- [OWASP API Security Top 10](https://owasp.org/www-project-api-security/)
- [Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines)
- [Vuetify Best Practices](https://vuetifyjs.com/en/introduction/best-practices/)

**竞品研究**:
- [Replicate](https://replicate.com/)
- [RunPod](https://www.runpod.io/)
- [Together AI](https://www.together.ai/)

---

## 结语

Aetherflow MVP 的成功关键在于：

1. **聚焦核心价值**：验证"资源托管→按需消费→价值流动"这一核心假设
2. **快速迭代**：2-3 周完成 MVP，快速获得用户反馈
3. **团队协作**：清晰分工，高效沟通，互相支持
4. **质量保证**：虽然是 MVP，但安全性、稳定性不能妥协

**让我们一起打造 AIGC 领域的价值交换层！🌊**

---

**文档维护**:
- 本文档为活文档，随项目进展持续更新
- 每周五 PM 负责更新进度和风险评估
- 所有成员可通过 PR 方式提出修改建议

**联系方式**:
- 项目看板：GitHub Projects
- 即时通讯：微信群/Discord
- 代码仓库：https://github.com/zzw4257/ImageGenerator (aetherflow/mvp 分支)

**最后更新**: 2025-10-19
**下次审查**: 2025-10-25 (Week 1 回顾)
