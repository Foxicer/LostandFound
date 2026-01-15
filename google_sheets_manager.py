import json
import os
import base64
from google.auth.transport.requests import Request
from google.oauth2.service_account import Credentials
from google.auth.exceptions import GoogleAuthError
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError

class GoogleSheetsManager:
    """Manages reading and writing to Google Sheets"""
    
    def __init__(self, credentials_file='credentials.json', spreadsheet_id=None):
        """
        Initialize the Google Sheets manager
        
        Args:
            credentials_file: Path to your service account JSON key
            spreadsheet_id: The ID of your Google Sheet (found in the URL)
        """
        self.credentials_file = credentials_file
        self.spreadsheet_id = spreadsheet_id
        self.service = None
        self._authenticate()
    
    def _authenticate(self):
        """Authenticate with Google using service account credentials"""
        try:
            # Define the scope (what this app is allowed to do)
            SCOPES = ['https://www.googleapis.com/auth/spreadsheets']
            
            # Load credentials from the JSON file
            credentials = Credentials.from_service_account_file(
                self.credentials_file,
                scopes=SCOPES
            )
            
            # Create the Sheets API service
            self.service = build('sheets', 'v4', credentials=credentials)
            print("✓ Successfully authenticated with Google Sheets")
            
        except FileNotFoundError:
            raise FileNotFoundError(f"Credentials file not found: {self.credentials_file}")
        except Exception as e:
            raise Exception(f"Authentication failed: {str(e)}")
    
    def read_data(self, sheet_name='users', range_notation=None):
        """
        Read data from a sheet
        
        Args:
            sheet_name: Name of the sheet tab (e.g., 'users')
            range_notation: Specific range like 'A1:D100' (if None, reads all data)
        
        Returns:
            List of dictionaries where each dict is a row
        """
        try:
            if not self.service or not self.spreadsheet_id:
                raise ValueError("Not properly initialized. Check credentials and spreadsheet_id")
            
            # Build the range to read
            if range_notation:
                range_to_read = f"{sheet_name}!{range_notation}"
            else:
                range_to_read = f"{sheet_name}"
            
            # Get the data
            result = self.service.spreadsheets().values().get(
                spreadsheetId=self.spreadsheet_id,
                range=range_to_read
            ).execute()
            
            rows = result.get('values', [])
            
            if not rows:
                return []
            
            # First row is headers
            headers = rows[0]
            # Convert remaining rows to dictionaries
            data = []
            for row in rows[1:]:
                # Pad row with empty strings if needed
                row_data = row + [''] * (len(headers) - len(row))
                data.append({headers[i]: row_data[i] for i in range(len(headers))})
            
            return data
        
        except HttpError as e:
            print(f"Error reading from Google Sheets: {e}")
            return []
        except Exception as e:
            print(f"Unexpected error: {e}")
            return []
    
    def write_data(self, data, sheet_name='users'):
        """
        Overwrite all data in a sheet
        
        Args:
            data: List of dictionaries to write
            sheet_name: Name of the sheet tab
        """
        try:
            if not self.service or not self.spreadsheet_id:
                raise ValueError("Not properly initialized")
            
            if not data:
                raise ValueError("No data to write")
            
            # Extract headers from first dictionary
            headers = list(data[0].keys())
            
            # Build rows: headers + data rows
            rows = [headers]
            for item in data:
                row = [item.get(header, '') for header in headers]
                rows.append(row)
            
            # Write to sheet
            body = {'values': rows}
            result = self.service.spreadsheets().values().update(
                spreadsheetId=self.spreadsheet_id,
                range=f"{sheet_name}!A1",
                valueInputOption='RAW',
                body=body
            ).execute()
            
            print(f"✓ Updated {result.get('updatedRows')} rows in Google Sheets")
            return True
        
        except Exception as e:
            print(f"Error writing to Google Sheets: {e}")
            return False
    
    def ensure_sheet_exists(self, sheet_name, headers):
        """
        Ensure a sheet exists with the proper headers. Creates if missing.
        
        Args:
            sheet_name: Name of the sheet tab
            headers: List of header column names
        """
        try:
            # Try to read the sheet
            existing_data = self.read_data(sheet_name)
            if existing_data and len(existing_data) > 0:
                print(f"✓ Sheet '{sheet_name}' already exists")
                return True
            elif existing_data is not None:
                # Sheet exists but is empty, add headers
                body = {'values': [headers]}
                self.service.spreadsheets().values().update(
                    spreadsheetId=self.spreadsheet_id,
                    range=f"{sheet_name}!A1",
                    valueInputOption='RAW',
                    body=body
                ).execute()
                print(f"✓ Added headers to '{sheet_name}'")
                return True
        except:
            pass
        
        # Sheet doesn't exist, create it with headers
        try:
            body = {'values': [headers]}
            self.service.spreadsheets().values().update(
                spreadsheetId=self.spreadsheet_id,
                range=f"{sheet_name}!A1",
                valueInputOption='RAW',
                body=body
            ).execute()
            print(f"✓ Created sheet '{sheet_name}' with headers")
            return True
        except Exception as e:
            print(f"Error ensuring sheet exists: {e}")
            return False
    
    def append_row(self, row_data, sheet_name='users'):
        """
        Append a single row to the sheet
        
        Args:
            row_data: Dictionary representing one row
            sheet_name: Name of the sheet tab
        """
        try:
            if not self.service or not self.spreadsheet_id:
                raise ValueError("Not properly initialized")
            
            # Get existing data to extract headers
            existing_data = self.read_data(sheet_name)
            if not existing_data:
                raise ValueError("Sheet is empty, use write_data first")
            
            # Get headers from first row
            headers = list(existing_data[0].keys())
            
            # Build row in correct order
            row = [row_data.get(header, '') for header in headers]
            
            # Append to sheet
            body = {'values': [row]}
            result = self.service.spreadsheets().values().append(
                spreadsheetId=self.spreadsheet_id,
                range=f"{sheet_name}!A2",
                valueInputOption='RAW',
                body=body
            ).execute()
            
            print(f"✓ Appended row to Google Sheets")
            return True
        
        except Exception as e:
            print(f"Error appending to Google Sheets: {e}")
            return False
    
    def log_captured_image(self, filename, user_name, tags, file_path, timestamp=None, image_data=None):
        """
        Log a captured image to the 'images' sheet
        
        Args:
            filename: Name of the image file
            user_name: Name of user who captured it
            tags: Tags for the image (comma-separated string)
            file_path: Local file path to the image
            timestamp: Capture timestamp (auto-generated if not provided)
            image_data: Optional base64-encoded image data to store in sheets
        
        Returns:
            True if successful, False otherwise
        """
        import datetime
        try:
            if not timestamp:
                timestamp = datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')
            
            image_record = {
                'filename': filename,
                'capture_timestamp': timestamp,
                'user_name': user_name,
                'tags': tags,
                'status': 'active',
                'file_path': file_path
            }
            
            # If base64 image data is provided, store it
            if image_data:
                image_record['image_data'] = image_data
                print(f"✓ Will store base64 image data in Google Sheets ({len(image_data)} chars)")
            
            return self.append_row(image_record, 'images')
        
        except Exception as e:
            print(f"Error logging captured image: {e}")
            return False
    
    def encode_image_to_base64(self, file_path):
        """
        Encode an image file to base64 string
        
        Args:
            file_path: Path to the image file
        
        Returns:
            Base64-encoded string of the image, or None if error
        """
        try:
            with open(file_path, 'rb') as image_file:
                return base64.b64encode(image_file.read()).decode('utf-8')
        except Exception as e:
            print(f"Error encoding image to base64: {e}")
            return None
    
    def decode_image_from_base64(self, base64_string, output_path):
        """
        Decode a base64 string and save it as an image file
        
        Args:
            base64_string: Base64-encoded image data
            output_path: Path where to save the decoded image
        
        Returns:
            True if successful, False otherwise
        """
        try:
            image_data = base64.b64decode(base64_string)
            with open(output_path, 'wb') as image_file:
                image_file.write(image_data)
            return True
        except Exception as e:
            print(f"Error decoding image from base64: {e}")
            return False
    
    def save_user_with_image(self, user_data, image_path=None):
        """
        Save a user record with optional profile picture encoded as base64
        
        Args:
            user_data: Dictionary containing user information
            image_path: Optional path to profile picture to encode and store
        
        Returns:
            True if successful, False otherwise
        """
        try:
            # If an image path is provided, encode it and add to user data
            if image_path and os.path.exists(image_path):
                base64_image = self.encode_image_to_base64(image_path)
                if base64_image:
                    user_data['profile_picture'] = base64_image
                    print(f"✓ Encoded profile picture ({len(base64_image)} chars)")
            
            # Get all users and update/add this user
            users = self.read_data('users')
            if users is None:
                users = []
            
            # Find and update existing user, or add new one
            email = user_data.get('email', '')
            found = False
            for i, user in enumerate(users):
                if user.get('email', '').lower() == email.lower():
                    users[i] = user_data
                    found = True
                    break
            
            if not found:
                users.append(user_data)
            
            # Write back to sheets
            return self.write_data(users, 'users')
        
        except Exception as e:
            print(f"Error saving user with image: {e}")
            return False
