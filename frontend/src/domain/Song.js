import {AudioManager as audioManager, AudioManager} from "../services/AudioManager.js";

export class Song {
    constructor(title, artist, questionUrl, answerUrl) {
        this.title = title;
        this.artist = artist;
        this.questionUrl = questionUrl;
        this.answerUrl = answerUrl;
    }

    play(gameState) {
        if (!this.questionUrl) return;

        const url = `/api/play?file_name=${encodeURIComponent(this.questionUrl)}`;
        AudioManager.set(url);
        AudioManager.play();

        AudioManager.currentAudio.addEventListener('ended', () => {
            AudioManager.currentAudio = null;
        });
    }

    playCorrectAnswer(gameState, onEnded = null) {
        if (!this.answerUrl) return;

        const url = `/api/play?file_name=${encodeURIComponent(this.answerUrl)}`;
        AudioManager.set(url);
        AudioManager.play();

        AudioManager.currentAudio.addEventListener('ended', () => {
            AudioManager.currentAudio = null;
            if (onEnded) onEnded();
        });
    }
}