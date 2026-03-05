import os
import re
from pathlib import Path

root_dir = "c:\\Users\\jayde\\Downloads\\LostandFound"

files_processed = 0

# Process .html files
for html_file in list(Path(root_dir).rglob('*.html')):
    try:
        content = html_file.read_text(encoding='utf-8')
        original_len = len(content)
        
        # Remove /* */ comments
        content = re.sub(r'/\*[^*]*\*+(?:[^/*][^*]*\*+)*/', '', content)
        
        # Remove <!-- --> comments
        content = re.sub(r'<!--[^-]*(?:-(?!-)[^-]*)*-->', '', content)
        
        # Clean up extra newlines
        content = re.sub(r'(\r?\n){3,}', '\r\n\r\n', content)
        
        if len(content) != original_len:
            html_file.write_text(content, encoding='utf-8')
            print(f"✓ Cleaned: {html_file.name}")
            files_processed += 1
    except Exception as e:
        print(f"✗ Error processing {html_file.name}: {e}")

# Process .py files (remove docstrings)
for py_file in Path(root_dir).rglob('*.py'):
    if '__pycache__' in str(py_file):
        continue
    try:
        content = py_file.read_text(encoding='utf-8')
        original_len = len(content)
        
        # Remove docstrings (triple quotes)
        content = re.sub(r'"""[\s\S]*?"""', '', content)
        content = re.sub(r"'''[\s\S]*?'''", '', content)
        
        # Clean up extra blank lines introduced by removal
        content = re.sub(r'\n\n\n+', '\n\n', content)
        
        if len(content) != original_len:
            py_file.write_text(content, encoding='utf-8')
            print(f"✓ Cleaned: {py_file.name}")
            files_processed += 1
    except Exception as e:
        print(f"✗ Error processing {py_file.name}: {e}")

# Process .js files (remove /* */ and // style comments)
for js_file in Path(root_dir).rglob('*.js'):
    try:
        content = js_file.read_text(encoding='utf-8')
        original_len = len(content)
        
        # Remove /* */ comments
        content = re.sub(r'/\*[^*]*\*+(?:[^/*][^*]*\*+)*/', '', content)
        
        # Clean up extra newlines
        content = re.sub(r'(\r?\n){3,}', '\r\n\r\n', content)
        
        if len(content) != original_len:
            js_file.write_text(content, encoding='utf-8')
            print(f"✓ Cleaned: {js_file.name}")
            files_processed += 1
    except Exception as e:
        print(f"✗ Error processing {js_file.name}: {e}")

# Process .resx files (XML files with comments)
for resx_file in Path(root_dir).rglob('*.resx'):
    try:
        content = resx_file.read_text(encoding='utf-8')
        original_len = len(content)
        
        # Remove <!-- --> comments
        content = re.sub(r'<!--[^-]*(?:-(?!-)[^-]*)*-->', '', content)
        
        # Clean up extra newlines
        content = re.sub(r'(\r?\n){3,}', '\r\n\r\n', content)
        
        if len(content) != original_len:
            resx_file.write_text(content, encoding='utf-8')
            print(f"✓ Cleaned: {resx_file.name}")
            files_processed += 1
    except Exception as e:
        print(f"✗ Error processing {resx_file.name}: {e}")

# Process .drawio files (XML with comments)
for drawio_file in Path(root_dir).rglob('*.drawio'):
    try:
        content = drawio_file.read_text(encoding='utf-8')
        original_len = len(content)
        
        # Remove <!-- --> comments
        content = re.sub(r'<!--[^-]*(?:-(?!-)[^-]*)*-->', '', content)
        
        if len(content) != original_len:
            drawio_file.write_text(content, encoding='utf-8')
            print(f"✓ Cleaned: {drawio_file.name}")
            files_processed += 1
    except Exception as e:
        print(f"✗ Error processing {drawio_file.name}: {e}")

print(f"\nTotal files processed: {files_processed}")

