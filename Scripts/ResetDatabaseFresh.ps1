# ============================================
# PowerShell Script to Reset Database with Fresh Migrations
# ============================================

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Resetting Database with Fresh Migrations" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Step 0: Stop the application if running
Write-Host "Step 0: Please ensure your application is NOT running..." -ForegroundColor Yellow
Write-Host "Press any key to continue after stopping the application..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Write-Host ""

# Step 1: Drop the existing database using SQL
Write-Host "Step 1: Force dropping existing database via SQL..." -ForegroundColor Yellow

# Get connection string from appsettings.json
$appsettings = Get-Content "appsettings.json" | ConvertFrom-Json
$connectionString = $appsettings.ConnectionStrings.DefaultConnection

# Extract server from connection string
if ($connectionString -match "Server=([^;]+)") {
	$server = $matches[1]
} else {
	$server = "localhost"
}

# Create SQL script to force drop database
$dropScript = @"
USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RavenDb')
BEGIN
	ALTER DATABASE RavenDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE RavenDb;
	PRINT 'Database RavenDb dropped successfully.';
END
GO
"@

# Save to temp file
$tempSqlFile = [System.IO.Path]::GetTempFileName() + ".sql"
$dropScript | Out-File -FilePath $tempSqlFile -Encoding UTF8

# Execute SQL script
sqlcmd -S $server -E -i $tempSqlFile

# Clean up temp file
Remove-Item $tempSqlFile

Write-Host "✓ Database drop completed!" -ForegroundColor Green
Write-Host ""

# Step 2: Remove all existing migrations
Write-Host "Step 2: Removing all existing migrations..." -ForegroundColor Yellow
if (Test-Path "Migrations") {
	Remove-Item -Path "Migrations\*" -Recurse -Force
	Write-Host "✓ All migrations removed!" -ForegroundColor Green
} else {
	Write-Host "✓ No migrations folder found" -ForegroundColor Green
}
Write-Host ""

# Step 3: Create a fresh initial migration
Write-Host "Step 3: Creating fresh initial migration..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate

if ($LASTEXITCODE -eq 0) {
	Write-Host "✓ Fresh migration created successfully!" -ForegroundColor Green
} else {
	Write-Host "✗ Failed to create migration" -ForegroundColor Red
	exit 1
}
Write-Host ""

# Step 4: Apply the migration to create database
Write-Host "Step 4: Creating database and applying migration..." -ForegroundColor Yellow
dotnet ef database update

if ($LASTEXITCODE -eq 0) {
	Write-Host "✓ Database created successfully!" -ForegroundColor Green
} else {
	Write-Host "✗ Failed to create database" -ForegroundColor Red
	exit 1
}
Write-Host ""

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Database Reset Complete!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Your database has been recreated with a single fresh migration" -ForegroundColor White
Write-Host "reflecting your current model relationships." -ForegroundColor White
Write-Host ""
