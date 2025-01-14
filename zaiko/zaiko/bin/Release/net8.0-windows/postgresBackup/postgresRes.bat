@echo off
setlocal enabledelayedexpansion

:: 環境変数の設定
set PGUSER=arisa
set PGPASSWORD=arisa
set PGHOST=localhost
set PGPORT=5432
set DBNAME=SqueegeeManagement
set BACKUP_DIR=%~dp0
set BACKUP_FILE=%BACKUP_DIR%backup_20250113_19.sql
set LOG_FILE=%BACKUP_DIR%restore_error.log

:: 古いエラーログを削除
if exist "%LOG_FILE%" del "%LOG_FILE%"

:: データベース内のすべてのオブジェクトを削除
echo データベース内のすべてのオブジェクトを削除中...
"C:\Program Files\PostgreSQL\17\bin\psql.exe" -d %DBNAME% -U %PGUSER% -h %PGHOST% -p %PGPORT% -c "DROP SCHEMA public CASCADE;" >> "%LOG_FILE%" 2>&1
"C:\Program Files\PostgreSQL\17\bin\psql.exe" -d %DBNAME% -U %PGUSER% -h %PGHOST% -p %PGPORT% -c "CREATE SCHEMA public;" >> "%LOG_FILE%" 2>&1

if %errorlevel% neq 0 (
    echo オブジェクト削除に失敗しました。詳細は %LOG_FILE% を確認してください。
    exit /b %errorlevel%
)

:: リストア実行
echo リストア実行中...
"C:\Program Files\PostgreSQL\17\bin\pg_restore.exe" -d %DBNAME% -U %PGUSER% -h %PGHOST% -p %PGPORT% -v "%BACKUP_FILE%" >> "%LOG_FILE%" 2>&1

:: エラーチェック
if %errorlevel% neq 0 (
    echo リストアに失敗しました。詳細は %LOG_FILE% を確認してください。
    exit /b %errorlevel%
) else (
    echo リストア成功: %BACKUP_FILE%
)

:: スクリプト終了
exit /b 0