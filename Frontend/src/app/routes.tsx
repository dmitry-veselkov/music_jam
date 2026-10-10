import { createBrowserRouter } from "react-router-dom";
import StartPage from "../pages/StartPage/StartPage";
import RegistrationPage from "@/pages/RegistrationPage/RegistrationPage.tsx";
import Packs from "@/pages/Packs/Packs.tsx";
import Pack from "@/pages/Pack/Pack.tsx";

export const routes = createBrowserRouter([
  { path: '/', element : <StartPage />},
  { path: '/register', element : <RegistrationPage />},
  { path: '/packs', element: <Packs />},
  { path: '/packs/:id', element: <Pack />}
])
