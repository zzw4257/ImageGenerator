<template>
    <v-main class="d-flex align-center justify-center min-h-screen">
        <v-container class="fill-height">
            <v-row justify="center" align="center">
                <v-col cols="12" sm="8" md="6" lg="4" xl="3">
                    <v-card class="pa-6 login-card" elevation="8" rounded="lg">
                        <v-card-title class="text-center text-h4 mb-4">
                            <v-icon size="large" class="mr-2">mdi-account-circle</v-icon>
                            {{ pageTitle }}
                        </v-card-title>
                        <v-card-text>
                            <router-view v-slot="{ Component }">
                                <transition name="fade" mode="out-in">
                                    <component :is="Component" />
                                </transition>
                            </router-view>
                        </v-card-text>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </v-main>
</template>

<script lang="ts" setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const pageTitle = computed(() => {
    if (route.path === '/register') {
        return 'Create Account'
    }
    return 'Login'
})
</script>