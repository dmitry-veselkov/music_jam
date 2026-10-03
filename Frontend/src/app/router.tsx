import { createBrowserRouter } from "react-router-dom";
import StartPage from "../pages/StartPage/StartPage";

export const router = createBrowserRouter([
  { path: '/', element : <StartPage />}
])
