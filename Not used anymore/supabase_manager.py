import os
import base64
import datetime
from supabase import create_client, Client

class SupabaseManager:
    """Manages reading and writing to Supabase PostgreSQL database"""
    
    def __init__(self, url=None, key=None):
        """
        Initialize the Supabase manager
        
        Args:
            url: Supabase project URL (from environment or parameter)
            key: Supabase API key (from environment or parameter)
        """
        self.url = url or os.environ.get('SUPABASE_URL')
        self.key = key or os.environ.get('SUPABASE_KEY')
        
        if not self.url or not self.key:
            raise ValueError("SUPABASE_URL and SUPABASE_KEY environment variables are required")
        
        self.client: Client = create_client(self.url, self.key)
        print(f"✓ Supabase connected: {self.url}")
    
    def read_data(self, table_name='users', where_clause=None, where_value=None):
        """
        Read data from a table
        
        Args:
            table_name: Table name ('users' or 'images')
            where_clause: Optional column to filter on (e.g., 'email')
            where_value: Value to match in where_clause
        
        Returns:
            List of dictionaries where each dict is a row
        """
        try:
            query = self.client.table(table_name).select('*')
            
            if where_clause and where_value:
                query = query.eq(where_clause, where_value)
            
            response = query.execute()
            return response.data if response.data else []
        
        except Exception as e:
            print(f"Error reading from Supabase: {e}")
            return []
    
    def write_data(self, data, table_name='users'):
        """
        Overwrite all data in a table
        
        Args:
            data: List of dictionaries to write
            table_name: Table name
        """
        try:
            if not data:
                raise ValueError("No data to write")
            
            # Delete all existing rows
            self.client.table(table_name).delete().neq('id', 0).execute()
            
            # Insert new data
            response = self.client.table(table_name).insert(data).execute()
            
            print(f"✓ Updated {len(data)} rows in Supabase")
            return True
        
        except Exception as e:
            print(f"Error writing to Supabase: {e}")
            return False
    
    def append_row(self, row_data, table_name='users'):
        """
        Append a single row to the table
        
        Args:
            row_data: Dictionary representing one row
            table_name: Table name
        """
        try:
            response = self.client.table(table_name).insert(row_data).execute()
            print(f"✓ Appended row to Supabase table '{table_name}'")
            return True
        
        except Exception as e:
            print(f"Error appending to Supabase: {e}")
            return False
    
    def update_row(self, row_id, row_data, table_name='users'):
        """
        Update a single row by ID
        
        Args:
            row_id: ID of the row to update
            row_data: Dictionary with fields to update
            table_name: Table name
        """
        try:
            response = self.client.table(table_name).update(row_data).eq('id', row_id).execute()
            print(f"✓ Updated row in Supabase table '{table_name}'")
            return True
        
        except Exception as e:
            print(f"Error updating Supabase: {e}")
            return False
    
    def log_captured_image(self, filename, user_name, tags, file_path, timestamp=None, image_data=None):
        """
        Log a captured image to the 'images' table
        
        Args:
            filename: Name of the image file
            user_name: Name of user who captured it
            tags: Tags for the image (comma-separated string)
            file_path: Local file path to the image
            timestamp: Capture timestamp (auto-generated if not provided)
            image_data: Optional base64-encoded image data to store
        
        Returns:
            True if successful, False otherwise
        """
        try:
            if not timestamp:
                timestamp = datetime.datetime.now().isoformat()
            
            image_record = {
                'filename': filename,
                'capture_timestamp': timestamp,
                'user_name': user_name,
                'tags': tags,
                'status': 'active',
                'file_path': file_path,
                'image_data': image_data or ''
            }
            
            if image_data:
                print(f"✓ Will store base64 image data in Supabase ({len(image_data)} chars)")
            
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
            
            # Check if user exists (by email)
            email = user_data.get('email', '')
            existing = self.read_data('users', 'email', email)
            
            if existing:
                # Update existing user
                user_id = existing[0]['id']
                return self.update_row(user_id, user_data, 'users')
            else:
                # Insert new user
                return self.append_row(user_data, 'users')
        
        except Exception as e:
            print(f"Error saving user with image: {e}")
            return False
