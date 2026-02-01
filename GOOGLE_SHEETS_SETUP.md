# Google Sheets Integration Setup Guide

## Overview
This guide will help you connect your Lost and Found app to Google Sheets so all user data is stored in the cloud instead of a local JSON file.

---

## Step 1: Google Cloud Setup ✅ (Already done? Skip to Step 5)

If you haven't done Steps 1-2 from the main guide yet:

### 1a. Create Google Cloud Project
- Go to https://console.cloud.google.com/
- Click on the project dropdown (top-left)
- Click "NEW PROJECT"
- Name it `LostAndFound` and click "CREATE"

### 1b. Enable Google Sheets API
- In the search bar, search for "Google Sheets API"
- Click on it and press "ENABLE"

### 1c. Create Service Account
- Go to "APIs & Services" > "Credentials" (left sidebar)
- Click "CREATE CREDENTIALS" > "Service Account"
- Fill in:
  - Service account name: `lostandfound-bot`
  - Leave the rest blank
- Click "CREATE AND CONTINUE" then "DONE"

### 1d. Create and Download Key
- Click on the service account you just created
- Go to "KEYS" tab
- Click "ADD KEY" > "Create new key"
- Select "JSON" and click "CREATE"
- A file will download - **save it as `credentials.json` in your project folder**

### 1e. Share Google Sheet with Service Account
- Open your Google Sheet in Google Sheets
- Click "Share" (top right)
- Copy this email from your `credentials.json` file: the value of `"client_email"`
- Paste it in the Share dialog
- Give it "Editor" access
- Uncheck "Notify people"

---

## Step 2: Extract Your Google Sheet ID

1. Open your Google Sheet in Google Sheets
2. Look at the URL in your browser - it looks like:
   ```
   https://docs.google.com/spreadsheets/d/1a2B3c4D5e6F7g8H9i0J/edit
   ```
3. Copy the long ID (the part between `/d/` and `/edit`):
   ```
   1a2B3c4D5e6F7g8H9i0J
   ```
   This is your `GOOGLE_SHEETS_ID`

---

## Step 3: Set Up Your Sheet Tabs

### 3a. Create "users" Tab
1. In Google Sheets, look for the sheet tabs at the bottom
2. Right-click on the default sheet and rename it to `users`
3. Add these column headers in the first row:
   - A1: `username`
   - B1: `email`
   - C1: `password`
   - D1: `profile_picture`

Your sheet should look like:
```
| username | email | password | profile_picture |
|----------|-------|----------|-----------------|
|          |       |          |                 |
```

---

## Step 4: Install Dependencies

In your terminal/command prompt, run:
```bash
pip install google-auth-oauthlib google-auth-httplib2 google-api-python-client
```

This installs the libraries your app needs to talk to Google Sheets.

---

## Step 5: Configure Your App

### 5a. Set Environment Variable (Local Testing)

**On Windows (PowerShell):**
```powershell
$env:GOOGLE_SHEETS_ID = "your-sheet-id-here"
```

**On Windows (Command Prompt):**
```cmd
set GOOGLE_SHEETS_ID=your-sheet-id-here
```

**On Mac/Linux:**
```bash
export GOOGLE_SHEETS_ID="your-sheet-id-here"
```

Replace `your-sheet-id-here` with the actual ID you copied in Step 2.

### 5b. Place credentials.json
Make sure the `credentials.json` file you downloaded is in your project root folder (same level as `app.py`).

---

## Step 6: Test Your Integration

### 6a. Run Your App
```bash
python app.py
```

### 6b. Check the Console Output
You should see:
```
✓ Successfully authenticated with Google Sheets
✓ Saved to Google Sheets
```

### 6c. Register a New User
1. Go to http://localhost:5000
2. Click "Register"
3. Register with a test account
4. Check your Google Sheet - the new user should appear!

---

## Step 7: Understanding What You're Using

### `google_sheets_manager.py`
This file contains the `GoogleSheetsManager` class which handles:
- **Authentication**: Logging in with your service account
- **Reading**: Fetching user data from Google Sheets
- **Writing**: Saving/updating user data
- **Appending**: Adding new rows

### How `app.py` Uses It
```python
# Load users
users = gs_manager.read_data('users')

# Save users  
gs_manager.write_data(users, 'users')

# Add a new user
gs_manager.append_row(new_user, 'users')
```

### Fallback to JSON
If Google Sheets fails (no internet, invalid credentials), your app automatically saves to `users.json` instead. This ensures your app never crashes.

---

## Troubleshooting

### Error: "Could not find credentials.json"
- Make sure the file is in the same folder as `app.py`
- Check the filename is exactly `credentials.json`

### Error: "Invalid GOOGLE_SHEETS_ID"
- Copy the Sheet ID again from your URL
- Make sure you set the environment variable before running `app.py`

### Error: "Permission denied"
- Make sure you shared the Google Sheet with the service account email
- Check that you gave it "Editor" access

### No error but data isn't syncing
- Check your internet connection
- Look for error messages in the console
- Try refreshing your Google Sheet in a browser

---

## Next Steps: Adding More Sheets

You can add more tabs for different data:
- `items` - Lost and found items
- `claims` - Claims made by users
- `tags` - Item tags

Just create new tabs in your Google Sheet and use:
```python
gs_manager.read_data('items')
gs_manager.write_data(items_list, 'items')
```

---

## Security Notes

- **Never share your `credentials.json` file publicly**
- Never commit it to GitHub
- Add it to `.gitignore`:
  ```
  credentials.json
  ```
- Only share your Google Sheet with people you trust

---

## Questions?

Check the comments in:
- `google_sheets_manager.py` - How the API works
- `app.py` - How it's integrated
