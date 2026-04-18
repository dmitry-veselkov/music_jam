export async function fetchGetTo(api_path) {
    /**
     * GET-запрос на api_path
     */
    try {
        const response = await fetch(api_path, {
            credentials: 'include',
        });
        if (!response.ok) {
            throw new Error(`Ошибка сервера при запросе на ${api_path}: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error(`Ошибка при запросе на ${api_path}: ${error}`);
        return null;
    }
}

export async function fetchPostTo(api_path, payload) {
    /**
     * POST-запрос на api_path c данными из payload
     */
    try {
        const response = await fetch(api_path, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error(`Ошибка сервера при запросе на ${api_path}: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error(`Ошибка при запросе на ${api_path}: ${error}`);
        return null;
    }
}