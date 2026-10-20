import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import '@/app/styles/base.css'
import '@/app/styles/variables.css'
import App from './app/App.tsx'

// закомментировал, чтобы отключить моки фронте и начать обрабатывать апи запросы на бэке
// 
// async function enableMocking() {
// 
//   const { worker } = await import('./app/mocks/browser');
//   return worker.start();
// }
// 
// enableMocking().then(() => {
//   createRoot(document.getElementById('root')!).render(
//     <StrictMode>
//       <App />
//     </StrictMode>,
//   )
// })

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)