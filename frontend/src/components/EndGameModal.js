import {Button} from "./UI.js";

export const ConfirmEndGameModal = (show) => {
    if (!show) return '';
    return `
        <div class="modal-overlay" id="confirm-end-overlay">
            <div class="card modal-card answer-card">
                <div class="answer-icon">⚠️</div>
                <div class="answer-label">Завершить игру?</div>
                <p style="color:var(--color-text-muted);font-size:0.9rem;margin:0 0 1.5rem;">Это действие нельзя отменить</p>
                <div class="btn-group-horizontal" style="justify-content:center;">
                    ${Button({text: 'Отмена', id: 'cancel-end-btn'})}
                    ${Button({text: 'Завершить', id: 'confirm-end-btn', extraClass: 'btn-danger'})}
                </div>
            </div>
        </div>
    `;
}