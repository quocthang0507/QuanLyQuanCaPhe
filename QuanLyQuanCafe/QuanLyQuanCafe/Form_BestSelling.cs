using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	public partial class Form_BestSelling : Form
	{
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
				var orders = database.DonHang.AsQueryable();
				switch (indexTime)
				{
					case 1:
						orders = orders.Where(o => o.HoaDon.NgayHD.Year == nam);
						break;
					case 2:
						orders = orders.Where(o => o.HoaDon.NgayHD.Year == nam && (o.HoaDon.NgayHD.Month + 2) / 3 == quy);
						break;
					case 3:
						orders = orders.Where(o => o.HoaDon.NgayHD.Year == nam && o.HoaDon.NgayHD.Month == thang);
						break;
					case 4:
						orders = orders.Where(o => o.HoaDon.NgayHD.Year == nam && o.HoaDon.NgayHD.Month == thang && o.HoaDon.NgayHD.Day == ngay);
						break;
				}

				var totals = orders.GroupBy(o => new { o.MaLoai, o.MaThucUong })
					.Select(g => new { g.Key.MaLoai, g.Key.MaThucUong, SoLuong = g.Sum(o => (int)o.SoLuong) })
					.ToList();
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
