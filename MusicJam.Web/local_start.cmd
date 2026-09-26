cd Frontend\

:: todo здесь должна быть логика по сборке фронтенда

cd ..

start dotnet run -c=Release

timeout 3

start https://localhost.musicjam.fun:5555