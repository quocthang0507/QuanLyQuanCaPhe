using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	internal sealed class Form_BillDetail : Form
	{
		private readonly string receipt;

		internal Form_BillDetail(string receipt)
		{
			this.receipt = receipt;
			Text = "Chi tiết hóa đơn";
			StartPosition = FormStartPosition.CenterParent;
			ClientSize = new Size(540, 500);
			MinimumSize = new Size(400, 300);
			var content = new TextBox
			{
				Dock = DockStyle.Fill,
				Multiline = true,
				ReadOnly = true,
				ScrollBars = ScrollBars.Both,
				WordWrap = false,
				Font = new Font("Segoe UI", 10),
				Text = receipt
			};
			var footer = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(6) };
			var print = new Button { Text = "In hóa đơn", Width = 130, Height = 32 };
			var close = new Button { Text = "Đóng", Width = 100, Height = 32 };
			print.Click += Print;
			close.Click += (sender, args) => Close();
			footer.Controls.Add(print);
			footer.Controls.Add(close);
			Controls.Add(content);
			Controls.Add(footer);
		}

		private void Print(object sender, EventArgs e)
		{
			try
			{
				BillReceipt.Print(this, receipt);
			}
			catch (InvalidPrinterException ex)
			{
				MessageBox.Show("Không thể in hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Win32Exception ex)
			{
				MessageBox.Show("Không thể in hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (ExternalException ex)
			{
				MessageBox.Show("Không thể in hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (IOException ex)
			{
				MessageBox.Show("Không thể in hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show("Không thể in hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
