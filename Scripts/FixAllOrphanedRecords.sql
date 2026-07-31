-- ============================================
-- Comprehensive Script to Fix All Orphaned Records
-- Database: RavenDb
-- ============================================

USE RavenDb;
GO

PRINT '============================================';
PRINT 'COMPREHENSIVE ORPHANED RECORDS FIX';
PRINT '============================================';
PRINT '';

-- ============================================
-- FIX 1: StatementItems -> FinanceRecords
-- ============================================
PRINT '============================================';
PRINT 'Fixing StatementItems orphaned records...';
PRINT '============================================';

DECLARE @StatementItemsOrphanedCount INT;
SELECT @StatementItemsOrphanedCount = COUNT(*)
FROM StatementItems si
LEFT JOIN FinanceRecords fr ON si.FinanceRecordId = fr.FinanceRecordId
WHERE fr.FinanceRecordId IS NULL;

PRINT 'Orphaned StatementItems found: ' + CAST(@StatementItemsOrphanedCount AS VARCHAR(10));

IF @StatementItemsOrphanedCount > 0
BEGIN
	DELETE FROM StatementItems
	WHERE FinanceRecordId NOT IN (SELECT FinanceRecordId FROM FinanceRecords);
	PRINT 'Deleted ' + CAST(@StatementItemsOrphanedCount AS VARCHAR(10)) + ' orphaned StatementItems records';
END
ELSE
BEGIN
	PRINT 'No orphaned StatementItems records found';
END
PRINT '';

-- ============================================
-- FIX 2: StatementItems -> PaymentSources
-- ============================================
PRINT '============================================';
PRINT 'Fixing StatementItems -> PaymentSources orphaned records...';
PRINT '============================================';

DECLARE @StatementItemsPaymentSourceOrphanedCount INT;
SELECT @StatementItemsPaymentSourceOrphanedCount = COUNT(*)
FROM StatementItems si
LEFT JOIN PaymentSources ps ON si.PaymentSourceId = ps.PaymentSourceId
WHERE ps.PaymentSourceId IS NULL AND si.PaymentSourceId IS NOT NULL;

PRINT 'Orphaned StatementItems (PaymentSource) found: ' + CAST(@StatementItemsPaymentSourceOrphanedCount AS VARCHAR(10));

IF @StatementItemsPaymentSourceOrphanedCount > 0
BEGIN
	DELETE FROM StatementItems
	WHERE PaymentSourceId IS NOT NULL 
	AND PaymentSourceId NOT IN (SELECT PaymentSourceId FROM PaymentSources);
	PRINT 'Deleted ' + CAST(@StatementItemsPaymentSourceOrphanedCount AS VARCHAR(10)) + ' orphaned StatementItems records';
END
ELSE
BEGIN
	PRINT 'No orphaned StatementItems (PaymentSource) records found';
END
PRINT '';

-- ============================================
-- FIX 3: PaymentSources -> Users
-- ============================================
PRINT '============================================';
PRINT 'Fixing PaymentSources orphaned records...';
PRINT '============================================';

DECLARE @PaymentSourcesOrphanedCount INT;
SELECT @PaymentSourcesOrphanedCount = COUNT(*)
FROM PaymentSources ps
LEFT JOIN Users u ON ps.UserId = u.UserId
WHERE u.UserId IS NULL;

PRINT 'Orphaned PaymentSources found: ' + CAST(@PaymentSourcesOrphanedCount AS VARCHAR(10));

IF @PaymentSourcesOrphanedCount > 0
BEGIN
	DELETE FROM PaymentSources
	WHERE UserId NOT IN (SELECT UserId FROM Users);
	PRINT 'Deleted ' + CAST(@PaymentSourcesOrphanedCount AS VARCHAR(10)) + ' orphaned PaymentSources records';
END
ELSE
BEGIN
	PRINT 'No orphaned PaymentSources records found';
END
PRINT '';

-- ============================================
-- FIX 4: PaymentSources -> FinanceRecords
-- ============================================
PRINT '============================================';
PRINT 'Fixing PaymentSources -> FinanceRecords orphaned records...';
PRINT '============================================';

DECLARE @PaymentSourcesFinanceRecordOrphanedCount INT;
SELECT @PaymentSourcesFinanceRecordOrphanedCount = COUNT(*)
FROM PaymentSources ps
LEFT JOIN FinanceRecords fr ON ps.FinanceRecordId = fr.FinanceRecordId
WHERE fr.FinanceRecordId IS NULL;

PRINT 'Orphaned PaymentSources (FinanceRecord) found: ' + CAST(@PaymentSourcesFinanceRecordOrphanedCount AS VARCHAR(10));

IF @PaymentSourcesFinanceRecordOrphanedCount > 0
BEGIN
	DELETE FROM PaymentSources
	WHERE FinanceRecordId NOT IN (SELECT FinanceRecordId FROM FinanceRecords);
	PRINT 'Deleted ' + CAST(@PaymentSourcesFinanceRecordOrphanedCount AS VARCHAR(10)) + ' orphaned PaymentSources records';
END
ELSE
BEGIN
	PRINT 'No orphaned PaymentSources (FinanceRecord) records found';
END
PRINT '';

-- ============================================
-- FIX 5: FinanceRecords -> Users
-- ============================================
PRINT '============================================';
PRINT 'Fixing FinanceRecords orphaned records...';
PRINT '============================================';

DECLARE @FinanceRecordsOrphanedCount INT;
SELECT @FinanceRecordsOrphanedCount = COUNT(*)
FROM FinanceRecords fr
LEFT JOIN Users u ON fr.UserId = u.UserId
WHERE u.UserId IS NULL;

PRINT 'Orphaned FinanceRecords found: ' + CAST(@FinanceRecordsOrphanedCount AS VARCHAR(10));

IF @FinanceRecordsOrphanedCount > 0
BEGIN
	DELETE FROM FinanceRecords
	WHERE UserId NOT IN (SELECT UserId FROM Users);
	PRINT 'Deleted ' + CAST(@FinanceRecordsOrphanedCount AS VARCHAR(10)) + ' orphaned FinanceRecords records';
END
ELSE
BEGIN
	PRINT 'No orphaned FinanceRecords records found';
END
PRINT '';

-- ============================================
-- FINAL VERIFICATION
-- ============================================
PRINT '============================================';
PRINT 'FINAL VERIFICATION';
PRINT '============================================';
PRINT '';

DECLARE @TotalOrphaned INT = 0;

SELECT @TotalOrphaned = @TotalOrphaned + COUNT(*)
FROM StatementItems si
LEFT JOIN FinanceRecords fr ON si.FinanceRecordId = fr.FinanceRecordId
WHERE fr.FinanceRecordId IS NULL;

SELECT @TotalOrphaned = @TotalOrphaned + COUNT(*)
FROM PaymentSources ps
LEFT JOIN Users u ON ps.UserId = u.UserId
WHERE u.UserId IS NULL;

SELECT @TotalOrphaned = @TotalOrphaned + COUNT(*)
FROM FinanceRecords fr
LEFT JOIN Users u ON fr.UserId = u.UserId
WHERE u.UserId IS NULL;

IF @TotalOrphaned = 0
BEGIN
	PRINT '✓ SUCCESS: All orphaned records have been cleaned up!';
	PRINT '✓ Your database is now ready for migration.';
END
ELSE
BEGIN
	PRINT '✗ WARNING: ' + CAST(@TotalOrphaned AS VARCHAR(10)) + ' orphaned records still exist!';
END

PRINT '';
PRINT '============================================';
PRINT 'Script completed!';
PRINT '============================================';
GO
