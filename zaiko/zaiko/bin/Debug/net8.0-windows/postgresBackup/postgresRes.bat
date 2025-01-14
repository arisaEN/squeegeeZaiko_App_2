@echo off
set PGUSER=arisa
set PGPASSWORD=arisa
set PGHOST=localhost
set PGPORT=5432
set DBNAME=SqueegeeManagement

:: バッチファイルのあるディレクトリをバックアップファイルのディレクトリとして設定
set SCRIPT_DIR=%~dp0
set BACKUP_FILE=%SCRIPT_DIR%backup_20250113_19.sql

:: リストア実行
"C:\Program Files\PostgreSQL\17\bin\pg_restore.exe" --clean -d %DBNAME% -U %PGUSER% -h %PGHOST% -p %PGPORT% -v "%BACKUP_FILE%" 2>> "%SCRIPT_DIR%restore_error.log"

:: エラーチェック
if %errorlevel% neq 0 (
    echo リストアに失敗しました。詳細は restore_error.log を確認してください。
    exit /b %errorlevel%
) else (
    echo リストア成功: %BACKUP_FILE%
    exit 0
)
