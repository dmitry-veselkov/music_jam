import {fetchGetTo, fetchPostTo} from "./NetServices.js";

export async function tryGetRoomInfo(code) {
    return await fetchGetTo(`/api/get_room_info?code=${code}`);
}

export async function getTeamNameByUUID(uuid) {
    return await fetchGetTo(`/api/get_team_name?uuid=${uuid}`);
}

export async function setTeamName(gameId, code, teamId, oldName, newName) {
    const payload = {"id": gameId, "code": code, "uuid": teamId, "oldName": oldName, "name": newName};
    console.log(payload);
    return await fetchPostTo(`/api/set_team_name`, payload);
}