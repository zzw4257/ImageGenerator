<template>
    <v-form>
        <v-text-field v-model="username" label="Username" prepend-inner-icon="mdi-account" variant="outlined" required
            class="mb-3"></v-text-field>
        <v-text-field v-model="password" label="Password" type="password" prepend-inner-icon="mdi-lock"
            variant="outlined" required class="mb-4"></v-text-field>
        <v-btn @click="login" color="primary" size="large" block class="text-none">
            Login
        </v-btn>
    </v-form>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import axios from "@/helpers/RequestHelper"
import type { AuthInfo } from '@/types';
import { useAppStore } from '@/stores/app';
import { useRouter } from 'vue-router';

const appStore = useAppStore();
const router = useRouter();

const username = ref('');
const password = ref('');

const login = () => {
    // Handle login logic here
    console.log('Login attempt:', { username: username.value, password: password.value });
    axios.post("/Authentication/login", {
        username: username.value,
        password: password.value
    }).then(res => {
        let data = res.data as AuthInfo
        appStore.setAuthInfo(data);
        router.push("/")
    })
};
</script>