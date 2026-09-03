import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // Forwards to the API Gateway (PaymentDetailApi.Gateway), which
      // load-balances across the API instances. Dev cert is self-signed,
      // so TLS verification is disabled for this proxy only.
      '/api': {
        target: 'https://localhost:7186',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
