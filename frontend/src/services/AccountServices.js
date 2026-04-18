import {redirectTo} from "./RouteServices.js";
import {fetchGetTo, fetchPostTo} from "./NetServices.js";

export async function registerNewUser(name, email, password) {
    return await fetchPostTo('/api/register', {name, email, password});
}

export async function checkUserExists(email, password) {
    return await fetchPostTo(`/api/login`, {email, password});
}

export async function tryGetUserInfo() {
    return fetchGetTo(`/api/get_user_info`);
}

export async function loadUserInfoOrRedirect() {
    try {
        const userInfo = await tryGetUserInfo();
        if (!userInfo) {
            redirectTo('/login');
            return;
        }
        return userInfo;
    } catch (e) {
        console.error(e);
        redirectTo('/login');
    }
}