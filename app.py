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
# app.py is in C:\Users\jayde\Downloads\LostandFound
# CapturedImages is in C:\Users\jayde\Downloads\LostandFound\ReturnPoint\bin\Debug\net6.0-windows\CapturedImages
CAPTURED_IMAGES_FOLDER = os.path.join(os.path.dirname(__file__), 'ReturnPoint', 'bin', 'Debug', 'net6.0-windows', 'CapturedImages')
ALLOWED_EXTENSIONS = {'png', 'jpg', 'jpeg', 'gif'}

if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)

# Ensure captured images folder exists
if not os.path.exists(CAPTURED_IMAGES_FOLDER):
    os.makedirs(CAPTURED_IMAGES_FOLDER)

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
    username_or_email = data.get('username', '').lower()
    password = data.get('password', '')
    
    # Find user by username or email
    user = None
    for u in users:
        if (u.get('username', '').lower() == username_or_email or 
            u.get('email', '').lower() == username_or_email) and u.get('password') == password:
            user = u
            break
    
    if user:
        session['username'] = user.get('username') or user.get('email')
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
    username_or_email = session.get('username', '')
    
    # Find user by username or email
    user = None
    for u in users:
        if (u.get('username', '').lower() == username_or_email.lower() or 
            u.get('email', '').lower() == username_or_email.lower()):
            user = u
            break
    
    if user:
        return jsonify({
            'username': user.get('username') or user.get('email'),
            'email': user.get('email'),
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

@app.route('/api/images', methods=['GET'])
def get_images():
    """Fetch all images from the CapturedImages folder"""
    try:
        if not os.path.exists(CAPTURED_IMAGES_FOLDER):
            return jsonify({'images': [], 'available_tags': []})
        
        images = []
        image_extensions = ('.jpg', '.jpeg', '.png', '.gif', '.webp')
        
        for filename in os.listdir(CAPTURED_IMAGES_FOLDER):
            if filename.lower().endswith(image_extensions):
                filepath = os.path.join(CAPTURED_IMAGES_FOLDER, filename)
                # Get file metadata
                stat_info = os.stat(filepath)
                timestamp = stat_info.st_mtime
                
                # Try to load associated metadata
                info_file = filepath.replace(os.path.splitext(filename)[1], '_info.txt')
                info = ''
                if os.path.exists(info_file):
                    try:
                        with open(info_file, 'r') as f:
                            info = f.read()
                    except:
                        pass
                
                # Try to load tags
                tags_file = filepath.replace(os.path.splitext(filename)[1], '_tags.txt')
                tags = []
                if os.path.exists(tags_file):
                    try:
                        with open(tags_file, 'r') as f:
                            tags = [tag.strip() for tag in f.readlines() if tag.strip()]
                    except:
                        pass
                
                images.append({
                    'id': len(images) + 1,
                    'filename': filename,
                    'url': f'/api/images/{filename}',
                    'tags': tags,
                    'info': info,
                    'timestamp': timestamp
                })
        
        # Load available tags from global tags.txt file
        available_tags = []
        global_tags_file = os.path.join(os.path.dirname(CAPTURED_IMAGES_FOLDER), 'tags.txt')
        if os.path.exists(global_tags_file):
            try:
                with open(global_tags_file, 'r') as f:
                    available_tags = [tag.strip() for tag in f.readlines() if tag.strip()]
            except:
                pass
        
        # Also extract tags from individual images in case they're not in global file
        image_tags = set()
        for img in images:
            for tag in img.get('tags', []):
                image_tags.add(tag)
        
        # Combine and deduplicate
        all_tags = list(set(available_tags + list(image_tags)))
        all_tags.sort()
        
        return jsonify({
            'images': images,
            'available_tags': all_tags
        })
    except Exception as e:
        print(f"Error loading images: {e}")
        return jsonify({'images': [], 'available_tags': []})

@app.route('/api/images/<filename>', methods=['GET'])
def get_image(filename):
    """Serve a specific image from CapturedImages folder"""
    try:
        # Security: prevent directory traversal
        if '..' in filename or '/' in filename or '\\' in filename:
            return jsonify({'message': 'Invalid filename'}), 400
        return send_from_directory(CAPTURED_IMAGES_FOLDER, filename)
    except Exception as e:
        return jsonify({'message': 'Image not found'}), 404

@app.route('/api/images/<filename>/info', methods=['GET', 'POST'])
def image_info(filename):
    """Get or save image info and tags"""
    try:
        if '..' in filename or '/' in filename or '\\' in filename:
            return jsonify({'message': 'Invalid filename'}), 400
        
        filepath = os.path.join(CAPTURED_IMAGES_FOLDER, filename)
        if not os.path.exists(filepath):
            return jsonify({'message': 'Image not found'}), 404
        
        if request.method == 'GET':
            # Return current info and tags
            info_file = filepath.replace(os.path.splitext(filename)[1], '_info.txt')
            tags_file = filepath.replace(os.path.splitext(filename)[1], '_tags.txt')
            
            info = ''
            tags = []
            
            if os.path.exists(info_file):
                with open(info_file, 'r') as f:
                    info = f.read()
            
            if os.path.exists(tags_file):
                with open(tags_file, 'r') as f:
                    tags = [tag.strip() for tag in f.readlines() if tag.strip()]
            
            return jsonify({'info': info, 'tags': tags})
        
        elif request.method == 'POST':
            # Save info and tags
            data = request.json
            info = data.get('info', '')
            tags = data.get('tags', [])
            
            info_file = filepath.replace(os.path.splitext(filename)[1], '_info.txt')
            tags_file = filepath.replace(os.path.splitext(filename)[1], '_tags.txt')
            
            with open(info_file, 'w') as f:
                f.write(info)
            
            with open(tags_file, 'w') as f:
                for tag in tags:
                    f.write(tag + '\n')
            
            return jsonify({'message': 'Image info saved successfully'})
    
    except Exception as e:
        print(f"Error handling image info: {e}")
        return jsonify({'message': 'Error processing request'}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)