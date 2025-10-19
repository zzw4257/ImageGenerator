# Aetherflow 团队任务分工清单 ✅

**项目周期**: 2024年10月19日 - 11月8日 (3周)  
**团队规模**: 7人  
**今日日期**: 2024-10-19 (第5周周日)  
**开发模式**: AI辅助编程 + 快速迭代

---

## 🎯 核心原则

1. **MVP为王**：只做必要功能，拒绝功能蔓延
2. **快速验证**：每周至少一次可演示的 Demo
3. **AI辅助**：善用 Cursor、GitHub Copilot、ChatGPT
4. **异步协作**：任务分解清晰，减少等待依赖
5. **每日同步**：晚上20:00线上站会（15分钟）

---

## 👥 团队配置与联系方式

| # | 角色 | 姓名 | 微信/邮箱 | 主要技能 |
|---|------|------|-----------|----------|
| 1 | 项目负责人 (PM) | ________ | ________ | 全栈、协调 |
| 2 | 后端开发1 (BE1) | ________ | ________ | .NET、数据库 |
| 3 | 后端开发2 (BE2) | ________ | ________ | .NET、API |
| 4 | 前端开发1 (FE1) | ________ | ________ | Vue、UI |
| 5 | 前端开发2 (FE2) | ________ | ________ | Vue、状态管理 |
| 6 | 调研与测试 (QA) | ________ | ________ | 调研、测试 |
| 7 | DevOps工程师 (OPS) | ________ | ________ | 部署、运维 |

---

## 📅 第一周任务 (10月19日 - 10月25日)

### 🎯 本周目标
**端到端跑通一次完整流程**：用户注册 → 接入API → 生成图片 → 扣费

---

### 👨‍💼 角色1: 项目负责人 (PM)

**本周工作量**: 20小时

#### 周日 10/19 (今天！)
- [ ] 组织项目启动会（晚上20:00，2小时）
  - [ ] 宣读 SPEC，确认目标
  - [ ] 确认每个人的分工和联系方式
  - [ ] 建立微信群 + GitHub 仓库权限
  - [ ] 约定每日站会时间（建议晚上20:00）
- [ ] 创建 GitHub Project 看板
  - [ ] 列出所有任务卡片
  - [ ] 分配给对应人员
- [ ] 设计数据库 ER 图（初稿）

#### 周一 10/20
- [ ] 完善数据库设计（Users, Wallets, Transactions, ApiResources, GenerationTasks）
- [ ] 编写 API 设计文档（至少 10 个端点）
- [ ] Review 后端的项目结构

#### 周二 10/21
- [ ] Review 后端的数据模型定义
- [ ] Review 前端的路由设计
- [ ] 协调前后端 API 契约（用 Postman Mock）

#### 周三-周四 10/22-23
- [ ] Code Review（每天至少 30 分钟）
- [ ] 协助解决技术难题
- [ ] 更新任务进度

#### 周五 10/24
- [ ] 第一周总结会（晚上）
- [ ] 更新风险清单
- [ ] 准备周末演示 Demo

#### 周六-周日 10/25-26
- [ ] 端到端测试
- [ ] Bug 修复协调
- [ ] 准备第二周计划

**本周交付物**:
- ✅ 数据库 ER 图
- ✅ API 设计文档
- ✅ 第一周进度报告

---

### 💻 角色2: 后端开发1 (BE1) - 核心业务

**本周工作量**: 25小时  
**主攻方向**: 用户系统 + 钱包系统

#### 周日 10/19
- [ ] 配置本地开发环境（.NET 8 + VSCode/Rider）
- [ ] 拉取 ImageGenerator 代码，熟悉项目结构
- [ ] 参加项目启动会

#### 周一 10/20
- [ ] 创建新分支 `aetherflow/mvp`
- [ ] 定义数据模型：
  - [ ] `User.cs`（扩展：Email, Role, WalletId）
  - [ ] `Wallet.cs`（Balance, UserId）
  - [ ] `Transaction.cs`（Type, Amount, BalanceAfter）
- [ ] 配置 EF Core DbContext
- [ ] 创建数据库迁移：`dotnet ef migrations add InitAetherflow`

#### 周二 10/21
- [ ] 实现 `IAuthenticationService`
  - [ ] `RegisterAsync(email, password)` - 邮箱注册
  - [ ] `LoginAsync(email, password)` - 返回 JWT
- [ ] 实现 `AuthenticationController`
  - [ ] `POST /api/auth/register`
  - [ ] `POST /api/auth/login`
- [ ] 配置 JWT 中间件（appsettings.json）

#### 周三 10/22
- [ ] 实现 `IWalletService`
  - [ ] `GetBalanceAsync(userId)` - 查询余额
  - [ ] `GetTransactionsAsync(userId, page)` - 交易流水
  - [ ] `AddTransactionAsync(...)` - 记录交易
- [ ] 实现 `WalletController`
  - [ ] `GET /api/wallet/balance`
  - [ ] `GET /api/wallet/transactions`

#### 周四 10/23
- [ ] 实现充值逻辑（先用模拟，不集成真实支付）
  - [ ] `RechargeAsync(userId, amount)` - 增加余额
  - [ ] `POST /api/wallet/recharge`
- [ ] 编写单元测试：
  - [ ] `AuthenticationServiceTests.cs`
  - [ ] `WalletServiceTests.cs`

#### 周五 10/24
- [ ] 完善交易记录（自动计算 BalanceAfter）
- [ ] 实现佣金计算逻辑（5% 平台抽成）
- [ ] Code Review 自查

#### 周末 10/25-26
- [ ] Bug 修复
- [ ] 协助前端联调登录/钱包接口

**本周交付物**:
- ✅ 用户注册/登录 API（可用 Postman 测试）
- ✅ 钱包余额/流水 API
- ✅ 数据库迁移脚本

**技术提示**:
```csharp
// 示例：JWT 生成
var claims = new[] {
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Role, user.Role.ToString())
};
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddHours(24), signingCredentials: creds);
```

---

### 💻 角色3: 后端开发2 (BE2) - 资源调度

**本周工作量**: 25小时  
**主攻方向**: API 资源接入 + 图片生成

#### 周日 10/19
- [ ] 配置开发环境
- [ ] 注册 Stability AI 账号，获取测试 API Key
- [ ] 参加项目启动会

#### 周一 10/20
- [ ] 实现 API 密钥加密模块 `ApiKeyEncryption.cs`
  - [ ] `Encrypt(plainKey)` - AES-256 加密
  - [ ] `Decrypt(encrypted)` - 解密
  - [ ] 从环境变量读取主密钥
- [ ] 编写单元测试验证加密解密

#### 周二 10/21
- [ ] 定义数据模型：
  - [ ] `ApiResource.cs`（Provider, ApiKeyEncrypted, Status, AvailableCredits）
  - [ ] `GenerationTask.cs`（Prompt, Status, CreditsCost, ResultUrl）
- [ ] 创建数据库迁移

#### 周三 10/22
- [ ] 实现 Stability AI 客户端封装
  - [ ] `StabilityAIClient.cs`
  - [ ] `GenerateImageAsync(apiKey, prompt, params)` 
  - [ ] 错误处理（超时、限流、无效密钥）
- [ ] 编写集成测试（用真实 API Key）

#### 周四 10/23
- [ ] 实现 `IResourceGatewayService`
  - [ ] `ConnectApiAsync(userId, provider, apiKey)` - 接入 API
  - [ ] `ValidateApiKeyAsync(provider, apiKey)` - 验证有效性
  - [ ] `GetUserResourcesAsync(userId)` - 用户的资源列表
- [ ] 实现 `ResourceController`
  - [ ] `POST /api/resource/connect`
  - [ ] `GET /api/resource/list`

#### 周五 10/24
- [ ] 实现 `IGenerationService`
  - [ ] `GenerateImageAsync(userId, prompt, params)` - 生成图片
  - [ ] 智能路由：选择可用的 ApiResource
  - [ ] 扣费逻辑（调用 WalletService）
  - [ ] 保存结果到本地文件系统
- [ ] 实现 `GenerationController`
  - [ ] `POST /api/generate/text-to-image`
  - [ ] `GET /api/generate/task/{id}`

#### 周末 10/25-26
- [ ] 完善失败重试机制
- [ ] 优化资源选择算法（价格优先）
- [ ] 协助前端联调生成接口

**本周交付物**:
- ✅ 资源接入 API（可以添加 Stability AI Key）
- ✅ 图片生成 API（端到端可用）
- ✅ 图片保存到 `images/` 目录

**技术提示**:
```csharp
// 示例：Stability AI 调用
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
var payload = new { prompt = "a cat", width = 512, height = 512 };
var response = await client.PostAsJsonAsync("https://api.stability.ai/v1/generation/stable-diffusion-xl-1024-v1-0/text-to-image", payload);
var result = await response.Content.ReadFromJsonAsync<StabilityResponse>();
```

---

### 🎨 角色4: 前端开发1 (FE1) - 用户端

**本周工作量**: 25小时  
**主攻方向**: 用户注册/登录 + 图片生成界面

#### 周日 10/19
- [ ] 配置开发环境（Node.js, pnpm, VSCode）
- [ ] 拉取 WebUI 代码，熟悉项目结构
- [ ] 参加项目启动会

#### 周一 10/20
- [ ] 创建新页面组件：
  - [ ] `src/pages/auth/register.vue` - 注册页
  - [ ] `src/pages/auth/login.vue` - 登录页
- [ ] 配置路由 `router/index.ts`
- [ ] 设计注册/登录表单 UI（用 Vuetify）

#### 周二 10/21
- [ ] 封装 Auth API Service：
  - [ ] `src/services/auth.ts`
  - [ ] `register(email, password)`
  - [ ] `login(email, password)` 
  - [ ] 保存 JWT 到 localStorage
- [ ] 实现登录状态管理（Pinia）
  - [ ] `src/stores/auth.ts`
  - [ ] `currentUser`, `isLoggedIn`, `logout()`

#### 周三 10/22
- [ ] 对接后端登录/注册 API
- [ ] 实现 Axios 拦截器（自动添加 JWT Header）
- [ ] 实现路由守卫（未登录跳转到登录页）

#### 周四 10/23
- [ ] 创建个人中心页面 `src/pages/profile.vue`
  - [ ] 显示邮箱、用户ID
  - [ ] 显示钱包余额
  - [ ] 充值按钮（先只是个按钮，功能下周做）
- [ ] 封装 Wallet API Service
  - [ ] `src/services/wallet.ts`
  - [ ] `getBalance()`, `getTransactions()`

#### 周五 10/24
- [ ] **核心功能**：创建图片生成页面 `src/pages/generate.vue`
  - [ ] Prompt 输入框（大文本域）
  - [ ] 参数选择（宽度、高度、模型）
  - [ ] "生成"按钮（显示消耗 Credits）
  - [ ] 结果展示区（Loading + 图片预览）
- [ ] 封装 Generation API Service
  - [ ] `src/services/generation.ts`
  - [ ] `generateImage(prompt, params)`

#### 周末 10/25-26
- [ ] 优化生成页面交互：
  - [ ] Loading 动画
  - [ ] 错误提示（余额不足、生成失败）
  - [ ] 图片下载功能
- [ ] 创建生成历史页面 `src/pages/history.vue`（简单列表）
- [ ] Bug 修复

**本周交付物**:
- ✅ 登录/注册页面（可用）
- ✅ 图片生成页面（核心功能）
- ✅ 个人中心页面

**技术提示**:
```typescript
// 示例：Axios 拦截器
axios.interceptors.request.use(config => {
  const token = localStorage.getItem('jwt_token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// 示例：生成 API 调用
const generateImage = async (prompt: string) => {
  loading.value = true;
  try {
    const res = await api.post('/api/generate/text-to-image', { prompt });
    imageUrl.value = res.data.resultUrl;
  } catch (err) {
    showError(err.message);
  } finally {
    loading.value = false;
  }
};
```

---

### 🎨 角色5: 前端开发2 (FE2) - 资源端

**本周工作量**: 25小时  
**主攻方向**: 资源接入界面 + 仪表盘

#### 周日 10/19
- [ ] 配置开发环境
- [ ] 熟悉 Vuetify 组件库文档
- [ ] 参加项目启动会

#### 周一 10/20
- [ ] 创建资源管理页面 `src/pages/resources/index.vue`
- [ ] 设计页面布局（列表 + 统计卡片）
- [ ] 学习 Vuetify Data Table 组件

#### 周二 10/21
- [ ] 创建资源接入向导 `src/pages/resources/connect.vue`
  - [ ] 步骤1: 选择平台（Stability AI / OpenAI）
  - [ ] 步骤2: 输入 API Key
  - [ ] 步骤3: 验证（显示额度）
  - [ ] 步骤4: 完成提示
- [ ] 使用 Vuetify Stepper 组件

#### 周三 10/22
- [ ] 封装 Resource API Service
  - [ ] `src/services/resource.ts`
  - [ ] `connectApi(provider, apiKey)`
  - [ ] `getResourceList()`
  - [ ] `updateResourceStatus(id, status)`
- [ ] 对接后端资源接入 API

#### 周四 10/23
- [ ] 实现资源列表页面：
  - [ ] 表格显示（平台、状态、可用额度、总额度）
  - [ ] 操作按钮（暂停、启用、删除）
  - [ ] 状态标签（Active/Paused/Error）
- [ ] 实现状态切换功能

#### 周五 10/24
- [ ] 创建资源详情/统计页面 `src/pages/resources/[id].vue`
  - [ ] 显示资源信息
  - [ ] 消耗趋势图表（Chart.js / ECharts）
  - [ ] 收益统计
- [ ] 优化 UI 细节

#### 周末 10/25-26
- [ ] 添加响应式设计（移动端适配）
- [ ] 完善交互细节（Loading、Toast提示）
- [ ] Bug 修复

**本周交付物**:
- ✅ 资源接入向导（完整流程）
- ✅ 资源管理列表页
- ✅ 资源详情页

**技术提示**:
```vue
<!-- 示例：资源接入向导 -->
<template>
  <v-stepper v-model="step">
    <v-stepper-header>
      <v-stepper-item :value="1" title="选择平台"></v-stepper-item>
      <v-stepper-item :value="2" title="输入密钥"></v-stepper-item>
      <v-stepper-item :value="3" title="验证"></v-stepper-item>
    </v-stepper-header>
    <v-stepper-window>
      <v-stepper-window-item :value="1">
        <v-card>选择 Stability AI 或 OpenAI</v-card>
      </v-stepper-window-item>
      <!-- ... -->
    </v-stepper-window>
  </v-stepper>
</template>
```

---

### 🔍 角色6: 调研与测试 (QA)

**本周工作量**: 30小时  
**主攻方向**: AIGC 平台深度调研

#### 周日 10/19
- [ ] 参加项目启动会
- [ ] 制定调研计划和模板

#### 周一-周二 10/20-21 (16小时)
- [ ] **AIGC 平台调研**（至少10个平台）
  - [ ] Stability AI ⭐
  - [ ] OpenAI DALL-E
  - [ ] Replicate
  - [ ] Hugging Face Inference API
  - [ ] RunPod
  - [ ] Together AI
  - [ ] Google Gemini (Imagen)
  - [ ] Anthropic Claude (图片理解)
  - [ ] 通义万相
  - [ ] 文心一格

- **每个平台记录**：
  - [ ] 官方 API 文档 URL
  - [ ] 注册流程（需要信用卡吗？）
  - [ ] 定价模型（每张图/每次调用/月费）
  - [ ] 免费额度（新用户有多少？）
  - [ ] API 限流规则（QPM, QPD）
  - [ ] 是否支持第三方集成
  - [ ] ToS 中的关键条款
  - [ ] Python/C# SDK 是否可用
  - [ ] 示例代码（复制一份）

- [ ] 输出文档：`AIGC_Platforms_Research.md`（Markdown 表格格式）

#### 周三 10/22 (8小时)
- [ ] **合规性分析**
  - [ ] 仔细阅读 Stability AI ToS
  - [ ] 仔细阅读 OpenAI ToS
  - [ ] 识别禁止行为：
    - 账号共享 ❌
    - 自动化滥用 ❌
    - API Key 转售 ❌（我们是托管，用户仍是所有者）
  - [ ] 我们的方案是否合规？（写分析报告）
  - [ ] 如何避免风险？（用户协议设计）

- [ ] 输出文档：`Compliance_Analysis.md`

#### 周四 10/23 (6小时)
- [ ] **技术验证 Demo**
  - [ ] 用 Python/C# 调用 Stability AI API
  - [ ] 测试文生图（5个不同 Prompt）
  - [ ] 测试图生图
  - [ ] 测试错误处理（无效 Key, 超限流）
  - [ ] 记录响应时间（每次调用）

- [ ] 输出代码：`demos/stability_test.cs` 或 `.py`

#### 周五-周末 10/24-26
- [ ] 帮助开发团队测试功能
- [ ] 编写测试用例（10个基础用例）
- [ ] 手动测试注册/登录/生成流程
- [ ] 记录 Bug（提交 GitHub Issues）

**本周交付物**:
- ✅ `AIGC_Platforms_Research.md`（10+ 平台）
- ✅ `Compliance_Analysis.md`（合规性分析）
- ✅ 技术验证 Demo 代码
- ✅ 初步测试报告

**调研模板**:
```markdown
## Stability AI

**官网**: https://stability.ai  
**API文档**: https://platform.stability.ai/docs/api-reference  
**定价**: $10 = 1000 credits, 1张图 ≈ 0.2-0.8 credits  
**免费额度**: 新用户 25 credits  
**限流**: 150 requests/10s  
**SDK**: Python ✅ / JavaScript ✅ / C# ❌（需要自己封装）  
**ToS关键条款**: 允许API集成，禁止账号共享  
**合规性**: ✅ 我们的托管方案合规  

**示例代码**:
\`\`\`python
import requests
response = requests.post(
    "https://api.stability.ai/v1/generation/text-to-image",
    headers={"Authorization": f"Bearer {api_key}"},
    json={"prompt": "a cat", "width": 512, "height": 512}
)
\`\`\`
```

---

### 🛠️ 角色7: DevOps 工程师 (OPS)

**本周工作量**: 20小时  
**主攻方向**: 开发环境 + CI/CD

#### 周日 10/19
- [ ] 参加项目启动会
- [ ] 获取 GitHub 仓库 Admin 权限

#### 周一 10/20
- [ ] 编写 `docker-compose.yml`：
  - [ ] .NET 后端服务
  - [ ] PostgreSQL 数据库（或 SQLite）
  - [ ] Redis（可选）
  - [ ] Vue 前端服务（开发模式）
- [ ] 测试一键启动：`docker-compose up`

#### 周二 10/21
- [ ] 完善 `README.md` 开发环境配置文档
- [ ] 编写 `scripts/setup.sh`（自动安装依赖）
- [ ] 帮助团队成员配置环境（解决问题）

#### 周三 10/22
- [ ] 配置 GitHub Actions CI：
  - [ ] `.github/workflows/backend-ci.yml`
    - [ ] dotnet restore
    - [ ] dotnet build
    - [ ] dotnet test
  - [ ] `.github/workflows/frontend-ci.yml`
    - [ ] pnpm install
    - [ ] pnpm build
    - [ ] pnpm test (如果有)

#### 周四 10/23
- [ ] 协助后端解决 CORS 问题
- [ ] 协助前端解决 API 调用 404 问题
- [ ] 优化 Docker 构建速度（多阶段构建）

#### 周五 10/24
- [ ] Code Review（关注安全、性能）
- [ ] 帮助修复环境相关 Bug

#### 周末 10/25-26
- [ ] 准备预生产环境（阿里云 ECS / 腾讯云）
- [ ] 测试生产构建流程

**本周交付物**:
- ✅ `docker-compose.yml`（可用）
- ✅ GitHub Actions CI 配置
- ✅ 开发环境文档

**技术提示**:
```yaml
# docker-compose.yml 示例
version: '3.8'
services:
  backend:
    build: ./ImageGenerator
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__Source=Host=db;Database=aetherflow
      - Jwt__Secret=your-secret
    depends_on:
      - db
  
  db:
    image: postgres:15
    environment:
      POSTGRES_PASSWORD: password
    volumes:
      - db-data:/var/lib/postgresql/data
  
  frontend:
    build: ./WebUI
    ports:
      - "5173:5173"
    volumes:
      - ./WebUI:/app
    command: pnpm dev

volumes:
  db-data:
```

---

## 📅 第二周任务 (10月26日 - 11月1日)

### 🎯 本周目标
**完善所有功能，实现完整业务闭环**

### 简化版任务清单（详细任务周日晚开会确定）

#### PM
- [ ] 协调所有模块集成
- [ ] 准备中期演示 Demo
- [ ] 更新文档

#### BE1
- [ ] 完善交易流水（分页、筛选）
- [ ] 实现退款逻辑
- [ ] 支付宝沙箱集成（真实充值）

#### BE2
- [ ] 完善 Generation Service（失败重试）
- [ ] 资源池同步机制（定时任务）
- [ ] Admin API（基础版）

#### FE1
- [ ] 完善生成历史页面（分页）
- [ ] 充值功能对接
- [ ] 交易流水页面

#### FE2
- [ ] 资源收益统计页面
- [ ] 管理后台界面（用户列表、交易监控）
- [ ] 移动端适配

#### QA
- [ ] 功能测试（50+ 用例）
- [ ] 安全测试（OWASP）
- [ ] 性能测试（100并发）

#### OPS
- [ ] 性能优化（SQL查询、API响应时间）
- [ ] 生产环境部署（域名、HTTPS）
- [ ] 监控配置（日志、告警）

---

## 📅 第三周任务 (11月2日 - 11月8日)

### 🎯 本周目标
**测试、优化、上线、准备答辩**

- [ ] 全面测试（所有成员参与）
- [ ] Bug 修复（优先级排序）
- [ ] 性能优化
- [ ] 文档完善
- [ ] 答辩 PPT 准备
- [ ] 最终部署上线

---

## 🔄 每日工作流程

### 每日站会（20:00，15分钟）

**每人回答3个问题**：
1. 今天完成了什么？
2. 明天计划做什么？
3. 遇到什么阻碍？

**记录人**: PM（轮流）  
**会议记录**: 发到微信群

---

## 📊 任务追踪

### GitHub Project 看板

```
📋 Backlog       🏃 In Progress      👀 Review         ✅ Done
- 未开始的任务   - 正在做的任务      - 等待Review      
