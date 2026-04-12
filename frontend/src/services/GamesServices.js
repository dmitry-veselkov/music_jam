export async function getAllUserGames(email) {
    const response = await fetch(`/api/get_all_user_games?email=${email}`);
    if (response.ok) {
        const json = await response.json();
        console.log(json.games);
        return json.games;
    } else {
        return [];
    }
}