import {get404, redirectTo} from "./RouteServices.js";
import {fetchGetTo, fetchPostTo} from "./NetServices.js";
import {tryGetGameSettings} from "./GamesServices.js";

export async function registerNewUser(name, email, password) {
    return await fetchPostTo('/api/register', {name, email, password});
}

export async function checkUserExists(email, password) {
    return await fetchPostTo(`/api/login`, {email, password});
}

export async function tryGetUserInfo() {
    return fetchGetTo(`/api/get_user_info`);
}

export async function checkAuthorizedOrRedirect() {
    try {
        const userInfo = await tryGetUserInfo();
        if (!userInfo) {
            redirectTo('/login');
            return;
        }
        return userInfo;
    } catch (e) {
        redirectTo('/login');
    }
}

export async function authenticationToGame(container, roomCode) {
    const userInfo = await checkAuthorizedOrRedirect();
    if (!userInfo) return;

    const roomInfo = await tryGetGameSettings(roomCode);
    if (!roomInfo || !roomInfo.exists || userInfo.id != roomInfo.admin_user_id) {
        container.innerHTML = get404();
        return;
    }

    return [userInfo, roomInfo];
}