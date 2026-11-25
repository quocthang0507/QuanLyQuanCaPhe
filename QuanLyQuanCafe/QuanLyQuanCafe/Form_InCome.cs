using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QuanLyQuanCafe
{
	public partial class Form_InCome : Form
	{
		int nam = 0, quy = 0, thang = 0, ngay = 0;

		public Form_InCome()
		{
			InitializeComponent();
		}

		public Form_InCome(int nam, int quy, int thang, int ngay)
		{
			InitializeComponent();
			this.nam = nam;
			this.quy = quy;
			this.thang = thang;
			this.ngay = ngay;
		}

		private void Form_InCome_Load(object sender, EventArgs e)
		{
			if (nam == 0)
				GetDateTime();
			ShowIncome();
		}

		/// <summary>
		/// Gán thời gian hiện tại vào các biến
		/// </summary>
		void GetDateTime()
		{
			DateTime today = DateTime.Today;
			nam = today.Year;
			thang = today.Month;
			quy = (thang + 2) / 3;
			ngay = today.Day;
		}

		/// <summary>
		/// Xuất thu nhập qua các mốc thời gian
		/// </summary>
		void ShowIncome()
		{
			ShowIncome_Total();
			ShowIncome_Year();
			ShowIncome_Quarter();
			ShowIncome_Month();
			ShowIncome_Day();
		}

		private void ShowIncome_Day()
		{
			tbx_TongTrongNgay.Text = Income("YEAR(h.NgayHD) = @year AND MONTH(h.NgayHD) = @month AND DAY(h.NgayHD) = @day").ToString();
		}

		private void ShowIncome_Month()
		{
			tbx_TongTrongThang.Text = Income("YEAR(h.NgayHD) = @year AND MONTH(h.NgayHD) = @month").ToString();
		}

		private void ShowIncome_Quarter()
		{
			tbx_TongTrongQuy.Text = Income("YEAR(h.NgayHD) = @year AND (MONTH(h.NgayHD) + 2) / 3 = @quarter").ToString();
		}

		private void ShowIncome_Year()
		{
			tbx_TongTrongNam.Text = Income("YEAR(h.NgayHD) = @year").ToString();
		}

		void ShowIncome_Total()
		{
			tbx_TongThuNhap.Text = Income("1 = 1").ToString();
		}

		long Income(string period)
		{
			using (var database = new CafeRestaurantEntities())
				return database.Database.SqlQuery<long>(
					@"SELECT COALESCE(SUM(CONVERT(bigint, h.TongTien)), CONVERT(bigint, 0))
					  FROM dbo.HoaDon h WHERE NOT EXISTS
					  (SELECT 1 FROM dbo.HoanTienHoaDon r WHERE r.MaHD = h.MaHD) AND " + period,
					new SqlParameter("@year", nam), new SqlParameter("@month", thang),
					new SqlParameter("@day", ngay), new SqlParameter("@quarter", quy)).Single();
		}
	}
}
