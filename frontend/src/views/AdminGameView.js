import {Component} from "../core/Component.js";
import {addPlayedTrack, endGame, getRoomState} from "../services/GamesServices.js";
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
import {ConfirmEndGameModal} from "../components/EndGameModal.js";

export class AdminGameView extends Component {
    constructor(container, data) {
        super(container, data);
        this.gameSettings = new GameSettings();

        this.state = {
            activeCell: null,
            buzzedTeam: null,
            disabledTeams: [],
            teamAnswer: null,
            showCalculatorModal: false,
            players: null,
            playedCells: [],
            currentAnswer: null,
            isShowingAnswer: false,
            showEndConfirm : false
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


        const roomState = await getRoomState(this.data.roomCode);
        console.log(roomState);
        if (roomState) {
            this.state.players = roomState.teams;
            this.state.playedCells = roomState.played_tracks;
        }

        this.questionService = new QuestionService(
            this.state,
            this.gameSettings,
            this.ws,
            () => this.updateDOM(),
            this.data.roomCode
        );
        this.updateDOM();
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        console.log('render currentAnswer:', this.state.currentAnswer);
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
            ${ConfirmEndGameModal(this.state.showEndConfirm)}
            </div>
        `;
    }

    attachEvents() {
        const valueEl = this.container.querySelector('#points-value');

        this.container
            .querySelectorAll('.organizer-track-btn')
            .forEach(btn => btn.addEventListener('click', this.questionService.startSong.bind(this)));

        const awardBtn = this.container.querySelector('#award-btn');
        if (awardBtn) {
            awardBtn.addEventListener('click', async () => {
                const cell = this.state.activeCell;
                const row = cell?.row;
                const col = cell?.col;
                await this.questionService.processTeamAnswer(true, +valueEl.value);
                await addPlayedTrack(row, col, this.data.roomCode);
            })
        }

        const wrongBtn = this.container.querySelector('#wrong-btn');
        if (wrongBtn) {
            wrongBtn.addEventListener('click', async () => await
                this.questionService.processTeamAnswer(false, -valueEl.value));
        }

        const endQuestionBtn = this.container.querySelector('#end-question-btn');
        if (endQuestionBtn) {
            endQuestionBtn.addEventListener('click', async () => {
                const cell = this.state.activeCell;
                const row = cell?.row;
                const col = cell?.col;
                await this.questionService.stop();
                await addPlayedTrack(row, col, this.data.roomCode);
            });
        }

        const endBtn = this.container.querySelector('#end-btn');
        if (endBtn) {
            endBtn.addEventListener('click', () => {
                this.state.showEndConfirm = true;
                this.updateDOM();
            });
        }

        const confirmEndBtn = this.container.querySelector('#confirm-end-btn');
        if (confirmEndBtn) {
            confirmEndBtn.addEventListener('click', async () => {
                sessionStorage.setItem('final-scores', JSON.stringify(this.state.players));
                await endGame(this.data.roomCode);
                redirectTo(`/room/finish/${this.data.roomCode}`);
            });
        }

        const cancelEndBtn = this.container.querySelector('#cancel-end-btn');
        if (cancelEndBtn) {
            cancelEndBtn.addEventListener('click', () => {
                this.state.showEndConfirm = false;
                this.updateDOM();
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