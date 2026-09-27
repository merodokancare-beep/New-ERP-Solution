<#
.SYNOPSIS
    Deploy SDK Solutions ERP Database to any client Microsoft SQL Server instance.
.DESCRIPTION
    Automates the deployment of SDK_ERP_DB, schemas, tables, constraints, and foundational seeds.
.EXAMPLE
    .\Deploy-Database.ps1 -ServerInstance "localhost" -ScriptType "Complete"
    .\Deploy-Database.ps1 -ServerInstance ".\SQLEXPRESS" -ScriptType "Fresh"
#>

[CmdletBinding()]
param (
    [Parameter(Mandatory=$false)]
    [string]$ServerInstance = "(localdb)\mssqllocaldb",

    [Parameter(Mandatory=$false)]
    [ValidateSet("Complete", "Fresh")]
    [string]$ScriptType = "Complete",

    [Parameter(Mandatory=$false)]
    [switch]$UseSqlAuth,

    [Parameter(Mandatory=$false)]
    [string]$Username,

    [Parameter(Mandatory=$false)]
    [string]$Password
)

$ErrorActionPreference = "Stop"

Write-Host "===============================================================================" -ForegroundColor Cyan
Write-Host "                SDK SOLUTIONS ERP - DATABASE DEPLOYMENT TOOL (PowerShell)      " -ForegroundColor Cyan
Write-Host "===============================================================================" -ForegroundColor Cyan
Write-Host ""

$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path

$scriptFile = if ($ScriptType -eq "Complete") {
    Join-Path $scriptDirectory "SDK_ERP_Database_Complete.sql"
} else {
    Join-Path $scriptDirectory "SDK_ERP_Database_Fresh_Install.sql"
}

if (-not (Test-Path $scriptFile)) {
    Write-Error "Database script file not found at: $scriptFile"
    exit 1
}

Write-Host "Target SQL Server Instance : $ServerInstance" -ForegroundColor Yellow
Write-Host "Deployment Package         : $ScriptType ($scriptFile)" -ForegroundColor Yellow
Write-Host ""

$sqlCmdPath = (Get-Command sqlcmd -ErrorAction SilentlyContinue)?.Source
if (-not $sqlCmdPath) {
    Write-Error "The 'sqlcmd' utility is not found in your system PATH. Please install SQL Server Management Studio (SSMS) or SQL Command Line Tools, or execute the .sql script manually in SSMS."
    exit 1
}

$cmdArgs = @("-S", $ServerInstance, "-C", "-b", "-i", $scriptFile)

if ($UseSqlAuth) {
    if (-not $Username -or -not $Password) {
        $Username = Read-Host "Enter SQL Server Username"
        $Password = Read-Host -AsSecureString "Enter SQL Server Password"
        $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($Password)
        $PlainPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
    } else {
        $PlainPassword = $Password
    }
    $cmdArgs += @("-U", $Username, "-P", $PlainPassword)
} else {
    $cmdArgs += "-E"
}

Write-Host "Executing SQL script against $ServerInstance..." -ForegroundColor Green
$process = Start-Process -FilePath "sqlcmd" -ArgumentList $cmdArgs -NoNewWindow -Wait -PassThru

if ($process.ExitCode -eq 0) {
    Write-Host "`n[SUCCESS] Database [SDK_ERP_DB] successfully deployed to $ServerInstance!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Verification Query:" -ForegroundColor Cyan
    $verifyArgs = @("-S", $ServerInstance, "-C", "-d", "SDK_ERP_DB", "-Q", "SET NOCOUNT ON; SELECT 'Total Tables Created:' AS Info, count(*) AS Count FROM sys.tables UNION ALL SELECT 'Total Companies:' AS Info, count(*) FROM [admin].[companies] UNION ALL SELECT 'Total Active Users:' AS Info, count(*) FROM [admin].[users];")
    if ($UseSqlAuth) {
        $verifyArgs += @("-U", $Username, "-P", $PlainPassword)
    } else {
        $verifyArgs += "-E"
    }
    Start-Process -FilePath "sqlcmd" -ArgumentList $verifyArgs -NoNewWindow -Wait

    Write-Host ""
    Write-Host "Connection string setting for appsettings.json:" -ForegroundColor Yellow
    Write-Host "Server=$ServerInstance;Database=SDK_ERP_DB;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True" -ForegroundColor White
    Write-Host ""
    Write-Host "Default Login Credentials:" -ForegroundColor Green
    Write-Host "Username: admin" -ForegroundColor White
    Write-Host "Password: Admin@123  (or admin123)" -ForegroundColor White
} else {
    Write-Host "`n[ERROR] Deployment failed with exit code $($process.ExitCode)." -ForegroundColor Red
    exit $process.ExitCode
}
