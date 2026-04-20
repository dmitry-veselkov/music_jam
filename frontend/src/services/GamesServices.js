import {fetchGetTo, fetchPostTo} from "./NetServices.js";

export async function getAllUserGames() {
    const games = await fetchGetTo(`/api/get_all_user_games`);
    return games ?? [];
}

export async function generateEmptyGame() {
    return await fetchPostTo(`/api/create_new_game`);
}

export async function tryRunGame(code) {
    return await fetchGetTo(`/api/run_game?code=${code}`);
}

export async function tryGetGameSettings(code) {
    return await fetchGetTo(`/api/gameSettings?code=${code}`);
}

export async function saveGameSettings(payload) {
    return await fetchPostTo(`/api/gameSettings`, payload);
}