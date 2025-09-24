<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import type { PromptTemplateCategory, PromptTemplate, PlaceholderSpec } from '@/constants/promptTemplates'
import { promptTemplateCategories, extractPlaceholders } from '@/constants/promptTemplates'

interface Emits {
    (e: 'close'): void
    (e: 'apply', value: { template: PromptTemplate; values: Record<string, string>; finalPrompt: string }): void
}
const emit = defineEmits<Emits>()

const props = defineProps<{ modelValue: boolean }>()
const internalModel = ref(props.modelValue)
watch(() => props.modelValue, v => (internalModel.value = v))
watch(internalModel, v => { if (!v) emit('close') })

// Step control
const step = ref(1) // 1: select template, 2: fill placeholders

const categories = ref<PromptTemplateCategory[]>(promptTemplateCategories)
const selectedCategoryId = ref<string | null>(categories.value[0]?.id || null)

const selectedCategory = computed(() => {
    return categories.value.find(c => c.id === selectedCategoryId.value) || null
})

const templatesForCategory = computed(() => {
    return selectedCategory.value?.templates || []
})

const selectedTemplate = ref<PromptTemplate | null>(null);

// Placeholder management with defaults
const placeholders = ref<PlaceholderSpec[]>([])
const placeholderValues = ref<Record<string, string>>({})

function selectTemplate(tmpl: PromptTemplate) {
    selectedTemplate.value = tmpl
    placeholders.value = extractPlaceholders(tmpl.raw)
    const map: Record<string, string> = {}
    placeholders.value.forEach(ph => {
        const existing = placeholderValues.value[ph.key]
        map[ph.key] = existing != null && existing.length > 0 ? existing : ph.defaultValue
    })
    placeholderValues.value = map
    step.value = 2
}

const allFilled = computed(() => placeholders.value.every(p => placeholderValues.value[p.key]?.trim().length))

const livePreview = computed(() => buildFinalPrompt())

function prevStep() {
    if (step.value === 2) step.value = 1
}

function close() {
    internalModel.value = false
    emit('close')
    // reset after animation (optional) – leave selection so user reopening resumes
}

function buildFinalPrompt(): string {
    if (!selectedTemplate.value) return ''
    let result = selectedTemplate.value.raw
    placeholders.value.forEach(ph => {
        const value = placeholderValues.value[ph.key] || ph.defaultValue || ''
        const escaped = ph.key.replace(/[.*+?^${}()|[\\]\\]/g, '\\$&')
        // Pattern matches [key:whateverDefaultTextMaybeWithColons]
        const pattern = new RegExp('\\[' + escaped + '(?::[^\\]]*)?\\]', 'g')
        result = result.replace(pattern, value)
    })
    return result
}

function applyTemplate() {
    if (!selectedTemplate.value) return
    const finalPrompt = buildFinalPrompt()
    emit('apply', { template: selectedTemplate.value, values: { ...placeholderValues.value }, finalPrompt })
    close()
}
</script>

<template>
    <v-dialog v-model="internalModel" max-width="920" persistent>
        <v-card>
            <v-toolbar color="primary" density="compact" flat>
                <v-toolbar-title class="text-subtitle-1">选择提示模板</v-toolbar-title>
                <v-spacer />
                <v-btn icon="mdi-close" variant="text" @click="close" />
            </v-toolbar>
            <v-card-text class="pt-4" style="max-height: 70vh; overflow: hidden auto;">
                <v-window v-model="step">
                    <v-window-item :value="1">
                        <div>
                            <v-tabs v-model="selectedCategoryId">
                                <v-tab v-for="cat in categories" :key="cat.id" :value="cat.id">
                                    {{ cat.name }}
                                </v-tab>
                            </v-tabs>
                        </div>
                        <div class="ml-4 mt-4 text-caption">
                            {{ selectedCategory?.description }}
                        </div>
                        <div class="flex-grow-1">
                            <v-list density="comfortable" class="template-list">
                                <v-list-subheader>Templates</v-list-subheader>
                                <v-list-item v-for="t in templatesForCategory" :key="t.id"
                                    @click="() => selectTemplate(t)">
                                    <v-list-item-title>{{ t.title }}</v-list-item-title>
                                    <v-list-item-subtitle class="text-caption text-truncate">{{ t.raw
                                    }}</v-list-item-subtitle>
                                    <template #append>
                                        <v-icon>mdi-chevron-right</v-icon>
                                    </template>
                                </v-list-item>
                            </v-list>
                        </div>
                    </v-window-item>
                                        <v-window-item :value="2">
                                            <div class="d-flex flex-column" style="gap: 12px;">
                                                <div class="text-subtitle-2">填写占位符 & 预览</div>
                                                <v-alert type="info" variant="tonal" density="compact" v-if="selectedTemplate">
                                                    {{ selectedTemplate.description }}
                                                </v-alert>
                                                <v-row class="edit-preview-grid">
                                                    <v-col cols="12" md="8" class="edit-panel">
                                                        <v-form>
                                                            <div v-for="ph in placeholders" :key="ph.key" class="mb-3">
                                                                <v-text-field
                                                                    v-model="placeholderValues[ph.key]"
                                                                    :label="ph.key"
                                                                    :placeholder="ph.defaultValue"
                                                                    density="compact"
                                                                    variant="outlined"
                                                                    hide-details="auto"
                                                                    clearable
                                                                    persistent-hint
                                                                    :rules="[v => !!v && v.trim().length > 0 || '必填']"
                                                                />
                                                            </div>
                                                        </v-form>
                                                    </v-col>
                                                    <v-col cols="12" md="4" class="preview-panel">
                                                        <div class="text-caption mb-1 d-flex align-center">
                                                            <v-icon size="14" class="mr-1">mdi-eye-outline</v-icon> 实时预览
                                                        </div>
                                                        <div class="preview-text">{{ livePreview }}</div>
                                                    </v-col>
                                                </v-row>
                                            </div>
                                        </v-window-item>
                </v-window>
            </v-card-text>
            <v-divider />
            <v-card-actions>
                <template v-if="step === 1">
                    <div class="ml-8">
                        Learn more about prompt templates <a
                            href="https://ai.google.dev/gemini-api/docs/image-generation?hl=zh-cn#prompt-guide"
                            target="_blank">here</a>.
                    </div>
                    <v-spacer />
                </template>
                <template v-else>
                    <v-btn variant="text" @click="prevStep">上一步</v-btn>
                    <v-spacer />
                    <v-btn variant="flat" color="primary" :disabled="!allFilled" @click="applyTemplate">应用</v-btn>
                </template>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<style scoped>
.template-list .v-list-item {
    cursor: pointer;
}
</style>