using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	public partial class Form_ChangeInfo : Form
	{
		User account = new User();
		BalloonTooltip tooltip = new BalloonTooltip();

		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public User Account
		{
			get { return account; }
			set { account = value; }
		}

		/// <summary>
		/// Truyền thông tin tài khoản của người dùng đang đăng nhập
		/// </summary>
		/// <param name="user">Tài khoản người dùng</param>
		public Form_ChangeInfo(User user)
		{
			InitializeComponent();
			this.Account = user;
		}

		#region Events

		private void Form_ChangeInfo_Load(object sender, EventArgs e)
		{
			tbx_HoTen.Text = Account.Fullname;
			tbx_CMND.Text = Account.CMND;
			tbx_NgaySinh.Text = Account.Birthday;
			tbx_SoDT.Text = Account.PhoneNumber;
			AddTooltip();
		}

		private void btn_CapNhat_Click(object sender, EventArgs e)
		{
			string hoTen = tbx_HoTen.Text;
			string CMND = tbx_CMND.Text;
			DateTime ngaySinh;
			string soDT = tbx_SoDT.Text;
			string password = tbx_MKCu.Text;
			if (hoTen == "" || CMND == "" || password == "" || !VNDateTime.Initial(tbx_NgaySinh.Text, out ngaySinh) || soDT == "")
			{
				MessageBox.Show("Không được để trống các trường bắt buộc", "Thông báo");
				return;
			}
			if (account.OnlyChangeInfo(password, hoTen, CMND, ngaySinh, soDT))
			{
				MessageBox.Show("Cập nhật thông tin hoàn tất", "Thông báo");
				this.Close();
			}
		}

		private void btn_CapNhatHet_Click(object sender, EventArgs e)
		{
			string hoTen = tbx_HoTen.Text;
			string CMND = tbx_CMND.Text;
			DateTime ngaySinh;
			string soDT = tbx_SoDT.Text;
			string mk_cu = tbx_MKCu.Text;
			string mk_moi = tbx_MKMoi.Text;
			string mk_nhaplai = tbx_MKXacNhan.Text;
			if (hoTen == "" || CMND == "" || !VNDateTime.Initial(tbx_NgaySinh.Text, out ngaySinh) ||
				soDT == "" || mk_cu == "" || mk_moi == "" || mk_nhaplai == "")
			{
				MessageBox.Show("Không được để trống các trường bắt buộc", "Thông báo");
				return;
			}
			if (!mk_moi.Equals(mk_nhaplai))
			{
				MessageBox.Show("Mật khẩu mới và mật khẩu nhập lại không khớp", "Thông báo");
				return;
			}
			if (account.FullChange(mk_cu, mk_moi, hoTen, CMND, ngaySinh, soDT))
			{
				MessageBox.Show("Thay đổi mật khẩu thành công", "Thông báo");
				this.Close();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Thêm tooltip vào 2 nút
		/// </summary>
		void AddTooltip()
		{
			tooltip.SetToopTip(btn_CapNhat, "Cập nhật các thông tin cá nhân, ngoại trừ thay đổi mật khẩu.\r\n" +
				"Vui lòng xác nhận mật khẩu tại trường bắt buộc");
			tooltip.SetToopTip(btn_CapNhatHet, "Cập nhật tất cả các thông tin cá nhân, bao gồm thay đổi mật khẩu");
		}

		#endregion
	}
}
