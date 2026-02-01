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
CAPTURED_IMAGES_FOLDER = os.path.join(os.path.dirname(__file__), 'ReturnPoint', 'bin', 'Debug', 'net6.0-windows', 'CapturedImages')
ALLOWED_EXTENSIONS = {'png', 'jpg', 'jpeg', 'gif'}

if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)

if not os.path.exists(CAPTURED_IMAGES_FOLDER):
    os.makedirs(CAPTURED_IMAGES_FOLDER)

def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS

def load_users():
    try:
        if os.path.exists(USERS_FILE):
            with open(USERS_FILE, 'r') as f:
                return json.load(f)
        return []
    except Exception as e:
        return []

def get_full_name(user):
    return user.get('name', 'User').strip()

def combine_name(first_name, middle_name, last_name):
    parts = []
    if first_name and first_name.strip():
        parts.append(first_name.strip())
    if middle_name and middle_name.strip():
        parts.append(middle_name.strip())
    if last_name and last_name.strip():
        parts.append(last_name.strip())
    return ' '.join(parts) if parts else None

def save_users(users):
    try:
        with open(USERS_FILE, 'w') as f:
            json.dump(users, f, indent=2)
    except Exception as e:
        pass

@app.route('/')
def index():
    return send_from_directory('.', 'index.html')

@app.route('/register.html')
def register_page():
    return send_from_directory('.', 'register.html')

@app.route('/dashboard.html')
def dashboard_page():
    if 'email' not in session:
        return redirect(url_for('index'))
    return send_from_directory('.', 'dashboard.html')

@app.route('/api/register', methods=['POST'])
def register():
    data = request.json
    users = load_users()
    email = data.get('email', '').lower()
    password = data.get('password', '')
    confirm_password = data.get('confirm_password', '')
    
    if password != confirm_password:
        return jsonify({'message': 'Password and confirm password do not match'}), 400
    
    if any(u.get('email', '').lower() == email for u in users):
        return jsonify({'message': 'Email already registered'}), 400
    
    first_name = data.get('first_name', '').strip()
    middle_name = data.get('middle_name', '').strip()
    last_name = data.get('last_name', '').strip()
    full_name = combine_name(first_name, middle_name, last_name)
    
    if not full_name:
        return jsonify({'message': 'At least first name and last name are required'}), 400
    
    users.append({
        'name': full_name,
        'first_name': first_name,
        'middle_name': middle_name,
        'last_name': last_name,
        'email': email,
        'grade_section': data.get('grade_section', '').strip(),
        'password': data.get('password', ''),
        'profile_picture': '',
        'role': 'user'
    })
    save_users(users)
    return jsonify({'message': 'Registration successful'})

@app.route('/api/login', methods=['POST'])
def login():
    data = request.json
    users = load_users()
    email = data.get('email', '').lower()
    password = data.get('password', '')
    
    for u in users:
        if u.get('email', '').lower() == email:
            if u.get('password') == password:
                session['email'] = u.get('email')
                session['full_name'] = get_full_name(u)
                try:
                    with open('current_user_email.txt', 'w') as f:
                        f.write(email)
                except:
                    pass
                return jsonify({'message': 'Login successful'})
            else:
                return jsonify({'message': 'Incorrect password'}), 401
    
    return jsonify({'message': 'Email not registered'}), 401

@app.route('/api/logout', methods=['POST'])
def logout():
    session.pop('email', None)
    session.pop('full_name', None)
    try:
        if os.path.exists('current_user_email.txt'):
            os.remove('current_user_email.txt')
    except:
        pass
    return jsonify({'message': 'Logged out successfully'})

@app.route('/api/user', methods=['GET'])
def get_user():
    user_email = session.get('email', '')
    
    if not user_email:
        user_email = request.args.get('email', '').lower()
    
    if not user_email:
        return jsonify({'message': 'Not logged in'}), 401
    
    users = load_users()
    
    user = None
    for u in users:
        if u.get('email', '').lower() == user_email.lower():
            user = u
            break
    
    if user:
        grade_section = user.get('grade_section', '') or 'N/A'
        profile_pic = user.get('profile_picture', '')
        
        return jsonify({
            'name': user.get('name', ''),
            'first_name': user.get('first_name', ''),
            'middle_name': user.get('middle_name', ''),
            'last_name': user.get('last_name', ''),
            'email': user.get('email', ''),
            'grade_section': grade_section,
            'profile_picture': profile_pic,
            'role': user.get('role', 'user')
        })
    return jsonify({'message': 'User not found'}), 404

@app.route('/api/update-username', methods=['POST'])
def update_username():
    if 'email' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    data = request.json
    first_name = data.get('first_name', '').strip()
    middle_name = data.get('middle_name', '').strip()
    last_name = data.get('last_name', '').strip()
    grade_section = data.get('grade_section', '').strip()
    
    full_name = combine_name(first_name, middle_name, last_name)
    
    if not full_name:
        return jsonify({'message': 'At least first name and last name are required'}), 400
    
    users = load_users()
    user_email = session.get('email', '')
    
    for user in users:
        if user.get('email', '').lower() == user_email.lower():
            user['name'] = full_name
            user['first_name'] = first_name
            user['middle_name'] = middle_name
            user['last_name'] = last_name
            user['grade_section'] = grade_section
            session['full_name'] = full_name
            save_users(users)
            return jsonify({
                'message': 'Profile updated successfully',
                'name': user['name'],
                'first_name': user.get('first_name', ''),
                'middle_name': user.get('middle_name', ''),
                'last_name': user.get('last_name', ''),
                'grade_section': user['grade_section']
            })
    
    return jsonify({'message': 'User not found'}), 404

@app.route('/api/upload-profile-picture', methods=['POST'])
def upload_profile_picture():
    if 'email' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    if 'profile_picture' not in request.files:
        return jsonify({'message': 'No file provided'}), 400
    
    file = request.files['profile_picture']
    
    if file.filename == '':
        return jsonify({'message': 'No file selected'}), 400
    
    if file and allowed_file(file.filename):
        user_email = session.get('email', '').replace('@', '_').replace('.', '_')
        filename = secure_filename(f"{user_email}_{file.filename}")
        filepath = os.path.join(UPLOAD_FOLDER, filename)
        file.save(filepath)
        
        users = load_users()
        for user in users:
            if user.get('email', '').lower() == session['email'].lower():
                user['profile_picture'] = f"/uploads/profile_pictures/{filename}"
                save_users(users)
                return jsonify({
                    'message': 'Profile picture uploaded successfully',
                    'profile_picture': user.get('profile_picture', '')[:50] + '...' if len(user.get('profile_picture', '')) > 50 else user.get('profile_picture', '')
                })
        
        return jsonify({'message': 'User not found'}), 404
    
    return jsonify({'message': 'Invalid file type. Allowed: png, jpg, jpeg, gif'}), 400

@app.route('/uploads/profile_pictures/<filename>')
def uploaded_file(filename):
    return send_from_directory(UPLOAD_FOLDER, filename)

@app.route('/api/images', methods=['GET'])
def get_images():
    if not os.path.exists(CAPTURED_IMAGES_FOLDER):
        return jsonify({'images': [], 'available_tags': []})
    
    images = []
    image_extensions = ('.jpg', '.jpeg', '.png', '.gif', '.webp')
    
    for filename in os.listdir(CAPTURED_IMAGES_FOLDER):
        if filename.lower().endswith(image_extensions):
            filepath = os.path.join(CAPTURED_IMAGES_FOLDER, filename)
            stat_info = os.stat(filepath)
            timestamp = stat_info.st_mtime
            
            info_file = filepath.replace(os.path.splitext(filename)[1], '_info.txt')
            info = ''
            if os.path.exists(info_file):
                try:
                    with open(info_file, 'r') as f:
                        info = f.read()
                except:
                    pass
            
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
    
    available_tags = []
    global_tags_file = os.path.join(os.path.dirname(CAPTURED_IMAGES_FOLDER), 'tags.txt')
    if os.path.exists(global_tags_file):
        try:
            with open(global_tags_file, 'r') as f:
                available_tags = [tag.strip() for tag in f.readlines() if tag.strip()]
        except:
            pass
    
    image_tags = set()
    for img in images:
        for tag in img.get('tags', []):
            image_tags.add(tag)
    
    all_tags = list(set(available_tags + list(image_tags)))
    all_tags.sort()
    
    return jsonify({
        'images': images,
        'available_tags': all_tags
    })

@app.route('/api/images/<filename>', methods=['GET'])
def get_image(filename):
    if '..' in filename or '/' in filename or '\\' in filename:
        return jsonify({'message': 'Invalid filename'}), 400
    try:
        return send_from_directory(CAPTURED_IMAGES_FOLDER, filename)
    except:
        return jsonify({'message': 'Image not found'}), 404

@app.route('/api/images/<filename>/info', methods=['GET', 'POST'])
def image_info(filename):
    if '..' in filename or '/' in filename or '\\' in filename:
        return jsonify({'message': 'Invalid filename'}), 400
    
    filepath = os.path.join(CAPTURED_IMAGES_FOLDER, filename)
    if not os.path.exists(filepath):
        return jsonify({'message': 'Image not found'}), 404
    
    if request.method == 'GET':
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

@app.route('/api/log-captured-image', methods=['POST'])
def log_captured_image():
    if 'email' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    data = request.json
    filename = data.get('filename', '')
    
    if not filename:
        return jsonify({'message': 'Filename required'}), 400
    
    return jsonify({'message': 'Image recorded'})

@app.route('/api/delete-captured-image', methods=['POST'])
def delete_captured_image():
    if 'email' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    data = request.json
    filename = data.get('filename', '')
    
    if not filename:
        return jsonify({'message': 'Filename required'}), 400
    
    return jsonify({'message': 'Deleted'})

@app.route('/api/restore-captured-image', methods=['POST'])
def restore_captured_image():
    if 'email' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    data = request.json
    filename = data.get('filename', '')
    
    if not filename:
        return jsonify({'message': 'Filename required'}), 400
    
    return jsonify({'message': 'Restored'})

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)