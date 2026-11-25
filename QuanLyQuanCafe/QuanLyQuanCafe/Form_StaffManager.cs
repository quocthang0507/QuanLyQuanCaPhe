using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity.Infrastructure;

namespace QuanLyQuanCafe
{
	public partial class Form_StaffManager : Form
	{
		CafeRestaurantEntities database = new CafeRestaurantEntities();

		public Form_StaffManager()
		{
			if (!User.Type)
				InitializeComponent();
			else this.Close();
		}

		#region Events
		private void Form_StaffManager_Load(object sender, EventArgs e)
		{
			exportTable();
			AddDataBinding();
			AddTooltip();
		}

		private void btn_Them_Click(object sender, EventArgs e)
		{
			string tenDangNhap = tbx_TenDangNhap.Text;
			string hoTen = tbx_HoTen.Text;
			string CMND = tbx_CMND.Text;
			string soDT = tbx_SoDT.Text;
			bool conLam = chk_ConLam.Checked;
			DateTime ngaySinh = DateTime.Now;
			if (tenDangNhap == "" || hoTen == "" || CMND == "" || !VNDateTime.Initial(tbx_NgaySinh.Text, out ngaySinh) || soDT == "")
			{
				MessageBox.Show("Không được để trống các trường bắt buộc hoặc nhập sai ngày", "Thông báo");
				return;
			}
			string matKhau = Bcrypt.HashPassword(tenDangNhap);
			TaiKhoan staff = new TaiKhoan { TenDangNhap = tenDangNhap, HoTen = hoTen, MatKhau = matKhau, LoaiTaiKhoan = true, CMND = CMND, NgaySinh = ngaySinh, SoDT = soDT, ConLam = conLam };
			database.TaiKhoan.Add(staff);
			database.SaveChanges();
			btn_Xuat.PerformClick();
		}

		private void btn_Xoa_Click(object sender, EventArgs e)
		{
			string tenDangNhap = tbx_TenDangNhap.Text;
			if (tenDangNhap == "")
			{
				MessageBox.Show("Không được để trống trường bắt buộc", "Thông báo");
				return;
			}
			else if (tenDangNhap.Equals("admin"))
			{
				MessageBox.Show("Bạn không thể xóa tài khoản này được", "Thông báo");
				return;
			}
			TaiKhoan staff = database.TaiKhoan.Where(s => s.TenDangNhap.Equals(tenDangNhap)).SingleOrDefault();
			if (staff == null)
			{
				MessageBox.Show("Không tìm thấy nhân viên.", "Thông báo");
				return;
			}
			database.TaiKhoan.Remove(staff);
			try
			{
				database.SaveChanges();
			}
			catch (DbUpdateException ex)
			{
				database.Entry(staff).State = System.Data.Entity.EntityState.Unchanged;
				MessageBox.Show("Không thể xóa nhân viên có lịch sử hoạt động; hãy khóa đăng nhập thay thế: " +
					ex.GetBaseException().Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			btn_Xuat.PerformClick();
		}

		private void btn_CapNhatTrangThai_Click(object sender, EventArgs e)
		{
			if (User.Type)
			{
				MessageBox.Show("Chỉ người quản trị mới được thay đổi trạng thái nhân viên.", "Thông báo");
				return;
			}
			string tenDangNhap = tbx_TenDangNhap.Text.Trim();
			if (tenDangNhap == "")
			{
				MessageBox.Show("Vui lòng chọn nhân viên.", "Thông báo");
				return;
			}
			TaiKhoan staff = database.TaiKhoan.SingleOrDefault(s => s.TenDangNhap == tenDangNhap);
			if (staff == null || staff.LoaiTaiKhoan != true)
			{
				MessageBox.Show("Không tìm thấy nhân viên hoặc tài khoản không phải nhân viên.", "Thông báo");
				return;
			}
			if (MessageBox.Show(chk_ConLam.Checked ? "Cho phép nhân viên đăng nhập lại?" : "Khóa đăng nhập của nhân viên này?",
				"Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;
			bool? previousStatus = staff.ConLam;
			staff.ConLam = chk_ConLam.Checked;
			try
			{
				database.SaveChanges();
				btn_Xuat.PerformClick();
			}
			catch (DbUpdateException ex)
			{
				staff.ConLam = previousStatus;
				MessageBox.Show("Không thể cập nhật trạng thái nhân viên: " + ex.GetBaseException().Message, "Lỗi",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btn_Xuat_Click(object sender, EventArgs e)
		{
			exportTable();
			AddDataBinding();
		}

		private void btn_TimTheoTen_Click(object sender, EventArgs e)
		{
			string hoTen = tbx_HoTen.Text;
			if (hoTen != "")
			{
				var table = from data in database.TaiKhoan
							where data.HoTen.Contains(hoTen)
							select new
							{
								TênĐăngNhập = data.TenDangNhap,
								HọTên = data.HoTen,
								CMND = data.CMND,
								NgàySinh = data.NgaySinh,
								SốĐiệnThoại = data.SoDT,
								CònLàm = data.ConLam
							};
				grid_Staff.DataSource = table.ToList();
				AddDataBinding();
			}
		}

		private void btn_TimTheoDT_Click(object sender, EventArgs e)
		{
			string soDT = tbx_SoDT.Text;
			if (soDT != "")
			{
				var table = from data in database.TaiKhoan
							where data.SoDT.Contains(soDT)
							select new
							{
								TênĐăngNhập = data.TenDangNhap,
								HọTên = data.HoTen,
								CMND = data.CMND,
								NgàySinh = data.NgaySinh,
								SốĐiệnThoại = data.SoDT,
								CònLàm = data.ConLam
							};
				grid_Staff.DataSource = table.ToList();
				AddDataBinding();
			}
		}

		private void btn_TimTheoConLam_Click(object sender, EventArgs e)
		{
			bool conLam = chk_ConLam.Checked;
			var table = from data in database.TaiKhoan
						where data.ConLam == conLam
						select new
						{
							TênĐăngNhập = data.TenDangNhap,
							HọTên = data.HoTen,
							CMND = data.CMND,
							NgàySinh = data.NgaySinh,
							SốĐiệnThoại = data.SoDT,
							CònLàm = data.ConLam
						};
			grid_Staff.DataSource = table.ToList();
			AddDataBinding();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Xuất bảng TaiKhoan vào DataGridVew
		/// </summary>
		void exportTable()
		{
			var table = from data in database.TaiKhoan
						select new
						{
							TênĐăngNhập = data.TenDangNhap,
							HọTên = data.HoTen,
							CMND = data.CMND,
							NgàySinh = data.NgaySinh,
							SốĐiệnThoại = data.SoDT,
							CònLàm = data.ConLam
						};
			grid_Staff.DataSource = table.ToList();
		}

		/// <summary>
		/// Xóa các DataBinding
		/// </summary>
		void ResetDatabinding()
		{
			tbx_TenDangNhap.DataBindings.Clear();
			tbx_HoTen.DataBindings.Clear();
			tbx_CMND.DataBindings.Clear();
			tbx_NgaySinh.DataBindings.Clear();
			tbx_SoDT.DataBindings.Clear();
			chk_ConLam.DataBindings.Clear();
		}

		/// <summary>
		/// Thêm các DataBinding
		/// </summary>
		void AddDataBinding()
		{
			ResetDatabinding();
			tbx_TenDangNhap.DataBindings.Add(new Binding("Text", grid_Staff.DataSource, "TênĐăngNhập", true, DataSourceUpdateMode.OnValidation));
			tbx_HoTen.DataBindings.Add(new Binding("Text", grid_Staff.DataSource, "HọTên", true, DataSourceUpdateMode.OnValidation));
			tbx_CMND.DataBindings.Add(new Binding("Text", grid_Staff.DataSource, "CMND", true, DataSourceUpdateMode.OnValidation));
			tbx_NgaySinh.DataBindings.Add(new Binding("Text", grid_Staff.DataSource, "NgàySinh", true, DataSourceUpdateMode.OnValidation, "", "dd/MM/yyyy"));
			tbx_SoDT.DataBindings.Add(new Binding("Text", grid_Staff.DataSource, "SốĐiệnThoại", true, DataSourceUpdateMode.OnValidation));
			chk_ConLam.DataBindings.Add(new Binding("Checked", grid_Staff.DataSource, "CònLàm", true, DataSourceUpdateMode.Never));
		}

		/// <summary>
		/// Thêm tooltip vào các button
		/// </summary>
		void AddTooltip()
		{
			BalloonTooltip tip = new BalloonTooltip(10000);
			tip.SetToopTip(btn_Them, "Thêm một nhân viên mới vào danh sách nhân viên\r\n" +
				"Mật khẩu mặc định chính là tên đăng nhập, vì vậy yêu cầu nhân viên sau khi đăng nhập thành công đổi mật khẩu ngay");
			tip.SetToopTip(btn_Xoa, "Xóa một nhân viên khỏi danh sách dựa vào tên đăng nhập\r\n" +
				"Lưu ý: Nhân viên nào đã từng hoạt động (có lưu ở hóa đơn) thì không xóa được");
			tip.SetToopTip(btn_CapNhatTrangThai, "Chọn hoặc bỏ chọn Còn làm, sau đó lưu để mở hoặc khóa đăng nhập");
			tip.SetToopTip(btn_TimTheoConLam, "Lọc danh sách nhân viên theo tình trạng làm việc");
			tip.SetToopTip(btn_TimTheoDT, "Tìm nhân viên theo số điện thoại tương đối");
			tip.SetToopTip(btn_TimTheoTen, "Tìm nhân viên theo họ tên tương đối");
		}

		#endregion

	}
}
