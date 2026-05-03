import {Component} from "../core/Component.js";
import {Logo} from "../components/UI.js";
import {saveGameSettings} from "../services/GamesServices.js";
import {authenticationToGame} from "../services/AccountServices.js";
import {GameSettings} from "../domain/GameSettings.js";
import {Song} from "../domain/Song.js";
import {ButtonLoader} from "../components/ButtonLoader.js";
import {AsideSettings, MainEditor, Toast, ModeExplanationModal} from "../components/GameSettingsPanels.js";
import {AddMusicModal} from "../components/AddMusicModal.js";

export class GameSettingsView extends Component {
    constructor(container, data) {
        super(container, data);
        this.gameSettings = new GameSettings();
        this.currentCell = null;
    }

    async mount() {
        const authData = await authenticationToGame(this.container, this.data.roomCode);
        if (!authData) {
            return;
        }

        const [userInfo, roomInfo] = authData;
        this.gameSettings.init(roomInfo, userInfo);
        this.updateDOM();
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        return `
                <div class="logo-corner">${Logo()}</div>
                <div class="editor-layout">
                    ${AsideSettings(this.gameSettings)}
                    ${MainEditor(this.gameSettings, this.data.roomCode)}
                </div>
                ${AddMusicModal(this.currentCell)}
                ${Toast()}
                ${ModeExplanationModal()}
        `;
    }

    attachEvents() {
        this.container.querySelectorAll('.sync-input').forEach(input => {
            input.addEventListener('input', (e) => {
                const key = e.target.dataset.key;
                this.gameSettings[key] = e.target.value;
            });
        });

        this.container.querySelector('#add-row').onclick = () => {
            const newCategory = `Категория ${this.gameSettings.categories.length + 1}`;
            this.gameSettings.addRow(newCategory);
            this.updateDOM();
        };

        this.container.querySelector('#add-col').onclick = () => {
            const last = this.gameSettings.costs.at(-1) || 0;
            const newCost = last + 100;
            this.gameSettings.addCol(newCost);
            this.updateDOM();
        };

        this.container.querySelectorAll('.remove-row').forEach(btn => {
            btn.onclick = () => {
                const idx = +btn.dataset.idx;
                this.gameSettings.removeRow(idx);
                this.updateDOM();
            };
        });

        this.container.querySelectorAll('.remove-col').forEach(btn => {
            btn.onclick = () => {
                const idx = +btn.dataset.idx;
                this.gameSettings.removeCol(idx);
                this.updateDOM();
            };
        });

        this.container.querySelectorAll('.cost-edit').forEach(input => {
            input.onchange = (e) => {
                const idx = +e.target.dataset.idx;
                this.gameSettings.costs[idx] = Number(e.target.value);
                this.updateDOM();
            };
        });

        this.container.querySelectorAll('.cat-edit').forEach(input => {
            input.oninput = (e) => {
                const idx = +e.target.dataset.idx;
                this.gameSettings.categories[idx] = e.target.value;
            };
        });

        this.container.querySelectorAll('.preview-cell').forEach(cell => {
            cell.onclick = (e) => {
                this.currentCell = {
                    row: +e.currentTarget.dataset.row,
                    col: +e.currentTarget.dataset.col,
                    song: this.gameSettings.cells[e.currentTarget.dataset.row][e.currentTarget.dataset.col].song,
                };
                this.updateDOM();
                this.openTrackModal();
            };
        });

        const cancelBtn = this.container.querySelector('#cancel-track');
        if (cancelBtn) {
            cancelBtn.onclick = () => this.closeTrackModal();
        }

        const closeBtn = this.container.querySelector('#close-modal');
        if (closeBtn) {
            closeBtn.onclick = () => this.closeTrackModal();
        }

        const saveTrackBtn = this.container.querySelector('#save-track');
        if (saveTrackBtn) {
            saveTrackBtn.onclick = async () => {
                await ButtonLoader.wrap(saveTrackBtn, async () => {
                    await this.uploadSong();
                });
            };
        }

        const saveChangesBtn = this.container.querySelector('#save-changes');
        if (saveChangesBtn) {
            saveChangesBtn.onclick = async () => {
                await ButtonLoader.wrap(saveChangesBtn, async () => {
                    await saveGameSettings(this.getPayload());
                    this.updateDOM();
                    this.showToast();
                });
            }
        }

        this.container.querySelectorAll('input[name="game-mode"]').forEach(radio => {
            radio.onchange = (e) => {
                this.gameSettings.mode = e.target.value === 'voice';
            };
        });

        const closeModeBtn = this.container.querySelector('#close-mode-explanation');
        if (closeModeBtn)
        {
            closeModeBtn.onclick = () => {
                const modal = this.container.querySelector('#mode-explanation-overlay');
                modal.classList.add('hidden');
            }
        }

        const openModeBtn = this.container.querySelector('#open-mode-explanation');
        if (openModeBtn)
        {
            openModeBtn.onclick = () => {
                const modal = this.container.querySelector('#mode-explanation-overlay');
                modal.classList.remove('hidden');
            }
        }

        const closeModeBackground = this.container.querySelector('#mode-explanation-overlay');
        if (closeModeBackground)
        {
            closeModeBackground.onclick = (e) => {
                if (e.target.id === 'mode-explanation-overlay')
                {
                    const modal = this.container.querySelector('#mode-explanation-overlay');
                    modal.classList.add('hidden');
                }
            }
        }
    }

    openTrackModal() {
        const modal = this.container.querySelector('#track-modal');
        modal.classList.remove('hidden');
    }

    closeTrackModal() {
        const modal = this.container.querySelector('#track-modal');
        modal.classList.add('hidden');
    }

    getPayload() {
        return {
            roomCode: this.data.roomCode,
            title: this.gameSettings.title,
            mode: this.gameSettings.mode,
            description: this.gameSettings.description,
            categories: this.gameSettings.categories,
            costs: this.gameSettings.costs,

            tracks: this.gameSettings.cells.map(row =>
                row.map(cell => cell.song ? {
                    title: cell.song.title,
                    artist: cell.song.artist,
                    question_url: cell.song.questionUrl,
                    answer_url: cell.song.answerUrl
                } : null)
            )
        };
    }

    async uploadSong() {
        const {row, col} = this.currentCell;
        const cell = this.gameSettings.cells[row][col];

        const title = this.container.querySelector('#track-title-input').value;
        const artist = this.container.querySelector('#track-artist-input').value;
        const question = this.container.querySelector('#question-track-input').files[0];
        const answer = this.container.querySelector('#answer-track-input').files[0];

        const formData = new FormData();
        formData.append("title", title);
        formData.append("artist", artist);
        formData.append("category", this.gameSettings.categories[row]);
        formData.append("cost", cell.cost);

        if (question) formData.append("question", question);
        if (answer) formData.append("answer", answer);
        console.log(cell.song?.answerUrl);
        formData.append("question_url", cell.song?.questionUrl ?? '');
        formData.append("answer_url", cell.song?.answerUrl ?? '');

        const res = await fetch("/api/upload_music", {
            method: "POST",
            body: formData
        });

        const data = await res.json();

        if (data.status !== "ok") {
            alert(data.error);
            return;
        }

        const song = new Song(
            data.song.title,
            data.song.artist,
            data.song.questionUrl,
            data.song.answerUrl
        );

        this.gameSettings.setSong(row, col, song);

        this.closeTrackModal();
        this.updateDOM();
    }

    showToast(text = 'Изменения сохранены') {
        const toast = document.getElementById('save-toast');
        if (!toast) return;

        toast.textContent = `✅ ${text}`;
        toast.classList.add('visible');

        setTimeout(() => toast.classList.remove('visible'), 3000);
    }
}
