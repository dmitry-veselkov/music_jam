export async function tryGetRoomInfo(code) {
    try {
        const response = await fetch(`/api/get_room_info?code=${code}`);
        if (!response.ok) {
            throw new Error(`Ошибка сервера: ${response.status}`);
        }

        return await response.json();

    } catch (error) {
        console.error('Ошибка: ', error);
        return null;
    }
}

export async function tryGetGameSettings(code) {
    // TODO Запрос на бэк
    try {
        const response = await fetch(`/api/gameSettings?code=${code}`);
        if (!response.ok) {
            throw new Error(`Ошибка сервера: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error('Ошибка: ', error);
        return {exists: false, roomCode: code, roomInfo: null};
    }
}

export async function saveGameSettings(payload) {
    // TODO Запрос на бэк
    try {
        const response = await fetch('/api/gameSettings', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error('Ошибка сохранения');
        }

        const result = await response.json();
        console.log('Сохранено:', result);
    } catch (error) {
        console.error('Не удалось сохранить игру:', error);
    }
}

export async function getTeamNameByUUID(uuid) {
    try {
        const response = await fetch(`/api/get_team_name?uuid=${uuid}`, {
            credentials: 'include',
        });

        if (!response.ok) {
            throw new Error(`Ошибка сервера: ${response.status}`);
        }

        const json = await response.json();
        return json.name;

    } catch (error) {
        console.error('Ошибка: ', error);
        return null;
    }
}

export async function setTeamName(gameId, teamId, newName) {
    try {
        const response = await fetch(`/api/set_team_name`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                "id": gameId,
                "uuid": teamId,
                "name": newName,
            })
        });

        if (!response.ok) {
            throw new Error(`Ошибка сервера: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('Ошибка: ', error);
    }
}

export function get404() {
    /**
     * Струтура страницы 404, которая будет отображаться,
     * когда страница была не найдена
     */
    return `
            <div class="page-layout">
                <main class="main-content">
                    <h1>404</h1>
                    <p>Страница не найдена</p>
                    <a href="/" class="btn btn-primary">На главную</a>
                </main>
            </div>
        `;
}