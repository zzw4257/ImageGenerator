# Market 架构设计说明

## 整体架构

Market 是 Aetherflow 的核心交易平台，整合了所有 AIGC 资产的交易功能。

### 资产类型层次结构

```
Market (创意市场)
├── Templates (提示词模板)
│   ├── 产品摄影模板
│   ├── UI设计模板
│   ├── 动漫角色模板
│   └── ...
├── Workflows (工作流)
│   ├── 电商摄影工作流
│   ├── 品牌设计工作流
│   └── ...
├── Workspaces (工作空间)
│   ├── UI设计工作空间
│   ├── 插画创作工作空间
│   └── ...
└── Models (AI模型)
    ├── LoRA 模型
    ├── 风格化模型
    └── ...
```

## 功能关系

### 1. Templates vs Muse
- **Muse** (`/presets`): 官方精选的免费预设，主要用于快速开始
- **Templates** (Market 内): 用户分享的付费/免费模板，更丰富多样

### 2. 数据流向
```
用户创作 → 分享到 Market → 其他用户购买/使用 → 收益分成
```

### 3. 定价策略
- **Templates**: 1-10 Credits (轻量级)
- **Workflows**: 10-50 Credits (中等复杂度)
- **Workspaces**: 30-100 Credits (高价值)
- **Models**: 50-200 Credits (专业级)

## 技术实现

### 页面结构
```
/market
├── 全部 (所有资产)
├── 提示词模板 (type: template)
├── 工作流 (type: workflow)
├── 工作空间 (type: workspace)
├── 模型市场 (type: model)
└── 我的挂单 (用户发布的资产)
```

### 组件复用
- `MarketItemCard`: 统一的资产卡片组件，支持所有类型
- `CreateListingDialog`: 发布资产的对话框
- `PurchaseDialog`: 购买确认对话框

### 数据模型
```typescript
interface MarketItem {
  type: 'template' | 'workflow' | 'workspace' | 'model'
  // 模板特有字段
  prompt?: string
  difficulty?: 'beginner' | 'intermediate' | 'advanced'
  // 通用字段
  price: number
  author: AuthorInfo
  stats: UsageStats
}
```

## 商业逻辑

### 收益分成
- **平台抽佣**: 10%
- **创作者收益**: 90%
- **推广奖励**: 额外 5% (推荐用户购买)

### 质量控制
1. **自动检测**: AI 检测内容质量和原创性
2. **人工审核**: 专业团队审核高价值资产
3. **用户反馈**: 评分和举报机制
4. **版本管理**: 支持资产更新和版本控制

## 用户体验

### 发现机制
- **智能推荐**: 基于用户历史和偏好
- **分类浏览**: 按类型、分类、难度筛选
- **搜索功能**: 关键词、标签、作者搜索
- **热门排行**: 下载量、评分、收益排行

### 购买流程
1. 浏览 → 2. 预览 → 3. 购买 → 4. 下载 → 5. 使用 → 6. 评价

## 扩展规划

### 短期 (MVP+)
- [x] 基础 Market 功能
- [x] Templates 整合
- [ ] 支付系统集成
- [ ] 用户认证完善

### 中期
- [ ] 工作流可视化编辑器
- [ ] 模型训练平台
- [ ] 社区功能 (关注、评论)
- [ ] API 开放平台

### 长期
- [ ] 企业级工作空间
- [ ] AI 辅助创作工具
- [ ] 跨平台同步
- [ ] 国际化支持

## 技术债务

当前存在的 TypeScript 模块导入错误是因为组件文件已创建但路径解析问题。这些是开发阶段的临时问题，不影响核心架构设计。

实际部署时需要：
1. 配置正确的模块路径
2. 添加组件类型声明
3. 完善错误处理机制
