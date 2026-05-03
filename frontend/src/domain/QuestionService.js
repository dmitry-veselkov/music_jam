import {AudioManager} from "../services/AudioManager.js";

export class QuestionService {
    constructor(state, gameSettings, ws, updateDOM) {
        this.state = state;
        this.gameSettings = gameSettings;
        this.ws = ws;
        this.updateDOM = updateDOM;
    }

    async stop() {
        /**
         * Метод для завершения вопроса: нужно показать правильный ответ
         */
        if (!this.state.isShowingAnswer){
            this._showAnswer();
        } else {
            this.reset();
        }
    }

    async processTeamAnswer(isCorrect, points) {
        AudioManager.pause();
        this._addPointsToBuzzedTeam(points);

        if (isCorrect) {
            this._showAnswer();
        } else {
            this._processWrongAnswer();
        }

        this._resetAnswerButton();
        this.updateDOM();
    }

    _showAnswer() {
        /**
         * Сбрасываем состояние вопроса, отправляем сокет на показ правильного ответа
         * и включаем файл с правильным ответом
         */

        const cell = this._getActiveCell();
        if (!cell) return;

        this.ws.send(JSON.stringify({
            type: 'show_answer',
            title: cell.song.title,
            artist: cell.song.artist,
        }));

        this.reset();

        this.state.isShowingAnswer = true;
        this.updateDOM();
        cell.song.playCorrectAnswer(this.state, () => {
            this.state.isShowingAnswer = false;
            this.updateDOM();
        });
    }

    _processWrongAnswer() {
        this.state.disabledTeams.push(this.state.buzzedTeam);
        this.state.buzzedTeam = null;
        this.state.teamAnswer = null;
        AudioManager.play();
    }

    startSong(e) {
        const row = +e.currentTarget.dataset.row;
        const col = +e.currentTarget.dataset.col;
        if (this.state.playedCells.some(c => c.row === row && c.col === col)) return;

        const cell = this.gameSettings.cells?.[row]?.[col];
        if (!cell) return;

        this.state.activeCell = {row, col};
        this.state.currentAnswer = {
            title: cell.song.title,
            artist: cell.song.artist,
        };

        cell.song.play(this.state);
        cell.played = true;
        this.ws.send(JSON.stringify({
            type: 'track_started',
            row,
            col
        }));
        this.state.playedCells.push(this.state.activeCell);
        this.updateDOM();
    }

    reset() {
        this.state.activeCell = null;
        this.state.buzzedTeam = null;
        this.state.disabledTeams = [];
        this.state.teamAnswer = null;
        this.state.currentAnswer = null;
    }

    _getActiveCell() {
        if (!this.state.activeCell)
            return;
        const {row, col} = this.state.activeCell;
        return this.gameSettings.cells?.[row]?.[col];
    }

    _addPointsToBuzzedTeam(points) {
        this.ws.send(JSON.stringify({
            type: 'add_points',
            team: this.state.buzzedTeam,
            points: points,
        }));
    }

    _resetAnswerButton() {
        this.ws.send(JSON.stringify({
            type: 'reset_answer_btn',
            disabledTeams: this.state.disabledTeams,
        }));
    }
}