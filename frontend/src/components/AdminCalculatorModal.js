import {Button} from "./UI.js";

export const CalculatorModal = (state, gameSettings) => {
    const [activeCell, buzzedTeam, teamAnswer] = [state.activeCell, state.buzzedTeam, state.teamAnswer];
    if (!buzzedTeam) return '';

    const defaultPoints = activeCell
        ? gameSettings.costs[activeCell.col]
        : 0;


    return `
        <div id="calculator-modal" class="modal">
            <div class="modal-backdrop"></div>

            <div class="modal-dialog">
                <button class="modal-close" id="close-calculator-modal" aria-label="Закрыть">×</button>

                <div class="modal-header">
                    <div class="modal-badge">🧮 Калькулятор</div>
                    <h3 class="modal-title">Отвечает: <strong>${buzzedTeam}</strong></h3>
                </div>

                <div class="modal-body">
                    ${teamAnswer ? `
                        <div class="form-group">
                            <p>🎤 <strong>${teamAnswer.artist}</strong></p>
                            <p>🎵 <strong>${teamAnswer.title}</strong></p>
                        </div>
                    ` : '<p class="muted">Ожидание ответа команды...</p>'}

                    <div class="points-stepper">
                        <input
                            class="stepper-value"
                            id="points-value"
                            type="number"
                            value="${defaultPoints}"
                            min="0"
                            placeholder="${defaultPoints}">
                    </div>
                </div>

                <div class="modal-actions">
                    ${Button(awardButtonSettings)}
                    ${Button(wrongButtonSetting)}
                </div>
            </div>
        </div>
    `;
}

const awardButtonSettings = {
    text: '+',
    id: 'award-btn',
    extraClass: 'w-100'
}

const wrongButtonSetting = {
    text: '-',
    id: 'wrong-btn',
    variant: 'outline',
    extraClass: 'w-100'
}