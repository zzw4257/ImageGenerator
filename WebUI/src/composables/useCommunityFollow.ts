import { computed, ref } from 'vue'

const followedCreators = ref<Set<string>>(new Set())

export function useCommunityFollow () {
  const followingIds = computed(() => [...followedCreators.value])

  const isFollowing = (creatorId: string) => followedCreators.value.has(creatorId)

  const toggleFollow = (creatorId: string) => {
    const next = new Set(followedCreators.value)
    if (next.has(creatorId)) {
      next.delete(creatorId)
    } else {
      next.add(creatorId)
    }
    followedCreators.value = next
    return followedCreators.value.has(creatorId)
  }

  const resetFollow = () => {
    followedCreators.value = new Set()
  }

  return {
    followingIds,
    isFollowing,
    toggleFollow,
    resetFollow,
  }
}
