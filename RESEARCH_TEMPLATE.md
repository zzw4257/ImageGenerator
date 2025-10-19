# AIGC 平台调研模板

**调研负责人**: QA团队  
**调研周期**: 2024-10-20 至 2024-10-23 (4天)  
**调研目标**: 评估至少10个AIGC平台的API可集成性、定价模式、合规性，为Aetherflow平台选择首批接入的API提供商

---

## 📋 调研平台清单

### 优先级 P0（必须调研）
- [ ] **Stability AI** - 主要文生图平台 ⭐⭐⭐
- [ ] **OpenAI (DALL-E)** - 知名度高，备选方案
- [ ] **Replicate** - API市场，多模型支持

### 优先级 P1（重要）
- [ ] **Hugging Face Inference API** - 开源友好
- [ ] **Together AI** - 新兴平台，价格有竞争力
- [ ] **RunPod** - GPU租赁+API

### 优先级 P2（次要）
- [ ] **Google Gemini (Imagen)** - 技术前沿
- [ ] **Anthropic Claude** - 主要是文本，但有图片理解
- [ ] **Leonardo.ai** - 游戏美术向
- [ ] **Midjourney** - 仅了解ToS，**不集成**（禁止API）

### 国内平台（可选）
- [ ] **通义万相** (阿里云)
- [ ] **文心一格** (百度)
- [ ] **智谱清言** (智谱AI)

---

## 🔍 调研维度（每个平台都要填写）

### 模板格式

```markdown
## [平台名称]

### 基础信息
- **官网**: 
- **API文档**: 
- **注册日期**: 
- **调研人**: 
- **最后更新**: 

### 1. 产品能力
- **支持的功能**:
  - [ ] 文生图 (Text-to-Image)
  - [ ] 图生图 (Image-to-Image)
  - [ ] 图片编辑 (Inpainting/Outpainting)
  - [ ] 图片放大 (Upscaling)
  - [ ] 视频生成
  - [ ] 音频生成
  - [ ] 其他: __________

- **模型列表**:
  - 模型1: 名称, 版本, 适用场景
  - 模型2: ...

- **输出质量** (主观评价 1-5星): ⭐⭐⭐⭐⭐

### 2. API技术评估

#### 2.1 接入难度
- **是否需要审核**: 是 / 否
- **审核时长**: ____ 天
- **是否需要信用卡**: 是 / 否
- **是否需要企业账户**: 是 / 否
- **注册复杂度**: 简单 / 中等 / 复杂

#### 2.2 API设计
- **认证方式**: API Key / OAuth2 / 其他
- **API风格**: RESTful / GraphQL / gRPC
- **请求格式**: JSON / Form-data / 其他
- **响应格式**: JSON / Binary / 其他
- **是否有官方SDK**: 
  - Python: 是 / 否 (链接: ___)
  - JavaScript/Node.js: 是 / 否
  - C#: 是 / 否
  - Go: 是 / 否

#### 2.3 示例代码
```python
# 示例：文生图
import requests

response = requests.post(
    "https://api.example.com/v1/generate",
    headers={"Authorization": f"Bearer {api_key}"},
    json={
        "prompt": "a beautiful sunset",
        "width": 512,
        "height": 512
    }
)

print(response.json())
```

#### 2.4 错误处理
- **错误码文档**: 完善 / 一般 / 缺失
- **常见错误码**:
  - 401: Unauthorized
  - 429: Rate Limit
  - 500: Server Error
  - ...

### 3. 定价模型

#### 3.1 计费方式
- **计费单位**: 
  - [ ] 按次计费 (per request)
  - [ ] 按Token/Credits计费
  - [ ] 按图片分辨率计费
  - [ ] 按月订阅
  - [ ] 其他: _______

#### 3.2 具体价格
| 功能 | 分辨率 | 单价 | 备注 |
|------|--------|------|------|
| 文生图 | 512x512 | $0.02/张 | 示例 |
| 文生图 | 1024x1024 | $0.08/张 | |
| 图生图 | 512x512 | $0.03/张 | |

- **充值方式**: 信用卡 / PayPal / 支付宝 / 微信
- **最低充值金额**: $____
- **是否支持后付费**: 是 / 否

#### 3.3 免费额度
- **新用户赠送**: ____ Credits / ____ 次调用
- **每月免费额度**: ____ (如果有)
- **试用限制**: _______

#### 3.4 价格竞争力
- **与市场对比**: 便宜 / 适中 / 昂贵
- **性价比评分** (1-5分): ⭐⭐⭐☆☆

### 4. 性能与限流

#### 4.1 限流规则
- **QPS限制** (Queries Per Second): ____
- **QPM限制** (Queries Per Minute): ____
- **QPD限制** (Queries Per Day): ____
- **QPH限制** (Queries Per Hour): ____
- **并发限制**: ____ 个请求

#### 4.2 响应时间
- **文生图 (512x512)**: ____ 秒
- **文生图 (1024x1024)**: ____ 秒
- **图生图**: ____ 秒

*（需要实测，记录5次取平均值）*

#### 4.3 成功率
- **测试次数**: ____ 次
- **成功次数**: ____ 次
- **成功率**: _____%

### 5. 合规性分析 ⚖️

#### 5.1 服务条款 (ToS)
- **ToS链接**: 
- **最后更新日期**: 
- **已仔细阅读**: 是 / 否

#### 5.2 关键条款摘录

**允许的行为**:
- ✅ _______
- ✅ _______

**明确禁止的行为**:
- ❌ 账号共享 (Account Sharing)
- ❌ API密钥转售 (Reselling API Keys)
- ❌ 自动化滥用 (Abusive Automation)
- ❌ 爬虫/逆向工程 (Scraping/Reverse Engineering)
- ❌ _______

#### 5.3 第三方集成政策
- **是否允许第三方平台集成**: 是 / 否 / 不明确
- **是否需要申请**: 是 / 否
- **是否需要展示品牌标识**: 是 / 否
- **用户协议要求**: _______

#### 5.4 我们的方案合规性
- **Aetherflow的做法**: 
  > 用户自愿将其API Key托管在我们平台，Key所有权仍归用户。我们代为调用API，用户保留随时撤销的权利。

- **合规性评估**: 
  - [ ] ✅ 完全合规
  - [ ] ⚠️ 灰色地带（需法务确认）
  - [ ] ❌ 明确违规（不可接入）

- **风险等级**: 低 / 中 / 高

- **风险说明**: _______

#### 5.5 内容政策
- **禁止生成的内容**:
  - [ ] 暴力血腥
  - [ ] 色情内容
  - [ ] 政治敏感
  - [ ] 侵犯版权
  - [ ] 其他: _______

- **我们的应对**: _______

### 6. 开发者体验

#### 6.1 文档质量
- **完整性**: 完善 / 一般 / 缺失
- **示例代码**: 丰富 / 够用 / 缺少
- **更新频率**: 频繁 / 偶尔 / 很少
- **中文文档**: 有 / 无
- **文档评分** (1-5分): ⭐⭐⭐⭐☆

#### 6.2 社区支持
- **Discord/Slack**: 是 / 否 (链接: ___)
- **GitHub Issues**: 是 / 否
- **论坛**: 是 / 否
- **响应速度**: 快 / 中 / 慢

#### 6.3 稳定性
- **历史宕机记录**: 查询 status.example.com
- **近3个月宕机次数**: ____ 次
- **平均可用性**: _____%

### 7. 集成优先级评估

#### 综合评分
| 维度 | 权重 | 得分(1-5) | 加权分 |
|------|------|-----------|--------|
| 功能完整性 | 20% | ___ | ___ |
| 价格竞争力 | 25% | ___ | ___ |
| 合规性 | 30% | ___ | ___ |
| API易用性 | 15% | ___ | ___ |
| 稳定性 | 10% | ___ | ___ |
| **总分** | **100%** | | **___** |

#### 接入建议
- [ ] 🟢 **强烈推荐** - 首批接入
- [ ] 🟡 **推荐** - 第二批考虑
- [ ] 🟠 **保留** - 未来可能
- [ ] 🔴 **不推荐** - 不接入

#### 理由
_______

---

### 8. 技术验证记录

#### 8.1 测试环境
- **测试时间**: 2024-10-__ 
- **测试工具**: Postman / Python / C#
- **API Key来源**: 个人注册 / 团队账号

#### 8.2 测试用例

**用例1: 基础文生图**
- **Prompt**: "a cat sitting on a chair"
- **参数**: width=512, height=512
- **预期**: 成功返回图片URL
- **实际结果**: ✅成功 / ❌失败
- **响应时间**: ____ 秒
- **生成图片**: ![](截图链接)

**用例2: 复杂Prompt**
- **Prompt**: "a futuristic cityscape at sunset, cyberpunk style, highly detailed, 8k"
- **结果**: ✅成功 / ❌失败
- **响应时间**: ____ 秒

**用例3: 错误处理 - 无效API Key**
- **预期**: 返回 401 错误
- **实际**: ✅符合预期 / ❌不符合

**用例4: 限流测试**
- **方法**: 连续发送10个请求
- **结果**: 第__个请求触发429错误
- **Retry-After**: ____ 秒

**用例5: 并发测试**
- **方法**: 同时发送5个请求
- **成功**: ____ 个
- **失败**: ____ 个

#### 8.3 集成代码示例
```csharp
// C# 集成示例
public class StabilityAIClient : IImageGenerationClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public async Task<GenerationResult> GenerateAsync(string prompt, GenerationParams parameters)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.stability.ai/v1/generation/...");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = JsonContent.Create(new {
            prompt = prompt,
            width = parameters.Width,
            height = parameters.Height
        });
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<StabilityResponse>();
        return new GenerationResult {
            ImageUrl = result.Artifacts[0].Base64, // 或URL
            CreditsCost = result.FinishReason // 根据实际返回
        };
    }
}
```

---

## 📊 汇总对比表

调研完所有平台后，填写此对比表：

| 平台 | 功能 | 价格 | 合规性 | 易用性 | 总分 | 推荐度 |
|------|------|------|--------|--------|------|--------|
| Stability AI | ⭐⭐⭐⭐⭐ | $0.02/张 | ✅ | ⭐⭐⭐⭐ | 4.5 | 🟢 |
| OpenAI DALL-E | ⭐⭐⭐⭐ | $0.04/张 | ✅ | ⭐⭐⭐⭐⭐ | 4.2 | 🟢 |
| Replicate | ⭐⭐⭐⭐ | 按模型 | ✅ | ⭐⭐⭐ | 3.8 | 🟡 |
| ... | | | | | | |

---

## 📝 合规性分析报告框架

完成所有平台调研后，输出独立的合规性分析报告：

### 报告结构
1. **执行摘要**
   - 调研了哪些平台
   - 哪些可以合规接入
   - 主要风险点

2. **合规性分析**
   - ToS关键条款对比
   - 第三方集成政策
   - 我们的方案分析

3. **风险评估**
   - 高风险平台及原因
   - 灰色地带及应对
   - 法务建议（如需要）

4. **用户协议设计**
   - 我们需要向用户声明什么
   - 用户需要同意什么
   - 免责条款

5. **监控与应对**
   - 如何监控政策变化
   - 如果被禁用的应对方案

---

## 🛠️ 技术验证清单

- [ ] 注册至少3个平台的账号
- [ ] 获取测试API Key
- [ ] 编写集成Demo（至少2个平台）
- [ ] 测试基础功能（文生图）
- [ ] 测试错误处理
- [ ] 测试限流机制
- [ ] 记录响应时间
- [ ] 保存示例图片
- [ ] 编写集成代码（.NET）

---

## 📅 时间安排

- **Day 1 (10/20)**: 调研5个平台（Stability AI, OpenAI, Replicate, HF, Together AI）
- **Day 2 (10/21)**: 调研5个平台（RunPod, Google, 通义万相, 文心一格, Leonardo）
- **Day 3 (10/22)**: 合规性分析，输出报告
- **Day 4 (10/23)**: 技术验证Demo，测试集成

---

## 📤 交付物清单

- [ ] `AIGC_Platforms_Research.md` - 主调研报告（10+平台详细信息）
- [ ] `Compliance_Analysis.md` - 合规性分析报告
- [ ] `Competitor_Analysis.md` - 竞品分析（可选）
- [ ] `demos/stability_test.cs` - Stability AI 集成Demo
- [ ] `demos/openai_test.cs` - OpenAI 集成Demo（可选）
- [ ] `images/` - 测试生成的图片样本
- [ ] `API_Comparison.xlsx` - Excel对比表（可选）

---

## 💡 调研技巧

1. **善用AI助手**: 用ChatGPT/Claude快速总结API文档
2. **保存证据**: 截图ToS关键条款，防止平台修改政策
3. **实际测试**: 不要只看文档，一定要真实调用API
4. **关注变化**: 订阅平台的更新通知
5. **社区反馈**: 看Twitter/Reddit上开发者的真实评价

---

## 📞 遇到问题时

- **技术问题**: 联系 DevOps (OPS) 或后端团队
- **API调用失败**: 检查API Key、限流、余额
- **合规性不确定**: 标记为"灰色地带"，团队讨论
- **时间不够**: 优先完成P0平台，P2可以简化

---

**调研愉快！记住：质量 > 数量，合规性是第一优先级！** 🔍✅