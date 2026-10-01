@echo off
cd /d "%~dp0"

pushd ..\..\frontend
call npm install
call npm run build
popd
:: todo здесь должна быть логика по сборке фронтенда

start dotnet run -c=Release

timeout 3

start https://localhost.musicjam.fun:5555
