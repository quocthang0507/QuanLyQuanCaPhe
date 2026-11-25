-- Run against the existing CafeRestaurant database after taking a backup.
-- The original constraint permits a drink to be sold only once in the entire database.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.DonHang', N'U') IS NULL
    THROW 50000, 'Table dbo.DonHang was not found. Select the correct database.', 1;

DECLARE @constraint sysname;
DECLARE @statement nvarchar(max);
DECLARE constraints_to_remove CURSOR LOCAL FAST_FORWARD FOR
    SELECT kc.name
    FROM sys.key_constraints AS kc
    WHERE kc.parent_object_id = OBJECT_ID(N'dbo.DonHang')
      AND kc.type = 'UQ'
      AND 2 = (SELECT COUNT(*) FROM sys.index_columns AS ic
               WHERE ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id AND ic.key_ordinal > 0)
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic
                  JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
                  WHERE ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id
                    AND ic.key_ordinal > 0 AND c.name = N'MaThucUong')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic
                  JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
                  WHERE ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id
                    AND ic.key_ordinal > 0 AND c.name = N'MaLoai');

OPEN constraints_to_remove;
FETCH NEXT FROM constraints_to_remove INTO @constraint;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @statement = N'ALTER TABLE dbo.DonHang DROP CONSTRAINT ' + QUOTENAME(@constraint);
    EXEC sp_executesql @statement;
    FETCH NEXT FROM constraints_to_remove INTO @constraint;
END;
CLOSE constraints_to_remove;
DEALLOCATE constraints_to_remove;

IF EXISTS (
    SELECT 1 FROM sys.indexes AS i
    WHERE i.object_id = OBJECT_ID(N'dbo.DonHang') AND i.is_unique = 1
      AND i.is_primary_key = 0 AND i.is_unique_constraint = 0 AND i.has_filter = 0
      AND 2 = (SELECT COUNT(*) FROM sys.index_columns AS ic
               WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.key_ordinal > 0)
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic
                  JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
                  WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
                    AND ic.key_ordinal > 0 AND c.name = N'MaThucUong')
      AND EXISTS (SELECT 1 FROM sys.index_columns AS ic
                  JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
                  WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
                    AND ic.key_ordinal > 0 AND c.name = N'MaLoai')
)
    THROW 50001, 'A separate unique index restricts drink sales. Review it before migrating.', 1;
COMMIT TRANSACTION;
