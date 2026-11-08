import { http } from "@/shared/api/axios.ts";

export const gameProcess = {
  create : async ({ packId }  : { packId : string}) => {
    const { data } = await http.post(`/runtime/games`, { packId });
    return data;
  }
}