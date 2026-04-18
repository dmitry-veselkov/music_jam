import {Component} from "../core/Component.js";
import {Logo, Input, Button} from "../components/UI.js";
import {redirectTo} from "../services/RouteServices.js";
import {registerNewUser} from "../services/AccountServices.js";

export class RegisterView extends Component {
    mount() {
        if (window.currentUser) {
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
        const nameInput = document.querySelector("#reg-name");
        const emailInput = document.querySelector("#reg-email");
        const passwordInput = document.querySelector("#reg-password");
        const registerButton = document.querySelector("#reg-btn");

        if (registerButton) {
            registerButton.addEventListener("click", async () =>
                await this._register(nameInput.value, emailInput.value, passwordInput.value));
        }
    }

    async _register(name, email, password) {
        try {
            window.currentUser = await registerNewUser(name, email, password);
            redirectTo('/account');
        } catch (e) {
            alert(e.message);
        }
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