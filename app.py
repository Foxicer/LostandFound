from flask import Flask, request, jsonify, send_from_directory
import os
import json

app = Flask(__name__, static_folder='.', static_url_path='')

USERS_FILE = 'users.json'

def load_users():
    if not os.path.exists(USERS_FILE):
        return []
    with open(USERS_FILE, 'r') as f:
        return json.load(f)

def save_users(users):
    with open(USERS_FILE, 'w') as f:
        json.dump(users, f)

@app.route('/')
def index():
    return send_from_directory('.', 'index.html')

@app.route('/register.html')
def register_page():
    return send_from_directory('.', 'register.html')

@app.route('/api/register', methods=['POST'])
def register():
    data = request.json
    users = load_users()
    if any(u['username'] == data['username'] for u in users):
        return jsonify({'message': 'Username already exists'}), 400
    users.append({
        'username': data['username'],
        'email': data['email'],
        'password': data['password']
    })
    save_users(users)
    return jsonify({'message': 'Registration successful'})

@app.route('/api/login', methods=['POST'])
def login():
    data = request.json
    users = load_users()
    user = next((u for u in users if u['username'] == data['username'] and u['password'] == data['password']), None)
    if user:
        return jsonify({'message': 'Login successful'})
    return jsonify({'message': 'Invalid username or password'}), 401

if __name__ == '__main__':
    app.run(debug=True)