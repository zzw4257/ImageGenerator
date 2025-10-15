import { type ThemeDefinition } from "vuetify";
import colors from "vuetify/util/colors";

/**
 * Creates a light theme definition from a given color.
 * @param color - The color to create the theme from.
 * @returns The theme definition.
 */
function createTheme(color: any): ThemeDefinition {
  return {
    colors: {
      primary: color.darken2,
      secondary: color.darken1,
      accent: color.darken3,
      info: color.base,
      background: color.lighten5,
      surface: color.lighten4,
      "surface-light": color.lighten4,
      "on-background": color.darken3,
      "on-surface": color.darken3,
      "on-surface-light": color.darken4,
    },
    dark: false,
    variables: {
      font: color.darken4,
    },
  };
}

/**
 * Creates a dark theme definition from a given color.
 * @param color - The color to create the theme from.
 * @returns The theme definition.
 */
function createDarkTheme(color: any): ThemeDefinition {
  return {
    colors: {
      primary: color.lighten2,
      secondary: color.lighten3,
      accent: color.lighten1,
      info: color.base,
      background: colors.grey.darken4,
      surface: colors.grey.darken3,
      "surface-light": colors.grey.darken2,
      "on-background": color.lighten5,
      "on-surface": color.lighten5,
      "on-surface-light": color.lighten4,
    },
    dark: true,
    variables: {
      "border-color": color.lighten3,
      "theme-colors-opacity": 0.8,
      font: color.lighten2,
    },
  };
}

/**
 * Generates a record of all the themes.
 * @returns A record of all the themes.
 */
export function generateTheme(): Record<string, ThemeDefinition> {
  const themes: Record<string, ThemeDefinition> = {};

  for (const key in colors) {
    if (key != "shades") {
      themes[`light-${key}`] = createTheme(colors[key as keyof typeof colors]);
      themes[`dark-${key}`] = createDarkTheme(
        colors[key as keyof typeof colors]
      );
    }
  }

  return themes;
}
