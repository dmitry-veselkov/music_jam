# music_jam
MusicJam version 2.0

## Локальный запуск для разработки

0. Один раз в жизни запустить в PowerShell от имени админа `dotnet dev-certs https --trust`, чтобы заработал HTTPS
1. Перейти на нужную ветку репозитория
3. Из корня репозитория запустить `.\Backend\MusicJam.ApiGateway\.local_start.cmd` (если из-за сетевых ошибок падает `npm install` включить/отключить vpn)
4. По завершении скрипта автоматически откроется https://localhost:5000 со стартовой страницей сайта