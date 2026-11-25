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

    $sqlDirectory = $directory.Replace("'", "''")
    $sql = Get-Content -LiteralPath (Join-Path $root 'SQL Script.sql') -Raw -Encoding UTF8
    if (-not $sql.Contains('create database CafeRestaurant')) { throw 'Database creation statement was not found.' }
    $sql = $sql.Replace('create database CafeRestaurant',
        "create database CafeRestaurant ON (NAME=CafeRestaurant, FILENAME='$sqlDirectory\CafeRestaurant.mdf') LOG ON (NAME=CafeRestaurant_log, FILENAME='$sqlDirectory\CafeRestaurant_log.ldf')")
    $bootstrap = Join-Path $directory 'bootstrap.sql'
    Set-Content -LiteralPath $bootstrap -Value $sql -Encoding UTF8
    $server = "(localdb)\$instance"
    & sqlcmd -S $server -d master -b -f 65001 -i $bootstrap | Out-Null
    Assert-Exit 'Creating test database'
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
