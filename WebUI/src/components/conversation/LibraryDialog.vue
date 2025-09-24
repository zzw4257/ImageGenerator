<template>
    <v-dialog v-model="modelValue" max-width="60vw">
        <v-card>
            <v-toolbar color="primary" density="compact" flat>
                <v-toolbar-title class="text-subtitle-1">从图片库中选择</v-toolbar-title>
                <v-spacer />
                <v-btn icon="mdi-close" variant="text" @click="close" />
            </v-toolbar>
            <v-card-text v-if="!loadingImages" class="pt-4" style="max-height: 70vh; overflow: hidden auto;">
                <v-row dense>
                    <v-col
                        v-for="image in images"
                        :key="image.id"
                        cols="6"
                        sm="4"
                        md="3"
                        class="pa-4"
                    >
                        <v-card
                            hover
                            @click="emit('select', image); close()"
                            class="ma-2"
                        >
                            <v-img
                                :src="`/${image.imagePath}`"
                                aspect-ratio="1"
                                cover
                            />
                            <v-card-text class="d-flex justify-space-between pa-4">
                                <span>{{ new Date(image.createdAt+'Z').toLocaleDateString() }}</span>
                                <span>{{ (image.size / 1024).toFixed(1) }} KB</span>
                            </v-card-text>
                        </v-card>
                    </v-col>
                </v-row>
            </v-card-text>
            <template v-else>
                <v-card-text class="pa-6">
                    <v-skeleton-loader type="list-item-avatar, list-item-content" :loading="loadingImages" />
                </v-card-text>
            </template>
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

onMounted(async () => {
    loadingImages.value = true
    try {
        // load images from API
        images.value = await listUploadedImages()
    } catch (e) {
        console.error('Error loading images', e)
    } finally {
        loadingImages.value = false
    }
})

function close() {
    modelValue.value = false
    emit('close')
    // reset after animation (optional) – leave selection so user reopening resumes
}

const modelValue = defineModel<boolean>({ default: false, required: true })
</script>