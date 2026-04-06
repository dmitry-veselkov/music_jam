export async function tryGetRoomInfo(code) {
    try {
        // TODO Запрос на бэк
        const response = await fetch(`/api/check_room?roomCode=${code}`);
        if (!response.ok) {
            throw new Error(`Ошибка сервера: ${response.status}`);
        }

        return await response.json();

    } catch (error) {
        console.error('Ошибка: ', error);
        return {exists: false, roomCode: code, roomInfo: null};
    }
}

export async function getTeamNameByUUID(uuid) {
    try {
        // TODO Запрос на бэк
        const response = await fetch(`/api/get_team_name`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                "uuid": uuid,
            })
        });
        if (!response.ok) {
            throw new Error(`Ошибка сервера: ${response.status}`);
        }

        const json = await response.json();
        return json['teamName'];

    } catch (error) {
        console.error('Ошибка: ', error);
        return null;
    }
}

export async function setTeamName(teamId, newName) {
    try {
        const response = await fetch(`/api/set_team_name`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                "teamId": teamId,
                "newName": newName,
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