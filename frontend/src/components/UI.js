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
    <div class="logo-text ${extraClass}">🎵 MusicJam</div>
`;

export const Header = () => `
    <header class="start-header">
        ${Logo()}
        <div class="auth-buttons">
            ${Button({text: 'Вход', id: 'login-btn', variant: 'outline'})}
            ${Button({text: 'Регистрация', id: 'register-btn', variant: 'secondary'})}
        </div>
    </header>
`;

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