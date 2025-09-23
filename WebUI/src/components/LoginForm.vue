<template>
    <v-form @submit.prevent="login">
        <v-text-field v-model="username" label="Username" prepend-inner-icon="mdi-account" variant="outlined" required
            class="mb-3"></v-text-field>
        <v-text-field v-model="password" label="Password" type="password" prepend-inner-icon="mdi-lock"
            variant="outlined" required class="mb-4"></v-text-field>
        <v-btn :loading="loading" :disabled="!canSubmit" type="submit" color="primary" size="large" block class="text-none">
            Login
        </v-btn>
        <div v-if="error" class="mt-3 text-error text-body-2">{{ error }}</div>
        
        <div class="text-center mt-4">
            <span class="text-body-2 text-grey-darken-1">Don't have an account? </span>
            <router-link to="/register" class="text-primary text-decoration-none">
                Sign up
            </router-link>
        </div>
    </v-form>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue';
import type { AuthInfo } from '@/types';
import { useAppStore } from '@/stores/app';
import { useRouter } from 'vue-router';
import * as authApi from '@/services/auth'

const appStore = useAppStore();
const router = useRouter();

const username = ref('');
const password = ref('');
const loading = ref(false)
const error = ref('')

const canSubmit = computed(() => !!username.value && !!password.value)

const login = async () => {
    error.value = ''
    if (!canSubmit.value) return

    loading.value = true
    try {
        const data: AuthInfo = await authApi.login({
            username: username.value,
            password: password.value,
        })
        appStore.setAuthInfo(data)
        // Navigate to home after successful login
        router.push('/')
    } catch (e: any) {
        // Provide user-friendly error
        error.value = e?.response?.data?.message || 'Login failed. Please check your credentials.'
    } finally {
        loading.value = false
    }
};
</script>