@echo off
:: 作成日: 2025年1月14日17:00
:: 環境変数の設定
set PGUSER=arisa
set PGPASSWORD=arisa
set PGHOST=localhost
set PGPORT=5432
set DBNAME=SqueegeeManagement

:: バッチファイルのあるディレクトリをリストアファイルおよびログファイルのディレクトリとして設定
set SCRIPT_DIR=%~dp0
set BACKUP_FILE=%SCRIPT_DIR%backup_20250113_19.sql
set LOG_FILE=%SCRIPT_DIR%restore_error.log

:: リストア実行
"C:\Program Files\PostgreSQL\17\bin\pg_restore.exe" --clean -d %DBNAME% -U %PGUSER% -h %PGHOST% -p %PGPORT% -v "%BACKUP_FILE%" 2>> "%LOG_FILE%"

:: エラーチェック
if %errorlevel% neq 0 (
    echo リストアに失敗しました。詳細は restore_error.log を確認してください。
    timeout /t 5 >nul
    exit /b %errorlevel%
) else (
    echo リストア成功: %BACKUP_FILE%
    timeout /t 3 >nul
    exit 0
)