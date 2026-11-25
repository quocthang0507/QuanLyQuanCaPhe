-- Run against CafeRestaurant after taking a backup. Requires SQL Server 2016 SP1 or later.
-- IDs retain the existing HDddMMyyNNNN / DHddMMyyNNNN format.
CREATE OR ALTER FUNCTION dbo.fn_TaoID_TiepTheo(@ID_BanDau nchar(12))
RETURNS nchar(12)
AS
BEGIN
    DECLARE @prefix nchar(2) = LEFT(@ID_BanDau, 2);
    DECLARE @today nchar(6) = dbo.fn_DateToString(GETDATE());
    DECLARE @number int = TRY_CONVERT(int, RIGHT(@ID_BanDau, 4));
    IF @prefix NOT IN (N'HD', N'DH') OR @number IS NULL RETURN NULL;
    IF SUBSTRING(@ID_BanDau, 3, 6) <> @today RETURN CONCAT(@prefix, @today, N'0001');
    IF @number >= 9999 RETURN NULL;
    RETURN CONCAT(@prefix, @today, RIGHT(N'0000' + CONVERT(nvarchar(4), @number + 1), 4));
END
GO

CREATE OR ALTER FUNCTION dbo.fn_SinhMaHD()
RETURNS nchar(12)
AS
BEGIN
    DECLARE @prefix nchar(8) = CONCAT(N'HD', dbo.fn_DateToString(GETDATE()));
    DECLARE @number int;
    SELECT @number = MAX(TRY_CONVERT(int, RIGHT(MaHD, 4)))
    FROM dbo.HoaDon
    WHERE MaHD LIKE @prefix + N'[0-9][0-9][0-9][0-9]';
    IF @number >= 9999 RETURN NULL;
    RETURN CONCAT(@prefix, RIGHT(N'0000' + CONVERT(nvarchar(4), ISNULL(@number, 0) + 1), 4));
END
GO

CREATE OR ALTER FUNCTION dbo.fn_SinhMaDH()
RETURNS nchar(12)
AS
BEGIN
    DECLARE @prefix nchar(8) = CONCAT(N'DH', dbo.fn_DateToString(GETDATE()));
    DECLARE @number int;
    SELECT @number = MAX(TRY_CONVERT(int, RIGHT(MaDonHang, 4)))
    FROM dbo.DonHang
    WHERE MaDonHang LIKE @prefix + N'[0-9][0-9][0-9][0-9]';
    IF @number >= 9999 RETURN NULL;
    RETURN CONCAT(@prefix, RIGHT(N'0000' + CONVERT(nvarchar(4), ISNULL(@number, 0) + 1), 4));
END
GO
