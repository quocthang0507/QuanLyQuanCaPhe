using System;
using System.Collections;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QuanLyQuanCafe;

internal static class Program
{
	private static object Call(string type, string method, params object[] args) =>
		typeof(User).Assembly.GetType("QuanLyQuanCafe." + type)
			.GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, args);

	private static void Assert(bool condition, string message)
	{
		if (!condition) throw new InvalidOperationException(message);
	}

	private static void Reject(Action action)
	{
		try { action(); }
		catch (TargetInvocationException ex) when (ex.InnerException is InvalidOperationException) { return; }
		throw new InvalidOperationException("Expected operation to be rejected.");
	}

	[STAThread]
	private static int Main()
	{
		try
		{
			EnsureIsolatedDatabase();
			Run();
			Console.WriteLine("Integration checks passed.");
			return 0;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex);
			return 1;
		}
	}

	private static void EnsureIsolatedDatabase()
	{
		string instance = Environment.GetEnvironmentVariable("CAFE_TEST_INSTANCE");
		var entity = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["CafeRestaurantEntities"].ConnectionString);
		var sql = new SqlConnectionStringBuilder(entity.ProviderConnectionString);
		if (instance == null || !Regex.IsMatch(instance, @"\ACopilotCafeTest_[0-9a-f]{32}\z") ||
			!string.Equals(sql.DataSource, @"(localdb)\" + instance, StringComparison.OrdinalIgnoreCase) ||
			!string.Equals(sql.InitialCatalog, "CafeRestaurant", StringComparison.Ordinal))
			throw new InvalidOperationException("Tests require a dedicated CopilotCafeTest LocalDB instance.");
	}

	private static void Run()
	{
		using (var db = new CafeRestaurantEntities())
		{
			db.Database.ExecuteSqlCommand(@"INSERT INTO dbo.TaiKhoan
				(TenDangNhap, HoTen, MatKhau, LoaiTaiKhoan, CMND, NgaySinh, SoDT, ConLam)
				VALUES ('teststaff', N'Test staff', @password, 1, '9898989898', '2000-01-01', '1234567890', 1)",
				new SqlParameter("@password", Bcrypt.HashPassword("test")));
			db.Database.ExecuteSqlCommand(@"INSERT INTO dbo.HoaDon (MaHD, NgayHD, MaNV, MaBan, MaTang, TongTien, MaSuKien, DuaTruoc, ConLai)
				VALUES ('HD0101010001', '2020-01-01', 'admin', 'BAN01', 'TANG0', 100000, 'NONAME', 100000, 0),
				       ('HD0101010002', '2020-01-01', 'admin', 'BAN02', 'TANG0', 65000, 'NONAME', 65000, 0)");
			db.Database.ExecuteSqlCommand(@"INSERT INTO dbo.DonHang (MaDonHang, MaThucUong, MaLoai, SoLuong, MaHD)
				VALUES ('DH0101010001', 'CAPPU', 'Nong', 2, 'HD0101010001'),
				       ('DH0101010002', 'LATTE', 'Nong', 1, 'HD0101010002')");
			db.Database.ExecuteSqlCommand("INSERT INTO dbo.GiaBanDonHang VALUES ('DH0101010002', 65000)");
			db.Database.ExecuteSqlCommand("UPDATE dbo.HoaDon SET DuaTruoc = NULL, ConLai = NULL WHERE MaHD = 'HD0101010001'");
		}
		var user = new User();
		Assert(user.Login("teststaff", "test"), "Active employee could not log in.");
		using (var db = new CafeRestaurantEntities())
		{
			db.TaiKhoan.Single(x => x.TenDangNhap == "teststaff").ConLam = false;
			db.SaveChanges();
		}
		Assert(!user.Login("teststaff", "test"), "Locked employee could log in.");
		Assert(user.OnlyChangeInfo("test", "Updated", "9898989898", new DateTime(2000, 1, 1), "1234567890"),
			"Profile update failed.");
		Assert(!user.Login("teststaff", "test"), "Profile update unlocked the employee.");
		Assert(((string)Call("BillReceipt", "Load", "HD0101010001")).Contains("không có dữ liệu giá"), "Historical price was guessed.");
		Assert(((string)Call("BillReceipt", "Load", "HD0101010001")).Contains("Khách đưa: không có dữ liệu"), "Missing payment amount was shown as zero.");
		Assert(((string)Call("BillReceipt", "Load", "HD0101010002")).Contains("65.000"), "Snapshot price was not shown.");

		Call("ShiftService", "Open", "admin", 200000L);
		Reject(() => Call("ShiftService", "Open", "admin", 0L));
		using (var db = new CafeRestaurantEntities())
			db.Database.ExecuteSqlCommand(@"INSERT INTO dbo.ThanhToanHoaDon (MaHD, MaCa, PhuongThuc, LucThanhToan)
				SELECT 'HD0101010002', MaCa, 'CASH', SYSDATETIME() FROM dbo.CaLamViec WHERE LucDong IS NULL");
		Reject(() => Call("RefundService", "Refund", "HD0101010002", "teststaff", "CASH", "No permission"));
		Reject(() => Call("RefundService", "Refund", "HD0101010002", "admin", "TRANSFER", "Wrong method"));
		Call("RefundService", "Refund", "HD0101010002", "admin", "CASH", "Test");
		Reject(() => Call("RefundService", "Refund", "HD0101010002", "admin", "CASH", "Duplicate"));
		using (var income = new Form_InCome(2020, 1, 1, 1))
		{
			long total = (long)typeof(Form_InCome).GetMethod("Income", BindingFlags.Instance | BindingFlags.NonPublic)
				.Invoke(income, new object[] { "1 = 1" });
			Assert(total == 100000, "Revenue includes a refunded invoice.");
		}
		using (var report = new Form_BestSelling(2020, 1, 1, 1))
		{
			typeof(Form_BestSelling).GetMethod("CountAllDrinkOrder", BindingFlags.Instance | BindingFlags.NonPublic)
				.Invoke(report, new object[] { 0 });
			var grid = (DataGridView)typeof(Form_BestSelling).GetField("grid_Drink", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(report);
			var latte = grid.Rows.Cast<DataGridViewRow>().Select(row => row.DataBoundItem).First(item =>
				(string)item.GetType().GetProperty("MaLoai").GetValue(item) == "Nong" &&
				(string)item.GetType().GetProperty("MaThucUong").GetValue(item) == "LATTE");
			Assert((int)latte.GetType().GetProperty("SoLuong").GetValue(latte) == 0, "Best-selling report includes a refunded item.");
		}
		object closed = Call("ShiftService", "Close", "admin", 201000L);
		Assert((long)closed.GetType().GetProperty("Expected").GetValue(closed) == 200000L, "Shift cash balance was wrong.");

		Call("DraftService", "Add", "BAN01", "TANG0", "CAPPU", "Nong", (byte)2);
		Call("DraftService", "Add", "BAN01", "TANG0", "LATTE", "Nong", (byte)1);
		Call("DraftService", "Add", "BAN02", "TANG0", "LATTE", "Nong", (byte)1);
		Reject(() => Call("DraftService", "Move", "BAN01", "TANG0", "BAN02", "TANG0"));
		Reject(() => Call("DraftService", "Cancel", "BAN01", "TANG0", "teststaff"));
		Call("DraftService", "Move", "BAN01", "TANG0", "BAN03", "TANG0");
		using (var db = new CafeRestaurantEntities())
		{
			var lines = (IList)Call("DraftService", "Lines", db, "BAN03", "TANG0");
			object firstLine = lines[0];
			lines[0] = lines[1];
			lines[1] = firstLine;
			using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
				Call("DraftService", "Consume", db, "BAN03", "TANG0", lines);
			Assert(db.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.ChiTietDonTam WHERE MaBan = 'BAN03' AND SoLuong = 2").Single() == 1,
				"Rollback lost the moved draft.");
		}
		Call("DraftService", "Cancel", "BAN03", "TANG0", "admin");
		object first = Call("TakeawayService", "Reserve");
		object second = Call("TakeawayService", "Reserve");
		string firstId = ((string)first.GetType().GetProperty("MaBan").GetValue(first)).Trim();
		string secondId = ((string)second.GetType().GetProperty("MaBan").GetValue(second)).Trim();
		Assert(firstId != secondId, "Takeaway orders shared a slot.");
		Call("DraftService", "Cancel", firstId, "MANG", "admin");
		object reused = Call("TakeawayService", "Reserve");
		Assert(((string)reused.GetType().GetProperty("MaBan").GetValue(reused)).Trim() == firstId, "Takeaway slot was not reused.");
	}
}
