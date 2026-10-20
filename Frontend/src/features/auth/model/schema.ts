import { z } from "zod";

export const authSchema = z.object({
  nickname: z.string(),
  email: z.string(),
  password: z.string(),
})

export type AuthRequest = z.infer<typeof authSchema>;

export const loginSchema = z.object({
  email: z.string(),
  password: z.string(),
})

export type LoginRequest = z.infer<typeof loginSchema>;