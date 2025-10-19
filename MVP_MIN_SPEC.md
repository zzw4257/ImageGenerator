# Aetherflow MVP (Slim) – SPEC v0.2

版本: 0.2 (Slim 1-week MVP)
最后更新: 2025-10-19
分支: aetherflow/mvp

目标: 基于现有 ImageGenerator，压缩范围，1 周内完成一个可演示的“Credits 可用、生成可跑、交互可看”的端到端 Demo。

本规格强调：
- 最小后端：.NET 9、Scalar 自动 OpenAPI、SQLite、最少 5 个接口。
- 最小前端：深色 UI、三页（首页/生成/钱包+历史）、一套“预制菜（Preset）”演示交互。
- Credits 必须可用（扣款、充值/发放、流水）。
- 生成 API 可“假接”（Qwen Image、Flux 开源/本地）或“静态图占位”双通道。


--------------------------------------
## A. 团队分工参考（轻量，不做硬性任务指派）

- 项目负责人（PM，含前端展示的资产生成）：周子为
  - 关注：演示所需的预制菜（Preset）定义、Demo 脚本、风险清单。
- 后端开发 1：陈以恒
  - 关注：Credits 核心、Wallet/Transactions、生成任务与 Provider 适配层。
- 后端开发 2：罗建明
  - 关注：Scalar 文档、SQLite/迁移、Stub/本地 Provider、健康检查与日志。
- 前端开发 1（UI 向）：钱满亮
  - 关注：深色主题、卡片与列表、动效与提示、空态/错误态。
- 前端开发 2（数据+资产管理）：金裕涵
  - 关注：Preset 配置（或 JSON 驱动）、历史列表、筛选与分页、持久化。
- 前端开发 3（后端对接项）：项科深
  - 关注：API service、鉴权拦截器、Polling、错误重试、统一通知。
- 前期调研 + 后期对比测试：魏云翔
  - 关注：Qwen/Flux 接入可行性验证、性能与体验对比、Demo 可靠性测试。

> 注：以上为关注域，非硬性任务分配；实际细分由小组协商确定。


--------------------------------------
## B. 后端（.NET 9 基线）

- 运行与工具
  - 运行时：.NET 9（≥ 9）；推荐 `dotnet` CLI。
  - 文档：Scalar.AspNetCore 输出 API 文档（开发环境：`/scalar/v1`）；OpenAPI JSON 路径可固定为 `/openapi/ig.json`。
  - 存储：SQLite（开发环境），自动迁移；连接串建议 `Data Source=User.db`。
  - 工程：`.csproj` 可直接 build/run；`.sln` 仅为 VS Windows 备选。

- 模块与接口（最小集）
  1) Auth（简化）
     - POST `/api/auth/login` { username, password } -> { token }
       - 开发期可接受固定种子用户（或免注册），密码明文或固定演示密码。
  2) Wallet / Credits（必须）
     - GET `/api/wallet/balance` -> { credits }
     - GET `/api/wallet/transactions?skip=&take=` -> 列表（type: Recharge|Consume|Earn|Refund, amount, balanceAfter, createdAt, memo）
     - POST `/api/wallet/grant` { amount, memo? } -> 发放/充值（开发态替代真实支付，需校验最小/最大额度）
  3) Preset（“预制菜”）
     - GET `/api/presets` -> 列表（见 D 节 JSON Schema）
  4) Generation（必须）
     - POST `/api/generate` { presetId?|prompt, provider, params } -> { taskId, estCost }
       - 流程：费用估算 -> 余额校验 -> 预扣款（或提交成功后扣款）-> 入队/直推执行 -> 返回 taskId
     - GET `/api/generate/{taskId}` -> { status: Pending|Running|Succeeded|Failed, imageUrl?, error? }

- Credits 规则（必须落实）
  - 余额字段：`User.Credits`（int 或 decimal）。
  - 扣费时机：
    - 简化方案 A：提交任务时直接扣费（若失败则自动冲正一笔 Refund）。
    - 或方案 B：任务成功后扣费（失败不扣费）。
  - 费用估算：按 provider+参数映射固定表（如：`Qwen.TextToImage: 2 credits`，`Flux.TextToImage: 1 credit`）。
  - 交易记录：每次变动必须落账（记录 `BalanceAfter` 快照，含 memo）。
  - 幂等：请求头 `Idempotency-Key` 可选；最低限度：后端在 30s 内去重（开发期可不强制）。

- Provider 适配（双通道）
  - Interface：`IImageProvider.GenerateAsync(GenRequest, CancellationToken)`
    - `StubProvider`：返回静态图 URL（`wwwroot/images/presets/...`），用于纯演示与前端联调。
    - `HttpProvider`：对接可选服务（Qwen/Flux）
      - Qwen Image（例）：`POST https://dashscope.aliyuncs.com/api/v1/services/aigc/text2image`
      - Flux（本地/开源）：统一 JSON `{ prompt, width, height }`；返回 URL 或 Base64（建议后端转 URL）。
  - 选择策略：请求体 `provider` 指定，默认 `Stub`。

- 数据模型（最小化）
  - User { Id, Username, PasswordHash?, Credits, CreatedAt }
  - Transaction { Id, UserId, Type, Amount, BalanceAfter, Memo, CreatedAt }
  - Preset { Id, Name, CoverUrl, Prompt, Provider, DefaultParams(json), PriceCredits, Tags[] }
  - GenerationTask { Id, UserId, PresetId?, Prompt, Provider, Params(json), Status, ResultUrl?, Error?, CreatedAt, CompletedAt? }

- 配置建议（appsettings.json）
  - `CreditCosts`: Provider 与能力映射（如 TextToImage）
  - `Providers.Qwen.ApiKey`: 环境变量优先；严禁硬编码 Key
  - `StaticImagesRoot`: StubProvider 静态图目录
  - Logger：控制台 + 简单文件（按天分割，保留 7 天）


--------------------------------------
## C. 前端（Vue 3 + TS + Vuetify，深色 UI 基准）

- 页面结构（最小三页）
  1) 首页（/）
     - “预制菜 Presets” 卡片墙：封面、标题、模型标识、预计消耗（如 2 Credits）
     - 行为：点击 -> 进入生成页并预填参数
  2) 生成页（/generate）
     - 左侧控制栏：
       - Prompt（多行输入）
       - Model/Provider（下拉：Stub / Qwen / Flux）
       - Params：分辨率（固定几个），数量（先限制 1），风格（可选）
       - 费用显示：根据选择动态计算（Credits）
       - 生成按钮（Primary，高亮荧光黄绿）
     - 中间内容区：
       - 步骤提示卡片（Choose motion / Add image / Get image 的类比结构，取意不取形）
       - 生成结果：单图/网格展示、下载、收藏（可选）
     - 右侧（可选）：
       - 历史快照/最近任务
  3) 钱包与历史（/account）
     - 余额展示 + 一键发放（开发态按钮，调用 `/wallet/grant`）
     - 交易流水列表（分页）
     - 生成历史列表（任务状态、结果图、小图预览）

- UI 基准风格
  - 背景：深色（#0E0F12），卡片：#16181D
  - 字体颜色：主文字 #E6E7EA，次级 #AEB1B7，禁用 #666B73
  - 品牌点缀色：荧光黄绿（#D7FF3F / #C9FF2E），用于主按钮与高亮指标
  - 按钮：
    - Primary：荧光黄绿 + 深色/黑色文本（对比度>4.5）
    - Secondary：灰阶描边按钮
  - 卡片：阴影弱、圆角 10–12px
  - 交互：
    - 生成中：骨架屏 + Loading 条
    - 错误：顶部 Snackbar（红橙），含错误码与重试
    - 成功：底部轻提示（绿）

- 组件与模块（建议）
  - `PresetCard`：封面、标题、价格徽标、Hover 微动效
  - `GeneratePanel`：左侧控制栏（组合表单）
  - `ResultGallery`：结果展示（单图/网格），点击放大
  - `HistoryList`：任务记录（状态徽标、时间、结果链接）
  - `WalletSummary`：余额 + 发放按钮 + 最近三条流水
  - `ApiService`：Axios 实例（BaseURL、Token 拦截器、错误统一处理）
  - `useWalletStore` / `useTaskStore`：Pinia Store（余额/任务状态与轮询）

- 前端“缺啥，写啥”（关键补齐）
  - API 封装：登录、余额/流水、Presets、生成/查询任务
  - 轮询方案：`GET /generate/{taskId}` 每 2–3s，最多 30 次或任务完成即停
  - 费用提示：生成按钮旁实时显示扣费金额，余额不足禁用并提示
  - 预制菜：从 `/api/presets` 拉取，或在前端内置 JSON 兜底
  - 错误体验：统一 ErrorBanner + Snackbar；错误码映射（余额不足/Provider 不可用/超时）
  - 空态：无 Preset/无历史/无余额的友好空态插画与文案


--------------------------------------
## D. 预制菜（Preset）JSON Schema（建议）

{
  "id": "string",
  "name": "product shot - neon",
  "coverUrl": "/images/presets/neon-shoe.png",
  "provider": "Stub|Qwen|Flux",
  "prompt": "a neon-lit product photograph of a sneaker on glossy floor, cinematic lighting, high contrast, 4k",
  "priceCredits": 2,
  "defaultParams": {
    "width": 768,
    "height": 512,
    "aspectRatio": "3:2",
    "style": "cinematic"
  },
  "tags": ["product", "neon", "cinematic"]
}

- 开发期可在后端种子 6–9 个 Preset，封面图放在 `wwwroot/images/presets/`。
- 前端只读使用；如需“收藏/常用”，在本地存储或追加简表皆可。


--------------------------------------
## E. Credits 与费用映射（示例）

- CreditCosts（appsettings.json）：
  - `Stub.TextToImage = 0`（纯演示可不扣费）
  - `Qwen.TextToImage = 2`
  - `Flux.TextToImage = 1`
- 生成页：切换 Provider 与参数时，右侧即时显示“将消耗 X Credits”。
- 扣费策略（推荐简化）：提交时扣费，失败自动补一条 Refund（相同金额）。


--------------------------------------
## F. 生成 Provider 接入（参考）

- Qwen Image（阿里 DashScope）
  - 鉴权：`Authorization: Bearer ${DASHSCOPE_API_KEY}`
  - 请求字段：`prompt`, `size`/`width`/`height`, `n=1`
  - 返回：图片 URL 或 Base64（建议后端落盘并下发 URL）
- Flux（本地/开源）
  - 部署端口 `localhost:xxxx`，约定统一 JSON：`{ prompt, width, height }`
  - 返回：URL（建议后端 Proxy 为 `/images/...` 统一域）
- 超时：单次 60s，失败返回 `Failed` 状态 + 具体 error 文案


--------------------------------------
## G. Demo 脚本（演示路径）

1) 登录（或自动登录） -> 访问首页
2) 看到 6 个“预制菜”卡片 -> 点击第一个 -> 跳转生成页并预填参数
3) 调整 Provider = Flux（或 Stub） -> 显示“将消耗 1 Credits”
4) 点击“Generate” -> Loading -> 出图 -> 下载按钮可用
5) 进入“钱包与历史” -> 看到扣费记录与生成历史
6) 点击“发放 10 Credits”按钮 -> 余额增加 -> 返回生成页再来一张


--------------------------------------
## H. 一周交付清单（可演示即可）

- 后端：
  - .NET 9、Scalar、SQLite 迁移、最小 5 接口可用
  - Credits 真实扣费与流水
  - StubProvider + 至少 1 个真实 Provider（Qwen 或 Flux）打通
- 前端：
  - 深色主题 + 首页卡片墙 + 生成页 + 钱包/历史
  - 费用提示、轮询、错误/空态
  - 预制菜可用，至少 6 个卡片（封面与提示文案）
- 文档：
  - README（启动步骤 .NET 9、环境变量、pnpm）、API 路径与示例


--------------------------------------
## I. API 摘要（OpenAPI/Scalar 标签）

- Auth
  - POST `/api/auth/login`
- Wallet
  - GET `/api/wallet/balance`
  - GET `/api/wallet/transactions?skip=&take=`
  - POST `/api/wallet/grant`
- Presets
  - GET `/api/presets`
- Generate
  - POST `/api/generate`
  - GET `/api/generate/{taskId}`


--------------------------------------
## J. 工程与规范

- 构建：`dotnet build`（csproj）；`.sln` 仅 VS 备选；前端 `pnpm dev`。
- 提交规范：`feat:/fix:/docs:/chore:`（小步提交，频率高）。
- 命名与目录：
  - Backend：Controllers / Services / Models / Providers / Database / Helpers
  - Frontend：pages / components / services / stores / assets
- 安全：
  - 演示期 JWT 可简化；避免泄露真实 API Key（使用环境变量与后端代理）。
  - 图片统一由后端下发 URL，避免前端直连第三方。
- 监控：
  - 健康检查 `/health`（200 即可）；日志按天落盘。

--------------------------------------
## K. 最小数据与运行（开发期建议）

- 种子数据：
  - 用户：`demo` / `demo123`，初始 `Credits = 20`
  - Presets：6–9 个（封面图、提示文案齐全），覆盖 Stub/Qwen/Flux
- 环境变量：
  - `DASHSCOPE_API_KEY`（如接 Qwen）
- 运行（开发）：
  - 后端：`.NET 9 / dotnet run`（生成 Scalar 文档，SQLite 自动迁移）
  - 前端：`pnpm install && pnpm dev`（深色主题，3 页可达）

本规格聚焦“一周能跑起来、能演示”的 Slim MVP：Credits 真扣、任务真跑（或可 Stub）、交互真看（预制菜+深色 UI）。后续再按实际反馈扩展。