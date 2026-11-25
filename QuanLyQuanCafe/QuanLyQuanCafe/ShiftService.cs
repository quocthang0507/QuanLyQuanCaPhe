using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace QuanLyQuanCafe
{
	internal sealed class ShiftSummary
	{
		public long MaCa { get; set; }
		public DateTime LucMo { get; set; }
		public long TienDauCa { get; set; }
		public long TienMatBanHang { get; set; }
		public long TienMatHoan { get; set; }
		public long Expected => checked(TienDauCa + TienMatBanHang - TienMatHoan);
	}

	internal static class ShiftService
	{
		internal static long LockOpenShift(CafeRestaurantEntities database)
		{
			var shifts = database.Database.SqlQuery<long>(
				"SELECT MaCa FROM dbo.CaLamViec WITH (UPDLOCK, HOLDLOCK) WHERE LucDong IS NULL").ToList();
			if (shifts.Count != 1)
				throw new InvalidOperationException("Cần mở một ca trước khi thanh toán hoặc hoàn tiền.");
			return shifts[0];
		}

		internal static void Open(string username, long openingCash)
		{
			if (openingCash < 0)
				throw new ArgumentOutOfRangeException(nameof(openingCash));
			using (var database = new CafeRestaurantEntities())
			using (var transaction = database.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				bool exists = database.Database.SqlQuery<int>(
					"SELECT COUNT(*) FROM dbo.CaLamViec WITH (UPDLOCK, HOLDLOCK) WHERE LucDong IS NULL").Single() != 0;
				if (exists)
					throw new InvalidOperationException("Đã có ca đang mở.");
				database.Database.ExecuteSqlCommand(
					"INSERT INTO dbo.CaLamViec (NguoiMo, LucMo, TienDauCa) VALUES (@user, SYSDATETIME(), @cash)",
					new SqlParameter("@user", username), new SqlParameter("@cash", openingCash));
				transaction.Commit();
			}
		}

		internal static ShiftSummary Current(CafeRestaurantEntities database)
		{
			return database.Database.SqlQuery<ShiftSummary>(
				@"SELECT c.MaCa, c.LucMo, c.TienDauCa,
				    COALESCE((SELECT SUM(CONVERT(bigint, h.TongTien))
				      FROM dbo.ThanhToanHoaDon t JOIN dbo.HoaDon h ON h.MaHD = t.MaHD
				      WHERE t.MaCa = c.MaCa AND t.PhuongThuc = 'CASH'), 0) AS TienMatBanHang,
				    COALESCE((SELECT SUM(CONVERT(bigint, h.TongTien))
				      FROM dbo.HoanTienHoaDon r JOIN dbo.HoaDon h ON h.MaHD = r.MaHD
				      WHERE r.MaCa = c.MaCa AND r.PhuongThuc = 'CASH'), 0) AS TienMatHoan
				  FROM dbo.CaLamViec c WHERE c.LucDong IS NULL").SingleOrDefault();
		}

		internal static ShiftSummary Close(string username, long closingCash)
		{
			if (closingCash < 0)
				throw new ArgumentOutOfRangeException(nameof(closingCash));
			using (var database = new CafeRestaurantEntities())
			using (var transaction = database.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				long shiftId = LockOpenShift(database);
				ShiftSummary summary = Current(database);
				if (summary == null || summary.MaCa != shiftId)
					throw new InvalidOperationException("Không tìm thấy ca đang mở.");
				int count = database.Database.ExecuteSqlCommand(
					@"UPDATE dbo.CaLamViec
					  SET NguoiDong = @user, LucDong = SYSDATETIME(), TienCuoiCa = @cash, TienDuKien = @expected
					  WHERE MaCa = @id AND LucDong IS NULL",
					new SqlParameter("@user", username), new SqlParameter("@cash", closingCash),
					new SqlParameter("@expected", summary.Expected), new SqlParameter("@id", shiftId));
				if (count != 1)
					throw new InvalidOperationException("Ca đã được chốt.");
				transaction.Commit();
				return summary;
			}
		}
	}
}
