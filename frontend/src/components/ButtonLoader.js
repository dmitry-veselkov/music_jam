export class LoadingButton {
    /**
     * Класс для управления кнопкой с лоудером
     */

    static html = `
        <button class="loading-btn">
            <span class="btn-text"></span>
            <span class="btn-spinner"></span>
        </button>
    `;

    constructor(container) {
        this.container = container;
        this.button = this.container.querySelector('.loading-btn');
        this.textEl = this.container.querySelector('.btn-text');
        this.spinner = this.container.querySelector('.btn-spinner');

        this.isLoading = false;
    }

    setText(text) {
        this.textEl.textContent = text;
    }

    startLoading() {
        /**
         * Включаем состояние загрузки
         */
        if (this.isLoading) return;

        this.isLoading = true;

        this.button.classList.add('loading');
        this.button.disabled = true;
    }

    stopLoading() {
        /**
         * Выключаем состояние загрузки
         */
        this.isLoading = false;

        this.button.classList.remove('loading');
        this.button.disabled = false;
    }
}