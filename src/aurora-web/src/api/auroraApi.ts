import axios from 'axios'

const baseURL = import.meta.env.VITE_AURORA_API_URL as string | undefined

if (!baseURL) {
  console.warn('VITE_AURORA_API_URL is not set, falling back to http://localhost:8080')
}

export const auroraApi = axios.create({
  baseURL: baseURL ?? 'http://localhost:8080',
  headers: {
    'Content-Type': 'application/json',
  },
})
