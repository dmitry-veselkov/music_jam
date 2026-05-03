import {Button} from "./UI.js";

export const CorrectAnswerModalForAdmin = (currentAnswer) => {
    if (!currentAnswer) return '';
    return `
        <div class="organizer-hint">
            <div>🎵 ${currentAnswer.title}</div>
            <div>🎤 ${currentAnswer.artist}</div>
        </div>
    `;
}

export const CorrectAnswerModalForPlayer = (answer) => {
    if (!answer) return '';

    return `
        <div class="modal-overlay">
            <div class="card modal-card answer-card">
                <button class="modal-close" id="close-answer-btn">×</button>
                <div class="answer-icon">🎵</div>
                <div class="answer-label">Правильный ответ</div>
                <p class="answer-title">${answer.title}</p>
                <p class="answer-artist">${answer.artist}</p>
            </div>
        </div>
    `;
}

export const IncorrectAnswerModal = (hadWrongAnswer) => {
    if (!hadWrongAnswer) return '';
    return `
        <div class="modal-overlay answer-fade">
            <div class="card modal-card answer-card">
                <div class="answer-icon">🎵</div>
                <div class="answer-label">Вы дали неправильный ответ!</div>
            </div>
        </div>
    `;
}

export const AnswerInput = (showAnswerInput) => {
    if (!showAnswerInput) return '';

    return `
            <div class="modal-overlay">
                <div class="card modal-card">
                    <h3 class="card-title">Ваш ответ</h3>
                        <div class="form-group">
                            <label>Исполнитель</label>
                            <input class="ui-input" id="answer-artist" placeholder="Введите исполнителя" />
                        </div>
                        <div class="form-group">
                            <label>Название трека</label>
                            <input class="ui-input" id="answer-title" placeholder="Введите название" />
                        </div>
                        <div class="btn-group-vertical">
                            ${Button(sendAnswerBtnSettings)}
                        </div>
                </div>
            </div>
    `;
}

const sendAnswerBtnSettings = {
    text: 'Отправить',
    id: 'submit-answer-btn',
    extraClass: 'w-100'
}