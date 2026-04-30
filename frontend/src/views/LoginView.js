import {Component} from "../core/Component.js";
import {Logo, Input, Button} from "../components/UI.js";
import {checkUserExists} from "../services/AccountServices.js";
import {redirectTo} from "../services/RouteServices.js";
import {WrongText} from "../components/Error.js";
import {ButtonLoader} from "../components/ButtonLoader.js";

export class LoginView extends Component {
    mount() {
        /**
         * Страница авторизации. Пользователь указывает почту и пароль.
         * В рамках сервиса почта должна быть уникальна.
         */

        // TODO в будущем можно настроить редирект уже залогиненного пользователя в ЛК
        this.container.innerHTML = this.render();

        this.loginButton = document.querySelector("#login-btn");
        this.emailInput = document.querySelector("#login-email");
        this.passwordInput = document.querySelector("#login-password");

        this.emailInput.focus();

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
                        <h1 class="text-center">Вход в систему</h1>
                        <div class="form-group mt-3">
                            <label>Email</label>
                            ${Input(this._emailInputSettings)}
                        </div>
                        <div class="form-group mt-3">
                            <label>Пароль</label>
                            ${Input(this._passwordInputSettings)}
                        </div>
                        <div class="btn-wrapper mt-3">
                            ${Button(this._loginButtonSettings)}
                        </div>
                        ${WrongText.html}
                        <p class="text-center text-muted mt-3">
                            Нет аккаунта? 
                            <a href="/register" class="text-accent" data-link>Зарегистрироваться</a>
                        </p>
                    </div>
                </main>
            </div>
        `;
    }

    _addEventListeners() {
        /**
         * Добавление обработчика на кнопку и удобных переключений между инпутами по enter
         */
        this.loginButton.addEventListener("click", async () => {
            await this._onLoginClickCallback();
        });

        this.emailInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                this.passwordInput.focus();
            }
        });

        this.passwordInput.addEventListener('keypress', async (e) => {
            if (e.key === 'Enter') {
                await this._onLoginClickCallback();
            }
        });
    }

    async _onLoginClickCallback() {
        this.wrongText.hideWrongText();
        await ButtonLoader.wrap(this.loginButton, async () => {
            await this._login(this.emailInput.value, this.passwordInput.value);
        });
    }

    async _login(email, password) {
        /**
         * Собственно, сам процесс авторизации
         */
        if (!email || !password) {
            this.wrongText.showWrongText('Заполните все поля формы!');
            return;
        }

        const user = await checkUserExists(email, password);
        if (user) {
            redirectTo('/account');
            return;
        }

        this.wrongText.showWrongText('Неверный логин или пароль!');
    }

    _emailInputSettings = {
        id: "login-email",
        placeholder: "mail@example.com",
        maxLength: 150,
        type: "email"
    };

    _passwordInputSettings = {
        id: "login-password",
        placeholder: "••••••••",
        maxLength: 100,
        type: "password"
    };

    _loginButtonSettings = {
        id: "login-btn",
        text: "Войти",
        variant: "primary"
    }
}