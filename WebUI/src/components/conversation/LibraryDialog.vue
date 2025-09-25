<template>
    <v-dialog v-model="modelValue" max-width="900px">
        <v-card>
            <v-toolbar density="compact" flat>
                <v-toolbar-title class="text-subtitle-1">从图片库中选择</v-toolbar-title>
                <v-spacer />
                <v-btn icon="mdi-close" variant="text" @click="close" />
            </v-toolbar>
            <v-card-text v-if="!loadingImages" class="pt-4" style="max-height: 70vh; overflow: hidden auto;">
                <v-row dense>
                    <v-col v-for="image in images" :key="image.id" cols="6" sm="4" md="3" class="pa-4">
                        <v-card hover @click="emit('select', image); close()" class="ma-2">
                            <v-img :src="`/${image.imagePath}`" aspect-ratio="1" cover />
                            <v-card-text class="pa-2">
                                <div class="font-weight-bold">Date: {{ new Date(image.createdAt+'Z').toLocaleDateString() }}</div>
                                <div class="text-caption">Size: {{ (image.size / 1024).toFixed(1) }} KB</div>
                            </v-card-text>
                        </v-card>
                    </v-col>
                </v-row>
            </v-card-text>
            <template v-else>
                <v-card-text class="pa-6">
                    <v-progress-circular color="primary" indeterminate class="mt-6" />
                </v-card-text>
            </template>
            <v-divider />
            <v-card-actions>
                <v-spacer />
                <div class="d-flex justify-center">
                  <v-pagination
                    v-model="pageNumberUI"
                    :length="pagination.TotalPages"
                    rounded="circle"
                    total-visible="5"
                    @update:modelValue="onPageChange"
                  />
                </div>
                <v-spacer />
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import type { ImageDto } from '@/types';
import { listUploadedImages } from '@/services/upload';

interface Emits {
    (e: 'close'): void
    (e: 'select', image: ImageDto): void
}

const emit = defineEmits<Emits>()
const images = ref<ImageDto[]>([])
const loadingImages = ref(true)
const pagination = ref({ TotalCount: 0, PageSize: 12, PageNumber: 0, TotalPages: 0 })
const pageNumberUI = ref(1)
const pageSize = ref(12)

async function loadImages() {
    loadingImages.value = true
    try {
        const { items, pagination: meta } = await listUploadedImages(pageNumberUI.value - 1, pageSize.value)
        images.value = items
        pagination.value = meta
    } catch (e) {
        console.error('Error loading images', e)
    } finally {
        loadingImages.value = false
    }
}

function onPageChange() { loadImages() }

onMounted(loadImages)

function close() {
    modelValue.value = false
    emit('close')
    // reset after animation (optional) – leave selection so user reopening resumes
}

const modelValue = defineModel<boolean>({ default: false, required: true })
</script>