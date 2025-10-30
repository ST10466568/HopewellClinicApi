-- SINGLE COMMAND FIX: Copy this entire block and run in Azure Portal Query Editor
-- Go to: Azure Portal -> SQL databases -> HopewellDatabase -> Query editor

-- Add StaffId column (if not exists)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'StaffId')
    ALTER TABLE Notifications ADD StaffId UNIQUEIDENTIFIER NULL;

-- Add foreign key (if staff table exists and FK doesn't exist)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'staff')
AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Notifications_staff_StaffId')
    ALTER TABLE Notifications ADD CONSTRAINT FK_Notifications_staff_StaffId FOREIGN KEY (StaffId) REFERENCES staff(id);

-- Add index (if not exists)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_StaffId' AND object_id = OBJECT_ID('Notifications'))
    CREATE INDEX IX_Notifications_StaffId ON Notifications(StaffId);

-- Verify: Should return 1 row if column exists
SELECT 
    'SUCCESS: StaffId column exists' AS Status,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Notifications' AND COLUMN_NAME = 'StaffId';

