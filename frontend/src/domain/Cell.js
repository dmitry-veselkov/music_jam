export class Cell {
    constructor(category, cost, song = null) {
        this.category = category;
        this.cost = cost;
        this.song = song;
    }

    setSong(song) {
        this.song = song;
    }

    hasSong() {
        return this.song !== null;
    }

    get label() {
        return this.song ? `${this.song.title} — ${this.song.artist}` : this.cost;
    }
}