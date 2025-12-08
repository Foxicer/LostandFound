# ReturnPoint - Lost and Found System

## Overview
ReturnPoint is a lost and found management system designed to help users report, track, and claim lost items. The project consists of two main components:

1. **Desktop Application (C# Windows Forms)**: A Windows desktop application for capturing photos, recording item information, and managing claims. This component is not runnable in the Replit Linux environment.

2. **Web Application (Flask)**: A web-based interface with user authentication (login/registration) and a dashboard for accessing the system.

## Current State
- The Flask web application is configured and ready to run on Replit
- User authentication system with JSON-based storage
- Basic login, registration, and dashboard pages
- The C# desktop application remains in the repository but is Windows-only

## Project Structure

### Web Application Files
- `app.py`: Flask backend server with API endpoints for login and registration
- `index.html`: Login page
- `register.html`: User registration page
- `dashboard.html`: Dashboard page (shown after successful login)
- `script.js`: Login page JavaScript
- `register.js`: Registration page JavaScript
- `styles.css`: Shared CSS styles
- `users.json`: User database (JSON file storage)

### Desktop Application Files (Windows-only)
- `ReturnPoint/`: C# Windows Forms application directory
  - Camera capture functionality
  - Item information forms
  - Gallery view
  - Claim management

## Features

### Web Application
- User registration with username, email, and password
- User login authentication
- Dashboard interface
- JSON-based user storage (requires persistent VM deployment)

### Desktop Application (Windows-only)
- Camera-based photo capture with countdown timer
- Item information input (location, date found)
- Photo gallery with item details
- Claim system for found items
- Claimant information management

## Technology Stack
- **Backend**: Python 3.11, Flask 3.1.2
- **Frontend**: HTML, CSS, JavaScript (vanilla)
- **Database**: JSON file storage (users.json)
- **Desktop App**: C# .NET 6.0, Windows Forms (not runnable on Replit)

## Recent Changes
- 2025-10-26: Added user profile features
  - Implemented Flask session-based authentication
  - Added profile editing (username change, profile picture upload)
  - Created protected dashboard with user information display
  - Added API endpoints for user management
- 2025-10-26: Initial Replit setup
  - Installed Python 3.11 and Flask
  - Configured Flask to run on 0.0.0.0:5000
  - Created project documentation

## Running the Application
The Flask server runs on port 5000 and serves the web interface. Users can:
1. Register a new account
2. Log in with their credentials
3. Access the dashboard after successful login

## User Preferences
None specified yet.

## Notes
- The C# desktop application is not compatible with Replit's Linux environment
- All web routes are served through Flask
- Static files (HTML, CSS, JS) are served from the root directory
- User data is stored in users.json file (requires VM deployment for persistence)
- Deployment uses VM mode to maintain file-based storage across restarts

## Security Considerations
- **Important**: Set FLASK_SECRET_KEY environment variable before deployment for secure session management
- User passwords are currently stored in plaintext (consider implementing password hashing for production use)
- Profile pictures are validated for file type (png, jpg, jpeg, gif only)
- All profile-related endpoints require authentication via Flask sessions
