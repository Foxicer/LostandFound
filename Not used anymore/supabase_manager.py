import os
import base64
import datetime
from supabase import create_client, Client

class SupabaseManager:

    def __init__(self, url=None, key=None):

        self.url = url or os.environ.get('SUPABASE_URL')
        self.key = key or os.environ.get('SUPABASE_KEY')
        
        if not self.url or not self.key:
            raise ValueError("SUPABASE_URL and SUPABASE_KEY environment variables are required")
        
        self.client: Client = create_client(self.url, self.key)
        print(f"✓ Supabase connected: {self.url}")
    
    def read_data(self, table_name='users', where_clause=None, where_value=None):
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
        try:
            response = self.client.table(table_name).insert(row_data).execute()
            print(f"✓ Appended row to Supabase table '{table_name}'")
            return True
        
        except Exception as e:
            print(f"Error appending to Supabase: {e}")
            return False
    
    def update_row(self, row_id, row_data, table_name='users'):
        try:
            response = self.client.table(table_name).update(row_data).eq('id', row_id).execute()
            print(f"✓ Updated row in Supabase table '{table_name}'")
            return True
        
        except Exception as e:
            print(f"Error updating Supabase: {e}")
            return False
    
    def log_captured_image(self, filename, user_name, tags, file_path, timestamp=None, image_data=None):
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
        try:
            with open(file_path, 'rb') as image_file:
                return base64.b64encode(image_file.read()).decode('utf-8')
        except Exception as e:
            print(f"Error encoding image to base64: {e}")
            return None
    
    def decode_image_from_base64(self, base64_string, output_path):
        try:
            image_data = base64.b64decode(base64_string)
            with open(output_path, 'wb') as image_file:
                image_file.write(image_data)
            return True
        except Exception as e:
            print(f"Error decoding image from base64: {e}")
            return False
    
    def save_user_with_image(self, user_data, image_path=None):
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
