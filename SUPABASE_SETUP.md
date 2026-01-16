# Supabase Setup Guide

## What is Supabase?

Supabase is a cloud-based PostgreSQL database with:
- **Web Dashboard**: View and edit data like Google Sheets
- **Real-time API**: Perfect for web and mobile apps
- **Authentication**: Built-in user management
- **Free Tier**: Plenty for small projects

## Quick Start (5 minutes)

### 1. Create Supabase Account & Project

1. Visit https://supabase.com
2. Click **"Start your project"** and sign up with GitHub/email
3. Create new project:
   - **Project name**: `lostandfound`
   - **Database password**: (save this securely!)
   - **Region**: Select closest to you (e.g., US-East)
   - Click **Create new project**
   - ⏳ Wait 2-3 minutes for initialization

### 2. Get Your API Credentials

Once the project is created:

1. In the sidebar, click **Settings** → **API**
2. Copy these values and save them:
   - **Project URL** (looks like: `https://xxxxxxxxxxx.supabase.co`)
   - **anon public** key

### 3. Set Environment Variables

**Windows PowerShell:**
```powershell
$env:SUPABASE_URL = "https://your-project.supabase.co"
$env:SUPABASE_KEY = "your-anon-key-here"
```

**Windows Command Prompt (cmd.exe):**
```cmd
set SUPABASE_URL=https://your-project.supabase.co
set SUPABASE_KEY=your-anon-key-here
```

**Permanent Setup (Add to System):**
1. Press `Win + R`
2. Type `sysdm.cpl` and press Enter
3. Go to **Advanced** → **Environment Variables**
4. Click **New** under "System variables"
   - Variable name: `SUPABASE_URL`
   - Variable value: `https://your-project.supabase.co`
5. Click **New** again for the key
   - Variable name: `SUPABASE_KEY`
   - Variable value: `your-anon-key-here`
6. Click OK and restart your terminal

### 4. Create Database Tables

1. In Supabase dashboard, click **SQL Editor** (left sidebar)
2. Click **New Query**
3. Paste this SQL:

```sql
-- Create users table
CREATE TABLE users (
  id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
  name TEXT NOT NULL,
  email TEXT UNIQUE NOT NULL,
  grade_section TEXT,
  password TEXT NOT NULL,
  profile_picture TEXT,
  role TEXT DEFAULT 'user',
  first_name TEXT,
  middle_name TEXT,
  last_name TEXT,
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW()
);

-- Create images table
CREATE TABLE images (
  id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
  filename TEXT NOT NULL UNIQUE,
  capture_timestamp TIMESTAMP DEFAULT NOW(),
  user_name TEXT,
  tags TEXT,
  status TEXT DEFAULT 'active',
  file_path TEXT,
  image_data TEXT,
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW()
);

-- Create indexes for performance
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_images_filename ON images(filename);
CREATE INDEX idx_images_status ON images(status);
```

4. Click **Run**
5. ✅ Tables are created!

### 5. Test the Connection

Run this command in your terminal:

```bash
python app.py
```

You should see:
```
✓ Supabase connected: https://your-project.supabase.co
DEBUG: Successfully initialized Supabase!
```

### 6. View & Edit Data

Now you can edit data directly in Supabase:

1. In Supabase dashboard, click **Table Editor** (left sidebar)
2. Click **users** or **images** table
3. Edit data just like Google Sheets:
   - Click any cell to edit
   - Click **+** to add new row
   - Click the trash icon to delete row
4. Changes are saved instantly!

---

## Using the Supabase Manager

The `supabase_manager.py` file provides these methods:

### Read Data
```python
# Get all users
users = db_manager.read_data('users')

# Get user by email
user = db_manager.read_data('users', where_clause='email', where_value='john@example.com')

# Get all images
images = db_manager.read_data('images')
```

### Add Data
```python
# Add new user
db_manager.append_row({
    'first_name': 'John',
    'last_name': 'Doe',
    'email': 'john@example.com',
    'password': 'hashed_password',
    'grade_section': 'GRADE12-PYTHON'
}, 'users')

# Add new image
db_manager.log_captured_image(
    filename='photo_001.jpg',
    user_name='John Doe',
    tags='lost-item,red-backpack',
    file_path='/path/to/photo.jpg'
)
```

### Update Data
```python
db_manager.update_row(user_id, {
    'grade_section': 'GRADE11-PYTHON'
}, 'users')
```

### Encode/Decode Images
```python
# Save image as base64 in database
base64_image = db_manager.encode_image_to_base64('/path/to/image.jpg')

# Retrieve image from database
db_manager.decode_image_from_base64(base64_string, '/path/to/save/image.jpg')
```

---

## Troubleshooting

### "SUPABASE_URL and SUPABASE_KEY environment variables are required"

**Solution:**
1. Make sure you set the environment variables (see Step 3 above)
2. After setting them, restart your terminal
3. Run `echo %SUPABASE_URL%` to verify it's set

### "Failed to connect to Supabase"

**Solution:**
1. Check your Project URL is correct (no trailing slash)
2. Make sure the API key is the `anon public` key, not the `service_role` key
3. Verify your Supabase project was created successfully

### "Table 'users' does not exist"

**Solution:**
1. Make sure you ran the SQL in the "Create Database Tables" section
2. Check that the query executed without errors
3. Refresh the Supabase dashboard

---

## Next Steps

1. ✅ Set up Supabase account
2. ✅ Create tables
3. ✅ Set environment variables
4. ✅ Test the connection
5. 🎉 Your app now uses Supabase!

Any registration/login will now save data to Supabase, and you can edit it anytime in the dashboard.

---

## Free Tier Limits

Supabase free tier includes:
- **500 MB** database space
- **2 GB** file storage
- **50,000** monthly active users

Perfect for testing and small applications!
