$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$suffix = [guid]::NewGuid().ToString('N')
$instance = "CopilotCafeTest_$suffix"
$directory = Join-Path ([IO.Path]::GetTempPath()) "CafeIntegration_$suffix"
$created = $false
$directoryCreated = $false

function Assert-Exit($description) {
    if ($LASTEXITCODE -ne 0) { throw "$description failed with exit code $LASTEXITCODE." }
}

try {
    if (Test-Path -LiteralPath $directory) { throw "Temporary directory already exists: $directory" }
    New-Item -ItemType Directory -Path $directory | Out-Null
    $directoryCreated = $true
    $instances = & sqllocaldb info
    Assert-Exit 'Listing LocalDB instances'
    if ($instances -contains $instance) { throw "LocalDB instance already exists: $instance" }
    & sqllocaldb create $instance
    Assert-Exit 'Creating LocalDB'
    $created = $true
    & sqllocaldb start $instance
    Assert-Exit 'Starting LocalDB'

    $server = "(localdb)\$instance"
    $sqlDirectory = $directory.Replace("'", "''")
    foreach ($entry in @(
        @{ Script = 'SQL Script.sql'; Database = 'CafeRestaurant' },
        @{ Script = 'Full Script.sql'; Database = 'CafeRestaurantFull' }
    )) {
        $database = $entry.Database
        $sql = Get-Content -LiteralPath (Join-Path $root $entry.Script) -Raw -Encoding UTF8
        $sql = $sql.Replace('CafeRestaurant', $database)
        $create = "create database $database"
        if (-not $sql.Contains($create)) { throw "Database creation statement not found in $($entry.Script)." }
        $sql = $sql.Replace($create,
            "create database $database ON (NAME=$database, FILENAME='$sqlDirectory\$database.mdf') LOG ON (NAME=${database}_log, FILENAME='$sqlDirectory\${database}_log.ldf')")
        $bootstrap = Join-Path $directory "$database.sql"
        Set-Content -LiteralPath $bootstrap -Value $sql -Encoding UTF8
        & sqlcmd -S $server -d master -b -f 65001 -i $bootstrap | Out-Null
        Assert-Exit "Creating $database from $($entry.Script)"

        $freshChecks = @"
IF EXISTS (
    SELECT name FROM (VALUES ('GiaBanDonHang'), ('CaLamViec'), ('ThanhToanHoaDon'),
        ('HoanTienHoaDon'), ('DonTam'), ('ChiTietDonTam'), ('BanMangDi')) required(name)
    WHERE OBJECT_ID(N'dbo.' + required.name, N'U') IS NULL
) THROW 50000, 'A required table is missing.', 1;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.CaLamViec')
    AND name = N'UX_CaLamViec_MotCaMo' AND is_unique = 1 AND has_filter = 1)
    THROW 50000, 'The single open shift index is missing.', 1;
IF (SELECT COUNT(*) FROM dbo.Ban) <> 20 OR (SELECT COUNT(*) FROM dbo.LoaiNuoc) <> 2
    OR (SELECT COUNT(*) FROM dbo.ThucUong) <> 16
    OR NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = 'admin')
    OR NOT EXISTS (SELECT 1 FROM dbo.SuKien WHERE MaSuKien = 'NONAME')
    THROW 50000, 'Seed data is incomplete.', 1;
IF ISNULL(dbo.fn_SinhMaHD(), '') <> CONCAT('HD', dbo.fn_DateToString(GETDATE()), '0001')
    OR ISNULL(dbo.fn_SinhMaDH(), '') <> CONCAT('DH', dbo.fn_DateToString(GETDATE()), '0001')
    THROW 50000, 'Initial identifiers are incorrect.', 1;
BEGIN TRANSACTION;
DECLARE @today nchar(6) = dbo.fn_DateToString(GETDATE());
DECLARE @hd1 nchar(12) = CONCAT('HD', @today, '0001'), @hd3 nchar(12) = CONCAT('HD', @today, '0003');
DECLARE @dh1 nchar(12) = CONCAT('DH', @today, '0001'), @dh3 nchar(12) = CONCAT('DH', @today, '0003');
INSERT dbo.HoaDon (MaHD, NgayHD, MaNV, MaBan, MaTang, TongTien, MaSuKien, DuaTruoc, ConLai)
    VALUES (@hd1, GETDATE(), 'admin', 'BAN01', 'TANG0', 50000, 'NONAME', 50000, 0),
           (@hd3, GETDATE(), 'admin', 'BAN01', 'TANG0', 50000, 'NONAME', 50000, 0);
INSERT dbo.DonHang (MaDonHang, MaThucUong, MaLoai, SoLuong, MaHD)
    VALUES (@dh1, 'CAPPU', 'Nong', 1, @hd1), (@dh3, 'CAPPU', 'Nong', 1, @hd3);
IF ISNULL(dbo.fn_SinhMaHD(), '') <> CONCAT('HD', @today, '0004')
    OR ISNULL(dbo.fn_SinhMaDH(), '') <> CONCAT('DH', @today, '0004')
    THROW 50000, 'Identifiers did not advance from the maximum.', 1;
DECLARE @hdMax nchar(12) = CONCAT('HD', @today, '9999'), @dhMax nchar(12) = CONCAT('DH', @today, '9999');
INSERT dbo.HoaDon (MaHD, NgayHD, MaNV, MaBan, MaTang, TongTien, MaSuKien, DuaTruoc, ConLai)
    VALUES (@hdMax, GETDATE(), 'admin', 'BAN01', 'TANG0', 50000, 'NONAME', 50000, 0);
INSERT dbo.DonHang (MaDonHang, MaThucUong, MaLoai, SoLuong, MaHD)
    VALUES (@dhMax, 'CAPPU', 'Nong', 1, @hdMax);
IF dbo.fn_SinhMaHD() IS NOT NULL OR dbo.fn_SinhMaDH() IS NOT NULL
    THROW 50000, 'Identifiers did not stop at the daily limit.', 1;
ROLLBACK TRANSACTION;
CREATE TABLE dbo.CafeBootstrapSentinel (Value int NOT NULL);
INSERT dbo.CafeBootstrapSentinel VALUES (42);
"@
        & sqlcmd -S $server -d $database -b -Q $freshChecks | Out-Null
        Assert-Exit "Checking fresh $database"
        foreach ($stopOnError in @($true, $false)) {
            $arguments = @('-S', $server, '-d', 'master', '-f', '65001', '-i', $bootstrap)
            if ($stopOnError) { $arguments += '-b' }
            $guardOutput = & sqlcmd @arguments
            if (($guardOutput -join "`n") -notmatch 'already exists') {
                throw "Existing database guard did not report an error for $database."
            }
            if ($stopOnError -and $LASTEXITCODE -eq 0) {
                throw "Existing database guard did not fail sqlcmd -b for $database."
            }
            & sqlcmd -S $server -d $database -b -Q "IF NOT EXISTS (SELECT 1 FROM dbo.CafeBootstrapSentinel WHERE Value=42) OR (SELECT COUNT(*) FROM dbo.Ban) <> 20 OR (SELECT COUNT(*) FROM dbo.ThucUong) <> 16 THROW 50000, 'Existing database changed.', 1;" | Out-Null
            Assert-Exit "Checking existing $database after guarded execution"
        }
    }

    $failureSql = (Get-Content -LiteralPath (Join-Path $root 'SQL Script.sql') -Raw -Encoding UTF8).Replace('CafeRestaurant', 'CafeCreateFailure')
    $failureSql = $failureSql.Replace('create database CafeCreateFailure',
        "create database CafeCreateFailure ON (NAME=CafeCreateFailure, FILENAME='$sqlDirectory\missing\failure.mdf') LOG ON (NAME=CafeCreateFailure_log, FILENAME='$sqlDirectory\missing\failure_log.ldf')")
    $failureScript = Join-Path $directory 'failure.sql'
    Set-Content -LiteralPath $failureScript -Value $failureSql -Encoding UTF8
    $masterTablesBefore = ((& sqlcmd -S $server -d master -b -h -1 -W -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.tables;") -join "`n").Trim()
    Assert-Exit 'Counting master tables'
    $failureOutput = & sqlcmd -S $server -d master -f 65001 -i $failureScript
    if (($failureOutput -join "`n") -notmatch 'Msg ') { throw 'Expected database creation failure was not reported.' }
    $masterTablesAfter = ((& sqlcmd -S $server -d master -b -h -1 -W -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.tables;") -join "`n").Trim()
    Assert-Exit 'Checking master tables after creation failure'
    if ($masterTablesBefore -ne $masterTablesAfter) { throw 'Failed database creation changed master.' }
    & sqlcmd -S $server -d master -b -Q "IF DB_ID(N'CafeCreateFailure') IS NOT NULL THROW 50000, 'Unexpected test database exists.', 1;" | Out-Null
    Assert-Exit 'Checking failed database creation'

    # Reconstruct the old schema only inside this newly created isolated test database.
    & sqlcmd -S $server -d CafeRestaurant -b -Q "DROP TABLE dbo.BanMangDi, dbo.ChiTietDonTam, dbo.DonTam, dbo.HoanTienHoaDon, dbo.ThanhToanHoaDon, dbo.CaLamViec, dbo.GiaBanDonHang; ALTER TABLE dbo.DonHang ADD CONSTRAINT UQ_Legacy_Drink UNIQUE (MaThucUong, MaLoai);" | Out-Null
    Assert-Exit 'Preparing legacy schema'
    & sqlcmd -S $server -d CafeRestaurant -b -Q "INSERT dbo.HoaDon (MaHD, NgayHD, MaNV, MaBan, MaTang, TongTien, MaSuKien, DuaTruoc, ConLai) VALUES ('HD0101010099', '2020-01-01', 'admin', 'BAN01', 'TANG0', 50000, 'NONAME', 50000, 0); INSERT dbo.DonHang (MaDonHang, MaThucUong, MaLoai, SoLuong, MaHD) VALUES ('DH0101010099', 'CAPPU', 'Nong', 1, 'HD0101010099');" | Out-Null
    Assert-Exit 'Creating legacy invoice'
    foreach ($migration in @('migrate-order-uniqueness.sql', 'migrate-identifiers.sql',
            'migrate-invoice-prices.sql', 'migrate-cafe-operations.sql', 'migrate-draft-orders.sql', 'migrate-takeaway.sql')) {
        $path = Join-Path (Join-Path $root 'QuanLyQuanCafe') $migration
        & sqlcmd -S $server -d CafeRestaurant -b -i $path
        Assert-Exit "Applying $migration"
        & sqlcmd -S $server -d CafeRestaurant -b -i $path
        Assert-Exit "Reapplying $migration"
    }
    & sqlcmd -S $server -d CafeRestaurant -b -Q "IF NOT EXISTS (SELECT 1 FROM dbo.DonHang WHERE MaDonHang='DH0101010099' AND MaHD='HD0101010099') OR EXISTS (SELECT 1 FROM dbo.GiaBanDonHang WHERE MaDonHang='DH0101010099') THROW 50000, 'Legacy invoice changed or acquired a guessed price.', 1; DELETE dbo.DonHang WHERE MaDonHang='DH0101010099'; DELETE dbo.HoaDon WHERE MaHD='HD0101010099';" | Out-Null
    Assert-Exit 'Checking legacy data'

    & dotnet build (Join-Path $PSScriptRoot 'CafeIntegration.csproj') -c Release --nologo -v:q
    Assert-Exit 'Building integration tests'
    $output = Join-Path $PSScriptRoot 'bin\Release\net10.0-windows'
    $config = Join-Path $output 'CafeIntegration.dll.config'
    $text = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'App.config') -Raw -Encoding UTF8
    if (-not $text.Contains('CopilotCafeTest_PLACEHOLDER')) { throw 'Test configuration placeholder was not found.' }
    Set-Content -LiteralPath $config -Value $text.Replace('CopilotCafeTest_PLACEHOLDER', $instance) -Encoding UTF8
    $env:CAFE_TEST_INSTANCE = $instance
    $stdout = Join-Path $directory 'out.txt'
    $stderr = Join-Path $directory 'err.txt'
    $process = Start-Process -FilePath (Join-Path $output 'CafeIntegration.exe') -NoNewWindow -PassThru `
        -RedirectStandardOutput $stdout -RedirectStandardError $stderr
    $null = $process.Handle
    if (-not $process.WaitForExit(120000)) {
        Stop-Process -Id $process.Id
        throw 'Integration tests timed out.'
    }
    $exitCode = $process.ExitCode
    Get-Content -LiteralPath $stdout
    Get-Content -LiteralPath $stderr
    if ($exitCode -ne 0) { throw "Integration tests failed with exit code $exitCode." }
}
finally {
    $safeToDeleteFiles = -not $created
    if ($created) {
        & sqllocaldb stop $instance
        if ($LASTEXITCODE -eq 0) {
            & sqllocaldb delete $instance
            if ($LASTEXITCODE -eq 0) {
                $remaining = & sqllocaldb info
                $safeToDeleteFiles = $LASTEXITCODE -eq 0 -and $remaining -notcontains $instance
            }
        }
        if (-not $safeToDeleteFiles) { Write-Warning "LocalDB cleanup failed for $instance; files remain at $directory." }
    }
    if ($directoryCreated -and $safeToDeleteFiles) { Remove-Item -LiteralPath $directory -Recurse -Force }
}
