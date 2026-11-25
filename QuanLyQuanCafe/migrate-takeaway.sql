-- Back up the database and select CafeRestaurant before running this script.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Ban', N'U') IS NULL OR OBJECT_ID(N'dbo.DonTam', N'U') IS NULL
    THROW 50000, 'Required tables were not found. Run the draft migration first.', 1;

IF OBJECT_ID(N'dbo.BanMangDi', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BanMangDi
    (
        MaBan nchar(5) NOT NULL,
        MaTang nchar(5) NOT NULL,
        CONSTRAINT PK_BanMangDi PRIMARY KEY (MaBan, MaTang),
        CONSTRAINT FK_BanMangDi_Ban FOREIGN KEY (MaBan, MaTang) REFERENCES dbo.Ban(MaBan, MaTang)
    );
END;

COMMIT TRANSACTION;
