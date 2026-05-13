import {fetchGetTo, fetchPostTo} from "./NetServices.js";

export async function tryGetRoomInfo(code) {
    return await fetchGetTo(`/api/get_room_info?code=${code}`);
}

export async function tryGetTeamInfo(code) {
    return await fetchGetTo(`/api/get_team_info?code=${code}`);
}

export async function setTeamName(code, newName) {
    const payload = {"code": code, "name": newName};
    return await fetchPostTo(`/api/update_team_name`, payload);
}

export async function removeTeam(teamName, code) {
    const payload = {"name": teamName, "code": code};
    return await fetchPostTo(`/api/remove_team`, payload);
}

export async function checkKicked(code) {
    return await fetchGetTo(`/api/check_kicked?code=${code}`);
}
