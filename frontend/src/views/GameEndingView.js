import {Component} from "../core/Component.js";
import {Button, Logo} from "../components/UI.js";
import {get404, redirectTo} from "../services/RouteServices.js";
import {tryGetGameSettings} from "../services/GamesServices.js";
import {GameSettings} from "../domain/GameSettings.js";
import {OnGameRating} from "../components/OnGameRating.js";
import {OnGameTable} from "../components/OnGameTable.js";

export class GameEndingView extends Component {
    constructor(container, data) {
        super(container, data);
    }

    async mount() {
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>

            <div class="editor-layout player-mode">
                <main class="editor-main">
                    <div class="table-header-actions">
                        <div class="badge">Код комнаты: ${this.data.roomCode}</div>
                    </div>

                    ${OnGameTable(this.gameSettings, this.state, this._tableOptions)}
                    ${OnGameRating(this.state.players, sessionStorage.getItem('team-name'))}
                </main>
            </div>
        `;
    }

    _connectSocket() {
        if (this.ws) {
            return;
        }

        const protocol = window.location.protocol === 'https:' ? 'wss' : 'ws';

        this.ws = new WebSocket(
            `${protocol}://${window.location.host}/api/ws/room/${this.data.roomCode}`
        );

        this.ws.onmessage = (event) => {
            const data = JSON.parse(event.data);
            console.log(data);

            if (data.type === 'track_started') {
                const row = data.row;
                const col = data.col;
                this.state.activeCell = {
                    row: row,
                    col: col,
                };
                const cell = this.gameSettings.cells?.[row]?.[col];
                cell.played = true;
                this.state.canBuzz = true;

                this.updateDOM();
            }

            if (data.type === 'reset_answer_btn') {
                const myTeam = sessionStorage.getItem('team-name') ?? 'Неизвестная команда';
                this.state.hadWrongAnswer = data.disabledTeams.includes(myTeam);
                this.state.canBuzz = !this.state.hadWrongAnswer;
                this.state.whoAnswers = null;
                this.updateDOM();

                if (this.state.hadWrongAnswer) {
                    setTimeout(() => {
                        this.state.hadWrongAnswer = false;
                        this.updateDOM();
                    }, 3000);
                }
            }

            if (data.type === 'player_buzzed') {
                this.state.canBuzz = false;
                const myTeam = sessionStorage.getItem('team-name') ?? 'Неизвестная команда';
                this.state.whoAnswers = myTeam;
                if (data.team === myTeam) {
                    this.state.showAnswerInput = this.gameSettings.mode === false;
                }
                this.updateDOM();
            }

            if (data.type === 'show_answer') {
                this.state.answer = {
                    title: data.title,
                    artist: data.artist,
                }
                this.state.whoAnswers = null;
                this.state.canBuzz = false;
                this.state.showAnswerInput = false;
                this.state.activeCell = null;
                this.updateDOM();
                setTimeout(() => {
                    this.state.answer = null
                    this.updateDOM();
                }, 5000);
            }

            if (data.type === 'game_ended') {
                sessionStorage.removeItem('teams');
                sessionStorage.removeItem('team-name');
                redirectTo('/');
            }

            if (data.type === 'add_points') {
                console.log('add_points');
                const team = data.team;
                const points = data.points;
                this.state.players[team] += points;
                this.updateDOM();
            }
        };
    }

    attachEvents() {

        }
}