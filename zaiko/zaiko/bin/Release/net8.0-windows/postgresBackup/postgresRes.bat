@echo off
:: 環境変数の設定
set PGUSER=arisa
set PGPASSWORD=arisa
set PGHOST=localhost
set PGPORT=5432
set DBNAME=SqueegeeManagement

:: バッチファイルのディレクトリを基準に相対パスを設定
set SCRIPT_DIR=%~dp0
set BACKUP_FILE=%SCRIPT_DIR%backup_20250113_19.sql
set LOG_FILE=%SCRIPT_DIR%restore_error.log

:: デバッグ用出力（確認用）
echo SCRIPT_DIR: "%SCRIPT_DIR%"
echo BACKUP_FILE: "%BACKUP_FILE%"

:: ファイルの存在確認
if not exist "%BACKUP_FILE%" (
    echo エラー: バックアップファイル "%BACKUP_FILE%" が存在しません。
    exit /b 1
)

:: リストア実行
"C:\Program Files\PostgreSQL\17\bin\pg_restore.exe" --clean -d %DBNAME% -U %PGUSER% -h %PGHOST% -p %PGPORT% -v "%BACKUP_FILE%" 2>> "%LOG_FILE%"

:: エラーチェック
if %errorlevel% neq 0 (
    echo リストアに失敗しました。詳細は "%LOG_FILE%" を確認してください。
    exit /b %errorlevel%
) else (
    echo リストア成功: "%BACKUP_FILE%"
    exit /b 0
)
