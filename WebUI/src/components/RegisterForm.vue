<template>
    <v-form @submit.prevent="register">        
        <v-text-field 
            v-model="username" 
            label="Username" 
            prepend-inner-icon="mdi-account" 
            variant="outlined" 
            required
            :error-messages="usernameErrors"
            class="mb-3"
        ></v-text-field>
        
        <v-text-field 
            v-model="password" 
            label="Password" 
            type="password" 
            prepend-inner-icon="mdi-lock"
            variant="outlined" 
            required
            :error-messages="passwordErrors"
            class="mb-3"
        ></v-text-field>
        
        <v-text-field 
            v-model="confirmPassword" 
            label="Confirm Password" 
            type="password" 
            prepend-inner-icon="mdi-lock-check"
            variant="outlined" 
            required
            :error-messages="confirmPasswordErrors"
            class="mb-4"
        ></v-text-field>

        <v-text-field 
            v-model="invitationCode" 
            label="Invitation Code" 
            prepend-inner-icon="mdi-ticket" 
            variant="outlined" 
            required
            :error-messages="invitationCodeErrors"
            class="mb-3"
        ></v-text-field>
        
        <v-btn 
            :loading="loading" 
            :disabled="!canSubmit" 
            type="submit" 
            color="primary" 
            size="large" 
            block 
            class="text-none"
        >
            Create Account
        </v-btn>
        
        <div v-if="error" class="mt-3 text-error text-body-2">{{ error }}</div>
        
        <div class="text-center mt-4">
            <span class="text-body-2 text-grey-darken-1">Already have an account? </span>
            <router-link to="/login" class="text-primary text-decoration-none">
                Sign in
            </router-link>
        </div>
    </v-form>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue';
import type { AuthInfo } from '@/types';

/**
 * A component that provides a registration form for new users.
 */
import { useAppStore } from '@/stores/app';
import { useRouter } from 'vue-router';
import { useNotificationStore } from '@/stores/notification';
import * as authApi from '@/services/auth'

const appStore = useAppStore();
const router = useRouter();
const notificationStore = useNotificationStore();

const invitationCode = ref('');
const username = ref('');
const password = ref('');
const confirmPassword = ref('');
const loading = ref(false);
const error = ref('');

// Validation computed properties
const invitationCodeErrors = computed(() => {
    if (!invitationCode.value) return ['Invitation code is required'];
    if (invitationCode.value.length < 6) return ['Invitation code must be at least 6 characters'];
    return [];
});

const usernameErrors = computed(() => {
    if (!username.value) return ['Username is required'];
    if (username.value.length < 3) return ['Username must be at least 3 characters'];
    if (username.value.length > 50) return ['Username must be less than 50 characters'];
    return [];
});

const passwordErrors = computed(() => {
    if (!password.value) return ['Password is required'];
    if (password.value.length < 6) return ['Password must be at least 6 characters'];
    if (password.value.length > 100) return ['Password must be less than 100 characters'];
    return [];
});

const confirmPasswordErrors = computed(() => {
    if (!confirmPassword.value) return ['Please confirm your password'];
    if (password.value !== confirmPassword.value) return ['Passwords do not match'];
    return [];
});

const canSubmit = computed(() => {
    return invitationCodeErrors.value.length === 0 &&
           usernameErrors.value.length === 0 &&
           passwordErrors.value.length === 0 &&
           confirmPasswordErrors.value.length === 0 &&
           invitationCode.value &&
           username.value &&
           password.value &&
           confirmPassword.value;
});

const register = async () => {
    error.value = '';
    if (!canSubmit.value) return;

    loading.value = true;
    try {
        const data: AuthInfo = await authApi.register({
            username: username.value,
            password: password.value,
            invitationCode: invitationCode.value,
        });
        
        appStore.setAuthInfo(data);
        notificationStore.success('Account created successfully');
        
        // Navigate to home after successful registration
        router.push('/');
    } catch (e: any) {
        // Provide user-friendly error messages
        const errorMessage = e?.response?.data || 'Registration failed. Please try again.';
        error.value = errorMessage;
        
        // Show specific error for invalid invitation code
        if (errorMessage.toLowerCase().includes('invitation')) {
            notificationStore.error('Invalid invitation code. Please check and try again.');
        } else if (errorMessage.toLowerCase().includes('username')) {
            notificationStore.error('Username already exists. Please choose a different one.');
        } else {
            notificationStore.error('Registration failed. Please try again.');
        }
    } finally {
        loading.value = false;
    }
};
</script>