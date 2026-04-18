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

game_characteristics = {
    '12345': {
        'name': 'Крутая игра',
        'author': 'Dima',
        'description': 'фывар',
        'categories': ['asdf', 'asdf'],
        'costs': [100, 200]
    }
}

game_active = {
    '12345': {
        "status": "waiting",
        'name': 'Крутая игра',
        'author': 'Dima',
        'description': 'фывар',
        'categories': ['asdf', 'asdf'],
        'costs': [100, 200],
        'teams': list(teams.values())
    }
}


@app.route('/api/gameSettings', methods=['GET', 'POST'])
def get_room_settings():
    if request.method == 'GET':
        code = request.args.get('roomCode', '').upper()
        game = game_characteristics.get(code)

        if not game:
            return jsonify({
                "exists": False
            })
        return jsonify({
            "exists": True,
            "roomCode": code,
            **game
        })
    else:
        data = request.get_json()
        code = data.get('roomCode', '').upper()
        if not code:
            return jsonify({
                "success": False,
                "error": "roomCode is required"
            }), 400

        game_characteristics[code] = {
            'name': data.get('name', ''),
            'author': data.get('author', ''),
            'description': data.get('description', ''),
            'maxTeams': data.get('maxTeams', 4),
            'categories': data.get('categories', []),
            'costs': data.get('costs', []),
            'tracks': data.get('tracks', {})
        }

        return jsonify({
            "success": True,
            "roomCode": code,
            "game": game_characteristics[code]
        })


@app.route('/api/check_room', methods=['GET'])
def get_room_info():
    code = request.args.get('roomCode').upper()
    try:
        room_info = game_active[code]
        exists = True
    except:
        room_info = None
        exists = False
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


if __name__ == '__main__':
    app.run(debug=True, port=5000)
