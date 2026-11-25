using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	internal sealed class Form_Shift : Form
	{
		private readonly User account;
		private readonly Label status;
		private readonly NumericUpDown cash;
		private readonly Button action;
		private ShiftSummary current;

		internal Form_Shift(User account)
		{
			this.account = account;
			Text = "Ca bán hàng";
			StartPosition = FormStartPosition.CenterParent;
			ClientSize = new Size(440, 235);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			status = new Label { Left = 18, Top = 18, Width = 404, Height = 115 };
			var cashLabel = new Label { Left = 18, Top = 145, Width = 145, Text = "Tiền thực tế (đ):" };
			cash = new NumericUpDown { Left = 165, Top = 140, Width = 240, Maximum = 1000000000000, ThousandsSeparator = true };
			action = new Button { Left = 165, Top = 180, Width = 140, Height = 32 };
			action.Click += ChangeShift;
			Controls.AddRange(new Control[] { status, cashLabel, cash, action });
			Load += (sender, args) => RefreshStatus();
		}

		private void RefreshStatus()
		{
			try
			{
				using (var database = new CafeRestaurantEntities())
					current = ShiftService.Current(database);
				status.Text = current == null ? "Chưa có ca đang mở.\r\nNhập tiền đầu ca để mở ca." :
					$"Ca #{current.MaCa} mở lúc {current.LucMo:dd/MM/yyyy HH:mm}\r\n" +
					$"Tiền đầu ca: {current.TienDauCa:N0} đ\r\n" +
					$"Thu tiền mặt: {current.TienMatBanHang:N0} đ\r\n" +
					$"Hoàn tiền mặt: {current.TienMatHoan:N0} đ\r\n" +
					$"Tiền mặt dự kiến: {current.Expected:N0} đ";
				action.Text = current == null ? "Mở ca" : "Chốt ca";
				cash.Value = 0;
			}
			catch (SqlException ex)
			{
				action.Enabled = false;
				status.Text = "Không thể tải ca: " + ex.Message;
			}
		}

		private void ChangeShift(object sender, EventArgs e)
		{
			long amount = decimal.ToInt64(cash.Value);
			if (MessageBox.Show(current == null ? "Mở ca với số tiền đã nhập?" : "Chốt ca với số tiền kiểm đếm đã nhập?",
				"Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;
			try
			{
				if (current == null)
				{
					ShiftService.Open(account.MaNhanVien, amount);
					MessageBox.Show("Đã mở ca.", "Thông báo");
				}
				else
				{
					var closed = ShiftService.Close(account.MaNhanVien, amount);
					MessageBox.Show($"Đã chốt ca.\r\nDự kiến: {closed.Expected:N0} đ\r\n" +
						$"Thực tế: {amount:N0} đ\r\nChênh lệch: {amount - closed.Expected:N0} đ", "Thông báo");
				}
				RefreshStatus();
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể cập nhật ca: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				RefreshStatus();
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message, "Thông báo");
				RefreshStatus();
			}
		}
	}
}
