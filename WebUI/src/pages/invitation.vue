<template>
    <div class="invitation-page mt-2">
        <v-container>
            <v-row>
                <v-col cols="12">
                    <div class="d-flex justify-space-between align-center mb-6">
                        <div>
                            <h1 class="text-h4 font-weight-bold">Invitation Codes</h1>
                            <p class="text-body-1 text-grey-darken-1">Manage invitation codes for new users</p>
                        </div>
                        <v-btn color="primary" size="large" prepend-icon="mdi-plus" @click="createCode"
                            :loading="isCreating" rounded="xl">
                            Create Code
                        </v-btn>
                    </div>
                </v-col>
            </v-row>

            <v-row>
                <v-col cols="12">
                    <v-card rounded="xl" elevation="2">
                        <v-card-title class="pa-6">
                            <div class="d-flex align-center">
                                <v-icon class="mr-3">mdi-ticket</v-icon>
                                <span>Active Invitation Codes</span>
                                <v-spacer />
                                <v-chip color="primary" variant="outlined">
                                    {{ invitations.length }} codes
                                </v-chip>
                            </div>
                        </v-card-title>

                        <v-divider />

                        <v-card-text class="pa-0">
                            <div v-if="isLoading" class="text-center py-8">
                                <v-progress-circular color="primary" indeterminate />
                                <p class="text-body-2 mt-4">Loading invitation codes...</p>
                            </div>

                            <div v-else-if="invitations.length === 0" class="text-center py-12">
                                <v-icon size="80" color="grey-lighten-2" class="mb-4">
                                    mdi-ticket-outline
                                </v-icon>
                                <h3 class="text-h6 mb-2">No invitation codes</h3>
                                <p class="text-body-2 text-grey-darken-1 mb-4">
                                    Create your first invitation code to get started
                                </p>
                                <v-btn color="primary" variant="outlined" prepend-icon="mdi-plus" @click="createCode"
                                    :loading="isCreating">
                                    Create Code
                                </v-btn>
                            </div>

                            <div v-else>
                                <v-list lines="three">
                                    <template v-for="(invitation, index) in invitations" :key="invitation.id">
                                        <v-list-item class="px-6 py-4">
                                            <template #prepend>
                                                <v-avatar color="primary" variant="tonal" size="48">
                                                    <v-icon>mdi-ticket</v-icon>
                                                </v-avatar>
                                            </template>

                                            <v-list-item-title class="text-h6 font-weight-medium mb-1">
                                                {{ invitation.code }}
                                            </v-list-item-title>

                                            <v-list-item-subtitle class="mb-2">
                                                <div class="d-flex align-center">
                                                    <v-icon size="14" class="mr-1">mdi-clock</v-icon>
                                                    Created {{ formatDate(invitation.createdAt) }}
                                                </div>
                                            </v-list-item-subtitle>

                                            <div class="d-flex align-center mt-2">
                                                <v-chip :color="invitation.remainingUses > 0 ? 'success' : 'warning'"
                                                    variant="outlined" size="small" class="mr-2">
                                                    {{ invitation.remainingUses }} uses left
                                                </v-chip>

                                                <v-chip color="primary" variant="outlined" size="small"
                                                    prepend-icon="mdi-identifier">
                                                    ID: {{ invitation.id }}
                                                </v-chip>
                                            </div>

                                            <template #append>
                                                <div class="d-flex align-center">
                                                    <v-btn icon="mdi-content-copy" variant="text" size="small"
                                                        @click="copyCode(invitation.code)" class="mr-2">
                                                        <v-icon>mdi-content-copy</v-icon>
                                                        <v-tooltip activator="parent" location="top">
                                                            Copy code
                                                        </v-tooltip>
                                                    </v-btn>

                                                    <v-btn icon="mdi-delete" variant="text" color="error" size="small"
                                                        @click="confirmDelete(invitation)"
                                                        :loading="deletingIds.has(invitation.id)">
                                                        <v-icon>mdi-delete</v-icon>
                                                        <v-tooltip activator="parent" location="top">
                                                            Delete code
                                                        </v-tooltip>
                                                    </v-btn>
                                                </div>
                                            </template>
                                        </v-list-item>

                                        <v-divider v-if="index < invitations.length - 1" />
                                    </template>
                                </v-list>
                            </div>
                        </v-card-text>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import * as invitationApi from '@/services/invitation'
import type { InvitationDto } from '@/types/api'
import { useAppStore } from '@/stores/app'
import { useNotificationStore } from '@/stores/notification'

const appStore = useAppStore()
const notificationStore = useNotificationStore()
const invitations = ref<InvitationDto[]>([])
const isLoading = ref(false)
const isCreating = ref(false)
const deletingIds = ref(new Set<string>())

const loadInvitations = async () => {
    isLoading.value = true
    try {
        invitations.value = await invitationApi.listInvitationCodes()
    } catch (error) {
        notificationStore.error('Failed to load invitation codes')
    } finally {
        isLoading.value = false
    }
}

const createCode = async () => {
    isCreating.value = true
    try {
        const newInvitation = await invitationApi.createInvitationCode()
        invitations.value.unshift(newInvitation)
        notificationStore.success('Invitation code created successfully')
    } catch (error) {
        notificationStore.error('Failed to create invitation code')
    } finally {
        isCreating.value = false
    }
}

const confirmDelete = async (invitation: InvitationDto) => {
    await appStore.showDeleteDialog({
        title: 'Delete Invitation Code',
        message: 'Are you sure you want to delete the invitation code',
        itemName: invitation.code,
        confirmText: 'Delete',
        onConfirm: async () => {
            deletingIds.value.add(invitation.id)
            try {
                await invitationApi.deleteInvitationCode(invitation.id)
                invitations.value = invitations.value.filter(inv => inv.id !== invitation.id)
                notificationStore.success('Invitation code deleted successfully')
            } catch (error) {
                notificationStore.error('Failed to delete invitation code')
                throw error
            } finally {
                deletingIds.value.delete(invitation.id)
            }
        }
    })
}

const copyCode = async (code: string) => {
    try {
        await navigator.clipboard.writeText(code)
        notificationStore.success('Code copied to clipboard', { icon: 'mdi-content-copy' })
    } catch (error) {
        notificationStore.error('Failed to copy code')
    }
}

const formatDate = (dateString: string): string => {
    const date = new Date(dateString)
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], {
        hour: '2-digit',
        minute: '2-digit'
    })
}

onMounted(() => {
    loadInvitations()
})
</script>

<style scoped>
.invitation-page {
    min-height: 100vh;
    background-color: rgb(var(--v-theme-background));
}

.v-list-item {
    transition: background-color 0.2s ease;
}

.v-list-item:hover {
    background-color: rgba(var(--v-theme-primary), 0.04);
}
</style>

<route lang="yaml">
meta:
  title: Invitation Codes
  requiresAuth: true
</route>