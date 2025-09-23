// Utilities
import { defineStore } from 'pinia'
import type { AuthInfo, ThemePreference } from '@/types';

export interface DeleteDialogInfo {
  show: boolean;
  title: string;
  message: string;
  itemName?: string;
  confirmText?: string;
  isDeleting: boolean;
  onConfirm?: () => Promise<void> | void;
}

export interface PromptReplaceDialogInfo {
  show: boolean;
  title: string;
  message: string;
  currentPrompt: string;
  newPrompt: string;
  isProcessing: boolean;
  onConfirm?: () => Promise<void> | void;
  onCancel?: () => void;
}

export const useAppStore = defineStore('app', () => {
  const isDarkMode = ref<boolean>(false); // Add isDarkMode state
  const ThemePreference = ref<ThemePreference>(localStorage.getItem('themePreference') as ThemePreference || 'system');
  const ColorPreference = ref<string>(localStorage.getItem('colorPreference') || 'default');
  const authInfo = ref<AuthInfo>({
    userId: localStorage.getItem('userId') || '',
    token: localStorage.getItem('token') || '',
    expirationTime: localStorage.getItem('expirationTime') || ''
  });

  const deleteDialogInfo = ref<DeleteDialogInfo>({
    show: false,
    title: '',
    message: '',
    itemName: '',
    confirmText: 'Delete',
    isDeleting: false,
    onConfirm: undefined
  });

  const promptReplaceDialogInfo = ref<PromptReplaceDialogInfo>({
    show: false,
    title: '',
    message: '',
    currentPrompt: '',
    newPrompt: '',
    isProcessing: false,
    onConfirm: undefined,
    onCancel: undefined
  });

  const setAuthInfo = (data: AuthInfo) => {
    authInfo.value = data;
    localStorage.setItem('userId', data.userId);
    localStorage.setItem('token', data.token);
    localStorage.setItem('expirationTime', data.expirationTime);
  }

  const clearAuthInfo = () => {
    authInfo.value = { userId: '', token: '', expirationTime: '' };
    localStorage.removeItem('userId');
    localStorage.removeItem('token');
    localStorage.removeItem('expirationTime');
  }

  const setDarkMode = (value: boolean) => {
    isDarkMode.value = value;
  };

  const setPreferences = (theme: ThemePreference, color: string) => {
    ThemePreference.value = theme;
    ColorPreference.value = color;
    localStorage.setItem('themePreference', theme);
    localStorage.setItem('colorPreference', color);
  }

  const showDeleteDialog = (options: {
    title: string;
    message: string;
    itemName?: string;
    confirmText?: string;
    onConfirm: () => Promise<void> | void;
  }): Promise<boolean> => {
    return new Promise((resolve) => {
      deleteDialogInfo.value = {
        show: true,
        title: options.title,
        message: options.message,
        itemName: options.itemName,
        confirmText: options.confirmText || 'Delete',
        isDeleting: false,
        onConfirm: async () => {
          try {
            deleteDialogInfo.value.isDeleting = true;
            await options.onConfirm();
            hideDeleteDialog();
            resolve(true);
          } catch (error) {
            deleteDialogInfo.value.isDeleting = false;
            resolve(false);
            throw error;
          }
        }
      };
    });
  };

  const hideDeleteDialog = () => {
    deleteDialogInfo.value = {
      show: false,
      title: '',
      message: '',
      itemName: '',
      confirmText: 'Delete',
      isDeleting: false,
      onConfirm: undefined
    };
  };

  const showPromptReplaceDialog = (options: {
    currentPrompt: string;
    newPrompt: string;
    onConfirm: () => Promise<void> | void;
    onCancel?: () => void;
  }): Promise<boolean> => {
    return new Promise((resolve) => {
      promptReplaceDialogInfo.value = {
        show: true,
        title: 'Replace Prompt',
        message: 'You have text in the prompt input. Do you want to replace it with the selected timeline prompt?',
        currentPrompt: options.currentPrompt,
        newPrompt: options.newPrompt,
        isProcessing: false,
        onConfirm: async () => {
          try {
            promptReplaceDialogInfo.value.isProcessing = true;
            await options.onConfirm();
            hidePromptReplaceDialog();
            resolve(true);
          } catch (error) {
            promptReplaceDialogInfo.value.isProcessing = false;
            resolve(false);
            throw error;
          }
        },
        onCancel: () => {
          hidePromptReplaceDialog();
          if (options.onCancel) {
            options.onCancel();
          }
          resolve(false);
        }
      };
    });
  };

  const hidePromptReplaceDialog = () => {
    promptReplaceDialogInfo.value = {
      show: false,
      title: '',
      message: '',
      currentPrompt: '',
      newPrompt: '',
      isProcessing: false,
      onConfirm: undefined,
      onCancel: undefined
    };
  };

  return {
    ThemePreference,
    ColorPreference,
    authInfo,
    deleteDialogInfo,
    promptReplaceDialogInfo,
    setDarkMode,
    isDarkMode,
    setPreferences,
    setAuthInfo,
    clearAuthInfo,
    showDeleteDialog,
    hideDeleteDialog,
    showPromptReplaceDialog,
    hidePromptReplaceDialog
  };
})
