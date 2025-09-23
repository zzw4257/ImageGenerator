<template>
  <!-- Delete Dialog -->
  <v-dialog v-model="appStore.deleteDialogInfo.show" max-width="400" persistent>
    <v-card rounded="xl">
      <v-card-title class="pa-6">
        <div class="d-flex align-center">
          <v-icon color="error" class="mr-3">mdi-delete</v-icon>
          {{ appStore.deleteDialogInfo.title }}
        </div>
      </v-card-title>

      <v-card-text class="px-6">
        <p class="text-body-1">
          {{ appStore.deleteDialogInfo.message }}
          <strong v-if="appStore.deleteDialogInfo.itemName">{{ appStore.deleteDialogInfo.itemName }}</strong>?
        </p>
        <p class="text-body-2 text-grey-darken-1 mt-2">
          This action cannot be undone.
        </p>
      </v-card-text>

      <v-card-actions class="pa-6 pt-0">
        <v-spacer />
        <v-btn
          variant="outlined"
          @click="handleDeleteCancel"
          :disabled="appStore.deleteDialogInfo.isDeleting"
        >
          Cancel
        </v-btn>
        <v-btn
          color="error"
          @click="handleDeleteConfirm"
          :loading="appStore.deleteDialogInfo.isDeleting"
        >
          {{ appStore.deleteDialogInfo.confirmText }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <!-- Prompt Replace Dialog -->
  <v-dialog v-model="appStore.promptReplaceDialogInfo.show" max-width="500" persistent>
    <v-card rounded="xl">
      <v-card-title class="pa-6 pb-2">
        <div class="d-flex align-center">
          <v-icon color="warning" class="mr-3">mdi-swap-horizontal</v-icon>
          {{ appStore.promptReplaceDialogInfo.title }}
        </div>
      </v-card-title>

      <v-card-text class="pa-6 pt-2">
        <p class="text-body-1 mb-4">{{ appStore.promptReplaceDialogInfo.message }}</p>
        
        <div class="mb-4">
          <v-label class="text-caption text-grey-darken-1 mb-1">Current Prompt:</v-label>
          <v-card variant="outlined" class="pa-3 mb-3">
            <div class="text-body-2">{{ appStore.promptReplaceDialogInfo.currentPrompt || 'Empty' }}</div>
          </v-card>
          
          <v-label class="text-caption text-grey-darken-1 mb-1">Timeline Prompt:</v-label>
          <v-card variant="outlined" class="pa-3">
            <div class="text-body-2">{{ appStore.promptReplaceDialogInfo.newPrompt }}</div>
          </v-card>
        </div>
      </v-card-text>

      <v-card-actions class="pa-6 pt-0">
        <v-spacer />
        <v-btn 
          variant="outlined" 
          @click="handlePromptCancel"
          :disabled="appStore.promptReplaceDialogInfo.isProcessing"
        >
          Keep Current
        </v-btn>
        <v-btn 
          color="primary" 
          variant="flat"
          @click="handlePromptConfirm"
          :loading="appStore.promptReplaceDialogInfo.isProcessing"
        >
          Replace
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { useAppStore } from '@/stores/app'

const appStore = useAppStore()

// Delete dialog handlers
const handleDeleteCancel = () => {
  if (!appStore.deleteDialogInfo.isDeleting) {
    appStore.hideDeleteDialog()
  }
}

const handleDeleteConfirm = async () => {
  if (appStore.deleteDialogInfo.onConfirm) {
    await appStore.deleteDialogInfo.onConfirm()
  }
}

// Prompt replace dialog handlers
const handlePromptCancel = () => {
  if (!appStore.promptReplaceDialogInfo.isProcessing) {
    if (appStore.promptReplaceDialogInfo.onCancel) {
      appStore.promptReplaceDialogInfo.onCancel()
    } else {
      appStore.hidePromptReplaceDialog()
    }
  }
}

const handlePromptConfirm = async () => {
  if (appStore.promptReplaceDialogInfo.onConfirm) {
    await appStore.promptReplaceDialogInfo.onConfirm()
  }
}
</script>