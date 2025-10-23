<template>
  <v-container fluid class="pa-8">
    <div class="d-flex justify-space-between align-center mb-6">
      <h1 class="text-h4 font-weight-bold">Presets</h1>
    </div>

    <v-row>
      <v-col v-for="p in presets" :key="p.id" cols="12" sm="6" md="4" lg="3">
        <PresetCard :preset="p" />
      </v-col>
    </v-row>

    <div class="d-flex justify-center mt-6">
      <v-pagination v-model="pageNumberUI" :length="pagination.TotalPages || 1" total-visible="7" @update:modelValue="onPageChange" />
    </div>
  </v-container>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import PresetCard from '@/components/PresetCard.vue'
import { listPresets, PresetDto } from '@/services/presets'

const presets = ref<PresetDto[]>([])
const pagination = ref({ TotalPages: 1, PageNumber: 0, PageSize: 12, TotalCount: 0 })
const pageNumberUI = ref(1)
const pageSize = 12

async function load() {
  try {
    const { items, pagination: meta } = await listPresets(pageNumberUI.value - 1, pageSize)
    presets.value = items
    pagination.value = meta
  } catch (e) {
    console.error('Failed to load presets', e)
  }
}

function onPageChange() { load() }

onMounted(load)
</script>

<route lang="yaml">
meta:
  title: Presets
</route>