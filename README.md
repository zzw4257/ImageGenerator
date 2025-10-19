# Aetherflow (Slim MVP) – README

状态: 任务刚布置，进入 Slim MVP 一周冲刺阶段。请全员先通读本文件与 `MVP_MIN_SPEC.md`，即可开干。

本分支: aetherflow/mvp  
目标: 一周内完成“Credits 可用、生成可跑、交互可看”的端到端演示。

相关文档:
- 精简规格: MVP_MIN_SPEC.md
- 历史较大文档: old_docs/（已归档，先不看）
- 启动脚本: start.sh（macOS/Linux）、start.bat（Windows）


## 1. 当前MVP范围（Slim）
- 后端（.NET 9 + SQLite + Scalar）
  - 最小接口集：Auth/Login、Wallet（balance/transactions/grant）、Presets、Generation（submit/query）
  - Credits 必须可用（扣费、发放、流水记录）
  - 生成 Provider 双通道：
    - StubProvider：静态图占位，用于联调与演示
    - HttpProvider（可选其一或多）：Qwen Image（DashScope）、Flux（本地/开源）
- 前端（Vue 3 + TS + Vuetify）
  - 深色主题、三页：首页（预制菜卡片墙）/ 生成页 / 钱包与历史
  - 费用提示、轮询查询任务状态、空态/错误态友好展示
  - “预制菜（Preset）”演示素材：由PM提供封面+Prompt；也可先用后端预置


## 2. 团队分工（关注域，非硬性指派）
- 项目负责人（PM，含前端展示的资产生成）：周子为
- 后端开发 1：陈以恒（Credits/Wallet/Transactions、生成任务与Provider适配）
- 后端开发 2：罗建明（.NET 9、Scalar/SQLite、Stub/本地Provider、健康检查与日志）
- 前端开发 1（UI向）：钱满亮（深色主题、卡片/列表、动效、空态/错误态）
- 前端开发 2（数据+资产管理）：金裕涵（Preset配置/JSON、历史/筛选/分页、持久化）
- 前端开发 3（后端对接项）：项科深（API service、鉴权拦截、轮询、错误重试、统一通知）
- 前期调研 + 后期对比测试：魏云翔（Qwen/Flux接入可行性、性能/体验对比、Demo可靠性测试）


## 3. 技术基线与约定
- 后端
  - 运行时: .NET 9（≥9），推荐 `dotnet` CLI 构建运行
  - API 文档: Scalar（开发环境）→ http://localhost:5000/scalar/v1
  - OpenAPI JSON: /openapi/ig.json（开发环境）
  - 数据库: SQLite（开发），默认 Data Source=User.db
  - 解决方案: `.csproj` 直接可 build/run；`.sln` 仅 Windows VS 备选
  - 配置:
    - 在 appsettings.json 中配置 CreditCosts、静态资源目录
    - 外部 API Key 使用环境变量（例如 `DASHSCOPE_API_KEY`），不要硬编码
- 前端
  - 技术: Vue 3 + TypeScript + Vite + Pinia + Vuetify（深色主题）
  - 路由: /（首页-预制菜）、/generate（生成页）、/account（钱包与历史）
  - 交互: 轮询 `GET /api/generate/{taskId}` 每 2–3s，最多 30 次或状态完成即停
  - 费用显示: 根据 provider/参数实时显示将消耗的 Credits（余额不足禁用）

提交规范（必须）:
- 小步快跑，频率高；信息明确
- 前缀: feat:/fix:/docs:/chore:
- 示例: `feat: add wallet balance API`, `fix: handle refund when generation fails`


## 4. 快速启动
前置:
- 后端: .NET 9
- 前端: Node.js 18+ 与 pnpm

方式A：一键脚本
- macOS/Linux:
  - `chmod +x start.sh && ./start.sh`
- Windows:
  - 双击 `start.bat`

方式B：手动
- 后端:
  - `cd ImageGenerator`
  - `dotnet restore`
  - 首次本地数据库迁移（若需要，按实现方式执行）
  - `dotnet run`
  - 后端API: http://localhost:5000
  - Scalar文档: http://localhost:5000/scalar/v1
- 前端:
  - `cd WebUI`
  - `pnpm install`
  - `pnpm dev`
  - 前端站点: http://localhost:5173


## 5. 最小API一览（以实际实现为准）
- Auth
  - POST `/api/auth/login` -> { token }
- Wallet
  - GET `/api/wallet/balance` -> { credits }
  - GET `/api/wallet/transactions?skip=&take=` -> [...]
  - POST `/api/wallet/grant` { amount, memo? } -> { ok }
- Presets
  - GET `/api/presets` -> 预制菜列表
- Generate
  - POST `/api/generate` { presetId?|prompt, provider, params } -> { taskId, estCost }
  - GET `/api/generate/{taskId}` -> { status: Pending|Running|Succeeded|Failed, imageUrl?, error? }


## 6. 预制菜 Preset（示例Schema）
用于首页卡片和生成页的“预置任务”。开发期可在后端种子 6–9 个，并在 `wwwroot/images/presets/` 放封面。

示例:
```
{
  "id": "string",
  "name": "product shot - neon",
  "coverUrl": "/images/presets/neon-shoe.png",
  "provider": "Stub|Qwen|Flux",
  "prompt": "a neon-lit product photograph of a sneaker on glossy floor, cinematic lighting, high contrast, 4k",
  "priceCredits": 2,
  "defaultParams": { "width": 768, "height": 512, "aspectRatio": "3:2", "style": "cinematic" },
  "tags": ["product", "neon", "cinematic"]
}
```


## 7. Credits 映射与扣费建议
- 映射（示例）:
  - `Stub.TextToImage = 0`（演示不扣费）
  - `Qwen.TextToImage = 2`
  - `Flux.TextToImage = 1`
- 扣费时机（推荐简化）: 提交任务时扣费，失败自动补一条 Refund（等额）


## 8. Demo 演示脚本（对外演示按此走）
1) 登录（或自动） -> 访问首页  
2) 选择一个预制菜 -> 跳到生成页且预填参数  
3) 切换 Provider = Flux（或 Stub） -> 显示“将消耗 X Credits”  
4) 点击 Generate -> Loading -> 出图 -> 下载按钮可用  
5) 打开“钱包与历史” -> 看到扣费流水和生成历史  
6) 点击“发放 10 Credits” -> 余额增加 -> 回生成页再来一张


## 9. 注意事项
- 任务刚布置：优先完成最小闭环，不要扩展需求
- 外部 API Key 必须使用环境变量注入，代码仓库不得出现明文
- 图片统一由后端下发 URL，前端不直连第三方 API
- 以 `.csproj` 为准进行构建；`.sln` 仅做 VS Windows 备选项
- 任何新增接口/字段，请同步更新 Scalar 文档注释，保持可读

——  
有问题在群里同步，或在仓库提 Issue。让我们把 Slim MVP 一周端到端 Demo 跑起来。