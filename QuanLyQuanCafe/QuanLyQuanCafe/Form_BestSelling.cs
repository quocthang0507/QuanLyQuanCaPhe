using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QuanLyQuanCafe
{
	public partial class Form_BestSelling : Form
	{
		private sealed class DrinkTotal
		{
			public string MaLoai { get; set; }
			public string MaThucUong { get; set; }
			public int SoLuong { get; set; }
		}
		int nam = 0, thang = 0, ngay = 0, quy = 0;

		#region Events

		public Form_BestSelling()
		{
			InitializeComponent();
		}

		public Form_BestSelling(int nam, int quy, int thang, int ngay)
		{
			InitializeComponent();
			this.nam = nam;
			this.quy = quy;
			this.thang = thang;
			this.ngay = ngay;
		}

		private void Form_BestSelling_Load(object sender, EventArgs e)
		{
			if (nam == 0)
				GetDateTime();
			cbx_ThoiGian.SelectedIndex = 0;
		}

		private void cbx_ThoiGian_SelectedIndexChanged(object sender, EventArgs e)
		{
			CountAllDrinkOrder(cbx_ThoiGian.SelectedIndex);
		}

		#endregion

		#region Methods

		private void GetDateTime()
		{
			DateTime today = DateTime.Today;
			nam = today.Year;
			thang = today.Month;
			quy = (thang + 2) / 3;
			ngay = today.Day;
		}

		/// <summary>
		/// Thống kê đơn hàng tất cả các loại nước uống
		/// </summary>
		/// <param name="indexTime"></param>
		void CountAllDrinkOrder(int indexTime)
		{
			using (var database = new CafeRestaurantEntities())
			{
				string period = "1 = 1";
				switch (indexTime)
				{
					case 1:
						period = "YEAR(h.NgayHD) = @year";
						break;
					case 2:
						period = "YEAR(h.NgayHD) = @year AND (MONTH(h.NgayHD) + 2) / 3 = @quarter";
						break;
					case 3:
						period = "YEAR(h.NgayHD) = @year AND MONTH(h.NgayHD) = @month";
						break;
					case 4:
						period = "YEAR(h.NgayHD) = @year AND MONTH(h.NgayHD) = @month AND DAY(h.NgayHD) = @day";
						break;
				}

				var totals = database.Database.SqlQuery<DrinkTotal>(
					@"SELECT d.MaLoai, d.MaThucUong, SUM(CONVERT(int, d.SoLuong)) AS SoLuong
					  FROM dbo.DonHang d JOIN dbo.HoaDon h ON h.MaHD = d.MaHD
					  WHERE NOT EXISTS (SELECT 1 FROM dbo.HoanTienHoaDon r WHERE r.MaHD = h.MaHD) AND " + period +
					  " GROUP BY d.MaLoai, d.MaThucUong",
					new SqlParameter("@year", nam), new SqlParameter("@quarter", quy),
					new SqlParameter("@month", thang), new SqlParameter("@day", ngay)).ToList();
				var drinks = database.ThucUong.ToList();
				grid_Drink.DataSource = drinks.Select(item => new DrinkOrder(item.MaLoai, item.MaThucUong, item.TenThucUong)
				{
					SoLuong = totals.Where(t => t.MaLoai == item.MaLoai && t.MaThucUong == item.MaThucUong)
						.Select(t => t.SoLuong).FirstOrDefault()
				}).OrderByDescending(d => d.SoLuong).ThenBy(d => d.TenThucUong).ToList();
			}
		}
		#endregion
	}
}
