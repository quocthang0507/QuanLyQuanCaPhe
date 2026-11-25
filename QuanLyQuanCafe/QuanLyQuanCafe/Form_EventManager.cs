using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	public partial class Form_EventManager : Form
	{
		CafeRestaurantEntities database = new CafeRestaurantEntities();

		public Form_EventManager()
		{
			if (!User.Type)
				InitializeComponent();
			else this.Close();
		}

		#region Events

		private void Form_EventManager_Load(object sender, EventArgs e)
		{
			exportTable();
			AddDataBinding();
			AddTooltip();
		}

		private void btn_Them_Click(object sender, EventArgs e)
		{
			string maSK = tbx_MaSuKien.Text;
			string tenSK = tbx_TenSuKien.Text;
			byte tyLe = 0;
			short soLuong = 0;
			DateTime batDau = DateTime.Today, ketThuc = DateTime.Today;
			if (maSK == "" || tenSK == "" || !Byte.TryParse(tbx_TyLe.Text, out tyLe) || !short.TryParse(tbx_SoLuong.Text, out soLuong) ||
				!VNDateTime.Initial(tbx_NgayBD.Text, out batDau) || !VNDateTime.Initial(tbx_NgayKT.Text, out ketThuc))
			{
				MessageBox.Show("Không được để trống trường bắt buộc", "Thông báo");
				return;
			}
			if (maSK == "NONAME" || tyLe > 100 || soLuong < 0 || batDau > ketThuc)
			{
				MessageBox.Show("Mã, tỷ lệ giảm, số lượng hoặc thời gian khuyến mãi không hợp lệ", "Thông báo");
				return;
			}
			SuKien item = new SuKien { MaSuKien = maSK, TenSuKien = tenSK, TyLeGiam = tyLe, SoLuong = soLuong, ThoiGian_BD = batDau, ThoiGian_KT = ketThuc };
			database.SuKien.Add(item);
			database.SaveChanges();
			btn_Xuat.PerformClick();
		}

		private void btn_Xoa_Click(object sender, EventArgs e)
		{
			string maSK = tbx_MaSuKien.Text;
			if (maSK == "" || maSK == "NONAME")
			{
				MessageBox.Show("Không được để trống trường bắt buộc", "Thông báo");
				return;
			}
			SuKien item = database.SuKien.Where(i => i.MaSuKien.Equals(maSK)).SingleOrDefault();
			if (item == null)
			{
				MessageBox.Show("Không tìm thấy sự kiện", "Thông báo");
				return;
			}
			database.SuKien.Remove(item);
			database.SaveChanges();
			btn_Xuat.PerformClick();
		}

		private void btn_Sua_Click(object sender, EventArgs e)
		{
			string maSK = tbx_MaSuKien.Text;
			string tenSK = tbx_TenSuKien.Text;
			byte tyLe = 0;
			short soLuong = 0;
			DateTime batDau = DateTime.Today, ketThuc = DateTime.Today;
			if (maSK == "" || tenSK == "" || !Byte.TryParse(tbx_TyLe.Text, out tyLe) || !short.TryParse(tbx_SoLuong.Text, out soLuong) ||
				!VNDateTime.Initial(tbx_NgayBD.Text, out batDau) || !VNDateTime.Initial(tbx_NgayKT.Text, out ketThuc))
			{
				MessageBox.Show("Không được để trống trường bắt buộc", "Thông báo");
				return;
			}
			if (maSK == "NONAME" || tyLe > 100 || soLuong < 0 || batDau > ketThuc)
			{
				MessageBox.Show("Mã, tỷ lệ giảm, số lượng hoặc thời gian khuyến mãi không hợp lệ", "Thông báo");
				return;
			}
			SuKien item = database.SuKien.Where(i => i.MaSuKien.Equals(maSK)).SingleOrDefault();
			if (item == null)
			{
				MessageBox.Show("Không tìm thấy sự kiện", "Thông báo");
				return;
			}
			item.TenSuKien = tenSK;
			item.TyLeGiam = tyLe;
			item.SoLuong = soLuong;
			item.ThoiGian_BD = batDau;
			item.ThoiGian_KT = ketThuc;
			database.SaveChanges();
			btn_Xuat.PerformClick();
		}

		private void btn_TimTheoMa_Click(object sender, EventArgs e)
		{
			string maSuKien = tbx_MaSuKien.Text;
			if (maSuKien == "")
			{
				MessageBox.Show("Không được để trống trường bắt buộc", "Thông báo");
				return;
			}
			var table = from data in database.SuKien
						where data.MaSuKien.Contains(maSuKien)
						select new
						{
							MãSựKiện = data.MaSuKien,
							TênSựKiện = data.TenSuKien,
							TỷLệGiảm = data.TyLeGiam,
							SốLượng = data.SoLuong,
							NgàyBắtĐầu = data.ThoiGian_BD,
							NgàyKếtThúc = data.ThoiGian_KT
						};
			grid_Event.DataSource = table.ToList();
			AddDataBinding();
		}

		private void btn_TimTheoTL_Click(object sender, EventArgs e)
		{
			byte tyLe = 0;
			if (!Byte.TryParse(tbx_TyLe.Text, out tyLe))
			{
				MessageBox.Show("Nhập tỷ lệ không đúng", "Thông báo");
				return;
			}
			var table = from data in database.SuKien
						where data.TyLeGiam.Equals(tyLe)
						select new
						{
							MãSựKiện = data.MaSuKien,
							TênSựKiện = data.TenSuKien,
							TỷLệGiảm = data.TyLeGiam,
							SốLượng = data.SoLuong,
							NgàyBắtĐầu = data.ThoiGian_BD,
							NgàyKếtThúc = data.ThoiGian_KT
						};
			grid_Event.DataSource = table.ToList();
			AddDataBinding();
		}


		private void btn_KhaDung_Click(object sender, EventArgs e)
		{
			var table = from data in database.SuKien
						where DateTime.Compare((DateTime)data.ThoiGian_KT, DateTime.Today) > 0
						select new
						{
							MãSựKiện = data.MaSuKien,
							TênSựKiện = data.TenSuKien,
							TỷLệGiảm = data.TyLeGiam,
							SốLượng = data.SoLuong,
							NgàyBắtĐầu = data.ThoiGian_BD,
							NgàyKếtThúc = data.ThoiGian_KT
						};
			grid_Event.DataSource = table.ToList();
			AddDataBinding();
		}

		private void btn_Xuat_Click(object sender, EventArgs e)
		{
			exportTable();
			AddDataBinding();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Xuất bảng SuKien vào DataGridView
		/// </summary>
		void exportTable()
		{
			var table = from data in database.SuKien
						select new
						{
							MãSựKiện = data.MaSuKien,
							TênSựKiện = data.TenSuKien,
							TỷLệGiảm = data.TyLeGiam,
							SốLượng = data.SoLuong,
							NgàyBắtĐầu = data.ThoiGian_BD,
							NgàyKếtThúc = data.ThoiGian_KT
						};
			grid_Event.DataSource = table.ToList();
		}

		/// <summary>
		/// Xóa các DataBinding
		/// </summary>
		void ResetDatabinding()
		{
			tbx_MaSuKien.DataBindings.Clear();
			tbx_NgayBD.DataBindings.Clear();
			tbx_NgayKT.DataBindings.Clear();
			tbx_SoLuong.DataBindings.Clear();
			tbx_TenSuKien.DataBindings.Clear();
			tbx_TyLe.DataBindings.Clear();
		}

		/// <summary>
		/// Thêm các DataBinding
		/// </summary>
		void AddDataBinding()
		{
			ResetDatabinding();
			tbx_MaSuKien.DataBindings.Add(new Binding("Text", grid_Event.DataSource, "MãSựKiện", true, DataSourceUpdateMode.OnValidation));
			tbx_TenSuKien.DataBindings.Add(new Binding("Text", grid_Event.DataSource, "TênSựKiện", true, DataSourceUpdateMode.OnValidation));
			tbx_TyLe.DataBindings.Add(new Binding("Text", grid_Event.DataSource, "TỷLệGiảm", true, DataSourceUpdateMode.OnValidation));
			tbx_SoLuong.DataBindings.Add(new Binding("Text", grid_Event.DataSource, "SốLượng", true, DataSourceUpdateMode.OnValidation));
			tbx_NgayBD.DataBindings.Add(new Binding("Text", grid_Event.DataSource, "NgàyBắtĐầu", true, DataSourceUpdateMode.OnValidation, "", "dd/MM/yyyy"));
			tbx_NgayKT.DataBindings.Add(new Binding("Text", grid_Event.DataSource, "NgàyKếtThúc", true, DataSourceUpdateMode.OnValidation, "", "dd/MM/yyyy"));
		}

		/// <summary>
		/// Thêm tooltip vào các button
		/// </summary>
		void AddTooltip()
		{
			BalloonTooltip tip = new BalloonTooltip();
			tip.SetToopTip(btn_Them, "Thêm một sự kiện mới vào danh sách sự kiện khuyến mãi");
			tip.SetToopTip(btn_Xoa, "Xóa một sự kiện khỏi danh sách sự kiện khuyến mãi dựa vào mã sự kiện\r\n" +
				"Nếu sự kiện đó đã được áp dụng ít nhất một lần thì không thể xóa");
			tip.SetToopTip(btn_Sua, "Thay đổi thông tin của một sự kiện, mã sự kiện không được thay đổi");
			tip.SetToopTip(btn_TimTheoMa, "Tìm sự kiện theo mã khuyến mãi tương đối");
			tip.SetToopTip(btn_TimTheoTL, "Tìm sự kiện theo tỷ lệ giảm");
			tip.SetToopTip(btn_KhaDung, "Tìm những sự kiện còn khả dụng");
		}

		#endregion
	}
}
