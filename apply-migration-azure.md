# Apply StaffId Migration - Quick Guide

## Option 1: Azure Portal Query Editor (EASIEST)

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to: **SQL databases** → **HopewellDatabase**
3. Click **Query editor (preview)** in the left menu
4. Login with SQL authentication:
   - **Server admin login**: `sqladmin`
   - **Password**: `Liverpool@2024!!`
5. Copy and paste this SQL:

```sql
-- Add StaffId column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'StaffId')
BEGIN
    ALTER TABLE Notifications ADD StaffId UNIQUEIDENTIFIER NULL;
    SELECT '✅ StaffId column added' AS Result;
END
ELSE
BEGIN
    SELECT '⚠️ StaffId column already exists' AS Result;
END

-- Add foreign key
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'staff')
AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Notifications_staff_StaffId')
BEGIN
    ALTER TABLE Notifications
    ADD CONSTRAINT FK_Notifications_staff_StaffId 
    FOREIGN KEY (StaffId) REFERENCES staff(id);
    SELECT '✅ Foreign key added' AS Result;
END

-- Add index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_StaffId' AND object_id = OBJECT_ID('Notifications'))
BEGIN
    CREATE INDEX IX_Notifications_StaffId ON Notifications(StaffId);
    SELECT '✅ Index created' AS Result;
END

-- Verify
SELECT 'StaffId column exists' AS Status
WHERE EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'StaffId');
```

6. Click **Run**
7. You should see success messages

---

## Option 2: Azure CLI (Command Line)

Run this PowerShell command:

```powershell
az sql db execute-query `
  --resource-group "AZ-JHB-RSG-RCNA-ST10466568-TER" `
  --server "vuyo-rosebank2" `
  --database "HopewellDatabase" `
  --query-text "ALTER TABLE Notifications ADD StaffId UNIQUEIDENTIFIER NULL;" `
  --admin-user "sqladmin" `
  --admin-password "Liverpool@2024!!"
```

Then add the foreign key:

```powershell
az sql db execute-query `
  --resource-group "AZ-JHB-RSG-RCNA-ST10466568-TER" `
  --server "vuyo-rosebank2" `
  --database "HopewellDatabase" `
  --query-text "ALTER TABLE Notifications ADD CONSTRAINT FK_Notifications_staff_StaffId FOREIGN KEY (StaffId) REFERENCES staff(id);" `
  --admin-user "sqladmin" `
  --admin-password "Liverpool@2024!!"
```

Then add the index:

```powershell
az sql db execute-query `
  --resource-group "AZ-JHB-RSG-RCNA-ST10466568-TER" `
  --server "vuyo-rosebank2" `
  --database "HopewellDatabase" `
  --query-text "CREATE INDEX IX_Notifications_StaffId ON Notifications(StaffId);" `
  --admin-user "sqladmin" `
  --admin-password "Liverpool@2024!!"
```

---

## Option 3: SQL Server Management Studio (SSMS)

1. Connect to: `vuyo-rosebank2.database.windows.net`
2. Database: `HopewellDatabase`
3. Login: `sqladmin` / `Liverpool@2024!!`
4. Run the SQL from Option 1

---

## Verify Migration Applied

After running any option above, verify with:

```sql
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Notifications' AND COLUMN_NAME = 'StaffId';
```

You should see one row with `StaffId` column details.

