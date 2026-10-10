import { z } from "zod";

export const authSchema = z.object({
  nickname: z.string(),
  email: z.string(),
  password: z.string(),
})

export type AuthRequest = z.infer<typeof authSchema>;