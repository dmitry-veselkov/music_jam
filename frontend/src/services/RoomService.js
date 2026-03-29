export async function checkRoomInfo(code) {
    // TODO Написать на бэке обработку с проверкой введенного кода
    const url = `/api/check_room?roomCode=${code}`;
    try {
        const fetchResult = await fetch(url);
        if (fetchResult.ok) {
            const data = await fetchResult.json();
            return {
                exists: data['exists'],
                roomCode: data['code'],
                status: data['status'],
            };
        }
    } catch (error) {
        console.error(`Ошибка сети: ${error}`);
    }
}

export async function tryEnterToRoom(data) {
    const d = await checkRoomInfo(data.roomCode);
    console.log(d);
    if (!d.exists) {
        return false;
    }

    data.roomCode = d.roomCode;
    return true;
}

export function get404() {
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