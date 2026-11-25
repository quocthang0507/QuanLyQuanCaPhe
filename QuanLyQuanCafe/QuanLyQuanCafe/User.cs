using System;
using System.Linq;
using System.Windows;

namespace QuanLyQuanCafe
{
	/// <summary>
	/// Quản lý tài khoản người dùng
	/// </summary>
	public class User
	{
		static TaiKhoan account = new TaiKhoan();

		public User()
		{

		}

		/// <summary>
		/// Lấy mã nhân viên (tên đăng nhập)
		/// </summary>
		public string MaNhanVien
		{
			get { return account.TenDangNhap; }
		}

		/// <summary>
		/// Kiểm tra tài khoản là nhân viên
		/// </summary>
		public static bool Type
		{
			get { return (bool)account.LoaiTaiKhoan; }
		}

		/// <summary>
		/// Lấy họ và tên đầy đủ
		/// </summary>
		public string Fullname
		{
			get { return account.HoTen; }
		}

		/// <summary>
		/// Lấy chứng minh nhân dân
		/// </summary>
		public string CMND
		{
			get { return account.CMND; }
		}

		/// <summary>
		/// Lấy ngày sinh dạng dd/MM/yyyy
		/// </summary>
		public string Birthday
		{
			get { return account.NgaySinh.ToString("dd/MM/yyyy"); }
		}

		/// <summary>
		/// Lấy số điện thoại
		/// </summary>
		public string PhoneNumber
		{
			get { return account.SoDT; }
		}

		/// <summary>
		/// Đăng nhập tài khoản vào hệ thống
		/// </summary>
		/// <param name="username">Tên đăng nhập/ Tên tài khoản</param>
		/// <param name="password">Mật khẩu</param>
		/// <returns>True nếu đăng nhập thành công, False nếu không thành công</returns>
		public bool Login(string username, string password)
		{
			using (var database = new CafeRestaurantEntities())
			{
				var list = database.TaiKhoan.Where(x => x.TenDangNhap.Equals(username) &&
					(x.LoaiTaiKhoan == false || x.ConLam == true)).ToList();
				if (list.Count != 0)
				{
					var account = list[0];
					if (Bcrypt.ValidatePassword(password, account.MatKhau))
					{
						User.account = account;
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Cập nhật tất cả các thông tin
		/// </summary>
		/// <param name="oldPassword">Mật khẩu ban đầu</param>
		/// <param name="newPassword">Mật khẩu mới</param>
		/// <param name="hoTen">Họ và tên</param>
		/// <param name="CMND">Chứng minh nhân dân</param>
		/// <param name="ngaySinh">Ngày tháng năm sinh</param>
		/// <param name="soDT">Số điện thoại liên lạc</param>
		/// <returns>True nếu sự thay đổi đó thành công, False nếu không thành công</returns>
		public bool FullChange(string oldPassword, string newPassword, string hoTen, string CMND, DateTime ngaySinh, string soDT)
		{
			if (!Bcrypt.ValidatePassword(oldPassword, account.MatKhau))
			{
				MessageBox.Show("Mật khẩu nhập lại không đúng!", "Thông báo");
				return false;
			}
			account.MatKhau = Bcrypt.HashPassword(newPassword);
			account.HoTen = hoTen;
			account.CMND = CMND;
			account.NgaySinh = ngaySinh;
			account.SoDT = soDT;
			ApplyChange();
			return true;
		}

		/// <summary>
		/// Chỉ cập nhật các thông tin cơ bản, nhưng không bao gồm thay đổi mật khẩu
		/// </summary>
		/// <param name="oldPassword">Mật khẩu xác nhận việc thay đổi</param>
		/// <param name="hoTen">Họ và tên</param>
		/// <param name="CMND">Chứng minh nhân dân</param>
		/// <param name="ngaySinh">Ngày sinh</param>
		/// <param name="soDT">Số điện thoại</param>
		/// <returns>True nếu sự thay đổi đó thành công, False nếu không thành công</returns>
		public bool OnlyChangeInfo(string oldPassword, string hoTen, string CMND, DateTime ngaySinh, string soDT)
		{
			if (!Bcrypt.ValidatePassword(oldPassword, account.MatKhau))
			{
				MessageBox.Show("Mật khẩu xác nhận không đúng!", "Thông báo");
				return false;
			}
			account.HoTen = hoTen;
			account.CMND = CMND;
			account.NgaySinh = ngaySinh;
			account.SoDT = soDT;
			ApplyChange();
			return true;
		}

		/// <summary>
		/// Cập nhật những thay đổi trên tài khoản vào hệ thống
		/// </summary>
		public void ApplyChange()
		{
			using (var database = new CafeRestaurantEntities())
			{
				var oldData = database.TaiKhoan.Find(account.TenDangNhap);
				oldData.MatKhau = account.MatKhau;
				oldData.HoTen = account.HoTen;
				oldData.CMND = account.CMND;
				oldData.NgaySinh = account.NgaySinh;
				oldData.SoDT = account.SoDT;
				database.SaveChanges();
			}
		}
	}
}
