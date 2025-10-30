-- Manual SQL script to add StaffId column to Notifications table
-- Run this if auto-migration doesn't work

-- Check if column already exists
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'StaffId')
BEGIN
    PRINT 'Adding StaffId column to Notifications table...';
    
    -- Add StaffId column
    ALTER TABLE Notifications 
    ADD StaffId UNIQUEIDENTIFIER NULL;
    
    PRINT 'StaffId column added successfully.';
    
    -- Add foreign key constraint (if staff table exists)
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'staff')
    BEGIN
        -- Check if foreign key already exists
        IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Notifications_staff_StaffId')
        BEGIN
            ALTER TABLE Notifications
            ADD CONSTRAINT FK_Notifications_staff_StaffId 
            FOREIGN KEY (StaffId) REFERENCES staff(id);
            
            PRINT 'Foreign key constraint added successfully.';
        END
        ELSE
        BEGIN
            PRINT 'Foreign key constraint already exists.';
        END
    END
    
    -- Add index for StaffId
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_StaffId' AND object_id = OBJECT_ID('Notifications'))
    BEGIN
        CREATE INDEX IX_Notifications_StaffId ON Notifications(StaffId);
        PRINT 'Index IX_Notifications_StaffId created successfully.';
    END
    ELSE
    BEGIN
        PRINT 'Index IX_Notifications_StaffId already exists.';
    END
END
ELSE
BEGIN
    PRINT 'StaffId column already exists in Notifications table.';
END

-- Verify the column was added
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Notifications' AND COLUMN_NAME = 'StaffId';

