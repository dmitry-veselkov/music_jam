import {Router} from "./core/Router.js";

import {StartView} from "./views/StartView.js";
import {EnterInviteCodeView} from "./views/EnterInviteCodeView.js";
import {GameSettingsView} from "./views/GameSettingsView.js";
import {WaitingRoomView} from "./views/WaitingRoomView.js";
import {LoginView} from "./views/LoginView.js";
import {RegisterView} from "./views/RegisterView.js";
import {AccountView} from "./views/AccountView.js";
import {GameView} from "./views/GameView.js";
import {AdminWaitingRoomView} from "./views/AdminWaitingRoomView.js";
import {AdminGameView} from "./views/AdminGameView.js";
import {GameEndingViewUniversal} from "./views/GameEndingViewUniversal.js";

const appContainer = document.getElementById('app');
const routes = {
    '/': StartView,
    '/lobby': EnterInviteCodeView,
    '/room/game_settings': GameSettingsView,
    '/room/waiting': WaitingRoomView,
    '/login': LoginView,
    '/register': RegisterView,
    '/account': AccountView,
    '/room/active': GameView,
    '/room/admin_waiting': AdminWaitingRoomView,
    '/room/admin_active': AdminGameView,
    '/room/finish' : GameEndingViewUniversal
};

const router = new Router(routes, appContainer);