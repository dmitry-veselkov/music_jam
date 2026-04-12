import {Component} from "../core/Component.js";
import {Logo, Input, Button} from "../components/UI.js";
import {checkUserExists} from "../services/AccountServices.js";
import {redirectTo} from "../services/RouteServices.js";

export class LoginView extends Component {
    mount() {
        if (window.currentUser)
        {
            redirectTo('/account');
            return;
        }

        this.container.innerHTML = this.render();
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
        const loginButton = document.querySelector("#login-btn");
        const emailInput = document.querySelector("#login-email");
        const passwordInput = document.querySelector("#login-password");


        if (loginButton) {
            loginButton.addEventListener("click", async () =>
                await this._login(emailInput.value, passwordInput.value));
        }
    }

    async _login(email, password) {
        try {
            await checkUserExists(email, password)
            redirectTo('/account')
        } catch (e) {
            // TODO Заменить алерт на что-то красивое
            alert('Неверный логин или пароль!')
        }
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