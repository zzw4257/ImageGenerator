export type ThemePreference = "dark" | "light" | "system";

export interface AuthInfo {
    userId: string,
    token: string,
    expirationTime: string
}