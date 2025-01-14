@echo off
set PGUSER=arisa
set PGPASSWORD=arisa
set PGHOST=localhost
set PGPORT=5432
set DBNAME=SqueegeeManagement
set BACKUP_DIR=C:\backup

:: バックアップディレクトリが存在しない場合は作成
if not exist "%BACKUP_DIR%" mkdir "%BACKUP_DIR%"

:: 日付付きファイル名を生成 (不正文字を除去)
for /f "tokens=2 delims= " %%a in ('date /t') do set DATE=%%a
for /f "tokens=1 delims=:" %%a in ('time /t') do set TIME=%%a
set TIMESTAMP=%DATE:~0,4%%DATE:~5,2%%DATE:~8,2%_%TIME:~0,2%%TIME:~3,2%
set FILENAME=%BACKUP_DIR%\backup_%TIMESTAMP%.sql

:: バックアップ実行
"C:\Program Files\PostgreSQL\17\bin\pg_dump.exe" -F c -b -v -f "%FILENAME%" %DBNAME% 2>> "%BACKUP_DIR%\error.log"

:: エラーチェック
if %errorlevel% neq 0 (
    echo バックアップに失敗しました。詳細は error.log を確認してください。
    pause
    exit /b %errorlevel%
) else (
    echo バックアップ成功: %FILENAME%
    pause
)
