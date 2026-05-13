export async function fetchGetTo(apiPath) {
    /**
     * GET-запрос на api_path
     */
    try {
        const response = await fetch(apiPath, {
            credentials: 'include',
        });
        if (!response.ok) {
            throw new Error(`Получен статус ${response.status} при запросе на {api_path}`);
        }
        return await response.json();
    } catch (error) {
        console.error(`Сетевая ошибка при запросе на ${apiPath}: ${error}`);
        return null;
    }
}

export async function fetchPostTo(apiPath, payload) {
    /**
     * POST-запрос на api_path c данными из payload
     */
    try {
        const response = await fetch(apiPath, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error(`Ошибка сервера при запросе на ${apiPath}: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error(`Ошибка при запросе на ${apiPath}: ${error}`);
        return null;
    }
}