import type {
  CommunityCreator,
  CommunityFeedItem,
  CommunityFeedTab,
  CommunityProject,
  TrendingTopic,
} from '@/types/community'
import creatorsRaw from '@/mock/community-creators.json'
import feedRaw from '@/mock/community-feed.json'

interface RawCommunityCreator extends CommunityCreator {
  metrics: CommunityCreator['metrics']
  recentProjects: CommunityProject[]
}

interface CommunityCreatorsPayload {
  creators: RawCommunityCreator[]
  trendingTopics: TrendingTopic[]
}

interface RawCommunityFeedItem
  extends Omit<CommunityFeedItem, 'createdAt'> {
  createdAt: string
}

interface CommunityFeedPayload {
  feed: RawCommunityFeedItem[]
}

const payload = creatorsRaw as CommunityCreatorsPayload
const feedPayload = feedRaw as CommunityFeedPayload

function cloneCreator (creator: RawCommunityCreator): CommunityCreator {
  return {
    ...creator,
    topics: [...creator.topics],
    recentProjects: creator.recentProjects.map(project => ({ ...project })),
    achievements: [...creator.achievements],
    metrics: { ...creator.metrics },
  }
}

function cloneFeedItem (item: RawCommunityFeedItem): CommunityFeedItem {
  return {
    ...item,
    createdAt: new Date(item.createdAt),
    tags: [...item.tags],
  }
}

export function getCommunityCreators (): CommunityCreator[] {
  return payload.creators.map(creator => cloneCreator(creator))
}

export function getTrendingTopics (): TrendingTopic[] {
  return payload.trendingTopics.map(topic => ({ ...topic }))
}

export function findCommunityCreatorById (
  creatorId: string,
): CommunityCreator | undefined {
  const found = payload.creators.find(creator => creator.id === creatorId)
  if (!found) {
    return undefined
  }
  return cloneCreator(found)
}

export function getCommunityFeed (
  tab: CommunityFeedTab,
): CommunityFeedItem[] {
  return feedPayload.feed
    .map(item => cloneFeedItem(item))
    .toSorted((a, b) => {
      if (tab === 'latest') {
        return b.createdAt.getTime() - a.createdAt.getTime()
      }
      return b.popularityScore - a.popularityScore
    })
}

export function getCreatorFeed (creatorId: string): CommunityFeedItem[] {
  return feedPayload.feed
    .filter(item => item.creatorId === creatorId)
    .map(item => cloneFeedItem(item))
    .toSorted((a, b) => b.createdAt.getTime() - a.createdAt.getTime())
}
