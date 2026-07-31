-- ============================================
-- Script to Drop and Recreate Database
-- Database: RavenDb
-- ============================================

USE master;
GO

-- Drop the database if it exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RavenDb')
BEGIN
	ALTER DATABASE RavenDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE RavenDb;
	PRINT 'Database RavenDb dropped successfully.';
END
ELSE
BEGIN
	PRINT 'Database RavenDb does not exist.';
END
GO

PRINT 'Database is ready to be recreated by EF migrations.';
PRINT 'Run: dotnet ef database update';
GO
