from flask import Flask, request, jsonify, send_from_directory, session, redirect, url_for
import os
import json
from werkzeug.utils import secure_filename

app = Flask(__name__, static_folder='.', static_url_path='')

if os.environ.get('REPLIT_DEPLOYMENT') == '1':
    if 'FLASK_SECRET_KEY' not in os.environ:
        raise ValueError('FLASK_SECRET_KEY must be set in production')
    app.secret_key = os.environ['FLASK_SECRET_KEY']
else:
    app.secret_key = os.environ.get('FLASK_SECRET_KEY', 'dev-secret-key-for-local-testing-only')

USERS_FILE = 'users.json'
UPLOAD_FOLDER = 'uploads/profile_pictures'
ALLOWED_EXTENSIONS = {'png', 'jpg', 'jpeg', 'gif'}

if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)

def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS

def load_users():
    if not os.path.exists(USERS_FILE):
        return []
    with open(USERS_FILE, 'r') as f:
        return json.load(f)

def save_users(users):
    with open(USERS_FILE, 'w') as f:
        json.dump(users, f, indent=2)

@app.route('/')
def index():
    return send_from_directory('.', 'index.html')

@app.route('/register.html')
def register_page():
    return send_from_directory('.', 'register.html')

@app.route('/dashboard.html')
def dashboard_page():
    if 'username' not in session:
        return redirect(url_for('index'))
    return send_from_directory('.', 'dashboard.html')

@app.route('/api/register', methods=['POST'])
def register():
    data = request.json
    users = load_users()
    if any(u['username'] == data['username'] for u in users):
        return jsonify({'message': 'Username already exists'}), 400
    users.append({
        'username': data['username'],
        'email': data['email'],
        'password': data['password'],
        'profile_picture': None
    })
    save_users(users)
    return jsonify({'message': 'Registration successful'})

@app.route('/api/login', methods=['POST'])
def login():
    data = request.json
    users = load_users()
    user = next((u for u in users if u['username'] == data['username'] and u['password'] == data['password']), None)
    if user:
        session['username'] = user['username']
        return jsonify({'message': 'Login successful'})
    return jsonify({'message': 'Invalid username or password'}), 401

@app.route('/api/logout', methods=['POST'])
def logout():
    session.pop('username', None)
    return jsonify({'message': 'Logged out successfully'})

@app.route('/api/user', methods=['GET'])
def get_user():
    if 'username' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    users = load_users()
    user = next((u for u in users if u['username'] == session['username']), None)
    if user:
        return jsonify({
            'username': user['username'],
            'email': user['email'],
            'profile_picture': user.get('profile_picture')
        })
    return jsonify({'message': 'User not found'}), 404

@app.route('/api/update-username', methods=['POST'])
def update_username():
    if 'username' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    data = request.json
    new_username = data.get('username')
    
    if not new_username:
        return jsonify({'message': 'Username is required'}), 400
    
    users = load_users()
    current_username = session['username']
    
    if any(u['username'] == new_username and u['username'] != current_username for u in users):
        return jsonify({'message': 'Username already taken'}), 400
    
    for user in users:
        if user['username'] == current_username:
            user['username'] = new_username
            session['username'] = new_username
            save_users(users)
            return jsonify({'message': 'Username updated successfully', 'username': new_username})
    
    return jsonify({'message': 'User not found'}), 404

@app.route('/api/upload-profile-picture', methods=['POST'])
def upload_profile_picture():
    if 'username' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    if 'profile_picture' not in request.files:
        return jsonify({'message': 'No file provided'}), 400
    
    file = request.files['profile_picture']
    
    if file.filename == '':
        return jsonify({'message': 'No file selected'}), 400
    
    if file and allowed_file(file.filename):
        filename = secure_filename(f"{session['username']}_{file.filename}")
        filepath = os.path.join(UPLOAD_FOLDER, filename)
        file.save(filepath)
        
        users = load_users()
        for user in users:
            if user['username'] == session['username']:
                user['profile_picture'] = f"/uploads/profile_pictures/{filename}"
                save_users(users)
                return jsonify({
                    'message': 'Profile picture uploaded successfully',
                    'profile_picture': user['profile_picture']
                })
        
        return jsonify({'message': 'User not found'}), 404
    
    return jsonify({'message': 'Invalid file type. Allowed: png, jpg, jpeg, gif'}), 400

@app.route('/uploads/profile_pictures/<filename>')
def uploaded_file(filename):
    return send_from_directory(UPLOAD_FOLDER, filename)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)