export class ButtonLoader {
    /**
     * Специальный класс, реализующий логику отображения спинера загрузки внутри кнопки,
     * которая вызвала долгий метод, например, обращающийся к БД. Принимает метод как колбэк
     */
    static start(button) {
        if (!button || button.classList.contains('loading')) return;

        button.classList.add('loading');
        button.disabled = true;
    }

    static stop(button) {
        if (!button) return;

        button.classList.remove('loading');
        button.disabled = false;
    }

    static async wrap(button, callback) {
        if (!button || button.classList.contains('loading')) return;

        this.start(button);

        try {
            await callback();
        } finally {
            this.stop(button);
        }
    }
}