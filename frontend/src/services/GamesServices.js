export async function getAllUserGames() {
    const response = await fetch(`/api/get_all_user_games`);
    if (response.ok) {
        const json = await response.json();
        return json.games;
    } else {
        return [];
    }
}

export async function generateEmptyGame() {
    const response = await fetch(`/api/get_new_game_code`);
    if (response.ok) {
        const json = await response.json();
        return json.code;
    } else {
        return null;
    }
}