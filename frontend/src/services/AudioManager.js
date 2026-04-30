export class AudioManager {
    static currentAudio = null;

    static set(src) {
        this.stop();
        this.currentAudio = new Audio(src);
    }

    static play() {
        if (this.currentAudio) {
            this.currentAudio.play();
        }
    }

    static stop() {
        if (this.currentAudio) {
            this.currentAudio.pause();
            this.currentAudio.currentTime = 0;
            this.currentAudio = null;
        }
    }

    static pause() {
        if (this.currentAudio) {
            this.currentAudio.pause();
        }
    }
}