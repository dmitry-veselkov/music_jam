export class Song {
    constructor(title, artist, questionUrl, answerUrl) {
        this.title = title;
        this.artist = artist;
        this.questionUrl = questionUrl;
        this.answerUrl = answerUrl;
    }

    play(gameState) {
        if (!this.questionUrl) return;

        if (gameState.audio) {
            gameState.audio.pause();
            gameState.audio = null;
        }

        const url = `/api/play?file_name=${encodeURIComponent(this.questionUrl)}`;
        gameState.audio = new Audio(url);

        gameState.audio.play().catch(err => {
            console.error("Ошибка воспроизведения:", err);
        });
    }
}