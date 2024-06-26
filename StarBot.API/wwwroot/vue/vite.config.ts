import { fileURLToPath, URL } from 'node:url'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers'

// https://vitejs.dev/config/
export default defineConfig({
  base: "./",
  plugins: [
    vue(),
    AutoImport({
      resolvers: [ElementPlusResolver()],
    }),
    Components({
      resolvers: [ElementPlusResolver()],
    })
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    host: '0.0.0.0',
    open: true,
    hmr: true,
    proxy: {},
  },
  build: {
    outDir: '../bot/',
    chunkSizeWarningLimit: 2048 * 2,
    rollupOptions: {
      output: {
        entryFileNames: 'index.js',
        assetFileNames: '[name][extname]',
        chunkFileNames: '[name].js'
      }
    }
  }
})