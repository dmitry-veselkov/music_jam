from flask import Flask, send_from_directory, jsonify, request
import os

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
FRONTEND_DIR = os.path.abspath(os.path.join(BASE_DIR, '..', 'frontend'))

app = Flask(__name__, static_folder=FRONTEND_DIR)


# @app.route('/api/game/<game_id>', methods=['GET'])
# def get_game_settings(game_id):
#     # Здесь будет логика базы данных. Пока отдаем заглушку.
#     settings = {
#         "id": game_id,
#         "name": f"Квиз {game_id}",
#         "categories": ["Рок", "Поп", "Кино"],
#         "status": "ready"
#     }
#     return jsonify(settings)

@app.route('/api/check_room', methods=['GET'])
def get_room_info():
    code = request.args.get('roomCode').upper()
    exists = code == "12345"  # TODO Переделать MOCK
    return jsonify({"code": code, "exists": exists, "status": "waiting"})


# @app.route('/room/<room_code>', methods=['GET'])
# def check_room_exists(room_code):
#     exists = room_code == "12345"  # TODO Переделать MOCK
#     if exists:
#


@app.route('/', defaults={'path': ''})
@app.route('/<path:path>')
def catch_all(path):
    file_path = os.path.join(app.static_folder, path)

    if path != "" and os.path.exists(file_path):
        return send_from_directory(app.static_folder, path)
    return send_from_directory(app.static_folder, 'index.html')


if __name__ == '__main__':
    app.run(debug=True, port=5000)
