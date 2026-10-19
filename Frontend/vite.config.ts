import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'
import { fileURLToPath, URL } from 'node:url'
import { msw } from "msw/vite";

export default defineConfig({
  plugins: [react(), msw()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost.musicjam.fun:5555',
        secure: false,
        changeOrigin: true,
      },
    },
  },
})
