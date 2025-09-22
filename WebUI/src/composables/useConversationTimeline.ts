import type { GenerationRecordDto, ImageDto } from '@/types/api'
import type { TimelineItem } from '@/types/ui'

export function useConversationTimeline() {

  const mapRecordsToTimeline = (records: GenerationRecordDto[]): TimelineItem[] => {
    if (!Array.isArray(records)) return []
    const items: TimelineItem[] = []
    records.forEach((r) => {
  // prompt item — uses first input image (if any)
      const promptImage: ImageDto | undefined = r.inputImages?.[0]
      items.push({
        id: `${r.id}-prompt`,
        type: 'prompt',
        prompt: r.prompt ?? '',
        timestamp: new Date(r.createdAt),
        image: promptImage,
      })
  // output image item
      if (r.outputImage) {
        items.push({
          id: `${r.id}-image`,
          type: 'image',
          prompt: r.prompt ?? '',
          timestamp: new Date(r.completedAt ?? r.createdAt),
          image: r.outputImage,
        })
      }
    })
    return items
  }

  return { mapRecordsToTimeline }
}
