using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace QuanLyQuanCafe
{
	public partial class Form_DrinkManager : Form
	{
		CafeRestaurantEntities database = new CafeRestaurantEntities();

		public Form_DrinkManager()
		{
			if (!User.Type)
				InitializeComponent();
			else this.Close();
		}

		#region Events

		private void From_DrinkManager_Load(object sender, EventArgs e)
		{
			exportTable();
			AddDataBinding();
			AddTooltip();
		}

		private void btn_TimTheoTen_Click(object sender, EventArgs e)
		{
			string ten = tbx_TenThucUong.Text;
			if (ten != "")
			{
				var table = from data in database.ThucUong
							where data.TenThucUong.Contains(ten)
							select new
							{
								MãThứcUống = data.MaThucUong,
								MãLoại = data.MaLoai,
								TênThứcUống = data.TenThucUong,
								ĐơnGiá = data.DonGia,
								KhảDụng = data.KhaDung
							};
				grid_Drink.DataSource = table.ToList();
				AddDataBinding();
			}
		}

		private void btn_TimTheoGia_Click(object sender, EventArgs e)
		{
			decimal gia;
			if (decimal.TryParse(tbx_DonGia.Text, out gia))
			{
				var table = from data in database.ThucUong
							where data.DonGia >= gia
							select new
							{
								MãThứcUống = data.MaThucUong,
								MãLoại = data.MaLoai,
								TênThứcUống = data.TenThucUong,
								ĐơnGiá = data.DonGia,
								KhảDụng = data.KhaDung
							};
				grid_Drink.DataSource = table.ToList();
				AddDataBinding();
			}
		}

		private void btn_KhaDung_Click(object sender, EventArgs e)
		{
			bool khaDung = chk_KhaDung.Checked;
			var table = from data in database.ThucUong
						where data.KhaDung == khaDung
						select new
						{
							MãThứcUống = data.MaThucUong,
							MãLoại = data.MaLoai,
							TênThứcUống = data.TenThucUong,
							ĐơnGiá = data.DonGia,
							KhảDụng = data.KhaDung
						};
			grid_Drink.DataSource = table.ToList();
			AddDataBinding();
		}

		private void btn_Them_Click(object sender, EventArgs e)
		{
			string maThucUong = tbx_MaThucUong.Text;
			string maLoai = tbx_MaLoai.Text;
			string tenThucUong = tbx_TenThucUong.Text;
			decimal donGia;
			if (maThucUong == "" || maLoai == "" || tenThucUong == "" || !decimal.TryParse(tbx_DonGia.Text, out donGia))
			{
				MessageBox.Show("Không được để trống các trường bắt buộc", "Thông báo");
				return;
			}
			ThucUong item = new ThucUong { MaThucUong = maThucUong, MaLoai = maLoai, TenThucUong = tenThucUong, DonGia = donGia, KhaDung = chk_KhaDung.Checked };
			database.ThucUong.Add(item);
			database.SaveChanges();
			btn_Xuat.PerformClick();
		}

		private void btn_Xoa_Click(object sender, EventArgs e)
		{
			string maThucUong = tbx_MaThucUong.Text;
			string maLoai = tbx_MaLoai.Text;
			if (maLoai == "" || maThucUong == "")
			{
				MessageBox.Show("Không được để trống các trường bắt buộc", "Thông báo");
				return;
			}
			ThucUong item = database.ThucUong.Where(i => i.MaThucUong.Equals(maThucUong) && i.MaLoai.Equals(maLoai)).SingleOrDefault();
			if (item == null)
			{
				MessageBox.Show("Không tìm thấy thức uống.", "Thông báo");
				return;
			}
			database.ThucUong.Remove(item);
			try
			{
				database.SaveChanges();
			}
			catch (DbUpdateException ex)
			{
				database.Entry(item).State = EntityState.Unchanged;
				MessageBox.Show("Không thể xóa thức uống có trong đơn hoặc hóa đơn: " + ex.GetBaseException().Message,
					"Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			btn_Xuat.PerformClick();
		}

		private void btn_Sua_Click(object sender, EventArgs e)
		{
			string maThucUong = tbx_MaThucUong.Text;
			string maLoai = tbx_MaLoai.Text;
			string tenThucUong = tbx_TenThucUong.Text;
			decimal donGia;
			bool khaDung = chk_KhaDung.Checked;
			if (maLoai == "" || maThucUong == "" || tenThucUong == "" || !decimal.TryParse(tbx_DonGia.Text, out donGia))
			{
				MessageBox.Show("Không được để trống các trường bắt buộc", "Thông báo");
				return;
			}
			ThucUong item = database.ThucUong.Where(i => i.MaThucUong.Equals(maThucUong) && i.MaLoai.Equals(maLoai)).SingleOrDefault();
			item.TenThucUong = tenThucUong;
			item.KhaDung = khaDung;
			item.DonGia = donGia;
			database.SaveChanges();
			btn_Xuat.PerformClick();
		}

		private void btn_Xuat_Click(object sender, EventArgs e)
		{
			exportTable();
			AddDataBinding();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Xuất bảng ThucUong vào DataGridView
		/// </summary>
		void exportTable()
		{
			var table = from data in database.ThucUong
						select new
						{
							MãThứcUống = data.MaThucUong,
							MãLoại = data.MaLoai,
							TênThứcUống = data.TenThucUong,
							ĐơnGiá = data.DonGia,
							KhảDụng = data.KhaDung
						};
			grid_Drink.DataSource = table.ToList();
		}

		/// <summary>
		/// Xóa các DataBindings
		/// </summary>
		void ResetDatabinding()
		{
			tbx_DonGia.DataBindings.Clear();
			tbx_MaLoai.DataBindings.Clear();
			tbx_MaThucUong.DataBindings.Clear();
			tbx_TenThucUong.DataBindings.Clear();
			chk_KhaDung.DataBindings.Clear();
		}

		/// <summary>
		/// Thêm các DataBinding
		/// </summary>
		void AddDataBinding()
		{
			ResetDatabinding();
			tbx_MaThucUong.DataBindings.Add(new Binding("Text", grid_Drink.DataSource, "MãThứcUống", true, DataSourceUpdateMode.Never));
			tbx_MaLoai.DataBindings.Add(new Binding("Text", grid_Drink.DataSource, "MãLoại", true, DataSourceUpdateMode.Never));
			tbx_TenThucUong.DataBindings.Add(new Binding("Text", grid_Drink.DataSource, "TênThứcUống", true, DataSourceUpdateMode.Never));
			tbx_DonGia.DataBindings.Add(new Binding("Text", grid_Drink.DataSource, "ĐơnGiá", true, DataSourceUpdateMode.Never));
			chk_KhaDung.DataBindings.Add(new Binding("Checked", grid_Drink.DataSource, "KhảDụng", true, DataSourceUpdateMode.Never));
		}

		/// <summary>
		/// Thêm tooltip vào các button
		/// </summary>
		void AddTooltip()
		{
			BalloonTooltip tip = new BalloonTooltip(10000);
			tip.SetToopTip(btn_Them, "Thêm một thức uống mới vào danh mục thức uống");
			tip.SetToopTip(btn_Xoa, "Xóa một thức uống khỏi danh mục dựa vào mã thức uống\r\n" +
				"Lưu ý: Thức uống nào có lưu ở đơn hàng thì không xóa được");
			tip.SetToopTip(btn_Sua, "Thay đổi thông tin của thức uống\r\n" +
				"Bạn chỉ có thể thay đổi ngoại trừ mã thức uống và mã loại");
			tip.SetToopTip(btn_TimTheoTen, "Tìm thức uống theo tên tương đối");
			tip.SetToopTip(btn_TimTheoGia, "Tìm thức uống có giá lớn hơn hoặc bằng tương đối");
			tip.SetToopTip(btn_KhaDung, "Lọc danh mục theo tình trạng khả dụng");
		}

		#endregion
	}
}
