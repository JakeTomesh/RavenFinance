@echo off
echo ============================================
echo Recreating Migration with Fixed Cascade Deletes
echo ============================================
echo.

echo Step 1: Deleting existing InitialCreate migration...
if exist "Migrations" (
	rmdir /s /q "Migrations"
	echo Migrations folder deleted!
) else (
	echo No migrations folder found.
)
echo.

echo Step 2: Creating fresh migration with fixed relationships...
dotnet ef migrations add InitialCreate
if errorlevel 1 (
	echo ERROR: Failed to create migration!
	pause
	exit /b 1
)
echo.

echo Step 3: Applying migration to database...
dotnet ef database update
if errorlevel 1 (
	echo ERROR: Failed to update database!
	pause
	exit /b 1
)
echo.

echo ============================================
echo SUCCESS! Database created with fixed cascade deletes!
echo ============================================
echo.
echo Your database is now ready with the following relationships:
echo - FinanceRecord -^> StatementItems: CASCADE DELETE
echo - User -^> StatementItems: RESTRICT (no cascade)
echo - User -^> FinanceRecords: RESTRICT (no cascade)
echo - User -^> PaymentSources: RESTRICT (no cascade)
echo - PaymentSource -^> StatementItems: RESTRICT (no cascade)
echo - Category -^> StatementItems: RESTRICT (no cascade)
echo.
pause
