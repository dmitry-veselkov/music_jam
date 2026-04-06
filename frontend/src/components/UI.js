export const Button = ({text = '', id = '', variant = 'primary', extraClass = '', icon = '', textClass = 'text'}) => {
    return `
        <button id="${id}" class="btn btn-${variant} ${extraClass}">
            <span class="${textClass}">${text}</span>
            ${icon ? `<img src="${icon}"/>` : ''}
        </button>
    `;
};

export const Input = ({id, placeholder, maxLength = 10, type = 'text'}) => {
    return `
        <input 
            type="${type}" 
            id="${id}" 
            class="ui-input" 
            placeholder="${placeholder}" 
            maxlength="${maxLength}" 
            autocomplete="off"
        >
    `;
};

export const Logo = (extraClass = '') => `
    <div class="logo-text ${extraClass}">MusicJam</div>
`;

export const Header = () => `
    <header class="start-header">
        ${Logo()}
        <div class="auth-buttons">
            ${Button({text: 'регистрация/войти→', id: 'login-btn', variant: 'register', icon: '/icons/user-icon.svg', textClass: 'register-text'})}
        </div>
    </header>
`;

export const Footer = () => `
    <footer class="start-footer">
        <div class="creators">
            <p>Разработано командой: <strong>Бой с Панасовой</strong> © 2026</p>
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