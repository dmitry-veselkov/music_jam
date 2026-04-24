export const AddMusicModal = () => {
    return `
            <div id="track-modal" class="modal hidden">
                <div class="modal-backdrop"></div>
    
                <div class="modal-dialog">
                    <button class="modal-close" id="close-modal" aria-label="Закрыть">×</button>
    
                    <div class="modal-header">
                        <div class="modal-badge">🎵 Трек</div>
                        <h3 class="modal-title">Добавить трек</h3>
                    </div>
    
                    <div class="modal-body">
                        <label for="track-title-input" class="modal-label">Название трека</label>
                        <input
                            type="text"
                            id="track-title-input"
                            class="modal-input"
                            placeholder="Введите название трека">
    
                            <label for="track-artist-input" class="modal-label">Исполнитель</label>
                            <input
                                type="text"
                                id="track-artist-input"
                                class="modal-input"
                                placeholder="Введите исполнителя">
    
                                <label for="question-track-input" class="modal-label">Аудио для вопроса</label>
                                <input
                                    type="file"
                                    id="question-track-input"
                                    class="modal-input">
    
                                    <label for="answer-track-input" class="modal-label">Ответ</label>
                                    <input
                                        type="file"
                                        id="answer-track-input"
                                        class="modal-input">
                    </div>
    
                    <div class="modal-actions">
                        <button class="btn btn-secondary" id="cancel-track">Отмена</button>
                        <button class="btn btn-primary" id="save-track">Сохранить</button>
                    </div>
                </div>
            </div>
    `;
}