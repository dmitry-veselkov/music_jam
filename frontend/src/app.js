import {EventBus} from "./core/EventBus.js";
import {Router} from "./core/Router.js";

import {StartView} from "./views/StartView.js";
import {LobbyView} from "./views/LobbyView.js";
import {GameSettingsView} from "./views/GameSettingsView.js";
import {WaitingRoomView} from "./views/WaitingRoomView.js";
import {LoginView} from "./views/LoginView.js";
import {RegisterView} from "./views/RegisterView.js";

const bus = new EventBus();
const appContainer = document.getElementById('app');
const routes = {
    '/': StartView,
    '/lobby': LobbyView,
    '/room/game_settings': GameSettingsView,
    '/room/waiting': WaitingRoomView,
    '/login': LoginView,
    '/register': RegisterView,
};

const router = new Router(routes, appContainer);