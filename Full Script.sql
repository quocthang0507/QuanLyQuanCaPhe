USE [master]
GO
/****** Object:  Database [CafeRestaurant]    Script Date: 12/12/2018 8:58:04 SA ******/
CREATE DATABASE [CafeRestaurant]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CafeRestaurant', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\CafeRestaurant.mdf' , SIZE = 3264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CafeRestaurant_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\CafeRestaurant_log.ldf' , SIZE = 832KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [CafeRestaurant] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CafeRestaurant].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CafeRestaurant] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CafeRestaurant] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CafeRestaurant] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CafeRestaurant] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CafeRestaurant] SET ARITHABORT OFF 
GO
ALTER DATABASE [CafeRestaurant] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [CafeRestaurant] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CafeRestaurant] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CafeRestaurant] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CafeRestaurant] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CafeRestaurant] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CafeRestaurant] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CafeRestaurant] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CafeRestaurant] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CafeRestaurant] SET  ENABLE_BROKER 
GO
ALTER DATABASE [CafeRestaurant] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CafeRestaurant] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CafeRestaurant] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CafeRestaurant] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CafeRestaurant] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CafeRestaurant] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CafeRestaurant] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CafeRestaurant] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CafeRestaurant] SET  MULTI_USER 
GO
ALTER DATABASE [CafeRestaurant] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CafeRestaurant] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CafeRestaurant] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CafeRestaurant] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [CafeRestaurant] SET DELAYED_DURABILITY = DISABLED 
GO
USE [CafeRestaurant]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_DateToString]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-----------------------------------------------------------------------------------------

------------------------------------HÀM SINH MÃ HÓA ĐƠN VÀ MÃ ĐƠN HÀNG------------------------------------
--drop function fn_DateToString
create function [dbo].[fn_DateToString](@Ngay date) returns nchar(6)
as
	begin
		declare @chuoi nchar(6), @day tinyint, @month tinyint, @year int
		set @day = datepart(day, @Ngay)
		set @month = datepart(month, @Ngay)
		set @year  = datepart(year, @Ngay)
		set @chuoi = concat(right('00' + convert(varchar, @day), 2), right('00' + convert(varchar, @month), 2), right(@year, 2))
		return @chuoi
	end

GO
/****** Object:  UserDefinedFunction [dbo].[fn_SinhMaDH]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--print dbo.fn_SinhMaHD()

create function [dbo].[fn_SinhMaDH]() returns nchar(12)
as
	begin
		declare @i int
		set @i = 0
		--Go to the last bill
		declare cur_MaDH cursor local
			for select MaDonHang
		from DonHang
		open cur_MaDH
		declare @MaDH nchar(12)
		fetch next from cur_MaDH into @MaDH
		while @@FETCH_STATUS = 0
			begin
				fetch next from cur_MaDH into @MaDH
				set @i = @i + 1
			end
		close cur_MaDH
		deallocate cur_MaDH
		--Generate new order id
		if (@i = 0)
			begin
				return concat('DH', dbo.fn_DateToString(getdate()), '0001')
			end
		return dbo.fn_TaoID_TiepTheo(@MaDH)
	end

GO
/****** Object:  UserDefinedFunction [dbo].[fn_SinhMaHD]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create function [dbo].[fn_SinhMaHD]() returns nchar(12)
as
	begin
		--Go to the last bill
		declare @i int
		set @i = 0
		declare cur_MaHD cursor local
			for select MaHD
		from HoaDon
		open cur_MaHD
		declare @MaHD nchar(12)
		fetch next from cur_MaHD into @MaHD
		while @@FETCH_STATUS = 0
			begin
				fetch next from cur_MaHD into @MaHD
				set @i = @i + 1
			end
		close cur_MaHD
		deallocate cur_MaHD
		--Generate new bill id
		if (@i = 0)
			begin
				return concat('HD', dbo.fn_DateToString(getdate()), '0001')
			end
		return (dbo.fn_TaoID_TiepTheo(@MaHD))
	end

GO
/****** Object:  UserDefinedFunction [dbo].[fn_TaoID_TiepTheo]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create function [dbo].[fn_TaoID_TiepTheo](@ID_BanDau nchar(12)) returns nchar(12)
as
	begin
		declare @chuoi_NgayTuID nchar(6), @date_NgayTuID date, @date_BayGio date, @TienTo nchar(2)
		set @chuoi_NgayTuID = substring(@ID_BanDau, 3, 8)
		set @date_BayGio = getdate()
		set @TienTo = left(@ID_BanDau, 2)
		if(@chuoi_NgayTuID = dbo.fn_DateToString(@date_BayGio))
			begin
				declare @temp int
				set @temp = cast(right(@ID_BanDau, 4) as int)
				return concat(@TienTo, dbo.fn_DateToString(@date_BayGio), right('0000' + convert(varchar, @temp + 1), 4))
			end
		return concat(@TienTo, dbo.fn_DateToString(@date_BayGio), '0001')
	end

GO
/****** Object:  Table [dbo].[Ban]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ban](
	[MaBan] [nchar](5) NOT NULL,
	[MaTang] [nchar](5) NOT NULL,
	[TenBan] [nvarchar](20) NOT NULL,
	[TrangThai] [tinyint] NULL DEFAULT ((0)),
 CONSTRAINT [pk_Ban] PRIMARY KEY CLUSTERED 
(
	[MaBan] ASC,
	[MaTang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DonHang]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DonHang](
	[MaDonHang] [nchar](12) NOT NULL,
	[MaThucUong] [nchar](5) NOT NULL,
	[MaLoai] [nchar](4) NOT NULL,
	[SoLuong] [tinyint] NOT NULL,
	[MaHD] [nchar](12) NOT NULL,
 CONSTRAINT [pk_DonHang] PRIMARY KEY CLUSTERED 
(
	[MaDonHang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HoaDon]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HoaDon](
	[MaHD] [nchar](12) NOT NULL,
	[NgayHD] [date] NOT NULL,
	[MaNV] [nvarchar](20) NOT NULL,
	[MaBan] [nchar](5) NOT NULL,
	[MaTang] [nchar](5) NOT NULL,
	[TongTien] [int] NULL,
	[MaSuKien] [nvarchar](10) NULL,
	[DuaTruoc] [int] NULL,
	[ConLai] [int] NULL,
 CONSTRAINT [pk_HoaDon] PRIMARY KEY CLUSTERED 
(
	[MaHD] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LoaiNuoc]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiNuoc](
	[MaLoai] [nchar](4) NOT NULL,
	[TenLoai] [nvarchar](20) NOT NULL,
 CONSTRAINT [pk_LoaiNuoc] PRIMARY KEY CLUSTERED 
(
	[MaLoai] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SuKien]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuKien](
	[MaSuKien] [nvarchar](10) NOT NULL,
	[TenSuKien] [nvarchar](256) NULL,
	[TyLeGiam] [tinyint] NULL DEFAULT ((0)),
	[SoLuong] [smallint] NULL,
	[ThoiGian_BD] [datetime] NULL,
	[ThoiGian_KT] [datetime] NULL,
 CONSTRAINT [pk_SuKien] PRIMARY KEY CLUSTERED 
(
	[MaSuKien] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TaiKhoan]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaiKhoan](
	[TenDangNhap] [nvarchar](20) NOT NULL,
	[HoTen] [nvarchar](100) NULL,
	[MatKhau] [nvarchar](100) NOT NULL,
	[LoaiTaiKhoan] [bit] NULL DEFAULT ((0)),
	[CMND] [nvarchar](10) NOT NULL,
	[NgaySinh] [date] NOT NULL,
	[SoDT] [nvarchar](10) NOT NULL,
	[ConLam] [bit] NULL DEFAULT ((1)),
 CONSTRAINT [pk_TaiKhoan] PRIMARY KEY CLUSTERED 
(
	[TenDangNhap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ThucUong]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThucUong](
	[MaThucUong] [nchar](5) NOT NULL,
	[MaLoai] [nchar](4) NOT NULL,
	[TenThucUong] [nvarchar](50) NOT NULL,
	[DonGia] [smallmoney] NULL DEFAULT ((0)),
	[KhaDung] [bit] NULL DEFAULT ((1)),
 CONSTRAINT [pk_ThucUong] PRIMARY KEY CLUSTERED 
(
	[MaThucUong] ASC,
	[MaLoai] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN01', N'TANG0', N'Bàn số 1', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN01', N'TANG1', N'Bàn số 1', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN02', N'TANG0', N'Bàn số 2', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN02', N'TANG1', N'Bàn số 2', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN03', N'TANG0', N'Bàn số 3', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN03', N'TANG1', N'Bàn số 3', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN04', N'TANG0', N'Bàn số 4', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN04', N'TANG1', N'Bàn số 4', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN05', N'TANG0', N'Bàn số 5', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN05', N'TANG1', N'Bàn số 5', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN06', N'TANG0', N'Bàn số 6', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN06', N'TANG1', N'Bàn số 6', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN07', N'TANG0', N'Bàn số 7', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN07', N'TANG1', N'Bàn số 7', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN08', N'TANG0', N'Bàn số 8', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN08', N'TANG1', N'Bàn số 8', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN09', N'TANG0', N'Bàn số 9', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN09', N'TANG1', N'Bàn số 9', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN10', N'TANG0', N'Bàn số 10', 0)
INSERT [dbo].[Ban] ([MaBan], [MaTang], [TenBan], [TrangThai]) VALUES (N'BAN10', N'TANG1', N'Bàn số 10', 0)
INSERT [dbo].[LoaiNuoc] ([MaLoai], [TenLoai]) VALUES (N'Lanh', N'Cold drinks')
INSERT [dbo].[LoaiNuoc] ([MaLoai], [TenLoai]) VALUES (N'Nong', N'Hot drinks')
INSERT [dbo].[SuKien] ([MaSuKien], [TenSuKien], [TyLeGiam], [SoLuong], [ThoiGian_BD], [ThoiGian_KT]) VALUES (N'NONAME', N'', 0, 0, CAST(N'1900-01-01 00:00:00.000' AS DateTime), CAST(N'1900-01-01 00:00:00.000' AS DateTime))
INSERT [dbo].[TaiKhoan] ([TenDangNhap], [HoTen], [MatKhau], [LoaiTaiKhoan], [CMND], [NgaySinh], [SoDT], [ConLam]) VALUES (N'admin', N'', N'$2y$12$rihHzgA0E.Gg3vpuPA9H1OzujEStxSqS5D0Ic5cjy9hhu9402eNfG', 0, N'0', CAST(N'1980-01-01' AS Date), N'0', 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'AVOCA', N'Lanh', N'Avocado', 30000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'BLACK', N'Nong', N'Chocolate', 70000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'BLLEM', N'Lanh', N'Blueberry Lemonade', 80000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'CAPPU', N'Nong', N'Cappuccino', 50000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'CARAM', N'Lanh', N'Salted Caramel', 80000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'ESPRE', N'Nong', N'Espresso', 30000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'EXTR1', N'Nong', N'Extra Cream', 10000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'EXTR2', N'Nong', N'Extra Marshmallows', 10000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'LATTE', N'Lanh', N'Latte', 80000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'LATTE', N'Nong', N'Latte', 65000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'MAPAS', N'Lanh', N'Mango & Passion Fruit', 80000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'MOCHA', N'Lanh', N'Mocha', 80000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'MOCHA', N'Nong', N'Mocha', 65000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'ORANG', N'Lanh', N'Orange Juice', 40000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'STRAW', N'Lanh', N'Strawberry Juice', 40000.0000, 1)
INSERT [dbo].[ThucUong] ([MaThucUong], [MaLoai], [TenThucUong], [DonGia], [KhaDung]) VALUES (N'WHITE', N'Nong', N'White Chocolate', 70000.0000, 1)
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Ban__FCF7D724779BD5DD]    Script Date: 12/12/2018 8:58:05 SA ******/
ALTER TABLE [dbo].[Ban] ADD UNIQUE NONCLUSTERED 
(
	[MaBan] ASC,
	[MaTang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__DonHang__B050DEDDFD9FA84F]    Script Date: 12/12/2018 8:58:05 SA ******/
ALTER TABLE [dbo].[DonHang] ADD UNIQUE NONCLUSTERED 
(
	[MaThucUong] ASC,
	[MaLoai] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__TaiKhoan__F67C8D0BDB71F3CC]    Script Date: 12/12/2018 8:58:05 SA ******/
ALTER TABLE [dbo].[TaiKhoan] ADD UNIQUE NONCLUSTERED 
(
	[CMND] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__ThucUong__B050DEDD468DB619]    Script Date: 12/12/2018 8:58:05 SA ******/
ALTER TABLE [dbo].[ThucUong] ADD UNIQUE NONCLUSTERED 
(
	[MaThucUong] ASC,
	[MaLoai] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DonHang] ADD  DEFAULT ((0)) FOR [SoLuong]
GO
ALTER TABLE [dbo].[HoaDon] ADD  DEFAULT ((0)) FOR [TongTien]
GO
ALTER TABLE [dbo].[HoaDon] ADD  DEFAULT ((0)) FOR [DuaTruoc]
GO
ALTER TABLE [dbo].[HoaDon] ADD  DEFAULT ((0)) FOR [ConLai]
GO
ALTER TABLE [dbo].[DonHang]  WITH CHECK ADD FOREIGN KEY([MaThucUong], [MaLoai])
REFERENCES [dbo].[ThucUong] ([MaThucUong], [MaLoai])
GO
ALTER TABLE [dbo].[DonHang]  WITH CHECK ADD FOREIGN KEY([MaHD])
REFERENCES [dbo].[HoaDon] ([MaHD])
GO
ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD FOREIGN KEY([MaBan], [MaTang])
REFERENCES [dbo].[Ban] ([MaBan], [MaTang])
GO
ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD FOREIGN KEY([MaNV])
REFERENCES [dbo].[TaiKhoan] ([TenDangNhap])
GO
ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD FOREIGN KEY([MaSuKien])
REFERENCES [dbo].[SuKien] ([MaSuKien])
GO
ALTER TABLE [dbo].[ThucUong]  WITH CHECK ADD FOREIGN KEY([MaLoai])
REFERENCES [dbo].[LoaiNuoc] ([MaLoai])
GO
/****** Object:  StoredProcedure [dbo].[usp_ThemBan]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---------------------------------------
create proc [dbo].[usp_ThemBan]
	@MaBan nchar(5),
	@MaTang nchar(5),
	@TenBan nvarchar(20),
	@TrangThai tinyint
as
if exists (select * from Ban where MaBan = @MaBan and MaTang = @MaTang)
	print N'Đã tồn tại'
else
	insert into Ban values (@MaBan, @MaTang, @TenBan, @TrangThai)

GO
/****** Object:  StoredProcedure [dbo].[usp_ThemDonHang]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
---------------------------------------
create proc [dbo].[usp_ThemDonHang]
	@MaDonHang nchar(12),
	@MaThucUong nchar(5),
	@MaLoai nchar(4),
	@SoLuong tinyint,
	@MaHD nchar(12)
as
if exists (select * from DonHang where MaDonHang = @MaDonHang)
	print N'Đã tồn tại'
else
	insert into DonHang values (@MaDonHang, @MaThucUong, @MaLoai, @SoLuong, @MaHD)

GO
/****** Object:  StoredProcedure [dbo].[usp_ThemHoaDon]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---------------------------------------
create proc [dbo].[usp_ThemHoaDon]
	@MaHD nchar(12),
	@NgayHD date,
	@MaNV nvarchar(20),
	@MaBan nchar(5),
	@MaTang nchar(5),
	@TongTien int,
	@MaSuKien nvarchar(10),
	@DuaTruoc int,
	@ConLai int
as
if exists (select * from HoaDon where MaHD = @MaHD)
	print N'Đã tồn tại'
else
	begin
		insert into HoaDon values (@MaHD, @NgayHD, @MaNV, @MaBan, @MaTang, @TongTien, @MaSuKien, @DuaTruoc, @ConLai)
		if (select SoLuong from SuKien where MaSuKien = @MaSuKien) > 0
			begin
				update SuKien
				set SoLuong = SoLuong - 1
				where MaSuKien = @MaSuKien
			end
	end

GO
/****** Object:  StoredProcedure [dbo].[usp_ThemLoaiNuoc]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------THỦ TỤC NHẬP LIỆU------------------------------------

create proc [dbo].[usp_ThemLoaiNuoc]
	@MaLoai nchar(4),
	@TenLoai nvarchar(20)
as
if exists (select * from LoaiNuoc where MaLoai = @MaLoai)
	print N'Đã tồn tại'
else
	insert into LoaiNuoc values (@MaLoai, @TenLoai)

GO
/****** Object:  StoredProcedure [dbo].[usp_ThemSuKien]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---------------------------------------
create proc [dbo].[usp_ThemSuKien]
	@MaSuKien nvarchar(10),
	@TenSuKien nvarchar(100),
	@TyLeGiam tinyint,
	@SoLuong smallint,
	@ThoiGian_BD datetime,
	@ThoiGian_KT datetime
as
if exists (select * from SuKien where MaSuKien = @MaSuKien)
	print N'Đã tồn tại'
else
	insert into SuKien values (@MaSuKien, @TenSuKien, @TyLeGiam, @SoLuong, @ThoiGian_BD, @ThoiGian_KT)

GO
/****** Object:  StoredProcedure [dbo].[usp_ThemTaiKhoan]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---------------------------------------
create proc [dbo].[usp_ThemTaiKhoan]
	@TenDangNhap nvarchar(20),
	@HoTen nvarchar(100),
	@MatKhau nvarchar(100),
	@LoaiTaiKhoan bit,
	@CMND nvarchar(10),
	@NgaySinh date,
	@SoDT nvarchar(10),
	@ConLam bit
as
if exists (select * from TaiKhoan where TenDangNhap = @TenDangNhap)
	print N'Đã tồn tại'
else
	insert into TaiKhoan values (@TenDangNhap, @HoTen, @MatKhau, @LoaiTaiKhoan, @CMND, @NgaySinh, @SoDT, @ConLam)

GO
/****** Object:  StoredProcedure [dbo].[usp_ThemThucUong]    Script Date: 12/12/2018 8:58:05 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---------------------------------------
create proc [dbo].[usp_ThemThucUong]
	@MaThucUong nchar(5),
	@MaLoai nchar(4),
	@TenThucUong nvarchar(50),
	@DonGia smallmoney,
	@KhaDung bit
as
if exists (select * from ThucUong where MaLoai = @MaLoai and MaThucUong = @MaThucUong)
	print N'Đã tồn tại'
else
	insert into ThucUong values (@MaThucUong, @MaLoai, @TenThucUong, @DonGia, @KhaDung)

GO
USE [master]
GO
ALTER DATABASE [CafeRestaurant] SET  READ_WRITE 
GO
