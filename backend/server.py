from flask import Flask, send_from_directory, jsonify, request, abort, make_response
import os
import secrets

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
FRONTEND_DIR = os.path.abspath(os.path.join(BASE_DIR, '..', 'frontend'))

app = Flask(__name__, static_folder=FRONTEND_DIR)

teams = {
    "fake-uuid-1": "БобрДобр",
    "fake-uuid-2": "Недавно гипербола"
}

users_db = {
    'pupu@yandex.ru': {'name': "Дмитрий", 'password': '123'},
}

sessions = {

}

games = {
    'pupu@yandex.ru': [
        {'id': "g1", 'title': "Музыкальный Квиз", 'date': "10 апр 2026", 'code': "12345"},
        {'id': "g2", 'title': "Новогодний Свояк", 'date': "25 дек 2025", 'code': "98765"},
    ]
}


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


@app.route('/api/login', methods=['POST'])
def login():
    data = request.get_json()

    email = data.get('email')
    password = data.get('password')

    if email in users_db and users_db[email]['password'] == password:
        token = secrets.token_hex(16)
        sessions[token] = email
        response = make_response()
        response.set_cookie('token', token, httponly=True, samesite='Lax')
        return response, 200
    else:
        return jsonify({"message": "Invalid credentials"}), 400


@app.route('/api/register', methods=['POST'])
def register():
    data = request.get_json()

    name = data.get('name')
    email = data.get('email')
    password = data.get('password')

    if email in users_db:
        return jsonify({"message": "Пользователь уже существует"}), 400

    users_db[email] = {
        'name': name,
        "password": password
    }

    token = secrets.token_hex(16)
    sessions[token] = email

    response = make_response(jsonify({
        "user": {
            "name": name,
            "email": email,
        }
    }))

    response.set_cookie('token', token, httponly=True, samesite='Lax')
    return response, 200


@app.route('/api/get_user_info', methods=['GET'])
def get_user_info():
    token = request.cookies.get('token')

    if not token:
        return jsonify({"message": "Not authenticated"}), 401

    email = sessions.get(token)
    user = users_db.get(email)

    return jsonify({
        "email": email,
        "name": user['name']
    }), 200


@app.route('/api/get_all_user_games', methods=['GET'])
def get_all_user_games():
    email = request.args.get('email')
    print(request.args)
    user_games = games[email]
    return jsonify({"games": user_games}), 200


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


if __name__ == '__main__':
    app.run(debug=True, port=5000)
