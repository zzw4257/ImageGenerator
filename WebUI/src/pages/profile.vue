<template>
  <v-container class="py-6">
    <v-row>
      <v-col cols="12" md="6">
        <v-card rounded="xl" elevation="2" class="mb-6">
          <v-card-title class="pa-6 d-flex align-center">
            <v-icon class="mr-3">mdi-account</v-icon>
            Profile
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <v-list lines="two">
              <v-list-item prepend-icon="mdi-identifier" :title="profile?.id" subtitle="User ID" />
              <v-list-item prepend-icon="mdi-account" :title="profile?.username" subtitle="Username" />
              <v-list-item prepend-icon="mdi-calendar" :title="formatDate(profile?.createdAt)" subtitle="Joined" />
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>

      <v-col cols="12" md="6">
        <v-card rounded="xl" elevation="2" class="mb-6">
          <v-card-title class="pa-6 d-flex align-center">
            <v-icon class="mr-3">mdi-star-circle</v-icon>
            Credits
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <div class="d-flex align-center mb-4">
              <v-chip color="primary" variant="outlined" size="large" class="mr-3">
                {{ profile?.credits ?? 0 }} credits
              </v-chip>
              <v-btn color="primary" :loading="claiming" @click="onClaimCredits" :disabled="claimDisabled">
                Claim Daily Credits
              </v-btn>
            </div>
            <div class="text-caption text-grey-darken-1">
              Last claimed: {{ formatDate(profile?.lastCreditClaimedAt) || 'Never' }}
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { onMounted, ref, computed } from 'vue'
import { useNotificationStore } from '@/stores/notification'
import { getProfile, claimCredits } from '@/services/profile'
import type { ProfileDto } from '@/types/api'

const notificationStore = useNotificationStore()

const profile = ref<ProfileDto | null>(null)
const loading = ref(false)
const claiming = ref(false)

const formatDate = (d?: string | null) => {
  if (!d) return ''
  const date = new Date(d)
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const claimDisabled = computed(() => claiming.value || !profile.value)

const load = async () => {
  loading.value = true
  try {
    profile.value = await getProfile()
  } catch (e) {
    notificationStore.error('Failed to load profile')
  } finally {
    loading.value = false
  }
}

const onClaimCredits = async () => {
  claiming.value = true
  try {
    profile.value = await claimCredits()
    notificationStore.success('Credits claimed')
  } catch (e) {
    notificationStore.error('Failed to claim credits')
  } finally {
    claiming.value = false
  }
}

onMounted(load)
</script>

<route lang="yaml">
meta:
  title: Profile
</route>
