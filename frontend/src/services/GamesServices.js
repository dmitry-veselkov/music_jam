export async function getAllUserGames() {
    const response = await fetch(`/api/get_all_user_games`);
    if (response.ok) {
        const json = await response.json();
        console.log(json.games);
        return json.games;
    } else {
        return [];
    }
}