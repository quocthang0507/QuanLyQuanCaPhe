using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	internal static class BillReceipt
	{
		private sealed class Line
		{
			public string TenThucUong { get; set; }
			public byte SoLuong { get; set; }
			public decimal? DonGia { get; set; }
		}

		internal static string Load(string billId)
		{
			using (var database = new CafeRestaurantEntities())
			{
				var bill = database.HoaDon.AsNoTracking().SingleOrDefault(b => b.MaHD == billId);
				if (bill == null)
					throw new InvalidOperationException("Không tìm thấy hóa đơn.");

				var lines = database.Database.SqlQuery<Line>(
					@"SELECT t.TenThucUong, d.SoLuong, g.DonGia
					  FROM dbo.DonHang d
					  JOIN dbo.ThucUong t ON t.MaThucUong = d.MaThucUong AND t.MaLoai = d.MaLoai
					  LEFT JOIN dbo.GiaBanDonHang g ON g.MaDonHang = d.MaDonHang
					  WHERE d.MaHD = @id ORDER BY d.MaDonHang",
					new SqlParameter("@id", billId)).ToList();
				string method = database.Database.SqlQuery<string>(
					"SELECT PhuongThuc FROM dbo.ThanhToanHoaDon WHERE MaHD = @id",
					new SqlParameter("@id", billId)).FirstOrDefault();
				bool refunded = database.Database.SqlQuery<int>(
					"SELECT COUNT(*) FROM dbo.HoanTienHoaDon WHERE MaHD = @id",
					new SqlParameter("@id", billId)).Single() != 0;
				bool takeaway = database.Database.SqlQuery<int>(
					"SELECT COUNT(*) FROM dbo.BanMangDi WHERE MaBan = @table AND MaTang = @floor",
					new SqlParameter("@table", bill.MaBan), new SqlParameter("@floor", bill.MaTang)).Single() != 0;
				var culture = CultureInfo.GetCultureInfo("vi-VN");
				var text = new List<string>
				{
					"HÓA ĐƠN " + bill.MaHD.Trim(),
					"Ngày: " + bill.NgayHD.ToString("dd/MM/yyyy"),
					"Nhân viên: " + bill.MaNV,
					takeaway ? "Loại đơn: mang đi" : "Bàn: " + bill.MaBan.Trim() + " - " + bill.MaTang.Trim(),
					"Thanh toán: " + (method == "CASH" ? "tiền mặt" : method == "TRANSFER" ? "chuyển khoản" : "không có dữ liệu"),
					refunded ? "TRẠNG THÁI: ĐÃ HOÀN TOÀN BỘ" : "TRẠNG THÁI: ĐÃ THANH TOÁN",
					"--------------------------------",
				};
				foreach (var line in lines)
				{
					string price = line.DonGia.HasValue
						? line.DonGia.Value.ToString("#,##0.####", culture) + " đ"
						: "không có dữ liệu giá";
					text.Add(line.TenThucUong + " x " + line.SoLuong + " — đơn giá " + price);
				}
				text.Add("--------------------------------");
				text.Add("Thành tiền: " + FormatAmount(bill.TongTien, culture));
				text.Add("Khuyến mãi: " + bill.MaSuKien);
				text.Add("Khách đưa: " + FormatAmount(bill.DuaTruoc, culture));
				text.Add("Tiền thừa: " + FormatAmount(bill.ConLai, culture));
				text.Add("CẢM ƠN QUÝ KHÁCH!");
				return string.Join(Environment.NewLine, text);
			}
		}

		private static string FormatAmount(int? value, CultureInfo culture)
		{
			return value.HasValue ? value.Value.ToString("N0", culture) + " đ" : "không có dữ liệu";
		}

		internal static void Print(IWin32Window owner, string text)
		{
			using (var document = new PrintDocument())
			using (var font = new Font("Segoe UI", 10))
			using (var dialog = new PrintDialog { Document = document })
			{
				int position = 0;
				document.BeginPrint += (sender, args) => position = 0;
				document.PrintPage += (sender, args) =>
				{
					var remaining = text.Substring(position);
					args.Graphics.MeasureString(remaining, font, args.MarginBounds.Size,
						StringFormat.GenericTypographic, out int chars, out int lines);
					if (chars == 0 && remaining.Length > 0)
						throw new InvalidOperationException("Khổ giấy quá nhỏ để in hóa đơn.");
					args.Graphics.DrawString(remaining.Substring(0, chars), font, Brushes.Black,
						args.MarginBounds, StringFormat.GenericTypographic);
					position += chars;
					args.HasMorePages = position < text.Length;
				};
				if (dialog.ShowDialog(owner) == DialogResult.OK)
					document.Print();
			}
		}
	}
}
