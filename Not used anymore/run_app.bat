@echo off
setlocal enabledelayedexpansion
REM ============================================
REM Lost and Found Application Launcher
REM Starts ReturnPoint desktop app + Flask web server
REM ============================================

echo.
echo ╔══════════════════════════════════════════════════════════╗
echo ║  Lost and Found Application Launcher                    ║
echo ║  ReturnPoint + Web Server with Supabase                 ║
echo ╚══════════════════════════════════════════════════════════╝
echo.

REM Set Supabase environment variables
echo Setting Supabase credentials...
set "SUPABASE_URL=https://ckbasyzrjicopduwcfqs.supabase.co"
set "SUPABASE_KEY=sb_publishable_RqSvyql9IdLQnQVO1r7JTQ_sTiezEW3"
echo ✓ SUPABASE_URL=!SUPABASE_URL!
echo ✓ SUPABASE_KEY=!SUPABASE_KEY!

echo.
echo Starting ReturnPoint desktop application...
REM Start the ReturnPoint shortcut
start "" "ReturnPoint.exe - Shortcut.lnk"
echo ✓ ReturnPoint launched

echo.
echo Starting Flask web server...
echo ✓ Web server will run on http://127.0.0.1:5000
echo.
echo ═══════════════════════════════════════════════════════════
echo  Press CTRL+C in this window to stop the web server
echo ═══════════════════════════════════════════════════════════
echo.

REM Run the Python Flask app
python app.py

pause
