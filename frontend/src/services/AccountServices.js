import {redirectTo} from "./RouteServices.js";

export async function registerNewUser(name, email, password) {
    const response = await fetch("/api/register", {
        method: "POST",
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({name, email, password})
    });

    let data = {};
    try {
        data = await response.json();
    } catch {
    }

    if (response.ok) {
        return data.user;
    } else {
        throw new Error(data.message || "Ошибка регистрации");
    }
}

export async function checkUserExists(email, password) {
    const response = await fetch("/api/login", {
        method: "POST",
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({email, password})
    });

    if (response.ok) {
        return true;
    } else {
        const errorData = await response.json();
        throw new Error(errorData.message || "Login failed");
    }
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

export async function tryGetUserInfo() {
    const response = await fetch('/api/get_user_info', {
            credentials: 'include'
        }
    );

    if (response.ok) {
        return await response.json();
    }

    return null;
}