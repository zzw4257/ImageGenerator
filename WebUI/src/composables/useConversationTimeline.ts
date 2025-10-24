import type { GenerationRecordDto, ImageDto } from '@/types/api'
import type { TimelineItem } from '@/types/ui'

/**
 * A composable for transforming conversation records into a timeline format.
 */
export function useConversationTimeline() {

  /**
   * Maps an array of generation records to an array of timeline items.
   * @param records - The array of generation records.
   * @returns An array of timeline items.
   */
  const mapRecordsToTimeline = (records: GenerationRecordDto[]): TimelineItem[] => {
    if (!Array.isArray(records)) return []
    const items: TimelineItem[] = []
    records.forEach((r) => {
  // prompt item — uses first input image (if any)
      items.push({
        id: `${r.id}-prompt`,
        type: 'prompt',
        prompt: r.prompt ?? '',
        timestamp: new Date(r.createdAt),
        image: r.inputImages ?? [],
      })
  // output image item
      if (r.outputImage) {
        items.push({
          id: `${r.id}-image`,
          type: 'image',
          prompt: r.prompt ?? '',
          timestamp: new Date(r.completedAt ?? r.createdAt),
          image: [r.outputImage],
        })
      }
    })
    return items
  }

  return { mapRecordsToTimeline }
}
