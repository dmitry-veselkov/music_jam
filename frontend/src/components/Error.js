export class WrongText {
    /**
     * Класс для вывода сообщения об ошибке
     */

    static html = `<p class="wrong-text"></p>`;

    constructor(container) {
        this.container = container;
        this.wrongText = this.container.querySelector('.wrong-text');
    }

    showWrongText(msg) {
        /**
         * Отображаем текст об ошибке
         */
        this.wrongText.style.display = 'block';
        this.wrongText.textContent = msg;
    }

    hideWrongText() {
        /**
         * Скрываем текст об ошибке
         */
        this.wrongText.style.display = 'none';
    }
}