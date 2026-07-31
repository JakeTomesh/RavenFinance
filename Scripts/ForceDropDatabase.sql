-- ============================================
-- Force Drop Database Script
-- This will forcefully disconnect all users and drop the database
-- ============================================

USE master;
GO

-- Terminate all connections to RavenDb
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RavenDb')
BEGIN
	PRINT 'Setting database to SINGLE_USER mode to disconnect all connections...';
	ALTER DATABASE RavenDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

	PRINT 'Dropping database...';
	DROP DATABASE RavenDb;

	PRINT 'Database RavenDb has been successfully dropped!';
END
ELSE
BEGIN
	PRINT 'Database RavenDb does not exist.';
END
GO

PRINT '';
PRINT '============================================';
PRINT 'Next Steps:';
PRINT '1. Delete the Migrations folder manually';
PRINT '2. Run: dotnet ef migrations add InitialCreate';
PRINT '3. Run: dotnet ef database update';
PRINT '============================================';
GO
