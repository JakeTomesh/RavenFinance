# ============================================
# PowerShell Script to Reset Database
# ============================================

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Dropping and Recreating RavenDb Database" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Drop the existing database
Write-Host "Step 1: Dropping existing database..." -ForegroundColor Yellow
dotnet ef database drop --force

if ($LASTEXITCODE -eq 0) {
	Write-Host "✓ Database dropped successfully!" -ForegroundColor Green
} else {
	Write-Host "✗ Failed to drop database (may not exist)" -ForegroundColor Red
}
Write-Host ""

# Step 2: Create a new migration for IsMonthlyRecurringBill change
Write-Host "Step 2: Creating migration for IsMonthlyRecurringBill fix..." -ForegroundColor Yellow
dotnet ef migrations add FixIsMonthlyRecurringBillNullable

if ($LASTEXITCODE -eq 0) {
	Write-Host "✓ Migration created successfully!" -ForegroundColor Green
} else {
	Write-Host "✗ Failed to create migration" -ForegroundColor Red
}
Write-Host ""

# Step 3: Apply all migrations to recreate database
Write-Host "Step 3: Applying all migrations to database..." -ForegroundColor Yellow
dotnet ef database update

if ($LASTEXITCODE -eq 0) {
	Write-Host "✓ Database updated successfully!" -ForegroundColor Green
} else {
	Write-Host "✗ Failed to update database" -ForegroundColor Red
	Write-Host ""
	Write-Host "If you see migration errors, you may need to:" -ForegroundColor Yellow
	Write-Host "1. Delete the migration that was just created" -ForegroundColor Yellow
	Write-Host "2. Run this script again" -ForegroundColor Yellow
	exit 1
}
Write-Host ""

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Database Reset Complete!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Run your application" -ForegroundColor White
Write-Host "2. Use UserId = 2 for testing" -ForegroundColor White
Write-Host ""
