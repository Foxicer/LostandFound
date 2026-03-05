import os
import re
from pathlib import Path

root = Path("c:\\Users\\jayde\\Downloads\\LostandFound")

# Remove helper scripts
for helper_file in ["remove_comments.py", "strip_docstrings.py"]:
    file_path = root / helper_file
    if file_path.exists():
        file_path.unlink()
        print(f"✓ Deleted: {helper_file}")

# Clean .drawio files
for drawio_file in root.rglob("*.drawio"):
    content = drawio_file.read_text(encoding='utf-8')
    original_len = len(content)
    
    # Remove all HTML comments
    content = re.sub(r'<!--\s*[^-]*(?:-(?!-)[^-]*)*-->\s*\n?', '', content)
    
    if len(content) != original_len:
        drawio_file.write_text(content, encoding='utf-8')
        print(f"✓ Cleaned: {drawio_file.name}")

print("Cleanup complete!")
