using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace QuanLyQuanCafe
{
	internal static class RefundService
	{
		internal static string PaymentMethod(string billId)
		{
			using (var database = new CafeRestaurantEntities())
				return database.Database.SqlQuery<string>(
					"SELECT PhuongThuc FROM dbo.ThanhToanHoaDon WHERE MaHD = @id",
					new SqlParameter("@id", billId)).FirstOrDefault();
		}

		internal static void Refund(string billId, string username, string method, string reason)
		{
			if (method != "CASH" && method != "TRANSFER")
				throw new InvalidOperationException("Vui lòng chọn phương thức hoàn tiền.");
			if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 256)
				throw new InvalidOperationException("Vui lòng nhập lý do hoàn tiền, tối đa 256 ký tự.");
			using (var database = new CafeRestaurantEntities())
			using (var transaction = database.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				bool isAdmin = database.TaiKhoan.AsNoTracking().Any(a => a.TenDangNhap == username && a.LoaiTaiKhoan == false);
				if (!isAdmin)
					throw new InvalidOperationException("Chỉ quản trị viên được hoàn tiền.");
				long shiftId = ShiftService.LockOpenShift(database);
				var bill = database.HoaDon.AsNoTracking().SingleOrDefault(b => b.MaHD == billId);
				if (bill == null || !bill.TongTien.HasValue || bill.TongTien < 0)
					throw new InvalidOperationException("Không tìm thấy hóa đơn hợp lệ để hoàn tiền.");
				bool refunded = database.Database.SqlQuery<int>(
					"SELECT COUNT(*) FROM dbo.HoanTienHoaDon WITH (UPDLOCK, HOLDLOCK) WHERE MaHD = @id",
					new SqlParameter("@id", billId)).Single() != 0;
				if (refunded)
					throw new InvalidOperationException("Hóa đơn đã được hoàn tiền.");
				string originalMethod = database.Database.SqlQuery<string>(
					"SELECT PhuongThuc FROM dbo.ThanhToanHoaDon WHERE MaHD = @id",
					new SqlParameter("@id", billId)).FirstOrDefault();
				if (originalMethod != null && originalMethod != method)
					throw new InvalidOperationException("Phương thức hoàn phải trùng phương thức thanh toán.");
				database.Database.ExecuteSqlCommand(
					@"INSERT INTO dbo.HoanTienHoaDon (MaHD, MaCa, NguoiHoan, LucHoan, PhuongThuc, LyDo)
					  VALUES (@id, @shift, @user, SYSDATETIME(), @method, @reason)",
					new SqlParameter("@id", billId), new SqlParameter("@shift", shiftId),
					new SqlParameter("@user", username), new SqlParameter("@method", method),
					new SqlParameter("@reason", reason.Trim()));
				transaction.Commit();
			}
		}
	}
}
