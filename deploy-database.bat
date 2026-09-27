@echo off
setlocal enabledelayedexpansion

echo ===============================================================================
echo                SDK SOLUTIONS ERP - DATABASE DEPLOYMENT TOOL
echo ===============================================================================
echo.
echo This utility creates the [SDK_ERP_DB] database on Microsoft SQL Server,
echo builds all 61 required tables, schemas, relations, and foundational seed data.
echo.

:: 1. Check for sqlcmd availability
where sqlcmd >nul 2>nul
if %errorlevel% neq 0 (
    echo [ERROR] 'sqlcmd' command-line utility was not found in your system PATH.
    echo Please install Microsoft Command Line Utilities for SQL Server or SSMS,
    echo or open the .sql script manually in SQL Server Management Studio (SSMS).
    echo.
    pause
    exit /b 1
)

:: 2. Choose SQL Server Instance
echo Select target SQL Server instance:
echo   1. LocalDB instance           [(localdb)\mssqllocaldb]
echo   2. SQL Server Express         [.\SQLEXPRESS]
echo   3. Local Default Instance     [localhost]
echo   4. Custom server name or IP
echo.
set /p INST_CHOICE="Enter choice (1-4) [Default: 1]: "
if "%INST_CHOICE%"=="" set INST_CHOICE=1

if "%INST_CHOICE%"=="1" set DB_SERVER=(localdb)\mssqllocaldb
if "%INST_CHOICE%"=="2" set DB_SERVER=.\SQLEXPRESS
if "%INST_CHOICE%"=="3" set DB_SERVER=localhost
if "%INST_CHOICE%"=="4" (
    set /p DB_SERVER="Enter SQL Server instance name (e.g., SERVERNAME or SERVER\INSTANCE): "
)

echo.
echo Target Server: %DB_SERVER%
echo.

:: 3. Choose Script to Execute
echo Select deployment package:
echo   1. Complete Database with Working Data [Recommended for Client Demo / Trial]
echo      (Includes full schema + multi-tenant company, admin users, tax rates, demo clients/projects)
echo.
echo   2. Fresh Production Database
echo      (Includes full schema + empty clean masters + default 'admin' user)
echo.
set /p SCRIPT_CHOICE="Enter choice (1 or 2) [Default: 1]: "
if "%SCRIPT_CHOICE%"=="" set SCRIPT_CHOICE=1

if "%SCRIPT_CHOICE%"=="1" (
    set SCRIPT_FILE=SDK_ERP_Database_Complete.sql
) else (
    set SCRIPT_FILE=SDK_ERP_Database_Fresh_Install.sql
)

if not exist "%~dp0%SCRIPT_FILE%" (
    echo [ERROR] Script file "%SCRIPT_FILE%" not found in %~dp0
    pause
    exit /b 1
)

echo.
echo Target Script: %SCRIPT_FILE%
echo.

:: 4. Choose Authentication
echo Select Authentication Method:
echo   1. Windows Authentication (Current Windows User)
echo   2. SQL Server Authentication (sa or specific SQL login)
echo.
set /p AUTH_CHOICE="Enter choice (1 or 2) [Default: 1]: "
if "%AUTH_CHOICE%"=="" set AUTH_CHOICE=1

if "%AUTH_CHOICE%"=="1" (
    set AUTH_FLAGS=-E
) else (
    set /p SQL_USER="Enter SQL username: "
    set /p SQL_PASS="Enter SQL password: "
    set AUTH_FLAGS=-U !SQL_USER! -P !SQL_PASS!
)

echo.
echo Executing deployment script against %DB_SERVER%...
echo -------------------------------------------------------------------------------
sqlcmd -S "%DB_SERVER%" %AUTH_FLAGS% -C -b -i "%~dp0%SCRIPT_FILE%"

if %errorlevel% neq 0 (
    echo.
    echo [FAILURE] The database deployment script encountered an error.
    echo Please review the error message printed above.
    echo.
    pause
    exit /b %errorlevel%
)

echo.
echo -------------------------------------------------------------------------------
echo [SUCCESS] Database [SDK_ERP_DB] successfully deployed to %DB_SERVER%!
echo.
echo Quick Verification:
sqlcmd -S "%DB_SERVER%" %AUTH_FLAGS% -C -d "SDK_ERP_DB" -Q "SET NOCOUNT ON; SELECT 'Total Tables Created:' AS Info, count(*) AS Count FROM sys.tables UNION ALL SELECT 'Total Companies:' AS Info, count(*) FROM [admin].[companies] UNION ALL SELECT 'Total Active Users:' AS Info, count(*) FROM [admin].[users];"
echo.
echo Next Steps on Client Machine:
echo 1. Ensure src\SDK.ERP.Web\appsettings.json ConnectionStrings:DefaultConnection
echo    points to: Server=%DB_SERVER%;Database=SDK_ERP_DB;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True
echo 2. Launch the ERP application with: dotnet run --project src\SDK.ERP.Web
echo 3. Login Credentials:
echo    Username: admin
echo    Password: Admin@123  (or admin123)
echo ===============================================================================
pause
