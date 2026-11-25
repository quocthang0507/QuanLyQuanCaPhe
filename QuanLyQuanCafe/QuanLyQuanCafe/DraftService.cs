using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace QuanLyQuanCafe
{
	internal sealed class DraftLine
	{
		public long MaDong { get; set; }
		public string MaThucUong { get; set; }
		public string MaLoai { get; set; }
		public byte SoLuong { get; set; }
	}

	internal sealed class DraftTable
	{
		public string MaBan { get; set; }
		public string MaTang { get; set; }
	}

	internal static class DraftService
	{
		internal static List<DraftTable> Tables(CafeRestaurantEntities database)
		{
			return database.Database.SqlQuery<DraftTable>("SELECT MaBan, MaTang FROM dbo.DonTam").ToList();
		}

		internal static List<DraftLine> Lines(CafeRestaurantEntities database, string table, string floor)
		{
			return database.Database.SqlQuery<DraftLine>(
				@"SELECT MaDong, MaThucUong, MaLoai, SoLuong FROM dbo.ChiTietDonTam
				  WHERE MaBan = @table AND MaTang = @floor ORDER BY MaDong",
				new SqlParameter("@table", table), new SqlParameter("@floor", floor)).ToList();
		}

		private static void LockTable(CafeRestaurantEntities database, string table, string floor)
		{
			database.Database.SqlQuery<int>(
				"SELECT COUNT(*) FROM dbo.DonTam WITH (UPDLOCK, HOLDLOCK) WHERE MaBan = @table AND MaTang = @floor",
				new SqlParameter("@table", table), new SqlParameter("@floor", floor)).Single();
		}

		internal static void Add(string table, string floor, string drink, string type, byte quantity)
		{
			if (quantity == 0)
				throw new InvalidOperationException("Số lượng phải lớn hơn 0.");
			using (var database = new CafeRestaurantEntities())
			using (var transaction = database.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				LockTable(database, table, floor);
				database.Database.ExecuteSqlCommand(
					@"IF NOT EXISTS (SELECT 1 FROM dbo.DonTam WHERE MaBan = @table AND MaTang = @floor)
					  INSERT INTO dbo.DonTam (MaBan, MaTang) VALUES (@table, @floor)",
					new SqlParameter("@table", table), new SqlParameter("@floor", floor));
				database.Database.ExecuteSqlCommand(
					@"INSERT INTO dbo.ChiTietDonTam (MaBan, MaTang, MaThucUong, MaLoai, SoLuong)
					  VALUES (@table, @floor, @drink, @type, @quantity)",
					new SqlParameter("@table", table), new SqlParameter("@floor", floor),
					new SqlParameter("@drink", drink), new SqlParameter("@type", type),
					new SqlParameter("@quantity", quantity));
				transaction.Commit();
			}
		}

		private static void RequireAdmin(CafeRestaurantEntities database, string actor)
		{
			if (!database.TaiKhoan.AsNoTracking().Any(a => a.TenDangNhap == actor && a.LoaiTaiKhoan == false))
				throw new InvalidOperationException("Chỉ quản trị viên được hủy đơn chưa thanh toán.");
		}

		internal static void RemoveLine(string table, string floor, long line, string actor)
		{
			using (var database = new CafeRestaurantEntities())
			using (var transaction = database.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				LockTable(database, table, floor);
				var lines = Lines(database, table, floor);
				if (!lines.Any(l => l.MaDong == line))
					throw new InvalidOperationException("Món đã thay đổi. Hãy chọn lại bàn.");
				if (lines.Count == 1)
					RequireAdmin(database, actor);
				int count = database.Database.ExecuteSqlCommand(
					"DELETE dbo.ChiTietDonTam WHERE MaDong = @id AND MaBan = @table AND MaTang = @floor",
					new SqlParameter("@id", line), new SqlParameter("@table", table), new SqlParameter("@floor", floor));
				if (count != 1)
					throw new InvalidOperationException("Không thể xóa món.");
				if (lines.Count == 1)
					database.Database.ExecuteSqlCommand("DELETE dbo.DonTam WHERE MaBan = @table AND MaTang = @floor",
						new SqlParameter("@table", table), new SqlParameter("@floor", floor));
				transaction.Commit();
			}
		}

		internal static void Cancel(string table, string floor, string actor)
		{
			using (var database = new CafeRestaurantEntities())
			using (var transaction = database.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				RequireAdmin(database, actor);
				LockTable(database, table, floor);
				int count = database.Database.ExecuteSqlCommand("DELETE dbo.DonTam WHERE MaBan = @table AND MaTang = @floor",
					new SqlParameter("@table", table), new SqlParameter("@floor", floor));
				if (count != 1)
					throw new InvalidOperationException("Đơn đã thay đổi. Hãy chọn lại bàn.");
				transaction.Commit();
			}
		}

		internal static void Move(string table, string floor, string newTable, string newFloor)
		{
			using (var database = new CafeRestaurantEntities())
			using (var transaction = database.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				LockTable(database, table, floor);
				LockTable(database, newTable, newFloor);
				int count = database.Database.ExecuteSqlCommand(
					@"UPDATE dbo.DonTam SET MaBan = @newTable, MaTang = @newFloor
					  WHERE MaBan = @table AND MaTang = @floor
					  AND NOT EXISTS (SELECT 1 FROM dbo.DonTam WHERE MaBan = @newTable AND MaTang = @newFloor)",
					new SqlParameter("@newTable", newTable), new SqlParameter("@newFloor", newFloor),
					new SqlParameter("@table", table), new SqlParameter("@floor", floor));
				if (count != 1)
					throw new InvalidOperationException("Bàn nguồn hoặc bàn đích đã thay đổi.");
				transaction.Commit();
			}
		}

		internal static void Consume(CafeRestaurantEntities database, string table, string floor, IReadOnlyList<DraftLine> expected)
		{
			LockTable(database, table, floor);
			var actual = Lines(database, table, floor);
			var orderedExpected = expected.OrderBy(line => line.MaDong).ToList();
			if (actual.Count != orderedExpected.Count || actual.Zip(orderedExpected, (a, b) =>
				a.MaDong == b.MaDong && a.MaThucUong.Trim() == b.MaThucUong.Trim() &&
				a.MaLoai.Trim() == b.MaLoai.Trim() && a.SoLuong == b.SoLuong).Any(equal => !equal))
				throw new InvalidOperationException("Đơn đã thay đổi. Hãy chọn lại bàn và tạo lại hóa đơn.");
			int count = database.Database.ExecuteSqlCommand("DELETE dbo.DonTam WHERE MaBan = @table AND MaTang = @floor",
				new SqlParameter("@table", table), new SqlParameter("@floor", floor));
			if (count != 1)
				throw new InvalidOperationException("Không tìm thấy đơn chưa thanh toán.");
		}
	}
}
