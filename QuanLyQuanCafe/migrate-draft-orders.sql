-- Back up the database and select CafeRestaurant before running this script.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Ban', N'U') IS NULL OR OBJECT_ID(N'dbo.ThucUong', N'U') IS NULL
    THROW 50000, 'Required tables were not found. Select the correct database.', 1;

IF OBJECT_ID(N'dbo.DonTam', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DonTam
    (
        MaBan nchar(5) NOT NULL,
        MaTang nchar(5) NOT NULL,
        LucTao datetime2 NOT NULL CONSTRAINT DF_DonTam_LucTao DEFAULT (SYSDATETIME()),
        CONSTRAINT PK_DonTam PRIMARY KEY (MaBan, MaTang),
        CONSTRAINT FK_DonTam_Ban FOREIGN KEY (MaBan, MaTang) REFERENCES dbo.Ban(MaBan, MaTang)
    );
END;

IF OBJECT_ID(N'dbo.ChiTietDonTam', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChiTietDonTam
    (
        MaDong bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChiTietDonTam PRIMARY KEY,
        MaBan nchar(5) NOT NULL,
        MaTang nchar(5) NOT NULL,
        MaThucUong nchar(5) NOT NULL,
        MaLoai nchar(4) NOT NULL,
        SoLuong tinyint NOT NULL,
        CONSTRAINT CK_ChiTietDonTam_SoLuong CHECK (SoLuong > 0),
        CONSTRAINT FK_ChiTietDonTam_DonTam FOREIGN KEY (MaBan, MaTang)
            REFERENCES dbo.DonTam(MaBan, MaTang) ON UPDATE CASCADE ON DELETE CASCADE,
        CONSTRAINT FK_ChiTietDonTam_ThucUong FOREIGN KEY (MaThucUong, MaLoai)
            REFERENCES dbo.ThucUong(MaThucUong, MaLoai)
    );
END;

COMMIT TRANSACTION;
