# ReturnPoint System Documentation

## Overview

ReturnPoint is a comprehensive lost and found management system that combines a web-based user interface with a Windows desktop application for camera integration and image management. The system allows users to register, login, capture images using connected cameras, tag and organize found items, and manage the entire lost and found process.

## Architecture

### System Components

1. **Web Backend (Flask API)**
   - Built with Python Flask
   - Handles user authentication, registration, and data management
   - Supports both local JSON storage and Google Sheets integration
   - Provides RESTful API endpoints for all operations

2. **Web Frontend**
   - HTML/CSS/JavaScript based user interface
   - Responsive design for various screen sizes
   - User registration, login, and dashboard functionality

3. **Windows Desktop Application**
   - Built with C# .NET 6.0 Windows Forms
   - Camera integration using AForge.NET and OpenCV
   - Image capture, tagging, and gallery management
   - Role-based access (User/Admin)

4. **Avalonia UI Application**
   - Cross-platform desktop application
   - Alternative UI implementation using Avalonia framework

## Technology Stack

### Backend
- **Python 3.x**
- **Flask** - Web framework
- **Google Sheets API** - Cloud data storage
- **Werkzeug** - File handling utilities
- **JSON** - Local data storage fallback

### Frontend
- **HTML5**
- **CSS3** - Custom styling
- **JavaScript** - Client-side interactivity
- **Fetch API** - HTTP requests

### Desktop Application
- **C# .NET 6.0**
- **Windows Forms** - UI framework
- **AForge.NET** - Camera and video processing
- **OpenCV** - Computer vision operations
- **System.Drawing** - Image manipulation

### Cross-Platform Desktop
- **Avalonia UI** - Cross-platform XAML-based UI framework
- **.NET Core** - Runtime environment

## Project Structure

```
ReturnPoint/
├── app.py                          # Flask API server
├── google_sheets_manager.py        # Google Sheets integration
├── index.html                      # Login page
├── register.html                   # Registration page
├── dashboard.html                  # User dashboard
├── styles.css                      # Global styles
├── login.js                        # Login page JavaScript
├── users.json                      # Local user storage (fallback)
├── credentials.json                # Google API credentials
├── current_user_email.txt          # Current session user
├── uploads/                        # Profile picture uploads
├── ReturnPoint/                    # C# Desktop Application
│   ├── Program.cs                  # Application entry point
│   ├── FormLogin.cs                # Login form
│   ├── FormRegister.cs             # Registration form
│   ├── FormGallery.cs              # User gallery interface
│   ├── FormGalleryAdmin.cs         # Admin gallery interface
│   ├── FormCamera.cs               # Camera capture interface
│   ├── FormSelectCamera.cs         # Camera selection
│   ├── Theme.cs                    # UI theming utilities
│   ├── Models/
│   │   └── User.cs                 # User data model
│   └── bin/Debug/net6.0-windows/   # Build output
│       └── CapturedImages/         # Stored captured images
├── ReturnPoint.Avalonia/           # Cross-platform Avalonia app
│   ├── App.axaml                   # Application definition
│   ├── MainWindow.axaml            # Main window XAML
│   ├── Views/                      # View components
│   └── ViewModels/                 # MVVM view models
└── Not used anymore/               # Deprecated files
```

## API Endpoints

### Authentication
- `POST /api/login` - User login
- `POST /api/logout` - User logout
- `POST /api/register` - User registration

### User Management
- `GET /api/user` - Get current user information
- `POST /api/update-username` - Update user profile
- `POST /api/upload-profile-picture` - Upload profile picture

### Image Management
- `GET /api/images` - Get all captured images with metadata
- `GET /api/images/<filename>` - Get specific image file
- `GET /api/images/<filename>/info` - Get image metadata
- `POST /api/images/<filename>/info` - Update image metadata
- `POST /api/log-captured-image` - Log newly captured image
- `POST /api/delete-captured-image` - Mark image as deleted
- `POST /api/restore-captured-image` - Restore deleted image

## Data Models

### User Model
```csharp
public class User
{
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? GradeSection { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; } // "user" or "admin"
    public string? ProfilePicture { get; set; }
}
```

### Image Metadata
- **Filename**: Image file name
- **Tags**: Array of descriptive tags
- **Info**: Additional description/notes
- **Timestamp**: Capture timestamp
- **User**: Associated user information
- **Status**: Active/Deleted status

## Key Features

### User Authentication
- Email/password based authentication
- Gmail domain restriction for security
- Session management with Flask sessions
- Cross-application user context sharing

### Camera Integration
- Real-time camera preview
- Multiple camera device support
- Image capture with timestamp
- Automatic metadata generation

### Image Management
- Tag-based organization
- Search and filter functionality
- Admin approval workflow
- Soft delete with restore capability

### Role-Based Access
- **User Role**: Basic gallery viewing and personal image management
- **Admin Role**: Full system administration, user management, image approval

### Data Storage Options
- **Google Sheets**: Cloud-based storage for multi-device access
- **Local JSON**: Fallback storage for offline operation
- **File System**: Image storage with metadata files

## Configuration

### Environment Variables
- `FLASK_SECRET_KEY`: Session encryption key
- `GOOGLE_SHEETS_ID`: Google Sheets spreadsheet ID

### Google Sheets Setup
1. Create Google Cloud Project
2. Enable Google Sheets API
3. Create Service Account
4. Share spreadsheet with service account
5. Download credentials.json

### Local Development
- Run Flask app: `python app.py`
- Build C# app: `dotnet build`
- Run C# app: `dotnet run`

## Security Features

- Password hashing (basic implementation)
- Session-based authentication
- File upload validation
- SQL injection prevention through parameterized queries
- Cross-site request forgery protection

## UI/UX Design

### Theme System
- Consistent color scheme (Teal Green, Accent Blue, Dark Gray)
- Gradient backgrounds
- Flat UI design with subtle borders
- Responsive layout with scrollable panels

### Responsive Design
- Adaptive layouts for different screen sizes
- Scrollable content areas
- Touch-friendly button sizes
- Optimized for both desktop and tablet use

## Deployment

### Web Application
- Deployable to any Flask-compatible hosting (Heroku, DigitalOcean, etc.)
- Static file serving for frontend assets
- Environment variable configuration

### Desktop Application
- Self-contained .NET executable
- Windows installer creation
- Cross-platform Avalonia version for Linux/Mac

## Development Workflow

1. **User Registration**: Web interface for new user signup
2. **Authentication**: Login through web or desktop app
3. **Image Capture**: Desktop app connects to camera devices
4. **Metadata Entry**: Tag and describe captured items
5. **Gallery Management**: View, search, and organize images
6. **Admin Oversight**: Approve/reject submissions (admin role)

## Integration Points

- **Web ↔ Desktop**: Shared user sessions via file-based communication
- **Local ↔ Cloud**: Seamless switching between JSON and Google Sheets storage
- **Camera ↔ Storage**: Direct image capture to organized file system
- **UI ↔ API**: RESTful communication between frontend and backend

## Future Enhancements

- Mobile application development
- Advanced image recognition for automatic tagging
- Multi-language support
- Real-time notifications
- Advanced search and filtering
- Bulk image operations
- Export functionality (PDF reports, etc.)

## Troubleshooting

### Common Issues
- Camera device not detected: Check device permissions and drivers
- Google Sheets connection failed: Verify credentials and spreadsheet sharing
- Image upload errors: Check file size limits and format restrictions
- Session timeout: Implement automatic refresh or extend session duration

### Debug Information
- Flask app provides detailed console logging
- C# application includes error handling with user-friendly messages
- Google Sheets operations include success/failure feedback

## Contributing

### Code Standards
- C# naming conventions (PascalCase for classes, camelCase for variables)
- Python PEP 8 compliance
- HTML/CSS semantic structure
- Comprehensive error handling

### Testing
- Unit tests for API endpoints
- UI testing for form validation
- Integration tests for camera functionality
- Cross-platform compatibility testing

This documentation provides a comprehensive overview of the ReturnPoint system architecture, components, and functionality. The system is designed to be modular, scalable, and maintainable with clear separation of concerns between web and desktop components.
