import { createBrowserRouter } from "react-router-dom";
import StartPage from "../pages/StartPage/StartPage";
import RegistrationPage from "@/pages/RegistrationPage/RegistrationPage.tsx";
import Packs from "@/pages/Packs/Packs.tsx";
import Pack from "@/pages/Pack/Pack.tsx";
import LoginPage from "@/pages/LoginPage/LoginPage.tsx";
import AccountPage from "@/pages/AccountPage/AccountPage.tsx";
import WaitingRoomPage from "@/pages/WaitingRoomPage/WaitingRoomPage.tsx";

export const routes = createBrowserRouter([
  { path: '/', element : <StartPage />},
  { path: '/register', element : <RegistrationPage />},
  { path: '/login', element : <LoginPage />},
  { path: '/account', element : <AccountPage />},
  { path: '/packs', element: <Packs />},
  { path: '/packs/:id', element: <Pack />},
  { path : '/games/:gameId/waiting-room', element : <WaitingRoomPage />},
])