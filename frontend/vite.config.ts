import VueDevTools from "vite-plugin-vue-devtools";
import vue from "@vitejs/plugin-vue";
import vueJsx from "@vitejs/plugin-vue-jsx";
import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";

import packageJson from "./package.json";

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue(), vueJsx(), VueDevTools()],
  define: {
    "import.meta.env.VITE_APP_VERSION": JSON.stringify(packageJson.version),
  },
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  build: {
    outDir: "../backend/src/Krakenar.Web/wwwroot",
    emptyOutDir: true,
    rollupOptions: {
      output: {
        entryFileNames: "assets/[name].js",
        chunkFileNames: "assets/[name].js",
        assetFileNames: "assets/[name].[ext]",
      },
    },
  },
  server: {
    port: 7746,
  },
});
