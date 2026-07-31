@echo off
echo ============================================
echo Force Reset Database - Complete Clean Slate
echo ============================================
echo.

echo Step 1: Stopping any running applications...
echo Please close your application if it's running, then press any key...
pause > nul
echo.

echo Step 2: Force dropping database via SQL...
sqlcmd -S (localdb)\MSSQLLocalDB -E -i "%~dp0ForceDropDatabase.sql"
if errorlevel 1 (
	echo Trying alternate SQL Server instance...
	sqlcmd -S localhost -E -i "%~dp0ForceDropDatabase.sql"
)
echo.

echo Step 3: Deleting all existing migrations...
if exist "%~dp0..\Migrations" (
	rmdir /s /q "%~dp0..\Migrations"
	echo Migrations folder deleted!
) else (
	echo No migrations folder found.
)
echo.

echo Step 4: Creating fresh migration...
cd "%~dp0.."
dotnet ef migrations add InitialCreate
if errorlevel 1 (
	echo ERROR: Failed to create migration!
	pause
	exit /b 1
)
echo.

echo Step 5: Creating database and applying migration...
dotnet ef database update
if errorlevel 1 (
	echo ERROR: Failed to update database!
	pause
	exit /b 1
)
echo.

echo ============================================
echo SUCCESS! Database reset complete!
echo ============================================
echo.
echo Your database has been recreated with a single clean migration.
echo.
pause
