import {Cell} from "./Cell.js";
import {Song} from "./Song.js";

export class GameSettings {
    constructor() {
        this.author = '';
        this.title = '';
        this.description = '';

        this.categories = [];
        this.costs = [];

        this.cells = [];
    }

    init(roomInfo, userInfo) {
        this.author = roomInfo.author;
        this.title = roomInfo.title;
        this.description = roomInfo.description;

        this.categories = roomInfo.categories ?? [];
        this.costs = roomInfo.costs ?? [];

        this._buildGrid(roomInfo.tracks ?? {});
    }

    _buildGrid(tracks) {
        this.cells = this.categories.map((cat, rIdx) => {
            return this.costs.map((cost, cIdx) => {

                const track = tracks?.[cat]?.[cost];

                const song = track
                    ? new Song(
                        track.title,
                        track.artist,
                        track.question_url,
                        track.answer_url
                    )
                    : null;

                return new Cell(cat, cost, song);
            });
        });
        console.log(this.cells);
    }

    addRow(categoryName) {
        this.categories.push(categoryName);

        const newRow = this.costs.map(cost => ({
            cost,
            song: null
        }));

        this.cells.push(newRow);
    }

    addCol(cost) {
        this.costs.push(cost);

        this.cells.forEach(row => {
            row.push({
                cost,
                song: null
            });
        });
    }

    removeRow(index) {
        this.categories.splice(index, 1);
        this.cells.splice(index, 1);
    }

    removeCol(index) {
        this.costs.splice(index, 1);

        this.cells.forEach(row => {
            row.splice(index, 1);
        });
    }

    getCell(row, col) {
        return this.cells[row][col];
    }

    setSong(row, col, song) {
        this.cells[row][col].song = song;
    }
}