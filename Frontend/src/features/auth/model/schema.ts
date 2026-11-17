import { z } from "zod";

export const authSchema = z.object({
  username: z.string(),
  email: z.string(),
  password: z.string(),
})

export type AuthRequest = z.infer<typeof authSchema>;

export const loginSchema = z.object({
  email: z.string(),
  password: z.string(),
})

export type Role = 'host' | 'player' | 'viewer';

export type CheckRoleResponse = {
  role: Role;
  teamId: string | null;
  teamName: string | null;
}

export type LoginRequest = z.infer<typeof loginSchema>;