import re
from pathlib import Path

# Process supabase_manager.py and any other Python files with docstrings
py_files = [
    Path("c:\\Users\\jayde\\Downloads\\LostandFound\\Not used anymore\\supabase_manager.py"),
]

for py_file in py_files:
    if py_file.exists():
        content = py_file.read_text(encoding='utf-8')
        original_len = len(content)
        
        # Remove all docstrings (triple quotes)
        content = re.sub(r'^\s*"""[\s\S]*?"""\s*$', '', content, flags=re.MULTILINE)
        content = re.sub(r"^\s*'''[\s\S]*?'''\s*$", '', content, flags=re.MULTILINE)
        
        # Clean up excess newlines
        content = re.sub(r'\n\n\n+', '\n\n', content)
        
        py_file.write_text(content, encoding='utf-8')
        print(f"✓ Stripped docstrings from {py_file.name} ({original_len} -> {len(content)} chars)")

print("Done!")
