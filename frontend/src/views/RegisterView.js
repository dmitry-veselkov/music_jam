import {Component} from "../core/Component.js";
import {Logo, Input, Button} from "../components/UI.js";
import {redirectTo} from "../services/RouteServices.js";
import {registerNewUser} from "../services/AccountServices.js";
import {WrongText} from "../components/Error.js";
import {ButtonLoader} from "../components/ButtonLoader.js";

export class RegisterView extends Component {
    mount() {
        /**
         * Страница регистрации. Пользователь указывает ФИ, почту и пароль.
         * В рамках сервиса почта должна быть уникальна. Все поля должны быть заполнены
         */

        // TODO в будущем можно настроить редирект уже залогиненного пользователя в ЛК
        this.container.innerHTML = this.render();

        this.nameInput = document.querySelector("#reg-name");
        this.emailInput = document.querySelector("#reg-email");
        this.passwordInput = document.querySelector("#reg-password");
        this.registerButton = document.querySelector("#reg-btn");

        this.nameInput.focus();

        this.wrongText = new WrongText(this.container);
        this.wrongText.hideWrongText();

        this._addEventListeners();
    }

    render() {
        return `
            <div class="page-layout">
                <div class="header-top">${Logo()}</div>
                <main class="auth-container d-flex-center">
                    <div class="card auth-card">
                        <h1 class="text-center">Регистрация</h1>
                        
                        <div class="form-group mt-3">
                            <label>Имя Фалимия</label>
                            ${Input(this._nameInputSettings)}
                        </div>

                        <div class="form-group mt-3">
                            <label>Email</label>
                            ${Input(this._emailInputSettings)}
                        </div>
                        
                        <div class="form-group mt-3">
                            <label>Пароль</label>
                            ${Input(this._passwordInputSettings)}
                        </div>
                        
                        <div class="btn-wrapper mt-3">
                            ${Button(this._registerButtonSettings)}
                        </div>
                        
                        ${WrongText.html}
                        
                        <p class="text-center text-muted mt-3">
                            Уже есть аккаунт? 
                            <a href="/login" class="text-accent" data-link>Войти</a>
                        </p>
                    </div>
                </main>
            </div>
        `;
    }

    _addEventListeners() {
        /**
         * Навешиваем обработчик на кнопку регистрации и добавляем переходы по enter
         */
        this.registerButton.addEventListener("click", async () => {
            await this._onLoginClickCallback();
        });

        this.nameInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                this.emailInput.focus();
            }
        });

        this.emailInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                this.passwordInput.focus();
            }
        });

        this.passwordInput.addEventListener('keypress', async (e) => {
            if (e.key === 'Enter') {
                await this._onRegisterClickCallback();
            }
        });
    }

    async _onRegisterClickCallback() {
        this.wrongText.hideWrongText();
        await ButtonLoader.wrap(this.registerButton, async () => {
            await this._register(this.nameInput.value, this.emailInput.value, this.passwordInput.value);
        })
    }

    async _register(name, email, password) {
        /**
         * Логика регистрации. Пока без особой валидации, просто смотрим, что данные не пустые.
         * Делаем запрос на сервер, в зависимости от его ответа выдаем сообщение об ошибке,
         * либо перекидываем в ЛК
         */
        if (!name || !email || !password) {
            this.wrongText.showWrongText('Заполните все поля формы!');
            return;
        }

        const user = await registerNewUser(name, email, password);
        if (user) {
            redirectTo('/account');
            return;
        }

        this.wrongText.showWrongText('Email уже зарегистрирован!');
    }

    _nameInputSettings = {
        id: "reg-name",
        placeholder: "Иван Иванов",
        maxLength: 250,
    };

    _emailInputSettings = {
        id: "reg-email",
        placeholder: "mail@example.com",
        type: "email",
        maxLength: 150,
    };

    _passwordInputSettings = {
        id: "reg-password",
        placeholder: "••••••••",
        type: "password",
        maxLength: 100,
    };

    _registerButtonSettings = {
        id: "reg-btn",
        text: "Создать аккаунт",
        variant: "primary"
    };
}