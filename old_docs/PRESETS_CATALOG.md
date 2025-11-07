# Presets 目录（设计向）

本文件汇总当前前端展示的预设（Preset）配置，包含封面占位图路径、默认参数与完整提示词，便于你用其他模型预先生成并替换占位图片。

后端静态文件映射：`/images/*` → 本地目录 `ImageGenerator/ImageGenerator/images/`
建议将封面图统一放在：`ImageGenerator/ImageGenerator/images/presets/`

若后端返回的 `coverUrl` 为空，前端会使用兜底：`/images/default-preset.png`（可选自行创建以避免空图）。

---

## 1. 产品商业摄影 (Qwen)
- **Provider**: Qwen
- **Price**: 2 credits
- **CoverUrl（占位）**: `/images/presets/product-shot.png`
- **本地文件路径**: `ImageGenerator/ImageGenerator/images/presets/product-shot.png`
- **DefaultParams**:
```json
{"style": "photorealistic", "width": 1024, "height": 1024, "aspectRatio": "1:1"}
```
- **Prompt**:
```
A high-resolution, studio-lit product photograph of a [product description:matte black wireless earbud case] on a [background surface/description:brushed aluminum surface with soft vignette]. The lighting is a [lighting setup:three-point softbox] to [lighting purpose:emphasize subtle curves]. The camera angle is a [angle type:slight low angle] to showcase [specific feature:charging indicator + hinge]. Ultra-realistic, with sharp focus on [key detail:texture + logo etching]. [Aspect ratio:1:1].
```

---

## 2. 文字图形标识 (Flux)
- **Provider**: Flux
- **Price**: 1 credit
- **CoverUrl（占位）**: `/images/presets/text-graphic.png`
- **本地文件路径**: `ImageGenerator/ImageGenerator/images/presets/text-graphic.png`
- **DefaultParams**:
```json
{"style": "graphic", "width": 768, "height": 768, "aspectRatio": "1:1"}
```
- **Prompt**:
```
Create a [image type:logo badge] for [brand/concept:Arctic Labs] with the text "[text to render:POLAR AI]" in a [font style:geometric sans-serif]. The design should be [style description:minimal, futuristic] with a [color scheme:icy blue + white gradient].
```

---

## 3. 风格化贴纸 (Stub)
- **Provider**: Stub
- **Price**: 0 credit
- **CoverUrl（占位）**: `/images/presets/sticker.png`
- **本地文件路径**: `ImageGenerator/ImageGenerator/images/presets/sticker.png`
- **DefaultParams**:
```json
{"style": "sticker", "width": 512, "height": 512, "aspectRatio": "1:1"}
```
- **Prompt**:
```
A [style:kawaii chibi] sticker of a [subject:cat astronaut], featuring [key characteristics:round helmet, floating fish] and a [color palette:pastel neon mix]. The design should have [line style:clean bold outline] and [shading style:soft cell shading]. The background must be transparent.
```

---

## 4. 逼真摄影场景 (Qwen)
- **Provider**: Qwen
- **Price**: 2 credits
- **CoverUrl（占位）**: `/images/presets/photorealistic.png`
- **本地文件路径**: `ImageGenerator/ImageGenerator/images/presets/photorealistic.png`
- **DefaultParams**:
```json
{"style": "photorealistic", "width": 1024, "height": 576, "aspectRatio": "16:9"}
```
- **Prompt**:
```
A photorealistic [shot type:close-up] of [subject:a mystical fox], [action or expression:looking into the distance], set in [environment:ancient forest]. The scene is illuminated by [lighting description:soft golden hour rim light], creating a [mood:serene] atmosphere. Captured with a [camera/lens details:Canon EOS R5 + 85mm f1.2], emphasizing [key textures and details:detailed fur, shimmering particles]. The image should be in a [aspect ratio:16:9] format.
```

---

## 5. 极简负空间 (Flux)
- **Provider**: Flux
- **Price**: 1 credit
- **CoverUrl（占位）**: `/images/presets/minimal.png`
- **本地文件路径**: `ImageGenerator/ImageGenerator/images/presets/minimal.png`
- **DefaultParams**:
```json
{"style": "minimalist", "width": 768, "height": 512, "aspectRatio": "3:2"}
```
- **Prompt**:
```
A minimalist composition featuring a single [subject:solitary bonsai] positioned in the [position in frame:lower right] of the frame. The background is a vast, empty [color:off-white] canvas, creating significant negative space. Soft, subtle lighting. [Aspect ratio:3:2].
```

---

## 6. 漫画单格 (Stub)
- **Provider**: Stub
- **Price**: 0 credit
- **CoverUrl（占位）**: `/images/presets/comic.png`
- **本地文件路径**: `ImageGenerator/ImageGenerator/images/presets/comic.png`
- **DefaultParams**:
```json
{"style": "comic", "width": 512, "height": 910, "aspectRatio": "9:16"}
```
- **Prompt**:
```
A single comic book panel in a [art style:neo-noir ink wash] style. In the foreground, [character description and action:detective leaning over a glowing map]. In the background, [setting details:rain streaked window + neon signs]. The panel has a [dialogue/caption box:caption] with the text "[Text:We were already too late]". The lighting creates a [mood:brooding] mood. [Aspect ratio:9:16].
```

---

## 其他说明
- 前端卡片组件：`WebUI/src/components/PresetCard.vue` 使用 `preset.coverUrl || '/images/default-preset.png'` 渲染封面。
- 列表页：`WebUI/src/pages/presets.vue` 与 `WebUI/src/pages/home.vue` 会调用接口 `/Presets` 加载。
- 生成页：`WebUI/src/pages/generate.vue` 在点击卡片后，通过路由 query 接收 `prompt/provider/params`。
- 可选创建兜底图片：`ImageGenerator/ImageGenerator/images/default-preset.png`（对应 URL `/images/default-preset.png`）。
