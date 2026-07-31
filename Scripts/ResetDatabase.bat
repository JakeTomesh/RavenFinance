@echo off
echo ============================================
echo Dropping and Recreating RavenDb Database
echo ============================================
echo.

echo Step 1: Dropping existing database...
dotnet ef database drop --force
if %errorlevel% equ 0 (
	echo SUCCESS: Database dropped!
) else (
	echo WARNING: Database may not exist or drop failed
)
echo.

echo Step 2: Creating migration for IsMonthlyRecurringBill fix...
dotnet ef migrations add FixIsMonthlyRecurringBillNullable
if %errorlevel% neq 0 (
	echo WARNING: Migration creation failed - may already exist
)
echo.

echo Step 3: Applying all migrations to database...
dotnet ef database update
if %errorlevel% equ 0 (
	echo SUCCESS: Database updated successfully!
) else (
	echo ERROR: Failed to update database
	pause
	exit /b 1
)
echo.

echo ============================================
echo Database Reset Complete!
echo ============================================
echo.
echo Next steps:
echo 1. Run your application
echo 2. Use UserId = 2 for testing
echo.
pause
