export const CorrectAnswerModal = (currentAnswer) => {
    if (!currentAnswer) return '';
    return `
        <div class="organizer-hint">
            <div>🎵 ${currentAnswer.title}</div>
            <div>🎤 ${currentAnswer.artist}</div>
        </div>
    `;
}