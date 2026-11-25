-- Back up the database and select CafeRestaurant before running this script.
-- Existing invoice lines deliberately remain without a price snapshot.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.DonHang', N'U') IS NULL OR OBJECT_ID(N'dbo.HoaDon', N'U') IS NULL
    THROW 50000, 'Required tables were not found. Select the correct database.', 1;

IF OBJECT_ID(N'dbo.GiaBanDonHang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.GiaBanDonHang
    (
        MaDonHang nchar(12) NOT NULL,
        DonGia smallmoney NOT NULL,
        CONSTRAINT PK_GiaBanDonHang PRIMARY KEY (MaDonHang),
        CONSTRAINT FK_GiaBanDonHang_DonHang FOREIGN KEY (MaDonHang) REFERENCES dbo.DonHang(MaDonHang),
        CONSTRAINT CK_GiaBanDonHang_DonGia CHECK (DonGia >= 0)
    );
END;

COMMIT TRANSACTION;
