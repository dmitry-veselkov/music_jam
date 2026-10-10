import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import '@/app/styles/base.css'
import '@/app/styles/variables.css'
import App from './app/App.tsx'

async function enableMocking() {
  // if (import.meta.env.VITE_USE_MOCKS !== 'true') return;

  const { worker } = await import('./app/mocks/browser');
  return worker.start();
}

enableMocking().then(() => {
  createRoot(document.getElementById('root')!).render(
    <StrictMode>
      <App />
    </StrictMode>,
  )
})

