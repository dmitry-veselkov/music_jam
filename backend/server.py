from flask import Flask, send_from_directory, jsonify, request
import os

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
FRONTEND_DIR = os.path.abspath(os.path.join(BASE_DIR, '..', 'frontend'))

app = Flask(__name__, static_folder=FRONTEND_DIR)

teams = {
    "fake-uuid-1": "БобрДобр",
    "fake-uuid-2": "Недавно гипербола"
}

users_db = {}

@app.route('/api/check_room', methods=['GET'])
def get_room_info():
    code = request.args.get('roomCode').upper()

    # TODO Переделать MOCK
    exists = code == "12345"
    room_info = {
        "status": "waiting",
        "gameName": "Хиты 80-х",
        "creator": "Очень добрый человек",
        "teams": list(teams.values())
    } if exists else None

    return jsonify({"roomCode": code, "exists": exists, "roomInfo": room_info})


@app.route('/api/set_team_name', methods=['POST'])
def set_team_name():
    data = request.get_json()
    team_id = data.get('teamId')

    is_new_team = team_id not in teams
    old_name = teams.get(team_id)

    teams[data.get('teamId')] = data.get('newName')
    return jsonify({
        "status": "ok",
        "isNewTeam": is_new_team,
        "oldName": old_name,
        "newName": data.get('newName')
    })


@app.route('/api/get_team_name', methods=['POST'])
def get_team_name():
    data = request.get_json()
    team_name = teams.get(data.get('uuid'))
    return jsonify({"teamName": team_name})


@app.route('/', defaults={'path': ''})
@app.route('/<path:path>')
def catch_all(path):
    file_path = os.path.join(app.static_folder, path)

    if path != "" and os.path.exists(file_path):
        return send_from_directory(app.static_folder, path)
    return send_from_directory(app.static_folder, 'index.html')

# @app.route('/login', methods=['POST'])
# def login():
#     data = request.get_json()
#     username = data.get('name')
#     password = data.get('password')
#     user = users_db.get(username)
#     if not user:
#         return jsonify({"error": "Пользователь не найден"}), 404
#     if user["password"] != password:
#         return jsonify({"error": "Неверный пароль"}), 401
#     return jsonify({
#         "message": "Успешный вход",
#         "user": {
#             "name": username
#         }
#     })
#
# @app.route('/register', methods=['POST'])
# def register():
#     data = request.get_json()
#     username = data.get('name')
#     password = data.get('password')
#     if username in users_db:
#         return jsonify({"error": "Пользователь уже существует"}), 400
#     users_db[username] = {
#         "password": password
#     }
#     return jsonify({
#         "message": "Регистрация успешна",
#         "user": {
#             "name": username
#         }
#     })

if __name__ == '__main__':
    app.run(debug=True, port=5000)
