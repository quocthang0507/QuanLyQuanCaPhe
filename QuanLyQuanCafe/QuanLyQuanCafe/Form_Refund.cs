using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	internal sealed class Form_Refund : Form
	{
		private readonly string billId;
		private readonly TextBox reason;
		private readonly ComboBox method;

		internal Form_Refund(string billId, string originalMethod)
		{
			this.billId = billId;
			Text = "Hoàn toàn bộ hóa đơn " + billId.Trim();
			StartPosition = FormStartPosition.CenterParent;
			ClientSize = new Size(440, 190);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			var methodLabel = new Label { Left = 15, Top = 20, Width = 140, Text = "Phương thức hoàn:" };
			method = new ComboBox { Left = 160, Top = 15, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
			method.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản" });
			method.SelectedIndex = originalMethod == "CASH" ? 0 : originalMethod == "TRANSFER" ? 1 : -1;
			method.Enabled = originalMethod == null;
			var reasonLabel = new Label { Left = 15, Top = 60, Width = 140, Text = "Lý do:" };
			reason = new TextBox { Left = 160, Top = 55, Width = 250, MaxLength = 256, Multiline = true, Height = 60 };
			var button = new Button { Left = 160, Top = 130, Width = 150, Height = 35, Text = "Hoàn toàn bộ" };
			button.Click += Refund;
			Controls.AddRange(new Control[] { methodLabel, method, reasonLabel, reason, button });
		}

		private void Refund(object sender, EventArgs e)
		{
			if (method.SelectedIndex < 0 || string.IsNullOrWhiteSpace(reason.Text))
			{
				MessageBox.Show("Chọn phương thức và nhập lý do hoàn tiền.", "Thông báo");
				return;
			}
			if (MessageBox.Show("Xác nhận đã hoàn toàn bộ số tiền cho khách? Thao tác này không thể hoàn tác.",
				"Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;
			try
			{
				RefundService.Refund(billId, new User().MaNhanVien,
					method.SelectedIndex == 0 ? "CASH" : "TRANSFER", reason.Text);
				MessageBox.Show("Đã ghi nhận hoàn toàn bộ hóa đơn.", "Thông báo");
				DialogResult = DialogResult.OK;
				Close();
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể hoàn tiền: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message, "Thông báo");
			}
		}
	}
}
