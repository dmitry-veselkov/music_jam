import { Component } from "../core/Component.js";
import {Logo, GameTable} from "../components/UI.js";


export class GamesView extends Component {
    render() {
        const categories = ['Русский рок', 'Зарубежный хип-хоп', 'Саундтреки', 'Классика'];
        const costs = [100, 200, 300, 400, 500];

        return `
            <div class="games-page">
                <div class="logo-corner">${Logo()}</div>
                <main class="games-container">
                    <h1 class="games-title">Выберите вопрос</h1>
                    ${GameTable(categories, costs)}
                </main>
            </div>
        `;
    }

    mount() {
        this.container.innerHTML = this.render();

        const table = this.container.querySelector('.game-table');
        if (table) {
            table.addEventListener('click', (e) => {
                const btn = e.target.closest('.game-table__btn');
                if (btn) {
                    const cell = btn.parentElement;
                    const category = cell.dataset.category;
                    const cost = cell.dataset.cost;

                    this.handleQuestionSelect(category, cost, btn);
                }
            });
        }
    }

    handleQuestionSelect(category, cost, button) {
        console.log(`Выбрана категория "${category}" за ${cost}`);

        button.disabled = true;
        button.classList.add('game-table__btn--used');

        // Тут можно вызвать navigate('/question', { category, cost })
    }
}