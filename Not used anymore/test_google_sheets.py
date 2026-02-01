"""
Test script to verify Google Sheets integration
Run this to debug connection issues
"""

import os
from google_sheets_manager import GoogleSheetsManager

# Check environment
SPREADSHEET_ID = os.environ.get('GOOGLE_SHEETS_ID')
CREDENTIALS_FILE = 'credentials.json'

print("=" * 50)
print("GOOGLE SHEETS INTEGRATION TEST")
print("=" * 50)

print(f"\n1. Checking credentials.json...")
if os.path.exists(CREDENTIALS_FILE):
    print(f"   ✓ Found: {CREDENTIALS_FILE}")
else:
    print(f"   ✗ NOT FOUND: {CREDENTIALS_FILE}")
    print("   Make sure credentials.json is in the current directory!")
    exit(1)

print(f"\n2. Checking GOOGLE_SHEETS_ID environment variable...")
if SPREADSHEET_ID:
    print(f"   ✓ Found: {SPREADSHEET_ID}")
else:
    print(f"   ✗ NOT SET!")
    print("   Run this first:")
    print("   $env:GOOGLE_SHEETS_ID = '1DVU0dggyukStQyidy7SYaSyoVmE5Z5gkALYS5pLGjQ4'")
    exit(1)

print(f"\n3. Attempting to authenticate...")
try:
    gs = GoogleSheetsManager(CREDENTIALS_FILE, SPREADSHEET_ID)
    print(f"   ✓ Authentication successful!")
except Exception as e:
    print(f"   ✗ Authentication failed: {e}")
    import traceback
    traceback.print_exc()
    exit(1)

print(f"\n4. Attempting to read data from 'users' sheet...")
try:
    data = gs.read_data('users')
    print(f"   ✓ Read successful! Found {len(data)} rows")
    if data:
        print(f"   Sample row: {data[0]}")
except Exception as e:
    print(f"   ✗ Read failed: {e}")
    import traceback
    traceback.print_exc()
    exit(1)

print(f"\n5. Attempting to write test data...")
try:
    test_user = {
        'username': 'testuser',
        'email': 'test@example.com',
        'password': 'test123',
        'profile_picture': ''
    }
    gs.append_row(test_user, 'users')
    print(f"   ✓ Write successful!")
except Exception as e:
    print(f"   ✗ Write failed: {e}")
    import traceback
    traceback.print_exc()
    exit(1)

print("\n" + "=" * 50)
print("ALL TESTS PASSED! ✓")
print("=" * 50)
