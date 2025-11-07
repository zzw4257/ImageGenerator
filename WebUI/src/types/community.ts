export interface CommunityProject {
  id: string
  title: string
  thumbnail: string
  likes: number
}

export interface CommunityCreatorMetrics {
  engagementRate: number
  avgDelivery: string
  clientSatisfaction: number
}

export interface CommunityCreator {
  id: string
  name: string
  avatar: string
  title: string
  followers: number
  artworks: number
  bio: string
  topics: string[]
  city: string
  highlightImage: string
  highlightDescription: string
  recentProjects: CommunityProject[]
  achievements: string[]
  metrics: CommunityCreatorMetrics
}

export type TrendingTopicMomentum = 'rising' | 'steady' | 'new'

export interface TrendingTopic {
  id: string
  name: string
  posts: number
  momentum: TrendingTopicMomentum
}

export interface CommunityFeedItem {
  id: string
  creatorId: string
  title: string
  caption: string
  preview: string
  tags: string[]
  likes: number
  comments: number
  saves: number
  shares: number
  popularityScore: number
  createdAt: Date
}

export type CommunityFeedTab = 'latest' | 'popular'
