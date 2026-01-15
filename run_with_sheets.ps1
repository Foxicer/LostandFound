# PowerShell script to run Flask with Google Sheets environment variable
# Usage: .\run_with_sheets.ps1 -SheetID "your-spreadsheet-id-here"

param(
    [Parameter(Mandatory=$true)]
    [string]$SheetID,
    
    [Parameter(Mandatory=$false)]
    [string]$SecretKey = "dev-secret-key-for-local-testing-only"
)

Write-Host "Setting up environment variables..." -ForegroundColor Cyan
$env:GOOGLE_SHEETS_ID = $SheetID
$env:FLASK_SECRET_KEY = $SecretKey

Write-Host "Environment variables set:" -ForegroundColor Green
Write-Host "  GOOGLE_SHEETS_ID = $env:GOOGLE_SHEETS_ID"
Write-Host "  FLASK_SECRET_KEY = (set)"
Write-Host ""
Write-Host "Starting Flask application..." -ForegroundColor Cyan
Write-Host ""

python app.py
