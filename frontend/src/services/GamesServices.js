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

export async function startGame(code){
    return await fetchPostTo(`/api/start_game?code=${code}`);
}

export async function endGame(code){
    return await fetchPostTo(`/api/end_game?code=${code}`);
}

export async function addPlayedTrack(rIdx, cIdx, code){
    const payload = { code, row: rIdx, column: cIdx };
    return await fetchPostTo(`/api/add_played_track?code=${code}`, payload);
}

export async function getRoomState(code) {
    return await fetchGetTo(`/api/room_state?code=${code}`);
}

export async function givePoints(code, team, points) {
    const payload = { code, team, points };
    return await fetchPostTo(`/api/give_points?code=${code}`, payload);
}