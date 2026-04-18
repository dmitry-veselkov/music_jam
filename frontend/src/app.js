import {EventBus} from "./core/EventBus.js";
import {Router} from "./core/Router.js";

import {StartView} from "./views/StartView.js";
import {LobbyView} from "./views/LobbyView.js";
import {GameSettingsView} from "./views/GameSettingsView.js";
import {WaitingRoomView} from "./views/WaitingRoomView.js";
import {LoginView} from "./views/LoginView.js";
import {RegisterView} from "./views/RegisterView.js";
import {AccountView} from "./views/AccountView.js";
import {GameView} from "./views/GameView.js";
import {HostAdminView} from "./views/HostAdminView.js";

const bus = new EventBus();
const appContainer = document.getElementById('app');
const routes = {
    '/': StartView,
    '/lobby': LobbyView,
    '/room/game_settings': GameSettingsView,
    '/room/waiting': WaitingRoomView,
    '/login': LoginView,
    '/register': RegisterView,
    '/account': AccountView,
    '/room/active' : GameView,
    '/room/admin_panel': HostAdminView,
};

const router = new Router(routes, appContainer);