from flask import Flask, request, jsonify, send_from_directory, session, redirect, url_for
import os
import json
from werkzeug.utils import secure_filename
from google_sheets_manager import GoogleSheetsManager

app = Flask(__name__, static_folder='.', static_url_path='')

if os.environ.get('REPLIT_DEPLOYMENT') == '1':
    if 'FLASK_SECRET_KEY' not in os.environ:
        raise ValueError('FLASK_SECRET_KEY must be set in production')
    app.secret_key = os.environ['FLASK_SECRET_KEY']
else:
    app.secret_key = os.environ.get('FLASK_SECRET_KEY', 'dev-secret-key-for-local-testing-only')

# Google Sheets configuration
SPREADSHEET_ID = os.environ.get('GOOGLE_SHEETS_ID')  # You'll set this in Step 6
CREDENTIALS_FILE = 'credentials.json'  # Your service account JSON key

# Debug: Check what we have
print(f"DEBUG: CREDENTIALS_FILE exists? {os.path.exists(CREDENTIALS_FILE)}")
print(f"DEBUG: SPREADSHEET_ID = {SPREADSHEET_ID}")

# Initialize Google Sheets manager (we'll handle errors gracefully)
gs_manager = None
try:
    if os.path.exists(CREDENTIALS_FILE) and SPREADSHEET_ID:
        print("DEBUG: Attempting to connect to Google Sheets...")
        gs_manager = GoogleSheetsManager(CREDENTIALS_FILE, SPREADSHEET_ID)
        print("DEBUG: Successfully initialized Google Sheets!")
    else:
        print("DEBUG: Skipping Google Sheets (missing credentials or ID)")
except Exception as e:
    print(f"⚠ Warning: Could not initialize Google Sheets: {e}")
    import traceback
    traceback.print_exc()
    print("  Falling back to local JSON storage")

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
    """Load users from Google Sheets if available, otherwise from JSON"""
    # Try Google Sheets first
    if gs_manager:
        try:
            print("DEBUG: Attempting to load users from Google Sheets...")
            data = gs_manager.read_data('users')
            if data:
                print(f"DEBUG: Loaded {len(data)} users from Google Sheets")
                for u in data:
                    print(f"   - {u.get('email', 'NO EMAIL')}")
                return data
            else:
                print("DEBUG: Google Sheets returned empty data, falling back to JSON")
        except Exception as e:
            print(f"ERROR: Error reading from Google Sheets: {e}")
            import traceback
            traceback.print_exc()
    
    # Fallback to JSON
    print("DEBUG: Loading users from local JSON file...")
    if not os.path.exists(USERS_FILE):
        print(f"DEBUG: {USERS_FILE} does not exist")
        return []
    with open(USERS_FILE, 'r') as f:
        data = json.load(f)
        print(f"DEBUG: Loaded {len(data)} users from JSON")
        for u in data:
            print(f"   - {u.get('email', 'NO EMAIL')}")
        return data

def get_full_name(user):
    """Get full name from user object"""
    return user.get('name', 'User').strip()

def combine_name(first_name, middle_name, last_name):
    """Combine first, middle, and last name into full name"""
    parts = []
    if first_name and first_name.strip():
        parts.append(first_name.strip())
    if middle_name and middle_name.strip():
        parts.append(middle_name.strip())
    if last_name and last_name.strip():
        parts.append(last_name.strip())
    return ' '.join(parts) if parts else None

def save_users(users):
    """Save users to Google Sheets if available, otherwise to JSON"""
    # Try Google Sheets first
    if gs_manager:
        try:
            gs_manager.write_data(users, 'users')
            print("✓ Saved to Google Sheets")
            return
        except Exception as e:
            print(f"Error writing to Google Sheets: {e}")
    
    # Fallback to JSON
    with open(USERS_FILE, 'w') as f:
        json.dump(users, f, indent=2)
    print("✓ Saved to local JSON")

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
    
    # Check if password and confirm password match
    if password != confirm_password:
        return jsonify({'message': 'Password and confirm password do not match'}), 400
    
    # Check if email already exists
    if any(u.get('email', '').lower() == email for u in users):
        return jsonify({'message': 'Email already registered'}), 400
    
    # Combine name parts
    first_name = data.get('first_name', '').strip()
    middle_name = data.get('middle_name', '').strip()
    last_name = data.get('last_name', '').strip()
    full_name = combine_name(first_name, middle_name, last_name)
    
    if not full_name:
        return jsonify({'message': 'At least first name and last name are required'}), 400
    
    users.append({
        'name': full_name,
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
    
    print(f"\n📝 LOGIN ATTEMPT:")
    print(f"   Email: {email}")
    print(f"   Password: {password}")
    print(f"   Users in database: {len(users)}")
    for i, u in enumerate(users):
        print(f"      User {i+1}: {u.get('email', 'NO EMAIL')} (pwd: {u.get('password', 'NO PASSWORD')})")
    
    # Check if email exists
    email_found = False
    for u in users:
        if u.get('email', '').lower() == email:
            email_found = True
            # Email exists, check password
            if u.get('password') == password:
                print(f"   ✓ LOGIN SUCCESSFUL for {email}")
                session['email'] = u.get('email')
                session['full_name'] = get_full_name(u)
                # Save logged-in user email to file so C# app can read it
                try:
                    with open('current_user_email.txt', 'w') as f:
                        f.write(email)
                    print(f"   ✓ Saved current user email to current_user_email.txt")
                except Exception as e:
                    print(f"   ✗ Error saving current user email: {e}")
                return jsonify({'message': 'Login successful'})
            else:
                # Email found but password is wrong
                print(f"   ✗ LOGIN FAILED for {email} - incorrect password")
                return jsonify({'message': 'Incorrect password'}), 401
    
    if not email_found:
        # Email not found
        print(f"   ✗ LOGIN FAILED for {email} - email not found")
        return jsonify({'message': 'Email not registered'}), 401

@app.route('/api/logout', methods=['POST'])
def logout():
    session.pop('email', None)
    session.pop('full_name', None)
    # Clear the current user email file
    try:
        if os.path.exists('current_user_email.txt'):
            os.remove('current_user_email.txt')
            print(f"✓ Cleared current_user_email.txt on logout")
    except Exception as e:
        print(f"Error clearing current user email: {e}")
    return jsonify({'message': 'Logged out successfully'})

@app.route('/api/user', methods=['GET'])
def get_user():
    # First try to get email from session (web client)
    user_email = session.get('email', '')
    
    # If no session email, try to get from query parameter (C# app)
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
        
        # If profile picture is base64, return it as data URI
        if profile_pic and profile_pic.startswith('iVBOR') or profile_pic.startswith('/9j/'):  # PNG or JPEG base64
            profile_pic = f"data:image/png;base64,{profile_pic}" if profile_pic.startswith('iVBOR') else f"data:image/jpeg;base64,{profile_pic}"
        
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
            user['grade_section'] = grade_section
            session['full_name'] = full_name
            save_users(users)
            return jsonify({
                'message': 'Profile updated successfully',
                'name': user['name'],
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
                # Try to save with base64 encoding if Google Sheets is available
                if gs_manager:
                    try:
                        import base64
                        with open(filepath, 'rb') as img_file:
                            image_data = base64.b64encode(img_file.read()).decode('utf-8')
                        user['profile_picture'] = image_data
                        print(f"✓ Stored profile picture as base64 in Google Sheets ({len(image_data)} chars)")
                    except Exception as e:
                        print(f"Warning: Could not encode to base64: {e}")
                        user['profile_picture'] = f"/uploads/profile_pictures/{filename}"
                else:
                    # Fallback to file path if no Google Sheets
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
    try:
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
    except Exception as e:
        print(f"Error loading images: {e}")
        return jsonify({'images': [], 'available_tags': []})

@app.route('/api/images/<filename>', methods=['GET'])
def get_image(filename):
    try:
        if '..' in filename or '/' in filename or '\\' in filename:
            return jsonify({'message': 'Invalid filename'}), 400
        return send_from_directory(CAPTURED_IMAGES_FOLDER, filename)
    except Exception as e:
        return jsonify({'message': 'Image not found'}), 404

@app.route('/api/images/<filename>/info', methods=['GET', 'POST'])
def image_info(filename):
    try:
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
    
    except Exception as e:
        print(f"Error handling image info: {e}")
        return jsonify({'message': 'Error processing request'}), 500

@app.route('/api/log-captured-image', methods=['POST'])
def log_captured_image():
    if 'email' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    data = request.json
    filename = data.get('filename', '')
    tags = data.get('tags', '')
    file_path = data.get('file_path', '')
    timestamp = data.get('timestamp', '')
    
    if not filename:
        return jsonify({'message': 'Filename required'}), 400
    
    try:
        users = load_users()
        user_email = session.get('email', '')
        user_name = 'Unknown'
        
        for u in users:
            if u.get('email', '').lower() == user_email.lower():
                user_name = u.get('name', 'Unknown')
                break
        
        # Try to log to Google Sheets
        if gs_manager:
            try:
                # Read and encode image as base64 if file exists
                image_data = None
                if file_path and os.path.exists(file_path):
                    try:
                        import base64
                        with open(file_path, 'rb') as img_file:
                            image_data = base64.b64encode(img_file.read()).decode('utf-8')
                        print(f"✓ Encoded image to base64 ({len(image_data)} chars)")
                    except Exception as e:
                        print(f"Warning: Could not encode image: {e}")
                
                result = gs_manager.log_captured_image(
                    filename=filename,
                    user_name=user_name,
                    tags=tags,
                    file_path=file_path,
                    timestamp=timestamp,
                    image_data=image_data
                )
                if result:
                    print(f"✓ Image logged to Google Sheets: {filename}")
                    return jsonify({'message': 'Image logged successfully'})
                else:
                    print(f"⚠ Failed to log image to Google Sheets: {filename}")
            except Exception as e:
                print(f"Error logging to Google Sheets: {e}")
        
        # Fallback successful regardless (local storage already saved)
        return jsonify({'message': 'Image recorded'})
    
    except Exception as e:
        print(f"Error in log_captured_image: {e}")
        return jsonify({'message': 'Error recording image'}), 500

@app.route('/api/delete-captured-image', methods=['POST'])
def delete_captured_image():
    if 'email' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    data = request.json
    filename = data.get('filename', '')
    
    if not filename:
        return jsonify({'message': 'Filename required'}), 400
    
    try:
        # Try to update status to deleted in Google Sheets
        if gs_manager:
            try:
                images = gs_manager.read_data('images')
                for img in images:
                    if img.get('filename', '') == filename:
                        img['status'] = 'deleted'
                
                # Write back updated images
                result = gs_manager.write_data(images, 'images')
                if result:
                    print(f"✓ Image marked as deleted in Google Sheets: {filename}")
                    return jsonify({'message': 'Image deleted successfully'})
            except Exception as e:
                print(f"Error updating Google Sheets: {e}")
        
        return jsonify({'message': 'Deleted'})
    
    except Exception as e:
        print(f"Error in delete_captured_image: {e}")
        return jsonify({'message': 'Error deleting image'}), 500

@app.route('/api/restore-captured-image', methods=['POST'])
def restore_captured_image():
    if 'email' not in session:
        return jsonify({'message': 'Not logged in'}), 401
    
    data = request.json
    filename = data.get('filename', '')
    
    if not filename:
        return jsonify({'message': 'Filename required'}), 400
    
    try:
        # Try to update status to active in Google Sheets
        if gs_manager:
            try:
                images = gs_manager.read_data('images')
                for img in images:
                    if img.get('filename', '') == filename:
                        img['status'] = 'active'
                
                # Write back updated images
                result = gs_manager.write_data(images, 'images')
                if result:
                    print(f"✓ Image restored to active in Google Sheets: {filename}")
                    return jsonify({'message': 'Image restored successfully'})
            except Exception as e:
                print(f"Error updating Google Sheets: {e}")
        
        return jsonify({'message': 'Restored'})
    
    except Exception as e:
        print(f"Error in restore_captured_image: {e}")
        return jsonify({'message': 'Error restoring image'}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)