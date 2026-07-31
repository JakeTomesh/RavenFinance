-- ============================================
-- Script to Fix Orphaned StatementItems Records
-- Database: RavenDb
-- ============================================

USE RavenDb;
GO

PRINT '============================================';
PRINT 'Step 1: Checking for orphaned StatementItems records...';
PRINT '============================================';
PRINT '';

-- Find orphaned StatementItems records
SELECT si.StatementItemId, si.FinanceRecordId, si.Merchant, si.Amount, si.DateOfTransaction
FROM StatementItems si
LEFT JOIN FinanceRecords fr ON si.FinanceRecordId = fr.FinanceRecordId
WHERE fr.FinanceRecordId IS NULL;

PRINT '';
PRINT '============================================';
PRINT 'Step 2: Listing valid FinanceRecords...';
PRINT '============================================';
PRINT '';

-- Show valid FinanceRecordIds
SELECT FinanceRecordId, UserId, DateCreated FROM FinanceRecords;

PRINT '';
PRINT '============================================';
PRINT 'Step 3: Counting orphaned records...';
PRINT '============================================';
PRINT '';

DECLARE @OrphanedCount INT;
SELECT @OrphanedCount = COUNT(*)
FROM StatementItems si
LEFT JOIN FinanceRecords fr ON si.FinanceRecordId = fr.FinanceRecordId
WHERE fr.FinanceRecordId IS NULL;

PRINT 'Total orphaned StatementItems records: ' + CAST(@OrphanedCount AS VARCHAR(10));
PRINT '';

-- ============================================
-- FIX: Delete orphaned records
-- ============================================
PRINT '============================================';
PRINT 'Step 4: Deleting orphaned StatementItems records...';
PRINT '============================================';
PRINT '';

DELETE FROM StatementItems
WHERE FinanceRecordId NOT IN (SELECT FinanceRecordId FROM FinanceRecords);

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
FROM StatementItems si
LEFT JOIN FinanceRecords fr ON si.FinanceRecordId = fr.FinanceRecordId
WHERE fr.FinanceRecordId IS NULL;

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
