import {Component} from "../core/Component.js";
import {endGame} from "../services/GamesServices.js";
import {authenticationToGame} from "../services/AccountServices.js";
import {Logo} from "../components/UI.js";
import {GameSettings} from "../domain/GameSettings.js";
import {OnGameRating} from "../components/OnGameRating.js";
import {OnGameTable} from "../components/OnGameTable.js";
import {redirectTo} from "../services/RouteServices.js";
import {AudioManager} from "../services/AudioManager.js";
import {CalculatorModal} from "../components/AdminCalculatorModal.js";
import {CorrectAnswerModalForAdmin} from "../components/AnswerModals.js";
import {EditorSidebar} from "../components/EditorSidebar.js";
import {QuestionService} from "../domain/QuestionService.js";

export class AdminGameView extends Component {
    constructor(container, data) {
        super(container, data);
        const teams = JSON.parse(sessionStorage.getItem('teams') || '[]');

        this.gameSettings = new GameSettings();

        this.state = {
            activeCell: null,
            buzzedTeam: null,
            disabledTeams: [],
            teamAnswer: null,
            showCalculatorModal: false,
            players: Object.fromEntries(teams.map(team => [team, 0])),
            playedCells: [], // TODO это нужно подгружать с сервера
            currentAnswer: null,
            isShowingAnswer: false,
        };
    }

    _applyRoomInfo(roomInfo) {
        this.gameSettings.init(roomInfo);
    }

    async mount() {
        const authData = await authenticationToGame(this.container, this.data.roomCode);
        if (!authData) {
            return;
        }

        const [_, roomInfo] = authData;
        this._connectSocket();
        this._applyRoomInfo(roomInfo);

        this.questionService = new QuestionService(
            this.state,
            this.gameSettings,
            this.ws,
            () => this.updateDOM()
        );

        this.updateDOM();
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>
    
            <div class="editor-layout organizer-mode">
                
                ${EditorSidebar(this.state, this.gameSettings)} 
               
                <main class="editor-main">
                    <div class="table-header-actions">
                        <div class="badge">Код комнаты: ${this.data.roomCode}</div>
                        <div class="badge">Режим организатора</div>
                    </div>
                    ${CorrectAnswerModalForAdmin(this.state.currentAnswer)}
                    ${OnGameTable(this.gameSettings, this.state, this._tableOptions)}
                    ${OnGameRating(this.state.players)}
                </main>
               
            
            ${CalculatorModal(this.state, this.gameSettings)}
        `;
    }

    attachEvents() {
        const valueEl = this.container.querySelector('#points-value');

        this.container
            .querySelectorAll('.organizer-track-btn')
            .forEach(btn => btn.addEventListener('click', this.questionService.startSong.bind(this)));

        const awardBtn = this.container.querySelector('#award-btn');
        if (awardBtn) {
            awardBtn.addEventListener('click', async () => await
                this.questionService.processTeamAnswer(true, +valueEl.value));
        }

        const wrongBtn = this.container.querySelector('#wrong-btn');
        if (wrongBtn) {
            wrongBtn.addEventListener('click', async () => await
                this.questionService.processTeamAnswer(false, -valueEl.value));
        }

        const endQuestionBtn = this.container.querySelector('#end-question-btn');
        if (endQuestionBtn) {
            endQuestionBtn.addEventListener('click', async () => await
                this.questionService.stop())
        }

        const endBtn = this.container.querySelector('#end-btn');
        if (endBtn) {
            endBtn.addEventListener('click', async () => {
                await endGame(this.data.roomCode);
                sessionStorage.removeItem('teams');
                new Promise(r => setTimeout(r, 200));
                redirectTo('/account');
            });
        }

        if (valueEl) {
            valueEl.addEventListener('input', () => {
                const v = +valueEl.value || 0;
                awardBtn.textContent = `+${v}`;
                wrongBtn.textContent = `−${v}`;
            });
        }

        const musicBtn = this.container.querySelector('#music-btn');
        if (musicBtn) {
            musicBtn.addEventListener('click', () => {
                if (!AudioManager.currentAudio) return;
                if (AudioManager.currentAudio.paused) {
                    AudioManager.play();
                    musicBtn.textContent = '❚❚';
                } else {
                    AudioManager.pause();
                    musicBtn.textContent = '▶';
                }
            })
        }

        const clearAudioBtn = this.container.querySelector('#clear-audio-btn');
        if (clearAudioBtn) {
            clearAudioBtn.addEventListener('click', () => {
                AudioManager.stop();
                this.state.isShowingAnswer = false;
                this.updateDOM();
            })
        }
    }

    _connectSocket() {
        if (this.ws) return;

        const protocol = window.location.protocol === 'https:' ? 'wss' : 'ws';

        this.ws = new WebSocket(
            `${protocol}://${window.location.host}/api/ws/room/${this.data.roomCode}`
        );

        this.ws.onmessage = (event) => {
            const data = JSON.parse(event.data);

            if (data.type === 'player_buzzed') {
                this.state.buzzedTeam = data.team;
                AudioManager.pause();
                this.updateDOM();
            }

            if (data.type === 'team_answer') {
                this.state.teamAnswer = {
                    artist: data.artist,
                    title: data.title,
                };
                this.updateDOM();
            }

            if (data.type === 'add_points') {
                const team = data.team;
                const points = data.points;
                this.state.players[team] += points;
                this.updateDOM();
            }
        };
    }

    _tableOptions = {
        showFilledState: false,
        showActiveState: false,
        useLaunchText: true
    }
}