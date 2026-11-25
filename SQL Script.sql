USE [master]
GO
---------------------------------------
--drop database CafeRestaurant
go
---------------------------------------
create database CafeRestaurant
go
---------------------------------------
use CafeRestaurant
go
---------------------------------------
SET DATEFORMAT dmy; 
go
---------------------------------------
create table LoaiNuoc
(
	MaLoai nchar(4) not null,
	TenLoai nvarchar(20) not null,
	constraint pk_LoaiNuoc primary key (MaLoai),
);
go
---------------------------------------
create table ThucUong
(
	MaThucUong nchar(5) not null,
	MaLoai nchar(4) not null,
	TenThucUong nvarchar(50) not null,
	DonGia smallmoney default 0,
	KhaDung bit default 1,
	constraint pk_ThucUong primary key (MaThucUong, MaLoai),
	foreign key (MaLoai) references LoaiNuoc(MaLoai),
	unique (MaThucUong, MaLoai)
);
go
---------------------------------------
create table TaiKhoan
(
	TenDangNhap nvarchar(20) not null,
	HoTen nvarchar(100) null,
	MatKhau nvarchar(100) not null,
	LoaiTaiKhoan bit default 0,
	CMND nvarchar(10) not null unique,
	NgaySinh date not null,
	SoDT nvarchar(10) not null,
	ConLam bit default 1,
	constraint pk_TaiKhoan primary key (TenDangNhap)
);
go
---------------------------------------
create table Ban
(
	MaBan nchar(5) not null,
	MaTang nchar(5) not null,
	TenBan nvarchar(20) not null,
	TrangThai tinyint default 0,
	constraint pk_Ban primary key (MaBan, MaTang),
	unique (MaBan, MaTang)
);
go
---------------------------------------
create table SuKien
(
	MaSuKien nvarchar(10) not null,
	TenSuKien nvarchar(256) null,
	TyLeGiam tinyint null default 0,
	SoLuong smallint null,
	ThoiGian_BD datetime null,
	ThoiGian_KT datetime null,
	constraint pk_SuKien primary key (MaSuKien)
);
go
---------------------------------------
create table HoaDon
(
	MaHD nchar(12) not null,
	NgayHD date not null,
	MaNV nvarchar(20) not null,
	MaBan nchar(5) not null,
	MaTang nchar(5) not null,
	TongTien int default 0,
	MaSuKien nvarchar(10) null,
	DuaTruoc int default 0,
	ConLai int default 0,
	constraint pk_HoaDon primary key (MaHD),
	foreign key (MaBan, MaTang) references Ban(MaBan, MaTang),
	foreign key (MaNV) references TaiKhoan(TenDangNhap),
	foreign key (MaSuKien) references SuKien(MaSuKien)
);
go
---------------------------------------
create table DonHang
(
	MaDonHang nchar(12) not null,
	MaThucUong nchar(5) not null,
	MaLoai nchar(4) not null,
	SoLuong tinyint not null default 0,
	MaHD nchar(12) not null,
	constraint pk_DonHang primary key (MaDonHang),
	foreign key (MaThucUong, MaLoai) references ThucUong(MaThucUong, MaLoai),
	foreign key (MaHD) references HoaDon(MaHD)
);
go

------------------------------------THỦ TỤC NHẬP LIỆU------------------------------------

create proc usp_ThemLoaiNuoc
	@MaLoai nchar(4),
	@TenLoai nvarchar(20)
as
if exists (select * from LoaiNuoc where MaLoai = @MaLoai)
	print N'Đã tồn tại'
else
	insert into LoaiNuoc values (@MaLoai, @TenLoai)
go
----
exec usp_ThemLoaiNuoc 'Nong','Hot drinks'
exec usp_ThemLoaiNuoc 'Lanh','Cold drinks'
go

---------------------------------------
create proc usp_ThemThucUong
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
go
----
exec usp_ThemThucUong 'CAPPU', 'Nong', 'Cappuccino', 50000, 1
exec usp_ThemThucUong 'LATTE', 'Nong', 'Latte', 65000, 1
exec usp_ThemThucUong 'MOCHA', 'Nong', 'Mocha', 65000, 1
exec usp_ThemThucUong 'BLACK', 'Nong', 'Chocolate', 70000, 1
exec usp_ThemThucUong 'WHITE', 'Nong', 'White Chocolate', 70000, 1
exec usp_ThemThucUong 'ESPRE', 'Nong', 'Espresso', 30000, 1
exec usp_ThemThucUong 'EXTR1', 'Nong', 'Extra Cream', 10000, 1
exec usp_ThemThucUong 'EXTR2', 'Nong', 'Extra Marshmallows', 10000, 1
exec usp_ThemThucUong 'LATTE', 'Lanh', 'Latte', 80000, 1
exec usp_ThemThucUong 'MOCHA', 'Lanh', 'Mocha', 80000, 1
exec usp_ThemThucUong 'CARAM', 'Lanh', 'Salted Caramel', 80000, 1
exec usp_ThemThucUong 'BLLEM', 'Lanh', 'Blueberry Lemonade', 80000, 1
exec usp_ThemThucUong 'MAPAS', 'Lanh', 'Mango & Passion Fruit', 80000, 1
exec usp_ThemThucUong 'AVOCA', 'Lanh', 'Avocado', 30000, 1
exec usp_ThemThucUong 'STRAW', 'Lanh', 'Strawberry Juice', 40000, 1
exec usp_ThemThucUong 'ORANG', 'Lanh', 'Orange Juice', 40000, 1
go

---------------------------------------
create proc usp_ThemBan
	@MaBan nchar(5),
	@MaTang nchar(5),
	@TenBan nvarchar(20),
	@TrangThai tinyint
as
if exists (select * from Ban where MaBan = @MaBan and MaTang = @MaTang)
	print N'Đã tồn tại'
else
	insert into Ban values (@MaBan, @MaTang, @TenBan, @TrangThai)
go
----
exec usp_ThemBan 'BAN01', 'TANG0', N'Bàn số 1', 0
exec usp_ThemBan 'BAN02', 'TANG0', N'Bàn số 2', 0
exec usp_ThemBan 'BAN03', 'TANG0', N'Bàn số 3', 0
exec usp_ThemBan 'BAN04', 'TANG0', N'Bàn số 4', 0
exec usp_ThemBan 'BAN05', 'TANG0', N'Bàn số 5', 0
exec usp_ThemBan 'BAN06', 'TANG0', N'Bàn số 6', 0
exec usp_ThemBan 'BAN07', 'TANG0', N'Bàn số 7', 0
exec usp_ThemBan 'BAN08', 'TANG0', N'Bàn số 8', 0
exec usp_ThemBan 'BAN09', 'TANG0', N'Bàn số 9', 0
exec usp_ThemBan 'BAN10', 'TANG0', N'Bàn số 10', 0
exec usp_ThemBan 'BAN01', 'TANG1', N'Bàn số 1', 0
exec usp_ThemBan 'BAN02', 'TANG1', N'Bàn số 2', 0
exec usp_ThemBan 'BAN03', 'TANG1', N'Bàn số 3', 0
exec usp_ThemBan 'BAN04', 'TANG1', N'Bàn số 4', 0
exec usp_ThemBan 'BAN05', 'TANG1', N'Bàn số 5', 0
exec usp_ThemBan 'BAN06', 'TANG1', N'Bàn số 6', 0
exec usp_ThemBan 'BAN07', 'TANG1', N'Bàn số 7', 0
exec usp_ThemBan 'BAN08', 'TANG1', N'Bàn số 8', 0
exec usp_ThemBan 'BAN09', 'TANG1', N'Bàn số 9', 0
exec usp_ThemBan 'BAN10', 'TANG1', N'Bàn số 10', 0
go

---------------------------------------
create proc usp_ThemTaiKhoan
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
go
----
exec usp_ThemtaiKhoan 'admin', '', N'$2y$12$rihHzgA0E.Gg3vpuPA9H1OzujEStxSqS5D0Ic5cjy9hhu9402eNfG', 0, 0, '1/1/1980', 0, 1
go

---------------------------------------
create proc usp_ThemSuKien
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
go
----
exec usp_ThemSuKien 'NONAME', '', 0, 0, '', ''
go

---------------------------------------
create proc usp_ThemHoaDon
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
go
---------------------------------------
create proc usp_ThemDonHang
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
go

-----------------------------------------------------------------------------------------

------------------------------------HÀM SINH MÃ HÓA ĐƠN VÀ MÃ ĐƠN HÀNG------------------------------------
--drop function fn_DateToString
create function fn_DateToString(@Ngay date) returns nchar(6)
as
	begin
		declare @chuoi nchar(6), @day tinyint, @month tinyint, @year int
		set @day = datepart(day, @Ngay)
		set @month = datepart(month, @Ngay)
		set @year  = datepart(year, @Ngay)
		set @chuoi = concat(right('00' + convert(varchar, @day), 2), right('00' + convert(varchar, @month), 2), right(@year, 2))
		return @chuoi
	end
go

create function fn_TaoID_TiepTheo(@ID_BanDau nchar(12)) returns nchar(12)
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
go

create function fn_SinhMaHD() returns nchar(12)
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
go

--print dbo.fn_SinhMaHD()

create function fn_SinhMaDH() returns nchar(12)
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
go

--print dbo.fn_SinhMaDH()
------------------------------------------------HẾT----------------------------------------------------------