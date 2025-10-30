-- QUICK FIX: Add StaffId column to Notifications table
-- Run this SQL in Azure Portal -> SQL Database -> Query Editor
-- OR via Azure CLI: az sql db execute-query

-- Step 1: Add StaffId column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'StaffId')
BEGIN
    ALTER TABLE Notifications ADD StaffId UNIQUEIDENTIFIER NULL;
    PRINT '✅ StaffId column added';
END
ELSE
BEGIN
    PRINT '⚠️ StaffId column already exists';
END

-- Step 2: Add foreign key (if staff table exists)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'staff')
AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Notifications_staff_StaffId')
BEGIN
    ALTER TABLE Notifications
    ADD CONSTRAINT FK_Notifications_staff_StaffId 
    FOREIGN KEY (StaffId) REFERENCES staff(id);
    PRINT '✅ Foreign key added';
END

-- Step 3: Add index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_StaffId' AND object_id = OBJECT_ID('Notifications'))
BEGIN
    CREATE INDEX IX_Notifications_StaffId ON Notifications(StaffId);
    PRINT '✅ Index created';
END

-- Verify
SELECT 'StaffId column exists' AS Status
WHERE EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'StaffId');

