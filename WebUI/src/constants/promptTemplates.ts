export interface PromptTemplateCategory {
  id: string
  name: string
  description?: string
  templates: PromptTemplate[]
}

export interface PromptTemplate {
  id: string
  title: string
  categoryId: string
  raw: string
  // derived at runtime
  description: string
  placeholders?: PlaceholderSpec[]
}

export interface PlaceholderSpec {
  key: string
  defaultValue: string
}

// Raw templates assembled from templates.txt (Chinese comments kept concise)
export const promptTemplateCategories: PromptTemplateCategory[] = [
  {
    id: 'generate',
    name: '生成类 (Creation)',
    description: '用于直接生成新图片的提示模板',
    templates: [
      {
        id: 'photorealistic',
        title: '逼真摄影场景',
        categoryId: 'generate',
        description: '对于逼真的图片，请使用摄影术语。提及拍摄角度、镜头类型、光线和细节，引导模型生成逼真的效果。',
        raw: 'A photorealistic [shot type:close-up] of [subject:a mystical fox], [action or expression:looking into the distance], set in [environment:ancient forest]. The scene is illuminated by [lighting description:soft golden hour rim light], creating a [mood:serene] atmosphere. Captured with a [camera/lens details:Canon EOS R5 + 85mm f1.2], emphasizing [key textures and details:detailed fur, shimmering particles]. The image should be in a [aspect ratio:16:9] format.',
      },
      {
        id: 'sticker',
        title: '风格化贴纸 / 图标',
        categoryId: 'generate',
        description: '用于创建带有指定风格的贴纸 / 图标素材，强调线条、配色与透明背景。',
        raw: 'A [style:kawaii chibi] sticker of a [subject:cat astronaut], featuring [key characteristics:round helmet, floating fish] and a [color palette:pastel neon mix]. The design should have [line style:clean bold outline] and [shading style:soft cell shading]. The background must be transparent.',
      },
      {
        id: 'text-graphic',
        title: '带文字的图形',
        categoryId: 'generate',
        description: '用于生成包含特定文字的图形 / 标识，明确字体感受、风格与配色。',
        raw: 'Create a [image type:logo badge] for [brand/concept:Arctic Labs] with the text "[text to render:POLAR AI]" in a [font style:geometric sans-serif]. The design should be [style description:minimal, futuristic] with a [color scheme:icy blue + white gradient].',
      },
      {
        id: 'product-shot',
        title: '产品商业摄影',
        categoryId: 'generate',
        description: '适合电商/广告用途的专业产品照片，强调光线布置、角度与核心细节。',
        raw: 'A high-resolution, studio-lit product photograph of a [product description:matte black wireless earbud case] on a [background surface/description:brushed aluminum surface with soft vignette]. The lighting is a [lighting setup:three-point softbox] to [lighting purpose:emphasize subtle curves]. The camera angle is a [angle type:slight low angle] to showcase [specific feature:charging indicator + hinge]. Ultra-realistic, with sharp focus on [key detail:texture + logo etching]. [Aspect ratio:1:1].',
      },
      {
        id: 'minimal-negative',
        title: '极简负空间',
        categoryId: 'generate',
        description: '生成带大量留白与单主体的极简风图像，适合做背景或叠加文案。',
        raw: 'A minimalist composition featuring a single [subject:solitary bonsai] positioned in the [position in frame:lower right] of the frame. The background is a vast, empty [color:off-white] canvas, creating significant negative space. Soft, subtle lighting. [Aspect ratio:3:2].',
      },
      {
        id: 'comic-panel',
        title: '漫画单格 / 分镜',
        categoryId: 'generate',
        description: '生成漫画风单格场景，分离前景角色动作与背景设定，可含对白框。',
        raw: 'A single comic book panel in a [art style:neo-noir ink wash] style. In the foreground, [character description and action:detective leaning over a glowing map]. In the background, [setting details:rain streaked window + neon signs]. The panel has a [dialogue/caption box:caption] with the text "[Text:We were already too late]". The lighting creates a [mood:brooding] mood. [Aspect ratio:9:16].',
      },
    ],
  },
  {
    id: 'edit',
    name: '编辑类 (Editing)',
    description: '对已上传图片进行修改的提示模板',
    templates: [
      {
        id: 'add-remove',
        title: '添加/移除元素',
        categoryId: 'edit',
        description: '在保持原图整体风格与光照的前提下增删或修改局部元素。',
        raw: 'Using the provided image of [subject:medieval marketplace], please [add/remove/modify:add] [element:a banner] to/from the scene. Ensure the change is [description of how the change should integrate:lit consistently + matches fabric texture].',
      },
      {
        id: 'partial-redraw',
        title: '局部重绘',
        categoryId: 'edit',
        description: '仅针对指定元素进行替换或重绘，严格保持其余部分不变。',
        raw: 'Using the provided image, change only the [specific element:tabletop object] to [new element/description:translucent crystal sphere emitting faint blue light]. Keep everything else in the image exactly the same, preserving the original style, lighting, and composition.',
      },
      {
        id: 'style-transfer',
        title: '风格迁移',
        categoryId: 'edit',
        description: '将原图内容转化为目标艺术家 / 风格，同时保留构图主体与结构。',
        raw: 'Transform the provided photograph of [subject:old lighthouse on cliff] into the artistic style of [artist/art style:Studio Ghibli watercolor]. Preserve the original composition but render it with [description of stylistic elements:soft painterly gradients, gentle atmospheric haze].',
      },
      {
        id: 'multi-compose',
        title: '多图合成',
        categoryId: 'edit',
        description: '将多张参考图的元素融合为新场景，明确取用源与合成关系。',
        raw: 'Create a new image by combining the elements from the provided images. Take the [element from image 1:red vintage car] and place it with/on the [element from image 2:cobbled alley]. The final image should be a [description of the final scene:filmic dusk ambience with soft reflections].',
      },
      {
        id: 'hi-fidelity-preserve',
        title: '高保真细节保留',
        categoryId: 'edit',
        description: '合成或叠加新元素时确保关键原始细节（如面部/标识）完全保留。',
        raw: 'Using the provided images, place [element from image 2:golden emblem] onto [element from image 1:leather journal cover]. Ensure that the features of [element from image 1] remain completely unchanged. The added element should [description of how the element should integrate:match emboss depth + warm specular highlights].',
      },
    ],
  },
]

export function extractPlaceholders (raw: string): PlaceholderSpec[] {
  const map = new Map<string, PlaceholderSpec>()
  const regex = /\[([^\]]+)]/g
  let m: RegExpExecArray | null
  while ((m = regex.exec(raw)) !== null) {
    const content = m[1]
    if (typeof content === 'string') {
      const parts = content.split(':')
      const first = parts[0]
      if (!first) {
        continue
      }
      const key = first.trim()
      const defaultValue = parts.slice(1).join(':').trim() || ''
      if (!map.has(key)) {
        map.set(key, { key, defaultValue })
      }
    }
  }
  return [...map.values()]
}
