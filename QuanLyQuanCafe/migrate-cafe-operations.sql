-- Back up the database and select CafeRestaurant before running this script.
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.HoaDon', N'U') IS NULL OR OBJECT_ID(N'dbo.TaiKhoan', N'U') IS NULL
    THROW 50000, 'Required tables were not found. Select the correct database.', 1;

IF OBJECT_ID(N'dbo.CaLamViec', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CaLamViec
    (
        MaCa bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_CaLamViec PRIMARY KEY,
        NguoiMo nvarchar(20) NOT NULL,
        LucMo datetime2 NOT NULL,
        TienDauCa bigint NOT NULL,
        NguoiDong nvarchar(20) NULL,
        LucDong datetime2 NULL,
        TienCuoiCa bigint NULL,
        TienDuKien bigint NULL,
        MotCa bit NOT NULL CONSTRAINT DF_CaLamViec_MotCa DEFAULT (1),
        CONSTRAINT CK_CaLamViec_MotCa CHECK (MotCa = 1),
        CONSTRAINT CK_CaLamViec_Tien CHECK (TienDauCa >= 0 AND
            (TienCuoiCa IS NULL OR TienCuoiCa >= 0)),
        CONSTRAINT CK_CaLamViec_Dong CHECK (
            (LucDong IS NULL AND NguoiDong IS NULL AND TienCuoiCa IS NULL AND TienDuKien IS NULL) OR
            (LucDong IS NOT NULL AND NguoiDong IS NOT NULL AND TienCuoiCa IS NOT NULL AND TienDuKien IS NOT NULL)),
        CONSTRAINT FK_CaLamViec_NguoiMo FOREIGN KEY (NguoiMo) REFERENCES dbo.TaiKhoan(TenDangNhap),
        CONSTRAINT FK_CaLamViec_NguoiDong FOREIGN KEY (NguoiDong) REFERENCES dbo.TaiKhoan(TenDangNhap)
    );
    CREATE UNIQUE INDEX UX_CaLamViec_MotCaMo ON dbo.CaLamViec(MotCa) WHERE LucDong IS NULL;
END;

IF OBJECT_ID(N'dbo.ThanhToanHoaDon', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ThanhToanHoaDon
    (
        MaHD nchar(12) NOT NULL CONSTRAINT PK_ThanhToanHoaDon PRIMARY KEY,
        MaCa bigint NOT NULL,
        PhuongThuc varchar(12) NOT NULL,
        LucThanhToan datetime2 NOT NULL,
        CONSTRAINT CK_ThanhToanHoaDon_PhuongThuc CHECK (PhuongThuc IN ('CASH', 'TRANSFER')),
        CONSTRAINT FK_ThanhToanHoaDon_HoaDon FOREIGN KEY (MaHD) REFERENCES dbo.HoaDon(MaHD),
        CONSTRAINT FK_ThanhToanHoaDon_Ca FOREIGN KEY (MaCa) REFERENCES dbo.CaLamViec(MaCa)
    );
END;

IF OBJECT_ID(N'dbo.HoanTienHoaDon', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HoanTienHoaDon
    (
        MaHD nchar(12) NOT NULL CONSTRAINT PK_HoanTienHoaDon PRIMARY KEY,
        MaCa bigint NOT NULL,
        NguoiHoan nvarchar(20) NOT NULL,
        LucHoan datetime2 NOT NULL,
        PhuongThuc varchar(12) NOT NULL,
        LyDo nvarchar(256) NOT NULL,
        CONSTRAINT CK_HoanTienHoaDon_PhuongThuc CHECK (PhuongThuc IN ('CASH', 'TRANSFER')),
        CONSTRAINT FK_HoanTienHoaDon_HoaDon FOREIGN KEY (MaHD) REFERENCES dbo.HoaDon(MaHD),
        CONSTRAINT FK_HoanTienHoaDon_Ca FOREIGN KEY (MaCa) REFERENCES dbo.CaLamViec(MaCa),
        CONSTRAINT FK_HoanTienHoaDon_NguoiHoan FOREIGN KEY (NguoiHoan) REFERENCES dbo.TaiKhoan(TenDangNhap)
    );
END;

COMMIT TRANSACTION;
