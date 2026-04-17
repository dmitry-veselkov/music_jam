export const Button = ({text, id = '', variant = 'primary', extraClass = ''}) => {
    return `
        <button id="${id}" class="btn btn-${variant} ${extraClass}">
            ${text}
        </button>
    `;
};

export const Input = ({
                          id,
                          placeholder,
                          maxLength = 10,
                          type = 'text',
                          value = '',
                          disabled = false
                      }) => {
    return `
        <input 
            type="${type}" 
            id="${id}" 
            class="ui-input" 
            placeholder="${placeholder}" 
            maxlength="${maxLength}" 
            autocomplete="off"
            value="${value}"
            ${disabled ? 'disabled' : ''} 
        >
    `;
};

export const Logo = (extraClass = '') => `
    <div class="logo-text ${extraClass}">
        <a href="/">
            🎵 MusicJam
        </a></div>
`;

export const Header = (isAuth = false) => {
    const loginBtnSettings = {text: 'Вход', id: 'login-btn', variant: 'outline'}
    const registerBtnSettings = {text: 'Регистрация', id: 'register-btn', variant: 'secondary'}

    const enterButtons = `<div class="auth-buttons">
                                    ${Button(loginBtnSettings)}
                                    ${Button(registerBtnSettings)}
                                </div>`

    const account = `
        <a href="/account" class="user-profile-link" data-link>
            <div class="user-info">
                <span class="user-name">Личный кабинет</span>
                <div class="user-avatar">
                    <i class="icon-user"></i> <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                        <circle cx="12" cy="7" r="4"></circle>
                    </svg>
                </div>
            </div>
        </a>`;

    return `<header class="start-header">
                ${Logo()}
                ${isAuth ? account : enterButtons}
            </header>
`};

export const Footer = () => `
    <footer class="start-footer">
        <div class="creators">
            <p>Разработано командой: <strong>MusicJam</strong> © 2026</p>
            <a href="https://github.com/dmitry-veselkov/music_jam" target="_blank" class="github-link">GitHub проекта</a>
        </div>
    </footer>
`;

export const GameTable = (categories = [], costs = []) => {
    const rows = categories.map(category => `
        <tr class="game-table__row">
            <td class="game-table__cell game-table__cell--category">
                ${category}
            </td>
            ${costs.map(cost => `
                <td class="game-table__cell game-table__cell--question" data-cost="${cost}" data-category="${category}">
                    <button class="game-table__btn">${cost}</button>
                </td>
            `).join('')}
        </tr>
    `).join('');

    return `
        <div class="game-table-wrapper">
            <table class="game-table">
                <tbody>
                    ${rows}
                </tbody>
            </table>
        </div>
    `;
};