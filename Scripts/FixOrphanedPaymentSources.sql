-- ============================================
-- Script to Fix Orphaned PaymentSources Records
-- Database: RavenDb
-- ============================================

USE RavenDb;
GO

PRINT '============================================';
PRINT 'Step 1: Checking for orphaned PaymentSources records...';
PRINT '============================================';
PRINT '';

-- Find orphaned PaymentSources records
SELECT ps.PaymentSourceId, ps.UserId, ps.Name
FROM PaymentSources ps
LEFT JOIN Users u ON ps.UserId = u.UserId
WHERE u.UserId IS NULL;

PRINT '';
PRINT '============================================';
PRINT 'Step 2: Listing valid Users...';
PRINT '============================================';
PRINT '';

-- Show valid UserIds
SELECT UserId, FirstName, LastName, Email FROM Users;

PRINT '';
PRINT '============================================';
PRINT 'Step 3: Counting orphaned records...';
PRINT '============================================';
PRINT '';

DECLARE @OrphanedCount INT;
SELECT @OrphanedCount = COUNT(*)
FROM PaymentSources ps
LEFT JOIN Users u ON ps.UserId = u.UserId
WHERE u.UserId IS NULL;

PRINT 'Total orphaned PaymentSources records: ' + CAST(@OrphanedCount AS VARCHAR(10));
PRINT '';

-- ============================================
-- FIX: Delete orphaned records
-- ============================================
PRINT '============================================';
PRINT 'Step 4: Deleting orphaned PaymentSources records...';
PRINT '============================================';
PRINT '';

DELETE FROM PaymentSources
WHERE UserId NOT IN (SELECT UserId FROM Users);

PRINT 'Orphaned records deleted successfully!';
PRINT '';

-- ============================================
-- Verification
-- ============================================
PRINT '============================================';
PRINT 'Step 5: Verifying fix...';
PRINT '============================================';
PRINT '';

SELECT @OrphanedCount = COUNT(*)
FROM PaymentSources ps
LEFT JOIN Users u ON ps.UserId = u.UserId
WHERE u.UserId IS NULL;

IF @OrphanedCount = 0
BEGIN
	PRINT 'SUCCESS: No orphaned records remain!';
END
ELSE
BEGIN
	PRINT 'WARNING: ' + CAST(@OrphanedCount AS VARCHAR(10)) + ' orphaned records still exist!';
END

PRINT '';
PRINT '============================================';
PRINT 'Script completed!';
PRINT '============================================';
GO
