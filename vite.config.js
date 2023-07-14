import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import fs from 'fs';

const path = require("path");

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: [{ find: "@", replacement: path.resolve(__dirname, "src") }]
  },
  define: {
    "process.env": process.env,
  },
  css: {
    preprocessorOptions: {
      scss: {
        additionalData: `@import "./src/_variables.scss";`,
        url: true,
      },
    },
  },
  server: {
    open: false,
    host: "0.0.0.0", // open up to all connections, but we reach it at local.allocate.build
    port: 3000,
    https: {
      key: fs.readFileSync("localcert.key"),
      cert: fs.readFileSync("localcert.pem")
    },
    watch: {
      usePolling: true,
    },
    proxy: {
        '/api': {
           target: 'https://local.allocate.build:5001'
           ,rewrite: (path) => path.replace(/^\/api/, ''),
        }
    }
  },
});
