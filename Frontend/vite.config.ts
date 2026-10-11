import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'
import { fileURLToPath, URL } from 'node:url'
import { msw } from "msw/vite";
import * as fs from "node:fs";

export default defineConfig({
  plugins: [react(), msw()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    host: 'localhost.musicjam.fun',
    port: 5173,
    https: {
      key: fs.readFileSync("C:/Users/aaa94/music_jam/Backend/MusicJam.Web/localhost.musicjam.fun-key.pem"),
      cert: fs.readFileSync("C:/Users/aaa94/music_jam/Backend/MusicJam.Web/localhost.musicjam.fun.pem")
    },
    proxy: {
      '/api': {
        target: 'https://localhost.musicjam.fun:5555',
        secure: false,
        changeOrigin: true,
      },
    },
  },
})
